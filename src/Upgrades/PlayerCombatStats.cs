using System;
using System.Collections.Generic;

namespace GameHoldOn.Upgrades;

/// <summary>Aggregates card stacks into combat numbers (gdd-upgrade-selection.md §4).</summary>
public sealed class PlayerCombatStats
{
    public float FireCooldown { get; init; } = GameBalance.FireCooldownBase;
    public float DamageMultiplier { get; init; } = 1f;
    public float MoveSpeed { get; init; } = GameBalance.PlayerMoveSpeed;
    public float ProjectileSpeed { get; init; } = GameBalance.ProjectileSpeed;
    public float XpGainMultiplier { get; init; } = 1f;
    public float MaxHpBonus { get; init; }
    public int ProjectileCount { get; init; } = 1;
    public int PierceCount { get; init; }
    public float CritChance { get; init; }
    public float CritMultiplier { get; init; } = 1.5f;
    public float HpRegenPerSec { get; init; }
    public float ContactDodgeChance { get; init; }
    public float DamageTakenMultiplier { get; init; } = 1f;
    public int OrbitalCount { get; init; }

    public static PlayerCombatStats FromStacks(IReadOnlyDictionary<UpgradeCardId, int> stacks)
    {
        var stats = new PlayerCombatStats();
        var fireCd = GameBalance.FireCooldownBase;
        var dmgMult = 1f;
        var move = GameBalance.PlayerMoveSpeed;
        var projSpeed = GameBalance.ProjectileSpeed;
        var xpGain = 1f;
        var maxHpBonus = 0f;
        var projectileCount = 1;
        var pierce = 0;
        var crit = 0f;
        var regen = 0f;
        var dodge = 0f;
        var takenMult = 1f;
        var orbitals = 0;

        foreach (var (id, count) in stacks)
        {
            if (count <= 0)
            {
                continue;
            }

            switch (id)
            {
                case UpgradeCardId.OvertimeShift:
                    fireCd *= MathF.Pow(0.87f, count);
                    break;
                case UpgradeCardId.CommitRush:
                    projectileCount += count;
                    break;
                case UpgradeCardId.PullRequestMerge:
                    dmgMult *= MathF.Pow(1.18f, count);
                    break;
                case UpgradeCardId.Hotfix:
                    projSpeed *= MathF.Pow(1.25f, count);
                    break;
                case UpgradeCardId.LeanMethodology:
                    move *= MathF.Pow(1.10f, count);
                    break;
                case UpgradeCardId.SprintBoost:
                    xpGain *= MathF.Pow(1.30f, count);
                    break;
                case UpgradeCardId.PtoUse:
                    maxHpBonus += 30f * count;
                    break;
                case UpgradeCardId.BugReport:
                    pierce += count;
                    break;
                case UpgradeCardId.CodeReview:
                    crit += 0.15f * count;
                    break;
                case UpgradeCardId.TeamBuilding:
                    regen += 2f * count;
                    break;
                case UpgradeCardId.RemoteWork:
                    dodge += 0.10f * count;
                    break;
                case UpgradeCardId.TechDebtRelief:
                    takenMult *= MathF.Pow(0.85f, count);
                    break;
                case UpgradeCardId.RestructuringCounter:
                    projectileCount += 2 * count;
                    dmgMult *= MathF.Pow(1.40f, count);
                    break;
                case UpgradeCardId.JuniorDeveloper:
                    orbitals += count;
                    break;
                case UpgradeCardId.CeoIdea:
                    dmgMult *= MathF.Pow(1.15f, count);
                    fireCd /= MathF.Pow(1.15f, count);
                    move *= MathF.Pow(1.15f, count);
                    break;
            }
        }

        projectileCount = Math.Min(projectileCount, 4);
        pierce = Math.Min(pierce, 2);
        crit = Math.Min(crit, 0.45f);
        dodge = Math.Min(dodge, 0.20f);

        return new PlayerCombatStats
        {
            FireCooldown = MathF.Max(GameBalance.FireCooldownMin, fireCd),
            DamageMultiplier = dmgMult,
            MoveSpeed = move,
            ProjectileSpeed = projSpeed,
            XpGainMultiplier = xpGain,
            MaxHpBonus = maxHpBonus,
            ProjectileCount = Math.Max(1, projectileCount),
            PierceCount = pierce,
            CritChance = crit,
            CritMultiplier = 1.5f,
            HpRegenPerSec = regen,
            ContactDodgeChance = dodge,
            DamageTakenMultiplier = takenMult,
            OrbitalCount = orbitals
        };
    }
}
