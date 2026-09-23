---
name: test-me
description: >
  Ask one interactive multiple-choice question about the current project,
  grounded in context/ foundation docs. Use when the user says "TestMe",
  "test me", "quiz me", "sprawdź mnie", or wants a quick project comprehension check.
disable-model-invocation: true
---

# TestMe

Quiz the user with **one** project-related question drawn from `context/`. Collect the answer via the interactive **AskQuestion** tool (not a plain chat prompt).

## When to use

- User invokes `/test-me` or mentions TestMe / "test me" / "quiz me" / "sprawdź mnie".
- Skip when `context/` is missing or empty of readable foundation docs — tell the user to add or generate foundation context first.

## Workflow

### 1. Load project context

Read available files under `context/` (prefer `context/foundation/` first: `prd.md`, `shape-notes.md`, `stack-decision.md`, `tech-stack.md`, and any other foundation markdown). Skim enough to ground a real question — do not invent product facts.

If no useful foundation content exists, stop and say so. Do not ask a generic unrelated question.

### 2. Craft one question

Build a single multiple-choice question that checks understanding of this project (vision, persona, MVP scope, constraints, stack choice, non-goals, etc.).

Rules:
- Exactly **one** question this run.
- **3–4 options**, one clearly best answer grounded in the docs; plausible distractors allowed.
- Keep the prompt short; cite the topic, not file paths, in the question text.
- Prefer Polish if the foundation docs are primarily in Polish; otherwise match the docs' language.

### 3. Ask interactively with AskQuestion

**Required:** use the **AskQuestion** tool so the user picks an option in the UI. Do not paste the quiz as a normal chat message when AskQuestion is available.

Call AskQuestion with approximately this shape:

- `title`: `TestMe`
- `questions`: one item:
  - `id`: `testme-q1`
  - `prompt`: the crafted question
  - `options`: 3–4 entries with stable `id` values (`a`, `b`, `c`, …) and clear `label` text
  - `allowMultiple`: `false`

Wait for the tool result before continuing.

**Fallback:** if AskQuestion is not available in this session, ask the same question conversationally with labeled options (A/B/C/…) and wait for the user's reply.

### 4. Respond to the answer

After the user answers (or skips/cancels):

- **Answered:** say whether it matches the docs (correct / partially / incorrect) in 1–3 short sentences, citing the relevant idea from `context/` (no lecture).
- **Skipped / cancelled:** acknowledge briefly and stop — do not re-ask unless they invoke TestMe again.

## Guardrails

1. One question per invocation — no multi-round quiz unless the user asks again.
2. Ground every question in `context/` — never quiz on unrelated general knowledge.
3. Prefer AskQuestion over chat for collecting the choice.
4. Do not modify project files; this skill is read-and-ask only.
