---
project: "Grodowładcy"
version: 1
status: draft
created: 2026-10-11
updated: 2026-10-11
prd_version: 1
main_goal: learn
top_blocker: capacity
milestone_id: first-playable-turn
milestone_seq: 1
milestone_status: open
---

# Roadmap: Grodowładcy

> Wynika z `context/foundation/prd.md` i z przeglądu kodu.
> Poprawiaj w tym pliku; gdy powstanie nowa wersja, starą przenieś do archiwum.
> Plasterki są w kolejności zależności. Tabela „At a glance” jest spisem.

## Milestone

**M-1: Pierwsza grywalna tura** — Status: open

- **Intent:** Gracz jednoosobowy kończy jedną turę na ustalonej mapie — od poddanych, przez ogłoszenie, do przyjęcia albo odmowy i wyniku misji — a potem może wrócić do zapisanej sesji i grać dalej aż do wygranej sławą. Kamień milowy dowozi sesję z głównych kryteriów sukcesu; drugi gracz zostaje poza nim.
- **Source materials:** `context/foundation/prd.md` (v1)
- **Done when:** gotowe, gdy każdy S-NN poniżej ma status `done`.
- **Scope anchors:** FR-001, FR-002, FR-003, FR-004, FR-005, FR-006, FR-007, FR-008, FR-014, US-01

## Vision recap

Krótka, lokalna gra na telefony dla Magdy i stałej paczki przyjaciół: wspólny projekt do kreatywnego rozwijania, nie lekarstwo na nudę w drodze. Sednem jest krótka strategia, w której gracz przekonuje niezależnych bohaterów, na jednym urządzeniu i bez kont w chmurze. Świadome ryzyko: paczka może woleć spać, niż grać.

## North star

„North star” to najmniejszy przebieg od początku do końca, którego ukończenie pokazuje, że sedno gry działa. Stoi tak wcześnie, jak pozwalają zależności, bo reszta sesji ma sens tylko wtedy, gdy ten przebieg działa.

**S-04: Gracz kończy pierwszą turę i widzi wynik zlecenia** — ta tura jest pierwszym dowodem, że przekonywanie bohaterów w ogóle daje się zagrać. Cel nauki stawia ją przed wygraną sławą: najpierw ekran, mapa i fazy, dopiero potem próg zwycięstwa.

## At a glance

| ID   | Change ID                     | Outcome (user can …)                                                                                                      | Prerequisites | PRD refs                         | Status   |
| ---- | ----------------------------- | ------------------------------------------------------------------------------------------------------------------------- | ------------- | -------------------------------- | -------- |
| S-01 | start-on-visible-map          | gracz może nadać lokalną nazwę, rozpocząć nową grę i zobaczyć mapę heksów ze stolicą, monetami, sławą i pulą poddanych   | —             | US-01, FR-001, FR-008            | ready    |
| S-02 | plan-subject-action           | gracz może wybrać sąsiednie pole i zaplanować zajęcie, farmę albo dom, a potem zakończyć fazę poddanych                  | S-01          | US-01, FR-003, FR-004, FR-005    | proposed |
| S-03 | post-hero-announcement        | gracz może przejrzeć trzech bohaterów i wystawić ogłoszenie z polem oraz nagrodą w monetach                              | S-02          | US-01, FR-005, FR-006, FR-007    | blocked  |
| S-04 | resolve-first-turn            | gracz może zakończyć turę i zobaczyć, czy bohater przyjął zlecenie oraz czy misja się udała, razem ze skutkiem planów poddanych | S-03          | US-01, FR-005, FR-014            | blocked  |
| S-05 | repeat-turns-until-fame-win   | gracz może powtarzać tury i wygrać, gdy sława osiągnie próg                                                              | S-04          | US-01, FR-005                    | proposed |
| S-06 | load-phase-one-autosave       | gracz może wyjść z sesji i wczytać ją z menu                                                                             | S-01          | US-01, FR-002                    | proposed |

## Streams

Pomoc przy czytaniu: grupuje plasterki o wspólnym łańcuchu zależności. Kolejność wiążąca jest niżej, w grafie zależności. Ta tabela to proponowana kolejność czytania równoległych torów.

| Stream | Theme            | Chain                                              | Note                                                                                          |
| ------ | ---------------- | -------------------------------------------------- | --------------------------------------------------------------------------------------------- |
| A      | Pętla tury       | `S-01` → `S-02` → `S-03` → `S-04` → `S-05`         | Nauka: najpierw widoczna plansza, potem pierwsza domknięta tura, a wygrana sławą dopiero za nią. |
| B      | Powrót do sesji  | `S-06`                                             | Dołącza do strumienia A przy `S-01` i może iść obok `S-02`.                                   |

## Baseline

Co już jest w kodzie na 2026-10-11 (sprawdzone w repozytorium i potwierdzone).
Fundamenty poniżej zakładają, że te warstwy już są, i nie stawiają ich od nowa.

- **Ekran:** częściowo — silnik rysuje pustą scenę (kamera, światło, sprite pola); pakiet interfejsu jest podłączony, ale nie ma menu, zasobów na ekranie ani mapy do wybrania (`Assets/Scenes/SampleScene.unity`, `Packages/manifest.json`)
- **Serwer:** brak — nie ma serwera ani sieci; reguły przyjęcia zlecenia, wyniku misji i wygranej są lokalne (`Assets/Scripts/Runtime/MissionResolver.cs`, `HeroAssignment.cs`, `WinCondition.cs`)
- **Dane:** częściowo — stałe balansu są w kodzie (`Assets/Scripts/Runtime/GameConstants.cs`); brak zapisu sesji do pliku; wczytanie jest tylko w dokumencie produktu (FR-002)
- **Tożsamość:** brak — nie ma profilu, sesji ani logowania w kodzie
- **Wydanie:** częściowo — da się złożyć paczkę na telefon i opublikować ją z edytora (`Assets/Editor/CiBuild.cs`, `Assets/Editor/GhReleaseWindow.cs`); brak automatycznego składania przy każdej zmianie
- **Obserwowalność:** częściowo — komunikaty tylko w narzędziu składania paczki (`Assets/Editor/CiBuild.cs`); brak śledzenia błędów u gracza

## Foundations

Brak. Reguły przyjęcia zlecenia, wyniku misji i progu sławy już są w kodzie (patrz Baseline). Pierwszy ekran, na którym da się zagrać, wchodzi w S-01. Osobny fundament „cały ekran” dokończyłby warstwę, zanim powstanie jakakolwiek tura.

## Slices

### S-01: Widoczna plansza nowej gry

- **Outcome:** gracz może nadać lokalną nazwę, rozpocząć nową grę i zobaczyć mapę heksów ze stolicą, monetami, sławą i pulą poddanych
- **Change ID:** start-on-visible-map
- **PRD refs:** US-01, FR-001, FR-008
- **Prerequisites:** —
- **Parallel with:** —
- **Blockers:** —
- **Unknowns:**
  - Jak dokładnie ułożyć heksy poza minimum ze źródła (stolica, puste pola, neutralna wieś, potwór)? — Owner: user. Block: no.
- **Risk:** To pierwsza nieznana warstwa: widoczna plansza. Bez niej nie ma czego dotykać, a reguły rozstrzygnięć już są i nie powinny wyprzedzać ekranu.
- **Status:** ready

### S-02: Plan akcji poddanych

- **Outcome:** gracz może wybrać sąsiednie pole i zaplanować zajęcie, farmę albo dom, a potem zakończyć fazę poddanych
- **Change ID:** plan-subject-action
- **PRD refs:** US-01, FR-003, FR-004, FR-005
- **Prerequisites:** S-01
- **Parallel with:** S-06
- **Blockers:** —
- **Unknowns:** —
- **Risk:** Wybór jest widoczny od razu, a farma, dom i zajęte pole pojawiają się na mapie dopiero przy rozstrzygnięciu tury, bo tak źródło ustawia kolejność faz.
- **Status:** proposed

### S-03: Ogłoszenie dla bohatera

- **Outcome:** gracz może przejrzeć trzech bohaterów i wystawić ogłoszenie z polem oraz nagrodą w monetach
- **Change ID:** post-hero-announcement
- **PRD refs:** US-01, FR-005, FR-006, FR-007
- **Prerequisites:** S-02
- **Parallel with:** —
- **Blockers:** —
- **Unknowns:**
  - Jakie publiczne statystyki, ukryty próg monet i siła mają trzej bohaterowie w puli każdej tury? — Owner: user. Block: yes.
- **Risk:** Lista bohaterów bez tych liczb byłaby zgadywaniem. Typ zadania i tak wynika z pola, więc po ustaleniu liczb plasterek jest wąski.
- **Status:** blocked

### S-04: Rozstrzygnięcie pierwszej tury

- **Outcome:** gracz może zakończyć turę i zobaczyć, czy bohater przyjął zlecenie oraz czy misja się udała, razem ze skutkiem planów poddanych
- **Change ID:** resolve-first-turn
- **PRD refs:** US-01, FR-005, FR-014
- **Prerequisites:** S-03
- **Parallel with:** —
- **Blockers:** —
- **Unknowns:**
  - Jakie publiczne statystyki, ukryty próg monet i siła mają trzej bohaterowie w puli każdej tury? — Owner: user. Block: yes.
- **Risk:** To domknięcie pierwszej tury i stoi najwcześniej, jak pozwalają fazy. Przyjęcie zlecenia i wynik misji są w źródle jedną fazą, więc nie rozdzielam ich na dwa plasterki.
- **Status:** blocked

### S-05: Kolejne tury aż do wygranej

- **Outcome:** gracz może powtarzać tury i wygrać, gdy sława osiągnie próg
- **Change ID:** repeat-turns-until-fame-win
- **PRD refs:** US-01, FR-005
- **Prerequisites:** S-04
- **Parallel with:** —
- **Blockers:** —
- **Unknowns:** —
- **Risk:** Próg sławy nie pokazuje, czy sedno gry działa, dopóki jedna tura nie zmienia sławy. Dlatego wygrana idzie za domknięciem pierwszej tury.
- **Status:** proposed

### S-06: Wczytanie zapisanej sesji

- **Outcome:** gracz może wyjść z sesji i wczytać ją z menu
- **Change ID:** load-phase-one-autosave
- **PRD refs:** US-01, FR-002
- **Prerequisites:** S-01
- **Parallel with:** S-02
- **Blockers:** —
- **Unknowns:** —
- **Risk:** Zapis potrzebuje tylko rozpoczętej sesji, więc może iść obok fazy poddanych. Przy małej liczbie godzin nie wyprzedza pierwszej grywalnej tury.
- **Status:** proposed

## Backlog Handoff

| Roadmap ID | Change ID                   | Suggested issue title                                              | Ready for `/10x-plan` | Notes                                      |
| ---------- | --------------------------- | ------------------------------------------------------------------ | --------------------- | ------------------------------------------ |
| S-01       | start-on-visible-map        | Gracz zaczyna grę i widzi mapę oraz zasoby                         | yes                   | Uruchom `/10x-plan start-on-visible-map`   |
| S-02       | plan-subject-action         | Gracz planuje akcję poddanych na sąsiednim polu                    | no                    | Czeka na S-01; potem obok S-06             |
| S-03       | post-hero-announcement      | Gracz przegląda bohaterów i wystawia ogłoszenie                    | no                    | Blokuje brak liczb bohaterów               |
| S-04       | resolve-first-turn          | Gracz rozstrzyga pierwszą turę                                     | no                    | Blokuje brak liczb bohaterów               |
| S-05       | repeat-turns-until-fame-win | Gracz powtarza tury aż do wygranej sławą                           | no                    | Czeka na S-04                              |
| S-06       | load-phase-one-autosave     | Gracz wczytuje zapisaną sesję z menu                               | no                    | Czeka na S-01; potem obok S-02             |

## Open Roadmap Questions

1. **Jakie publiczne statystyki, ukryty próg monet i siła mają trzej bohaterowie w puli każdej tury?** — Owner: user. Block: S-03, S-04.

## Parked

- **Gra wieloosobowa, sieć lokalna, kod hosta** — Odłożone, bo dokument produktu wynosi to poza zakres; wraca po działającej sesji jednoosobowej (FR-009).
- **Cechy bohaterów i bogatsze skutki misji** — Odłożone, bo dokument produktu wynosi to poza zakres; FR-015 zostaje poza sesją jednoosobową.
- **Atak pól wroga i wygrana przez podbicie ziem** — Odłożone, bo dokument produktu wynosi to poza zakres; FR-012 i FR-013 czekają na grę wieloosobową.
- **Bogatsze karty bohaterów** — Odłożone; FR-011 jest poza sesją jednoosobową.
- **Więcej typów pól** — Odłożone; FR-010 jest poza sesją jednoosobową.
- **iOS, konta w chmurze, wymagany internet** — Odłożone, bo dokument produktu wynosi to poza zakres.
- **Mapa proceduralna albo nieskończona** — Odłożone, bo dokument produktu wynosi to poza zakres; zostaje wcześniej ustalona mapa.
- **Pełny cykl bohatera (doświadczenie, rany, powrót do karczmy)** — Odłożone, bo dokument produktu wynosi to poza zakres.

## Milestone History

(Pusto. To pierwszy kamień milowy.)

## Done

(Pusto. Nic jeszcze nie zarchiwizowano.)
