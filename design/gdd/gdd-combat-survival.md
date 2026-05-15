# GDD — Combat & Survival

> Parent: `gdd-office-layoff-survivor.md` · Implementation: `src/Main.cs`, `Player.cs`, `Enemy.cs`, `Projectile.cs`

## 1. Overview

탑다운 **자동 조준 투사체** + **접촉 피해** 서바이벌. **8주** 타이머와 스폰 곡선으로 난이도 상승. 적 3종은 HR/CEO/CTO 페르소나.

## 2. Player Fantasy

- 움직이며 **압박(적)을 피하고** 자동으로 **“일(탄환)”을 쏴** 한 주를 버틴다.

## 3. Detailed Rules

| Actor | Behavior | MVP art hook |
|-------|----------|--------------|
| Player | WASD, HP 100, fire CD 0.28s | `chr_player_dev.png` |
| HR | 빠름, 낮은 HP, 스폰 45% | `chr_enemy_hr.png` |
| CEO | 느림, 높은 HP, 스폰 25% | `chr_enemy_ceo.png` |
| CTO | 중간, 스폰 30% | `chr_enemy_cto.png` |
| Projectile | 직선, 수명 2.2s, 반경 22px hit | `prj_commit_default.png` |

- **Week**: 45s/주, Week 8 클리어 시 승리.
- **스폰**: 링 반경 520px, 간격 `max(0.55, 2.4 - (Week-1)*0.22)`.

## 4. Formulas

(메인 GDD §4와 동일 — 단일 출처는 `gdd-office-layoff-survivor.md`, 구현 상수는 `entities.yaml` constants)

## 5. Edge Cases

- 적 0명: 발사 쿨만 소모, 탄 미발사.
- 동시 다수 적: 가장 가까운 대상 1명만 조준.

## 6. Dependencies

- `gdd-visual-audio.md` — 스프라이트·히트 VFX.
- Godot groups: `player`, `enemies`.

## 7. Tuning Knobs

- `BossKind` spawn weights, `WeekDurationSec`, projectile `Damage` scale per week.

## 8. Acceptance Criteria

- [ ] 스프라이트 교체 후에도 **충돌 반경** 유지(플레이어 r=14, 투사체 hit 22).
- [ ] 3적 색/실루엣이 art bible과 일치.

## Visual / Audio Requirements

- **Player**: 연두 후드 치비, 64×64, pivot 발밑.
- **Enemies**: 역할색 + 소품 1개(클립보드/넥타이/태블릿).
- **Projectile**: 작은 **커밋 메시지 말풍선** 또는 **노란 괄호 `{}`** 형태.
- **SFX (후속)**: `sfx_fire_soft.wav`, `sfx_hit_paper.wav`, `sfx_week_tick.wav`.
