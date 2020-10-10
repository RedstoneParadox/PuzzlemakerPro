using Godot;
using System;

public class RuntimeRoot : Node
{
    private readonly PackedScene LevelScene = GD.Load<PackedScene>("res://Scenes/Editor/Level.tscn");
    
    public override void _Ready()
    {
        AddChild(LevelScene.Instance());
    }
}
