---
project: null
context_type: greenfield
created: 2026-09-22
updated: 2026-09-22
checkpoint:
  current_phase: 8
  phases_completed: [1, 2, 3, 4, 5, 6, 7]
  gray_areas_resolved:
    - topic: "pain category"
      decision: "missing capability + coordination overhead + decision paralysis (shared creative start + short local session)"
    - topic: "insight"
      decision: "shared creative project > local host without cloud accounts > hero-persuasion differentiator for short sessions"
    - topic: "primary persona scope"
      decision: "hobby niche: Magda + fixed friend group as co-creators and players"
    - topic: "socratic risk acknowledged"
      decision: "friends may not buy the concept; on trips people may prefer sleep — travel boredom is opportunity not acute pain"
    - topic: "auth strategy"
      decision: "local profile only; no cloud accounts; host sets optional match settings and shares join code; in-match rules otherwise flat"
    - topic: "MVP primary flow"
      decision: "single-player hot-path (~3 weeks after hours); hero hire via hidden min gold threshold instead of competing offers; multiplayer deferred to Secondary"
    - topic: "persistence"
      decision: "autosave to file at start of phase 1; menu offers new game or load autosave"
    - topic: "hero job UX"
      decision: "task type is derived from selected field type; player only picks field + reward"
    - topic: "FR-015 traits"
      decision: "richer mission outcomes/traits deferred as Non-Goal until Primary works"
    - topic: "business rule acceptance inputs"
      decision: "MVP acceptance uses only offered coins vs hidden coin threshold; richer factors are nice-to-have later"
    - topic: "business rule mission success inputs"
      decision: "success uses hero strength vs enemy strength"
    - topic: "NFRs"
      decision: "offline session+autosave; snappy UI feedback; Android-only MVP; no cloud accounts / no sending game data off-device"
    - topic: "product framing"
      decision: "mobile; small scale; no hard deadline; after-hours; domain rules unchanged at 100x scale"
  frs_drafted: 15
  quality_check_status: accepted
timeline_budget:
  mvp_weeks: 3
  hard_deadline: null
  after_hours_only: true
product_type: mobile
target_scale:
  users: small
  qps: low
  data_volume: small
---

# Shape notes — Grodowładcy (working title)

## Seed idea (verbatim from prior assessment)

Turowa strategia multiplayer na mapie hexowej: gracze rozwijają małe królestwa i rywalizują o niezależnych bohaterów. Gracz nie steruje bohaterami bezpośrednio — wystawia tajne zlecenia; bohaterowie wybierają najbardziej atrakcyjną ofertę. MVP: aplikacja Android, lokalny host (jedna osoba = serwer), 2–3 gracze, monety/sława, trzy fazy tury w okrojonej formie, zwycięstwo na sławie. Poza zakresem na start: pełny cykl bohaterów (XP/rany), rozbudowane PvP terytorialne.

## Vision & Problem Statement

Budujecie krótką, lokalną grę multiplayer na telefony jako wspólny projekt kreatywny (Magda + stała paczka przyjaciół) — pretekst do współpracy i nauki. „Nuda w aucie/pociągu” to okazja do grania, nie ostry ból: dziś znajomi śpią, nudzą się, patrzą przez okno i to samo w sobie nie jest złe.

Insight: ważniejszy jest wspólny start projektu do kreatywnego rozwijania gry niż sam produkt „na nudę”; lokalny host na telefonach bez kont w chmurze wystarcza; krótka strategia z przekonywaniem niezależnych bohaterów lepiej pasuje do trasy niż klasyczna ciężka strategia online. Świadome ryzyko: koncept może nie wciągnąć paczki; w trasie ludzie mogą woleć spać. Skala 100×: reguły przyjęcia zlecenia i sukcesu misji się nie zmieniają (offline / mała paczka).

## User & Persona

Primary persona: Magda i stała paczka przyjaciół — jednocześnie współtwórcy gry i gracze lokalnych potyczek (2–3 osoby na sesję). Kontekst użycia: wspólny wyjazd, jazda autem/pociągiem; sesja ma być krótka.

## Access Control

Local profile na urządzeniu — bez kont w chmurze / bez klasycznego logowania. Gracz ustawia lokalny profil (np. nick).

Dla MVP Primary wystarczy lokalny profil + start gry jednoosobowej. Host z opcjonalnymi ustawieniami meczu i kodem dołączenia należy do Secondary (obsługa drugiego gracza / lokalna sieć); gdy to dojdzie: poza uprawnieniami hosta do ustawień i kodu model w meczu pozostaje płaski.

## Success Criteria

### Primary

Gracz jednoosobowy może ukończyć pełną sesję:

1. Menu główne → „Nowa gra” lub „Wczytaj autosave” (jeden lokalny profil). Autosave zapisywany do pliku na początku fazy pierwszej.
2. Faza Wierni poddani: mapa hex; wydawanie puli poddanych na sąsiednie pola — zajmij puste / zbuduj farmę (koszt monet, generuje monety w kolejnych turach) / zbuduj dom (koszt monet, zwiększa pulę poddanych); „Zakończ fazę”.
3. Faza Najmij bohatera: lista bohaterów ze statystykami; wystawienie ogłoszenia — gracz wybiera pole i nagrodę X monet; typ zadania wynika automatycznie z typu pola (potwór → zabij, wieś → przejmij, własne → broń). Ukryty krok przydziału: oferta monet vs ukryty próg monet bohatera → przyjęte lub zignorowane.
4. Faza Rozwiązanie: realizacja zaplanowanych akcji (zajęcia, budynki, misje). Dla przyjętych zleceń gra rozstrzyga sukces/porażkę na podstawie siły bohatera vs siły wroga.
5. Powrót do fazy poddanych; powtarzanie tur aż osiągnięcie ustalonego progu sławy.

Timeline: ~3 tygodnie pracy po godzinach (`mvp_weeks: 3`).

### Secondary

W kolejności preferencji (po Primary):

1. Obsługa drugiego gracza — lokalny host z kodem dołączenia (lokalna sieć; transport niepinowany do BT).
2. Rozbudowane statystyki bohaterów.
3. Więcej typów pól na mapie z interakcjami.
4. Walka pomiędzy graczami.

### Guardrails

- A) Tura musi dać się dokończyć bez crasha / softlocka.
- C) Stan gry (mapa, monety, sława) nie ginie przy wyjściu z aplikacji w trakcie sesji (autosave).

## Functional Requirements

### Session & persistence

- FR-001: Player can start a new game. Priority: must-have
  > Socrates: No counter-argument; it stands as written.
- FR-002: Player can load the autosave; the game autosaves to a file at the start of phase 1; menu offers "Nowa gra" or "Wczytaj autosave". Priority: must-have
  > Socrates: Counter-argument considered: persistence shape unclear. Resolution: changed — explicit autosave-at-phase-1 + load autosave (not vague "last game").

### Map & loyal subjects

- FR-003: Player can select a hex field on the map. Priority: must-have
  > Socrates: No counter-argument; it stands as written.
- FR-004: Player can choose a field action (claim empty field, build a farm, build a house). Priority: must-have
  > Socrates: Counter-argument considered: earlier "build village" made subject actions inconsistent. Resolution: "buduj wioskę" removed everywhere; only claim / farm / house. Neutral village remains a map tile for hero jobs.
- FR-005: Player can advance between turn phases. Priority: must-have
  > Socrates: No counter-argument; it stands as written.

### Heroes & contracts

- FR-006: Player can browse the list of available heroes with their public stats. Priority: must-have
  > Socrates: No counter-argument; it stands as written.
- FR-007: Player can post a hero announcement by selecting a field and a coin reward; the task type is derived automatically from the field type. Priority: must-have
  > Socrates: Counter-argument considered: choosing task manually adds UI steps. Resolution: changed — task follows field type; player picks field + reward only.
- FR-014: Hero missions may fail; the game resolves success or failure from hero strength versus enemy strength. Priority: must-have
  > Socrates: No counter-argument; it stands as written. (Later refined: enemy strength, not abstract "task difficulty" alone.)

### Resources UI

- FR-008: Player can always see current coins and fame. Priority: must-have
  > Socrates: No counter-argument; it stands as written.

### Nice-to-have (post-Primary)

- FR-009: Players can play local multiplayer over a local network (transport not pinned to Bluetooth). Priority: nice-to-have
  > Socrates: Counter-argument considered: Bluetooth-specific scope is disproportionately expensive. Resolution: changed — "local network"; BT is a later transport choice, not the FR wording.
- FR-010: Player can interact with more field types on the map. Priority: nice-to-have
  > Socrates: No counter-argument; it stands as written.
- FR-011: Player can view richer hero character cards and stats. Priority: nice-to-have
  > Socrates: No counter-argument; it stands as written.
- FR-012: Player can attack enemy fields and capture them (empty enemy fields via loyal subjects; village/capital fields via heroes). Priority: nice-to-have
  > Socrates: Counter-argument considered: PvP capture is meaningless before multiplayer. Resolution: kept as nice-to-have but explicitly gated on FR-009 (after local multiplayer exists).
- FR-013: Player can win by conquering the opponent's lands (alternate win condition). Priority: nice-to-have
  > Socrates: No counter-argument; it stands as written.
- FR-015: Hero tasks can produce richer outcomes than success/failure (e.g. granting new hero traits). Priority: nice-to-have
  > Socrates: Counter-argument considered: trait system too early. Resolution: deferred — treat as Non-Goal until Primary works; do not build in MVP.

## User Stories

### US-01: First solo turn through resolution and win check

- **Given** the player starts a new game on a predefined hex map, with a capital, starting coins, fame, and loyal subjects; the map has empty fields, a neutral village, and a monster
- **When** the first turn begins in phase 1 (loyal subjects)
- **Then** the player sees the map and can select fields adjacent to their kingdom to plan one available action: claim empty hex, build farm, or build house

- **When** the player presses "Zakończ fazę"
- **Then** the game advances to phase 2 (hire heroes) and the player can browse available heroes with their public stats

- **When** the player presses "Wystaw ogłoszenie"
- **Then** the player picks the target field; the job summary (including task type derived from the field) is shown; the player enters the coin reward

- **When** the player presses "Zakończ fazę"
- **Then** the game enters a hidden hero-assignment step: offered coins vs the hero's hidden coin threshold decide accept vs ignore; the player sees "Twoje zlecenie zostało… zignorowane" or "przyjęte"

- **When** hero assignment finishes
- **Then** phase 3 (resolution) runs: map view is active; phase-1 plans execute; for accepted jobs, before showing the outcome, the game resolves success using hero strength vs enemy strength; on success the player sees "Udało się, zyskałeś X, Y"; on failure "Bohater poległ w zadaniu"

- **When** resolution finishes
- **Then** win conditions are checked — if the player reached the set fame threshold they win; otherwise the game returns to phase 1 (autosave occurs at the start of phase 1)

#### Acceptance Criteria
- Subject-phase actions: claim empty / farm / house only (no "build village"); adjacent to kingdom only
- Task type is automatic from field type; player does not pick task separately
- Hero acceptance uses offered coins vs hidden coin threshold only (MVP)
- Mission outcome uses hero strength vs enemy strength; messages: success gains / hero fell
- Fame threshold ends the game; otherwise the turn loop continues
- Menu supports new game and load autosave; autosave at start of phase 1

## Business Logic

Gra porównuje monetową nagrodę zlecenia z ukrytym progiem monet oczekiwanym przez bohatera i decyduje, czy bohater przyjmie zlecenie.

Po przyjęciu zlecenia gra wylicza szanse na sukces misji na podstawie siły bohatera i siły wroga, którego ma pokonać.

Wejścia (MVP): przy przyjęciu — liczba monet nagrody oraz ukryty próg monet bohatera (tylko tyle; rozbudowa czynników to nice-to-have). Przy wykonaniu — siła bohatera i siła wroga.

Wynik dla gracza: komunikat „przyjęte” / „zignorowane”; dla przyjętych zadań — „Udało się, zyskałeś X, Y” albo „Bohater poległ w zadaniu”.

Moment w turze: decyzja o przyjęciu zaraz po „Zakończ fazę” w fazie najmu; decyzja o sukcesie misji w fazie trzeciej, zanim wynik zostanie pokazany graczowi.

## Non-Functional Requirements

- Sesja i autosave działają offline (bez wymaganego internetu).
- Akcje UI dają widoczną odpowiedź w odczuwalnie krótkim czasie (orientacyjnie < ~1–2 s).
- MVP działa na Androidzie na telefonach docelowej paczki (bez iOS w MVP).
- Brak kont w chmurze i brak wysyłki danych gry poza urządzenie.

## Non-Goals

- Unikać w Primary: multiplayer / lokalna sieć / kod hosta (to Secondary).
- Unikać do czasu działającego Primary: system cech bohaterów i bogatszych skutków misji (FR-015).
- Unikać: PvP — atak wrogich pól i alternatywne zwycięstwo przez podbicie ziem.
- Unikać: iOS, konta w chmurze, wymagany internet.
- Unikać: proceduralna / nieskończona mapa — zostaje wcześniej zdefiniowana mapa.
- Unikać: pełny cykl bohaterów (XP, rany, długi powrót do karczmy).

## Quality cross-check

- Access Control: present
- Business Logic: present (two one-sentence rules)
- Project artifacts: present
- Timeline-cost ack: present (mvp_weeks: 3)
- Non-Goals: present
- Preserved behavior: n/a (greenfield)
- Status: accepted — no gaps overriding required
- Note: `project` frontmatter still null (working title "Grodowładcy"); confirm formal name at `/10x-prd`

## Forward: tech-stack

**Locked 2026-09-23:** Unity (manual). Outside course starter registry — no `starter_id`, do not run `/10x-bootstrapper` for this stack.

Rationale: learning goal is Unity; registry mobile defaults are Expo/Flutter only. Project will be created via Unity Hub (Android target). PRD at `context/foundation/prd.md` remains the product contract.

Local connectivity (optional later / Secondary): not pinned; decide in Unity networking layer when multiplayer is in scope.

Użytkownik wskazał wcześniej też Godot jako alternatywę — odrzucone na rzecz Unity.
