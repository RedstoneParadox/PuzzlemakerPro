using Godot;
using System;

public class Camera : Godot.Camera
{
    private Vector3 pitchAxis = Vector3.Right;
    private Vector3 yawAxis = Vector3.Up;
    private PackedScene debug = GD.Load<PackedScene>("res://Scenes/Editor/DEBUG.tscn");

    private float pitch = 0.0f;
    private float length = 20.0f;

    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_up")) pitch -= Mathf.Pi / 90;
        if (Input.IsActionPressed("ui_down")) pitch += Mathf.Pi / 90;

        pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2 + 0.00001f, Mathf.Pi / 2 - 0.00001f);

        if (length < 0.01f) length = 0.01f;

        var translation = new Vector3(0, 0, length);
        translation = translation.Rotated(new Vector3(1, 0, 0), pitch);

        Translation = translation;
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
                    length -= 0.1f;
                }
                if (emb.ButtonIndex == (int)ButtonList.WheelDown)
                {
                    length += 0.1f;
                }
            }
        }
    }
}
