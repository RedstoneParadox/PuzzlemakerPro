using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace PuzzlemakerPro.Scripts.Editor
{
    class Level
    {
        private readonly Dictionary<VoxelPos, Voxel> Voxels = new Dictionary<VoxelPos, Voxel>();
        private readonly SurfaceTool Builder = new SurfaceTool();
        private readonly Color White = Color.Color8(255, 255, 255);

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
                            neighbor.southTexture = "";
                            voxel.northTexture = "";
                            break;
                        // South Neighbor
                        case 1:
                            neighbor.northTexture = "";
                            voxel.southTexture = "";
                            break;
                        // East Neighbor
                        case 2:
                            neighbor.westTexture = "";
                            voxel.eastTexture = "";
                            break;
                        // West Neighbor
                        case 3:
                            neighbor.eastTexture = "";
                            voxel.westTexture = "";
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
                            neighbor.southTexture = texture;
                            break;
                        // South Neighbor
                        case 1:
                            neighbor.northTexture = texture;
                            break;
                        // East Neighbor
                        case 2:
                            neighbor.westTexture = texture;
                            break;
                        // West Neighbor
                        case 3:
                            neighbor.eastTexture = texture;
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

            foreach (VoxelPos pos in Voxels.Keys)
            {
                var voxel = GetVoxel(pos, false);
                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        // Front
                        case 0:
                            BuildFace(pos.ToVector3(), Vector3.Right, Vector3.Up);
                            break;
                        // Back
                        case 1:
                            BuildFace(pos.South().ToVector3(), Vector3.Right, Vector3.Up);
                            break;
                        // Right
                        case 2:
                            BuildFace(pos.ToVector3(), Vector3.Back, Vector3.Up);
                            break;
                        // Left
                        case 3:
                            BuildFace(pos.West().ToVector3(), Vector3.Back, Vector3.Up);
                            break;
                        // Top
                        case 4:
                            BuildFace(pos.Up().ToVector3(), Vector3.Back, Vector3.Right);
                            break;
                        // Bottom
                        case 5:
                            BuildFace(pos.ToVector3(), Vector3.Back, Vector3.Right);
                            break;
                    }
                }

                var mesh = Builder.Commit();
                Builder.Clear();
            }
        }

        private void BuildFace(Vector3 start, Vector3 dirA, Vector3 dirB)
        {
            var first = start;
            var second = start + dirA;
            var third = start + dirA + dirB;
            var fourth = start + dirB;

            AddVertex(first, White);
            AddVertex(second, White);
            AddVertex(third, White);

            AddVertex(third, White);
            AddVertex(fourth, White);
            AddVertex(first, White);
        }

        private void AddVertex(Vector3 vertex, Color color)
        {
            Builder.AddColor(color);
            Builder.AddVertex(vertex);
        }
    }
}
