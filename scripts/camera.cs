using System.Numerics;
using Raylib_cs;

namespace Labirynt_3_Edytor.scripts;

public static class Camera
{
    public static Vector2 CameraOffset;

    public static void Start()
    {
        Program.UpdateScripts += UpdateCamera;
    }
    private static void UpdateCamera()
    {
        if(Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT)) CameraOffset += Raylib.GetMouseDelta();
        
        if (BlockSpawn.Board[0].Count * 86 + CameraOffset.X <= Convert.ToInt32(Raylib.GetScreenWidth() / 2))
        {
            CameraOffset.X = -BlockSpawn.Board[0].Count * 86 + Convert.ToInt32(Raylib.GetScreenWidth() / 2);
        }
        else if (CameraOffset.X >= Convert.ToInt32(Raylib.GetScreenWidth() / 2))
        {
            CameraOffset.X = 0 + Convert.ToInt32(Raylib.GetScreenWidth() / 2);
        }
        if (CameraOffset.Y >= Convert.ToInt32(Raylib.GetScreenHeight() / 2))
        {
            CameraOffset.Y = 0 + Convert.ToInt32(Raylib.GetScreenHeight() / 2);
        }
        else if (BlockSpawn.Board.Count * 86 + CameraOffset.Y <= Convert.ToInt32(Raylib.GetScreenHeight() / 2))
        {
            CameraOffset.Y = -BlockSpawn.Board.Count * 86 + Convert.ToInt32(Raylib.GetScreenHeight() / 2);
        }
    }
}