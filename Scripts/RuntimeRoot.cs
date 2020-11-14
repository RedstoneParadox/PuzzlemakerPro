using Godot;
using System;

public class RuntimeRoot : Node
{
    public static Camera CurrentCamera = new Camera();

    public override void _Process(float delta)
    {
        base._Process(delta);
        DebugOverlay.Information["FPS"] = Engine.GetFramesPerSecond();
        DebugOverlay.Information["Camera Position"] = CurrentCamera.GlobalTransform.origin;
    }
}
