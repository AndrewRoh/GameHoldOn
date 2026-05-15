using Godot;

namespace GameHoldOn;

/// <summary>뱀서류 MVP — 8주 생존, HR/CEO/CTO 스폰, 승패 처리.</summary>
public partial class Main : Node2D
{
    private const float WeekDurationSec = 45f;
    private const float SpawnRadius = 520f;

    private Camera2D? _camera;
    private Player? _player;
    private Label? _hud;
    private float _weekTimer;
    private float _spawnTimer;
    private bool _gameEnded;

    public int Week { get; private set; } = 1;

    public override void _Ready()
    {
        _camera = GetNodeOrNull<Camera2D>("Camera2D");
        BuildFloor();

        _player = new Player();
        _player.Name = "Player";
        AddChild(_player);

        var ui = new CanvasLayer();
        AddChild(ui);
        _hud = new Label();
        _hud.Position = new Vector2(16, 12);
        _hud.AddThemeFontSizeOverride("font_size", 18);
        ui.AddChild(_hud);

        _spawnTimer = 0.35f;
        UpdateHud();
    }

    public override void _Process(double delta)
    {
        var d = (float)delta;
        if (_gameEnded || _player == null)
        {
            return;
        }

        if (_player.Hp <= 0f)
        {
            GameOver();
            return;
        }

        _weekTimer += d;
        if (_weekTimer >= WeekDurationSec)
        {
            _weekTimer = 0f;
            if (Week >= 8)
            {
                Win();
                return;
            }

            Week++;
        }

        _spawnTimer -= d;
        if (_spawnTimer <= 0f)
        {
            SpawnEnemy();
            _spawnTimer = SpawnInterval();
        }

        if (_camera != null)
        {
            _camera.GlobalPosition = _player.GlobalPosition;
        }

        UpdateHud();
    }

    private float SpawnInterval()
    {
        return Mathf.Max(0.55f, 2.4f - (Week - 1) * 0.22f);
    }

    private void SpawnEnemy()
    {
        if (_player == null)
        {
            return;
        }

        var kind = RollBossKind();
        var hpScale = 1f + (Week - 1) * 0.12f;
        var speedScale = 1f + (Week - 1) * 0.08f;

        var enemy = new Enemy();
        enemy.GlobalPosition = RandomRingPoint(_player.GlobalPosition, SpawnRadius);
        enemy.Setup(kind, hpScale, speedScale);
        AddChild(enemy);
    }

    private static BossKind RollBossKind()
    {
        var r = GD.Randf();
        if (r < 0.45f)
        {
            return BossKind.Hr;
        }

        if (r < 0.70f)
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

    private void UpdateHud()
    {
        if (_hud == null || _player == null)
        {
            return;
        }

        if (_gameEnded)
        {
            return;
        }

        var timeLeft = Mathf.Max(0f, WeekDurationSec - _weekTimer);
        var phase = Week >= 8 ? "클리어까지" : "다음 주까지";
        _hud.Text =
            $"주차: {Week}/8  ({phase} {timeLeft:0}s)\n" +
            $"HP: {_player.Hp:0}/{_player.MaxHp:0}\n" +
            "이동: WASD";
    }

    private void Win()
    {
        if (_gameEnded)
        {
            return;
        }

        _gameEnded = true;
        ClearEnemies();
        if (_hud != null)
        {
            _hud.Text = "승리 — 8주를 버텼습니다.\n분기 생존 (R 키로 재시작은 아직 없음)";
        }
    }

    private void GameOver()
    {
        if (_gameEnded)
        {
            return;
        }

        _gameEnded = true;
        ClearEnemies();
        if (_hud != null)
        {
            _hud.Text = "패배 — 구조조정 라인에 걸렸습니다.\n(에디터에서 F5 재실행)";
        }
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
        var tex = ArtPaths.Load(ArtPaths.FloorTile);
        if (tex == null)
        {
            return;
        }

        var floor = new Node2D { Name = "Floor", ZIndex = -10 };
        const int extent = 2048;
        const int tile = 128;
        for (var y = -extent; y < extent; y += tile)
        {
            for (var x = -extent; x < extent; x += tile)
            {
                var tileSprite = new Sprite2D
                {
                    Texture = tex,
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
