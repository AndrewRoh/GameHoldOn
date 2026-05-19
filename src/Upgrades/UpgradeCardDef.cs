namespace GameHoldOn.Upgrades;

public sealed record UpgradeCardDef(
    UpgradeCardId Id,
    UpgradeRarity Rarity,
    string Title,
    string Description,
    int MinLevel,
    int MaxStacks);

public sealed record UpgradeOffer(UpgradeCardId Id, string Title, string Description);
