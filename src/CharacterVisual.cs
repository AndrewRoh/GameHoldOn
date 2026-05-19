using Godot;

namespace GameHoldOn;

/// <summary>Animated or static character sprite with 4-direction walk.</summary>
public partial class CharacterVisual : Node2D
{
    private AnimatedSprite2D? _anim;
    private Sprite2D? _static;
    private Vector2 _lastFacing = Vector2.Down;

    public void SetFacing(Vector2 velocity)
    {
        var moving = velocity.LengthSquared() >= 4f;
        if (_anim != null)
        {
            if (moving)
            {
                if (!_anim.IsPlaying())
                {
                    _anim.Play();
                }
            }
            else
            {
                _anim.Stop();
            }
        }

        if (!moving)
        {
            return;
        }

        _lastFacing = velocity.Normalized();
        UpdateFacing();
    }

    private void UpdateFacing()
    {
        if (_anim == null)
        {
            return;
        }

        var anim = PickDirection(_lastFacing);
        if (_anim.Animation != anim)
        {
            _anim.Play(anim);
        }
    }

    private static string PickDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.X) > Mathf.Abs(dir.Y))
        {
            return dir.X >= 0f ? "walk_east" : "walk_west";
        }

        return dir.Y >= 0f ? "walk_south" : "walk_north";
    }

    public static CharacterVisual? TryCreate(string characterSlug, Vector2 feetOffset)
    {
        var frames = AnimationFramesBuilder.TryBuildWalk(characterSlug);
        if (frames == null || frames.GetAnimationNames().Length == 0)
        {
            return null;
        }

        var root = new CharacterVisual();
        root._anim = new AnimatedSprite2D
        {
            SpriteFrames = frames,
            Animation = "walk_south",
            Centered = false,
            Offset = feetOffset,
            TextureFilter = CanvasItem.TextureFilterEnum.Nearest
        };
        root.AddChild(root._anim);
        root._anim.Play();
        return root;
    }

    public static CharacterVisual? TryCreateStatic(string texturePath, Vector2 feetOffset)
    {
        var tex = ArtPaths.Load(texturePath);
        if (tex == null)
        {
            return null;
        }

        var root = new CharacterVisual();
        root._static = new Sprite2D
        {
            Texture = tex,
            Centered = false,
            Offset = feetOffset,
            TextureFilter = CanvasItem.TextureFilterEnum.Linear
        };
        root.AddChild(root._static);
        return root;
    }
}
