﻿using System;
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
        private readonly SurfaceTool Builder = new SurfaceTool();
        private readonly Color White = Color.Color8(255, 255, 255);
        private readonly Color Black = Color.Color8(255, 255, 255);
        private readonly Color Red = Color.Color8(255, 0, 0);
        private readonly Color Blue = Color.Color8(0, 255, 0);
        private readonly Color Green = Color.Color8(0, 0, 255);
        private readonly Color Purple = Color.Color8(255, 0, 255);

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (Input.IsActionJustPressed("ui_accept"))
            {
                GenerateDefaultChamber();
            }
        }

        public void GenerateDefaultChamber()
        {
            var voxel = new Voxel();

            voxel.northTexture = "w";
            voxel.southTexture = "w";
            voxel.eastTexture = "w";
            voxel.westTexture = "w";
            voxel.topTexture = "w";
            voxel.bottomTexture = "w";

            SetVoxel(new VoxelPos(0, 0, 0), voxel);
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
            BuildVoxelMesh();
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

            BuildVoxelMesh();
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
                            BuildFace(pos.ToVector3(), Vector3.Right, Vector3.Up, White, Vector3.Forward);
                            break;
                        // Back
                        case 1:
                            BuildFace(pos.South().ToVector3(), Vector3.Right, Vector3.Up, Black, Vector3.Back);
                            break;
                        // Right
                        case 2:
                            BuildFace(pos.ToVector3(), Vector3.Back, Vector3.Up, Red, Vector3.Right);
                            break;
                        // Left
                        case 3:
                            BuildFace(pos.East().ToVector3(), Vector3.Back, Vector3.Up, Blue, Vector3.Left);
                            break;
                        // Top
                        case 4:
                            BuildFace(pos.Up().ToVector3(), Vector3.Back, Vector3.Right, Green, Vector3.Up);
                            break;
                        // Bottom
                        case 5:
                            BuildFace(pos.ToVector3(), Vector3.Back, Vector3.Right, Purple, Vector3.Down);
                            break;
                    }
                }

                var mesh = Builder.Commit();
                var meshInstance = GetNode<MeshInstance>("VoxelMesh");
                meshInstance.Mesh = mesh;

                Builder.Clear();
            }
        }

        private void BuildFace(Vector3 start, Vector3 dirA, Vector3 dirB, Color color, Vector3 normal)
        {
            var first = start;
            var second = start + dirA;
            var third = start + dirA + dirB;
            var fourth = start + dirB;

            AddVertex(first, color, normal);
            AddVertex(second, color, normal);
            AddVertex(third, color, normal);

            AddVertex(third, color, normal);
            AddVertex(fourth, color, normal);
            AddVertex(first, color, normal);
        }

        private void AddVertex(Vector3 vertex, Color color, Vector3 normal)
        {
            Builder.AddColor(color);
            Builder.AddNormal(normal);
            Builder.AddVertex(vertex);
        }
    }
}
