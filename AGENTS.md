# Cursor 에이전트 가이드 (GameHoldOn)

이 저장소는 **Godot 4.6.x (.NET)** + **C#** 을 기본으로 한다. Claude Code 전용 워크플로는 사용하지 않으며, Cursor 채팅·에이전트가 구현을 담당한다.

## 빠른 참조

| 항목 | 위치 |
|------|------|
| Godot 프로젝트 (`res://`) | `src/` (`project.godot`) |
| C# 프로젝트 | `src/GameHoldOn.csproj`, `src/GameHoldOn.sln` |
| 엔진 버전·마이그레이션 | `docs/engine-reference/godot/VERSION.md` |
| 코딩·테스트 기준 | `.claude/docs/coding-standards.md` |
| 설계·협업 | `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md` |

## 로컬 도구

1. **.NET SDK 8 이상** ([다운로드](https://dotnet.microsoft.com/download)) — 터미널에서 `dotnet --version` 으로 확인.
2. **Godot 4.x .NET(Mono) 빌드** — C# 프로젝트에는 GDScript 전용 빌드가 아닌 .NET 버전이 필요하다.
3. Cursor 확장: **Godot Tools** (`geequlim.godot-tools`), **C#**, **C# Dev Kit** — `.vscode/extensions.json` 참고.
4. Godot Tools **Editor Path: Godot4** (`godotTools.editorPath.godot4`)에 위 Godot 실행 파일 경로 지정.

## Godot ↔ Cursor

1. Godot **편집기 → 에디터 설정 → Dotnet → 편집기 → 외부 편집기** 에서 **Visual Studio Code** (Cursor는 VS Code 계열로 동일 연동) 선택.
2. Cursor에서 **`src/GameHoldOn.sln`** 또는 워크스페이스 루트로 폴더 연 뒤 `Main.cs` 를 연다.
3. 터미널: 저장소 루트에서 `dotnet build src/GameHoldOn.csproj` 가 성공하는지 확인.
4. **디버그 (선택)**: 환경 변수 **`GODOT4`** 에 Godot 실행 파일 경로를 설정한 뒤, **실행 및 디버그**에서 **Play (Godot .NET)** 구성 사용 (`.vscode/launch.json`). `program` 을 경로 문자열로 바꿔도 된다.

## 에이전트 동작 원칙

- `src/**/*.cs` 및 Godot API 변경 시 `docs/engine-reference/godot/` 를 먼저 확인한다.
- 사용자가 명시하지 않은 대규모 리팩터·무관한 문서 추가는 하지 않는다.
- 커밋은 사용자가 요청할 때만 수행한다.
