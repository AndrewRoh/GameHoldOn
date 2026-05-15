# Claude Code Game Studios -- Game Studio Agent Architecture

Indie game development managed through 49 coordinated Claude Code subagents.
Each agent owns a specific domain, enforcing separation of concerns and quality.

## Technology Stack

- **Engine**: Godot **4.6.x** (.NET) — project in `src/` (`src/project.godot`, `GameHoldOn.csproj`)
- **Language**: **C#** (Godot .NET); GDScript optional for small/tooling scripts
- **Version Control**: Git with trunk-based development
- **Build System**: `dotnet build` on `src/GameHoldOn.csproj` + Godot editor export (`export_presets.cfg` local; gitignored by default)
- **Asset Pipeline**: Godot `res://` under `src/`; binary art in `assets/` (see `.gitignore` for LFS notes)

> **Cursor**: Day-to-day development uses Cursor; see root `AGENTS.md` and `.cursor/rules/`.

> **Note**: Engine-specialist agents exist for Godot, Unity, and Unreal with
> dedicated sub-specialists. Use the set matching your engine.

## Project Structure

@.claude/docs/directory-structure.md

## Engine Version Reference

@docs/engine-reference/godot/VERSION.md

## Technical Preferences

@.claude/docs/technical-preferences.md

## Coordination Rules

@.claude/docs/coordination-rules.md

## Collaboration Protocol

**User-driven collaboration, not autonomous execution.**
Every task follows: **Question -> Options -> Decision -> Draft -> Approval**

- Agents MUST ask "May I write this to [filepath]?" before using Write/Edit tools
- Agents MUST show drafts or summaries before requesting approval
- Multi-file changes require explicit approval for the full changeset
- No commits without user instruction

See `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md` for full protocol and examples.

> **First session?** If the project has no engine configured and no game concept,
> run `/start` to begin the guided onboarding flow.

## Coding Standards

@.claude/docs/coding-standards.md

## Context Management

@.claude/docs/context-management.md
