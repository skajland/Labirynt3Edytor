using System.Numerics;
using Raylib_cs;

namespace Labirynt_3_Edytor.scripts{

    internal static class BlockSpawn{
        private static States _state = States.Running;
        private static int _currentIndexBlock;
        public static List<List<List<int>>> Board = new List<List<List<int>>>();
        private const int GridSize = 86;
        public static readonly List<int> UsedBlockIndex = new List<int>();
        
        public static readonly int Coins = SaveLoadSystem.LoadCoins();
        
        public static readonly Block[] BlocksList =
        {   new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/grass.png"), new Vector2(86,86),1, 1, "B", 0),
            new Block(Raylib.LoadImage(Program.DataDir + "res/enemy.png"), new Vector2(72,92),2,5, "E", 0),
            new Block(Raylib.LoadImage(Program.DataDir + "res/Player.png"), new Vector2(86,86),3, 3, "E", 0),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/key.png"), new Vector2(86,86), 4, 2, "BI", 0),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/entrance.png"), new Vector2(86,86), 5, 3, "BI", 0),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/WinBlock.png"),new Vector2(86,86), 6, 2, "B", 0),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/WaterBlock.png"), new Vector2(86,86), 7, 2, "B", 3),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/OakLog.png"), new Vector2(86,86), 8, 2, "BI", 3),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/sand.png"), new Vector2(86,86), 9, 1, "B", 4),
            new Block(Raylib.LoadImage(Program.DataDir + "res/enemyBoss.png"), new Vector2(104,124), 12, 5, "E", 7),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/TurretBase.png"), new Vector2(86,86), 13, 2, "BI", 6),        
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/Lava.png"), new Vector2(86,86), 14, 1, "B", 6),      
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/Redkey.png"), new Vector2(86,86), 15, 2, "BI", 1),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/Greenkey.png"), new Vector2(86,86), 16, 2, "BI", 1),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/Bluekey.png"), new Vector2(86,86), 17, 2, "BI", 2),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/entranceRed.png"), new Vector2(86,86), 18, 3, "BI", 1),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/entranceGreen.png"), new Vector2(86,86), 19, 3, "BI", 1),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/entranceBlue.png"), new Vector2(86,86), 20, 3, "BI", 2),
            new Block(Raylib.LoadImage(Program.DataDir + "res/enemyRound.png"), new Vector2(72,92), 21, 5, "E", 5),
            new Block(Raylib.LoadImage(Program.DataDir + "res/enemyTriangle.png"), new Vector2(86,110), 22, 5, "E", 5),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/netherack.png"), new Vector2(86,86), 23, 1, "B", 5),
            new Block(Raylib.LoadImage(Program.DataDir + "res/blocks/stone.png"), new Vector2(86,86), 30, 1, "B", 4)
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
                        var index = UseFull.CalculateIndex(Board[i][j][k]);
                        if(BlocksList[index].Filter != CurrentBlock.Filter) continue;
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
            Program.RenderScripts += Render;
        }
        private static void Update(){
            if (!UseFull.MenuRectCollision)
            {
                if(Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) || Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
                {
                    _state = States.Pressed;
                }
            }
            var mousePos = Raylib.GetMousePosition();
            const int centerGrid = GridSize / 2;
            CurrentBlock.Pos = new Vector2((float)Math.Round((mousePos.X - Camera.CameraOffset.X - centerGrid) / GridSize) * GridSize,
                (float)Math.Round((mousePos.Y - Camera.CameraOffset.Y - centerGrid) / GridSize) * GridSize) + Camera.CameraOffset;
            if (_state != States.Pressed) return;
            Placing();
            _state = States.Running;
            SpawnBlock();
            }

        private static void Render()
        {
            var currentBlockMiddleX = (86 - BlockSpawn.CurrentBlock.Texture.width) / 2;
            var currentBlockMiddleY = (86 - BlockSpawn.CurrentBlock.Texture.height) / 2;
            Raylib.DrawTextureV(CurrentBlock.Texture,
                CurrentBlock.Pos + new Vector2(currentBlockMiddleX, currentBlockMiddleY), Color.WHITE);
            var rect = new Rectangle(CurrentBlock.Pos.X + currentBlockMiddleX,CurrentBlock.Pos.Y + currentBlockMiddleY,
                CurrentBlock.Texture.width, CurrentBlock.Texture.height);
            Raylib.DrawRectangleRec(rect, new Color(150, 150, 150, 70));
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