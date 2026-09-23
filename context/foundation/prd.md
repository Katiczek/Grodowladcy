---
project: "Grodowładcy"
version: 1
status: draft
created: 2026-09-22
context_type: greenfield
product_type: mobile
target_scale:
  users: small
  qps: low
  data_volume: small
timeline_budget:
  mvp_weeks: 3
  hard_deadline: null
  after_hours_only: true
---

# Grodowładcy

## Vision & Problem Statement

Budujecie krótką, lokalną grę multiplayer na telefony jako wspólny projekt kreatywny (Magda + stała paczka przyjaciół) — pretekst do współpracy i nauki. „Nuda w aucie/pociągu” to okazja do grania, nie ostry ból: dziś znajomi śpią, nudzą się, patrzą przez okno i to samo w sobie nie jest złe.

Insight: ważniejszy jest wspólny start projektu do kreatywnego rozwijania gry niż sam produkt „na nudę”; lokalny host na telefonach bez kont w chmurze wystarcza; krótka strategia z przekonywaniem niezależnych bohaterów lepiej pasuje do trasy niż klasyczna ciężka strategia online. Świadome ryzyko: koncept może nie wciągnąć paczki; w trasie ludzie mogą woleć spać. Skala 100×: reguły przyjęcia zlecenia i sukcesu misji się nie zmieniają (offline / mała paczka).

## User & Persona

Primary persona: Magda i stała paczka przyjaciół — jednocześnie współtwórcy gry i gracze lokalnych potyczek (2–3 osoby na sesję). Kontekst użycia: wspólny wyjazd, jazda autem/pociągiem; sesja ma być krótka.

## Success Criteria

### Primary

Gracz jednoosobowy może ukończyć pełną sesję:

1. Menu główne → „Nowa gra” lub „Wczytaj autosave” (jeden lokalny profil). Autosave zapisywany do pliku na początku fazy pierwszej. Start: **10 monet, 0 sławy, 3 wiernych poddanych**.
2. Faza Wierni poddani: mapa hex; wydawanie puli poddanych na sąsiednie pola — zajmij puste / zbuduj farmę (**koszt 3, +2 monety/turę**) / zbuduj dom (**koszt 4, +1 do puli poddanych**); „Zakończ fazę”.
3. Faza Najmij bohatera: **3 bohaterów w puli, nowa pula co turę**; lista ze statystykami; wystawienie ogłoszenia — gracz wybiera pole i nagrodę X monet; typ zadania wynika automatycznie z typu pola (potwór → zabij, wieś → przejmij, własne → broń). Ukryty krok przydziału: oferta monet vs ukryty próg monet bohatera → przyjęte lub zignorowane.
4. Faza Rozwiązanie: realizacja zaplanowanych akcji (zajęcia, budynki, misje). Dla przyjętych zleceń gra rozstrzyga sukces/porażkę na podstawie siły bohatera vs siły wroga (**potwór 5 · wieś 3 · obrona własnego pola 2**). Sukces: potwór **+3 sławy** · wieś **+5 monet** (jednorazowo) · obrona **+1 sława**. Porażka: „Bohater poległ w zadaniu”.
5. Powrót do fazy poddanych; powtarzanie tur aż **sława ≥ 20**.

Timeline: ~3 tygodnie pracy po godzinach (`mvp_weeks: 3`).

### Secondary

W kolejności preferencji (po Primary):

1. Obsługa drugiego gracza — lokalny host z kodem dołączenia (lokalna sieć; transport niepinowany do konkretnego medium).
2. Rozbudowane statystyki bohaterów.
3. Więcej typów pól na mapie z interakcjami.
4. Walka pomiędzy graczami.

### Guardrails

- A) Tura musi dać się dokończyć bez crasha / softlocka.
- C) Stan gry (mapa, monety, sława) nie ginie przy wyjściu z aplikacji w trakcie sesji (autosave).

## User Stories

### US-01: First solo turn through resolution and win check

- **Given** the player starts a new game on a predefined hex map, with a capital, **10 coins, 0 fame, and 3 loyal subjects**; the map has empty fields, a neutral village, and a monster
- **When** the first turn begins in phase 1 (loyal subjects)
- **Then** the player sees the map and can select fields adjacent to their kingdom to plan one available action: claim empty hex, build farm, or build house

- **When** the player presses "Zakończ fazę"
- **Then** the game advances to phase 2 (hire heroes) and the player can browse available heroes with their public stats

- **When** the player presses "Wystaw ogłoszenie"
- **Then** the player picks the target field; the job summary (including task type derived from the field) is shown; the player enters the coin reward

- **When** the player presses "Zakończ fazę"
- **Then** the game enters a hidden hero-assignment step: offered coins vs the hero's hidden coin threshold decide accept vs ignore; the player sees "Twoje zlecenie zostało… zignorowane" or "przyjęte"

- **When** hero assignment finishes
- **Then** phase 3 (resolution) runs: map view is active; phase-1 plans execute; for accepted jobs, before showing the outcome, the game resolves success using hero strength vs enemy strength (monster 5 / village 3 / own-field defense 2); on success: monster +3 fame, village +5 coins once, defense +1 fame; on failure "Bohater poległ w zadaniu"

- **When** resolution finishes
- **Then** win conditions are checked — if fame ≥ 20 the player wins; otherwise the game returns to phase 1 (autosave occurs at the start of phase 1)

#### Acceptance Criteria

- Subject-phase actions: claim empty / farm (cost 3, +2 coins/turn) / house (cost 4, +1 subject pool); adjacent to kingdom only
- Task type is automatic from field type; player does not pick task separately
- Hero pool: 3 heroes per turn, refreshed each turn
- Hero acceptance uses offered coins vs hidden coin threshold only (MVP)
- Mission outcome uses hero strength vs enemy strength (5 / 3 / 2); rewards: +3 fame / +5 coins / +1 fame
- Fame ≥ 20 ends the game; otherwise the turn loop continues
- Menu supports new game and load autosave; autosave at start of phase 1

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

- FR-009: Players can play local multiplayer over a local network (transport not pinned to a specific medium). Priority: nice-to-have
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

## Non-Functional Requirements

- Sesja i autosave działają offline (bez wymaganego internetu).
- Akcje UI dają widoczną odpowiedź w odczuwalnie krótkim czasie (orientacyjnie < ~1–2 s).
- MVP działa na Androidzie na telefonach docelowej paczki (bez iOS w MVP).
- Brak kont w chmurze i brak wysyłki danych gry poza urządzenie.

## Business Logic

Gra porównuje monetową nagrodę zlecenia z ukrytym progiem monet oczekiwanym przez bohatera i decyduje, czy bohater przyjmie zlecenie.

Po przyjęciu zlecenia gra wylicza szanse na sukces misji na podstawie siły bohatera i siły wroga, którego ma pokonać.

Wejścia (MVP): przy przyjęciu — liczba monet nagrody oraz ukryty próg monet bohatera (tylko tyle; rozbudowa czynników to nice-to-have). Przy wykonaniu — siła bohatera i siła wroga (potwór 5, wieś 3, obrona własnego pola 2).

Wynik dla gracza: komunikat „przyjęte” / „zignorowane”; dla przyjętych zadań — sukces: potwór +3 sławy, wieś +5 monet (jednorazowo), obrona +1 sława; albo „Bohater poległ w zadaniu”.

Moment w turze: decyzja o przyjęciu zaraz po „Zakończ fazę” w fazie najmu; decyzja o sukcesie misji w fazie trzeciej, zanim wynik zostanie pokazany graczowi.

Balans startowy i ekonomia (MVP): start 10 monet / 0 sławy / 3 poddanych; farma koszt 3 (+2 monety/turę); dom koszt 4 (+1 poddany); 3 bohaterów w puli, odświeżanie co turę; wygrana przy sławie ≥ 20.

## Access Control

Local profile na urządzeniu — bez kont w chmurze / bez klasycznego logowania. Gracz ustawia lokalny profil (np. nick).

Dla MVP Primary wystarczy lokalny profil + start gry jednoosobowej. Host z opcjonalnymi ustawieniami meczu i kodem dołączenia należy do Secondary (obsługa drugiego gracza / lokalna sieć); gdy to dojdzie: poza uprawnieniami hosta do ustawień i kodu model w meczu pozostaje płaski.

## Non-Goals

- Unikać w Primary: multiplayer / lokalna sieć / kod hosta (to Secondary).
- Unikać do czasu działającego Primary: system cech bohaterów i bogatszych skutków misji (FR-015).
- Unikać: PvP — atak wrogich pól i alternatywne zwycięstwo przez podbicie ziem.
- Unikać: iOS, konta w chmurze, wymagany internet.
- Unikać: proceduralna / nieskończona mapa — zostaje wcześniej zdefiniowana mapa.
- Unikać: pełny cykl bohaterów (XP, rany, długi powrót do karczmy).

## Open Questions

None open as of 2026-09-23. Resolved (all Recommended / 5→A1):

1. Fame win threshold: **20**
2. Start: **10 coins / 0 fame / 3 subjects**
3. Farm: cost **3**, **+2 coins/turn**; House: cost **4**, **+1 subject pool**
4. Enemy strength: monster **5**, village **3**, own-field defense **2**
5. Mission success rewards: monster **+3 fame**, village **+5 coins** (once), defense **+1 fame**
6. Hero pool: **3 per turn**, refreshed each turn
