# Career Log

**Career Log** keeps a **personal raid history** for your SPT profile: where you went, what you looted, who you killed,
and how your stash grew over time. After each raid you get a **RAID MAP** and **RAID DEBRIEF**; from the main menu,
open **RECORDS** for lifetime stats, raid history, and a **Financial** view of stash worth.

**Website:** [softwyx.com](https://softwyx.com)

---

## What you need

- A working **SPT 4.x** install.
- This mod.
- The mod turned **on** in its config (see below).

If you use other mods that change **session end screens**, the **main menu**, or **inventory/stash UI**, mention them
when you report problems — they sometimes interact in odd ways.

---

## Install

1. Download the release ZIP — the filename looks like **`Softwyx.CareerLog-1.0.199.zip`** (the version number changes
   each release).
2. **Extract** it so that your game folder contains:
    - `BepInEx\plugins\Softwyx.CareerLog\Softwyx.CareerLog.dll`
    - `BepInEx\plugins\Softwyx.CareerLog\locales\` (language files)
    - `BepInEx\plugins\Softwyx.CareerLog\assets\` (maps and icons)
    - `BepInEx\plugins\Softwyx.CareerLog\managed\Unity.VectorGraphics.dll` (Unity Vector Graphics library)
3. Copy `Unity.VectorGraphics.dll` from `BepInEx\plugins\Softwyx.CareerLog\managed\` to your game's `EscapeFromTarkov_Data\Managed\` folder.
4. Start the game.

**First launch** may create a config file:

- `BepInEx\config\com.softwyx.careerlog.cfg`

Edit that file while the game is **closed**, or use **Configuration Manager** in-game.

---

## How to use in-game

### After a raid

When you finish a raid (survive, die, or MIA), the mod usually shows:

1. **RAID MAP** — your movement path and event markers (kills, loot picks, injuries, healing, extract/death, and more).
2. **RAID DEBRIEF** — loot summary and session counters for **that raid**.

### Main menu — RECORDS

1. From the **main menu**, click **RECORDS**.
2. Browse **Summary**, **Financial**, and **Raid history** tabs.
3. Select a raid to see detail, open its **map**, and review counters and loot for that run.
4. Use the **PMC / Scav / Combined** filter to switch which runs appear in lists and totals.

Raid data is saved when the raid **ends normally** (extract screen flow). Crashes or force-quits during a raid may not
produce a saved entry for that run (Stash finances are still tracked).

### Financial tab

When enabled, the mod can take a **stash snapshot** each time you return to the main menu (if stash worth changed).
The **Financial** tab shows worth over time, charts, and top valuable items. Snapshots are for your profile only — not
a live flea or trader audit.

---

## Settings

Open **Configuration Manager** in-game, or edit `BepInEx\config\com.softwyx.careerlog.cfg` while the game is **closed**.

**1. General**

- **Enabled** — master switch.
- **Minify JSON files** — slightly smaller save files on disk (files are already small but this will save you that
  little extra bit/byte).

**2. UI**

- **Show post-raid debrief in chain** — RAID MAP + RAID DEBRIEF after a raid.
- **Show Records menu button** — **RECORDS** on the main menu.
- **Records side filter** — default PMC / Scav / Combined filter (Read only. Change it in Records screen).

**3. Movement & maps**

- **Movement sample interval** — how often your position is recorded (lower = smoother trail, more data).
- **Trail simplification epsilon** — how aggressively the saved path is simplified.
- **Optimize movement trail** — extra reduction before saving (off = keep more raw points).
- **Restore vanilla statistics screen** — show vanilla **Session statistics** after the mod debrief.

**4. Event collectors**

Controls what shows as **markers** on the raid map (kills, loot, injuries, etc.):

- **Killstreak** gap and minimum kills.
- **Injury merge window** — combine nearby limb-loss markers in time.
- **Loot marker min value (₽)** — handbook value threshold for loot markers.
- **Loot marker template ids / category tokens** — always mark certain item types (advanced).
- **Healing large restore (%)** — threshold for “big heal” markers.
- **Loot marker merge** — combine nearby loot markers in time and on the map.

**5. Financial**

- **Capture stash snapshot on menu open** — track stash worth when returning to the menu.
- **Financial chart default style** — Line or Bar.
- **Top items exclusions** — hide secure containers or specific item types from “top items” lists (advanced).

Delete the config file with the game **closed** to reset all defaults (or reset manually in Configuration Manager).

Use this SPT [tool](https://db.sp-tarkov.com/) to get the ids you need.

---

## Known issues / quirks

These vary with game version, maps, and other mods.

1. **Map missing for a location** — Some maps may show a message that the image is unavailable; movement and markers may
   still be saved.
2. **Scav runs** — Scav raids are recorded and tagged; they are stored under your **PMC profile** id (for convenience).
3. **Bring-in gear vs looted gear** — Loot stats and markers try to ignore gear you brought into the raid; edge cases
   (stacks, containers, quest items) may still look odd in rare situations.
4. **Other session-end or menu UI mods** — Best-effort compatibility. If something breaks, test with only Career Log +
   SPT essentials.

---

## How to report a bug or ask for help

**Copy/paste this checklist** and fill in what you can:

1. **Mod version** — From BepInEx `LogOutput.log`: the line where **Career Log** loads (look for **`[CLG]`** and
   **v...**).
2. **Game / SPT** — Example: “SPT 4.0.x, EFT client build …”.
3. **What happened** — One short sentence (e.g. “No RECORDS button” / “RAID MAP empty after survive”).
4. **Steps** — Numbered: map, PMC or Scav, survive or die, what you clicked.
5. **Other mods** — Anything touching **UI, session end, inventory, economy, or maps**.
6. **Logs** — With the game **closed**: `BepInEx\LogOutput.log` (and `FullLogOutput.log` if asked).

If the mod you are using is **not signed by me** then it isn't from me and I can't help you with it.

**Where to send it** — Use the **issue tracker or comments** where you **downloaded** the mod. Do not email about mods.

Do **not** send passwords, full Windows usernames, or unrelated personal files.

---

## Third-party dependencies

This mod includes `Unity.VectorGraphics.dll`, which is part of Unity's official Vector Graphics package (com.unity.vectorgraphics). This is an official Unity package and is distributed under Unity's standard license terms. For more information, see [Unity's Vector Graphics documentation](https://docs.unity3d.com/Packages/com.unity.vectorgraphics@2.0/manual/index.html).

---

## Thanks

People across the **SPT** and **modding** community shared ideas, repros, and “what if you tried…” moments that shaped
this plugin.

Map assets bundle community SVG sources with attribution in the mod package; see `assets/maps/ATTRIBUTION.md` in the
plugin folder if you ship or mirror the mod.

---

## Support the author

Maintenance and compatibility work takes time. If Career Log helps you review your wipes and progress, consider
**[supporting on GitHub Sponsors](https://github.com/sponsors/FallegaHQ)**. There is no paywall — everything stays free
under the licence below.

---

## Licence

This project is licensed under the **Mozilla Public License 2.0** (MPL-2.0). See the [`LICENSE`](LICENSE) file for the
full text.

SPDX: `MPL-2.0`

---

## Disclaimer

*Escape from Tarkov* is a trademark of Battlestate Games. This mod is an independent, community-made add-on for **SPT**.
Use at your own risk. The author is not responsible for lost progress, banned accounts on other services, or damage from
third-party tools.
