using System;
using Godot;

namespace GameHoldOn;

/// <summary>탑다운 몹. 플레이어를 향해 직진하며 접촉 피해를 준다.</summary>
public partial class Enemy : Node2D
{
    public event Action<BossKind>? Killed;

    private float _hp;
    private CharacterVisual? _gfx;

    public BossKind Kind { get; private set; }
    public float Speed { get; private set; }

    public void Setup(BossKind kind, float hpScale, float speedScale)
    {
        Kind = kind;
        var (color, baseHp, baseSpeed) = kind switch
        {
            BossKind.Hr => (Colors.IndianRed, GameBalance.HrBaseHp, GameBalance.HrBaseSpeed),
            BossKind.Ceo => (Colors.MediumPurple, GameBalance.CeoBaseHp, GameBalance.CeoBaseSpeed),
            BossKind.Cto => (Colors.DarkOrange, GameBalance.CtoBaseHp, GameBalance.CtoBaseSpeed),
            _ => (Colors.White, 30f, 60f)
        };

        _hp = baseHp * hpScale;
        Speed = baseSpeed * speedScale;

        var slug = ArtPaths.EnemySlug(kind);
        _gfx = ArtPaths.TryCharacter(slug, ArtPaths.EnemyTexture(kind));
        if (_gfx != null)
        {
            AddChild(_gfx);
        }
        else
        {
            var body = new Polygon2D();
            body.Polygon = new[]
            {
                new Vector2(-14, -14),
                new Vector2(14, -14),
                new Vector2(14, 14),
                new Vector2(-14, 14)
            };
            body.Color = color;
            AddChild(body);
        }

        AddToGroup("enemies");
    }

    public void TakeDamage(float amount)
    {
        _hp -= amount;
        if (_hp > 0f)
        {
            return;
        }

        RemoveFromGroup("enemies");
        Killed?.Invoke(Kind);
        QueueFree();
    }

    public override void _Process(double delta)
    {
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count == 0)
        {
            return;
        }

        if (players[0] is not Node2D player)
        {
            return;
        }

        var dir = (player.GlobalPosition - GlobalPosition).Normalized();
        if (dir == Vector2.Zero)
        {
            dir = Vector2.Right;
        }

        GlobalPosition += dir * Speed * (float)delta;
        _gfx?.SetFacing(dir);
    }
}
