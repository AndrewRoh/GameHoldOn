using Godot;

namespace GameHoldOn;

/// <summary>뱀서류 MVP — 8주 생존, HR/CEO/CTO 스폰, XP·레벨·카드 선택, 승패.</summary>
public partial class Main : Node2D
{
    private Camera2D? _camera;
    private Player? _player;
    private GameHud? _hud;
    private UpgradeManager? _upgradeManager;
    private float _weekTimer;
    private float _waveTimer;
    private int _waveBurstLeft;
    private float _waveBurstCd;
    private float _trickleTimer;
    private bool _gameEnded;

    public int Week { get; private set; } = 1;
    public int Wave { get; private set; }
    public int AliveEnemies => GetTree().GetNodesInGroup("enemies").Count;
    public float WeekTimer => _weekTimer;
    public int Level { get; private set; } = 1;
    public int Xp { get; private set; }
    public int XpToNext => GameBalance.XpToNextLevel(Level);
    public bool IsMaxLevel => Level >= GameBalance.MaxLevel;
    public bool IsUpgradeSelectionOpen => _upgradeManager?.IsOpen ?? false;

    public override void _Ready()
    {
        AddToGroup("game_root");
        _camera = GetNodeOrNull<Camera2D>("Camera2D");
        BuildFloor();

        _player = new Player();
        _player.Name = "Player";
        AddChild(_player);

        var ui = new CanvasLayer();
        AddChild(ui);
        _hud = new GameHud();
        ui.AddChild(_hud);

        _upgradeManager = new UpgradeManager();
        ui.AddChild(_upgradeManager);
        _upgradeManager.Bind(this, _player);

        _waveTimer = 4f;
        _trickleTimer = 0.4f;
        StartWave();
        RefreshHud();
    }

    public override void _Process(double delta)
    {
        var d = (float)delta;
        if (_gameEnded || _player == null || IsUpgradeSelectionOpen)
        {
            if (!IsUpgradeSelectionOpen && _camera != null && _player != null)
            {
                _camera.GlobalPosition = _player.GlobalPosition;
            }

            return;
        }

        if (_player.Hp <= 0f)
        {
            EndGame(false);
            return;
        }

        _weekTimer += d;
        if (_weekTimer >= GameBalance.WeekDurationSec)
        {
            _weekTimer = 0f;
            if (Week >= GameBalance.TotalWeeks)
            {
                EndGame(true);
                return;
            }

            Week++;
            _hud?.ShowBanner($"—— {Week}주차 시작 ——", 1.6f);
        }

        UpdateSpawning(d);

        if (_camera != null)
        {
            _camera.GlobalPosition = _player.GlobalPosition;
        }

        RefreshHud();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_gameEnded)
        {
            return;
        }

        if (@event is not InputEventKey { Pressed: true, Echo: false } key)
        {
            return;
        }

        if (key.Keycode is Key.R or Key.Enter or Key.Space)
        {
            GetTree().ReloadCurrentScene();
            GetViewport().SetInputAsHandled();
        }
    }

    public void OnEnemyKilled(BossKind kind)
    {
        if (_gameEnded || _player == null)
        {
            return;
        }

        var xp = (int)(GameBalance.XpForKill(kind) * _player.XpGainMultiplier);
        Xp += Math.Max(1, xp);

        if (IsMaxLevel || Xp < XpToNext)
        {
            return;
        }

        if (_player.Hp <= 0f)
        {
            return;
        }

        Xp -= XpToNext;
        LevelUp();
    }

    private void LevelUp()
    {
        if (IsMaxLevel)
        {
            return;
        }

        Level++;
        _hud?.ShowBanner($"레벨 업! Lv {Level}", 0.2f);
        _upgradeManager?.OpenSelection(Level);
    }

    public void OnUpgradeSelectionFinished()
    {
        RefreshHud();
    }

    private void UpdateSpawning(float delta)
    {
        if (_waveBurstLeft > 0)
        {
            _waveBurstCd -= delta;
            if (_waveBurstCd <= 0f)
            {
                if (AliveEnemies < GameBalance.MaxEnemiesAlive)
                {
                    SpawnEnemy();
                    _waveBurstLeft--;
                }

                _waveBurstCd = GameBalance.WaveSpawnStagger;
            }
        }

        _waveTimer -= delta;
        if (_waveTimer <= 0f)
        {
            StartWave();
            _waveTimer = GameBalance.WaveInterval(Week);
        }

        _trickleTimer -= delta;
        if (_trickleTimer <= 0f)
        {
            if (AliveEnemies < GameBalance.MaxEnemiesAlive)
            {
                SpawnEnemy();
            }

            _trickleTimer = GameBalance.TrickleSpawnInterval(Week);
        }
    }

    private void StartWave()
    {
        Wave++;
        _waveBurstLeft = GameBalance.WaveEnemyCount(Week);
        _waveBurstCd = 0f;
        if (!IsUpgradeSelectionOpen)
        {
            _hud?.ShowBanner($"⚠ 웨이브 {Wave}  (적 {_waveBurstLeft}마리)", 1.5f);
        }
    }

    private void SpawnEnemy()
    {
        if (_player == null || AliveEnemies >= GameBalance.MaxEnemiesAlive)
        {
            return;
        }

        var kind = RollBossKind();
        var enemy = new Enemy();
        enemy.GlobalPosition = RandomRingPoint(_player.GlobalPosition, GameBalance.SpawnRadius);
        enemy.Setup(kind, GameBalance.HpScale(Week), GameBalance.SpeedScale(Week));
        enemy.Killed += OnEnemyKilled;
        AddChild(enemy);
    }

    private static BossKind RollBossKind()
    {
        var r = GD.Randf();
        if (r < GameBalance.HrSpawnWeight)
        {
            return BossKind.Hr;
        }

        if (r < GameBalance.HrSpawnWeight + GameBalance.CeoSpawnWeight)
        {
            return BossKind.Ceo;
        }

        return BossKind.Cto;
    }

    private static Vector2 RandomRingPoint(Vector2 center, float radius)
    {
        var ang = GD.Randf() * Mathf.Tau;
        return center + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
    }

    private void RefreshHud()
    {
        if (_hud == null || _player == null || _gameEnded)
        {
            return;
        }

        _hud.Update(this, _player);
    }

    private void EndGame(bool won)
    {
        if (_gameEnded)
        {
            return;
        }

        _gameEnded = true;
        if (_upgradeManager?.IsOpen == true)
        {
            Engine.TimeScale = 1f;
        }

        ClearEnemies();

        if (_hud == null)
        {
            return;
        }

        _hud.ShowEndScreen(won
            ? $"승리 — {GameBalance.TotalWeeks}주를 버텼습니다.\n레벨 {Level} · R/Enter/Space 재시작"
            : $"패배 — 구조조정 라인에 걸렸습니다.\n레벨 {Level} · R/Enter/Space 재시작");
    }

    private void ClearEnemies()
    {
        foreach (var n in GetTree().GetNodesInGroup("enemies"))
        {
            if (n is Node node)
            {
                node.QueueFree();
            }
        }
    }

    private void BuildFloor()
    {
        var texA = ArtPaths.Load(ArtPaths.FloorTile);
        var texB = ArtPaths.Load(ArtPaths.FloorTileAlt);
        if (texA == null)
        {
            return;
        }

        var floor = new Node2D { Name = "Floor", ZIndex = -10 };
        const int extent = 2048;
        const int tile = 64;
        for (var y = -extent; y < extent; y += tile)
        {
            for (var x = -extent; x < extent; x += tile)
            {
                var useAlt = texB != null && ((x / tile + y / tile) % 2 == 0);
                var tileSprite = new Sprite2D
                {
                    Texture = useAlt ? texB : texA,
                    Position = new Vector2(x, y),
                    Centered = false,
                    TextureFilter = CanvasItem.TextureFilterEnum.Linear
                };
                floor.AddChild(tileSprite);
            }
        }

        AddChild(floor);
        MoveChild(floor, 0);
    }
}
