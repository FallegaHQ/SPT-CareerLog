# Map SVG pipeline (debrief ground layer)

Reproducible preprocessing for post-raid / RECORDS map backgrounds.

## What it does

From each upstream file in `assets/maps/*.svg` (tarkov-dev sources):

1. Extracts the **ground** layer (`Ground_Level`, `Ground_Floor`, or per-map override).
2. Keeps only `<defs>` entries referenced by `<use>` in that layer.
3. Strips **shadows** (`.shadow` CSS, `filter` attrs), arrows/doors/details groups and
   task styling.
4. Writes `assets/maps/debrief/<same-name>.svg` (shipped with the mod).

Runtime loads debrief SVG when present, otherwise the full source SVG (would not look good in Unity).

## Commands

```bash
cd scripts/maps
npm install
npm run build
```

Re-run after updating upstream SVGs in `assets/maps/`.

## Per-map overrides

Edit `ground-layer.config.json`:

| Key                     | Meaning                                         |
|-------------------------|-------------------------------------------------|
| `layerId`               | Exact `<g id="...">` to extract                 |
| `useFullDocument`       | Clean whole file (e.g. Labyrinth)               |
| `removeGroupIdPatterns` | Extra regexes for `<g id>` to drop              |
| `removeRootGroupIds`    | Top-level groups to drop when `useFullDocument` |

Defaults try `Ground_Level` → `Ground_Floor` → `First_Level`.
