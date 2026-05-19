# Cross-GDD Review Report

**Date:** 2026-05-17  
**GDDs Reviewed:** 3  
**Systems Covered:** Core loop (Foundation), Combat & Survival (Core), Visual & Audio (Presentation)  
**Verdict:** FAIL — 6 blocking issues + 2 scenario blockers must be resolved before architecture begins.

---

## Consistency Issues

### Blocking

🔴 **D-01: WeekDurationSec 소유권 충돌**  
`gdd-office-layoff-survivor.md §7`과 `gdd-combat-survival.md §7`이 모두 `WeekDurationSec`를 자신의 Tuning Knob으로 선언. Entity registry는 메인 GDD를 source로 지정했으나 combat GDD가 이를 인정하지 않음.  
→ `gdd-combat-survival.md §7`에서 "단일 출처: gdd-office-layoff-survivor.md §4 — 여기서는 참조만" 명시 필요.

🔴 **D-02: 스폰 가중치 소유권 충돌**  
`gdd-office-layoff-survivor.md §7`("3종 적 비율")과 `gdd-combat-survival.md §7`("BossKind spawn weights")이 동일 데이터를 중복 소유. 단일 권위 출처 선언 없음.

🔴 **D-03: 플레이어 충돌 반경 모순 — Acceptance Criteria 충돌**  
`gdd-combat-survival.md §8 AC`: 플레이어 `r=14` (diameter=28px) 고정.  
`gdd-visual-audio.md §4`: "28–32px collision diameter" 범위로 기술.  
→ visual GDD 공식의 상한(r=16)이 combat GDD의 AC를 위반. 한쪽 수정 필요.

### Warnings

⚠️ **W-01: 단방향 의존성 3건**  
- `gdd-combat-survival.md §6` → `gdd-visual-audio.md` 의존 선언, 역방향 미기재.  
- `gdd-combat-survival.md §1` → `gdd-office-layoff-survivor.md` 부모 선언, 역방향 미기재.  
- `gdd-visual-audio.md §6` → `entity-inventory.md` 참조, 어느 GDD에도 역참조 없음.

⚠️ **W-02: 히트 VFX 스테일 레퍼런스**  
`gdd-combat-survival.md §6`이 "히트 VFX 정의 위치: gdd-visual-audio.md"로 기재했으나, gdd-visual-audio.md에 hit VFX 에셋 ID·스펙이 존재하지 않음.

⚠️ **W-03: art-bible.md §8 tiers 레퍼런스 검증 불가**  
`gdd-visual-audio.md §6`이 "art-bible.md §8 tiers" 참조 — 해당 섹션 존재 여부 확인 불가.

⚠️ **W-04: 스폰 공식 바닥값 데드코드**  
`max(0.55, 2.4-(Week-1)×0.22)` — Week 8 최솟값 0.86s로 0.55 클램프 도달 불가. 8주 범위 내에서 바닥값 작동 안 함.

⚠️ **W-05: effective_hp 합성 공식 미정의**  
HP 배율 공식(`1+(Week-1)×0.12`)의 output이 `base_hp`와 어떻게 합산되는지 어느 GDD에도 없음. `effective_hp = base_hp × hp_scale` 문서화 필요.

---

## Game Design Issues

### Blocking

🔴 **C3c: 리스크 없는 지배 전략 — 무한 카이팅**  
플레이어 이동 속도 220 px/s vs 최고속 적 HR 68 px/s (Week 8에서도 106 px/s). 플레이어가 모든 적보다 3.2–4.2배 빠름. 자동 공격 + 원형 카이팅으로 전 구간 무피해 가능. 적 종류별 대응 판단이 사라지고(Pillar 1 위반), 루프가 무반성적 원 그리기로 퇴행(Pillar 2 위반).  
→ 선택지: 플레이어 속도 하향 / 적 최고 속도 상향 / 카이팅 페널티 메카닉 추가 / 스폰 지점 다각화.

🔴 **C3d: XP 루프 불완전 — 소스만 있고 싱크 없음**  
적 처치로 XP 획득(6/8/10)되나, 레벨 임계치·레벨업 효과·업그레이드 선택 여부가 어느 GDD에도 없음. XP는 코드와 레지스트리에 존재하지만 설계 문서 없는 고아 시스템. 레벨업이 능력치 상승을 준다면 양의 피드백 루프의 상한도 미정의.  
→ **XP/레벨업 전용 GDD 신규 작성 필요.**

🔴 **C3e: 난이도 곡선 단측 가속 — 플레이어 파워 플랫**  
Week 1→8: 스폰 수 약 2배(18→54/구간), 적 HP ×1.84, 적 속도 ×1.56. 플레이어: 발사 간격·피해·이동 속도 전부 고정. TTK 증가 + 처치 수 증가 → Week 7–8에서 급경사 사망벽 발생 가능.  
→ XP/레벨업을 카운터 스케일링으로 쓸 것인지 선언하거나, 주차별 플레이어 파워 공식 추가 필요.

### Warnings

⚠️ **C3a: 진행 루프 경합 — XP vs 주차 생존**  
주차 타이머(8×45s)와 XP 레벨업이 모두 "핵심 루프" 위치를 차지하나, 둘의 관계가 정의되지 않음.

⚠️ **C3d: HP 순수 싱크 — 회복 없음**  
6분 세션에서 HP 회복 수단 없음. 의도적 설계라면 GDD에 명시 필요.

⚠️ **C3f: XP/레벨 시스템 필러 미연결**  
XP/레벨업에 Player Fantasy 서술도, 3개 필러 연결도 없음.

⚠️ **C3f: "동료 연대감" 후속 판타지 고아**  
`gdd-office-layoff-survivor.md §2`의 "동료 개발자 연대감(픽업·버프 확장)" 문구가 3개 필러 어디에도 연결되지 않음. `*(post-MVP, scoped out)*` 표시 권장.

---

## Cross-System Scenario Issues

**시나리오 워크: 4개**  
Week 전환 / XP→레벨업 / 플레이어 사망 / Week 8 승리

### Blockers

🔴 **Week 전환 시 기존 적 처리 미정의 — Combat + Main + Spawn**  
Week 타이머 만료 시 이전 주 적들의 처리 방침(지속 존재 / 일괄 제거 / 체력 리셋) 어느 GDD에도 없음. Undefined behavior.

🔴 **XP → 레벨업 전환 중단점 미정의 — Combat + Meta Progression**  
레벨업이 전투 중 발생할 때 게임플레이 일시정지 여부, 업그레이드 선택 UI 여부, 자동 적용 여부 어느 GDD에도 없음. Undefined behavior.

### Info

ℹ️ **플레이어 사망 → 패배 → 재시작** — CLEAN (GDD1 §5, §8 AC 커버)  
ℹ️ **Week 8 승리 → Victory UI → 재시작** — CLEAN (GDD1 §5 커버)

---

## GDDs Flagged for Revision

| GDD | 이유 | 유형 | 우선순위 |
|-----|------|------|---------|
| `gdd-office-layoff-survivor.md` | D-01/D-02 소유권 중복; C3a 루프 경합 미정의; C3d HP 회복 정책 미기재; 주 전환 적 처리 미정의 | Consistency + Design | Blocking |
| `gdd-combat-survival.md` | D-01/D-02 소유권 중복; D-03 충돌 반경 AC 모순; C3c 카이팅 지배 전략; C3e 플레이어 파워 플랫 | Consistency + Design | Blocking |
| `gdd-visual-audio.md` | D-03 충돌 직경 범위 모순; W-02 hit VFX 스펙 누락; W-01 combat 역참조 누락 | Consistency | Blocking |
| *(신규)* `gdd-meta-progression.md` | XP/레벨업 전용 GDD 없음 — 코드에 존재하나 설계 미문서 | Design | Blocking |

---

## Verdict: FAIL

### 아키텍처 전 필수 조치 6개

1. **`gdd-combat-survival.md §7`** — Tuning Knob을 "참조(단일 출처: 메인 GDD §4)"로 재기재 (D-01, D-02 해소)
2. **`gdd-visual-audio.md §4`** — 충돌 직경을 "28px (r=14, 코드 고정값 — gdd-combat-survival.md §8 AC 준수)"으로 확정 (D-03 해소)
3. **`gdd-combat-survival.md §3` 또는 `gdd-office-layoff-survivor.md §3`** — 카이팅 카운터 메카닉 결정 후 기재 (C3c 해소)
4. **신규 `gdd-meta-progression.md` 작성** — 레벨 공식, 업그레이드 선택 여부, XP 싱크 정의, 레벨업이 난이도 곡선의 카운터인지 선언 (C3d, C3e 부분 해소)
5. **`gdd-office-layoff-survivor.md §3`** — 주차 전환 시 기존 적 처리 정책 명시 (시나리오 블로커 해소)
6. **`gdd-office-layoff-survivor.md §4`** — 주차별 플레이어 파워 스케일링 공식 추가 또는 "레벨업이 카운터" 선언 (C3e 해소)
