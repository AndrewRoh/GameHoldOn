using System;
using System.Collections.Generic;

namespace GameHoldOn.Upgrades;

/// <summary>Draws three upgrade offers per level-up (gdd-upgrade-selection.md §3-2).</summary>
public static class UpgradeDrawService
{
    private const int OfferCount = 3;

    public static IReadOnlyList<UpgradeOffer> DrawOffers(
        int playerLevel,
        IReadOnlyDictionary<UpgradeCardId, int> stacks,
        Random rng)
    {
        var offers = new List<UpgradeOffer>(OfferCount);
        var used = new HashSet<UpgradeCardId>();

        for (var slot = 0; slot < OfferCount; slot++)
        {
            var rarity = RollRarity(playerLevel, rng);
            var card = PickCard(playerLevel, rarity, stacks, used, rng);
            if (card == null)
            {
                card = PickCard(playerLevel, Downgrade(rarity), stacks, used, rng);
            }

            if (card == null)
            {
                card = UpgradeCardCatalog.Get(UpgradeCardId.RestFallback);
            }

            used.Add(card.Id);
            offers.Add(new UpgradeOffer(card.Id, card.Title, card.Description));
        }

        return offers;
    }

    private static UpgradeRarity RollRarity(int level, Random rng)
    {
        var roll = rng.Next(0, 100);
        if (level <= 4)
        {
            return roll < 80 ? UpgradeRarity.Common : UpgradeRarity.Rare;
        }

        if (level <= 6)
        {
            if (roll < 65)
            {
                return UpgradeRarity.Common;
            }

            if (roll < 95)
            {
                return UpgradeRarity.Rare;
            }

            return UpgradeRarity.Epic;
        }

        if (roll < 50)
        {
            return UpgradeRarity.Common;
        }

        if (roll < 88)
        {
            return UpgradeRarity.Rare;
        }

        return UpgradeRarity.Epic;
    }

    private static UpgradeRarity Downgrade(UpgradeRarity rarity) =>
        rarity switch
        {
            UpgradeRarity.Epic => UpgradeRarity.Rare,
            UpgradeRarity.Rare => UpgradeRarity.Common,
            _ => UpgradeRarity.Common
        };

    private static UpgradeCardDef? PickCard(
        int level,
        UpgradeRarity rarity,
        IReadOnlyDictionary<UpgradeCardId, int> stacks,
        HashSet<UpgradeCardId> used,
        Random rng)
    {
        var pool = new List<UpgradeCardDef>();
        foreach (var def in UpgradeCardCatalog.All)
        {
            if (def.Id == UpgradeCardId.RestFallback)
            {
                continue;
            }

            if (def.Rarity != rarity)
            {
                continue;
            }

            if (level < def.MinLevel)
            {
                continue;
            }

            stacks.TryGetValue(def.Id, out var count);
            if (count >= def.MaxStacks)
            {
                continue;
            }

            if (used.Contains(def.Id))
            {
                continue;
            }

            pool.Add(def);
        }

        if (pool.Count == 0)
        {
            return null;
        }

        return pool[rng.Next(pool.Count)];
    }
}
