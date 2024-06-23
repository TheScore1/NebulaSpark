using System.Globalization;
using SFML.System;
using SFML.Graphics;
using Application;
using SFML.Window;
using static System.Formats.Asn1.AsnWriter;

namespace SFML
{
    public class Palette
    {
        private List<PaletteItem> savedColors;
        private const int MaxColors = 20;
        private static readonly Vector2f DefaultPosition = new Vector2f(3f, 3f);
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

        public RectangleShape AddBtnHitbox { get; private set; }
        private Texture AddBtnTexture;
        private Sprite AddBtnSprite;

        public Palette()
        {
            savedColors = new List<PaletteItem>();
            itemShapeSize = DefaultShapeSize;
        }

        public void LoadContent()
        {
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

        public void DeleteColor(int index)
        {
            if (index < 0 || index >= savedColors.Count)
            {
#if DEBUG
                Console.WriteLine($"Некорректный индекс для удаления цвета: index = {index}");
#endif
                return;
            }
#if DEBUG
            Console.WriteLine($"Удалён цвет с позиции {index}");
#endif
            savedColors.RemoveAt(index);
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
            foreach (var item in savedColors)
            {
                if (IsMouseOverShape(e, item.Shape))
                {
                    if (isMain)
                    {
                        ManagerUI.ActiveMainColor = item.Color;
#if DEBUG
                        Console.WriteLine($"Задан основной цвет: {item.Color}");
#endif
                    }
                    else
                    {
                        ManagerUI.ActiveSecondColor = item.Color;
#if DEBUG
                        Console.WriteLine($"Задан побочный цвет: {item.Color}");
#endif
                    }
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

        public void AddColor(Color color)
        {
            if (savedColors.Count >= MaxColors)
            {
                Console.WriteLine("Превышен лимит цветов в палитре");
                return;
            }

            savedColors.Add(new PaletteItem(color, ItemShapeSize, color.A < 255));
        }

        public byte[] TransformClipboard()
        {
            try
            {
                string text = Clipboard.Contents;
#if DEBUG
                Console.WriteLine(text);
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
                Console.WriteLine("Буфер обмена не корректный: ");
                Console.WriteLine(ex.Message);
#endif
                return new byte[1];
            }
        }

        public void Draw(GameLoop gameLoop, RectangleShape palettePanel)
        {
            int sizeOfRow = Math.Max(1, (int)Math.Floor((palettePanel.Size.X - DefaultPosition.X) / (itemShapeSize.X + 3)));
            int row;

            for (int i = 0; i < savedColors.Count; i++)
            {
                int column = i % sizeOfRow;
                row = i / sizeOfRow;

                float xPosition = DefaultPosition.X + (itemShapeSize.X + 3) * column;
                float yPosition = DefaultPosition.Y + (itemShapeSize.X + 3) * row;

                savedColors[i].Shape.Position = new Vector2f(xPosition, yPosition);
                savedColors[i].Draw(gameLoop);
            }

            int addBtnColumn = savedColors.Count % sizeOfRow;
            int addBtnRow = savedColors.Count / sizeOfRow;
            AddBtnSprite.Position = new Vector2f(
                DefaultPosition.X + (itemShapeSize.X + 3) * addBtnColumn,
                DefaultPosition.Y + (itemShapeSize.X + 3) * addBtnRow);
            AddBtnSprite.Scale = itemShapeSize / 32;
            AddBtnHitbox.Position = AddBtnSprite.Position;
            gameLoop.Window.Draw(AddBtnSprite);
        }
    }

    public class PaletteItem
    {
        public Color Color { get; }
        public bool IsAlpha { get; }
        public RectangleShape Shape { get; }

        public PaletteItem(Color color, Vector2f shapeSize, bool isAlpha = false)
        {
            IsAlpha = isAlpha;
            Color = color;
            Shape = new RectangleShape(shapeSize)
            {
                OutlineColor = Color.Black,
                OutlineThickness = -3
            };
            var backColorEven = new Color(51, 18, 73);
            var backColorOdd = new Color(217, 217, 220);
            if (isAlpha && Color.A == 0)
            {
                Image img = new Image(4, 4); 
                for (int i = 0; i < img.Size.X; i++)
                    for (int j = 0; j < img.Size.Y; j++)
                    {
                        if (j % 2 == 0)
                        {
                            if (i % 2 == 0)
                                img.SetPixel((uint)i, (uint)j, backColorEven);
                            else
                                img.SetPixel((uint)i, (uint)j, backColorOdd);
                        }
                        else
                        {
                            if (i % 2 == 0)
                                img.SetPixel((uint)i, (uint)j, backColorOdd);
                            else
                                img.SetPixel((uint)i, (uint)j, backColorEven);
                        }
                    }
                Shape.Texture = new Texture(img);
            }
            else if (isAlpha == true)
                Shape.FillColor = ColorWithAlpha(color);
            else
                Shape.FillColor = Color;
        }

        public static Color ColorWithAlpha(Color color)
        {
            byte alpha = color.A;
            if (alpha == 0)
            {
                return Color.Transparent;
            }
            else
            {
                byte red = (byte)((color.R * alpha + 255 * (255 - alpha)) / 255);
                byte green = (byte)((color.G * alpha + 255 * (255 - alpha)) / 255);
                byte blue = (byte)((color.B * alpha + 255 * (255 - alpha)) / 255);
                return new Color(red, green, blue); // 255 * (255 - alpha) / 255 это вклад белого фона по умолчанию
            }
        }

        public void Draw(GameLoop gameLoop)
        {
            gameLoop.Window.Draw(Shape);
        }
    }
}
