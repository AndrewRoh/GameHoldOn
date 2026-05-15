using Godot;

namespace GameHoldOn;

/// <summary>프롤로그 텍스트 후 메인 게임 씬으로 전환.</summary>
public partial class Prologue : Control
{
    private bool _went;

    public override void _Ready()
    {
        BuildUi();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_went)
        {
            return;
        }

        if (!@event.IsPressed())
        {
            return;
        }

        if (@event is InputEventKey { Echo: true })
        {
            return;
        }

        switch (@event)
        {
            case InputEventKey key when key.Keycode is Key.Space or Key.Enter:
                GoMain();
                GetViewport().SetInputAsHandled();
                return;
            case InputEventMouseButton mouse when mouse.ButtonIndex == MouseButton.Left:
                GoMain();
                GetViewport().SetInputAsHandled();
                break;
        }
    }

    private void BuildUi()
    {
        MouseFilter = MouseFilterEnum.Stop;
        SetAnchorsPreset(LayoutPreset.FullRect);

        var bg = new ColorRect
        {
            Color = new Color(0.05f, 0.06f, 0.09f, 1f),
            MouseFilter = MouseFilterEnum.Ignore
        };
        bg.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(bg);

        var quote = new Label
        {
            Text =
                "음악가는 음악을 만들고, 시인은 시를 쓴다.\n\n" +
                "게임 개발자는 게임으로 자신을 표현하는 게 맞다고, 문득 생각이 들었다.",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        quote.AddThemeFontSizeOverride("font_size", 26);
        quote.SetAnchorsPreset(LayoutPreset.FullRect);
        quote.OffsetLeft = 72f;
        quote.OffsetTop = 140f;
        quote.OffsetRight = -72f;
        quote.OffsetBottom = -160f;
        quote.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(quote);

        var hint = new Label
        {
            Text = "Space / Enter / 클릭 — 시작",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom
        };
        hint.AddThemeFontSizeOverride("font_size", 16);
        hint.SetAnchorsPreset(LayoutPreset.FullRect);
        hint.OffsetTop = -100f;
        hint.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(hint);
    }

    private void GoMain()
    {
        if (_went)
        {
            return;
        }

        _went = true;
        GetTree().ChangeSceneToFile("res://main.tscn");
    }
}
