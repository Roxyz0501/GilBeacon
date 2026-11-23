# Gil Beacon 1.0.0.5 validation — 2026-11-23

- StandardPluginInfo.cs and StandardPluginTabs.cs match canonical originals byte-for-byte; shared originals untouched.
- External dependency definitions and manifest RequiredPlugins are empty. Host SDK libraries are not plugin dependencies. No IPC dependencies introduced.
- One StandardPluginInfo instance per Plugin/window. Explicit title and callback passed to DrawStandardTrailingTabs. Empty definitions assign null: no Information tab, InstalledPlugins enumeration, install/manage actions, network requests or polling. Existing Settings, Language settings and Support tabs retain ordering and IDs.
- Visibility depends on definition count, never installation status. Future nonempty definitions activate the canonical UI between Settings and Language settings; current zero-dependency branch is hidden.
- Static integration assertions verify empty definition, null branch, call site and byte identity. Clean Release rebuild: zero warnings/errors. Existing regressions: 33,165 passed, including 73 persistence checks. No new glyphs displayed because Information is hidden.
- NPC reader/highlight/disposal section, persistence and row readiness unchanged from prior release. No production migration or installed-DLL replacement.
- Public documents remove reference-only project history while retaining actual reused-code copyright/license notices. Historical internal reports remain intact.
- Package author/version/URLs/allowlist and source/binary secret/local-path scans passed. Live game rendering/lifecycle and physical SSD I/O remain unverified.
