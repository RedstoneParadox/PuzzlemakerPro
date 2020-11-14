using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzlemakerPro.Scripts.Editor
{
    class Editor: Node
    {
        private readonly PackedScene LevelScene = GD.Load<PackedScene>("res://Scenes/Editor/Level.tscn");
        private Level level = null;

        public override void _Process(float delta)
        {
            base._Process(delta);

            CenterContainer centerContainer = GetNode<CenterContainer>("UI/CenterContainer");

            centerContainer.SetSize(GetViewport().Size);
        }

        public void OnCreateNewChamber()
        {
            if (level != null) CallDeferred("remove_child", level);
            level = (Level)LevelScene.Instance();
            CallDeferred("add_child", level);

            GetNode<Button>("UI/CenterContainer/NewChamberButton").Hide();

            level.GenerateDefaultChamber();
        }
    }
}
