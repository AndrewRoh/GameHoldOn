using System.Collections.Generic;

namespace GameHoldOn.Upgrades;

/// <summary>Static card pool (gdd-upgrade-selection.md §4).</summary>
public static class UpgradeCardCatalog
{
    public static IReadOnlyList<UpgradeCardDef> All { get; } =
    [
        new(UpgradeCardId.OvertimeShift, UpgradeRarity.Common, "야근 특근", "발사 간격 ×0.87", 2, 4),
        new(UpgradeCardId.CommitRush, UpgradeRarity.Common, "커밋 러시", "투사체 +1", 2, 3),
        new(UpgradeCardId.PullRequestMerge, UpgradeRarity.Common, "풀리퀘 머지", "피해 ×1.18", 2, 4),
        new(UpgradeCardId.Hotfix, UpgradeRarity.Common, "핫픽스", "탄속 ×1.25", 2, 2),
        new(UpgradeCardId.LeanMethodology, UpgradeRarity.Common, "린 방법론", "이동속도 ×1.10", 2, 3),
        new(UpgradeCardId.SprintBoost, UpgradeRarity.Common, "스프린트 가속", "XP 획득 ×1.30", 2, 2),
        new(UpgradeCardId.PtoUse, UpgradeRarity.Common, "연차 사용", "최대 HP +30, 즉시 +30 회복", 2, 4),
        new(UpgradeCardId.BugReport, UpgradeRarity.Rare, "버그 리포트", "관통 +1", 3, 2),
        new(UpgradeCardId.CodeReview, UpgradeRarity.Rare, "코드 리뷰", "치명타율 +15%", 3, 3),
        new(UpgradeCardId.TeamBuilding, UpgradeRarity.Rare, "팀 빌딩", "HP 재생 +2/s", 3, 3),
        new(UpgradeCardId.RemoteWork, UpgradeRarity.Rare, "재택근무", "접촉 피해 회피 +10%", 3, 2),
        new(UpgradeCardId.TechDebtRelief, UpgradeRarity.Rare, "기술 부채 탕감", "받는 피해 ×0.85", 3, 2),
        new(UpgradeCardId.RestructuringCounter, UpgradeRarity.Epic, "구조조정 역습", "투사체 +2, 피해 ×1.40", 5, 1),
        new(UpgradeCardId.JuniorDeveloper, UpgradeRarity.Epic, "주니어 개발자", "궤도 투사체 +1", 5, 2),
        new(UpgradeCardId.CeoIdea, UpgradeRarity.Epic, "CEO 아이디어", "피해·발사·이동 각 ×1.15", 5, 1),
        new(UpgradeCardId.RestFallback, UpgradeRarity.Common, "기운 회복", "HP +15", 1, 99)
    ];

    public static UpgradeCardDef Get(UpgradeCardId id)
    {
        foreach (var def in All)
        {
            if (def.Id == id)
            {
                return def;
            }
        }

        return All[^1];
    }
}
