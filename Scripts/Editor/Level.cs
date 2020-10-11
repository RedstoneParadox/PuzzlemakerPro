using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace PuzzlemakerPro.Scripts.Editor
{
    class Level: Spatial
    {
        private readonly Dictionary<VoxelPos, Voxel> Voxels = new Dictionary<VoxelPos, Voxel>();
        private SurfaceTool Builder = new SurfaceTool();
        private readonly List<Vector3> collisionShapeFaces = new List<Vector3>();
        private readonly Vector2 White = new Vector2(0, 0f);
        private readonly Vector2 Black = new Vector2(0.5f, 0f);
        private readonly Material voxelMaterial = GD.Load<Material>("res://Assets/Materials/voxel_material.tres");
        private bool updateMesh = false;

        private (VoxelPos, Vector3) selection = (new VoxelPos(0, 0, 0), Vector3.Zero);

        private readonly Plane BottomPlane = new Plane(0, -1, 0, 0);
        private readonly Plane TopPlane = new Plane(0, 1, 0, 1);
        private readonly Plane FrontPlane = new Plane(0, 0, -1, 0);
        private readonly Plane BackPlane = new Plane(0, 0, 1, 1);
        private readonly Plane RightPlane = new Plane(1, 0, 0, 0);
        private readonly Plane LeftPlane = new Plane(-1, 0, 0, -1);

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (Input.IsActionJustPressed("ui_accept"))
            {
                GenerateDefaultChamber();
            }
            if (Input.IsActionJustPressed("select"))
            {
                Vector2 mousePos = GetViewport().GetMousePosition();
                Camera camera = RuntimeRoot.CurrentCamera;

                Vector3 start = camera.ProjectPosition(mousePos, 1f);
                Vector3 direction = camera.ProjectRayNormal(mousePos);

                UpdateSelection(start, direction);
            }
            if (Input.IsActionJustPressed("extrude"))
            {
                Extrude("white", false);
            }
            if (Input.IsActionJustPressed("intrude"))
            {
                Extrude("white", true);
            }

            if (updateMesh)
            {
                updateMesh = false;
                BuildVoxelMesh();
            }

            DebugOverlay.Information["Selection"] = selection;
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

                var voxel = GetVoxel(pos, false);
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

                            if (normal.x != 0)
                            {
                                // No idea why I have to do this.
                                normal = new Vector3(-normal.x, 0, 0);
                            }

                            selection = (pos, normal);
                            return;
                        }
                    }
                }

                current = next;
            }
        }

        public void GenerateDefaultChamber()
        {
            Voxels.Clear();
            selection = (new VoxelPos(0, 0, 0), Vector3.Zero);

            var floor = new Voxel();
            floor.topTexture = "black";
            CreateVoxelShape(new VoxelPos(-5, -1, -5), new VoxelPos(6, -1, 6), floor);

            var ceiling = new Voxel();
            ceiling.bottomTexture = "black";
            CreateVoxelShape(new VoxelPos(-5, 12, -5), new VoxelPos(6, 12, 6), ceiling);

            var leftWall = new Voxel();
            leftWall.rightTexture = "white";
            CreateVoxelShape(new VoxelPos(7, 0, -5), new VoxelPos(7, 11, 6), leftWall);

            var rightWall = new Voxel();
            rightWall.leftTexture = "white";
            CreateVoxelShape(new VoxelPos(-6, 0, -5), new VoxelPos(-6, 11, 6), rightWall);

            var frontWall = new Voxel();
            frontWall.backTexture = "white";
            CreateVoxelShape(new VoxelPos(-5, 0, -6), new VoxelPos(6, 11, -6), frontWall);

            var backWall = new Voxel();
            backWall.frontTexture = "white";
            CreateVoxelShape(new VoxelPos(-5, 0, 7), new VoxelPos(6, 11, 7), backWall);
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
                        SetVoxel(pos, voxel.Copy());
                    }
                }
            }
        }

        private void Extrude(string texture, bool intrude)
        {
            var pos = selection.Item1;
            var norm = selection.Item2;

            if (norm == Vector3.Zero)
            {
                return;
            }

            if (intrude)
            {
                RemoveVoxel(pos, texture);
                selection = (pos.Translate(-norm), norm);
            }
            else
            {
                pos = pos.Translate(norm);
                SetVoxel(pos, new Voxel(texture));
                selection = (pos, norm);
            }
        }

        public void SetVoxel(VoxelPos pos, Voxel voxel)
        {
            var positions = new VoxelPos[] { pos.Forward(), pos.Backwards(), pos.Right(), pos.Left(), pos.Up(), pos.Down() }.ToList();

            for (int i  = 0; i < 6; i++)
            {
                var neighborPos = positions[i];
                var neighbor = GetVoxel(neighborPos, false);

                if (!neighbor.IsEmpty())
                {
                    switch(i)
                    {
                        // Front Neighbor
                        case 0:
                            if (neighbor.HasBack())
                            {
                                neighbor.backTexture = "";
                                voxel.frontTexture = "";
                            }
                            break;
                        // Back Neighbord
                        case 1:
                            if (neighbor.HasFront())
                            {
                                neighbor.frontTexture = "";
                                voxel.backTexture = "";
                            }
                            break;
                        // Right Neighbor
                        case 2:
                            if (neighbor.HasLeft())
                            {
                                neighbor.leftTexture = "";
                                voxel.rightTexture = "";
                            }
                            break;
                        // Left Neighbor
                        case 3:
                            if (neighbor.HasRight())
                            {
                                neighbor.rightTexture = "";
                                voxel.leftTexture = "";
                            }
                            break;
                        // Up Neighbor
                        case 4:
                            if (neighbor.HasBottom())
                            {
                                neighbor.bottomTexture = "";
                                voxel.topTexture = "";
                            }
                            break;
                        // Down Neighbor
                        case 5:
                            if (neighbor.HasTop())
                            {
                                neighbor.topTexture = "";
                                voxel.bottomTexture = "";
                            }
                            break;
                    }
                }

                if (neighbor.IsEmpty())
                {
                    Voxels.Remove(neighborPos);
                }
            }

            Voxels[pos] = voxel;
            updateMesh = true;
        }

        public void RemoveVoxel(VoxelPos pos, string texture)
        {
            var offsets = new Vector3[] { Vector3.Forward, Vector3.Back, Vector3.Left, Vector3.Right, Vector3.Up, Vector3.Down }.ToList();
            var voxel = GetVoxel(pos, false);

            for (int i = 0; i < 6; i++)
            {
                var neighborPos = pos.Translate(offsets[i]);
                var neighbor = GetVoxel(neighborPos, true);

                switch (i)
                {
                    // Front Neighbor
                    case 0:
                        if (!voxel.HasFront()) neighbor.backTexture = texture;
                        break;
                    // Back Neighbord
                    case 1:
                        if (!voxel.HasBack()) neighbor.frontTexture = texture;
                        break;
                    // Left Neighbor
                    case 2:
                        if (!voxel.HasLeft()) neighbor.rightTexture = texture;
                        break;
                    // Right Neighbor
                    case 3:
                        if (!voxel.HasRight()) neighbor.leftTexture = texture;
                        break;
                    // Up Neighbor
                    case 4:
                        if (!voxel.HasTop()) neighbor.bottomTexture = texture;
                        break;
                    // Down Neighbor
                    case 5:
                        if (!voxel.HasBottom()) neighbor.topTexture = texture;
                        break;
                }
            }

            Voxels.Remove(pos);
            updateMesh = true;
        }

        public Voxel GetVoxel(VoxelPos pos, bool addToLevel)
        {
            if (Voxels.ContainsKey(pos))
            {
                return Voxels[pos];
            }

            var voxel = new Voxel();

            if (addToLevel)
            {
                Voxels[pos] = voxel;
            }

            return voxel;
        }

        public void BuildVoxelMesh()
        {
            Builder.Begin(Mesh.PrimitiveType.Triangles);
            // GD.Print("Builder has begun!");
            Builder.SetMaterial(voxelMaterial);

            foreach (VoxelPos pos in Voxels.Keys)
            {
                var voxel = GetVoxel(pos, false);

                if (voxel.HasFront()) BuildFace(pos.ToVector3(), Vector3.Right, Vector3.Up, UVFromName(voxel.frontTexture), Vector3.Forward);
                if (voxel.HasBack()) BuildFace(pos.Backwards().ToVector3(), Vector3.Up, Vector3.Right, UVFromName(voxel.backTexture), Vector3.Back);
                if (voxel.HasLeft()) BuildFace(pos.Right().ToVector3(), Vector3.Back, Vector3.Up, UVFromName(voxel.leftTexture), Vector3.Left);
                if (voxel.HasRight()) BuildFace(pos.ToVector3(), Vector3.Up, Vector3.Back, UVFromName(voxel.rightTexture), Vector3.Right);
                if (voxel.HasTop()) BuildFace(pos.Up().ToVector3(), Vector3.Right, Vector3.Back, UVFromName(voxel.topTexture), Vector3.Up);
                if (voxel.HasBottom()) BuildFace(pos.ToVector3(), Vector3.Back, Vector3.Right, UVFromName(voxel.bottomTexture), Vector3.Down);

            }

            Builder.Index();
            var voxelMesh = GetNode<MeshInstance>("VoxelMesh");
            voxelMesh.Mesh = Builder.Commit();
            Builder.Clear();

            /*
            foreach (KeyValuePair<VoxelPos, Voxel> pair in Voxels)
            {
                GD.Print($"{pair.Key}: {pair.Value}");
            }
            */
        }

        public Vector2 UVFromName(string name)
        {
            switch (name)
            {
                case "white":
                    return White;
                case "black":
                    return Black;
                default:
                    return Black;
            }
        }

        private void BuildFace(Vector3 start, Vector3 dirA, Vector3 dirB, Vector2 uv, Vector3 normal)
        {
            var first = start;
            var second = start + dirA;
            var third = start + dirA + dirB;
            var fourth = start + dirB;

            AddVertex(first, uv, normal);
            AddVertex(second, uv + new Vector2(0, 0.5f), normal);
            AddVertex(third, uv + new Vector2(0.5f, 0.5f), normal);

            AddVertex(third, uv + new Vector2(0.5f, 0.5f), normal);
            AddVertex(fourth, uv + new Vector2(0.5f, 0), normal);
            AddVertex(first, uv, normal);
        }

        private void AddVertex(Vector3 vertex, Vector2 uv, Vector3 normal)
        {
            Builder.AddUv(uv);
            Builder.AddNormal(normal);
            Builder.AddVertex(vertex);
            collisionShapeFaces.Add(vertex);
        }
    }
}
