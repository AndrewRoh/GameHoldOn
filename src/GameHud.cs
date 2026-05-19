using Godot;

namespace GameHoldOn;

/// <summary>Week / HP bar / XP / flash banners.</summary>
public partial class GameHud : Control
{
    private Label? _label;
    private Label? _banner;
    private ProgressBar? _hpBar;
    private float _bannerTimer;

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.TopLeft);
        Position = new Vector2(16, 12);

        _label = new Label();
        _label.AddThemeFontSizeOverride("font_size", 18);
        AddChild(_label);

        _banner = new Label
        {
            Position = new Vector2(0, 100),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _banner.AddThemeFontSizeOverride("font_size", 22);
        _banner.Modulate = new Color(1f, 0.92f, 0.45f);
        AddChild(_banner);

        _hpBar = new ProgressBar
        {
            Position = new Vector2(0, 72),
            CustomMinimumSize = new Vector2(200, 14),
            MaxValue = 100,
            Value = 100,
            ShowPercentage = false
        };
        AddChild(_hpBar);
    }

    public override void _Process(double delta)
    {
        if (_bannerTimer <= 0f || _banner == null)
        {
            return;
        }

        _bannerTimer -= (float)delta;
        if (_bannerTimer <= 0f)
        {
            _banner.Text = "";
        }
    }

    public void Update(Main game, Player player)
    {
        if (_label == null || _hpBar == null)
        {
            return;
        }

        var timeLeft = Mathf.Max(0f, GameBalance.WeekDurationSec - game.WeekTimer);
        var phase = game.Week >= GameBalance.TotalWeeks ? "클리어까지" : "다음 주까지";
        var xpLine = game.Level >= GameBalance.MaxLevel
            ? $"레벨: MAX  XP: {game.Xp}"
            : $"레벨: {game.Level}  XP: {game.Xp}/{game.XpToNext}";

        _label.Text =
            $"주차: {game.Week}/{GameBalance.TotalWeeks}  ({phase} {timeLeft:0}s)\n" +
            $"웨이브: {game.Wave}  적: {game.AliveEnemies}\n" +
            $"{xpLine}\n" +
            $"HP: {player.Hp:0}/{player.MaxHp:0}  발사: {player.FireCooldown:0.00}s\n" +
            "이동: WASD";

        _hpBar.MaxValue = player.MaxHp;
        _hpBar.Value = player.Hp;
    }

    public void ShowBanner(string text, float seconds = 1.4f)
    {
        if (_banner == null)
        {
            return;
        }

        _banner.Text = text;
        _bannerTimer = seconds;
    }

    public void ShowEndScreen(string message)
    {
        if (_label != null)
        {
            _label.Text = message;
        }

        if (_banner != null)
        {
            _banner.Text = "";
            _bannerTimer = 0f;
        }
    }
}
