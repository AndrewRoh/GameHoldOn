using GameHoldOn.Upgrades;
using Xunit;

namespace GameHoldOn.Tests;

public class UpgradeDrawServiceTests
{
    [Fact]
    public void test_draw_offers_no_duplicate_cards_in_one_set()
    {
        var rng = new Random(42);
        for (var level = 2; level <= 8; level++)
        {
            var offers = UpgradeDrawService.DrawOffers(level, new Dictionary<UpgradeCardId, int>(), rng);
            Assert.Equal(3, offers.Count);
            Assert.Equal(offers.Count, offers.Select(o => o.Id).Distinct().Count());
        }
    }

    [Fact]
    public void test_draw_offers_lv2_to_4_excludes_epic_cards()
    {
        var rng = new Random(7);
        for (var level = 2; level <= 4; level++)
        {
            for (var i = 0; i < 30; i++)
            {
                var offers = UpgradeDrawService.DrawOffers(level, new Dictionary<UpgradeCardId, int>(), rng);
                foreach (var offer in offers)
                {
                    var def = UpgradeCardCatalog.Get(offer.Id);
                    Assert.NotEqual(UpgradeRarity.Epic, def.Rarity);
                }
            }
        }
    }

    [Fact]
    public void test_draw_offers_excludes_max_stacked_cards()
    {
        var stacks = new Dictionary<UpgradeCardId, int>
        {
            [UpgradeCardId.OvertimeShift] = 4
        };
        var rng = new Random(99);
        for (var i = 0; i < 40; i++)
        {
            var offers = UpgradeDrawService.DrawOffers(3, stacks, rng);
            Assert.DoesNotContain(offers, o => o.Id == UpgradeCardId.OvertimeShift);
        }
    }

    [Fact]
    public void test_player_combat_stats_commit_rush_three_stacks_four_projectiles()
    {
        var stacks = new Dictionary<UpgradeCardId, int> { [UpgradeCardId.CommitRush] = 3 };
        var stats = PlayerCombatStats.FromStacks(stacks);
        Assert.Equal(4, stats.ProjectileCount);
    }

    [Fact]
    public void test_player_combat_stats_junior_developer_two_stacks_two_orbitals()
    {
        var stacks = new Dictionary<UpgradeCardId, int> { [UpgradeCardId.JuniorDeveloper] = 2 };
        var stats = PlayerCombatStats.FromStacks(stacks);
        Assert.Equal(2, stats.OrbitalCount);
    }
}
