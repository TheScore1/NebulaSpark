using System;
using System.Collections.Generic;
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
#if DEBUG
        private Vector2u oldWindowSize;
#endif

        private static Dictionary<Keyboard.Key, bool> keyStates = new();

        private ManagerUI managerUI;

        private bool isHoldingLeftButton;
        private bool isHoldingRightButton;
        private bool isDraggingVerticalBar;
        private bool isDraggingHorizontalBar;

        public App(IntPtr hWnd) : base(hWnd, new SFML.Graphics.Color(128, 128, 128)) { }

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
#if DEBUG
            oldWindowSize = Window.Size;
#endif
            InitializeKeyStates();
            InitializeRandomColor();
            managerUI.canvas.LoadImage();
            InitializeEventHandlers();
        }

        private void InitializeKeyStates()
        {
            keyStates[Keyboard.Key.W] = false;
            keyStates[Keyboard.Key.S] = false;
            keyStates[Keyboard.Key.A] = false;
            keyStates[Keyboard.Key.D] = false;
        }

        private void InitializeRandomColor()
        {
            managerUI.palette.AddColor(SFML.Graphics.Color.Transparent);
            for (int i = 0; i < 10; i++)
            {
                var rand = new Random();
                managerUI.palette.AddColor(new SFML.Graphics.Color((byte)rand.NextInt64(0, 255), (byte)rand.NextInt64(0, 255), (byte)rand.NextInt64(0, 255)));
            }
        }

        private void InitializeEventHandlers()
        {
            Window.Resized += OnWindowResized;
            Window.MouseButtonPressed += OnMouseBtnPressed;
            Window.MouseButtonReleased += OnMouseBtnReleased;
            Window.MouseMoved += OnMouseMoved;
            Window.MouseWheelScrolled += OnMouseWheelScrolled;
            Window.KeyPressed += OnKeyPressed;
            Window.KeyReleased += OnKeyReleased;
        }

        private void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if (keyStates.ContainsKey(e.Code))
                keyStates[e.Code] = true;
        }
        
        private void OnKeyReleased(object sender, SFML.Window.KeyEventArgs e)
        {
            if (keyStates.ContainsKey(e.Code))
                keyStates[e.Code] = false;
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
                isHoldingLeftButton = true;
                HandleLeftMouseBtnPressed(e);
            }

            if (e.Button == Mouse.Button.Right)
            {
                isHoldingRightButton = true;
                HandleRightMouseBtnPressed(e);
            }

            if (e.Button == Mouse.Button.Middle)
                managerUI.palette.CheckForDeleteItems(e);
        }

        private void HandleLeftMouseBtnPressed(MouseButtonEventArgs e)
        {
            if (IsMouseOverVerticalBorder(e))
                isDraggingVerticalBar = true;

            if (IsMouseOverPaletteAddButton(e))
                AddColorFromClipboard();

            if (IsMouseOverPalette(e))
                managerUI.palette.CheckForColor(e, true);

            if (IsMouseOverCanvas(e.X, e.Y))
                PaintPixel(e.X, e.Y, true);
        }

        private bool IsMouseOverVerticalBorder(MouseButtonEventArgs e)
        {
            Vector2f pos = managerUI.vBorder.Position;
            Vector2f size = managerUI.vBorder.Size;
            return e.X >= pos.X && e.X <= pos.X + size.X;
        }

        private bool IsMouseOverPaletteAddButton(MouseButtonEventArgs e)
        {
            Vector2f BtnPos = managerUI.palette.AddBtnHitbox.Position;
            Vector2f BtnSize = managerUI.palette.AddBtnHitbox.Size;
            return e.X >= BtnPos.X && e.X <= BtnPos.X + BtnSize.X && e.Y >= BtnPos.Y && e.Y <= BtnPos.Y + BtnSize.Y && e.Y < managerUI.hBorderLastPos;
        }

        private void AddColorFromClipboard()
        {
            byte[] args = managerUI.palette.TransformClipboard();
            if (args.Length == 4)
                managerUI.palette.AddColor(new SFML.Graphics.Color(args[0], args[1], args[2], args[3]));
            if (args.Length == 3)
                managerUI.palette.AddColor(new SFML.Graphics.Color(args[0], args[1], args[2]));
        }

        private bool IsMouseOverPalette(MouseButtonEventArgs e)
        {
            return e.Y < managerUI.hBorderLastPos && e.X < managerUI.vBorderLastPos;
        }
        
        private bool IsMouseOverCanvas(int x, int y)
        {
            return x > managerUI.vBorderLastPos + ManagerUI.BorderSize;
        }

        private void HandleRightMouseBtnPressed(MouseButtonEventArgs e)
        {
            if (IsMouseOverCanvas(e.X, e.Y))
                PaintPixel(e.X, e.Y, false);
            if (IsMouseOverPalette(e))
                managerUI.palette.CheckForColor(e, false);
        }

        private void OnMouseBtnReleased(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left)
            {
                isHoldingLeftButton = false;
                isDraggingVerticalBar = false;
                isDraggingHorizontalBar = false;
            }
            if (e.Button == Mouse.Button.Right)
                isHoldingRightButton = false;
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (isHoldingLeftButton && isDraggingVerticalBar)
                if (e.X > 10 && e.X < Window.Size.X - 10)
                    managerUI.RecalcVBorderLastPos(e.X);
            if (isHoldingLeftButton && IsMouseOverCanvas(e.X, e.Y))
                PaintPixel(e.X, e.Y, true);
            if (isHoldingRightButton && IsMouseOverCanvas(e.X, e.Y))
                PaintPixel(e.X, e.Y, false);
        }

        private void OnWindowResized(object sender, SizeEventArgs e)
        {
            Window.SetView(new SFML.Graphics.View(new FloatRect(0, 0, e.Width, e.Height)));
#if DEBUG
            DebugUtility.Log($"Размер окна изменён: {oldWindowSize.X}, {oldWindowSize.Y} => {e.Width}, {e.Height}");
            oldWindowSize = Window.Size;
#endif
        }

        public void PaintPixel(int mouseX, int mouseY, bool isMainColor)
        {
            var cPos = managerUI.canvas.BackgroundLayer.Position;
            var cSize = new Vector2f(managerUI.canvas.BackgroundLayer.Size.X * managerUI.canvas.BackgroundLayer.Scale.X, managerUI.canvas.BackgroundLayer.Size.Y * managerUI.canvas.BackgroundLayer.Scale.Y);
            var cScale = managerUI.canvas.BackgroundLayer.Scale;

            if (mouseX >= cPos.X && mouseX <= cPos.X + cSize.X && mouseY >= cPos.Y && mouseY <= cPos.Y + cSize.Y)
            {
                // Рассчитываем координаты мыши относительно начала канваса
                float localX = (mouseX - cPos.X) / cScale.X;
                float localY = (mouseY - cPos.Y) / cScale.Y;

                // Преобразуем координаты в координаты пикселя
                uint pixelX = (uint)Math.Clamp(localX, 0, managerUI.canvas.Size.X - 1);
                uint pixelY = (uint)Math.Clamp(localY, 0, managerUI.canvas.Size.Y - 1);

                managerUI.canvas.SetPixelColor(pixelX, pixelY, isMainColor ? ManagerUI.ActiveMainColor : ManagerUI.ActiveSecondColor);
            }
        }

        static void UpdateCamera()
        {
            float speedMultiplier = (CanvasCam.OffsetScaleFactor + CanvasCam.OffsetScaleFactor) / 2.0f;
            float baseSpeed = 3.0f;
            float finalSpeed = baseSpeed * speedMultiplier;

            if (keyStates[Keyboard.Key.W])
                CanvasCam.OffsetPosY += finalSpeed;
            if (keyStates[Keyboard.Key.S])
                CanvasCam.OffsetPosY -= finalSpeed;
            if (keyStates[Keyboard.Key.A])
                CanvasCam.OffsetPosX += finalSpeed;
            if (keyStates[Keyboard.Key.D])
                CanvasCam.OffsetPosX -= finalSpeed;
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
            DebugUtility.DrawPerformanceData(this, SFML.Graphics.Color.White);
#endif
        }

        public void OnWindowClosed(object sender, EventArgs e)
        {
            managerUI.canvas.ExportImage();
            Window.Close();
        }
    }
}
