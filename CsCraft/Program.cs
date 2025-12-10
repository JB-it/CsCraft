using CsCraft;
using CsCraft.utils;
using Raylib_cs;
using System.Numerics;

public static class Program
{

    public static void Main(string[] args) {
        Raylib.InitWindow(2000, 1200, "CsCraft");
        Raylib.SetTargetFPS(60);

        World w = new World("world");

        Player player = new Player(new Vector3(0f, 10f, 0f), ref w);

        Raylib.DisableCursor();
        
        while(!Raylib.WindowShouldClose())
        {
            player.Update();
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.SkyBlue);

            Raylib.BeginMode3D(player.getCamera());

            Vector3 lookPos = player.LookingAt;
            Raylib.DrawSphere(lookPos, 0.3f, Color.Red);

            w.DrawAround(player.Position);

            Raylib.EndMode3D();

            Raylib.DrawCircle(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2, 5, Color.White);

            Raylib.DrawText("X: " + lookPos.X, 10, 10, 20, Color.White);
            Raylib.DrawText("Y: " + lookPos.Y, 10, 30, 20, Color.White);
            Raylib.DrawText("Z: " + lookPos.Z, 10, 50, 20, Color.White);

            Raylib.DrawText("Chunk Coordinates: " + World.getChunkCoordinates(new Vector3i((int)lookPos.X, (int)lookPos.Y, (int)lookPos.Z)), 10, 80, 20, Color.White);
            Raylib.DrawText("Block Coordinates: " + World.getCoordinatesInChunk(new Vector3i((int)lookPos.X, (int)lookPos.Y, (int)lookPos.Z)), 10, 100, 20, Color.White);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();

        Console.WriteLine("Hello, World!");
    }

}
