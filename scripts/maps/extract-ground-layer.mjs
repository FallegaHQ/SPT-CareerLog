import {mkdirSync, readdirSync, readFileSync, writeFileSync} from "node:fs";
import {dirname, join} from "node:path";
import {fileURLToPath} from "node:url";
import {DOMParser, XMLSerializer} from "@xmldom/xmldom";

const __dirname = dirname(fileURLToPath(import.meta.url));
const repoRoot = join(__dirname, "..", "..");
const sourceDir = join(repoRoot, "assets", "maps");
const outputDir = join(sourceDir, "debrief");
const configPath = join(__dirname, "ground-layer.config.json");

const SVG_NS = "http://www.w3.org/2000/svg";
const XLINK_NS = "http://www.w3.org/1999/xlink";

const config = JSON.parse(readFileSync(configPath, "utf8"));
const defaults = config.defaults;

const parser = new DOMParser();
const serializer = new XMLSerializer();

function listSourceSvgs() {
    return readdirSync(sourceDir).filter(
        (name) => name.endsWith(".svg") && !name.startsWith(".")
    );
}

function findById(root, id) {
    if (!id) return null;
    const walk = (node) => {
        if (!node || node.nodeType !== 1) return null;
        if (node.getAttribute?.("id") === id) return node;
        for (let child = node.firstChild; child; child = child.nextSibling) {
            const hit = walk(child);
            if (hit) return hit;
        }
        return null;
    };
    return walk(root);
}

function collectHrefIds(node, ids = new Set()) {
    if (!node || node.nodeType !== 1) return ids;

    const href =
        node.getAttribute?.("href") ||
        node.getAttribute?.("xlink:href") ||
        node.getAttributeNS?.(XLINK_NS, "href");
    if (href?.startsWith("#")) ids.add(href.slice(1));

    for (let child = node.firstChild; child; child = child.nextSibling) {
        collectHrefIds(child, ids);
    }
    return ids;
}

function cloneNode(doc, node) {
    return node ? doc.importNode(node, true) : null;
}

function matchesAnyPattern(value, patterns) {
    if (!value || !patterns?.length) return false;
    return patterns.some((p) => new RegExp(p, "i").test(value));
}

function shouldRemoveGroup(node, mapConfig) {
    const id = node.getAttribute?.("id") ?? "";
    const patterns = [
        ...(defaults.removeGroupIdPatterns ?? []),
        ...(mapConfig.removeGroupIdPatterns ?? []),
    ];
    return matchesAnyPattern(id, patterns);
}

function stripClasses(classAttr, tokensToRemove) {
    if (!classAttr) return "";
    const tokens = classAttr.split(/\s+/).filter(Boolean);
    const remove = new Set(tokensToRemove);
    return tokens.filter((t) => !remove.has(t)).join(" ");
}

function cleanupSubtree(node, mapConfig) {
    if (!node || node.nodeType !== 1) return;

    const removeClasses = [
        ...(defaults.removeClassTokens ?? []),
        ...(mapConfig.removeClassTokens ?? []),
    ];

    const toRemove = [];
    for (let child = node.firstChild; child; child = child.nextSibling) {
        if (child.nodeType !== 1) continue;

        const tag = child.localName || child.nodeName;
        if (tag === "g" || tag === "svg") {
            if (shouldRemoveGroup(child, mapConfig)) {
                toRemove.push(child);
                continue;
            }
        }

        cleanupSubtree(child, mapConfig);
    }

    for (const el of toRemove) {
        node.removeChild(el);
    }

    const walkAttrs = (el) => {
        if (el.nodeType !== 1) return;

        const cls = stripClasses(el.getAttribute("class") ?? "", removeClasses);
        if (cls) el.setAttribute("class", cls);
        else el.removeAttribute("class");

        el.removeAttribute("filter");
        const style = el.getAttribute("style");
        if (style) {
            // Keep fills/strokes, strip only filter/drop-shadow related declarations.
            const cleaned = style
                .replace(/(^|;)\s*filter\s*:[^;]+;?/gi, "$1")
                .replace(/(^|;)\s*-\w+-filter\s*:[^;]+;?/gi, "$1")
                .replace(/drop-shadow\s*\([^)]*\)/gi, "")
                .replace(/;{2,}/g, ";")
                .replace(/^\s*;\s*|\s*;\s*$/g, "")
                .trim();

            if (cleaned) el.setAttribute("style", cleaned);
            else el.removeAttribute("style");
        }

        for (let c = el.firstChild; c; c = c.nextSibling) {
            walkAttrs(c);
        }
    };

    walkAttrs(node);
}

function cleanStyleText(text, mapConfig) {
    if (!text) return text;
    let out = text;
    const patterns = [
        ...(defaults.removeStyleRulePatterns ?? []),
        ...(mapConfig.removeStyleRulePatterns ?? []),
    ];
    for (const pattern of patterns) {
        out = out.replace(new RegExp(pattern, "gi"), "");
    }
    return out.replace(/\n\s*\n/g, "\n").trim();
}

function collectStyleNodes(sourceSvg, doc, mapConfig) {
    const styles = [];
    for (let child = sourceSvg.firstChild; child; child = child.nextSibling) {
        if (child.nodeType !== 1) continue;
        if ((child.localName || child.nodeName) !== "style") continue;
        const cloned = cloneNode(doc, child);
        if (cloned?.firstChild?.data != null) {
            cloned.firstChild.data = cleanStyleText(cloned.firstChild.data, mapConfig);
        }
        styles.push(cloned);
    }
    return styles;
}

function buildDefs(doc, sourceSvg, contentRoot, mapConfig) {
    const sourceDefs = findById(sourceSvg, "defs1") || findById(sourceSvg, "defs");
    if (!sourceDefs) return null;

    const needed = collectHrefIds(contentRoot);
    const collected = new Set();
    const defsOut = doc.createElementNS(SVG_NS, "defs");
    defsOut.setAttribute("id", "debrief-defs");

    const addDefById = (id) => {
        if (!id || collected.has(id)) return;
        const el = findById(sourceSvg, id);
        if (!el) return;
        collected.add(id);
        const cloned = cloneNode(doc, el);
        defsOut.appendChild(cloned);
        collectHrefIds(cloned).forEach(addDefById);
    };

    needed.forEach(addDefById);

    return defsOut.childNodes.length > 0 ? defsOut : null;
}

function resolveLayerRoot(doc, sourceSvg, fileName, mapConfig) {
    if (mapConfig.useFullDocument) {
        const wrapper = doc.createElementNS(SVG_NS, "g");
        wrapper.setAttribute("id", "DebriefContent");

        for (let child = sourceSvg.firstChild; child; child = child.nextSibling) {
            if (child.nodeType !== 1) continue;
            const tag = child.localName || child.nodeName;
            if (tag === "defs" || tag === "style") continue;

            const id = child.getAttribute?.("id") ?? "";
            if (matchesAnyPattern(id, defaults.removeRootGroupIds ?? [])) continue;
            if (matchesAnyPattern(id, mapConfig.removeRootGroupIds ?? [])) continue;

            wrapper.appendChild(cloneNode(doc, child));
        }
        return wrapper;
    }

    const candidates = mapConfig.layerId
        ? [mapConfig.layerId]
        : [
            ...(mapConfig.layerIdCandidates ?? []),
            ...(defaults.layerIdCandidates ?? []),
        ];

    for (const id of candidates) {
        const layer = findById(sourceSvg, id);
        if (layer) return layer;
    }

    throw new Error(
        `No ground layer found (tried: ${candidates.join(", ")}). Set "layerId" in ground-layer.config.json.`
    );
}

function buildDebriefSvg(sourceText, fileName, mapConfig) {
    const doc = parser.parseFromString(sourceText, "image/svg+xml");
    const parseError = doc.getElementsByTagName("parsererror")[0];
    if (parseError) {
        throw new Error(`XML parse error: ${parseError.textContent}`);
    }

    const sourceSvg = doc.documentElement;
    const viewBox = sourceSvg.getAttribute("viewBox");
    const width = sourceSvg.getAttribute("width");
    const height = sourceSvg.getAttribute("height");

    const layerRoot = resolveLayerRoot(doc, sourceSvg, fileName, mapConfig);
    const contentClone = cloneNode(doc, layerRoot);
    cleanupSubtree(contentClone, mapConfig);

    const outDoc = parser.parseFromString(
        '<svg xmlns="http://www.w3.org/2000/svg" ></svg>',
        "image/svg+xml"
    );
    const outSvg = outDoc.documentElement;
    if (viewBox) outSvg.setAttribute("viewBox", viewBox);
    if (width) outSvg.setAttribute("width", width);
    if (height) outSvg.setAttribute("height", height);
    outSvg.setAttribute("data-careerlog-source", fileName);

    for (const styleNode of collectStyleNodes(sourceSvg, outDoc, mapConfig)) {
        outSvg.appendChild(styleNode);
    }

    const defs = buildDefs(outDoc, sourceSvg, contentClone, mapConfig);
    if (defs) outSvg.appendChild(defs);

    outSvg.appendChild(outDoc.importNode(contentClone, true));

    const xml = serializer.serializeToString(outDoc);
    return xml.startsWith("<?xml") ? xml : `<?xml version="1.0" encoding="UTF-8"?>\n${xml}`;
}

function main() {
    mkdirSync(outputDir, {recursive: true});

    const files = listSourceSvgs();
    let ok = 0;
    let failed = 0;

    for (const fileName of files) {
        const mapConfig = config.maps?.[fileName] ?? {};
        const sourcePath = join(sourceDir, fileName);
        const outPath = join(outputDir, fileName);

        try {
            const sourceText = readFileSync(sourcePath, "utf8");
            const output = buildDebriefSvg(sourceText, fileName, mapConfig);
            writeFileSync(outPath, output, "utf8");
            console.log(`OK  ${fileName} -> debrief/${fileName}`);
            ok++;
        } catch (err) {
            console.error(`FAIL ${fileName}: ${err.message}`);
            failed++;
        }
    }

    console.log(`\nDone: ${ok} written, ${failed} failed -> ${outputDir}`);
    if (failed > 0) process.exit(1);
}

main();
