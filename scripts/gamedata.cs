using System.Numerics;
using System.Text;
using Raylib_cs;

namespace Labirynt_3_Edytor.scripts;

public class GameDataFile
{
    public int[] CameraOffsetPos = new int[]{0, 0};
    public int[] BackgroundColor = new int[] {105, 200, 75};
    public int[] CameraBorders = new int[] { 258, 258, 258, 258 };
}

public static class SaveLoadSystem
{
    public static readonly GameDataFile GameData = new GameDataFile();

    public static void SaveGame()
    {
        ExportLevel();
        SaveGameData();
    }
    
    private static void SaveGameData()
    {
        var convertedCameraOffset = ":" + string.Join(",", GameData.CameraOffsetPos);
        var convertedColor = "\n:" + string.Join(",", GameData.BackgroundColor);
        var convertedCameraBorders = "\n:" + string.Join(",", GameData.CameraBorders);
        File.WriteAllText("MyLevel/gamedata", convertedCameraOffset + convertedColor + convertedCameraBorders);
    }
    private static string BoardToString(List<List<List<int>>> thisBoard)
    {
        StringBuilder result = new StringBuilder();
        foreach (var boardListPiece in thisBoard)
        {
            foreach (var boardPiece in boardListPiece)
            {
                result.Append("[").Append(string.Join(",", boardPiece)).Append("]");
            }
            result.AppendLine();
        }
        return result.ToString();
    }

    private static void ExportLevel()
    {
        File.WriteAllText("MyLevel/level", BoardToString(BlockSpawn.Board));
    }
    public static void LoadGame()
    {
        LoadLevel();
        LoadGameData();
    }
    
    private static void LoadLevel()
    {
        string[] boardRows = File.ReadAllLines("MyLevel/level");
        List<List<List<int>>> newBoard = new List<List<List<int>>>();
        foreach(string boardRow in boardRows)
        {
            string[] lineStripped = boardRow.Trim('[').Trim(']').Split("][");
            List<List<int>> row = new();
            foreach (var line in lineStripped)
            {
                string[] splatLine = line.Split(',');
                List<int> item = new List<int>();
                foreach (var l in splatLine)
                {
                    item.Add(Convert.ToInt32(l));
                }
                row.Add(item);
            }
            newBoard.Add(row);
        }

        BlockSpawn.Board = newBoard;
    }
    private static void LoadGameData()
    {
        var data = File.ReadAllLines("MyLevel/gamedata");
        var entireData = new List<List<int>>();

        foreach (var dataItem in data)
        {
            var splitData = dataItem.Trim(':').Split(",");
            var splitDataInt = new List<int>();
            foreach (var toInt in splitData)
            {
                splitDataInt.Add(Convert.ToInt32(toInt));
            }
            entireData.Add(splitDataInt);
        }
        GameData.CameraOffsetPos = entireData[0].ToArray();
        GameData.BackgroundColor = entireData[1].ToArray();
        GameData.CameraBorders = entireData[2].ToArray();
        var j = 0;
        foreach (var dataItem in entireData[0]) { MenuGameData.TextBoxesCameraOffset[j].NumberPressed = Convert.ToInt32(dataItem); j++; }
        j = 0;
        foreach (var dataItem in entireData[1]) { MenuGameData.TextBoxesBackgroundColor[j].NumberPressed = Convert.ToInt32(dataItem); j++; }
        j = 0;
        foreach (var dataItem in entireData[2]) { MenuGameData.TextBoxesCameraBoundaries[j].NumberPressed = Convert.ToInt32(dataItem); j++; }
    }
    public static void GenerateBoard()
    {
        const int width = 86;
        const int height = 86;
            
        BlockSpawn.Board.Clear();
            
        for (int i = 0; i < height; i++)
        {
            BlockSpawn.Board.Add(new List<List<int>>());
            for (int j = 0; j < width; j++)
            {
                BlockSpawn.Board[i].Add(new List<int>() {0});                  
            }
        }
    }
}

internal static class UseFull
{
    public static bool MenuRectCollision;

    static UseFull()
    {
        Program.UpdateScripts += MenuCollision;
    }
    public static Button CreateButton(Texture2D img, Vector2 pos, params Action[] onClick)
    {
        return new Button(img, pos, 
            new Color(130, 130, 130, 70), new Color(130, 130, 130, 100),
            new Color(160, 160, 160, 150), onClick);
    }
    public static Button CreateButton(string text, Color fontColor, int fontSize, Vector2 pos, params Action[] onClick)
    {
        return new Button(text, fontColor, pos, fontSize, 
            new Color(130, 130, 130, 70), new Color(130, 130, 130, 100),
            new Color(160, 160, 160, 150), onClick);
    }
    
    private static void MenuCollision()
    {
        MenuRectCollision = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), MenuBlock.MenuRect) || 
                            Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), MenuGameData.MenuRect) && MenuGameData.MenuRectEnabled ||
                            Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), MenuTop.TopRect) ||
                            Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), MiniMenuTop.Rect) && MiniMenuTop.Enabled;
    }
}