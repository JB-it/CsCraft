using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace CsCraft
{
    struct MeshPart     {
        public List<float> vertices;
        public List<float> texcoords;
        public List<float> normals;
    }

    internal enum Sides
    {
        TOP    = 0b000001,
        BOTTOM = 0b000010,
        LEFT   = 0b000100,
        RIGHT  = 0b001000,
        FRONT  = 0b010000,
        BACK   = 0b100000
    }

    internal class Block
    {
        private Vector3 position;
        public string type { get; set; }
        public Block(Vector3 position, String type)
        {
            this.position = position;
            this.type = type;
        }

        public MeshPart GenerateMeshAtBlockPosition(int side_bitmask)
        {
            List<float> vertices = new List<float>();
            List<float> texcoords = new List<float>();
            List<float> normals = new List<float>();

            if (side_bitmask == 0)
            {
                return new MeshPart
                {
                    vertices = vertices,
                    texcoords = texcoords,
                    normals = normals,
                };
            }

            // Define cube face vertices (each face has 2 triangles = 6 vertices)
            if ((side_bitmask & (int)Sides.BACK) > 0)
            {
                float[] frontFaceVertices = new float[]
                {
                    -0.5f, -0.5f, 0.5f,  0.5f, -0.5f, 0.5f,  0.5f, 0.5f, 0.5f,
                    -0.5f, -0.5f, 0.5f,  0.5f, 0.5f, 0.5f,  -0.5f, 0.5f, 0.5f
                };
                vertices.AddRange(frontFaceVertices);
                float[] uvs = Atlas.Instance.getTextureCoordinates(this.type, Sides.FRONT);
                float[] uv_coords = new float[] { uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] };
                texcoords.AddRange(uv_coords);
                for (int i = 0; i < 6; i++)
                {
                    normals.Add(0);
                    normals.Add(0);
                    normals.Add(1);
                }
            }

            if ((side_bitmask & (int)Sides.FRONT) > 0)
            {
                float[] backFaceVertices = new float[]
                {
                    0.5f, -0.5f, -0.5f,  -0.5f, -0.5f, -0.5f,  -0.5f, 0.5f, -0.5f,
                    0.5f, -0.5f, -0.5f,  -0.5f, 0.5f, -0.5f,  0.5f, 0.5f, -0.5f
                };
                vertices.AddRange(backFaceVertices);
                float[] uvs = Atlas.Instance.getTextureCoordinates(this.type, Sides.BACK);
                float[] uv_coords = new float[] { uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] };
                texcoords.AddRange(uv_coords);
                for (int i = 0; i < 6; i++)
                {
                    normals.Add(0);
                    normals.Add(0);
                    normals.Add(-1);
                }
            }

            if ((side_bitmask & (int)Sides.TOP) > 0)
            {
                float[] topFaceVertices = new float[]
                {
                    -0.5f, 0.5f, 0.5f,  0.5f, 0.5f, 0.5f,  0.5f, 0.5f, -0.5f,
                    -0.5f, 0.5f, 0.5f,  0.5f, 0.5f, -0.5f,  -0.5f, 0.5f, -0.5f
                };
                vertices.AddRange(topFaceVertices);
                float[] uvs = Atlas.Instance.getTextureCoordinates(this.type, Sides.TOP);
                float[] uv_coords = new float[] { uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] };
                texcoords.AddRange(uv_coords);
                for (int i = 0; i < 6; i++)
                {
                    normals.Add(0);
                    normals.Add(1);
                    normals.Add(0);
                }
            }

            if ((side_bitmask & (int)Sides.BOTTOM) > 0)
            {
                float[] bottomFaceVertices = new float[]
                {
                    -0.5f, -0.5f, -0.5f,  0.5f, -0.5f, -0.5f,  0.5f, -0.5f, 0.5f,
                    -0.5f, -0.5f, -0.5f,  0.5f, -0.5f, 0.5f,  -0.5f, -0.5f, 0.5f
                };
                vertices.AddRange(bottomFaceVertices);
                float[] uvs = Atlas.Instance.getTextureCoordinates(this.type, Sides.BOTTOM);
                float[] uv_coords = new float[] { uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] };
                texcoords.AddRange(uv_coords);
                for (int i = 0; i < 6; i++)
                {
                    normals.Add(0);
                    normals.Add(-1);
                    normals.Add(0);
                }
            }

            if ((side_bitmask & (int)Sides.RIGHT) > 0)
            {
                float[] rightFaceVertices = new float[]
                {
                    0.5f, -0.5f, 0.5f,  0.5f, -0.5f, -0.5f,  0.5f, 0.5f, -0.5f,
                    0.5f, -0.5f, 0.5f,  0.5f, 0.5f, -0.5f,  0.5f, 0.5f, 0.5f
                };
                vertices.AddRange(rightFaceVertices);
                float[] uvs = Atlas.Instance.getTextureCoordinates(this.type, Sides.RIGHT);
                float[] uv_coords = new float[] { uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] };
                texcoords.AddRange(uv_coords);
                for (int i = 0; i < 6; i++)
                {
                    normals.Add(1);
                    normals.Add(0);
                    normals.Add(0);
                }
            }

            if ((side_bitmask & (int)Sides.LEFT) > 0)
            {
                float[] leftFaceVertices = new float[]
                {
                    -0.5f, -0.5f, -0.5f,  -0.5f, -0.5f, 0.5f,  -0.5f, 0.5f, 0.5f,
                    -0.5f, -0.5f, -0.5f,  -0.5f, 0.5f, 0.5f,  -0.5f, 0.5f, -0.5f
                };
                vertices.AddRange(leftFaceVertices);
                float[] uvs = Atlas.Instance.getTextureCoordinates(this.type, Sides.LEFT);
                float[] uv_coords = new float[] { uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] + uvs[3], uvs[0] + uvs[2], uvs[1], uvs[0], uvs[1] };
                texcoords.AddRange(uv_coords);
                for (int i = 0; i < 6; i++)
                {
                    normals.Add(-1);
                    normals.Add(0);
                    normals.Add(0);
                }
            }

            for (int i = 0; i < vertices.Count; i += 3)
            {
                vertices[i] += this.position.X;
                vertices[i + 1] += this.position.Y;
                vertices[i + 2] += this.position.Z;
            }


            return new MeshPart
            {
                vertices = vertices,
                texcoords = texcoords,
                normals = normals,
            };
        }        
    }
}