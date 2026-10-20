# Publication state

## 2026-10-20 — Gil Beacon 1.0.0.1 highlight fix

- Release: https://github.com/Roxyz0501/GilBeacon/releases/tag/v1.0.0.1
- ZIP: https://github.com/Roxyz0501/GilBeacon/releases/download/v1.0.0.1/GilBeacon-1.0.0.1.zip
- Source/tag: `b2cdad035818555fe717df673f7f3facc069c875`; ZIP 309,367 bytes; SHA-256 `732848443AA479DFEFB30BDB08D7B6C1E7BDDBDEE1472CD4D127ED69358D128A`.
- Read-only live diagnostics found WaitingForListings=1 despite a ready addon, 35 listings, matching list count, 10 visible renderers and no pending UI/scroll update. Removed this incorrect gate. Matching counts, addon readiness, pending UI/scroll guards and per-listing item identity remain.
- Clean Release: 0 warnings/errors; 20,049 checks passed including reported 100-gil listing versus 217-gil vendor value (112 gil profit after 5 gil tax), readiness and partial-list rejection. Source/package scans passed. Canonical tabs, HQ pricing and right-side labels preserved.
- Public repository/icon HTTP 200; anonymous ZIP hash matches local. Shared index commit `6cd6cea7d683912886876902d7206e98152cf1bc`; pinned index returns 1.0.0.1. Main raw URL initially cached 1.0.0.0.
- Final in-game rendering after update still awaits user confirmation. No game-memory writes, game input, or installed-DLL replacement performed.
- Connector used for source/index/records; CLI only for unsupported Release/assets.


## 2026-10-20 — Gil Beacon 1.0.0.0 public preview

- Published at the user's explicit request; new standalone plugin, author Roxyz0501, Apache-2.0, Dalamud API 15.
- Repository: https://github.com/Roxyz0501/GilBeacon
- Release: https://github.com/Roxyz0501/GilBeacon/releases/tag/v1.0.0.0
- ZIP: https://github.com/Roxyz0501/GilBeacon/releases/download/v1.0.0.0/GilBeacon-1.0.0.0.zip
- Source/tag commit: `5e3171baa963200b17b925c7d322464a6682aeb0`.
- ZIP: 308,973 bytes; SHA-256 `D3CA336A7FA3E49647B568C8F6B99BFF6595727DF6EDDD99BC7DB7C7B97F341C`.
- Green listing highlights and right-side total profit labels after purchase tax; HQ NPC value includes a 10% bonus rounded upward per unit. Materia/special vendor bonuses and mannequin/set sales excluded. No automated purchase/sale or added market requests.
- Seven UI languages, canonical trailing tabs and original 512x512 PNG icon. Localized font/language utilities adapted from Roxyz0501 Bait Beacon with Apache-2.0 notice.
- Clean Release: 0 warnings/errors after fresh NuGet restore; 20,039 offline arithmetic/localization checks passed. Source/package secret and local-path scans passed. Public anonymous repository and PNG returned HTTP 200 (image/png); public ZIP hash matched local.
- Live-game row mapping, sorting/scrolling, DPI, glyph rendering and NPC receipt totals remain unverified. Right-edge label clipping is disclosed.
- Final anonymous verification: both normal main and commit-pinned index URLs returned HTTP 200 with 13 entries and GilBeacon 1.0.0.0; all 12 prior entries were unchanged.
- Shared index commit: `12f98ee8b970f940831651dc87116e4f8d26fe8a`, adding GilBeacon while preserving all 12 prior entries. Normal raw URL initially cached the previous list.
- Source/index/record writes used the GitHub connector. CLI used for unsupported repository creation, binary icon upload and Release/asset operations. No installed plugin DLL replaced.

