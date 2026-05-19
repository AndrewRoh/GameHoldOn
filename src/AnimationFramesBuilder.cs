using System.Collections.Generic;
using Godot;

namespace GameHoldOn;

/// <summary>Builds SpriteFrames from PixelLab walk frame folders under res://assets/art/animations/{slug}/.</summary>
public static class AnimationFramesBuilder
{
    private static readonly string[] Directions = ["south", "east", "north", "west"];

    public static SpriteFrames? TryBuildWalk(string characterSlug)
    {
        var baseDir = $"res://assets/art/animations/{characterSlug}";
        if (!DirAccess.DirExistsAbsolute(baseDir))
        {
            return null;
        }

        var frames = new SpriteFrames();
        var any = false;

        foreach (var dir in Directions)
        {
            var animName = $"walk_{dir}";
            var pattern = $"{baseDir}/walk/{dir}";
            if (!DirAccess.DirExistsAbsolute(pattern))
            {
                continue;
            }

            frames.AddAnimation(animName);
            frames.SetAnimationSpeed(animName, 8f);
            frames.SetAnimationLoop(animName, true);

            var paths = CollectFramePaths(pattern);
            foreach (var path in paths)
            {
                var tex = GD.Load<Texture2D>(path);
                if (tex != null)
                {
                    frames.AddFrame(animName, tex);
                    any = true;
                }
            }
        }

        return any ? frames : null;
    }

    private static List<string> CollectFramePaths(string dirPath)
    {
        var list = new List<string>();
        var access = DirAccess.Open(dirPath);
        if (access == null)
        {
            return list;
        }

        access.ListDirBegin();
        var file = access.GetNext();
        while (file != "")
        {
            if (!access.CurrentIsDir() && file.EndsWith(".png"))
            {
                list.Add($"{dirPath}/{file}");
            }

            file = access.GetNext();
        }

        access.ListDirEnd();
        list.Sort();
        return list;
    }
}
