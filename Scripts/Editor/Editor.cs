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
        private Selection selection = new Selection();

        private readonly Plane BottomPlane = new Plane(0, -1, 0, 0);
        private readonly Plane TopPlane = new Plane(0, 1, 0, 1);
        private readonly Plane FrontPlane = new Plane(0, 0, -1, 0);
        private readonly Plane BackPlane = new Plane(0, 0, 1, 1);
        private readonly Plane RightPlane = new Plane(1, 0, 0, 1);
        private readonly Plane LeftPlane = new Plane(-1, 0, 0, 0);

        public override void _Process(float delta)
        {
            base._Process(delta);

            CenterContainer centerContainer = GetNode<CenterContainer>("UI/CenterContainer");

            centerContainer.SetSize(GetViewport().Size);

            if (Input.IsActionJustPressed("ui_accept"))
            {
                GenerateDefaultChamber();
            }

            if (selection.Started() && !selection.Completed())
            {
                if (Input.IsActionPressed("select"))
                {
                    Vector2 mousePos = GetViewport().GetMousePosition();
                    Camera camera = RuntimeRoot.CurrentCamera;

                    Vector3 start = camera.ProjectPosition(mousePos, 5f);
                    Vector3 direction = camera.ProjectRayNormal(mousePos);

                    UpdateSelection(start, direction);
                }
                if (Input.IsActionJustReleased("select"))
                {
                    selection.FinishSelection();
                }
            }
            else if (selection.Completed())
            {
                if (Input.IsActionJustPressed("extrude"))
                {
                    Extrude(false);
                }
                if (Input.IsActionJustPressed("intrude"))
                {
                    Extrude(true);
                }
            }

            DebugOverlay.Information["Selection"] = selection;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);

            if (@event is InputEventMouseButton)
            {

                if (Input.IsActionJustPressed("select"))
                {
                    selection.ResetSelection();

                    Vector2 mousePos = GetViewport().GetMousePosition();
                    Camera camera = RuntimeRoot.CurrentCamera;

                    Vector3 start = camera.ProjectPosition(mousePos, 4f);
                    Vector3 direction = camera.ProjectRayNormal(mousePos);

                    UpdateSelection(start, direction);
                }
            }
        }

        public void OnCreateNewChamber()
        {
            if (level != null) CallDeferred("remove_child", level);
            level = (Level)LevelScene.Instance();
            CallDeferred("add_child", level);

            GetNode<CenterContainer>("UI/CenterContainer").Hide();

            GenerateDefaultChamber();
        }

        public void GenerateDefaultChamber()
        {
            level.ClearVoxels();
            selection.ResetSelection();

            var floor = new Voxel();
            floor.topTexture = "black";
            CreateVoxelShape(new VoxelPos(-6, -7, -6), new VoxelPos(5, -7, 5), floor);

            var ceiling = new Voxel();
            ceiling.bottomTexture = "black";
            CreateVoxelShape(new VoxelPos(-6, 6, -6), new VoxelPos(5, 6, 5), ceiling);

            var leftWall = new Voxel();
            leftWall.rightTexture = "white";
            CreateVoxelShape(new VoxelPos(-7, -6, -6), new VoxelPos(-7, 5, 5), leftWall);

            var rightWall = new Voxel();
            rightWall.leftTexture = "white";
            CreateVoxelShape(new VoxelPos(6, -6, -6), new VoxelPos(6, 5, 5), rightWall);

            var frontWall = new Voxel();
            frontWall.backTexture = "white";
            CreateVoxelShape(new VoxelPos(-6, -6, -7), new VoxelPos(5, 5, -7), frontWall);

            var backWall = new Voxel();
            backWall.frontTexture = "white";
            CreateVoxelShape(new VoxelPos(-6, -6, 6), new VoxelPos(5, 5, 6), backWall);
        }

        private void CreateVoxelShape(VoxelPos from, VoxelPos to, Voxel voxel)
        {
            for (int x = from.x; x <= to.x; x++)
            {
                for (int y = from.y; y <= to.y; y++)
                {
                    for (int z = from.z; z <= to.z; z++)
                    {
                        var pos = new VoxelPos(x, y, z);
                        level.SetVoxel(pos, voxel.Copy());
                    }
                }
            }
        }

        private void UpdateSelection(Vector3 start, Vector3 direction)
        {
            var current = start;
            // Prevent infinite looping.
            for (int i = 0; i < 500; i++)
            {
                var next = current + direction;
                var pos = VoxelPos.FromVector3(next);
                //GD.Print(pos);

                // Move head and tail of segment to origin.
                var offset = pos.ToVector3();
                var head = next - offset;
                var tail = current - offset;

                var voxel = level.GetVoxel(pos, false);
                //GD.Print(voxel);
                List<Plane> faces = new List<Plane>();

                if (voxel.HasTop()) faces.Add(TopPlane);
                if (voxel.HasBottom()) faces.Add(BottomPlane);
                if (voxel.HasLeft()) faces.Add(LeftPlane);
                if (voxel.HasRight()) faces.Add(RightPlane);
                if (voxel.HasFront()) faces.Add(FrontPlane);
                if (voxel.HasBack()) faces.Add(BackPlane);

                if (faces.Count > 0)
                {
                    foreach (Plane face in faces)
                    {
                        //GD.Print("here!");
                        Vector3? intersection = face.IntersectSegment(tail, head);

                        // Note: Both the head and the tail are allowed to end up inside the voxel; because I can't be bothered to properly account for that edge case,
                        // This null check is necessary.
                        if (intersection != null)
                        {
                            Vector3 point = (Vector3)intersection;
                            var normal = face.Normal;

                            // Continue to the next face if the intersection is not within bounds.
                            if (normal.x == 0 && (point.x < 0 || point.x > 1)) continue;
                            if (normal.y == 0 && (point.y < 0 || point.y > 1)) continue;
                            if (normal.z == 0 && (point.z < 0 || point.z > 1)) continue;

                            // Update the selection and exit the search.
                            if (selection.Started())
                            {
                                selection.UpdateSelection(pos);
                            }
                            else
                            {
                                selection.StartSelection(pos, normal);
                            }

                            return;
                        }
                    }
                }

                current = next;
            }
        }

        private void Extrude(bool intrude)
        {
            if (selection.Is3D())
            {
                GD.Print("3D Selections are not implemented!");
                return;
            }

            var (start, end, normal) = selection.GetSelectionTuple();

            if (normal == Vector3.Zero)
            {
                return;
            }

            // TODO: Stuff needs to be cached here for performance and memory.
            foreach (int x in Range(start.x, end.x + 1))
            {
                foreach (int y in Range(start.y, end.y + 1))
                {
                    foreach (int z in Range(start.z, end.z + 1))
                    {
                        VoxelPos pos = new VoxelPos(x, y, z);
                        Voxel voxel = level.GetVoxel(pos, false);
                        string texture = "";

                        if (voxel.IsEmpty())
                        {
                            continue;
                        }

                        if (normal == Vector3.Up) texture = voxel.topTexture;
                        else if (normal == Vector3.Down) texture = voxel.bottomTexture;
                        else if (normal == Vector3.Left) texture = voxel.leftTexture;
                        else if (normal == Vector3.Right) texture = voxel.rightTexture;
                        else if (normal == Vector3.Forward) texture = voxel.frontTexture;
                        else if (normal == Vector3.Back) texture = voxel.backTexture;

                        if (texture == "") texture = "white";

                        if (intrude)
                        {
                            level.RemoveVoxel(pos, texture);
                        }
                        else
                        {
                            pos = pos.Translate(normal);
                            level.SetVoxel(pos, new Voxel(texture));
                        }
                    }
                }
            }

            if (intrude)
            {
                selection.Translate(-normal);
            }
            else
            {
                selection.Translate(normal);
            }
        }

        private IEnumerable<int> Range(int start, int end)
        {
            if (start == end)
            {
                return new int[] { start };
            }

            if (start > end)
            {
                return GD.Range(end, start);
            }

            return GD.Range(start, end);
        }
    }
}
