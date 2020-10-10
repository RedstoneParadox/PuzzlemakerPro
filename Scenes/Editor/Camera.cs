using Godot;
using System;

public class Camera : Godot.Camera
{
    private PackedScene debug = GD.Load<PackedScene>("res://Scenes/Editor/DEBUG.tscn");
    private float length = 20.0f;

    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        if (length < 0.01f) length = 0.01f;

        Translation = new Vector3(0, 0, length);
        LookAt(GetParent<Spatial>().Translation, Vector3.Up);

        if (Input.IsActionJustPressed("debug_place"))
        {
            var spaceState = GetWorld().Space;
            var pos = GetViewport().GetMousePosition();

            Vector3 target = ProjectRayOrigin(pos);

            var instance = debug.Instance() as MeshInstance;
            instance.Translation = target;

            GetParent().AddChild(debug.Instance());
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            InputEventMouseButton emb = (InputEventMouseButton)@event;
            if (emb.IsPressed())
            {
                if (emb.ButtonIndex == (int)ButtonList.WheelUp)
                {
                    length -= 0.25f;
                }
                if (emb.ButtonIndex == (int)ButtonList.WheelDown)
                {
                    length += 0.25f;
                }
            }
        }
    }
}
