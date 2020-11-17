using Godot;
using System;
using System.Collections.Generic;

public class DebugOverlay : MarginContainer
{
    public static readonly Dictionary<string, object> Information = new Dictionary<string, object>();
    public override void _Process(float delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("debug_view"))
        {
            Visible = !Visible;
        }

        Label label = GetNode<Label>("Label");
        label.Text = "";

        foreach (KeyValuePair<string, object> info in Information)
        {
            label.Text += info.Key + ": " + info.Value.ToString() + "\n";
        }

        Vector2 viewportSize = GetViewport().Size;
        Vector2 labelSize = label.RectSize;

        MarginLeft = viewportSize.x - 10 - labelSize.x;
        MarginTop = viewportSize.y - 10 - labelSize.y;
        MarginRight = -10;
        MarginBottom = -10;
    }
}
