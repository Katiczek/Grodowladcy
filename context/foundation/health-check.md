---
project: Grodowładcy
checked_at: 2026-09-24T00:13:24Z
health_status: needs-attention
context_type: brownfield
language_family: dotnet
stack_assessment_available: false
checks_run:
  - lockfile
  - dependency_audit
  - outdated_deps
  - test_runner
  - ci_cd
  - configuration
audit_findings:
  critical: 0
  high: 0
  moderate: 0
  low: 0
test_runner_detected: true
ci_provider: null
recommended_fixes: 5
---

# Health Check — Grodowładcy

## Dependency Health

### Lockfile

```
Status: present (Packages/packages-lock.json)
Package manager: Unity Package Manager (UPM)
```

Unity pins package versions via `Packages/packages-lock.json` alongside `Packages/manifest.json`. NuGet `packages.lock.json` is not used — generated `.csproj` files reference Unity assemblies, not PackageReference NuGet packages.

### Security Audit

```
Tool: dotnet list package --vulnerable --include-transitive
Status: failed to run (not applicable to this project shape)
Reason: Unity-generated projects do not use PackageReference; the CLI reports they use package.config-style references and refuses the audit. UPM packages in Packages/manifest.json are outside NuGet vulnerability databases.
Summary: 0 CRITICAL, 0 HIGH, 0 MODERATE, 0 LOW (no NuGet advisories obtainable)
Direct vs transitive: not distinguished by this tool
```

Recommended external awareness (not a blocker): treat Unity Package Manager updates in the Editor (Window → Package Manager) as the primary dependency hygiene path; there is no built-in equivalent of `npm audit` for UPM in this toolchain.

### Outdated Dependencies

```
Packages with major version gaps: 0 (check skipped)
```

No ecosystem outdated command from the health-check disposition table applies cleanly to Unity/UPM. Direct UPM dependencies are version-pinned in `Packages/manifest.json` (e.g. URP `17.6.0`, Input System `1.20.0`, Test Framework `1.8.0`) against Unity Editor `6000.6.2f1`.

## Test Suite

```
Test runner: Unity Test Framework (com.unity.test-framework 1.8.0)
Tests found: 0 tests
Test execution: not attempted
```

```
Configuration: Packages/manifest.json (package present; no Tests/ asmdef or [Test] scripts under Assets/)
Framework: Unity Test Framework 1.8.0 (NUnit-based EditMode / PlayMode)
```

⚠ Runner package is installed, but there are no test assemblies or test scripts. The agent cannot verify gameplay-rule changes (turn loop, hero accept thresholds, fame/coins) until at least one EditMode assembly exists.

Recommended: create `Assets/Tests/EditMode/` with an assembly definition referencing `UnityEngine.TestRunner` / `UnityEditor.TestRunner`, add NUnit tests for pure C# game rules, then run via Unity Test Runner (Window → General → Test Runner) or `-runTests` batchmode when CI is set up later.

## CI/CD

```
Provider: not detected
Configuration: not found
```

| Stage      | Status | Notes                          |
|------------|--------|--------------------------------|
| Lint       | ✗      | not configured                 |
| Test       | ✗      | not configured                 |
| Build      | ✗      | not configured                 |
| Type check | ✗      | not configured (C# via Unity)  |
| Security   | ✗      | not configured                 |

ℹ No CI/CD configuration detected. You'll set this up in the infrastructure and deployment lesson.
For now, a local test runner is sufficient for agent collaboration.

## Configuration

### High severity

*(none)*

`.gitignore` is present (standard Unity template) — tracked-file exclusions look sound.

### Medium severity

*(none)*

Unity/C# does not use Prettier/ESLint/tsconfig. Formatting is typically EditorConfig + IDE (Rider / Visual Studio) + Unity analyzers already referenced from the Editor install.

### Low severity

- **`.editorconfig`** — consistent indent/encoding across editors and agents. Fix: add a root `.editorconfig` with `root = true`, `indent_style = space`, `indent_size = 4` for `*.cs`, and `charset = utf-8`.
- **`.env.example` / `.env.template`** — not applicable for this local-first Unity Android MVP (no cloud secrets expected per PRD). Skip unless you later add remote services.

## Stack Assessment Cross-Reference

No `stack-assessment.md` found. Run `/10x-stack-assess` for quality-gate analysis.

Companion context present: `context/foundation/prd.md`, `context/foundation/stack-decision.md` (Unity / Android MVP, bootstrapper skipped).

## Recommended Fixes

### Fix before agent work (Category A)

### 1. No executable game tests yet

**Impact**: Unity Test Framework is installed, but with zero tests the agent cannot regress the solo turn loop, hero accept/ignore rules, or win condition (`fame ≥ 20`).
**Severity**: high
**Effort**: moderate (15–30 min)
**Fix**:

1. Create `Assets/Tests/EditMode/EditModeTests.asmdef` with references to `UnityEngine.TestRunner` and `UnityEditor.TestRunner`, `"includePlatforms": ["Editor"]`, and `"optionalUnityReferences": ["TestAssemblies"]`.
2. Add a first test class, e.g. `HeroAssignmentTests.cs`, covering offered coins vs hidden threshold (accept vs ignore) as pure logic.
3. Open **Window → General → Test Runner**, run EditMode tests, confirm green before agent-heavy gameplay work.

### 2. NuGet security audit cannot cover UPM

**Impact**: Agents and humans get a false sense of “audited” if they only run `dotnet list package --vulnerable`. Unity packages are not scanned that way.
**Severity**: medium
**Effort**: quick (< 5 min)
**Fix**:

Document in your working notes: dependency hygiene = review Package Manager updates in Unity Editor for this project; do not expect NuGet audit output. Optionally pin a reminder in future agent instructions (after onboarding lesson).

### 3. Missing `.editorconfig`

**Impact**: Agent-generated C# may disagree with your IDE on whitespace and newlines, causing noisy diffs.
**Severity**: low
**Effort**: quick (< 5 min)
**Fix**:

Create `.editorconfig` at the repo root:

```
root = true

[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true

[*.cs]
indent_style = space
indent_size = 4
```

### Addressed in upcoming lessons (Category B)

### Missing CI/CD pipeline

**Lesson**: [Sprint Zero z Agentem: infrastruktura, walking skeleton i pierwszy deploy (M1L5)](https://platforma.przeprogramowani.pl/external/10xdevs-3/m1-l5)
**What you'll do there**: Configure CI (and later deploy) so lint/test/build run outside the Editor; until then, local Unity Test Runner coverage is what matters for agent collaboration.

### Missing agent instruction files (AGENTS.md)

**Lesson**: [Agent Onboarding: Agents.md, AI Rules i feedback loops (M1L4)](https://platforma.przeprogramowani.pl/external/10xdevs-3/m1-l4)
**What you'll do there**: Build AGENTS.md / AI rules with the right content for this repo. Generating a template now would be premature.

### Missing deployment configuration

**Lesson**: [Sprint Zero z Agentem: infrastruktura, walking skeleton i pierwszy deploy (M1L5)](https://platforma.przeprogramowani.pl/external/10xdevs-3/m1-l5)
**What you'll do there**: Wire Android build/deploy for the MVP target; not required before local agent-assisted gameplay implementation.

## Summary

Health status: needs-attention

Unity 6 (`6000.6.2f1`) project is scaffolded with a UPM lockfile, standard `.gitignore`, and Unity Test Framework already in the manifest — solid base. Gaps that matter for agents: no tests yet under `Assets/`, no CI (expected at this stage), and NuGet audit does not apply to UPM. Address EditMode tests (and optionally `.editorconfig`) before relying on agents for the PRD turn loop.

Next step: add a minimal EditMode test assembly for core game rules, then proceed to agent onboarding (M1L4).
