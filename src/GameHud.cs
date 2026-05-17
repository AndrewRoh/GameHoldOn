using Godot;

namespace GameHoldOn;

/// <summary>Week / HP bar / XP readout.</summary>
public partial class GameHud : Control
{
    private Label? _label;
    private ProgressBar? _hpBar;

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.TopLeft);
        Position = new Vector2(16, 12);

        _label = new Label();
        _label.AddThemeFontSizeOverride("font_size", 18);
        AddChild(_label);

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

    public void Update(Main game, Player player)
    {
        if (_label == null || _hpBar == null)
        {
            return;
        }

        var timeLeft = Mathf.Max(0f, GameBalance.WeekDurationSec - game.WeekTimer);
        var phase = game.Week >= GameBalance.TotalWeeks ? "클리어까지" : "다음 주까지";
        _label.Text =
            $"주차: {game.Week}/{GameBalance.TotalWeeks}  ({phase} {timeLeft:0}s)\n" +
            $"웨이브: {game.Wave}  적: {game.AliveEnemies}\n" +
            $"레벨: {game.Level}  XP: {game.Xp}/{game.XpToNext}\n" +
            $"HP: {player.Hp:0}/{player.MaxHp:0}\n" +
            "이동: WASD";

        _hpBar.MaxValue = player.MaxHp;
        _hpBar.Value = player.Hp;
    }

    public void ShowEndScreen(string message)
    {
        if (_label != null)
        {
            _label.Text = message;
        }
    }
}
