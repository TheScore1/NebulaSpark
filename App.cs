using System;
using SFML.System;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using Application;

namespace SFML
{
    public class App : GameLoop
    {
        public const uint DEFAULT_WINDOW_WIDTH = 960;
        public const uint DEFAULT_WINDOW_HEIGHT = 480;

        public const string WINDOW_TITLE = "Application";

        static Dictionary<Keyboard.Key, bool> keyStates = new Dictionary<Keyboard.Key, bool>();

        ManagerUI managerUI;

        // курсоры это задел на будущее для интерфейса
        Cursor cursorArrow = new Cursor(Cursor.CursorType.Arrow);
        Cursor cursorHand = new Cursor(Cursor.CursorType.Hand);
        Cursor cursorSizeHorizontal = new Cursor(Cursor.CursorType.SizeHorizontal);

        bool IsDraggingLeft;
        bool IsDraggingRight;
        bool IsDraggingVerticalBar;

        public App() : base(DEFAULT_WINDOW_WIDTH, DEFAULT_WINDOW_HEIGHT, WINDOW_TITLE, new Color(128, 128, 128))
        {
        }

        public override void LoadContent()
        {
#if DEBUG
            DebugUtility.LoadContent();
#endif
            managerUI = new ManagerUI(this);
            managerUI.LoadContent();
        }

        public override void Initialize()
        {
            keyStates[Keyboard.Key.W] = false;
            keyStates[Keyboard.Key.S] = false;
            keyStates[Keyboard.Key.A] = false;
            keyStates[Keyboard.Key.D] = false;

            

            IsDraggingLeft = false;

            for (int i = 0; i < 10; i++)
            {
                var rand = new Random();
                managerUI.palette.AddColor(new Color((byte)rand.NextInt64(0, 255), (byte)rand.NextInt64(0, 255), (byte)rand.NextInt64(0, 255)));
            }

            managerUI.canvas.LoadImage();

            Window.Resized += OnWindowResized;
            Window.MouseButtonPressed += OnMouseBtnPressed;
            Window.MouseButtonReleased += OnMouseBtnReleased;
            Window.MouseMoved += OnMouseMoved;
            Window.MouseWheelScrolled += OnMouseWheelScrolled;
            Window.KeyPressed += OnKeyPressed;
            Window.KeyReleased += OnKeyReleased;
            Window.Closed += OnWindowClosed;
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (keyStates.ContainsKey(e.Code))
            {
                keyStates[e.Code] = true;
            }
        }
        
        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            if (keyStates.ContainsKey(e.Code))
            {
                keyStates[e.Code] = false;
            }
        }

        private void OnMouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            if (e.Wheel == Mouse.Wheel.VerticalWheel)
            {
                var cPanelSize = managerUI.canvasPanel.Size;
                var cPanelPos = managerUI.canvasPanel.Position;
                if (e.X >= cPanelPos.X && e.X <= cPanelSize.X + cPanelSize.X && e.Y >= cPanelPos.Y && e.Y <= cPanelPos.Y + cPanelSize.Y)
                {
                    if (e.Delta < 0)
                    {

                        if (CanvasCam.OffsetScaleFactor * 0.9 < 0.4)
                            CanvasCam.OffsetScaleFactor = 0.4f;
                        else
                            CanvasCam.OffsetScaleFactor = (float)(CanvasCam.OffsetScaleFactor * 0.9);
                    }
                    if (e.Delta > 0)
                    {
                        if (CanvasCam.OffsetScaleFactor * 1.1 > 2)
                            CanvasCam.OffsetScaleFactor = 2f;
                        else
                            CanvasCam.OffsetScaleFactor = (float)(CanvasCam.OffsetScaleFactor * 1.1);
                    }
                }
            }
        }

        private void OnMouseBtnPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                IsDraggingLeft = true;

                Vector2f bSize = managerUI.vBorder.Size;
                Vector2f bPos = managerUI.vBorder.Position;
                if (e.X >= bPos.X && e.X <= bPos.X + bSize.X)
                    IsDraggingVerticalBar = true;

                Vector2f BtnPos = managerUI.palette.AddBtnHitbox.Position;
                Vector2f BtnSize = managerUI.palette.AddBtnHitbox.Size;
                if (e.X >= BtnPos.X && e.X <= BtnPos.X + BtnSize.X && e.Y >= BtnPos.Y && e.Y <= BtnPos.Y + BtnSize.Y && e.Y < managerUI.hBorderLastPos)
                {
                    byte[] args = managerUI.palette.TransformClipboard();
                    if (args.Length == 4)
                        managerUI.palette.AddColor(new Color(args[0], args[1], args[2], args[3]));
                    if (args.Length == 3)
                        managerUI.palette.AddColor(new Color(args[0], args[1], args[2]));
                }

                if (e.Y < managerUI.hBorderLastPos)
                {
                    managerUI.palette.CheckForColor(e, true);
                    Update(this.GameTime);
                }

                if (e.X > managerUI.vBorderLastPos + ManagerUI.BorderSize)
                    PaintPixel(e.X, e.Y, true);
            }

            if (e.Button == Mouse.Button.Right)
            {
                IsDraggingRight = true;

                if (e.X > managerUI.vBorderLastPos + ManagerUI.BorderSize)
                    PaintPixel(e.X, e.Y, false);
                if (e.Y < managerUI.hBorderLastPos)
                {
                    managerUI.palette.CheckForColor(e, false);
                    Update(this.GameTime);
                }
            }

            if (e.Button == Mouse.Button.Middle)
                managerUI.palette.CheckForDeleteItems(e);
        }
        
        private void OnMouseBtnReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                IsDraggingLeft = false;
                IsDraggingVerticalBar = false;
            }
            if (e.Button == Mouse.Button.Right)
            {
                IsDraggingRight = false;
            }
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (IsDraggingLeft && IsDraggingVerticalBar)
                if (e.X > 10 && e.X < Window.Size.X - 10)
                {
                    managerUI.RecalcVBorderLastPos(e.X);
                    managerUI.Update();
                }
            if (IsDraggingLeft)
            {
                if (e.X > managerUI.vBorderLastPos + ManagerUI.BorderSize)
                    PaintPixel(e.X, e.Y, true);
            }
            if (IsDraggingRight)
            {
                if (e.X > managerUI.vBorderLastPos + ManagerUI.BorderSize)
                    PaintPixel(e.X, e.Y, false);
            }
        }

        private void OnWindowResized(object sender, SizeEventArgs e)
        {
#if DEBUG
            Console.WriteLine($"Размер окна изменён: {e.Width}, {e.Height}");
#endif
            Window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
            
            managerUI.UpdatedSize(e);
        }

        public void PaintPixel(int mouseX, int mouseY, bool isMainColor)
        {
            var cPos = managerUI.canvas.backgroundLayer.Position;
            var cSize = new Vector2f(managerUI.canvas.backgroundLayer.Size.X * managerUI.canvas.backgroundLayer.Scale.X, managerUI.canvas.backgroundLayer.Size.Y * managerUI.canvas.backgroundLayer.Scale.Y);
            var cScale = managerUI.canvas.backgroundLayer.Scale;

            if (mouseX >= cPos.X && mouseX <= cPos.X + cSize.X && mouseY >= cPos.Y && mouseY <= cPos.Y + cSize.Y)
            {
                // Рассчитываем координаты мыши относительно начала канваса
                float localX = (mouseX - cPos.X) / cScale.X;
                float localY = (mouseY - cPos.Y) / cScale.Y;

                // Преобразуем координаты в координаты пикселя
                uint pixelX = (uint)Math.Clamp(localX, 0, managerUI.canvas.Size.X - 1);
                uint pixelY = (uint)Math.Clamp(localY, 0, managerUI.canvas.Size.Y - 1);

                // Устанавливаем цвет пикселя
                if (isMainColor)
                    managerUI.canvas.SetPixelColor(pixelX, pixelY, ManagerUI.ActiveMainColor);
                if (!isMainColor)
                    managerUI.canvas.SetPixelColor(pixelX, pixelY, ManagerUI.ActiveSecondColor);
            }
        }




        static void UpdateCamera()
        {
            float speedMultiplier = (CanvasCam.OffsetScaleFactor + CanvasCam.OffsetScaleFactor) / 2.0f;
            float baseSpeed = 3.0f;
            float finalSpeed = baseSpeed * speedMultiplier;

            if (keyStates[Keyboard.Key.W])
            {
                CanvasCam.OffsetPosY += finalSpeed;
            }
            if (keyStates[Keyboard.Key.S])
            {
                CanvasCam.OffsetPosY -= finalSpeed;
            }
            if (keyStates[Keyboard.Key.A])
            {
                CanvasCam.OffsetPosX += finalSpeed;
            }
            if (keyStates[Keyboard.Key.D])
            {
                CanvasCam.OffsetPosX -= finalSpeed;
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateCamera();
            managerUI.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            managerUI.Draw();
#if DEBUG
            DebugUtility.DrawPerformanceData(this, Color.White);
#endif
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            managerUI.canvas.ExportImage();
            Window.Close();
        }
    }
}
