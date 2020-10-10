using System;
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

        public Voxel()
        {
            
        }

        public Voxel(string texture)
        {
            topTexture = texture;
            bottomTexture = texture;
            frontTexture = texture;
            backTexture = texture;
            leftTexture = texture;
            rightTexture = texture;
        }

        public bool IsEmpty()
        {
            if (HasTop() || HasBottom() || HasLeft() || HasRight() || HasFront() || HasBack())
            {
                return false;
            }

            return true;
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

        public override string ToString()
        {
            return $"Voxel(top: {topTexture}, bottom: {bottomTexture}, left: {leftTexture}, right: {rightTexture}, front: {frontTexture}, back: {backTexture})";
        }
    }
}
