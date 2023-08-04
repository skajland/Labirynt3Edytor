using Raylib_cs;
using System.Numerics;

namespace Labirynt_3_Edytor.scripts;
//I'm making this game editor at the beginning of 14 years.
internal static class Program
{
    public static Action UpdateScripts = () => {};
    public static Action RenderScripts = () => {};
    public static bool Running = true;
    public static void Main()
    {
        Start();
        while (Running)
        {
            Update();
            Render();
        }
        Raylib.CloseWindow();
    }

    private static void Start()
    {
        Raylib.InitWindow(1462, 946, "Labirynt 3 Edytor");
        Raylib.SetTargetFPS(90);
        SaveLoadSystem.GenerateBoard();
        Camera.Start();
        UseFull.Start();
        BlockSpawn.Start();
        MenuBlock.Start();
        MenuGameData.Start();
        MenuTop.Start();
        MiniMenuTop.Start();
    }

    private static void Update()
    {
        UpdateScripts.Invoke();
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_J)) SaveLoadSystem.SaveGame();
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_L)) SaveLoadSystem.LoadGame();
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_X)) UseFull.DeleteItems();
        if (Raylib.WindowShouldClose()) Running = false;
    }
    private static void Render()
    {
        Raylib.BeginDrawing();
        var color = SaveLoadSystem.GameData.BackgroundColor;
        Raylib.ClearBackground(new Color(color[0], color[1], color[2], 255));
        RenderingBlocks();
        // top line
        Raylib.DrawLineEx(Camera.CameraOffset, Camera.CameraOffset with { X = BlockSpawn.Board[0].Count * 86 + Camera.CameraOffset.X }, 5, Color.DARKGREEN);
        // left line
        Raylib.DrawLineEx(Camera.CameraOffset, Camera.CameraOffset with { Y = BlockSpawn.Board.Count * 86 + Camera.CameraOffset.Y }, 5, Color.DARKGREEN);
        // bottom line
        Raylib.DrawLineEx(Camera.CameraOffset with {Y = BlockSpawn.Board.Count * 86 + Camera.CameraOffset.Y }, 
            new Vector2(BlockSpawn.Board[0].Count * 86 + Camera.CameraOffset.X,BlockSpawn.Board.Count * 86 + Camera.CameraOffset.Y ), 5, Color.DARKGREEN);
        // right line
        Raylib.DrawLineEx(Camera.CameraOffset with {X = BlockSpawn.Board[0].Count*86+Camera.CameraOffset.X},
            new Vector2(BlockSpawn.Board[0].Count*86+Camera.CameraOffset.X, BlockSpawn.Board.Count*86+Camera.CameraOffset.Y), 5, Color.DARKGREEN);
        RenderScripts.Invoke();
        Raylib.EndDrawing();
    }

    private static void RenderingBlocks()
    {
        var sortedBoard = BlockSpawn.Board;
        foreach (var row in sortedBoard)
        {
            foreach (var item in row)
            {
                item.Sort(CompareGrids);
            }
        }
        for (var i = 0; i < sortedBoard.Count; i++)
        {
            for (var j = 0; j < sortedBoard[i].Count; j++)
            {
                for (var k = 0; k < sortedBoard[i][j].Count; k++)
                {
                    if(sortedBoard[i][j][k] == 0) break;
                    var index = sortedBoard[i][j][k] - 1;
                    var blockMiddleX = (86 - BlockSpawn.BlocksList[index].Texture.width) / 2;
                    var blockMiddleY = (86 - BlockSpawn.BlocksList[index].Texture.height) / 2;
                    Raylib.DrawTextureV(BlockSpawn.BlocksList[index].Texture,
                        new Vector2(j * 86 + blockMiddleX + Camera.CameraOffset.X, i * 86 + blockMiddleY + Camera.CameraOffset.Y),
                        Color.WHITE);
                }
            }
        }
    }
    private static int CompareGrids(int grid1, int grid2)
    {
        int sum1 = BlockSpawn.BlocksList[grid1 - 1].Layer;
        int sum2 = BlockSpawn.BlocksList[grid2 - 1].Layer;

        return sum1.CompareTo(sum2);
    }
}