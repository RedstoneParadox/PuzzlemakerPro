using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using PuzzlemakerPro.Scripts.Texture;

namespace PuzzlemakerPro.Scripts.Editor
{
    class VoxelFace
    {
        public VoxelTextureInfo Info;
        public Vector2 Offset;

        public VoxelFace(VoxelTextureInfo info, Vector2 offset)
        {
            Info = info;
            Offset = offset;
        }
    }
}
