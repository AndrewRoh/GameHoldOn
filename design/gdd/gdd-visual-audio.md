# GDD — Visual & Audio (Presentation)

> Art bible: `design/art/art-bible.md` · Asset specs: `design/assets/specs/combat-visual-assets.md`

## 1. Overview

MVP는 **플레이스홀더 Polygon2D** → **정적 PNG 스프라이트**로 일괄 교체. 탑다운 가독성·역할색 일관성이 최우선.

## 2. Player Fantasy

- 개발자 아바타가 **“나”** 로 읽히고, 적과 색·실루엣이 즉시 구분된다.

## 3. Detailed Rules

### Sprite attachment (Godot)

| Node | Resource | Scale | Offset |
|------|----------|-------|--------|
| Player `Sprite2D` | `res://assets/art/sprites/chr_player_dev.png` | 1.0 | (0, -8) feet align |
| Enemy `Sprite2D` | per `BossKind` | 1.0 | (0, -8) |
| Projectile `Sprite2D` | `prj_commit_default.png` | 1.0 | center |

- **Collision**은 스프라이트와 분리 유지(코드 반경 그대로).
- **Label** 태그(HR/CEO/CTO)는 스프라이트 적용 후 **제거** 또는 8px 미니 배지로 축소.

### Animation (post-MVP)

| Asset | Frames | FPS | Notes |
|-------|--------|-----|-------|
| Player walk | 4 dir × 4 | 8 | 후드 흔들림 최소 |
| Enemy walk | 4 × 2 | 6 | HR 빠른 보행 |
| Hit flash | 1 | — | white multiply 0.1s |

## 4. Formulas

- On-screen size ≈ **28–32px** collision diameter → art **64×64** with ~40% character fill.

## 5. Edge Cases

- 4K 디스플레이: viewport stretch `canvas_items` — UI는 앵커 고정.
- 스프라이트 누락 시: 폴백 Polygon2D(현재 코드 색).

## 6. Dependencies

- `art-bible.md` §8 tiers.
- `entity-inventory.md`.

## 7. Tuning Knobs

- `scale`, `modulate` per week (후반 적 red tint +5%).

## 8. Acceptance Criteria

- [ ] 1280×720에서 **3적 + 플레이어** 동시 20마리여도 구분 가능.
- [ ] 아트 bible 금지색(플레이어=적 녹색) 미사용.

## Audio (descriptions only)

| ID | Type | Character |
|----|------|-----------|
| `bgm_week_pressure` | Music | lo-fi 긴장, 90–100 BPM, 사무실 앰비언스 |
| `bgm_prologue` | Music | 피아노 단음, 프롤로그 전용 |
| `sfx_ui_confirm` | SFX | 짧은 클릭 |
