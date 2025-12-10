using CsCraft.utils;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace CsCraft
{
    internal class Player
    {
        public Vector3 Position;
        private Camera3D camera;
        private World world;
        public Vector3 LookingAt;

        public Player(Vector3 startPos, ref World world)
        {
            Position = startPos;
            camera = new Camera3D
            {
                Position = new Vector3(startPos.X, startPos.Y + 2, startPos.Z),
                Target = new Vector3(1f, 0f, 0f) + startPos,
                Up = new Vector3(0.0f, 1.0f, 0.0f),
                FovY = 45.0f,
                Projection = CameraProjection.Perspective
            };

            this.world = world;
        }

        public Camera3D getCamera()
        {
            return camera;
        }

        public void Update()
        {
            if (world == null)
            {
                return;
            }

            //Raymarch r = new Raymarch(this.Position, new Vector3(0, -1, 0), 10.0f, this.world);
            //var (hit, hitPosition, length, blockType) = r.March();
            //if (hit)
            //{
            //    float fall_distance = 9.81f * Raylib.GetFrameTime();
            //    if (length > fall_distance)
            //    {
            //        Position.Y -= fall_distance;
            //    }
            //    else
            //    {
            //        Position.Y -= length;
            //    }
            //}

            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                Vector3 look_direction = camera.Target - Position;
                Position += Vector3.Normalize(camera.Target - camera.Position) * 5.0f * Raylib.GetFrameTime();
                camera.Target = Position + look_direction;
            }

            if (Raylib.IsKeyDown(KeyboardKey.Space))
            {
                Vector3 look_direction = camera.Target - Position;
                Position.Y += 5.0f * Raylib.GetFrameTime();
                camera.Target = Position + look_direction;
            }

            if (Raylib.IsKeyDown(KeyboardKey.LeftShift))
            {
                Vector3 look_direction = camera.Target - Position;
                Position.Y -= 5.0f * Raylib.GetFrameTime();
                camera.Target = Position + look_direction;
            }

            Vector3 lookingDir = Vector3.Normalize(camera.Target - camera.Position);
            Raycast lookingAt = new Raycast(this.Position + new Vector3(0, 1.8f, 0), lookingDir, 200.0f, this.world);
            var (isHit, hitPos, hitLength, hitBlockType) = lookingAt.Shoot();
            if(isHit)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    world.SetBlockAt(hitPos + lookingDir * 0.1f , "air");
                }
                if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                {
                    Vector3 placePos = hitPos - Vector3.Normalize(camera.Target - camera.Position) * 0.1f;
                    world.SetBlockAt(hitPos - lookingDir * 0.1f, "dirt");
                }

                this.LookingAt = hitPos;
            } else
            {
                this.LookingAt = Vector3.Zero;
            }

                UpdateCamera();
        }

        public void UpdateCamera()
        {
            Raylib.UpdateCamera(ref camera, CameraMode.FirstPerson);
            camera.Position = new Vector3(Position.X, Position.Y + 1.8f, Position.Z);
        }
    }
}
