using System.Globalization;
using SFML.System;
using SFML.Graphics;
using Application;
using SFML.Window;
using static System.Formats.Asn1.AsnWriter;
using static System.Windows.Forms.AxHost;
using System;

namespace SFML
{
    public class Palette
    {
        RectangleShape palettePanel;

        private List<PaletteItem> savedColors;
        private const int MaxColors = 20;
        public Vector2f DefaultPosition = new Vector2f(3f, 3f);
        public Vector2f CurrentPosition;
        public static readonly Vector2f DefaultShapeSize = new Vector2f(48f, 48f);

        private Vector2f itemShapeSize;
        public Vector2f ItemShapeSize
        {
            get => itemShapeSize;
            set
            {
                if (value.X <= DefaultShapeSize.X || value.X != value.Y)
                {
                    Console.WriteLine("Попытка изменить размер элементов палитры на некорректный");
                    itemShapeSize = DefaultShapeSize;
                    return;
                }
                itemShapeSize = value;
            }
        }

        public int SizeOfRow;
        public int SizeOfColumn;

        private Texture ActiveMainItemTexture;
        private RectangleShape ActiveMainItemOverlay;
        private Texture ActiveSecondItemTexture;
        private RectangleShape ActiveSecondItemOverlay;

        public RectangleShape AddBtnHitbox { get; private set; }
        private Texture AddBtnTexture;
        private Sprite AddBtnSprite;

        public Palette(RectangleShape PalettePanel)
        {
            palettePanel = PalettePanel;
            savedColors = new List<PaletteItem>();
            CurrentPosition = DefaultPosition;
            itemShapeSize = DefaultShapeSize;
        }

        public void LoadContent()
        {
            ActiveMainItemTexture = new Texture("sprites/active_main.png");
            ActiveMainItemOverlay = new RectangleShape(itemShapeSize)
            {
                Texture = ActiveMainItemTexture,
                Position = DefaultPosition
            };
            ActiveSecondItemTexture = new Texture("sprites/active_second.png");
            ActiveSecondItemOverlay = new RectangleShape(itemShapeSize)
            {
                Texture = ActiveSecondItemTexture,
                Position = DefaultPosition
            };

            AddBtnTexture = new Texture("sprites/add_item.png");
            AddBtnSprite = new Sprite(AddBtnTexture)
            {
                Scale = itemShapeSize / 32
            };
            AddBtnHitbox = new RectangleShape(itemShapeSize)
            {
                Position = DefaultPosition
            };
        }

        public void ApplyActiveStyle(PaletteItem item)
        {
            item.Shape.OutlineColor = TweaksUI.PaletteItemOutlineActiveColor;
            item.Shape.OutlineThickness = TweaksUI.PaletteItemOutlineThicknessActiveColor;
        }

        public void ResetStyle(PaletteItem item)
        {
            item.Shape.OutlineColor = TweaksUI.PaletteItemOutlineColor;
            item.Shape.OutlineThickness = TweaksUI.PaletteItemOutlineThickness;
        }

        public void UpdateColorItemStyles()
        {
            foreach (var color in savedColors)
                ResetStyle(color);
            if (ManagerUI.IdMainColor > -1)
                ApplyActiveStyle(savedColors[ManagerUI.IdMainColor]);
            if (ManagerUI.IdSecondColor > -1)
                ApplyActiveStyle(savedColors[ManagerUI.IdSecondColor]);
        }

        public void DeleteColor(int index)
        {
            if (index < 0 || index >= savedColors.Count)
            {
#if DEBUG
                DebugUtility.Log($"Некорректный индекс для удаления цвета: [{index}]");
#endif
                return;
            }
            // если удаляется активный в палитре элемент, убираем привязку к ней
            if (index == ManagerUI.IdMainColor)
                ManagerUI.IdMainColor = -1;
            if (index == ManagerUI.IdSecondColor)
                ManagerUI.IdSecondColor = -1;

            // если удаляется любой элемент до id, id активных цветов должен уменьшится, съехать влево
            // учитывая место удалённого
            if (index < ManagerUI.IdMainColor)
                ManagerUI.IdMainColor -= 1;
            if (index < ManagerUI.IdSecondColor)
                ManagerUI.IdSecondColor -= 1;

            var deletedColor = savedColors[index].Color;
            savedColors.RemoveAt(index);
#if DEBUG
            DebugUtility.Log($"Удалён цвет: [{index}] {deletedColor}");
#endif
        }

        public void CheckForDeleteItems(MouseButtonEventArgs e)
        {
            for (int i = 0; i < savedColors.Count; i++)
            {
                if (IsMouseOverShape(e, savedColors[i].Shape))
                {
                    DeleteColor(i);
                    return;
                }
            }
        }

        public void CheckForColor(MouseButtonEventArgs e, bool isMain)
        {
            for (int i = 0;i < savedColors.Count; i++)
            {
                if (IsMouseOverShape(e, savedColors[i].Shape))
                {
                    if (isMain)
                    {
                        ManagerUI.MainColor = savedColors[i].Color;
                        ManagerUI.IdMainColor = i;
                        ActiveMainItemOverlay.Position = savedColors[i].Shape.Position;
#if DEBUG
                        DebugUtility.Log($"Задан основной цвет с [id]: [{i}] {savedColors[i].Color}");
#endif
                    }
                    else
                    {
                        ManagerUI.SecondColor = savedColors[i].Color;
                        ManagerUI.IdSecondColor = i;
                        ActiveSecondItemOverlay.Position = savedColors[i].Shape.Position;
#if DEBUG
                        DebugUtility.Log($"Задан побочный цвет с [id]: [{i}] {savedColors[i].Color}");
#endif
                    }
                    UpdateColorItemStyles(); // чтобы корректно были отображены активные цвета, если они есть в палитре
                    break;
                }
            }
        }

        private bool IsMouseOverShape(MouseButtonEventArgs e, RectangleShape shape)
        {
            var pos = shape.Position;
            var size = shape.Size;
            return e.X >= pos.X && e.X <= pos.X + size.X && e.Y >= pos.Y && e.Y <= pos.Y + size.Y;
        }

        public void AddColor(SFML.Graphics.Color color)
        {
            if (savedColors.Count >= MaxColors)
            {
#if DEBUG
                DebugUtility.Log("Попытка привысить лимит цветов в палитре");
#endif
                return;
            }

            savedColors.Add(new PaletteItem(color, ItemShapeSize, color.A < 255));
        }

        public byte[] TransformClipboard()
        {
            try
            {
                string text = SFML.Window.Clipboard.Contents;
#if DEBUG
                DebugUtility.Log($"Импорт буфера обмена: {text}");
#endif
                string[] textArr = text.Split("(")[1].Split(")")[0].Trim().Split(",");
                byte[] byteArr = new byte[textArr.Length];
                for (int i = 0; i < textArr.Length; i++)
                {
                    if (byteArr.Length == 4 && i == textArr.Length - 1)
                        if (float.TryParse(textArr[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float alphaFloat))
                            byteArr[i] = (byte)(alphaFloat * 255);
                        else
                            byteArr[i] = 255;
                    else
                        byteArr[i] = Convert.ToByte(textArr[i]);
                }    
                return byteArr;
            }
            catch (Exception ex)
            {
#if DEBUG
                DebugUtility.Log("Буфер обмена не корректный: ");
                DebugUtility.Log(ex.Message);
#endif
                return new byte[1];
            }
        }

        public void ChangeItemShapeSize(Vector2f size)
        {
            itemShapeSize = size;
            for (int i = 0; i < savedColors.Count; i++)
                savedColors[i].Shape.Size = size;
            AddBtnHitbox.Size = size;
            ActiveMainItemOverlay.Size = size;
            ActiveSecondItemOverlay.Size = size;
        }

        public void ChangeItemPositions(Vector2f pos)
        {
            float paletteVisibleHeight = palettePanel.Position.Y + palettePanel.Size.Y;
            var lenght = SizeOfColumn * (itemShapeSize.Y + 3) + DefaultPosition.Y;
            if (pos.Y < DefaultPosition.Y && pos.Y > paletteVisibleHeight - lenght)
                CurrentPosition = pos;
            if (pos.Y < -lenght + paletteVisibleHeight)
                CurrentPosition = new Vector2f(DefaultPosition.X, paletteVisibleHeight - lenght);
            if (lenght <= paletteVisibleHeight)
                CurrentPosition = DefaultPosition;
        }

        public void GetPixelColorAsMain(GameLoop gameLoop, int x, int y)
        {
            var window = gameLoop.Window;

            SFML.Graphics.Texture screenTexture = new Texture(window.Size.X, window.Size.Y);
            screenTexture.Update(window);
            SFML.Graphics.Image screeshot = screenTexture.CopyToImage();
            var tempColor = screeshot.GetPixel((uint)x, (uint)y);
            ManagerUI.MainColor = tempColor;
            if (ManagerUI.IdMainColor > -1)
            {
                savedColors[ManagerUI.IdMainColor].Color = tempColor;
                savedColors[ManagerUI.IdMainColor].Shape.FillColor = tempColor;
            }
            UpdateColorItemStyles();
        }

        public void Update()
        {
            SizeOfRow = Math.Max(1, (int)Math.Floor((palettePanel.Size.X - CurrentPosition.X) / (itemShapeSize.X + 3)));
            if (savedColors.Count != MaxColors)
                SizeOfColumn = Math.Max(1, (int)Math.Ceiling((double)(savedColors.Count + 1) / SizeOfRow));
            else
                SizeOfColumn = Math.Max(1, (int)Math.Ceiling((double)(savedColors.Count) / SizeOfRow));
            ChangeItemPositions(CurrentPosition);
        }

        public void Draw(GameLoop gameLoop)
        {
            int row;

            for (int i = 0; i < savedColors.Count; i++)
            {
                int column = i % SizeOfRow;
                row = i / SizeOfRow;

                float xPosition = CurrentPosition.X + (itemShapeSize.X + 3) * column;
                float yPosition = CurrentPosition.Y + (itemShapeSize.X + 3) * row;

                savedColors[i].Shape.Position = new Vector2f(xPosition, yPosition);
                savedColors[i].Draw(gameLoop);
            }

            if (ManagerUI.IdMainColor > -1)
            {
                ActiveMainItemOverlay.Position = savedColors[ManagerUI.IdMainColor].Shape.Position;
                gameLoop.Window.Draw(ActiveMainItemOverlay);
            }
            if (ManagerUI.IdSecondColor > -1)
            {
                ActiveSecondItemOverlay.Position = savedColors[ManagerUI.IdSecondColor].Shape.Position;
                gameLoop.Window.Draw(ActiveSecondItemOverlay);
            }

            if (savedColors.Count != MaxColors)
            {
                int addBtnColumn = savedColors.Count % SizeOfRow;
                int addBtnRow = savedColors.Count / SizeOfRow;
                AddBtnSprite.Position = new Vector2f(
                    CurrentPosition.X + (itemShapeSize.X + 3) * addBtnColumn,
                    CurrentPosition.Y + (itemShapeSize.X + 3) * addBtnRow);
                AddBtnSprite.Scale = itemShapeSize / 32;
                AddBtnHitbox.Position = AddBtnSprite.Position;

                gameLoop.Window.Draw(AddBtnSprite);
            }
        }
    }

    public class PaletteItem
    {
        public SFML.Graphics.Color Color { get; set; }
        public bool IsAlpha { get; }
        public RectangleShape Shape { get; }

        public PaletteItem(SFML.Graphics.Color color, Vector2f shapeSize, bool isAlpha = false)
        {
            IsAlpha = isAlpha;
            Color = color;
            Shape = new RectangleShape(shapeSize)
            {
                OutlineColor = SFML.Graphics.Color.Black,
                OutlineThickness = TweaksUI.PaletteItemOutlineThickness
            };

            // Возможно изменить всё отображение на отображение прозрачной текстуры с вкладом цвета,
            // если прозрачную текстуру сделать ярче и более однотонной
            if (isAlpha && Color.A == 0)
                Shape.Texture = CreateTransparentTexture();
            else if (isAlpha)
                Shape.FillColor = ColorWithAlpha(color);
            else
                Shape.FillColor = Color;
        }

        private Texture CreateTransparentTexture()
        {
            var backColorEven = new SFML.Graphics.Color(51, 18, 73);
            var backColorOdd = new SFML.Graphics.Color(217, 217, 220);
            SFML.Graphics.Image img = new SFML.Graphics.Image(4, 4);
            for (int i = 0; i < img.Size.X; i++)
                for (int j = 0; j < img.Size.Y; j++)
                {
                    if (j % 2 == 0)
                        img.SetPixel((uint)i, (uint)j, i % 2 == 0 ? backColorEven : backColorOdd);
                    else
                        img.SetPixel((uint)i, (uint)j, i % 2 == 0 ? backColorOdd : backColorEven);
                }
            return new Texture(img);
        }

        public static SFML.Graphics.Color ColorWithAlpha(SFML.Graphics.Color color)
        {
            byte alpha = color.A;
            if (alpha == 0)
                return SFML.Graphics.Color.Transparent;
            else
            {
                byte red = (byte)((color.R * alpha + 255 * (255 - alpha)) / 255);
                byte green = (byte)((color.G * alpha + 255 * (255 - alpha)) / 255);
                byte blue = (byte)((color.B * alpha + 255 * (255 - alpha)) / 255);
                return new SFML.Graphics.Color(red, green, blue); // 255 * (255 - alpha) / 255 вклад белого фона по умолчанию
            }
        }

        public void Draw(GameLoop gameLoop)
        {
            gameLoop.Window.Draw(Shape);
        }
    }
}
