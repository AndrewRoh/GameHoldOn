using Godot;

namespace GameHoldOn;

/// <summary>탑다운 몹. 플레이어를 향해 직진하며 접촉 피해를 준다.</summary>
public partial class Enemy : Node2D
{
    private float _hp;

    public BossKind Kind { get; private set; }
    public float Speed { get; private set; }

    public void Setup(BossKind kind, float hpScale, float speedScale)
    {
        Kind = kind;
        var (color, baseHp, baseSpeed) = kind switch
        {
            BossKind.Hr => (Colors.IndianRed, 28f, 68f),
            BossKind.Ceo => (Colors.MediumPurple, 40f, 52f),
            BossKind.Cto => (Colors.DarkOrange, 34f, 60f),
            _ => (Colors.White, 30f, 60f)
        };

        _hp = baseHp * hpScale;
        Speed = baseSpeed * speedScale;

        var gfx = ArtPaths.TrySprite(ArtPaths.EnemyTexture(kind), ArtPaths.CharacterFeetOffset);
        if (gfx != null)
        {
            AddChild(gfx);
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
    }
}
