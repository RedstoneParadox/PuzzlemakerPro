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
            DrawAxis();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (updateMesh)
            {
                updateMesh = false;
                BuildVoxelMesh();
            }
        }

        private void DrawAxis()
        {
            ImmediateGeometry xAxis = GetNode<ImmediateGeometry>("xAxis");
            ImmediateGeometry yAxis = GetNode<ImmediateGeometry>("yAxis");
            ImmediateGeometry zAxis = GetNode<ImmediateGeometry>("zAxis");

            xAxis.Begin(Mesh.PrimitiveType.LineStrip);
            xAxis.AddVertex(Vector3.Zero);
            xAxis.AddVertex(new Vector3(100, 0, 0));
            xAxis.End();

            yAxis.Begin(Mesh.PrimitiveType.LineStrip);
            yAxis.AddVertex(Vector3.Zero);
            yAxis.AddVertex(new Vector3(0, 100, 0));
            yAxis.End();

            zAxis.Begin(Mesh.PrimitiveType.LineStrip);
            zAxis.AddVertex(Vector3.Zero);
            zAxis.AddVertex(new Vector3(0, 0, 100));
            zAxis.End();
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

        public void ClearVoxels()
        {
            Voxels.Clear();
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
                if (voxel.HasLeft()) BuildFace(pos.ToVector3(), Vector3.Up, Vector3.Back, UVFromName(voxel.leftTexture), Vector3.Left);
                if (voxel.HasRight()) BuildFace(pos.Right().ToVector3(), Vector3.Back, Vector3.Up, UVFromName(voxel.rightTexture), Vector3.Right);
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

        internal void UpdateMesh()
        {
            updateMesh = true;
        }
    }
}
