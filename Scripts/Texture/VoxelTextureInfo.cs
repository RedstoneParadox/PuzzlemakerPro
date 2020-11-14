using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzlemakerPro.Scripts.Texture
{
    class VoxelTextureInfo
    {
        

        public readonly Godot.Texture VoxelTexture;
        public readonly Vector2 Origin;
        // 1x1 fits on a single voxel face.
        public readonly Vector2 Size;

        VoxelTextureInfo(Godot.Texture voxelTexture, Vector2 origin, Vector2 size)
        {
            VoxelTexture = voxelTexture;
            Origin = origin;
            Size = size;
        }
    }
}
