using Godot;
using System;

public class RuntimeRoot : Node
{
    public static Camera CurrentCamera = new Camera();
    private readonly PackedScene LevelScene = GD.Load<PackedScene>("res://Scenes/Editor/Level.tscn");
    
    public override void _Ready()
    {
        AddChild(LevelScene.Instance());
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        DebugOverlay.Information["FPS"] = Engine.GetFramesPerSecond();
        DebugOverlay.Information["Camera Position"] = CurrentCamera.GlobalTransform.origin;
    }
}
