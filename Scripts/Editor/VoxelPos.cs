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
        int x;
        int y;
        int z;

        public VoxelPos North()
        {
            return new VoxelPos
            {
                x = this.x,
                y = this.y,
                z = this.z - 1,
            };
        }

        public VoxelPos South()
        {
            return new VoxelPos
            {
                x = this.x,
                y = this.y,
                z = this.z + 1,
            };
        }

        public VoxelPos East()
        {
            return new VoxelPos
            {
                x = this.x + 1,
                y = this.y,
                z = this.z,
            };
        }

        public VoxelPos West()
        {
            return new VoxelPos
            {
                x = this.x - 1,
                y = this.y,
                z = this.z,
            };
        }

        public VoxelPos Up()
        {
            return new VoxelPos
            {
                x = this.x,
                y = this.y + 1,
                z = this.z,
            };
        }

        public VoxelPos Down()
        {
            return new VoxelPos
            {
                x = this.x,
                y = this.y - 1,
                z = this.z,
            };
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
