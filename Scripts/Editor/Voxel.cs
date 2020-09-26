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
        public string northTexture = "";
        public string southTexture = "";
        public string eastTexture = "";
        public string westTexture = "";

        public bool IsEmpty()
        {
            var strings = (new string[] { topTexture, bottomTexture, northTexture, southTexture, eastTexture, westTexture }).ToList().FindAll(s => s.Length > 0);

            return strings.Count > 0;
        }
    }
}
