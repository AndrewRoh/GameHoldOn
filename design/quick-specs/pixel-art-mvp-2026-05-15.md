# Quick Design Spec: MVP Pixel Art via PixelLab

**Type**: Addition (Presentation pipeline)  
**System**: Visual & Audio / Combat sprites  
**GDD Reference**: `design/gdd/gdd-visual-audio.md`, `design/assets/specs/combat-visual-assets.md`  
**Date**: 2026-05-15

## Change Summary

프로토타입 **플레이스홀더 PNG**를 **PixelLab MCP**로 생성한 픽셀 아트로 교체한다. 탑다운 뱀서류 가독성(치비·윤곽선·역할색)은 art bible·combat-visual-assets 스펙을 따른다.

## Motivation

- GDD·에셋 스펙은 있으나 최종 아트가 없어 폴리시 전 단계에서 **일관된 픽셀 스타일**이 필요함.
- PixelLab `create_character` (4방향, chibi, low top-down) + `create_map_object` / `create_tiles_pro`로 인디 스코프 내 일괄 제작.

## Design Delta

**현재 GDD** (`gdd-office-layoff-survivor.md` §6):  
> 아트·사운드: MVP는 도형/플레이스홀더, 이후 스프라이트·SFX 교체.

**이 스펙 적용 후**:

| Asset ID | 파일 | PixelLab 도구 | 게임 사용 |
|----------|------|---------------|-----------|
| ASSET-001 | `chr_player_dev.png` | `create_character` → south | `ArtPaths.Player` |
| ASSET-002 | `chr_enemy_hr.png` | 동일 | HR enemy |
| ASSET-003 | `chr_enemy_ceo.png` | 동일 | CEO enemy |
| ASSET-004 | `chr_enemy_cto.png` | 동일 | CTO enemy |
| ASSET-005 | `prj_commit_default.png` | `create_map_object` 16×16 | Projectile |
| ASSET-006 | `env_floor_office.png` | `create_tiles_pro` square top-down | Main floor grid |

- MVP는 **남향(south) 프레임 1장**만 `Sprite2D`에 사용. 4방향 전체는 후속 `AnimatedSprite2D`.
- 생성 실패 시 기존 `tools/generate_placeholder_sprites.py` 폴백 유지.

## New Rules / Values

- 모든 캐릭터: `size=48`, `n_directions=4`, `view=low top-down`, `proportions=chibi`, `outline=single color black outline`, `shading=flat shading`.
- 색/소품은 combat-visual-assets **Generation prompt** 문구를 `description`에 반영.

## Acceptance Criteria

- [x] 5/6 PNG PixelLab → `src/assets/art/` (캐릭터 4 + 탄환 1)
- [x] 바닥 타일 PixelLab `tile_0` / `tile_1` 체커보드
- [x] F5 시 플레이어·3적·탄 PixelLab 아트 (바닥은 플레이스홀더 가능)
- [x] `asset-manifest.md` 갱신
- [x] `dotnet build` 성공

## PixelLab job IDs (2026-05-15)

| Asset | ID |
|-------|-----|
| Player | `5f750a95-08dc-4d18-bf64-90557f6a14b2` |
| HR | `cd3fa112-b0bd-4a45-a703-e27f7f3226fe` |
| CEO | `0d6bf6ab-f4d6-4192-b81c-95052f0e2e1a` |
| CTO | `103eb4fb-4415-4c70-85f4-066fd9313d62` |
| Projectile | `58aecabf-8b45-411e-a2a1-3ab79635b5a8` |
| Floor tiles | `029b3ff8-158d-4901-841c-dce8ca0d143f` (processing) |

## Out of scope

- Walk 애니메이션 (`animate_character`)
- UI/HUD 픽셀 아이콘
- BGM/SFX
