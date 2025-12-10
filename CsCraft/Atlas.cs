using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Raylib_cs;

namespace CsCraft
{
    internal class Atlas
    {
        private static Atlas instance;
        private List<BlockTexture> block_texture_offsets;
        private Texture2D texture;
        private Shader shader;
        private Material material;

        private static int tileSize = 16;
        private static int atlasSize = 256;

        private Atlas()
        {
            block_texture_offsets = new List<BlockTexture>();

            texture = Raylib.LoadTexture("assets/terrain.png");
            shader = Raylib.LoadShader("assets/shaders/glsl330/block.vs", "assets/shaders/glsl330/block.fs");
            material = Raylib.LoadMaterialDefault();

            // material.Shader = shader;
            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, texture);
            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Albedo, texture);

            block_texture_offsets.Add(newBlockTexture("grass", (0, 0), (3, 0), (2, 0)));
            block_texture_offsets.Add(newBlockTexture("snow", (2, 4), (4, 4), (2, 0)));
            block_texture_offsets.Add(newBlockTexture("sand", (2, 1)));
            block_texture_offsets.Add(newBlockTexture("log", (5, 1), (4, 1), (5, 1)));
            block_texture_offsets.Add(newBlockTexture("leaves", (5, 3)));
            block_texture_offsets.Add(newBlockTexture("dirt", (2, 0)));
            block_texture_offsets.Add(newBlockTexture("stone", (1, 0)));
            block_texture_offsets.Add(newBlockTexture("air", (0, 14)));
        }

        public Material GetMaterial()
        {
            return material;
        }

        public Shader GetShader()
        {
            return shader;
        }

        public Texture2D GetTexture()
        {
            return texture;
        }


        public static Atlas Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Atlas();
                }
                return instance;
            }
        }

        public float[] getTextureCoordinates(string type, Sides direction)
        {
            if (!block_texture_offsets.ConvertAll(t => t.Name).Contains(type))
            {
                throw new Exception("Texture not found: " + type);
            }

            BlockTexture bTex = block_texture_offsets.Find(t => t.Name == type);

            float sx = 0f;
            float sy = 0f;

            float stepSize = (float)tileSize / (float)atlasSize;

            if (direction == Sides.TOP)
            {
                sx = stepSize * bTex.top_tile.Item1;
                sy = stepSize * bTex.top_tile.Item2;
            }
            else if (direction == Sides.BOTTOM)
            {
                sx = stepSize * bTex.bottom_tile.Item1;
                sy = stepSize * bTex.bottom_tile.Item2;
            }
            else
            {
                sx = stepSize * bTex.side_tile.Item1;
                sy = stepSize * bTex.side_tile.Item2;
            }

            float wx = stepSize;
            float wy = stepSize;

            return new float[] { sx, sy, wx, wy };
        }

        private static BlockTexture newBlockTexture(string name, (int, int) tile)
        {
            return new BlockTexture
            {
                Name = name,
                top_tile = tile,
                side_tile = tile,
                bottom_tile = tile
            };
        }

        private static BlockTexture newBlockTexture(string name, (int, int) top_tile, (int, int) side_tile, (int, int) bottom_tile)
        {
            return new BlockTexture
            {
                Name = name,
                top_tile = top_tile,
                side_tile = side_tile,
                bottom_tile = bottom_tile
            };
        }
    }
}

struct BlockTexture
{
    public String Name;
    public (int, int) top_tile;
    public (int, int) side_tile;
    public (int, int) bottom_tile;
}
