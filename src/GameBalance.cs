using Godot;

namespace GameHoldOn;

/// <summary>Data-driven tuning (see design/gdd/gdd-combat-survival.md).</summary>
public static class GameBalance
{
    public const int TotalWeeks = 8;
    public const float WeekDurationSec = 45f;
    public const float SpawnRadius = 520f;

    public const float PlayerMoveSpeed = 220f;
    public const float PlayerMaxHpBase = 100f;
    public const float PlayerContactDamagePerSec = 18f;
    public const float PlayerContactRadius = 34f;
    public const float PlayerFireCooldown = 0.28f;
    public const float ProjectileSpeed = 640f;
    public const float ProjectileBaseDamage = 11f;
    public const float ProjectileDamagePerWeek = 0.4f;

    /// <summary>Seconds between wave bursts.</summary>
    public const float WaveIntervalBase = 11f;
    public const float WaveIntervalMin = 5f;
    public const float WaveIntervalPerWeek = 0.85f;

    /// <summary>Enemies spawned per wave (staggered).</summary>
    public const int WaveCountBase = 16;
    public const int WaveCountPerWeek = 6;
    public const int WaveCountMax = 58;

    public const float WaveSpawnStagger = 0.045f;

    /// <summary>Steady pressure between waves.</summary>
    public const float TrickleIntervalBase = 0.95f;
    public const float TrickleIntervalMin = 0.32f;
    public const float TrickleIntervalPerWeek = 0.08f;

    public const int MaxEnemiesAlive = 100;

    public const float EnemyHpScalePerWeek = 0.12f;
    public const float EnemySpeedScalePerWeek = 0.08f;

    public const float HrSpawnWeight = 0.45f;
    public const float CeoSpawnWeight = 0.25f;

    public const float HrBaseHp = 28f;
    public const float HrBaseSpeed = 68f;
    public const float CeoBaseHp = 40f;
    public const float CeoBaseSpeed = 52f;
    public const float CtoBaseHp = 34f;
    public const float CtoBaseSpeed = 60f;

    public const int XpHr = 6;
    public const int XpCeo = 10;
    public const int XpCto = 8;
    public const int XpPerLevelBase = 18;
    public const int XpPerLevelGrowth = 8;
    public const float LevelDamageBonus = 2f;
    public const float LevelMaxHpBonus = 8f;

    public static float WaveInterval(int week) =>
        Mathf.Max(WaveIntervalMin, WaveIntervalBase - (week - 1) * WaveIntervalPerWeek);

    public static int WaveEnemyCount(int week) =>
        Mathf.Min(WaveCountMax, WaveCountBase + (week - 1) * WaveCountPerWeek);

    public static float TrickleSpawnInterval(int week) =>
        Mathf.Max(TrickleIntervalMin, TrickleIntervalBase - (week - 1) * TrickleIntervalPerWeek);

    public static float HpScale(int week) => 1f + (week - 1) * EnemyHpScalePerWeek;

    public static float SpeedScale(int week) => 1f + (week - 1) * EnemySpeedScalePerWeek;

    public static int XpToNextLevel(int level) => XpPerLevelBase + level * XpPerLevelGrowth;

    public static int XpForKill(BossKind kind) => kind switch
    {
        BossKind.Hr => XpHr,
        BossKind.Ceo => XpCeo,
        BossKind.Cto => XpCto,
        _ => 5
    };
}
