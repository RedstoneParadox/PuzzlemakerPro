using PuzzlemakerPro.Scripts.Editor;
using System;
using System.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzlemakerPro.Scripts.Saving
{
    class SaveSystem
    {
        public void SaveLevel(string filepath, Level level)
        {
            JsonObject rootObject = new JsonObject();
            JsonArray textureArray = new JsonArray();
            JsonArray voxelArray = new JsonArray();

            List<(VoxelPos, Voxel)> voxelList = level.GetVoxelList();
            List<string> textures = new List<string>();

            foreach ((VoxelPos, Voxel) pair in voxelList)
            {
                VoxelPos pos = pair.Item1;
                Voxel voxel = pair.Item2;

                JsonObject voxelObject = new JsonObject();
                JsonObject posObject = new JsonObject();

                posObject["x"] = pos.x;
                posObject["y"] = pos.y;
                posObject["z"] = pos.z;

                voxelObject["pos"] = posObject;

                foreach
            }

            rootObject.Add("textures", textureArray);
        }
    }
}
