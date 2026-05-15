# Cursor 에이전트 가이드 (GameHoldOn)

이 저장소는 **Godot 4.6** + **GDScript** 를 전제로 한다. Claude Code 전용 워크플로는 사용하지 않으며, Cursor 채팅·에이전트가 구현을 담당한다.

## 빠른 참조

| 항목 | 위치 |
|------|------|
| Godot 프로젝트 (`res://`) | `src/project.godot` |
| 엔진 버전·마이그레이션 | `docs/engine-reference/godot/VERSION.md` |
| 코딩·테스트 기준 | `.claude/docs/coding-standards.md` |
| 설계·협업 | `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md` |

## 로컬 도구

1. [Godot 4.6](https://godotengine.org/download) 설치.
2. Cursor에서 확장 **Godot Tools** (`geequlim.godot-tools`) 설치 권장 — `.vscode/extensions.json` 참고.
3. Godot Tools 설정에서 **Editor path** 를 설치한 Godot 실행 파일로 지정하면 LSP·디버그가 안정적이다.  
   (저장소의 `.vscode/settings.json` 은 gitignore 되어 있으므로, 필요하면 워크스페이스에 로컬로 생성한다.)

## 에이전트 동작 원칙

- `src/` 이하 GDScript·장면 변경 시 `docs/engine-reference/godot/` 를 먼저 확인한다.
- 사용자가 명시하지 않은 대규모 리팩터·무관한 문서 추가는 하지 않는다.
- 커밋은 사용자가 요청할 때만 수행한다.
