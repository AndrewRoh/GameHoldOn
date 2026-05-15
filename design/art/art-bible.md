# Art Bible — Office Layoff Survivor

> **Scope**: 인디 MVP — 정적 스프라이트, 탑다운 2D, Godot 4.6.  
> **Sources**: `design/gdd/game-concept.md`, `design/gdd/gdd-office-layoff-survivor.md`

## 1. Visual Identity Statement

**한 줄 규칙:** *형광등 아래 치비 오피스 인물은 윤곽선과 역할색만으로 0.5초 안에 구분된다.*

- 탑다운에서 **머리·어깨 실루엣**이 가장 중요하다(몸통은 단순화).
- 풍자하되 **조롱 카툰**은 피하고, 플레이어는 **동정·연대** 톤.
- 배경은 **저채도 사무실 바닥/타일**; 캐릭터만 채도를 올린다.

## 2. Color Palette

| Role | Hex | Use |
|------|-----|-----|
| Player / Ally | `#5AD38E` | 개발자 후드·생존 주체 |
| Threat HR | `#C94C4C` | 인사과장 — 경고·서류 |
| Threat CEO | `#7B5EBF` | 대표 — 방향·슬로건 |
| Threat CTO | `#E07A2F` | 기술이사 — 도구·야근 |
| Projectile | `#FFE566` | 커밋/이슈 탄 — 가독성 |
| UI text | `#E8ECF0` | HUD·프롤로그 |
| BG dark | `#0D1117` | 프롤로그·야간 사무실 톤 |
| Floor | `#3D4A5C` | 오피스 바닥 타일 |

## 3. Lighting & Atmosphere

- **키 라이트**: 위에서 내려오는 형광등(그림자 짧게, 바닥에 타원 그림자 1개 optional).
- **대비**: 캐릭터 외곽선 `#1A1A22` 2px — 바닥과 분리.
- **주차 후반(6–8주)**: 환경 채도 −10% (코드/셰이더는 후속; 스펙만 기록).

## 4. Character Art Direction

| Cast | Silhouette | Costume cue | Read at 32px |
|------|------------|-------------|--------------|
| **Developer (player)** | 둥근 후드, 노트북/머그 | 연두 후드, 헤드폰 | ✓ |
| **HR** | 각진 어깨, 클립보드 | 빨간 재킷, 서류 | ✓ |
| **CEO** | 넓은 어깨, 넥타이 삼각 | 보라 정장 | ✓ |
| **CTO** | 안경 원형 2개, 태블릿 | 주황 가디건 | ✓ |

- **비율**: 2~2.5등신 치비. 머리 40% / 몸 60%.
- **방향**: MVP는 **정면(남향)** 단일 프레임; 후속 4방향 walk 4프레임.

## 5. Environment & Level Art

- **MVP**: 단색/체크 타일 바닥 `floor_office_tile.png` 128×128 seamless.
- **장애물**: 없음(프로토). 후속: 책상·복사기 콜리전.
- **스폰 링**: 화면 밖 520px — 아트는 스폰 이펙트만(후속).

## 6. UI Visual Language

- **폰트**: 시스템 sans 또는 Godot 기본 — 커스텀 폰트는 Polish.
- **HUD**: 좌상단 텍스트 + 얇은 반투명 패널 `#000000` 40% (후속).
- **프롤로그**: 전체 화면 다크 BG, 중앙 인용문, 하단 입력 힌트.

## 7. VFX & Particle Style

- **히트**: 4~6px 노란 스파크 3개, 0.15s.
- **사망**: 적 — 회색 종이 조각 4개; 플레이어 — 화면 비네트(후속).
- **발사**: 1프레임 muzzle flash 8×8 optional.

## 8. Asset Standards (Godot 2D MVP)

| Tier | Use | Canvas | Format | Notes |
|------|-----|--------|--------|-------|
| **T1 Character** | Player, enemies | 64×64 | PNG RGBA | pivot center-bottom (feet) |
| **T2 Projectile** | Bullet | 16×16 | PNG RGBA | pivot center |
| **T3 Environment** | Floor tile | 128×128 | PNG RGBA | repeat |
| **T4 UI** | Icons (week, HP) | 32×32 | PNG RGBA | optional MVP |

- **필터**: Nearest (pixel-crisp) 또는 Linear(부드러운 플랫) — **프로젝트는 Linear + 2px outline** 권장.
- **이름**: `chr_[role]_[variant].png`, `prj_commit_default.png`, `env_floor_office.png`
- **경로**: `assets/art/sprites/`, Godot import 후 `res://` 링크.

## 9. Style Prohibitions

- 사실적 3D 렌더·과도한 고어.
- 실존 인물·회사 로고·상표 모방.
- 적이 플레이어와 **같은 녹색** 계열 사용 금지.
