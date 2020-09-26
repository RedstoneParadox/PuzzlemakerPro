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
        private readonly Dictionary<VoxelPos, Voxel> Voxels;

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

                if (neighbor.IsEmpty())
                {
                    Voxels.Remove(neighborPos);
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
    }
}
