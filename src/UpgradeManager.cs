using System;
using System.Collections.Generic;
using GameHoldOn.Upgrades;
using Godot;

namespace GameHoldOn;

/// <summary>Level-up card draw, pause UI, and apply (ADR-0002).</summary>
public partial class UpgradeManager : Control
{
    private Main? _main;
    private Player? _player;
    private readonly List<UpgradeOffer> _currentOffers = [];
    private readonly List<Button> _cardButtons = [];
    private Control? _overlay;
    private Label? _titleLabel;
    private bool _isOpen;

    public bool IsOpen => _isOpen;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        MouseFilter = MouseFilterEnum.Ignore;
        Visible = false;
        SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        _overlay = new ColorRect
        {
            Color = new Color(0f, 0f, 0f, 0.72f),
            MouseFilter = MouseFilterEnum.Stop
        };
        _overlay.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        AddChild(_overlay);

        _titleLabel = new Label
        {
            Text = "레벨 업 — 카드 1장 선택",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        _titleLabel.SetAnchorsPreset(LayoutPreset.CenterTop);
        _titleLabel.OffsetTop = 48f;
        _titleLabel.AddThemeFontSizeOverride("font_size", 26);
        AddChild(_titleLabel);

        var row = new HBoxContainer
        {
            Alignment = BoxContainer.AlignmentMode.Center
        };
        row.SetAnchorsPreset(LayoutPreset.Center);
        row.OffsetTop = 20f;
        AddChild(row);

        for (var i = 0; i < 3; i++)
        {
            var btn = new Button
            {
                CustomMinimumSize = new Vector2(280, 160),
                Text = ""
            };
            btn.Pressed += () => OnCardPressed(btn);
            row.AddChild(btn);
            _cardButtons.Add(btn);
        }
    }

    public void Bind(Main main, Player player)
    {
        _main = main;
        _player = player;
    }

    public void OpenSelection(int newLevel)
    {
        if (_isOpen || _player == null || _main == null)
        {
            return;
        }

        _currentOffers.Clear();
        _currentOffers.AddRange(
            UpgradeDrawService.DrawOffers(newLevel, _player.UpgradeStacks, new Random()));

        for (var i = 0; i < _cardButtons.Count; i++)
        {
            if (i >= _currentOffers.Count)
            {
                _cardButtons[i].Visible = false;
                continue;
            }

            var offer = _currentOffers[i];
            _cardButtons[i].Visible = true;
            _cardButtons[i].Text = $"{offer.Title}\n{offer.Description}\n[{i + 1}]";
        }

        if (_titleLabel != null)
        {
            _titleLabel.Text = $"레벨 {newLevel} — 업그레이드 선택";
        }

        _isOpen = true;
        Visible = true;
        MouseFilter = MouseFilterEnum.Stop;
        Engine.TimeScale = 0f;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_isOpen)
        {
            return;
        }

        if (@event is not InputEventKey { Pressed: true, Echo: false } key)
        {
            return;
        }

        var index = key.Keycode switch
        {
            Key.Key1 => 0,
            Key.Key2 => 1,
            Key.Key3 => 2,
            _ => -1
        };

        if (index < 0 || index >= _currentOffers.Count)
        {
            return;
        }

        SelectOffer(index);
        GetViewport().SetInputAsHandled();
    }

    private void OnCardPressed(Button btn)
    {
        var index = _cardButtons.IndexOf(btn);
        if (index < 0 || index >= _currentOffers.Count)
        {
            return;
        }

        SelectOffer(index);
    }

    private void SelectOffer(int index)
    {
        if (!_isOpen || _player == null)
        {
            return;
        }

        var offer = _currentOffers[index];
        _player.ApplyUpgrade(offer.Id);
        CloseSelection();
        _main?.OnUpgradeSelectionFinished();
    }

    private void CloseSelection()
    {
        _isOpen = false;
        Visible = false;
        MouseFilter = MouseFilterEnum.Ignore;
        Engine.TimeScale = 1f;
        _currentOffers.Clear();
    }
}
