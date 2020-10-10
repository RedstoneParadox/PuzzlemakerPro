﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzlemakerPro.Scripts.Editor
{
    class Voxel
    {
        public string topTexture = "";
        public string bottomTexture = "";
        public string frontTexture = "";
        public string backTexture = "";
        public string rightTexture = "";
        public string leftTexture = "";

        public bool IsEmpty()
        {
            var strings = (new string[] { topTexture, bottomTexture, frontTexture, backTexture, rightTexture, leftTexture }).ToList().FindAll(s => s.Length > 0);

            return strings.Count == 0;
        }

        public bool HasFront()
        {
            return frontTexture != "";
        }

        public bool HasBack()
        {
            return backTexture != "";
        }

        public bool HasLeft()
        {
            return leftTexture != "";
        }

        public bool HasRight()
        {
            return rightTexture != "";
        }

        public bool HasTop()
        {
            return topTexture != "";
        }

        public bool HasBottom()
        {
            return bottomTexture != "";
        }

        public Voxel Copy()
        {
            var voxel = new Voxel();

            voxel.frontTexture = frontTexture;
            voxel.backTexture = backTexture;
            voxel.rightTexture = rightTexture;
            voxel.leftTexture = leftTexture;
            voxel.topTexture = topTexture;
            voxel.bottomTexture = bottomTexture;

            return voxel;
        }
    }
}
