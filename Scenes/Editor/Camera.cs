using Godot;
using System;

public class Camera : Godot.Camera
{
    private float length = 20.0f;

    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("scroll_up"))
        {
            length -= 0.25f;
        }
        if (Input.IsActionJustPressed("scroll_down"))
        {
            length += 0.25f;
        }

        if (length < 0.01f) length = 0.01f;

        Translation = new Vector3(0, 0, length);
        LookAt(GetParent<Spatial>().Translation, Vector3.Up);
    }
}
