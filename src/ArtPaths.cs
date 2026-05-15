using Godot;

namespace GameHoldOn;

/// <summary>Combat visual texture paths (see design/assets/specs/combat-visual-assets.md).</summary>
public static class ArtPaths
{
    public const string Player = "res://assets/art/sprites/chr_player_dev.png";
    public const string EnemyHr = "res://assets/art/sprites/chr_enemy_hr.png";
    public const string EnemyCeo = "res://assets/art/sprites/chr_enemy_ceo.png";
    public const string EnemyCto = "res://assets/art/sprites/chr_enemy_cto.png";
    public const string Projectile = "res://assets/art/sprites/prj_commit_default.png";
    public const string FloorTile = "res://assets/art/tiles/env_floor_office.png";

    public static Texture2D? Load(string path)
    {
        if (!ResourceLoader.Exists(path))
        {
            GD.PushWarning($"Missing art: {path}");
            return null;
        }

        return GD.Load<Texture2D>(path);
    }

    public static Sprite2D? TrySprite(string path, Vector2 feetOffset)
    {
        var tex = Load(path);
        if (tex == null)
        {
            return null;
        }

        var sprite = new Sprite2D
        {
            Texture = tex,
            Centered = false,
            Offset = feetOffset,
            TextureFilter = CanvasItem.TextureFilterEnum.Linear
        };
        return sprite;
    }

    public static string EnemyTexture(BossKind kind) => kind switch
    {
        BossKind.Hr => EnemyHr,
        BossKind.Ceo => EnemyCeo,
        BossKind.Cto => EnemyCto,
        _ => EnemyHr
    };

    /// <summary>Feet-aligned pivot for 64×64 sprites.</summary>
    public static readonly Vector2 CharacterFeetOffset = new(32f, 56f);
}
