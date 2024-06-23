using Application;
using SFML.Graphics;
using SFML.System;
using System;

namespace SFML
{
    public class Canvas
    {
        public RectangleShape backgroundLayer;
        public RectangleShape DrawnLayer;
        private Image background;
        private Image DrawnLayerImage;
        private Color backColorEven;
        private Color backColorOdd;

        public static int OffsetBounds;

        public int[,] PixelsArray;

        public Vector2f Size;

        public Canvas(uint width, uint height)
        {
            Size = new Vector2f(width, height);

            OffsetBounds = 10;

            backColorEven = new Color(51, 18, 73);
            backColorOdd = new Color(217, 217, 220);
            background = new Image(width, height);
            for (uint i = 0; i < background.Size.X; i++)
            {
                for (uint j = 0; j < background.Size.Y; j++)
                {
                    if (j % 2 == 0)
                    {
                        if (i % 2 == 0)
                            background.SetPixel(i, j, backColorEven);
                        else
                            background.SetPixel(i, j, backColorOdd);
                    }
                    else
                    {
                        if (i % 2 == 0)
                            background.SetPixel(i, j, backColorOdd);
                        else
                            background.SetPixel(i, j, backColorEven);
                    }
                }
            }
            DrawnLayerImage = new Image(width, height, Color.Transparent);
            backgroundLayer = new RectangleShape(new Vector2f(width, height));
            backgroundLayer.Texture = new Texture(background);
            DrawnLayer = new RectangleShape(new Vector2f(width, height));
            DrawnLayer.Texture = new Texture(DrawnLayerImage);
        }

        public void SetPixelColor(uint x, uint y, Color color)
        {
            if (x < Size.X && y < Size.Y)
            {
#if DEBUG
                Console.WriteLine($"[{x},{y}] Пиксель установлен");
#endif
                DrawnLayerImage.SetPixel(x, y, color);
                DrawnLayer.Texture.Update(DrawnLayerImage);
            }
        }

        public void ExportImage()
        {
            Texture texture = DrawnLayer.Texture;
            Image exportImage = texture.CopyToImage();
            bool success = exportImage.SaveToFile("Autosave.png");
#if DEBUG
            if (success)
                Console.WriteLine($"Изображение успешно экспортировано в: {"Autosave.png"}");
            else
                Console.WriteLine("Ошибка при экспорте изображения.");
#endif
        }

        public void LoadImage()
        {
            if (File.Exists("Autosave.png"))
            {
                DrawnLayerImage = new Image("Autosave.png");
                DrawnLayer.Texture.Update(DrawnLayerImage);
#if DEBUG
                Console.WriteLine($"Изображение успешно загружено из: {"Autosave.png"}");
#endif
            }
#if DEBUG
            else
                Console.WriteLine("Файл изображения не найден.");
#endif
        }

        public void Draw(GameLoop gameLoop, RectangleShape canvasShape)
        {
            var pos = new Vector2f(
                canvasShape.Position.X + OffsetBounds + CanvasCam.OffsetPosX,
                canvasShape.Position.Y + OffsetBounds + CanvasCam.OffsetPosY);
            var size = new Vector2f(
                canvasShape.Size.X - 2 * OffsetBounds,
                canvasShape.Size.Y - 2 * OffsetBounds);
            backgroundLayer.Scale = new Vector2f(
                Math.Min(size.X, size.Y) / backgroundLayer.Size.Y * CanvasCam.OffsetScaleFactor,
                Math.Min(size.X, size.Y) / backgroundLayer.Size.Y * CanvasCam.OffsetScaleFactor);
            backgroundLayer.Position = new Vector2f(
                pos.X - backgroundLayer.Size.X * backgroundLayer.Scale.X / 2 + (size.X / 2),
                pos.Y - backgroundLayer.Size.Y * backgroundLayer.Scale.Y / 2 + (size.Y / 2));
            DrawnLayer.Scale = backgroundLayer.Scale;
            DrawnLayer.Position = backgroundLayer.Position;
            gameLoop.Window.Draw(backgroundLayer);
            gameLoop.Window.Draw(DrawnLayer);
        }
    }
}
