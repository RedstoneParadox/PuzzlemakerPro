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
        public static void SaveLevel(string filepath, Level level)
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
                JsonArray faceArray = new JsonArray();

                posObject["x"] = pos.x;
                posObject["y"] = pos.y;
                posObject["z"] = pos.z;

                voxelObject["pos"] = posObject;

                foreach (string face in new string[] { voxel.frontTexture, voxel.backTexture, voxel.leftTexture, voxel.rightTexture, voxel.topTexture, voxel.bottomTexture })
                {
                    if (!textures.Contains(face))
                    {
                        textures.Add(face);
                    }

                    faceArray.Add(new JsonPrimitive(textures.IndexOf(face)));
                }

                voxelObject["faces"] = faceArray;

                voxelArray.Add(voxelObject);
            }

            textures.ForEach((texture) => textureArray.Add(new JsonPrimitive(texture)));

            rootObject.Add("textures", textureArray);
            rootObject.Add("voxels", voxelArray);

            Godot.File file = new Godot.File();
            file.Open("user://save.json", Godot.File.ModeFlags.Write);
            file.StoreString(rootObject.ToString());
            file.Close();
        }

        public static Level Load()
        {
            Godot.File file = new Godot.File();
            file.Open("user://save.json", Godot.File.ModeFlags.Read);
            file.StoreString(rootObject.ToString());
            file.Close();


            return new Level();
        }
    }
}
