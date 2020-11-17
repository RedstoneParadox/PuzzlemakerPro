using Godot;
using System;

public class RuntimeRoot : Node
{
    public static Camera CurrentCamera = null;

    public override void _Process(float delta)
    {
        base._Process(delta);
        DebugOverlay.Information["FPS"] = Engine.GetFramesPerSecond();
        if (CurrentCamera != null) DebugOverlay.Information["Camera Position"] = CurrentCamera.GlobalTransform.origin;
    }
}
