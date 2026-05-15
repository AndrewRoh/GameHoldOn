# Asset Specs — Combat Visual (Office Layoff Survivor)

> **Source**: `design/gdd/gdd-visual-audio.md`, `design/gdd/gdd-combat-survival.md`, `design/art/art-bible.md`  
> **Reference notebook**: [2d Game Development and AI Coding](https://notebooklm.google.com/notebook/6e9b7ae3-8203-443b-925d-c0040062e981) — 탑다운 가독성, 윤곽선, 프레임 최소화, 원형 콜라이더  
> **Generated**: 2026-05-15  
> **Status**: 6 specced · 6 placeholder PNG in `src/assets/art/`

## NotebookLM synthesis (applied)

| Topic | Recommendation | This project |
|-------|----------------|--------------|
| Canvas | 32×32 common for pixel; we use **64×64** for flat/chibi detail | T1 64×64 |
| Outline | Solid outline for BG separation; sel-out optional | **2px `#1A1A22`** on characters |
| Animation | Vampire Survivors style: **few/no frames** for perf | MVP **1 frame** each; walk 4×4 post-MVP |
| Pivot | Feet on ground for top-down | **center-bottom** (32, 56) on 64×64 |
| Projectile | Small; generous hitbox in code | 16×16 art, **22px** hit radius (code) |
| Collider | Circle colliders for many objects | Player r=14, projectile circle |
| Silhouette | Chibi, bent limbs, no eye whites at small size | Head 40%, prop 1 per enemy |

---

## ASSET-001 — Player Developer

| Field | Value |
|-------|-------|
| Category | Sprite |
| Dimensions | 64×64 px (display ~28px wide on screen) |
| Format | PNG RGBA |
| Naming | `chr_player_dev.png` |
| Path | `src/assets/art/sprites/chr_player_dev.png` |
| Pivot | Bottom center (feet y=56) |
| Frames (MVP) | **1** idle/front |
| Frames (post-MVP) | 4-dir × 4 walk = 16 (sprite sheet 256×64 or 64×256) |

**Shape language**

- **Head**: rounded hood dome, 18px wide, `#5AD38E` fill, darker hood shadow `#3FA86C`.
- **Body**: squat trapezoid torso; **headphones** band arc on hood.
- **Prop**: tiny **laptop/mug** blob lower-right (1–2 px readable block).
- **Outline**: 2px `#1A1A22` around entire silhouette.
- **Shadow**: optional 12×6 ellipse `#000000` 30% at feet.

**Art Bible anchors**

- §2 Player `#5AD38E`
- §4 Developer: round hood, headphones
- §9 No green on enemies

**Generation prompt**

```
top-down 2D game character sprite, single frame, chibi office developer,
mint green hoodie #5AD38E, round hood, simple headphones, holding coffee mug,
flat vector style, 2px dark outline #1A1A22, no anti-aliasing on outer edge,
transparent background, centered, feet at bottom, 64x64 pixels,
Vampire Survivors readability, cute not mocking
Negative: photorealistic, 3D, logo, text, gore, same red as enemies
```

**Godot**

- `Sprite2D`, `TextureFilter = Linear`, `Centered = false`, `Offset = (0, -56)` or custom pivot.
- Fallback: `Polygon2D` if texture missing.

**Status**: Placeholder generated

---

## ASSET-002 — Enemy HR (인사과장)

| Field | Value |
|-------|-------|
| Category | Sprite |
| Dimensions | 64×64 |
| Format | PNG RGBA |
| Naming | `chr_enemy_hr.png` |
| Pivot | Bottom center |
| Frames (MVP) | **1** |
| Frames (post-MVP) | 4 walk × 4 dir (faster cadence) |

**Shape language**

- **Silhouette**: angular shoulders (wider than player).
- **Color**: jacket `#C94C4C`, shirt `#E8A0A0`, hair bun dark `#4A2020`.
- **Prop**: **clipboard** rectangle left side, white paper `#F5F5F5`.
- **Face**: no eyes — single V-shaped collar notch only.

**Generation prompt**

```
top-down chibi HR manager enemy sprite, red blazer #C94C4C, clipboard,
angular shoulders, flat 2D, 2px outline #1A1A22, 64x64, transparent BG,
office satire, readable silhouette, no realistic face
Negative: green hoodie, gore, company logos
```

**Status**: Placeholder generated

---

## ASSET-003 — Enemy CEO (대표)

| Field | Value |
|-------|-------|
| Category | Sprite |
| Dimensions | 64×64 |
| Naming | `chr_enemy_ceo.png` |
| Frames (MVP) | **1** |

**Shape language**

- **Silhouette**: **broadest** shoulders of three enemies.
- **Color**: suit `#7B5EBF`, shirt `#B8A8E8`, **gold tie** triangle `#FFD700` center.
- **Head**: simple oval, optional single hair line.

**Generation prompt**

```
top-down chibi CEO enemy, purple suit #7B5EBF, wide shoulders, gold tie,
flat vector, 2px dark outline, 64x64, transparent background, intimidating but cartoon
Negative: green, red HR jacket, logos
```

**Status**: Placeholder generated

---

## ASSET-004 — Enemy CTO (기술이사)

| Field | Value |
|-------|-------|
| Category | Sprite |
| Dimensions | 64×64 |
| Naming | `chr_enemy_cto.png` |
| Frames (MVP) | **1** |

**Shape language**

- **Silhouette**: medium width; **two circle glasses** on face (12px apart).
- **Color**: cardigan `#E07A2F`, shirt `#F0C090`.
- **Prop**: **tablet** rectangle in front `#505868` with tiny cyan `#5ADCF0` screen dot (not player green).

**Generation prompt**

```
top-down chibi CTO enemy, orange cardigan #E07A2F, round glasses, holding tablet,
flat 2D game sprite, 2px outline, 64x64, transparent BG
Negative: green hoodie, photorealistic
```

**Status**: Placeholder generated

---

## ASSET-005 — Commit Projectile

| Field | Value |
|-------|-------|
| Category | Sprite |
| Dimensions | **16×16** |
| Naming | `prj_commit_default.png` |
| Pivot | Center |
| Frames (MVP) | **1** (optional 2-frame blink post-MVP) |

**Shape language**

- Yellow **`{ }`** brackets or tiny **speech bubble** `#FFE566` with 2px `#1A1A22` outline.
- Core fill `#FFF0A0`; max 8×8 drawable area inside 16×16.

**Generation prompt**

```
tiny 16x16 game projectile sprite, yellow curly braces or commit message bubble,
flat icon, high contrast, dark outline, transparent background, no glow bloom
Negative: large, realistic, bullet casing
```

**Godot**: `Sprite2D` scale 1.0; hit radius 22px in code (generous feel per notebook).

**Status**: Placeholder generated

---

## ASSET-006 — Office Floor Tile

| Field | Value |
|-------|-------|
| Category | Environment |
| Dimensions | **128×128** seamless tile |
| Naming | `env_floor_office.png` |
| Path | `src/assets/art/tiles/env_floor_office.png` |
| Frames | **1** |

**Shape language**

- Base `#3D4A5C`; checker **+8% luminance** squares 64×64.
- Subtle panel lines every 32px `#2E3848` (1px).
- No characters; low saturation so cast pop.

**Generation prompt**

```
seamless 128x128 top-down office floor tile texture, muted blue-gray carpet,
subtle checker pattern, flat game art, no objects, tileable edges
Negative: characters, furniture, high saturation
```

**Godot**: `TileMapLayer` or repeated `Sprite2D` / `TextureRect` under gameplay; z-index below entities.

**Status**: Placeholder generated

---

## Sprite sheet plan (post-MVP)

| Sheet | Layout | FPS |
|-------|--------|-----|
| `chr_player_dev_walk.png` | 4 columns × 4 rows (dir) | 8 |
| `chr_enemy_*_walk.png` | 4×4 each | HR 10, CEO 6, CTO 8 |

Use Godot `AnimatedSprite2D` or `Sprite2D` + `hframes`/`vframes`.

## Import settings (Godot 4)

| Asset | Filter | Mipmaps | Compress |
|-------|--------|---------|----------|
| Characters | Linear | Off | Lossless |
| Projectile | Nearest or Linear | Off | Lossless |
| Floor | Linear, Repeat enabled | Off | Lossless |

## Revision log

| Date | Change |
|------|--------|
| 2026-05-15 | Initial spec + NotebookLM 2d Game Dev notebook alignment |
