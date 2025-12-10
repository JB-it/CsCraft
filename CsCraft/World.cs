using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using CsCraft.utils;

namespace CsCraft
{
    internal class World
    {
        private Dictionary<Vector3i, Chunk> chunks = new Dictionary<Vector3i, Chunk>();
        private string name;
        private RNG rng;
        private utils.FastNoiseLite worldNoise;
        private utils.FastNoiseLite chunkNoise;

        public World(string name)
        {
            this.name = name;
            
            this.worldNoise = new utils.FastNoiseLite();
            this.worldNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            this.worldNoise.SetFrequency(0.01f);
            this.worldNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
            this.worldNoise.SetFractalOctaves(6);

            this.chunkNoise = new utils.FastNoiseLite();
            this.chunkNoise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            this.chunkNoise.SetFrequency(0.03f);
            this.chunkNoise.SetFractalType(FastNoiseLite.FractalType.PingPong);
            this.chunkNoise.SetFractalOctaves(4);
            this.chunkNoise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Euclidean);
            rng = new RNG((int) System.DateTime.Now.Ticks);
        }

        public string GetBlockAt(Vector3i position)
        {
            int chunk_x = (position.X / Chunk.CHUNK_SIZE);
            int chunk_y = (position.Y / Chunk.CHUNK_SIZE);
            int chunk_z = (position.Z / Chunk.CHUNK_SIZE);
            Vector3i chunk_pos = new Vector3i(chunk_x, chunk_y, chunk_z);

            if (chunks.ContainsKey(chunk_pos))
            {
                Chunk chunk = chunks[chunk_pos];
                Vector3 local_pos = new Vector3(
                    position.X - chunk_x * Chunk.CHUNK_SIZE,
                    position.Y - chunk_y * Chunk.CHUNK_SIZE,
                    position.Z - chunk_z * Chunk.CHUNK_SIZE
                );

                while (local_pos.X < 0) local_pos.X += Chunk.CHUNK_SIZE;
                while (local_pos.Y < 0) local_pos.Y += Chunk.CHUNK_SIZE;
                while (local_pos.Z < 0) local_pos.Z += Chunk.CHUNK_SIZE;

                return chunk.GetBlockAt(local_pos);
            }
            else
            {
                return "air";
            }
        }

        public static Vector3i getChunkCoordinates(Vector3i worldPos)
        {
            int chunk_x = (worldPos.X / Chunk.CHUNK_SIZE);
            int chunk_y = (worldPos.Y / Chunk.CHUNK_SIZE);
            int chunk_z = (worldPos.Z / Chunk.CHUNK_SIZE);
            return new Vector3i(chunk_x, chunk_y, chunk_z);
        }

        public static Vector3i getCoordinatesInChunk(Vector3i worldPos)
        {
            int local_x = worldPos.X % Chunk.CHUNK_SIZE;
            int local_y = worldPos.Y % Chunk.CHUNK_SIZE;
            int local_z = worldPos.Z % Chunk.CHUNK_SIZE;
            while (local_x < 0) local_x += Chunk.CHUNK_SIZE;
            while (local_y < 0) local_y += Chunk.CHUNK_SIZE;
            while (local_z < 0) local_z += Chunk.CHUNK_SIZE;
            return new Vector3i(local_x, local_y, local_z);
        } 

        public void DrawAround(Vector3 position)
        {
            int render_distance = 3;

            for (int x = -render_distance; x <= render_distance; x++)
            {
                for (int y = -render_distance; y <= render_distance; y++)
                {
                    for (int z = -render_distance; z <= render_distance; z++)
                    {
                        int jx = ((int)(position.X / Chunk.CHUNK_SIZE) + x);
                        int jy = ((int)(position.Y / Chunk.CHUNK_SIZE) + y);
                        int jz = ((int)(position.Z / Chunk.CHUNK_SIZE) + z);

                        Vector3i chunk_pos = new Vector3i(jx, jy, jz);

                        if (!chunks.ContainsKey(chunk_pos))
                        {
                            Console.WriteLine(chunk_pos);
                            Chunk new_chunk = new Chunk(new Vector3(jx * Chunk.CHUNK_SIZE, jy * Chunk.CHUNK_SIZE, jz * Chunk.CHUNK_SIZE));
                            new_chunk.Generate(this.chunkNoise, this.worldNoise);
                            chunks.Add(chunk_pos, new_chunk);
                        }

                        chunks[chunk_pos].Draw();
                    }
                }
            }
        }

        public void SetBlockAt(Vector3 position, string blockType)
        {
            int chunk_x = (int)(position.X / Chunk.CHUNK_SIZE);
            int chunk_y = (int)(position.Y / Chunk.CHUNK_SIZE);
            int chunk_z = (int)(position.Z / Chunk.CHUNK_SIZE);
            Vector3i chunk_pos = new Vector3i(chunk_x, chunk_y, chunk_z);
            if (chunks.ContainsKey(chunk_pos))
            {
                Chunk chunk = chunks[chunk_pos];
                Vector3 local_pos = new Vector3(
                    position.X - chunk_x * Chunk.CHUNK_SIZE,
                    position.Y - chunk_y * Chunk.CHUNK_SIZE,
                    position.Z - chunk_z * Chunk.CHUNK_SIZE
                );
                chunk.SetBlockAt(local_pos, blockType);
            }
        }
    }
}
