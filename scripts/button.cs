using Raylib_cs;
using System.Numerics;
namespace Labirynt_3_Edytor.scripts;
public class Button
{
    private readonly string? _text;
    private readonly Color _fontColor;
    private readonly int _fontSize;
    private readonly Color _defaultColor;
    private readonly Color _buttonHoveringColor;
    private readonly Color _buttonPressedColor;
    private readonly Action[] _onClick;
    private readonly Rectangle _rect;
    private readonly Texture2D _texture;
    private readonly bool _isImage;
    private ButtonState _buttonState;

    public Button(string whatToSay, Color fontColor, Vector2 pos, int fontSize, Color defaultColor, Color buttonHoveringColor, Color buttonPressedColor, params Action[] onClick)
    {
        _text = whatToSay;
        _fontColor = fontColor;
        _onClick = onClick;
        _fontSize = fontSize;
        _defaultColor = defaultColor;
        _buttonHoveringColor = buttonHoveringColor;
        _buttonPressedColor = buttonPressedColor;
        var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), whatToSay, fontSize, 0);
        float textSizeWidth = Raylib.MeasureText(whatToSay, fontSize);
        _rect = new Rectangle(pos.X, pos.Y, textSizeWidth, textSize.Y);
   
        _buttonState = ButtonState.None;
    }
    public Button(Texture2D texture, Vector2 pos, Color defaultColor, Color buttonHoveringColor, Color buttonPressedColor, params Action[] onClick)
    {
        _text = null;
        _texture = texture;
        _onClick = onClick;
        _defaultColor = defaultColor;
        _buttonHoveringColor = buttonHoveringColor;
        _buttonPressedColor = buttonPressedColor;
        _rect = new Rectangle(pos.X, pos.Y, texture.width, texture.height);
        _isImage = true;
        _buttonState = ButtonState.None;
    }
    
    public void Collision()
    {
        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _rect) && Raylib.GetMousePosition() != new Vector2(0,0))
        {
            if (_buttonState == ButtonState.None) Raylib.PlaySound(Raylib.LoadSound(Program.GameSounds.ButtonHighlightPath));
            _buttonState = ButtonState.Hovering;

            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON)) _buttonState = ButtonState.Pressed;

            if (!Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON)) return;
            Raylib.PlaySound(Raylib.LoadSound(Program.GameSounds.ButtonPressPath));
            foreach (var onClickFunc in _onClick)
            {
                onClickFunc.Invoke();
            }
            return;
        }
        _buttonState = ButtonState.None;
    }

    public void Render()
    {
        Color currentColor = _buttonState switch
        {
            ButtonState.Pressed => _buttonPressedColor,
            ButtonState.Hovering => _buttonHoveringColor,
            _ => _defaultColor
        };
        if (_isImage)
        {
            Raylib.DrawTexture(_texture, (int)_rect.x, (int)_rect.y, Color.WHITE);
            Raylib.DrawRectangleRec(_rect, currentColor);
            return;
        }
        
        Raylib.DrawText(_text, (int)_rect.x, (int)_rect.y, _fontSize, _fontColor);
        Raylib.DrawRectangleRec(_rect, currentColor);
    }
    private enum ButtonState
    {
        None,
        Hovering,
        Pressed
    }
}

public class TextBox
{
    public int NumberPressed;
    private readonly Rectangle _rect;
    private readonly bool _canType;
    private readonly int _fontSize;
    private readonly int _maxLength;
    private readonly bool _minusNumber;
    private readonly Color _fontColor;
    private readonly Color _boxColor;
    public TextBox(Rectangle rect, int startingNumber, int fontsize, int maxlength, Color fontcolor, Color boxColor, bool minusNumber, bool canType)
    {
        _rect = rect;
        NumberPressed = startingNumber;
        _minusNumber = minusNumber;
        _canType = canType;
        _fontSize = fontsize;
        _maxLength = maxlength;
        _fontSize = fontsize;
        _fontColor = fontcolor;
        _boxColor = boxColor;
    }
    public void Update()
    {
        // update the textBox check for input
        IsNumberKeyPressed(ref NumberPressed);
    }
    private void IsNumberKeyPressed(ref int number)
    {
        if (!_canType) return;
        if (!Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _rect)) return;
        var textBoxNumber = number.ToString();
        var length = textBoxNumber.Contains("-") ? 1 : 0;
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
        {
            textBoxNumber = textBoxNumber.Length <= length + 1 ? "0" : textBoxNumber.Substring(0, textBoxNumber.Length - 1);
            number = Convert.ToInt32(textBoxNumber);
        }
        
        if (_minusNumber && Raylib.IsKeyPressed(KeyboardKey.KEY_MINUS))
        {
            number = -Convert.ToInt32(textBoxNumber);
        }
        if(textBoxNumber.Length >= _maxLength + length) return;
        for (int i = 0; i <= 9; i++)
        {
            if (!Raylib.IsKeyPressed(KeyboardKey.KEY_ZERO + i)) continue;
            textBoxNumber += i.ToString();
            number = Convert.ToInt32(textBoxNumber);
        }
    }
    public void Render()
    {
        Raylib.DrawRectangleRec(_rect, _boxColor);
        var textWidth = Raylib.MeasureText(NumberPressed.ToString(), _fontSize);    
        var textPos = new Vector2(MathF.Round(_rect.x + _rect.width / 2 - Convert.ToInt32(textWidth / 2)),
            _rect.y + _rect.height / 2 - Convert.ToInt32(_fontSize / 2));
        Raylib.DrawText(NumberPressed.ToString(), (int)textPos.X, (int)textPos.Y, _fontSize, _fontColor);
    }
}