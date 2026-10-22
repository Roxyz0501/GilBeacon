# Publication state

## 2026-10-22 — Gil Beacon 1.0.0.3 persistence safeguards published

- Release: https://github.com/Roxyz0501/GilBeacon/releases/tag/v1.0.0.3
- ZIP: https://github.com/Roxyz0501/GilBeacon/releases/download/v1.0.0.3/GilBeacon-1.0.0.3.zip
- Source/tag: `6fb3085f7d2533f7d1fae44c7b7cfb1478ade3ac`; ZIP 323,868 bytes, eight allowlisted files; SHA-256 `7ED050C6A74F77A053AFF8ED25995EE49A1D585FFE81932917178262D5CCE838`.
- 60-second changed-only settings, logout/Dispose flush, atomic validated replacement, three valid backups, corruption recovery and bounded failure/manual retry. NPC calculations and highlight method unchanged. No transaction history or empty HistoryFiles tab.
- 20,133 checks passed (69 persistence); clean Release zero warnings/errors. Unchanged 8-hour simulation zero writes; 600 edits plus logout/Dispose: three commits, 133 logical payload bytes. Canonical tabs byte-identical and source/package scans passed.
- Public repository/icon HTTP200 (image/png); anonymous ZIP hash matched. Shared index commit `0d9b55163d7ae17ad85061f420346407b95f34bd`: pinned URL and connector verified1.0.0.3, all12 other entries preserved. Normal raw main URL initially cached1.0.0.2.
- Published at the user's explicit request. Connector used for source/index/records; CLI only for unsupported Release/asset operations. No production migration or installed-DLL replacement. Live-game lifecycle/rendering and physical SSD writes remain unverified; see PERSISTENCE_VERIFICATION.md.

- 2026-10-23 completion: normal public main index now returns 1.0.0.3 (HTTP 200). Publication records resumed after the prior approval-review usage-limit interruption.

## 2026-10-20 — Gil Beacon 1.0.0.2 materia pricing and incremental highlights

- Release: https://github.com/Roxyz0501/GilBeacon/releases/tag/v1.0.0.2
- ZIP: https://github.com/Roxyz0501/GilBeacon/releases/download/v1.0.0.2/GilBeacon-1.0.0.2.zip
- Source/tag: `9f551ad18feb10805ca0cd98d94cee6100a95232`; ZIP 311,197 bytes; SHA-256 `7CC94A3B230C1EA0459ABC4FF70B6408344448450294DDA60ABF583B5C4A2B85`.
- Materia item category applies the verified rounded-up 10% resale correction: base144 ->159, so price99 plus tax4 yields56. Ordinary qualityless items remain unchanged. Attached materia and special shop bonuses remain excluded.
- Available local rows sorted by price (maximum100); no whole-list count/update wait. Visible row price and quantity must match before immediate calculation/drawing. Cutoff uses greatest possible quality value to retain profitable later HQ rows. No network requests or database writes; one cached sheet row per item change. Limits are code-derived, not measured live.
- Clean Release zero warnings/errors; 20,064 offline checks passed. Canonical tabs byte-identical; source/package credential and local-path scans passed. Seven languages updated; ECommons layout reference documented.
- Public repository/icon HTTP200, image/png; anonymous ZIP hash matched. Index commit `60bd0d3646ab2677685a68fa4e23142fd00bfa8c`, connector and pinned URL verified1.0.0.2, all12 other entries unchanged. Normal main raw URL initially cached1.0.0.1.
- Updated live-game row mapping/speed remains unverified. No installed plugin binary replaced. Connector used for source/index/records; CLI only for unsupported Release/asset publication under standing user authorization.


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

