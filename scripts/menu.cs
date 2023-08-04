using Raylib_cs;
using System.Numerics;
namespace Labirynt_3_Edytor.scripts;

internal static class MenuBlock
{
    private static readonly List<Button> AllButtons = new List<Button>();
    public static Rectangle MenuRect;
    public static readonly List<Texture2D> PreloadedTextures = new List<Texture2D>();
    private static void Update()
    {
        foreach(var button in AllButtons)
        {
            button.Collision();
        }
    }

    public static void Start()
    {
        Program.UpdateScripts += Update;
        Program.RenderScripts += Render;
        var menuPos = new Vector2();
        float blockListLength = BlockSpawn.BlocksList.Length;
        const int buttonSize = 64;
        const int rowLength = 12;
        const int gridWidth = rowLength * buttonSize;
        const int gridHeight = buttonSize * 3;
        menuPos.X = Convert.ToInt32(Raylib.GetScreenWidth() / 2 - gridWidth / 2);
        menuPos.Y = Convert.ToInt32(Raylib.GetScreenHeight() - gridHeight);

        for (var i = 0; i < blockListLength; i++)
        {
            var row = i / rowLength;
            var col = i % rowLength;

            Texture2D buttonTexture = BlockSpawn.BlocksList[i].Texture;
            Image buttonImage = Raylib.LoadImageFromTexture(buttonTexture);
            Raylib.ImageResizeNN(ref buttonImage, 64, 64);
            buttonTexture = Raylib.LoadTextureFromImage(buttonImage);
            PreloadedTextures.Add(buttonTexture);
            var buttonX = menuPos.X + col * buttonSize;
            var buttonY = menuPos.Y + row * buttonSize;

            CreateButton(buttonTexture, new Vector2(buttonX, buttonY), i);
        }
        

        MenuRect = new Rectangle(menuPos.X, menuPos.Y, gridWidth, gridHeight);
    }

    private static void CreateButton(Texture2D img, Vector2 pos, int index)
    {
        AllButtons.Add(new Button(img, pos,
            new Color(130, 130, 130, 70), new Color(130, 130, 130, 100),
            new Color(160, 160, 160, 150), () => BlockSpawn.ChangeIndex(index), MenuTop.OnButtonClick));
    }

    private static void Render()
    {
        Raylib.DrawRectangleRec(MenuRect, Color.DARKGRAY);
            foreach (var button in AllButtons)
            {
                button.Render();     
            }
    }
}

public static class MenuGameData
{
    public static Rectangle MenuRect;
    public static bool MenuRectEnabled;
    public static readonly List<TextBox> TextBoxesCameraBoundaries = new List<TextBox>();
    public static readonly List<TextBox> TextBoxesCameraOffset = new List<TextBox>();
    public static readonly List<TextBox> TextBoxesBackgroundColor = new List<TextBox>();
    private static readonly List<Button> AddButtons = new List<Button>();
    private static readonly List<Button> RemoveButtons = new List<Button>();
    private static void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_T)) MenuRectEnabled = !MenuRectEnabled;
        if(!MenuRectEnabled) return;
        foreach (var textBox in TextBoxesCameraBoundaries) textBox.Update();
        foreach (var textBox in TextBoxesCameraOffset) textBox.Update();
        foreach (var textBox in TextBoxesBackgroundColor) textBox.Update();
        
        foreach (var addButton in AddButtons) addButton.Collision();
        foreach (var removeButton in RemoveButtons) removeButton.Collision();
        
        SaveLoadSystem.GameData.CameraOffsetPos = new []{TextBoxesCameraOffset[0].NumberPressed, TextBoxesCameraOffset[1].NumberPressed};
        SaveLoadSystem.GameData.CameraBorders = new []{TextBoxesCameraBoundaries[0].NumberPressed, TextBoxesCameraBoundaries[1].NumberPressed, 
            TextBoxesCameraBoundaries[2].NumberPressed, TextBoxesCameraBoundaries[3].NumberPressed};
        foreach (var textBoxBackgroundColor in TextBoxesBackgroundColor)
        {
            if (textBoxBackgroundColor.NumberPressed >= 255) textBoxBackgroundColor.NumberPressed = 255;
        }
        SaveLoadSystem.GameData.BackgroundColor = new []{TextBoxesBackgroundColor[0].NumberPressed, TextBoxesBackgroundColor[1].NumberPressed, 
            TextBoxesBackgroundColor[2].NumberPressed};   
    }

    public static void Start()
    {
        Program.UpdateScripts += Update;
        Program.RenderScripts += Render;
        Vector2 textBoxSize =  new Vector2(150, 100);
        Vector2 textBoxPos = new Vector2(Convert.ToInt32(Raylib.GetScreenWidth() / 2) - textBoxSize.X / 2,
            Convert.ToInt32(Raylib.GetScreenHeight() / 2) - textBoxSize.Y / 2);
        for (int i = -1; i < 1; i++)
        {
            var index = i + 1;
            var buttonPos = new Vector2(textBoxPos.X + 200 * i + 100, textBoxPos.Y + 30);
            TextBoxesCameraOffset.Add(new TextBox(new Rectangle(buttonPos.X, buttonPos.Y, textBoxSize.X, textBoxSize.Y),
                SaveLoadSystem.GameData.CameraOffsetPos[index], 64, 3, Color.BEIGE, Color.BROWN, true, false));
            
            var addButton = UseFull.CreateButton(MenuBlock.PreloadedTextures[0], buttonPos with {Y = buttonPos.Y + textBoxSize.Y},
                () => TextBoxesCameraOffset[index].NumberPressed += 86);
            var removeButton = UseFull.CreateButton(MenuBlock.PreloadedTextures[0], new Vector2(buttonPos.X + textBoxSize.X - MenuBlock.PreloadedTextures[0].width,
                    buttonPos.Y + textBoxSize.Y),() => TextBoxesCameraOffset[index].NumberPressed -= 86);
            AddButtons.Add(addButton);
            RemoveButtons.Add(removeButton);
        }

        for (int i = -2; i < 2; i++)
        {
            var index = i + 2;
            var buttonPos = new Vector2(textBoxPos.X + 170 * i + 85, textBoxPos.Y - 200 + textBoxSize.Y);
            var addButton = UseFull.CreateButton(MenuBlock.PreloadedTextures[0], buttonPos,
                () => TextBoxesCameraBoundaries[index].NumberPressed += 86);
            var removeButton = UseFull.CreateButton(MenuBlock.PreloadedTextures[0],
                buttonPos with {X = buttonPos.X + textBoxSize.X - MenuBlock.PreloadedTextures[0].width},
                () => TextBoxesCameraBoundaries[index].NumberPressed -= 86);
            
            TextBoxesCameraBoundaries.Add(new TextBox(new Rectangle(textBoxPos.X + 170 * i + 85, textBoxPos.Y - 200, textBoxSize.X, textBoxSize.Y),
                SaveLoadSystem.GameData.CameraBorders[index], 64, 3, Color.BEIGE, Color.BROWN, true, false));
            AddButtons.Add(addButton);
            RemoveButtons.Add(removeButton);
        }
        for (int i = -1; i < 2; i++){
            TextBoxesBackgroundColor.Add(new TextBox(new Rectangle(textBoxPos.X + 200 * i, textBoxPos.Y + 250, textBoxSize.X, textBoxSize.Y),
                SaveLoadSystem.GameData.BackgroundColor[i + 1], 64, 3, Color.BEIGE, Color.BROWN, false, true));
        }
        Vector2 size = new Vector2(750, 850);
        Vector2 menuPos = new Vector2(Convert.ToInt32(Raylib.GetScreenWidth() / 2) - size.X / 2,
            Convert.ToInt32(Raylib.GetScreenHeight()) - size.Y);
        MenuRect = new Rectangle(menuPos.X, menuPos.Y, size.X, size.Y);
    }
    private static void Render()
    {
        if (!MenuRectEnabled) return;
        Raylib.DrawRectangleRec(MenuRect, Color.DARKGRAY);
        foreach (var textBox in TextBoxesCameraBoundaries) textBox.Render();
        foreach (var textBox in TextBoxesCameraOffset) textBox.Render();
        foreach (var textBox in TextBoxesBackgroundColor) textBox.Render();
        
        foreach (var addButton in AddButtons) addButton.Render();
        foreach (var removeButton in RemoveButtons) removeButton.Render();
        
        const int fontSize = 48;
        Raylib.DrawText("Camera Offset", Raylib.GetScreenWidth() / 2 - Raylib.MeasureText("Camera Offset", fontSize) / 2,
            Raylib.GetScreenHeight() / 2 - 100, fontSize, Color.ORANGE);
        Raylib.DrawText("Background Color", Raylib.GetScreenWidth() / 2 - Raylib.MeasureText("Background Color", fontSize) / 2,
            Raylib.GetScreenHeight() / 2 + 130, fontSize, Color.ORANGE);
        Raylib.DrawText("Camera Boundaries", Raylib.GetScreenWidth() / 2 - Raylib.MeasureText("Camera Boundaries", fontSize) / 2,
            Raylib.GetScreenHeight() / 2 - 320, fontSize, Color.ORANGE);
    }
}

internal static class MenuTop
{
    public static readonly Rectangle TopRect = new Rectangle(0, 0, Raylib.GetScreenWidth(), 86);
    private static readonly List<Button> QuickButtons = new List<Button>();
    private static Button? _menuButton;
    private static Button? _tpToPlayer;
    private static void Update()
    {
        foreach (var button in QuickButtons) button.Collision();
        _menuButton?.Collision();
        _tpToPlayer?.Collision();
    }

    public static void Start()
    {
        Program.UpdateScripts += Update;
        Program.RenderScripts += Render;
        var menuImg = Raylib.LoadImage("res/menu.png");
        Raylib.ImageResizeNN(ref menuImg, 86, 86);
        _menuButton = UseFull.CreateButton(Raylib.LoadTextureFromImage(menuImg), new Vector2(0, 0),
            () => MiniMenuTop.Enabled = !MiniMenuTop.Enabled);
        
        Image buttonImage = Raylib.LoadImage("res/TpPlayer.png");
        Raylib.ImageResizeNN(ref buttonImage, 64, 64);
        Texture2D buttonTexture = Raylib.LoadTextureFromImage(buttonImage);
        var pos = new Vector2(Convert.ToInt32(Raylib.GetScreenWidth() - 300), TopRect.height / 2 - Convert.ToInt32(buttonTexture.height / 2));
        _tpToPlayer = UseFull.CreateButton(buttonTexture, pos, UseFull.TpToPlayer);    
    }
    public static void OnButtonClick()
    {
        QuickButtons.Clear();
        var i = 0;
        const int buttonSize = 64;
        foreach(var lastBlock in BlockSpawn.UsedBlockIndex)
        {
            Texture2D buttonTexture = MenuBlock.PreloadedTextures[lastBlock];
            float buttonX = i * buttonSize + 150;
            float buttonY = TopRect.height / 2 - Convert.ToInt32(buttonTexture.height / 2);

            CreateButton(buttonTexture, new Vector2(buttonX, buttonY), lastBlock);
            i++;
        }
    }

    private static void CreateButton(Texture2D img, Vector2 pos, int index)
    {
        QuickButtons.Add(new Button(img, pos,
            new Color(130, 130, 130, 70), new Color(130, 130, 130, 100),
            new Color(160, 160, 160, 150), () => BlockSpawn.ChangeIndex(index)));
    }

    private static void Render()
    {
        Raylib.DrawRectangleRec(TopRect, new Color(20, 20, 20, 200));
        foreach (var button in QuickButtons)
        {
            button.Render();
        }
        _menuButton?.Render();
        _tpToPlayer?.Render();
    }
}

internal static class MiniMenuTop
{
    public static readonly Rectangle Rect = new Rectangle(0, MenuTop.TopRect.height, 236, 258);
    public static bool Enabled;
    private static Button? _loadButton;
    private static Button? _saveButton;
    private static Button? _gameDataMenuButton;
    private static Button? _leaveButton;
    public static void Start()
    {
        Program.UpdateScripts += Update;
        Program.RenderScripts += Render;
        _loadButton = UseFull.CreateButton("Zaladuj(L)", Color.GOLD, 48, new Vector2(0,
            Rect.y), SaveLoadSystem.LoadGame);
        _saveButton = UseFull.CreateButton("Zapis(J)", Color.GOLD, 48, new Vector2(0,
            Rect.y + 64), SaveLoadSystem.SaveGame);
        _gameDataMenuButton = UseFull.CreateButton("Mapa(T)", Color.GOLD, 48, new Vector2(0,
            Rect.y + 128), () => MenuGameData.MenuRectEnabled = !MenuGameData.MenuRectEnabled); 
        _leaveButton = UseFull.CreateButton("Wyjc", Color.GOLD, 48, new Vector2(0,
            Rect.y + 192), () => Program.Running = false);
    }
    
    private static void Update()
    {
        if(!Enabled) return;
        _loadButton?.Collision();
        _saveButton?.Collision();
        _gameDataMenuButton?.Collision();
        _leaveButton?.Collision();
    }

    private static void Render()
    {
        if(!Enabled) return;
        Raylib.DrawRectangleRec(Rect, Color.DARKBROWN);
        _loadButton?.Render();
        _saveButton?.Render();      
        _gameDataMenuButton?.Render();       
        _leaveButton?.Render();      
    }
}