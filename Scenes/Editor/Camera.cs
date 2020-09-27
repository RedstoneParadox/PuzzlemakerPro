using Godot;
using System;

public class Camera : Godot.Camera
{
    private Vector3 pitchAxis = Vector3.Right;
    private Vector3 yawAxis = Vector3.Up;

    private float pitch = 0.0f;
    private float yaw = 0.0f;
    private float length = 2.0f;

    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_left")) yaw -= Mathf.Pi / 90;
        if (Input.IsActionPressed("ui_right")) yaw += Mathf.Pi / 90;

        if (Input.IsActionPressed("ui_up")) pitch -= Mathf.Pi / 90;
        if (Input.IsActionPressed("ui_down")) pitch += Mathf.Pi / 90;

        pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2 + 0.00001f, Mathf.Pi / 2 - 0.00001f);

        if (length < 0.01f) length = 0.01f;

        var translation = new Vector3(0, 0, length).Rotated(Vector3.Up, yaw);
        var pitchAxis = new Vector3(1, 0, 0).Rotated(Vector3.Up, yaw);
        translation = translation.Rotated(pitchAxis, pitch);

        this.Translation = translation;
        LookAt(Vector3.Zero, Vector3.Up);
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
