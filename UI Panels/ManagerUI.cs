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

        public static Color ActiveMainColor;
        public static Color ActiveSecondColor;

        RectangleShape palettePanel;
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

            ActiveMainColor = Color.Black;
            ActiveSecondColor = Color.White;

            canvas = new Canvas(20, 20);
            palette = new Palette();
            colorWheel = new ColorWheel();

            vBorderLastPos = gameLoop.Window.Size.X / 4 - BorderSize / 2;
            hBorderLastPos = gameLoop.Window.Size.Y / 2 - BorderSize / 2;

            vBorder = new RectangleShape(new Vector2f(BorderSize, gameLoop.Window.Size.Y));
            vBorder.Position = new Vector2f(vBorderLastPos, 0f);
            vBorder.FillColor = Color.Magenta;
#if RELEASE
            vBorder.FillColor = new Color(0, 120, 215);
#endif

            hBorder = new RectangleShape(new Vector2f(vBorderLastPos, BorderSize));
            hBorder.Position = new Vector2f(0f, hBorderLastPos);
#if RELEASE
            hBorder.FillColor = new Color(0, 120, 215);
#endif

            palettePanel = new RectangleShape(new Vector2f(vBorderLastPos - BorderSize / 2, gameLoop.Window.Size.Y / 2 - BorderSize / 2));
            palettePanel.Position = new Vector2f(0f, 0f);
            palettePanel.FillColor = Color.Yellow;
#if RELEASE
            palettePanel.FillColor = new Color(124, 85, 88);
#endif

            colorWheelPanel = new RectangleShape(new Vector2f(vBorderLastPos, gameLoop.Window.Size.Y / 2 - BorderSize / 2));
            colorWheelPanel.Position = new Vector2f(0f, gameLoop.Window.Size.Y / 2 + BorderSize / 2);
            colorWheelPanel.FillColor = Color.Cyan;
#if RELEASE
            colorWheelPanel.FillColor = new Color(124, 85, 88);
#endif

            canvasPanel = new RectangleShape(new Vector2f(gameLoop.Window.Size.X - vBorderLastPos - BorderSize, gameLoop.Window.Size.Y));
            canvasPanel.Position = new Vector2f(vBorderLastPos + BorderSize, 0f);
            canvasPanel.FillColor = new Color(124, 85, 88);

            colorMainPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorMainPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 - Palette.DefaultShapeSize.X * 2);
            if (ActiveMainColor.A > 0)
                colorMainPanel.FillColor = ActiveMainColor;
            else
            {
                var backColorEven = new Color(51, 18, 73);
                var backColorOdd = new Color(217, 217, 220);
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
                colorMainPanel.Texture = new Texture(img);
            }
            colorMainPanel.OutlineColor = Color.Black;
            colorMainPanel.OutlineThickness = -6;

            colorSecondPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorSecondPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 + 6);
            if (ActiveSecondColor.A > 0)
                colorSecondPanel.FillColor = ActiveSecondColor;
            else
            {
                var backColorEven = new Color(51, 18, 73);
                var backColorOdd = new Color(217, 217, 220);
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
                colorSecondPanel.Texture = new Texture(img);
            }
            colorSecondPanel.OutlineColor = Color.Black;
            colorSecondPanel.OutlineThickness = -6;
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

        public void UpdatedSize(SizeEventArgs e)
        {
            vBorder = new RectangleShape(new Vector2f(BorderSize, e.Height));
            vBorder.Position = new Vector2f(vBorderLastPos, 0f);
            vBorder.FillColor = Color.Magenta;
#if RELEASE
            vBorder.FillColor = new Color(0, 120, 215);
#endif

            hBorder = new RectangleShape(new Vector2f(vBorderLastPos, BorderSize));
            hBorder.Position = new Vector2f(0f, hBorderLastPos);
            hBorder.FillColor = Color.Blue;
#if RELEASE
            hBorder.FillColor = new Color(0, 120, 215);
#endif

            palettePanel = new RectangleShape(new Vector2f(vBorderLastPos, e.Height / 2 - BorderSize / 2));
            palettePanel.Position = new Vector2f(0f, 0f);
            palettePanel.FillColor = Color.Yellow;
#if RELEASE
            palettePanel.FillColor = new Color(124, 85, 88);
#endif

            colorWheelPanel = new RectangleShape(new Vector2f(vBorderLastPos, e.Height / 2 - BorderSize / 2));
            colorWheelPanel.Position = new Vector2f(0f, e.Height / 2 + BorderSize / 2);
            colorWheelPanel.FillColor = Color.Cyan;
#if RELEASE
            colorWheelPanel.FillColor = new Color(124, 85, 88);
#endif

            canvasPanel = new RectangleShape(new Vector2f(e.Width - vBorderLastPos - BorderSize, e.Height));
            canvasPanel.Position = new Vector2f(vBorderLastPos + BorderSize, 0f);
            canvasPanel.FillColor = new Color(124, 85, 88);

            colorMainPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorMainPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 - Palette.DefaultShapeSize.X * 2);
            if (ActiveMainColor.A > 0)
                colorMainPanel.FillColor = ActiveMainColor;
            else
            {
                var backColorEven = new Color(51, 18, 73);
                var backColorOdd = new Color(217, 217, 220);
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
                colorMainPanel.Texture = new Texture(img);
            }
            colorMainPanel.OutlineColor = Color.Black;
            colorMainPanel.OutlineThickness = -6;

            colorSecondPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorSecondPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 + 6);
            if (ActiveSecondColor.A > 0)
                colorSecondPanel.FillColor = ActiveSecondColor;
            else
            {
                var backColorEven = new Color(51, 18, 73);
                var backColorOdd = new Color(217, 217, 220);
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
                colorSecondPanel.Texture = new Texture(img);
            }
            colorSecondPanel.OutlineColor = Color.Black;
            colorSecondPanel.OutlineThickness = -6;
        }

        public void Update()
        {
            vBorder = new RectangleShape(new Vector2f(BorderSize, gameLoop.Window.Size.Y));
            vBorder.Position = new Vector2f(vBorderLastPos, 0f);
            vBorder.FillColor = Color.Magenta;
#if RELEASE
            vBorder.FillColor = new Color(0, 120, 215);
#endif

            hBorder = new RectangleShape(new Vector2f(vBorderLastPos, BorderSize));
            hBorder.Position = new Vector2f(0f, gameLoop.Window.Size.Y / 2 - BorderSize / 2);
            hBorder.FillColor = Color.Blue;
#if RELEASE
            hBorder.FillColor = new Color(0, 120, 215);
#endif

            palettePanel = new RectangleShape(new Vector2f(vBorderLastPos, gameLoop.Window.Size.Y / 2 - BorderSize / 2));
            palettePanel.Position = new Vector2f(0f, 0f);
            palettePanel.FillColor = Color.Yellow;
#if RELEASE
            palettePanel.FillColor = new Color(124, 85, 88);
#endif

            colorWheelPanel = new RectangleShape(new Vector2f(vBorderLastPos, gameLoop.Window.Size.Y / 2 -     BorderSize / 2));
            colorWheelPanel.Position = new Vector2f(0f, gameLoop.Window.Size.Y / 2 + BorderSize / 2);
            colorWheelPanel.FillColor = Color.Cyan;
#if RELEASE
            colorWheelPanel.FillColor = new Color(124, 85, 88);
#endif

            canvasPanel = new RectangleShape(new Vector2f(gameLoop.Window.Size.X - vBorderLastPos - BorderSize, gameLoop.Window.Size.Y));
            canvasPanel.Position = new Vector2f(vBorderLastPos + BorderSize, 0f);
            canvasPanel.FillColor = new Color(124, 85, 88);

            colorMainPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorMainPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 - Palette.DefaultShapeSize.X * 2);
            if (ActiveMainColor.A > 0)
                colorMainPanel.FillColor = ActiveMainColor;
            else
            {
                var backColorEven = new Color(51, 18, 73);
                var backColorOdd = new Color(217, 217, 220);
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
                colorMainPanel.Texture = new Texture(img);
            }
            colorMainPanel.OutlineColor = Color.Black;
            colorMainPanel.OutlineThickness = -6;

            colorSecondPanel = new RectangleShape(Palette.DefaultShapeSize * 2);
            colorSecondPanel.Position = new Vector2f(canvasPanel.Position.X + 6, canvasPanel.Size.Y / 2 + 6);
            if (ActiveSecondColor.A > 0)
                colorSecondPanel.FillColor = ActiveSecondColor;
            else
            {
                var backColorEven = new Color(51, 18, 73);
                var backColorOdd = new Color(217, 217, 220);
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
                colorSecondPanel.Texture = new Texture(img);
            }
            colorSecondPanel.OutlineColor = Color.Black;
            colorSecondPanel.OutlineThickness = -6;
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
