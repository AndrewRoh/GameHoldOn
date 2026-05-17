using Godot;

namespace GameHoldOn;

/// <summary>WASD 이동, 주변 적에게 자동 투사체.</summary>
public partial class Player : CharacterBody2D
{
    private float _fireCd;
    private float _contactTick;
    private Node2D? _gfx;

    public float Hp { get; private set; } = GameBalance.PlayerMaxHpBase;
    public float MaxHp { get; private set; } = GameBalance.PlayerMaxHpBase;

    public override void _Ready()
    {
        AddToGroup("player");
        var shape = new CollisionShape2D();
        shape.Shape = new CircleShape2D { Radius = 14f };
        AddChild(shape);

        var sprite = ArtPaths.TrySprite(ArtPaths.Player, ArtPaths.CharacterFeetOffset);
        _gfx = sprite != null ? sprite : CreateFallbackBody(Colors.PaleGreen);
        AddChild(_gfx);
    }

    public void RaiseMaxHp(float amount)
    {
        MaxHp += amount;
        Hp = Mathf.Min(MaxHp, Hp + amount);
    }

    public override void _PhysicsProcess(double delta)
    {
        var d = (float)delta;
        var dir = Vector2.Zero;
        if (Input.IsPhysicalKeyPressed(Key.W))
        {
            dir.Y -= 1f;
        }

        if (Input.IsPhysicalKeyPressed(Key.S))
        {
            dir.Y += 1f;
        }

        if (Input.IsPhysicalKeyPressed(Key.A))
        {
            dir.X -= 1f;
        }

        if (Input.IsPhysicalKeyPressed(Key.D))
        {
            dir.X += 1f;
        }

        if (dir != Vector2.Zero)
        {
            dir = dir.Normalized();
        }

        Velocity = dir * GameBalance.PlayerMoveSpeed;
        MoveAndSlide();

        ApplyContactDamage(d);
        TryFire(d);
    }

    public void ApplyDamage(float amount)
    {
        Hp = Mathf.Max(0f, Hp - amount);
    }

    private void ApplyContactDamage(float delta)
    {
        var touched = false;
        foreach (var node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Node2D mob)
            {
                continue;
            }

            if (GlobalPosition.DistanceTo(mob.GlobalPosition) > GameBalance.PlayerContactRadius)
            {
                continue;
            }

            touched = true;
            break;
        }

        if (!touched)
        {
            _contactTick = 0f;
            return;
        }

        _contactTick += delta;
        if (_contactTick < 0.18f)
        {
            return;
        }

        _contactTick = 0f;
        ApplyDamage(GameBalance.PlayerContactDamagePerSec * delta);
    }

    private void TryFire(float delta)
    {
        _fireCd -= delta;
        if (_fireCd > 0f)
        {
            return;
        }

        var target = FindNearestEnemy();
        if (target == null)
        {
            return;
        }

        _fireCd = GameBalance.PlayerFireCooldown;
        var dir = (target.GlobalPosition - GlobalPosition).Normalized();
        if (dir == Vector2.Zero)
        {
            dir = Vector2.Right;
        }

        var main = GetParent() as Main;
        var proj = new Projectile
        {
            Velocity = dir * GameBalance.ProjectileSpeed,
            Damage = main?.ProjectileDamage() ?? GameBalance.ProjectileBaseDamage
        };
        GetParent()?.AddChild(proj);
        proj.GlobalPosition = GlobalPosition + dir * 24f;
    }

    private Node2D? FindNearestEnemy()
    {
        Node2D? best = null;
        var bestD = float.MaxValue;
        foreach (var node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Node2D n)
            {
                continue;
            }

            var dist = GlobalPosition.DistanceSquaredTo(n.GlobalPosition);
            if (dist >= bestD)
            {
                continue;
            }

            bestD = dist;
            best = n;
        }

        return best;
    }

    private static Polygon2D CreateFallbackBody(Color color)
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
        return body;
    }
}
