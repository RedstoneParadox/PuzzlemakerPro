using Godot;
using System;

public class CameraBase : Spatial
{
    private float yaw = 0.0f;
    private float panSpeed = 1.0f;

    public override void _Ready()
    {
        
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

        Rotation = new Vector3(0, yaw, 0);
        Translation = Translation + targetOffset.Normalized() * panSpeed;
    }
}
