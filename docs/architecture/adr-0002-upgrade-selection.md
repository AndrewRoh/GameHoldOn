# ADR-0002: Upgrade Selection (Level-Up Card Picker)

## Status
Accepted

## Date
2026-05-19

## Engine Compatibility

| Field | Value |
|-------|-------|
| **Engine** | Godot 4.6 / C# .NET 8 |
| **Domain** | Core / UI |
| **Knowledge Risk** | LOW — `Engine.TimeScale`, `CanvasLayer`, `ProcessModeEnum.Always` are stable Godot 4 APIs |
| **References Consulted** | `docs/engine-reference/godot/VERSION.md` |
| **Post-Cutoff APIs Used** | None |
| **Verification Required** | Manual: level-up pauses gameplay, cards selectable via mouse and keys 1–3, TimeScale restores to 1 |

## ADR Dependencies

| Field | Value |
|-------|-------|
| **Depends On** | ADR-0001 (game session lifecycle — Playing only) |
| **Enables** | Future weapon/evolution epics that hook card stacks |
| **Blocks** | Meta-progression story implementation until Accepted |
| **Ordering Note** | `Main` owns level-up trigger; only `UpgradeManager` may set `Engine.TimeScale` during card pick |

## Context

### Problem Statement
레벨업 시 플레이어가 3장의 업그레이드 카드 중 하나를 고르는 뱀파이어 서바이버 스타일 빌드 시스템이 필요하다. 기존 `FireInterval(level)` / `DamageMultiplier(level)` 자동 스케일링은 GDD `gdd-upgrade-selection.md`에 따라 **비활성화**하고, 모든 전투 스탯 변화는 카드 스택으로만 적용한다.

### Constraints
- MVP: 코드 기반 UI (UMG/씬 에디터 카드 프리팹 없음)
- 레벨업 중 게임 완전 정지 (`Engine.TimeScale = 0`)
- 카드 추첨·스택 규칙은 GDD §3–4와 동일
- Godot 의존 로직과 순수 C# 로직 분리 (단위 테스트 가능)

### Requirements
- 15종 카드 + 풀 소진 시 "기운 회복" 폴백
- 레어도 가중치: Lv 2–4 / 5–6 / 7–8 구간별
- 동일 카드 3장 동시 제시 금지, 최대 스택 도달 카드 제외
- Lv 8 도달 후 레벨업 UI 없음

## Decision

`UpgradeManager` (`Node`, `CanvasLayer` 자식)가 카드 추첨·UI·적용을 전담한다. 추첨과 스탯 합성은 Godot 비의존 정적 클래스로 분리한다.

### Architecture Diagram

```
Main (level-up trigger, XP)
  │
  ├── Player (stacks, combat stats, fire/orbitals)
  │
  └── CanvasLayer
        ├── GameHud
        └── UpgradeManager
              ├── UpgradeDrawService  (pure C#)
              ├── UpgradeCardCatalog  (pure C#)
              └── PlayerCombatStats   (pure C#)
```

### Flow

1. `Main.OnEnemyKilled` → XP 임계치 → `Level++` (max 8)
2. 플레이어 HP ≤ 0이면 UI 생략 (사망 우선)
3. `UpgradeManager.OpenSelection(level)` → `Engine.TimeScale = 0`, 3카드 UI
4. 선택 → `Player.ApplyUpgrade(id)` → 스택++ → `PlayerCombatStats.FromStacks` 재계산
5. `Engine.TimeScale = 1`, UI 숨김, `Main` 스폰 재개

### Key Interfaces

```csharp
// UpgradeManager.cs
public void OpenSelection(int newLevel);
public bool IsOpen { get; }

// Player.cs
public void ApplyUpgrade(UpgradeCardId id);
public IReadOnlyDictionary<UpgradeCardId, int> UpgradeStacks { get; }

// UpgradeDrawService.cs (testable)
public static IReadOnlyList<UpgradeOffer> DrawOffers(
    int playerLevel,
    IReadOnlyDictionary<UpgradeCardId, int> stacks,
    Random rng);
```

### Stat ownership

| State | Owner | Access |
|-------|-------|--------|
| Card stack counts | `Player` | `UpgradeStacks` read-only |
| Derived combat stats | `Player` via `PlayerCombatStats` | `FireCooldown`, `DamageMultiplier`, etc. |
| TimeScale during pick | `UpgradeManager` | write only while `IsOpen` |
| XP / Level | `Main` | unchanged |

### Disabled patterns

- `Main.LevelUp`에서 `GameBalance.FireInterval(level)` / `DamageMultiplier(level)` **호출 금지**
- 카드 효과를 `Main`이나 `GameHud`에 직접 쓰지 않음

## Alternatives Considered

### Alternative 1: Autoload singleton `UpgradeService`
- **Pros**: 전역 접근 용이
- **Cons**: 테스트·씬 재로드 시 상태 잔류, ADR-0001 금지 패턴과 유사 결합
- **Rejection Reason**: `Main` 씬 트리의 자식 `UpgradeManager`로 세션 범위 명확화

### Alternative 2: Scene-based card `.tscn` per rarity
- **Pros**: 아트 교체 용이
- **Cons**: MVP 15종 × 3 레어도 씬 유지 비용
- **Rejection Reason**: 프로그래매틱 UI로 먼저 검증, 후속 아트 패스에서 씬화

## Consequences

### Positive
- GDD 카드 규칙과 1:1 매핑 가능
- `UpgradeDrawService` 단위 테스트로 추첨 회귀 방지
- TimeScale 일시정지로 선택 UX 명확

### Negative
- `Player`가 다수 카드 효과 분기를 알아야 함 (후속: effect strategy 리팩터 가능)
- 궤도 투사체(E2)는 추가 `OrbitalSatellite` 노드 비용

### Risks
- **TimeScale = 0** 중 `_Process(delta)`가 0 — `ProcessMode.Always` UI만 동작해야 함 → `UpgradeManager`에 `Always` 설정
- **풀 소진** 시 3장 미만 → 폴백 카드 `RestFallback` 삽입

## GDD Requirements Addressed

| GDD | Requirement | How This ADR Addresses It |
|-----|---------------|---------------------------|
| `gdd-upgrade-selection.md` | TimeScale 0, 3 cards, input 1/2/3 + click | `UpgradeManager` UI + input routing |
| `gdd-upgrade-selection.md` | Rarity weights by level band | `UpgradeDrawService` |
| `gdd-upgrade-selection.md` | No duplicate offers, max stack exclusion | `UpgradeDrawService` |
| `gdd-meta-progression.md` | No auto FireInterval/DamageMultiplier | `Main` delegates to cards only |
| `gdd-meta-progression.md` | Max level 8, one level per kill | `Main` unchanged contract |

## Performance Implications
- **CPU**: Draw 3 cards O(pool size); negligible per level-up
- **Memory**: 15 stack counters + ≤2 orbital nodes
- **UI**: 3 `Button` nodes, no per-frame allocation after open

## Migration Plan

1. Add `UpgradeManager` + pure C# upgrade types
2. Remove `Main.LevelUp` auto stat formulas
3. Wire `Player` multi-shot, pierce, orbitals
4. Add `tests/` for `UpgradeDrawService`

## Validation Criteria

- [ ] Level-up freezes gameplay and shows 3 cards
- [ ] Keys 1/2/3 and mouse select apply stack and resume
- [ ] Lv 2–4 offers contain no Epic cards (automated test)
- [ ] No duplicate card IDs in one offer set (automated test)
- [ ] Max-stack cards excluded from pool (automated test)

## Related Decisions
- ADR-0001: Game State Machine
- `design/gdd/gdd-upgrade-selection.md`
- `design/gdd/gdd-meta-progression.md`
