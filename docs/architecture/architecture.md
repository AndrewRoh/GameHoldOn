# Office Layoff Survivor — Master Architecture

## Document Status

| Field | Value |
|-------|-------|
| Version | 1.0 |
| Last Updated | 2026-05-17 |
| Engine | Godot 4.6 / C# .NET 8 |
| GDDs Covered | gdd-office-layoff-survivor, gdd-combat-survival, gdd-meta-progression, gdd-visual-audio |
| ADRs Referenced | None yet — see Required ADRs section |
| Technical Director Sign-Off | 2026-05-17 — APPROVED |
| Lead Programmer Feasibility | Skipped — Lean mode |

---

## Engine Knowledge Gap Summary

**LLM training covers Godot ~4.3. Post-cutoff versions (4.4–4.6) introduced changes.**

| Risk | Domain | Impact on This Project |
|------|--------|----------------------|
| ⚠️ HIGH | Rendering | D3D12 기본값(Windows), Glow 처리 순서 변경 — VFX 튜닝 시 실기기 확인 필요 |
| ⚠️ HIGH | UI | 4.6 Dual-focus: 마우스 포커스 ≠ 키보드 포커스 — HUD focus 동작 검증 필요 |
| LOW | 2D Physics | CharacterBody2D / Area2D — 변경 없음, 신뢰 가능 |
| LOW | Input | Input.GetVector, InputEvent — 변경 없음 |
| LOW | C# .NET 8 | Godot 바인딩 안정 |

**원칙:** Rendering·UI 코드 작성 시 `docs/engine-reference/godot/modules/rendering.md`, `ui.md` 필수 교차 확인.

---

## Technical Requirements Baseline

26개 요구사항 | GDD 4개

| Req ID | 요구사항 | 도메인 | 담당 모듈 |
|--------|---------|--------|---------|
| TR-CORE-001 | 탑다운 2D 카메라, 플레이어 추적 | Camera | PlayerModule |
| TR-CORE-002 | WASD 플레이어 이동 | Input | PlayerModule |
| TR-CORE-003 | 자동 공격 — 최근접 적 조준 투사체 | Combat | PlayerModule |
| TR-CORE-004 | 주 타이머 45s, Week 카운터 | Timer | WeekManagerModule |
| TR-CORE-005 | 주 전환: 적 페이드아웃(0.3s) → 신규 스폰 | State | WeekManagerModule |
| TR-CORE-006 | Week 8 완료 시 승리 | State | WeekManagerModule |
| TR-CORE-007 | 플레이어 HP 0 시 패배 | State | GameStateModule |
| TR-CORE-008 | 주차별 스폰 간격·HP·속도 배율 스케일링 | Balance | SpawnModule |
| TR-CORE-009 | R/Enter/Space 재시작 | Input | GameStateModule |
| TR-CORE-010 | 플레이어 이동 속도 220 px/s | Physics | PlayerModule |
| TR-CORE-011 | 프롤로그 텍스트 화면 (스킵 가능) | UI | PrologueModule |
| TR-COMBAT-001 | 플레이어 HP 100, 적 접촉 시 피해 | Combat | PlayerModule |
| TR-COMBAT-002 | 발사 쿨다운 0.28s (레벨업으로 감소) | Combat | PlayerModule |
| TR-COMBAT-003 | 스폰 링 반경 520px | Spawn | SpawnModule |
| TR-COMBAT-004 | HR/CEO/CTO 3종, 종별 HP·속도·가중치 | Spawn | SpawnModule + EnemyModule |
| TR-COMBAT-005 | 투사체: 직선, 수명 2.2s, 충돌 반경 22px | Physics | ProjectileModule |
| TR-COMBAT-006 | 플레이어 충돌 반경 14px (고정) | Physics | PlayerModule |
| TR-COMBAT-007 | 최근접 적 단일 조준; 적 0명 시 미발사 | Combat | PlayerModule |
| TR-META-001 | XP 지급: HR=6, CEO=10, CTO=8 | Progression | EnemyModule → GameEvents |
| TR-META-002 | 레벨업 임계치: 60+(lv-1)×40 | Progression | MetaProgressionModule |
| TR-META-003 | 레벨업: 발사 간격 자동 단축 (선택 UI 없음) | Progression | MetaProgressionModule |
| TR-META-004 | 레벨업: 피해 배율 자동 증가 | Progression | MetaProgressionModule |
| TR-META-005 | 재시작 시 XP·레벨 초기화 | State | MetaProgressionModule |
| TR-VIS-001 | Sprite2D 교체, 충돌 형상 분리 유지 | Rendering | EnemyModule + PlayerModule |
| TR-VIS-002 | hit/death/player-hit VFX 3종 | VFX | VfxModule |
| TR-VIS-003 | 4K viewport stretch canvas_items | Rendering | 프로젝트 설정 |

---

## System Layer Map

```
┌──────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER                                          │
│  HudModule · VfxModule · AudioModule(stub) · PrologueModule  │
├──────────────────────────────────────────────────────────────┤
│  FEATURE LAYER                                               │
│  MetaProgressionModule · WeekManagerModule · SpawnModule     │
├──────────────────────────────────────────────────────────────┤
│  CORE LAYER                                                  │
│  PlayerModule · EnemyModule · ProjectileModule               │
├──────────────────────────────────────────────────────────────┤
│  FOUNDATION LAYER                                            │
│  GameStateModule · GameEvents(EventBus) · GameBalance        │
│  ArtPaths                                                    │
├──────────────────────────────────────────────────────────────┤
│  PLATFORM LAYER  (Godot 4.6 / .NET 8)                        │
│  CharacterBody2D · Area2D · Sprite2D · Camera2D · CanvasLayer│
│  ⚠️ Rendering HIGH · ⚠️ UI MEDIUM                           │
└──────────────────────────────────────────────────────────────┘
```

**계층 간 의존 방향: 위 → 아래만 허용. 역방향 직접 참조 금지.**  
예외: 이벤트(GameEvents)는 어느 계층에서든 구독 가능.

---

## Module Ownership

### Foundation Layer

| 모듈 | 파일 | Owns | Exposes |
|------|------|------|---------|
| GameEvents | `GameEvents.cs` (신규) | 이벤트 선언 및 Raise 권한 | `static event` 필드 (어디서나 구독 가능) |
| GameStateModule | `Main.cs` (일부) | 게임 FSM 상태 (Prologue/Playing/Victory/Defeat) | `SetState()`, `RestartGame()` |
| GameBalance | `GameBalance.cs` (기존) | 모든 수치 상수 | `static readonly` 필드 |
| ArtPaths | `ArtPaths.cs` (기존) | 에셋 경로 상수 | `static readonly string` 필드 |

### Core Layer

| 모듈 | 파일 | Owns | Exposes | Engine API |
|------|------|------|---------|-----------|
| PlayerModule | `Player.cs` | 위치, 속도, HP, fireTimer, FireInterval, DamageMultiplier | `Transform2D`, `ApplyLevelStats()`, `IDamageable` | `CharacterBody2D.MoveAndSlide()` [LOW] |
| EnemyModule | `Enemy.cs` | per-enemy HP, speed, BossKind | `IDamageable.TakeDamage()` | `CharacterBody2D.MoveAndSlide()` [LOW] |
| ProjectileModule | `Projectile.cs` | velocity, lifetime, hitRadius | 없음 (Area2D 충돌로 EnemyModule 직접 접근) | `Area2D.BodyEntered` [LOW] |

### Feature Layer

| 모듈 | 파일 | Owns | Fires / Consumes |
|------|------|------|-----------------|
| WeekManagerModule | `Main.cs` (일부) | currentWeek, weekTimer | Fires: `WeekChanged`, `GameWon` / Consumes: `Timer.Timeout` |
| SpawnModule | `Main.cs` → `SpawnManager.cs` (분리 권장) | spawnTimer, 가중치 선택 | Consumes: `WeekChanged`, `GameBalance`, `Player.Transform2D` |
| MetaProgressionModule | `MetaProgression.cs` (신규) | currentXp, currentLevel | Fires: `LevelChanged` / Consumes: `EnemyKilled` |

### Presentation Layer

| 모듈 | 파일 | Owns | Consumes |
|------|------|------|---------|
| HudModule | `GameHud.cs` | 화면 표시 캐시 | `GameEvents` 구독 (HP, XP, Week, Level) ⚠️ Dual-focus 4.6 |
| VfxModule | *(Main.cs 내 인라인 → 분리 권장)* | Tween/파티클 생명주기 | `EnemyKilled`, `PlayerDamaged` |
| PrologueModule | `Prologue.cs` | 프롤로그 씬 상태 | 입력 → Fires: `PrologueComplete` |
| AudioModule | *(stub)* | BGM/SFX 재생 상태 | `WeekChanged`, `EnemyKilled` |

---

## Data Flow

### ① 프레임 업데이트 (Input → Physics → Render)

```
Input.GetVector("move_left","move_right","move_up","move_down")
  → Player.Velocity 계산
  → CharacterBody2D.MoveAndSlide()
  → Camera2D 자동 추적 (built-in, 별도 코드 불필요)
```

### ② 전투 루프 (Auto-attack → 피해 → 사망)

```
Player._PhysicsProcess(delta)
  → GetTree().GetNodesInGroup("enemies") → FindNearest(Vector2)
  → fireTimer >= FireInterval → Instantiate(ProjectilePrefab) at Player.Position
     → Projectile._PhysicsProcess: Position += Direction * Speed * delta
        → Area2D.BodyEntered(body)
           → body is IDamageable → TakeDamage(Damage * DamageMultiplier)
              → Enemy.HP -= damage
              → if HP <= 0:
                   GameEvents.RaiseEnemyKilled(BossKind, XpReward)
                   QueueFree()
```

### ③ 진행 루프 (XP → 레벨업 → 스탯 반영)

```
GameEvents.EnemyKilled (구독: MetaProgressionModule)
  → AddXp(xpAmount)
  → cumulativeXp += xp
  → while cumulativeXp >= XpToNext(currentLevel):
       cumulativeXp -= XpToNext(currentLevel)
       currentLevel++
       newFireCd  = Max(0.14f, 0.28f - currentLevel * 0.02f)
       newDmgMult = 1f + currentLevel * 0.08f
       GameEvents.RaiseLevelChanged(currentLevel, newFireCd, newDmgMult)
          → Player.ApplyLevelStats(fireCd, dmgMult)   [동기, 중단 없음]
          → HudModule.UpdateLevel(currentLevel)
```

### ④ 주 전환 (타이머 → 페이드 → 스케일링)

```
WeekTimer.Timeout
  → foreach enemy in Group("enemies"):
       Tween.TweenProperty(enemy, "modulate:a", 0f, 0.3f)
            .SetTrans(Tween.TransitionType.Linear)
       await tween.Finished → enemy.QueueFree()
  → currentWeek++
  → if currentWeek > GameBalance.TotalWeeks:
       GameEvents.RaiseGameWon()
  → else:
       GameEvents.RaiseWeekChanged(currentWeek)
          → SpawnModule.UpdateParams(currentWeek)
          → HudModule.UpdateWeek(currentWeek)
       WeekTimer.Start(GameBalance.WeekDurationSec)
```

### ⑤ 게임 상태 전환 (FSM)

```
States: Prologue → Playing → Victory | Defeat

Prologue:
  PrologueModule input → GameEvents.PrologueComplete
  → GameStateModule.SetState(Playing) → SpawnModule.Start()

Playing:
  GameEvents.PlayerDied → SetState(Defeat) → HudModule.ShowDefeat()
  GameEvents.GameWon    → SetState(Victory) → HudModule.ShowVictory()

Victory | Defeat:
  Input(R / Enter / Space)
  → GameStateModule.RestartGame()
  → GameEvents.RaiseGameRestarted()
     → MetaProgression.Reset()
     → WeekManager.Reset()
     → SpawnModule.Clear()
     → SceneTree.ReloadCurrentScene()
```

---

## API Boundaries

```csharp
// Foundation/GameEvents.cs
public static class GameEvents {
    public static event Action<BossKind, int>     EnemyKilled;   // kind, xpAmount
    public static event Action<float>             PlayerDamaged; // damage
    public static event Action                    PlayerDied;
    public static event Action<int>               WeekChanged;   // newWeek (1–8)
    public static event Action<int, float, float> LevelChanged;  // level, fireCd, dmgMult
    public static event Action                    GameWon;
    public static event Action                    GameRestarted;
    public static event Action                    PrologueComplete;

    // Raise 메서드는 이벤트 소유 모듈만 호출 — 타 모듈은 구독만
    public static void RaiseEnemyKilled(BossKind kind, int xp) => EnemyKilled?.Invoke(kind, xp);
    // ... (동일 패턴)
}

// Core/IDamageable.cs
public interface IDamageable {
    void TakeDamage(float amount);
    float MaxHp { get; }
    float CurrentHp { get; }
}

// Feature/MetaProgression.cs
public partial class MetaProgression : Node {
    public int   CurrentLevel { get; private set; }
    public int   CurrentXp   { get; private set; }
    public void  AddXp(int amount);
    public void  Reset();
    // 레벨업 임계치: 60 + (level-1) * 40
    // 발사 간격: Mathf.Max(0.14f, 0.28f - level * 0.02f)
    // 피해 배율: 1f + level * 0.08f
}

// Foundation/GameBalance.cs (기존 — 확장)
public static class GameBalance {
    public static readonly int   TotalWeeks        = 8;
    public static readonly float WeekDurationSec   = 45f;
    public static readonly float PlayerMaxHp       = 100f;
    public static readonly float PlayerSpeed       = 220f;
    public static readonly float BaseFireInterval  = 0.28f;
    public static readonly float MinFireInterval   = 0.14f;
    public static readonly float SpawnRingRadius   = 520f;
    public static readonly float ProjectileLifetime = 2.2f;
    public static readonly float ProjectileHitRadius = 22f;
    public static readonly float PlayerHitRadius   = 14f;
    // 스폰 간격 공식: Mathf.Max(0.55f, 2.4f - (week-1) * 0.22f)
    // 적 HP 배율: 1f + (week-1) * 0.12f
    // 적 속도 배율: 1f + (week-1) * 0.08f
    // XP 임계치: 60 + (level-1) * 40
}
```

---

## ADR Audit

기존 ADR: **없음** (tr-registry.yaml 비어있음)  
→ 아래 Required ADRs를 순서대로 작성해야 코딩 시작 가능.

---

## Required ADRs

### Foundation (코딩 전 필수)

| 우선순위 | 제목 | 커버하는 TR |
|---------|------|-----------|
| 1 | Game State Machine — FSM 상태·전환 규칙 | TR-CORE-006, TR-CORE-007, TR-CORE-009, TR-CORE-011 |
| 2 | EventBus 설계 — C# 정적 이벤트 vs Godot Signal | TR-META-001, TR-META-003, TR-CORE-005 |

### Core (해당 시스템 구현 전)

| 우선순위 | 제목 | 커버하는 TR |
|---------|------|-----------|
| 3 | Projectile Lifecycle — Instantiate per-fire vs ObjectPool | TR-COMBAT-005 |
| 4 | Enemy Targeting — Group query per-frame vs spatial cache | TR-COMBAT-007 |

### Presentation

| 우선순위 | 제목 | 커버하는 TR |
|---------|------|-----------|
| 5 | HUD Data Binding — GameEvents 구독 패턴 | TR-CORE-011, TR-META-003 |

---

## Architecture Principles

1. **단방향 계층 의존** — 상위 계층은 하위 계층을 참조할 수 있으나 역방향 직접 참조 금지. 역방향 통신은 GameEvents 이벤트만 사용.
2. **GameBalance 단일 출처** — 게임플레이 수치는 `GameBalance.cs`에만 존재. GDD 공식 변경 = GameBalance만 수정.
3. **엔진 API 격리** — Godot 특화 코드(`Node`, `GetTree()`, `PackedScene` 등)는 각 모듈의 `_Ready`/`_PhysicsProcess`에 국한. 비즈니스 로직 메서드는 순수 C# (테스트 가능성).
4. **이벤트는 사실 통보, 명령 아님** — Raise 시 결과를 가정하지 않음. 구독자가 반응을 결정. (ex: `EnemyKilled`를 Raise하는 쪽은 XP가 어떻게 처리되는지 모름)
5. **MVP 범위 고수** — 오브젝트 풀링, 세이브/로드, 멀티플레이어는 이 아키텍처 범위 밖. 필요 시 새 ADR로 도입.

---

## Open Questions

| ID | 요약 | 우선순위 | 해결 경로 |
|----|------|---------|---------|
| QQ-01 | `SpawnModule`을 `Main.cs`에서 분리 시점 | Medium | ADR-003 또는 스프린트 1 |
| QQ-02 | VfxModule 인라인 vs 별도 노드 | Low | ADR-005 또는 구현 중 결정 |
| QQ-03 | `WeekTimer`가 적 페이드 완료를 await하는 방식 (Tween vs 수동 카운터) | Medium | ADR-001 |
