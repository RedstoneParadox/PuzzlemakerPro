using Godot;
using System;

public class CameraBase : Spatial
{
    private float yaw = Mathf.Pi/4.0f;
    private float pitch = -Mathf.Pi/6.0f;
    private float length = 20.0f;
    private float panSpeed = 0.25f;

    public override void _Ready()
    {
        RuntimeRoot.CurrentCamera = GetNode<Camera>("Camera");
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        var targetOffset = new Vector3(0, 0, 0);

        if (Input.IsActionPressed("pan_up")) targetOffset.y += 1;
        if (Input.IsActionPressed("pan_down")) targetOffset.y -= 1;
        if (Input.IsActionPressed("pan_right")) targetOffset.x += 1;
        if (Input.IsActionPressed("pan_left")) targetOffset.x -= 1;

        if (Input.IsActionPressed("ui_left")) yaw -= Mathf.Pi / 90;
        if (Input.IsActionPressed("ui_right")) yaw += Mathf.Pi / 90;

        if (Input.IsActionPressed("ui_up")) pitch -= Mathf.Pi / 90;
        if (Input.IsActionPressed("ui_down")) pitch += Mathf.Pi / 90;

        pitch = Mathf.Clamp(pitch, -Mathf.Pi / 2 + 0.00001f, Mathf.Pi / 2 - 0.00001f);

        Rotation = new Vector3(pitch, yaw, 0);
        var pitchAxis = Vector3.Right.Rotated(Vector3.Up, yaw);
        Translation += targetOffset.Normalized().Rotated(Vector3.Up, yaw).Rotated(pitchAxis, pitch) * panSpeed;

        if (Input.IsActionJustPressed("scroll_up"))
        {
            length -= 0.25f;
        }
        if (Input.IsActionJustPressed("scroll_down"))
        {
            length += 0.25f;
        }

        if (length < 0.01f) length = 0.01f;

        Camera camera = GetNode<Camera>("Camera");

        camera.Translation = new Vector3(0, 0, length);
    }
}
