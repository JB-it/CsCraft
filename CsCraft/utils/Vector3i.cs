using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace CsCraft.utils
{
    internal class Vector3i
    {
        public int X;
        public int Y;
        public int Z;
        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3i FromVector3(System.Numerics.Vector3 v)
        {
            return new Vector3i((int)v.X, (int)v.Y, (int)v.Z);
        }

        public static Vector3i operator+ (Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3i operator -(Vector3i a, Vector3i b)
        {
            return new Vector3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Vector3i v2 = (Vector3i)obj;  

            return this.X == v2.X && this.Y == v2.Y && this.Z == v2.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
