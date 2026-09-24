# Repository Guidelines

## Project Structure & Module Organization

Grodowladcy is a turn-based Android game using Unity **6000.6.2f1**. Open the repository root as a Unity project.

- `Assets/Scripts/Runtime/`: C# gameplay rules, constants, mission resolution, and hero assignment in the `Grodowladcy.Runtime` assembly.
- `Assets/Tests/EditMode/`: NUnit tests in a separate Editor-only assembly.
- `Assets/Scenes/`: game scenes; `Assets/Settings/`: rendering and input assets.
- `Packages/`: Unity dependency manifests; `ProjectSettings/`: shared editor and player configuration.
- `context/foundation/prd.md`: product requirements; neighboring documents capture scope and stack decisions.

## Build, Test, and Development Commands

Install the matching Unity editor through Unity Hub, including Android Build Support and its SDK/NDK tools.

- Test interactively: open **Window > General > Test Runner**, select **EditMode**, and run all tests.
- Test from PowerShell, with `$unity` set to the installed `Unity.exe` path:

```powershell
& $unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults "$env:TEMP/Grodowladcy-tests.xml" -logFile "$env:TEMP/Grodowladcy-tests.log"
```

Close the project's editor before running batch tests. Build Android through **File > Build Profiles**, selecting Android and verifying the included scenes. No custom command-line build script is currently provided.

## Coding Style & Naming Conventions

Keep all code and comments in English, including identifiers, inline comments, and XML documentation comments.

Follow existing C# conventions: four-space indentation, braces on separate lines, PascalCase types and public members, and camelCase parameters and locals. Match filenames to their principal types. Use the `Grodowladcy` namespace for runtime code and keep gameplay rules independent of scene objects where practical. No repository formatter or lint configuration is present.

## Testing Guidelines

Use Unity Test Framework with NUnit. Name test files `<Subject>Tests.cs`; follow descriptive underscore-separated method names such as `Success_When_Hero_Meets_Enemy_Strength`. Cover rule boundaries, failure cases, rewards, and PRD balance values. Run all EditMode tests after gameplay changes. No numeric coverage threshold is configured.

## Commit & Pull Request Guidelines

History uses short descriptive subjects, such as `Add 10x skills and agent data`; no formal commit prefix convention is established. Keep each commit limited to one behavior change and its related tests, assets, and documentation; separate unrelated refactors. PRs should describe changed behavior, reference relevant issues or PRD requirements, and report validation. Include screenshots for visual changes.

## Unity Asset Hygiene

@.gitignore
