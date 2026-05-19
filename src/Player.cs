using System.Collections.Generic;
using GameHoldOn.Upgrades;
using Godot;

namespace GameHoldOn;

/// <summary>WASD 이동, 주변 적에게 자동 투사체, 카드 스택 기반 스탯.</summary>
public partial class Player : CharacterBody2D
{
    private static readonly float[] MultiShotAnglesDeg = [0f, -12f, 12f, -24f];

    private readonly Dictionary<UpgradeCardId, int> _stacks = new();
    private readonly List<OrbitalSatellite> _orbitals = [];

    private float _fireCd;
    private float _contactTick;
    private CharacterVisual? _gfx;
    private PlayerCombatStats _stats = PlayerCombatStats.FromStacks(new Dictionary<UpgradeCardId, int>());

    public float Hp { get; private set; } = GameBalance.PlayerMaxHpBase;
    public float MaxHp { get; private set; } = GameBalance.PlayerMaxHpBase;
    public float FireCooldown => _stats.FireCooldown;
    public float DamageMultiplier => _stats.DamageMultiplier;
    public float MoveSpeed => _stats.MoveSpeed;
    public float XpGainMultiplier => _stats.XpGainMultiplier;
    public IReadOnlyDictionary<UpgradeCardId, int> UpgradeStacks => _stacks;

    public override void _Ready()
    {
        AddToGroup("player");
        var shape = new CollisionShape2D();
        shape.Shape = new CircleShape2D { Radius = 14f };
        AddChild(shape);

        _gfx = ArtPaths.TryCharacter(ArtPaths.PlayerSlug, ArtPaths.Player);
        if (_gfx != null)
        {
            AddChild(_gfx);
        }
        else
        {
            AddChild(CreateFallbackBody(Colors.PaleGreen));
        }
        RebuildCombatStats();
    }

    public void ApplyUpgrade(UpgradeCardId id)
    {
        if (id == UpgradeCardId.RestFallback)
        {
            Hp = Mathf.Min(MaxHp, Hp + 15f);
            return;
        }

        _stacks.TryGetValue(id, out var count);
        var def = UpgradeCardCatalog.Get(id);
        if (count >= def.MaxStacks)
        {
            return;
        }

        _stacks[id] = count + 1;
        RebuildCombatStats();

        if (id == UpgradeCardId.PtoUse)
        {
            Hp = Mathf.Min(MaxHp, Hp + 30f);
        }
    }

    private void RebuildCombatStats()
    {
        var prevMax = MaxHp;
        _stats = PlayerCombatStats.FromStacks(_stacks);
        MaxHp = GameBalance.PlayerMaxHpBase + _stats.MaxHpBonus;
        if (MaxHp > prevMax)
        {
            Hp += MaxHp - prevMax;
        }

        Hp = Mathf.Min(Hp, MaxHp);
        SyncOrbitals();
    }

    private void SyncOrbitals()
    {
        foreach (var orbital in _orbitals)
        {
            orbital.QueueFree();
        }

        _orbitals.Clear();

        if (_stats.OrbitalCount <= 0)
        {
            return;
        }

        AddOrbital(80f, 0f);
        if (_stats.OrbitalCount >= 2)
        {
            AddOrbital(100f, Mathf.Pi);
        }
    }

    private void AddOrbital(float radius, float phase)
    {
        var orbital = new OrbitalSatellite();
        AddChild(orbital);
        orbital.Configure(this, radius, phase);
        _orbitals.Add(orbital);
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

        Velocity = dir * _stats.MoveSpeed;
        MoveAndSlide();
        if (dir != Vector2.Zero)
        {
            _gfx?.SetFacing(dir);
        }

        if (_stats.HpRegenPerSec > 0f)
        {
            Hp = Mathf.Min(MaxHp, Hp + _stats.HpRegenPerSec * d);
        }

        ApplyContactDamage(d);
        TryFire(d);
    }

    public void ApplyDamage(float amount)
    {
        Hp = Mathf.Max(0f, Hp - amount * _stats.DamageTakenMultiplier);
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
        if (_stats.ContactDodgeChance > 0f && GD.Randf() < _stats.ContactDodgeChance)
        {
            return;
        }

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

        _fireCd = _stats.FireCooldown;
        var baseDir = (target.GlobalPosition - GlobalPosition).Normalized();
        if (baseDir == Vector2.Zero)
        {
            baseDir = Vector2.Right;
        }

        var main = GetParent() as Main;
        var baseDmg = GameBalance.ProjectileBaseDamage * _stats.DamageMultiplier;
        var weekBonus = main != null ? main.Week * GameBalance.ProjectileDamagePerWeek : 0f;
        var damage = baseDmg + weekBonus;
        if (_stats.CritChance > 0f && GD.Randf() < _stats.CritChance)
        {
            damage *= _stats.CritMultiplier;
        }

        var count = _stats.ProjectileCount;
        for (var i = 0; i < count; i++)
        {
            var angleDeg = i < MultiShotAnglesDeg.Length ? MultiShotAnglesDeg[i] : 0f;
            var dir = baseDir.Rotated(Mathf.DegToRad(angleDeg));
            SpawnProjectile(dir, damage);
        }
    }

    private void SpawnProjectile(Vector2 dir, float damage)
    {
        var proj = new Projectile
        {
            Velocity = dir * _stats.ProjectileSpeed,
            Damage = damage,
            PierceCount = _stats.PierceCount
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
