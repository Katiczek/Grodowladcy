# Show Main Menu Implementation Plan

## Overview

Add the first screen a player sees: a main menu with „Nowa rozgrywka” and „Wznów”. „Wznów” stays on that menu and says there is no save. „Nowa rozgrywka” opens the existing board scene  `MapView`. This is roadmap slice S-07 (`show-main-menu`). The map, the local name, and loading a save stay in later slices.

## Current State Analysis

The player currently lands on one build scene, `Assets/Scenes/MapView.unity`. That scene has a camera, a 2D global light, and a field sprite. It has no canvas, no buttons, and no menu code.

Gameplay rules live in `Grodowladcy.Runtime`, which sets `noEngineReferences: true` (`Assets/Scripts/Runtime/Grodowladcy.Runtime.asmdef`). Nothing there models a session, a menu, or a save. EditMode tests reference only that assembly.

uGUI (`com.unity.ugui` 2.6.0) and the Input System are already dependencies. The player uses the Input System only (`ProjectSettings/ProjectSettings.asset`, `activeInputHandler: 1`). There is no UI Toolkit game UI and no localization package.

The scenes that actually ship are listed in two places. `CiBuild.TryBuildAndroidApk` copies `EditorBuildSettings.scenes` in order into the APK, and Android loads index 0. The Android build profile overrides that list (`m_OverrideGlobalSceneList: 1` in `Assets/Settings/Build Profiles/Grodowładcy.asset`) and is a second launch path.

## Desired End State

An Android launch loads the main menu, because `MainMenu` is scene index 0. Both labels are visible: „Nowa rozgrywka” and „Wznów”. Pressing „Wznów” shows „Brak zapisu do wznowienia.” and does not change scene. Pressing „Nowa rozgrywka” loads `MapView`, which is the renamed board scene and still shows only the camera, the 2D light, and the field sprite.

### Key Discoveries:

- `EditorBuildSettings` lists one enabled scene, `Assets/Scenes/MapView.unity`, guid `8c9cfa26abfee488c85f1582747f6a02`. The Android profile still points at `Assets/Scenes/SampleScene.unity` with that same guid.
- Runtime cannot host a `MonoBehaviour`. Menu code needs a new assembly that can reference `UnityEngine.UI`.
- `templateDefaultScene` in `ProjectSettings/ProjectSettings.asset` is leftover template metadata, not the list that decides which scene plays.

## What We're NOT Doing

- Building the hex map, capital, coins, fame, subjects, or the local name. That is S-01.
- Creating a save file, detecting a save, or opening a session from „Wznów”. That is S-06.
- Adding a return control on `MapView`.
- Changing `GameConstants`, `HeroAssignment`, `MissionResolver`, or `WinCondition`.
- Adding UI Toolkit, TextMeshPro, or a localization package.
- Changing `CiBuild` or `GhReleaseWindow`.
- Rewriting the roadmap baseline snapshot, `tasks-github.md`, `tasks-linear.md`, or `shape-notes.md`. Phase 3 only renames the menu buttons in `prd.md` and in the S-07, S-01, and S-06 lines of `roadmap.md`.

## Implementation Approach

Use uGUI, which is already in the project. Put menu code in a new `Grodowladcy.Presentation` assembly and keep Runtime engine-free and untouched. Add `Assets/Scenes/MainMenu.unity` as enabled scene index 0 in both scene lists. „Nowa rozgrywka” calls `SceneManager.LoadScene("MapView")`. „Wznów” only sets a `UnityEngine.UI.Text` on the same menu.

## Critical Implementation Details

- **Input.** `activeInputHandler` is `1`, so the menu EventSystem must use `InputSystemUIInputModule` with the project's existing UI actions. `StandaloneInputModule` will not receive clicks.
- **Scene lists.** Android loads scene index 0. `CiBuild.TryBuildAndroidApk` copies `EditorBuildSettings.scenes` in order and does not read the build profile, so index 0 there must be `Assets/Scenes/MainMenu.unity`. The Android profile overrides the global list and still names `SampleScene`, so its index 0 must be `MainMenu` too. Leave `CiBuild` unchanged.

## Phase 1: Main menu scene

### Overview

The player can open a main menu, read both labels, and press „Wznów” without leaving the menu.

### Changes Required:

#### 1. Presentation assembly

**File**: `Assets/Scripts/Presentation/Grodowladcy.Presentation.asmdef`

**Intent**: Give menu code a Unity-referencing assembly so Runtime stays free of engine types.

**Contract**: Assembly name `Grodowladcy.Presentation`, root namespace `Grodowladcy.Presentation`. References include `UnityEngine.UI` only. `noEngineReferences` is not set. No reference to `Grodowladcy.Runtime`.

#### 2. Menu controller

**File**: `Assets/Scripts/Presentation/MainMenuController.cs`

**Intent**: „Wznów” tells the player there is nothing to load. This phase does not load a scene.

**Contract**: A public resume method sets one `UnityEngine.UI.Text` to exactly `Brak zapisu do wznowienia.` and does not call `SceneManager.LoadScene`. Before that press, the text is empty. Use `UnityEngine.UI.Text`, not TextMeshPro.

#### 3. Main menu scene

**File**: `Assets/Scenes/MainMenu.unity`

**Intent**: Show the two buttons on a uGUI canvas that the Input System can click.

**Contract**: One Canvas, an EventSystem using `InputSystemUIInputModule`, two buttons labeled exactly `Nowa rozgrywka` and `Wznów`, and the message text from the controller. „Wznów” invokes the resume method. „Nowa rozgrywka” has no scene load in this phase.

### Success Criteria:

#### Automated Verification:

- Presentation asmdef exists at Assets/Scripts/Presentation/Grodowladcy.Presentation.asmdef, references UnityEngine.UI, and does not reference Grodowladcy.Runtime or set noEngineReferences
- Main menu scene and controller contain the literals Nowa rozgrywka, Wznów, and Brak zapisu do wznowienia.
- EditMode tests pass via Unity Test Runner

#### Manual Verification:

- Play starts on the main menu and both button labels are visible
- Pressing Wznów shows Brak zapisu do wznowienia. and does not change the scene

**Implementation Note**: After completing this phase and all automated verification passes, pause here for manual confirmation from the human that the manual testing was successful before proceeding to the next phase.

---

## Phase 2: Open MapView

### Overview

„Nowa rozgrywka” opens the existing board scene under the name `MapView`. Play still starts on the menu.

### Changes Required:

#### 1. Scene lists

**File**: `ProjectSettings/EditorBuildSettings.asset`

**Intent**: Boot the menu on Android, and keep the board loadable.

**Contract**: Index 0 is `Assets/Scenes/MainMenu.unity`. Index 1 is `Assets/Scenes/MapView.unity` with guid `8c9cfa26abfee488c85f1582747f6a02`. This order is what `CiBuild` puts into the APK, so an Android launch loads `MainMenu`.

#### 2. Android build profile

**File**: `Assets/Settings/Build Profiles/Grodowładcy.asset`

**Intent**: Ship the same order from the Android build profile, because this profile replaces the global scene list.

**Contract**: Index 0 is `Assets/Scenes/MainMenu.unity`. Index 1 is `Assets/Scenes/MapView.unity` with guid `8c9cfa26abfee488c85f1582747f6a02`. `m_OverrideGlobalSceneList` stays 1.

#### 4. New-game load

**File**: `Assets/Scripts/Presentation/MainMenuController.cs`

**Intent**: „Nowa rozgrywka” leaves the menu for the renamed board and does not build a map.

**Contract**: The new-game method loads the scene by the name `MapView` in single mode:

```csharp
SceneManager.LoadScene("MapView");
```

The resume method from Phase 1 stays unchanged.

#### 5. Wire the new-game button

**File**: `Assets/Scenes/MainMenu.unity`

**Intent**: The button from Phase 1 calls the load method.

**Contract**: The „Nowa rozgrywka” button OnClick calls the new-game method. The „Wznów” listener from Phase 1 stays unchanged.

### Success Criteria:

#### Automated Verification:

- Assets/Scenes/MapView.unity exists, Assets/Scenes/SampleScene.unity does not, and the MapView meta guid is 8c9cfa26abfee488c85f1582747f6a02
- EditorBuildSettings and the Android build profile list MainMenu at index 0 and MapView at index 1
- EditMode tests pass via Unity Test Runner

#### Manual Verification:

- Pressing Nowa rozgrywka loads MapView
- MapView still shows only the camera, the 2D light, and the field sprite, with no menu and no return control

**Implementation Note**: After completing this phase and all automated verification passes, pause here for manual confirmation from the human that the manual testing was successful before proceeding to the next phase.

---

## Phase 3: Align menu labels

### Overview

The buttons stay „Nowa rozgrywka” and „Wznów”. The PRD menu sentence and the roadmap names for those buttons use the same words.

### Changes Required:

#### 1. PRD menu offers

**File**: `context/foundation/prd.md`

**Intent**: FR-002 and the primary success criterion name the buttons the player sees.

**Contract**: In the primary success criterion and in FR-002, the menu offers „Nowa rozgrywka” and „Wznów” where it now offers „Nowa gra” and „Wczytaj autosave”. The autosave rule stays: one local profile, saved at the start of phase 1.

#### 2. Roadmap button names

**File**: `context/foundation/roadmap.md`

**Intent**: S-07, S-01, and S-06 name the same buttons.

**Contract**: Replace the menu button names only. „Nowa rozgrywka” is the new-game button. „Wznów” is the resume button. Update the milestone intent, stream B’s „przycisk Continue”, and the slice table, outcome lines, and risk lines for S-07, S-01, and S-06. The S-07 risk line that says the screen reads „Continue” while FR-002 still calls the action an autosave load now says the screen and FR-002 both use „Wznów”. Leave the SampleScene baseline paragraph as it is.

### Success Criteria:

#### Automated Verification:

- context/foundation/prd.md primary success criterion and FR-002 offer Nowa rozgrywka and Wznów, and the menu sentence no longer offers Nowa gra or Wczytaj autosave
- context/foundation/roadmap.md names the S-07, S-01, and S-06 buttons as Nowa rozgrywka and Wznów, and those slice outcomes no longer say New game or Continue

---

## Testing Strategy

### Unit Tests:

- Do not add EditMode tests for the canvas. `Grodowladcy.Tests.EditMode` references only `Grodowladcy.Runtime`.
- The existing Runtime suites (`GameBalanceTests`, `HeroAssignmentTests`, `MissionResolverTests`) must still pass after every phase, because this change does not edit them.

### Integration Tests:

- No PlayMode assembly exists. Scene loading is checked manually in the Editor.

### Manual Testing Steps:

1. Open the project in Unity `6000.6.2f1` and press Play. The main menu appears, not the board.
2. Confirm the labels „Nowa rozgrywka” and „Wznów”. The message line is empty.
3. Press „Wznów”. The text „Brak zapisu do wznowienia.” appears. The menu is still the active scene.
4. Press „Nowa rozgrywka”. The active scene is `MapView`: camera, 2D light, and field sprite only. There is no button back to the menu.
5. Stop Play and enter Play again. The menu is first again.

## Performance Considerations

Both scenes are local and already in the build. One `LoadScene` call is the whole transition. That sits inside the PRD expectation that a UI action responds within about 1–2 seconds. No cache and no extra scene in memory.

## Migration Notes

No player save exists yet, so there is nothing to migrate. The board scene moves with its `.meta`. `templateDefaultScene` may still mention `SampleScene`; that field does not select the startup scene. Update it only so the stale path does not remain. Do not treat it as a third build list.

## References

- Roadmap slice S-07: `context/foundation/roadmap.md`
- Menu requirements: `context/foundation/prd.md` (FR-001, FR-002, US-01)
- Engine boundary: `Assets/Scripts/Runtime/Grodowladcy.Runtime.asmdef`
- uGUI dependency: `Packages/manifest.json`
- Scene lists: `ProjectSettings/EditorBuildSettings.asset`, `Assets/Settings/Build Profiles/Grodowładcy.asset`
- No frame brief and no research doc for this change

## Progress

> Convention: `- [ ]` pending, `- [x]` done. Append ` — <commit sha>` when a step lands. Do not rename step titles. See `references/progress-format.md`.

### Phase 1: Main menu scene

#### Automated

- [x] 1.1 Presentation asmdef exists at Assets/Scripts/Presentation/Grodowladcy.Presentation.asmdef, references UnityEngine.UI, and does not reference Grodowladcy.Runtime or set noEngineReferences
- [x] 1.2 Main menu scene and controller contain the literals Nowa rozgrywka, Wznów, and Brak zapisu do wznowienia.
- [x] 1.3 EditMode tests pass via Unity Test Runner

#### Manual

- [x] 1.4 Play starts on the main menu and both button labels are visible
- [x] 1.5 Pressing Wznów shows Brak zapisu do wznowienia. and does not change the scene

### Phase 2: Open MapView

#### Automated

- [ ] 2.1 Assets/Scenes/MapView.unity exists, and the MapView meta guid is 8c9cfa26abfee488c85f1582747f6a02
- [ ] 2.2 EditorBuildSettings and the Android build profile list MainMenu at index 0 and MapView at index 1
- [ ] 2.3 EditMode tests pass via Unity Test Runner

#### Manual

- [ ] 2.4 Pressing Nowa rozgrywka loads MapView
- [ ] 2.5 MapView still shows only the camera, the 2D light, and the field sprite, with no menu and no return control

### Phase 3: Align menu labels

#### Automated

- [ ] 3.1 context/foundation/prd.md primary success criterion and FR-002 offer Nowa rozgrywka and Wznów, and the menu sentence no longer offers Nowa gra or Wczytaj autosave
- [ ] 3.2 context/foundation/roadmap.md names the S-07, S-01, and S-06 buttons as Nowa rozgrywka and Wznów, and those slice outcomes no longer say New game or Continue
