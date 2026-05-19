# ADR-0001: Game State Machine

## Status
Proposed

## Date
2026-05-17

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Godot 4.6 / C# .NET 8 |
| **Domain** | Core |
| **Knowledge Risk** | LOW — 순수 C# enum + switch 로직. Godot 특화 State Machine API 미사용. |
| **References Consulted** | `docs/engine-reference/godot/VERSION.md`, `docs/engine-reference/godot/breaking-changes.md` |
| **Post-Cutoff APIs Used** | 없음 |
| **Verification Required** | 없음 — 엔진 API 비의존 |

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | 없음 (첫 번째 Foundation ADR) |
| **Enables** | ADR-0002 (EventBus), ADR-0003 (Projectile Lifecycle) — 상태 전환 계약이 확정되어야 이벤트 설계 가능 |
| **Blocks** | 모든 게임플레이 Epic의 코딩 시작 — GameState 없이 Main.cs 구현 불가 |
| **Ordering Note** | 이 ADR이 Accepted 되기 전에 Main.cs의 상태 관련 코드를 수정하지 말 것 |

## Context

### Problem Statement
게임에는 Prologue, Playing, Victory, Defeat 4개의 고유한 실행 컨텍스트가 있다. 현재 `Main.cs`에 이 상태들이 암묵적으로 존재하며 명시적 전환 규칙이 없어, 잘못된 입력이나 타이밍 문제로 유효하지 않은 상태 조합이 발생할 수 있다.

### Constraints
- MVP 인디 범위 — 4개 상태 이상의 복잡한 HSM(계층 상태 기계)은 불필요
- 기존 `Main.cs` 구조를 크게 변경하지 않는 방향
- `SceneTree.ReloadCurrentScene()`으로 재시작 가능해야 함 (Godot 4.6 확인됨)
- 테스트 가능성: 상태 전환 로직을 단위 테스트할 수 있어야 함

### Requirements
- Prologue → Playing → Victory/Defeat → 재시작 경로가 유일해야 함
- 주 전환(Week+1)은 상태가 아닌 Playing 내 서브스텝
- 모든 시스템이 현재 게임 상태를 조회할 수 있어야 함 (읽기만)
- 오직 GameStateModule(Main.cs)만 상태를 변경할 수 있음

## Decision

`GameState` C# enum과 `Main.cs`의 `SetState(GameState)` 메서드로 구현하는 **Enum-based FSM**을 채택한다.

```csharp
// GameState.cs (또는 Main.cs 내 선언)
public enum GameState {
    Prologue,
    Playing,
    Victory,
    Defeat
}

// Main.cs — GameStateModule 역할
public partial class Main : Node {
    private GameState _state = GameState.Prologue;
    public  GameState CurrentState => _state;

    private void SetState(GameState next) {
        if (_state == next) return;
        OnExitState(_state);
        _state = next;
        OnEnterState(_state);
        // 상태 변경 후 이벤트 발화는 ADR-0002 확정 후 추가
    }

    private void OnEnterState(GameState s) {
        switch (s) {
            case GameState.Prologue:  StartPrologue(); break;
            case GameState.Playing:   StartPlaying();  break;
            case GameState.Victory:   ShowVictory();   break;
            case GameState.Defeat:    ShowDefeat();    break;
        }
    }

    private void OnExitState(GameState s) {
        switch (s) {
            case GameState.Playing: StopSpawn(); break;
            // 기타 정리 로직
        }
    }
}
```

### State Transition Diagram

```
              PrologueComplete
  ┌────────┐ ─────────────────► ┌─────────┐
  │Prologue│                    │ Playing │
  └────────┘                    └─────────┘
      ▲                          │       │
      │        HP = 0            ▼       │ Week 8
      │    ┌──────────┐    ┌─────────┐   │
      └────┤  Defeat  │    │ Victory │◄──┘
      │    └──────────┘    └─────────┘
      │       Restart            │
      └──────────────────────────┘
```

**유효 전환표:**

| From | To | Trigger |
|------|-----|---------|
| Prologue | Playing | `GameEvents.PrologueComplete` |
| Playing | Victory | Week 8 타이머 만료 |
| Playing | Defeat | 플레이어 HP ≤ 0 |
| Victory | Prologue | R / Enter / Space 입력 |
| Defeat | Prologue | R / Enter / Space 입력 |

**유효하지 않은 전환** (SetState 내에서 무시):
- Prologue → Victory / Defeat (직접 전환 없음)
- Victory → Defeat / Victory (동일 상태 재진입)
- Defeat → Victory (직접 전환 없음)

### 주 전환(Week+1)은 상태가 아님

Week 타이머 만료 시 `Playing` 상태를 유지하면서 내부 서브스텝 실행:
1. 화면 내 적 페이드아웃 (0.3s Tween, `await`)
2. `currentWeek++`
3. Week 8 초과 시 `SetState(Victory)` 호출
4. 아니면 스폰 파라미터 업데이트 후 계속 진행

QQ-03 해소: Tween await는 `Main.cs`의 `async Task` 메서드 또는 Godot `await ToSignal(tween, "finished")` 패턴 사용. 별도 상태 불필요.

### Key Interfaces

```csharp
// 읽기: 어느 모듈에서나 가능
GameState state = mainNode.CurrentState;

// 쓰기: Main.cs (GameStateModule) 독점
// 타 모듈은 SetState()를 직접 호출하지 않음
// → GameEvents를 통해 조건을 알리면 Main이 판단 후 SetState() 호출

// 재시작
public void RestartGame() {
    GetTree().ReloadCurrentScene();  // XP·레벨·Week 모두 초기화
}
```

## Alternatives Considered

### Alternative 1: Godot Scene-per-State
- **Description**: 각 상태를 별도 `.tscn`으로 구현. `SceneTree.ChangeSceneToFile()` 로 전환.
- **Pros**: 상태 간 완전한 격리, 씬 에디터에서 시각적 확인 가능
- **Cons**: Prologue는 이미 별도 씬이나 Playing/Victory/Defeat는 같은 게임 씬 위 오버레이. 씬 전환 시 엔티티 상태 유실, 적 페이드아웃 같은 전환 연출 구현 어려움.
- **Rejection Reason**: 4개 상태 중 3개가 같은 게임 월드를 공유 — 씬 분리 실익 없음.

### Alternative 2: State Pattern (C# 클래스 분리)
- **Description**: 추상 `GameState` 기반 클래스 + `PrologueState`, `PlayingState` 등 구체 클래스.
- **Pros**: 상태별 로직 파일 분리, OCP(개방-폐쇄 원칙) 준수
- **Cons**: 4개 상태에 6개 파일 추가. 상태 간 공유 데이터(스폰, 적 등) 접근을 위한 컨텍스트 전달 복잡도 증가.
- **Rejection Reason**: 인디 MVP 범위 초과. Enum FSM이 동일 보증을 훨씬 적은 코드로 제공.

## Consequences

### Positive
- 상태 전환이 코드베이스 한 곳(SetState)에서만 발생 → 디버깅 용이
- `CurrentState` 프로퍼티로 어느 시스템에서나 현재 상태 조회 가능
- 유효하지 않은 전환을 SetState 내에서 무시할 수 있음
- 단위 테스트 용이 (Godot 의존 없음)

### Negative
- Main.cs가 OnEnterState/OnExitState 내에서 여러 시스템 참조 — 결합도 존재
- 상태가 5개 이상으로 늘어나면 switch 블록이 길어짐

### Risks
- **리스크**: Playing 상태 진입 시 스폰이 시작되기 전에 입력이 처리되는 프레임 존재 가능  
  **완화**: OnEnterState에서 스폰 시작을 첫 번째 액션으로 처리

- **리스크**: `ReloadCurrentScene()`이 모든 서브시스템을 완전히 초기화한다고 가정  
  **완화**: 씬 로드 후 첫 `_Ready()` 호출에서 상태 검증 로그 추가

## GDD Requirements Addressed

| TR ID | 요구사항 | 해결 방법 |
|-------|---------|---------|
| TR-CORE-006 | Week 8 완료 시 승리 | Playing → Victory 전환 규칙으로 커버 |
| TR-CORE-007 | 플레이어 HP 0 시 패배 | Playing → Defeat 전환 규칙으로 커버 |
| TR-CORE-009 | R/Enter/Space 재시작 | Victory/Defeat 상태에서만 입력 유효, RestartGame() 호출 |
| TR-CORE-011 | 프롤로그 텍스트 화면 (스킵 가능) | Prologue 상태가 PrologueModule 활성화. 스킵 입력 → PrologueComplete |
| TR-CORE-005 | 주 전환: 적 페이드아웃 → 신규 스폰 | Playing 내 서브스텝으로 처리 (별도 상태 없음) |

## Performance Implications
- **CPU**: 무시 가능 — switch/case 분기, 프레임당 1회 이하 실행
- **Memory**: GameState enum 4바이트
- **Load Time**: 해당 없음
- **Network**: 해당 없음 (싱글플레이어)

## Migration Plan

`Main.cs` 현재 상태:
1. 암묵적 상태 플래그(`isGameOver`, `isVictory` 등)를 `GameState` enum으로 교체
2. 분산된 조건 분기를 `OnEnterState`/`OnExitState` switch 블록으로 통합
3. HudModule이 상태 변경을 직접 감지하는 대신 GameEvents 구독으로 전환 (ADR-0002 이후)

## Validation Criteria
- [ ] Prologue → Playing → Victory 경로를 게임 실행 후 확인
- [ ] Prologue → Playing → Defeat → Prologue 경로 확인
- [ ] Victory 상태에서 R 키로 재시작 시 XP·레벨·Week 모두 0 초기화 확인
- [ ] 유효하지 않은 전환(Playing → Playing)이 무시됨 확인

## Related Decisions
- ADR-0002: EventBus — GameEvents 이벤트가 SetState 호출의 트리거
- `design/gdd/gdd-office-layoff-survivor.md` §3, §5, §8, §9
