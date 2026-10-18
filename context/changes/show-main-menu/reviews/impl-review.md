# Implementation review: show-main-menu

Reviewed phases: 1, 2, 3
Plan: `context/changes/show-main-menu/plan.md`
Date: 2026-10-18
Verdict: pass with notes

The `/10x-impl-review` skill is not in this repository. This report follows the archive coverage contract (`Reviewed phases`) and checks the implementation against the plan’s contracts.

## Summary

The slice matches the desired end state. Play starts on `MainMenu`. The buttons are „Nowa rozgrywka” and „Wznów”. Resume stays on the menu and writes `Brak zapisu do wznowienia.` New game loads `MapView` by name. Runtime stays engine-free. Scene index 0 is the menu in both `EditorBuildSettings` and the Android build profile. Magda confirmed the Play Mode path.

## Phase 1: Main menu scene

| Contract | Result |
| --- | --- |
| `Grodowladcy.Presentation` asmdef, `UnityEngine.UI` only, no Runtime, no `noEngineReferences` | Pass |
| Public resume method sets `UnityEngine.UI.Text` to `Brak zapisu do wznowienia.` and does not load a scene | Pass (`Resume()`) |
| Canvas + EventSystem with `InputSystemUIInputModule` | Pass (script guid `01614664b831546d2ae94a42149d80ac`; actions guid `2bcd2660ca9b64942af0de543d8d7100`) |
| Labels `Nowa rozgrywka` and `Wznów`; message text empty until resume | Pass |
| Manual: both labels visible; Wznów shows the empty-save line | Pass (confirmed in Play Mode) |

## Phase 2: Open MapView

| Contract | Result |
| --- | --- |
| EditorBuildSettings index 0 `MainMenu`, index 1 `MapView` guid `8c9cfa26abfee488c85f1582747f6a02` | Pass |
| Android profile same order, `m_OverrideGlobalSceneList` stays 1 | Pass |
| `SceneManager.LoadScene("MapView")` | Pass (`NewGame()`) |
| Nowa rozgrywka OnClick → new-game method; Wznów listener unchanged | Pass |
| MapView exists; SampleScene absent; guid preserved | Pass |
| Manual: Nowa rozgrywka loads MapView; board still camera / 2D light / field sprite | Pass (confirmed in Play Mode) |

## Phase 3: Align menu labels

| Contract | Result |
| --- | --- |
| PRD primary success criterion and FR-002 offer „Nowa rozgrywka” and „Wznów”; no „Nowa gra” / „Wczytaj autosave” in those sentences | Pass |
| Autosave rule unchanged (one local profile, file at start of phase 1) | Pass |
| Roadmap S-07, S-01, S-06 outcomes, risk lines, milestone intent, stream B use the same button names | Pass |
| SampleScene baseline paragraph left as-is | Pass |

## Scope

Not changed, as planned: Runtime rules, CiBuild, GhReleaseWindow, UI Toolkit / TMP / localization, MapView return control, save/load session, tasks-github.md, tasks-linear.md, shape-notes.md.

EditMode suites stayed green (10/10) after Phases 1 and 2. No canvas tests were added.

## Notes (non-blocking)

1. `ProjectSettings/ProjectSettings.asset` `templateDefaultScene` still points at `Assets/Scenes/SampleScene.unity`. The plan says this field does not select startup and should not be treated as a third build list. The migration note also says the stale path may remain. Safe to leave; it is leftover template metadata.
2. US-01 acceptance still says “Menu supports new game and load autosave”. Phase 3 only retargeted the primary success criterion and FR-002.
3. `change.md` notes still say “S-01 from context/foundation/roadmap.md”. The change is S-07. Harmless leftover from planning.
4. Resume does not null-check `messageText`. The scene assigns it. Fine for this slice.

## Verdict

No contract misses that need a follow-up change. The implemented menu is the one the plan described.
