using System.Numerics;
using Raylib_cs;

namespace Labirynt_3_Edytor.scripts{

    internal static class BlockSpawn{
        private static States _state = States.Running;
        private static int _currentIndexBlock;
        public static List<List<List<int>>> Board = new List<List<List<int>>>();
        private const int GridSize = 86;
        public static readonly List<int> UsedBlockIndex = new List<int>();
        public static readonly Block[] BlocksList =
        {
            new Block(Raylib.LoadImage("res/blocks/grass.png"), new Vector2(86,86),1, 1, "B"),
            new Block(Raylib.LoadImage("res/entities/enemy.png"), new Vector2(72,92),2,2, "E"),
            new Block(Raylib.LoadImage("res/entities/Player.png"), new Vector2(86,86),3, 3, "E"),
            new Block(Raylib.LoadImage("res/blocks/key.png"), new Vector2(86,86), 4, 2, "BI"),
            new Block(Raylib.LoadImage("res/blocks/entrance.png"), new Vector2(86,86), 5, 3, "BI"),
            new Block(Raylib.LoadImage("res/blocks/entrance.png"), new Vector2(86,86), 6, 3, "BI"),
            new Block(Raylib.LoadImage("res/blocks/WinBlock.png"),new Vector2(86,86), 7, 2, "B"),
            new Block(Raylib.LoadImage("res/blocks/WaterBlock.png"), new Vector2(86,86), 8, 2, "B"),
            new Block(Raylib.LoadImage("res/blocks/OakLog.png"), new Vector2(86,86), 9, 2, "BI"),
            new Block(Raylib.LoadImage("res/blocks/sand.png"), new Vector2(86,86), 10, 1, "B")
        };
        public static Block CurrentBlock = BlocksList[0];

        private enum States { 
            Running,
            Pressed
        }
        private static void SpawnBlock(){
            _state = States.Running;
        }

        private static void Placing()
        {
            for (int i = 0; i < Board.Count; i++)
            {
                for (int j = 0; j < Board[i].Count; j++)
                {
                    if (CurrentBlock.Index != 3) break;
                    // see if placeblock is in the borders that the player wont go away(it's kinda a ugly process but what are you gonna do)
                    if (CurrentBlock.Pos.X - Camera.CameraOffset.X < 0) break;
                    if (CurrentBlock.Pos.X - Camera.CameraOffset.X > Board[0].Count * 86 - GridSize) break;
                    if (CurrentBlock.Pos.Y - Camera.CameraOffset.Y < 0) break;
                    if (CurrentBlock.Pos.Y - Camera.CameraOffset.Y > Board.Count * 86 - GridSize) break;               
                    for (int k = 0; k < Board[i][j].Count; k++)
                    {
                        if (Board[i][j][0] == 3) Board[i][j][0] = 0;
                        if (Board[i][j][k] == 3) Board[i][j].Remove(3);
                    }
                }
            }

            for (int i = 0; i < Board.Count; i++)
            {
                for (int j = 0; j < Board[i].Count; j++)
                {
                    Vector2 pos = new Vector2(86 * j, 86 * i);
                    if (pos != CurrentBlock.Pos - Camera.CameraOffset) continue;
                    for (int k = 0; k < Board[i][j].Count; k++)
                    {
                        if(Board[i][j][k] == 0) continue;
                        if(BlocksList[Board[i][j][k] - 1].Filter != CurrentBlock.Filter) continue;
                        Board[i][j][k] = CurrentBlock.Index; 
                        return;
                    }
                    if (Board[i][j][0] == 0) {
                        Board[i][j][0] = CurrentBlock.Index;   
                        return;
                    }
                    Board[i][j].Add(CurrentBlock.Index);
                    return;
                }
            }
        }
        public static void Start()
        {
            Program.UpdateScripts += Update;
        }
        private static void Update(){
            if (!UseFull.MenuRectCollision)
            {
                if(Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    _state = States.Pressed;
                }
            }
            var mousePos = Raylib.GetMousePosition();
            const int centerGrid = GridSize / 2;
            CurrentBlock.Pos = new Vector2((float)Math.Round((mousePos.X - Camera.CameraOffset.X - centerGrid) / GridSize) * GridSize,
                (float)Math.Round((mousePos.Y - Camera.CameraOffset.Y - centerGrid) / GridSize) * GridSize) + Camera.CameraOffset;
            if (_state != States.Pressed) { return; }
            Placing();
            _state = States.Running;
            SpawnBlock();
            }

        public static void ChangeIndex(params int[] index)
        {
            _currentIndexBlock = index[0];
            CurrentBlock = BlocksList[_currentIndexBlock];
            var mousePos = Raylib.GetMousePosition();
            const int centerGrid = GridSize / 2;
            CurrentBlock.Pos = new Vector2((float)Math.Round((mousePos.X - centerGrid) / 86) * 86, (float)Math.Round((mousePos.Y - centerGrid) / 86) * 86);
            foreach (var blockIndex in UsedBlockIndex)
            {
                if(_currentIndexBlock == blockIndex) return;
            }
            UsedBlockIndex.Insert(0, _currentIndexBlock);
            if (UsedBlockIndex.Count > 5)
            {
                UsedBlockIndex.RemoveAt(UsedBlockIndex.Count - 1);
            }
        }
    }
}