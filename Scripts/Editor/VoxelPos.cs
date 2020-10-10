using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace PuzzlemakerPro.Scripts.Editor
{
    class VoxelPos
    {
        public int x;
        public int y;
        public int z;

        public VoxelPos(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public VoxelPos North()
        {
            return new VoxelPos(x, y, z - 1);
        }

        public VoxelPos South()
        {
            return new VoxelPos(x, y, z + 1);
        }

        public VoxelPos East()
        {
            return new VoxelPos(x + 1, y, z);
        }

        public VoxelPos West()
        {
            return new VoxelPos(x - 1, y, z);
        }

        public VoxelPos Up()
        {
            return new VoxelPos(x, y + 1, z);
        }

        public VoxelPos Down()
        {
            return new VoxelPos(x, y - 1, z);
        }

        public VoxelPos Translate(Vector3 offset)
        {
            return new VoxelPos(x + Mathf.FloorToInt(offset.x), y + Mathf.FloorToInt(offset.y), z + Mathf.FloorToInt(offset.z));
        }

        public static VoxelPos FromVector3(Vector3 vector)
        {
            return new VoxelPos(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"VoxelPos({x}, {y}, {z})";
        }
    }
}
