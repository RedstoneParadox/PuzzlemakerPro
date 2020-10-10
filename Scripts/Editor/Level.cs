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
            if (updateMesh)
            {
                updateMesh = false;
                BuildVoxelMesh();
            }
        }

        public void GenerateDefaultChamber()
        {
            var floor = new Voxel();
            floor.topTexture = "black";
            CreateVoxelShape(new VoxelPos(-6, -1, -6), new VoxelPos(6, -1, 6), floor);

            var ceiling = new Voxel();
            ceiling.bottomTexture = "black";
            CreateVoxelShape(new VoxelPos(-6, 12, -6), new VoxelPos(6, 12, 6), ceiling);

            var leftWall = new Voxel();
            leftWall.rightTexture = "white";
            CreateVoxelShape(new VoxelPos(7, 0, -6), new VoxelPos(7, 11, 6), leftWall);

            var rightWall = new Voxel();
            rightWall.leftTexture = "white";
            CreateVoxelShape(new VoxelPos(-7, 0, -6), new VoxelPos(-7, 11, 6), rightWall);

            var frontWall = new Voxel();
            frontWall.backTexture = "white";
            CreateVoxelShape(new VoxelPos(-6, 0, -7), new VoxelPos(6, 11, -7), frontWall);

            var backWall = new Voxel();
            backWall.frontTexture = "white";
            CreateVoxelShape(new VoxelPos(-6, 0, 7), new VoxelPos(6, 11, 7), backWall);
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

        public void SetVoxel(VoxelPos pos, Voxel voxel)
        {
            var positions = new VoxelPos[] { pos.North(), pos.South(), pos.East(), pos.West(), pos.Up(), pos.Down() }.ToList();

            for (int i  = 0; i < 6; i++)
            {
                var neighborPos = positions[i];
                var neighbor = GetVoxel(neighborPos, false);

                if (!neighbor.IsEmpty())
                {
                    switch(i)
                    {
                        // North Neighbor
                        case 0:
                            neighbor.backTexture = "";
                            voxel.frontTexture = "";
                            break;
                        // South Neighbor
                        case 1:
                            neighbor.frontTexture = "";
                            voxel.backTexture = "";
                            break;
                        // East Neighbor
                        case 2:
                            neighbor.leftTexture = "";
                            voxel.rightTexture = "";
                            break;
                        // West Neighbor
                        case 3:
                            neighbor.rightTexture = "";
                            voxel.leftTexture = "";
                            break;
                        // Up Neighbor
                        case 4:
                            neighbor.bottomTexture = "";
                            voxel.topTexture = "";
                            break;
                        // Down Neighbor
                        case 5:
                            neighbor.topTexture = "";
                            voxel.bottomTexture = "";
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
            var positions = new VoxelPos[] { pos.North(), pos.South(), pos.East(), pos.West(), pos.Up(), pos.Down() }.ToList();

            for (int i = 0; i < 6; i++)
            {
                var neighborPos = positions[i];
                var neighbor = GetVoxel(neighborPos, true);

                if (!neighbor.IsEmpty())
                {
                    switch (i)
                    {
                        // North Neighbor
                        case 0:
                            neighbor.backTexture = texture;
                            break;
                        // South Neighbor
                        case 1:
                            neighbor.frontTexture = texture;
                            break;
                        // East Neighbor
                        case 2:
                            neighbor.leftTexture = texture;
                            break;
                        // West Neighbor
                        case 3:
                            neighbor.rightTexture = texture;
                            break;
                        // Up Neighbor
                        case 4:
                            neighbor.bottomTexture = texture;
                            break;
                        // Down Neighbor
                        case 5:
                            neighbor.topTexture = texture;
                            break;
                    }
                }
            }

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
            GD.Print("Builder has begun!");
            Builder.SetMaterial(voxelMaterial);

            foreach (VoxelPos pos in Voxels.Keys)
            {
                var voxel = GetVoxel(pos, false);

                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        // Front
                        case 0:
                            if (!voxel.HasFront()) break;
                            BuildFace(pos.ToVector3(), Vector3.Right, Vector3.Up, UVFromName(voxel.frontTexture), Vector3.Forward);
                            break;
                        // Back
                        case 1:
                            if (!voxel.HasBack()) break;
                            BuildFace(pos.South().ToVector3(), Vector3.Up, Vector3.Right, UVFromName(voxel.backTexture), Vector3.Back);
                            break;
                        // Right
                        case 2:
                            if (!voxel.HasRight()) break;
                            BuildFace(pos.ToVector3(), Vector3.Up, Vector3.Back, UVFromName(voxel.rightTexture), Vector3.Right);
                            break;
                        // Left
                        case 3:
                            if (!voxel.HasLeft()) break;
                            BuildFace(pos.East().ToVector3(), Vector3.Back, Vector3.Up, UVFromName(voxel.leftTexture), Vector3.Left);
                            break;
                        // Top
                        case 4:
                            if (!voxel.HasTop()) break;
                            BuildFace(pos.Up().ToVector3(), Vector3.Right, Vector3.Back, UVFromName(voxel.topTexture), Vector3.Up);
                            break;
                        // Bottom
                        case 5:
                            if (!voxel.HasBottom()) break;
                            BuildFace(pos.ToVector3(), Vector3.Back, Vector3.Right, UVFromName(voxel.bottomTexture), Vector3.Down);
                            break;
                    }
                }
            }

            Builder.Index();
            var voxelMesh = GetNode<MeshInstance>("VoxelMesh");
            voxelMesh.Mesh = Builder.Commit();
            Builder.Clear();

            var collisionShape = GetNode<CollisionShape>("VoxelBody/VoxelCollision");
            var shape = new ConcavePolygonShape();
            shape.Data = collisionShapeFaces.ToArray();

            collisionShape.Shape = shape;
            collisionShapeFaces.Clear();
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
