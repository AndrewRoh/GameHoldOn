using Godot;

namespace GameHoldOn;

/// <summary>Orbiting damage satellite (gdd-upgrade-selection.md §4-4, card E2).</summary>
public partial class OrbitalSatellite : Node2D
{
    private const float OrbitSpeedDeg = 150f;
    private const float FireInterval = 0.8f;
    private const float DamageScale = 0.8f;

    private Player? _player;
    private float _angleRad;
    private float _radius;
    private float _fireCd;

    public void Configure(Player player, float radius, float phaseRad)
    {
        _player = player;
        _radius = radius;
        _angleRad = phaseRad;
        _fireCd = 0.2f;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null || !GodotObject.IsInstanceValid(_player))
        {
            QueueFree();
            return;
        }

        var d = (float)delta;
        _angleRad += Mathf.DegToRad(OrbitSpeedDeg) * d;
        GlobalPosition = _player.GlobalPosition + new Vector2(Mathf.Cos(_angleRad), Mathf.Sin(_angleRad)) * _radius;

        _fireCd -= d;
        if (_fireCd > 0f)
        {
            return;
        }

        var target = FindNearestEnemy();
        if (target == null)
        {
            return;
        }

        _fireCd = FireInterval;
        target.TakeDamage(GameBalance.ProjectileBaseDamage * DamageScale);
    }

    private Enemy? FindNearestEnemy()
    {
        Enemy? best = null;
        var bestD = float.MaxValue;
        foreach (var node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Enemy e)
            {
                continue;
            }

            var dist = GlobalPosition.DistanceSquaredTo(e.GlobalPosition);
            if (dist >= bestD)
            {
                continue;
            }

            bestD = dist;
            best = e;
        }

        return best;
    }
}
