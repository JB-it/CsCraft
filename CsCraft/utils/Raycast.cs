using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CsCraft.utils
{
    internal class Raycast
    {
        public Vector3 startPosition { get; set; }
        public Vector3 direction { get; set; }
        public float maxDistance { get; set; }

        public World world { private get; set; }

        public Raycast(Vector3 startPosition, Vector3 direction, float maxDistance, World world)
        {
            this.startPosition = startPosition;
            this.direction = Vector3.Normalize(direction);
            this.maxDistance = maxDistance;
            this.world = world;
        }

        public (bool hit, Vector3 position, float length, string blockType) Shoot()
        {
            float totalDistance = 0.0f;
            Vector3 currentPosition = startPosition;
            while (totalDistance < maxDistance)
            {
                string blockType = world.GetBlockAt(Vector3i.FromVector3(currentPosition + new Vector3(0.5f, 0.5f, 0.5f)));
                if (blockType != "air")
                {
                    return (true, currentPosition, totalDistance, blockType);
                }
                // Move forward by a small step
                float stepSize = 0.001f;
                currentPosition += direction * stepSize;
                totalDistance += stepSize;
            }
            return (false, currentPosition, this.maxDistance, "air");
        }
    }
}
