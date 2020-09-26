using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzlemakerPro.Scripts.Export
{
    struct BrushFace
    {
        public Vector3 start;
        public Vector3 size;
        public Vector2 uv;
        public Vector2 uvScale;
        public string vMaterial;
        public double rotation;
        public int lightMapScale;
    }
}
