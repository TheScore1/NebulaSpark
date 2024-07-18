using Application;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace SFML
{
    public class ManagerUI
    {
        GameLoop gameLoop;

        public static SFML.Graphics.Color ActiveMainColor = SFML.Graphics.Color.Black;
        public static SFML.Graphics.Color ActiveSecondColor = SFML.Graphics.Color.White;

        public RectangleShape palettePanel;
        public RectangleShape colorWheelPanel;
        public RectangleShape canvasPanel;
        RectangleShape colorMainPanel;
        RectangleShape colorSecondPanel;

        public static float BorderSize = 10;

        public RectangleShape vBorder;
        public float vBorderLastPos;
        public RectangleShape hBorder;
        public float hBorderLastPos;

        public Palette palette;
        public ColorWheel colorWheel;
        public Canvas canvas;

        public ManagerUI(GameLoop mainGameLoop)
        {
            gameLoop = mainGameLoop;

            canvas = new Canvas(20, 20);
            palette = new Palette();
            colorWheel = new ColorWheel();

            vBorderLastPos = gameLoop.Window.Size.X / 4 - BorderSize / 2;
            hBorderLastPos = gameLoop.Window.Size.Y / 2 - BorderSize / 2;

            vBorder = new RectangleShape(new Vector2f(BorderSize, gameLoop.Window.Size.Y))
            {
                Position = new Vector2f(vBorderLastPos, 0f),
                FillColor = new SFML.Graphics.Color(0, 120, 215)
            };
#if DEBUG
            vBorder.FillColor = SFML.Graphics.Color.Magenta;
#endif

            hBorder = new RectangleShape(new Vector2f(vBorderLastPos, BorderSize))
            {
                Position = new Vector2f(0f, hBorderLastPos),
                FillColor = new SFML.Graphics.Color(0, 120, 215)
            };
#if DEBUG
            hBorder.FillColor = SFML.Graphics.Color.Blue;
#endif

            palettePanel = new RectangleShape(new Vector2f(vBorderLastPos, gameLoop.Window.Size.Y / 2 - BorderSize / 2))
            {
                Position = new Vector2f(0f, 0f),
                FillColor = new SFML.Graphics.Color(124, 85, 88)
            };
#if DEBUG
            palettePanel.FillColor = SFML.Graphics.Color.Yellow;
#endif

            colorWheelPanel = new RectangleShape(new Vector2f(vBorderLastPos, gameLoop.Window.Size.Y / 2 - BorderSize / 2))
            {
                Position = new Vector2f(0f, gameLoop.Window.Size.Y / 2 + BorderSize / 2),
                FillColor = new SFML.Graphics.Color(124, 85, 88)
            };
#if DEBUG
            colorWheelPanel.FillColor = SFML.Graphics.Color.Cyan;
#endif

            canvasPanel = new RectangleShape(new Vector2f(gameLoop.Window.Size.X - vBorderLastPos - BorderSize, gameLoop.Window.Size.Y))
            {
                Position = new Vector2f(vBorderLastPos + BorderSize, 0f),
                FillColor = new SFML.Graphics.Color(124, 85, 88)
            };

            colorMainPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorMainPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 - Palette.DefaultShapeSize.X * 2);
            if (ActiveMainColor.A == 255)
                colorMainPanel.FillColor = ActiveMainColor;
            else if (ActiveMainColor.A > 0) // надо создавать с вкладом цвета на фоне прозрачной текстуры или добавлять значок, что прозрачный цвет, изменить все условия
                colorMainPanel.FillColor = PaletteItem.ColorWithAlpha(ActiveMainColor);
            else
                colorMainPanel.Texture = CreateTransparentTexture(4, 4);
            colorMainPanel.OutlineColor = TweaksUI.OutlineColor;
            colorMainPanel.OutlineThickness = TweaksUI.OutlineThickness;

            colorSecondPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorSecondPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 + 6);
            if (ActiveSecondColor.A == 255)
                colorSecondPanel.FillColor = ActiveSecondColor;
            else if (ActiveSecondColor.A > 0) // надо создавать с вкладом цвета на фоне прозрачной текстуры или добавлять значок, что прозрачный цвет, изменить все условия
                colorSecondPanel.FillColor = PaletteItem.ColorWithAlpha(ActiveSecondColor);
            else
                colorSecondPanel.OutlineColor = TweaksUI.OutlineColor;
            colorSecondPanel.OutlineThickness = TweaksUI.OutlineThickness;
        }

        public void LoadContent()
        {
            palette.LoadContent();
        }

        public void RecalcVBorderLastPos(float size)
        {
            vBorderLastPos = size - BorderSize / 2;
        }

        public void RecalcHBorderLastPos(float size)
        {
            hBorderLastPos = size  - BorderSize / 2;
        }

        public void Update()
        {
            vBorder.Size = new Vector2f(BorderSize, gameLoop.Window.Size.Y);
            vBorder.Position = new Vector2f(vBorderLastPos, 0f);

            hBorder.Size = new Vector2f(vBorderLastPos, BorderSize);
            hBorder.Position = new Vector2f(0f, gameLoop.Window.Size.Y / 2 - BorderSize / 2);

            // убрать когда реализую его движение
            hBorderLastPos = gameLoop.Window.Size.Y / 2 - BorderSize / 2;

            palettePanel.Size = new Vector2f(vBorderLastPos, gameLoop.Window.Size.Y / 2 - BorderSize / 2);
            palettePanel.Position = new Vector2f(0f, 0f);

            colorWheelPanel.Size = new Vector2f(vBorderLastPos, gameLoop.Window.Size.Y / 2 - BorderSize / 2);
            colorWheelPanel.Position = new Vector2f(0f, gameLoop.Window.Size.Y / 2 + BorderSize / 2);

            canvasPanel.Size = new Vector2f(gameLoop.Window.Size.X - vBorderLastPos - BorderSize, gameLoop.Window.Size.Y);
            canvasPanel.Position = new Vector2f(vBorderLastPos + BorderSize, 0f);

            colorMainPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorMainPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 - Palette.DefaultShapeSize.X * 2);
            if (ActiveMainColor.A == 255)
                colorMainPanel.FillColor = ActiveMainColor;
            else if (ActiveMainColor.A > 0) // надо создавать с вкладом цвета на фоне прозрачной текстуры или добавлять значок, что прозрачный цвет, изменить все условия
                colorMainPanel.FillColor = PaletteItem.ColorWithAlpha(ActiveMainColor);
            else
                colorMainPanel.Texture = CreateTransparentTexture(4, 4);
            colorMainPanel.OutlineColor = TweaksUI.OutlineColor;
            colorMainPanel.OutlineThickness = TweaksUI.OutlineThickness;

            colorSecondPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorSecondPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 + 6);
            if (ActiveSecondColor.A == 255)
                colorSecondPanel.FillColor = ActiveSecondColor;
            else if (ActiveSecondColor.A > 0) // надо создавать с вкладом цвета на фоне прозрачной текстуры или добавлять значок, что прозрачный цвет, изменить все условия
                colorSecondPanel.FillColor = PaletteItem.ColorWithAlpha(ActiveSecondColor);
            else
                colorSecondPanel.Texture = CreateTransparentTexture(4, 4);
            colorSecondPanel.OutlineColor = TweaksUI.OutlineColor;
            colorSecondPanel.OutlineThickness = TweaksUI.OutlineThickness;
        }

        private Texture CreateTransparentTexture(uint width, uint height)
        {
            var backColorEven = new SFML.Graphics.Color(51, 18, 73);
            var backColorOdd = new SFML.Graphics.Color(217, 217, 220);
            SFML.Graphics.Image img = new SFML.Graphics.Image(width, height);
            for (uint i = 0; i < img.Size.X; i++)
                for (uint j = 0; j < img.Size.Y; j++)
                {
                    if (j % 2 == 0)
                        img.SetPixel(i, j, i % 2 == 0 ? backColorEven : backColorOdd);
                    else
                        img.SetPixel(i, j, i % 2 == 0 ? backColorOdd : backColorEven);
                }
            return new Texture(img);
        }

        public void Draw()
        {
            gameLoop.Window.Draw(canvasPanel);
            canvas.Draw(gameLoop, canvasPanel);

            gameLoop.Window.Draw(palettePanel);
            palette.Draw(gameLoop, palettePanel);

            gameLoop.Window.Draw(colorWheelPanel);

            gameLoop.Window.Draw(colorMainPanel);
            gameLoop.Window.Draw(colorSecondPanel);

            gameLoop.Window.Draw(vBorder);
            gameLoop.Window.Draw(hBorder);
        }
    }
}
