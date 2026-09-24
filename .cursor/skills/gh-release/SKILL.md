---
name: gh-release
description: >
  Bump Unity Android playtest version (minor), build APK, and publish a GitHub
  Release with gh. Use when the user says /gh-release, "gh release", "wydaj APK",
  or wants to ship Builds/Android/Grodowladcy.apk to GitHub Releases.
disable-model-invocation: true
---

# gh-release

Ship a Unity **Android APK** playtest via **GitHub Releases** for this repo.

## Arguments

`$ARGUMENTS` = **release notes** (required). Example:

```
/gh-release Smoke test solo turn loop for the friend group.
```

If notes are missing, ask once for notes, then continue. Do not invent release notes.

## Workflow

1. **Prereqs**
   - Confirm `gh` works (`gh --version`). If missing from PATH, try `"C:\Program Files\GitHub CLI\gh.exe"` or tell the user to open a new terminal / install GitHub CLI.
   - Confirm `gh auth status` is logged in.
   - Confirm Android APK build entry exists: `@Assets/Editor/CiBuild.cs`.

2. **Read current version**
   - From `@ProjectSettings/ProjectSettings.asset` field `bundleVersion` (e.g. `0.0.1`).

3. **Propose minor bump**
   - Increment the **second** number (MINOR); set PATCH to `0`.
   - Examples: `0.0.1` → `0.1.0`, `0.1.0` → `0.2.0`, `1.2.3` → `1.3.0`.
   - Ask the user (chat is fine): show current + proposed; they accept with empty reply / “ok” / Enter-equivalent, **or** type their own `X.Y.Z`.
   - Strip a leading `v` if they type `v0.2.0`.

4. **Run the release script** (preferred — updates version, builds, publishes)

   From repo root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\.cursor\skills\gh-release\scripts\gh-release.ps1 -Version "<chosen>" -Notes "<notes from arguments>"
```

   - Pass `-Version` with the version the user accepted so the script does not block on `Read-Host` in a non-interactive agent shell.
   - If the user wants to drive the prompt themselves in a terminal, tell them to run the same script **without** `-Version` (Enter accepts the proposal).
   - APK output path: `Builds/Android/Grodowladcy.apk` (Build App Bundle = off via `CiBuild`).

5. **Build mode (automatic)**
   - If `Temp/UnityLockfile` exists → **open Editor**: script writes `Temp/gh-release-build.request`; running Editor watcher in `@Assets/Editor/CiBuild.cs` builds APK and writes `Temp/gh-release-build.result` (`0` = ok). Do **not** start a second Unity.
   - If lockfile missing → **batchmode**: Hub Editor from `ProjectVersion.txt` + `-executeMethod Grodowladcy.Editor.CiBuild.BuildAndroidApk`. Log: `Builds/Android/gh-release-build.log`.
   - In Editor (interactive): menu **Grodowladcy → gh-release...** — prompts for version (proposed minor) + notes, builds APK, optional `gh release`.
   - Build only: **Grodowladcy → Build Android APK only**.

6. **On success**
   - Report tag `vX.Y.Z`, title `Grodowładcy vX.Y.Z`, and the release URL (`gh release view vX.Y.Z --json url -q .url`).
   - Remind: friends open the Release → download `.apk` → allow install from that source.

7. **On failure**
   - Do not invent a fake release. Show Unity Console / `gh-release-build.log` / `gh` error.
   - Open-Editor timeout: ensure scripts recompiled (watcher alive), Unity not frozen in Play Mode.

## Manual equivalent (reference)

```powershell
gh release create vX.Y.Z "D:\Grodowladcy\Builds\Android\Grodowladcy.apk" --title "Grodowładcy vX.Y.Z" --notes "..."
```

## Out of scope

- Google Play / AAB uploads
- iOS
- Changing package name or keystore
