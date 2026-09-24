---
project: Grodowładcy
researched_at: 2026-09-24
recommended_platform: GitHub Releases
runner_up: Firebase App Distribution
context_type: mvp
tech_stack:
  language: C#
  framework: Unity 6 (URP 2D)
  runtime: Android player
interview:
  persistent_connections: no
  cost_priority: minimize
  prior_platform_familiarity: none
  geography: single_region
  colocated_managed_services: undecided_na_local_save
---

## Recommendation

**Distribute MVP builds via GitHub Releases on `Katiczek/Grodowladcy`.**

This is not a web PaaS deploy: the game runs on-device with local autosave (@context/foundation/prd.md). “Infrastructure” for this MVP means shipping an installable **APK** to a small friend group. GitHub Releases fit the interview (minimize cost, no cloud DB, one region, repo already on GitHub) and stay agent-friendly via `gh release`.

## Platform Comparison

Classic web candidates (Cloudflare, Vercel, Netlify, Fly.io, Railway, Render) were researched and **hard-filtered out**: none host a Unity Android player as the primary artifact. Scoring below is for *Android build distribution*, not serverless hosting.

| Option | CLI-first | Managed | Agent-readable docs | Stable distribute API | MCP / CI hooks | Fit for local-save Unity MVP |
|---|---|---|---|---|---|---|
| GitHub Releases | Pass (`gh`) | Pass (GitHub hosts assets) | Pass | Pass | Pass (GitHub MCP / Actions) | **Best** — $0, already have remote |
| Firebase App Distribution | Pass | Pass | Pass | Pass | Partial | Strong free tester invites |
| itch.io + butler | Pass | Pass | Pass | Pass | Fail/Partial | Free game page; sideload UX |
| Google Play Internal Testing | Partial (API + Console) | Pass | Pass | Pass | Partial | Best store UX; **$25** one-time Play fee |
| Diawi / Drive / chat attach | Fail/Partial | Pass | N/A | Fail | Fail | Fine for one-offs only |
| App Center | — | — | — | — | — | **Retired** (distribution ~2025-03) — do not use |

Cost weight (interview: minimize) penalizes Play Console registration and any always-on PaaS. Co-located DB preference N/A — Primary persists on device only.

### Shortlisted Platforms

#### 1. GitHub Releases (Recommended)

Attach a signed APK to a tagged release; friends download and install (allow install from that source). Version history and release notes live next to the code. CLI: `gh release create`. Private repo limits downloads to collaborators; public repo makes the APK world-readable — choose visibility deliberately.

#### 2. Firebase App Distribution

Mail/invite flow and App Tester UX; free on Spark. Better when testers should not need GitHub access. Extra Google/Firebase project surface; build expiry (~150 days); CLI token deprecation toward service accounts.

#### 3. itch.io (+ butler)

Game-shaped landing page and `butler push …:android`. Good public/indie framing; still sideloading. Less natural if the only goal is a private friend loop already living on GitHub.

## Anti-Bias Cross-Check: GitHub Releases

### Devil's Advocate — Weaknesses

1. **Sideloading friction** — testers must allow installation outside Play; some phones/OEM policies block this.
2. **Visibility leak** — on a public repo, anyone can download the APK; secrets must never ship inside the binary.
3. **No Play-parity testing** — you will not catch Play-specific packaging (AAB, signing by Play) until you move tracks later.
4. **Asset size / CI** — large Unity APKs stress upload quotas and Actions minutes if you automate poorly.
5. **Manual friend ops** — no “invite by email” product; you share the release URL yourself.

### Pre-Mortem — How This Could Fail

The team treated Releases as “good enough forever.” A public repo leaked an unsigned or debug-signed APK with leftover logging. Friends on locked-down work phones could not sideload, so feedback stalled. When the group finally wanted Play Internal Testing, the debug keystore did not match the upload key, forcing a package-name/signing reset. Automating `gh release` from CI without retaining the release keystore in a secret destroyed the ability to ship updates that Android would accept as the same app. Lesson: Releases are excellent for MVP friend builds — keep a durable release keystore and plan Play as a later track, not a surprise.

### Unknown Unknowns

- Android treats updates as the same app only if **applicationId + signing cert** match across APKs.
- GitHub Release assets are not a CDN product for millions of downloads; fine for a handful of friends, not a store substitute.
- AAB is for Play; friends installing from a link need **APK** (disable “Build App Bundle” for this channel).
- `gh` auth in CI needs a fine-scoped token with `contents: write`; do not paste PATs into the repo.

## Operational Story

- **Preview deploys**: optional prerelease flag on GitHub Releases (`gh release create … --prerelease`) for spicy builds; no separate preview URL product.
- **Secrets**: Unity release keystore + passwords in a password manager / GitHub Actions secrets — never commit keystores. `GH_TOKEN` / `GITHUB_TOKEN` for CLI/Actions only.
- **Rollback**: point friends at the previous Release tag; GitHub keeps prior assets. Device-side: uninstall/reinstall older APK if needed (local saves may not migrate).
- **Approval**: creating a Release that others install is a human call for MVP; an agent may draft notes and run `gh` only with explicit go-ahead and existing secrets.
- **Logs**: build failures stay in Unity Editor / local CI logs; distribution audit is the Release page + `gh release view <tag>`.

## Risk Register

| Risk | Source | Likelihood | Impact | Mitigation |
|---|---|---|---|---|
| Sideload blocked on tester phones | Devil's advocate | M | M | Document OEM “install unknown apps”; fall back to USB/`adb install` for stubborn devices |
| Public APK exposure | Devil's advocate | M | H | Keep repo private for closed playtests, or use a private release-only repo |
| Keystore loss blocks updates | Pre-mortem | L | H | Back up release keystore offline; store passwords in secrets manager |
| Friends need Play-like UX later | Research / Pre-mortem | M | M | Graduate to Play Internal Testing ($25) when sideload friction hurts |
| Large APK upload failures | Unknown unknowns | M | L | Compress/split only if needed; prefer local `gh release upload` over flaky CI first |

## Getting Started

1. In Unity: Android Build Profile → build an **APK** (App Bundle off) signed with a dedicated release keystore you will keep.
2. Tag the commit you built from, e.g. `v0.1.0-android`.
3. Publish: `gh release create v0.1.0-android path\to\Grodowladcy.apk --title "Android playtest 0.1.0" --notes "Solo turn loop smoke build for friends."`
4. Share the Release URL with the friend group; they download the APK and allow install from the browser/file manager.
5. For each new playtest: bump tag, rebuild APK with the **same** keystore, create a new Release (optionally mark older ones as prerelease leftovers).

## Out of Scope

The following were not evaluated as primary needs for this MVP:
- Docker image configuration
- Full CI/CD pipeline setup (optional later: Actions → build APK → `gh release`)
- Production-scale architecture (multi-region, HA, DR)
- Always-on game servers / WebSocket backends (Non-Goal for Primary; local host is Secondary)
