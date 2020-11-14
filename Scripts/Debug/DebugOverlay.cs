using Godot;
using System;
using System.Collections.Generic;

public class DebugOverlay : Control
{
    public static readonly Dictionary<string, object> Information = new Dictionary<string, object>();
    public override void _Process(float delta)
    {
        base._Process(delta);

        Label label = GetNode<Label>("Label");
        label.Text = "";

        foreach (KeyValuePair<string, object> info in Information)
        {
            label.Text += info.Key + ": " + info.Value.ToString() + "\n";
        }
    }
}
