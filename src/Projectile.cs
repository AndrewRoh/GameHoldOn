using System.Collections.Generic;
using Godot;

namespace GameHoldOn;

/// <summary>가장 가까운 적 방향으로 날아가며 충돌 시 피해를 준다.</summary>
public partial class Projectile : Node2D
{
    private const float HitRadius = 22f;
    private const float Lifetime = 2.2f;

    private float _life;
    private readonly HashSet<ulong> _hitIds = [];
    public float Damage { get; set; } = 12f;
    public Vector2 Velocity { get; set; }
    public int PierceCount { get; set; }

    public override void _Ready()
    {
        _life = Lifetime;
        var sprite = ArtPaths.TrySprite(ArtPaths.Projectile, ArtPaths.ProjectileCenterOffset);
        if (sprite != null)
        {
            AddChild(sprite);
            return;
        }

        var fallback = new Polygon2D();
        fallback.Polygon = new[]
        {
            new Vector2(-4, -4),
            new Vector2(4, -4),
            new Vector2(4, 4),
            new Vector2(-4, 4)
        };
        fallback.Color = Colors.LightGoldenrod;
        AddChild(fallback);
    }

    public override void _PhysicsProcess(double delta)
    {
        var d = (float)delta;
        GlobalPosition += Velocity * d;
        _life -= d;
        if (_life <= 0f)
        {
            QueueFree();
            return;
        }

        var maxHits = 1 + PierceCount;
        if (_hitIds.Count >= maxHits)
        {
            QueueFree();
            return;
        }

        foreach (var node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Enemy enemy)
            {
                continue;
            }

            var id = enemy.GetInstanceId();
            if (_hitIds.Contains(id))
            {
                continue;
            }

            if (GlobalPosition.DistanceTo(enemy.GlobalPosition) > HitRadius)
            {
                continue;
            }

            enemy.TakeDamage(Damage);
            _hitIds.Add(id);
            if (_hitIds.Count >= maxHits)
            {
                QueueFree();
            }

            return;
        }
    }
}
