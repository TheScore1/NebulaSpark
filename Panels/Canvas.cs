using Application;
using SFML.Graphics;
using SFML.System;
using System;

namespace SFML
{
    public class Canvas
    {
        public RectangleShape BackgroundLayer { get; }
        public RectangleShape DrawnLayer { get; }
        private SFML.Graphics.Image background;
        private SFML.Graphics.Image drawnLayerImage;

        private readonly SFML.Graphics.Color backColorEven;
        private readonly SFML.Graphics.Color backColorOdd;

        public static int OffsetBounds { get; private set; }
        public Vector2f Size { get; }

        public Canvas(uint width, uint height)
        {
            backColorEven = new SFML.Graphics.Color(51, 18, 73);
            backColorOdd = new SFML.Graphics.Color(217, 217, 220);
            Size = new Vector2f(width, height);
            OffsetBounds = 10;

            background = CreateBackgroundImage(width, height);
            drawnLayerImage = new SFML.Graphics.Image(width, height, SFML.Graphics.Color.Transparent);

            BackgroundLayer = new RectangleShape(new Vector2f(width, height))
            {
                Texture = new Texture(background)
            };
            DrawnLayer = new RectangleShape(new Vector2f(width, height))
            {
                Texture = new Texture(drawnLayerImage)
            };
#if DEBUG
            DebugUtility.Log($"Создан холст размером [{width}, {height}] ({width * height} пикселей)");
#endif
        }

        private SFML.Graphics.Image CreateBackgroundImage(uint width, uint height)
        {
            var img = new SFML.Graphics.Image(width, height);
            for (uint i = 0; i < width; i++)
                for (uint j = 0; j < height; j++)
                    img.SetPixel(i, j, GetPixelColor(i, j));
            return img;
        }

        private SFML.Graphics.Color GetPixelColor(uint x, uint y)
        {
            if (y % 2 == 0)
                return x % 2 == 0 ? backColorEven : backColorOdd;
            else
                return x % 2 == 0 ? backColorOdd : backColorEven;
        }

        public void SetPixelColor(uint x, uint y, SFML.Graphics.Color color)
        {
            if (x < Size.X && y < Size.Y)
            {
#if DEBUG
                //DebugUtility.Log($"[{x},{y}] Пиксель установлен");
#endif
                drawnLayerImage.SetPixel(x, y, color);
                DrawnLayer.Texture.Update(drawnLayerImage);
            }
        }

        public void ExportImage(string filePath = "Autosave.png")
        {
            Texture texture = DrawnLayer.Texture;
            SFML.Graphics.Image exportImage = texture.CopyToImage();
            bool success = exportImage.SaveToFile(filePath);
#if DEBUG
            DebugUtility.Log(success
                ? $"Изображение успешно экспортировано в: {filePath}"
                : "Ошибка при экспорте изображения.");
#endif
        }

        public void LoadImage(string filePath = "Autosave.png")
        {
            if (File.Exists(filePath))
            {
                drawnLayerImage = new SFML.Graphics.Image(filePath);
                DrawnLayer.Texture.Update(drawnLayerImage);
#if DEBUG
                DebugUtility.Log($"Изображение успешно загружено из: {filePath}");
#endif
            }
#if DEBUG
            else
                DebugUtility.Log("Файл изображения не найден.");
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

            BackgroundLayer.Scale = CalculateScale(size);
            BackgroundLayer.Position = CalculatePosition(pos, size, BackgroundLayer);

            DrawnLayer.Scale = BackgroundLayer.Scale;
            DrawnLayer.Position = BackgroundLayer.Position;

            gameLoop.Window.Draw(BackgroundLayer);
            gameLoop.Window.Draw(DrawnLayer);
        }

        private Vector2f CalculateScale(Vector2f size)
        {
            float scaleFactor = Math.Min(size.X, size.Y) / BackgroundLayer.Size.Y * CanvasCam.OffsetScaleFactor;
            return new Vector2f(scaleFactor, scaleFactor);
        }

        private Vector2f CalculatePosition(Vector2f pos, Vector2f size, RectangleShape layer)
        {
            return new Vector2f(
                pos.X - layer.Size.X * layer.Scale.X / 2 + (size.X / 2),
                pos.Y - layer.Size.Y * layer.Scale.Y / 2 + (size.Y / 2));
        }
    }
}
