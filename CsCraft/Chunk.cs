using CsCraft.utils;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices.Swift;
using System.Text;

namespace CsCraft
{
    internal class Chunk
    {
        public const int CHUNK_SIZE = 16;
        public Vector3 position;
        public Block[][][] blocks = new Block[CHUNK_SIZE][][];
        private Mesh? mesh;
        private Model? model;
        private bool isGenerating = false;

        private float[] verticesArray;
        private float[] texcoordsArray;
        private float[] normalsArray;
        private ushort[] indicesArray;
        public Chunk(Vector3 pos)
        {
            this.mesh = null;
            this.position = pos;
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                blocks[i] = new Block[CHUNK_SIZE][];
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    blocks[i][j] = new Block[CHUNK_SIZE];
                }
            }
        }

        public String GetBlockAt(Vector3 localPosition)
        {
            Vector3i localIPosition = Vector3i.FromVector3(localPosition);
            return GetBlockAt(localIPosition);
        }

        public string GetBlockAt(Vector3i localPosition)
        {
            return blocks[localPosition.X][localPosition.Y][localPosition.Z].type;
        }

        public void Generate(utils.FastNoiseLite chunkNoise, utils.FastNoiseLite worldNoise)
        {
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        Vector3 block_pos = new Vector3(x, y, z);
                        blocks[x][y][z] = new Block(block_pos, "air");

                        Vector3 world_pos = block_pos + this.position;

                        int y_val = (int)(world_pos.Y + chunkNoise.GetNoise(world_pos.X, world_pos.Z) * 4f);
                        y_val += (int)(worldNoise.GetNoise(world_pos.X / 16f, world_pos.Z / 16f) * 128f);

                        //if(x == 0 && y == 0 && z == 0)
                        //{
                        //    blocks[x][y][z].type = "stone"; // Ensure at least one block to avoid empty chunk
                        //}

                        //continue;

                        if (y_val == 4)
                        {
                            blocks[x][y][z].type = "grass";
                            if(world_pos.Y + y_val > 20)
                            {
                                blocks[x][y][z].type = "snow";
                            }
                        }

                        if (y_val < 4)
                        {
                            blocks[x][y][z].type = "dirt";
                        }

                        if (y_val < 1)
                        {
                            blocks[x][y][z].type = "stone";
                        }
                    }
                }
            }
        }

        public void GenerateMesh()
        {
            this.verticesArray = Array.Empty<float>();
            this.texcoordsArray = Array.Empty<float>();
            this.normalsArray = Array.Empty<float>();
            this.indicesArray = Array.Empty<ushort>();

            List<float> vertices = new List<float>();
            List<float> texcoords = new List<float>();
            List<float> normals = new List<float>();
            List<float> indices = new List<float>();

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        Block b = this.blocks[x][y][z];
                        int side_bitmask = 0x0;

                        if(b.type == "air")
                        {
                            continue; // Skip air blocks
                        }

                        if (x == 0 || this.blocks[x - 1][y][z].type == "air")
                        {
                            side_bitmask |= (int)Sides.LEFT;
                        }
                        if (x == 15 || this.blocks[x + 1][y][z].type == "air")
                        {
                            side_bitmask |= (int)Sides.RIGHT;
                        }
                        if (y == 0 || this.blocks[x][y - 1][z].type == "air")
                        {
                            side_bitmask |= (int)Sides.BOTTOM;
                        }
                        if (y == 15 || this.blocks[x][y + 1][z].type == "air")
                        {
                            side_bitmask |= (int)Sides.TOP;
                        }
                        if (z == 0 || this.blocks[x][y][z - 1].type == "air")
                        {
                            side_bitmask |= (int)Sides.FRONT;
                        }
                        if (z == 15 || this.blocks[x][y][z + 1].type == "air")
                        {
                            side_bitmask |= (int)Sides.BACK;
                        }

                        // side_bitmask = (int)Sides.TOP; // TEMP: Show only top face for testing


                        MeshPart m = b.GenerateMeshAtBlockPosition(side_bitmask);

                        // Append block mesh data to chunk mesh data
                        vertices.AddRange(m.vertices);
                        texcoords.AddRange(m.texcoords);
                        normals.AddRange(m.normals);
                    }
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                indices.Add(i);
            }

            Mesh meshInstance = new Mesh(vertices.Count / 3, vertices.Count / 3 / 3);
            this.verticesArray = vertices.ToArray();
            this.texcoordsArray = texcoords.ToArray();
            this.normalsArray = normals.ToArray();
            this.indicesArray = Array.ConvertAll(indices.ToArray(), item => (ushort)item);

            unsafe
            {
                fixed (float* vPtr = this.verticesArray)
                fixed (float* tPtr = this.texcoordsArray)
                fixed (float* nPtr = this.normalsArray)
                fixed (ushort* iPtr = this.indicesArray)
                {
                    meshInstance.Vertices = vPtr;
                    meshInstance.TexCoords = tPtr;
                    meshInstance.Normals = nPtr;
                    meshInstance.Indices = iPtr;
                }
            }

            // Clean up the old mesh
            //if (this.mesh != null)
            //{
            //    this.model = null;
            //    Raylib.UnloadMesh((Mesh)this.mesh);
            //}

            Raylib.UploadMesh(ref meshInstance, false);

            this.mesh = meshInstance;

            Model tempModel = Raylib.LoadModelFromMesh(meshInstance);
            unsafe
            {
                Raylib.SetMaterialTexture(ref tempModel.Materials[0], MaterialMapIndex.Albedo, Atlas.Instance.GetTexture());
                Raylib.SetMaterialTexture(ref tempModel.Materials[0], MaterialMapIndex.Diffuse, Atlas.Instance.GetTexture());
            }
            this.model = tempModel;
            this.isGenerating = false;
        }

        public void SetBlockAt(Vector3 pos, String block_type)
        {
            Vector3i localPos = Vector3i.FromVector3(pos);
            if(localPos.X < 0 || localPos.X >= CHUNK_SIZE ||
               localPos.Y < 0 || localPos.Y >= CHUNK_SIZE ||
               localPos.Z < 0 || localPos.Z >= CHUNK_SIZE)
            {
                return;
            }
            this.blocks[localPos.X][localPos.Y][localPos.Z].type = block_type;

            this.GenerateMesh();
        }

        public void Draw()
        {
            if(this.model == null && !this.isGenerating) {
                this.isGenerating = true;
                // Thread generateThread = new Thread(() =>
                // {
                this.GenerateMesh();
                // });
                // generateThread.Start();
            }
            Raylib.DrawCubeWires(this.position - new Vector3(0.5f, 0.5f, 0.5f) + new Vector3(CHUNK_SIZE / 2, CHUNK_SIZE / 2, CHUNK_SIZE / 2), CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE, Color.Gray);


            if(this.model == null)
            {
                return;
            }

            Raylib.DrawModel((Model)model, this.position, 1f, Color.White);
        }
    }
}
