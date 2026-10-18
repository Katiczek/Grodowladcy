# Show Main Menu — Plan Brief

> Full plan: `context/changes/show-main-menu/plan.md`

## What & Why

The player needs a first screen before any turn exists. Roadmap slice S-07 is that screen: two buttons, and nothing else from the session. „Nowa rozgrywka” opens the board scene that is already in the project. „Wznów” does not pretend a save exists.

## Starting Point

Play opens `Assets/Scenes/MapView.unity`: a camera, a 2D light, and a field sprite. There is no menu, no session, and no save code. Rules in `Grodowladcy.Runtime` do not reference Unity and stay that way. uGUI and the Input System are already installed.

## Desired End State

Play starts on a menu labeled „Nowa rozgrywka” and „Wznów”. „Wznów” shows „Brak zapisu do wznowienia.” and stays there. „Nowa rozgrywka” loads `MapView`, the same board asset under a new name, still without a map, a name prompt, or a way back.

## Key Decisions Made

| Decision | Choice | Why (1 sentence) |
| --- | --- | --- |
| Button labels | „Nowa rozgrywka” and „Wznów” | Replaces both the roadmap’s „New game” / „Continue” and the PRD’s „Nowa gra” / „Wczytaj autosave”. |
| New game | Loads `MapView` | The existing scene is the destination; this slice does not build the map. |
| Continue with no save | Stay on the menu and show „Brak zapisu do wznowienia.” | The button stays visible, no session opens, and the press has a visible result. |
| UI stack | uGUI in `Grodowladcy.Presentation` | uGUI is already a dependency, and Runtime cannot reference UnityEngine. |
| MapView return | No back control | The slice is the two menu buttons; S-01 fills the board later. |

## Scope

**In scope:**

- A startup menu scene with those two labels
- The „Wznów” empty-save message
- Loading `MapView` for „Nowa rozgrywka”
- Pointing Editor build settings and the Android build profile at that order

**Out of scope:**

- Hex map, resources, and the local name (S-01)
- Autosave and actually resuming (S-06)
- A return button on `MapView`
- Runtime rule changes, UI Toolkit, and TextMeshPro

## Architecture / Approach

`MainMenu` is the first enabled scene. A presentation `MonoBehaviour` handles the two buttons. „Wznów” writes one uGUI text line. „Nowa rozgrywka” performs a single-mode `SceneManager.LoadScene("MapView")`. The EventSystem uses the Input System UI module because the project has the old input manager turned off. The board asset keeps guid `8c9cfa26abfee488c85f1582747f6a02`.

## Phases at a Glance

| Phase | What it delivers | Key risk |
| --- | --- | --- |
| 1. Main menu scene | Both labels, and „Wznów” reports that there is no save | Clicks do nothing if the EventSystem uses the standalone input module |
| 2. Open MapView | Scene loads from „Nowa rozgrywka”, and Play still starts on the menu | |
| 3. Align menu labels | PRD and roadmap S-07, S-01, and S-06 use „Nowa rozgrywka” and „Wznów” | Task trackers and shape notes keep the older names |

**Prerequisites:** Unity `6000.6.2f1` with the existing uGUI and Input System packages. No new packages.
**Estimated effort:** One session across 3 phases.

## Open Risks & Assumptions

- A hand-written menu scene may fail to open. Create it in the Editor if that happens.
- `templateDefaultScene` is not the startup list. The build profile is.

## Success Criteria (Summary)

- Play opens a menu with „Nowa rozgrywka” and „Wznów”.
- „Wznów” shows „Brak zapisu do wznowienia.” and does not change scene.
- „Nowa rozgrywka” opens `MapView`, which still shows only the camera, the 2D light, and the field sprite.
