using System;
using System.Collections.Generic;
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

//public void PaintPixel(int mouseX, int mouseY)
//{
//    var cPanelPos = managerUI.canvasPanel.Position;
//    var cPanelSize = managerUI.canvasPanel.Size;

//    if (mouseX >= cPanelPos.X && mouseX <= cPanelPos.X + cPanelSize.X && mouseY >= cPanelPos.Y && mouseY <= cPanelPos.Y + cPanelSize.Y)
//    {
//        // Рассчитываем масштабирование канваса относительно панели
//        float scaleX = (float)managerUI.canvas.Width / cPanelSize.X;
//        float scaleY = (float)managerUI.canvas.Height / cPanelSize.Y;

//        // Учитываем смещение камеры
//        var canvasOffsetX = (cPanelSize.X - managerUI.canvas.Width * CanvasCam.OffsetScaleFactor) / 2 + CanvasCam.OffsetPosX;
//        var canvasOffsetY = (cPanelSize.Y - managerUI.canvas.Height * CanvasCam.OffsetScaleFactor) / 2 + CanvasCam.OffsetPosY;

//        // Пересчитываем координаты мыши относительно канваса
//        float offsetX = (mouseX - cPanelPos.X - canvasOffsetX) / CanvasCam.OffsetScaleFactor;
//        float offsetY = (mouseY - cPanelPos.Y - canvasOffsetY) / CanvasCam.OffsetScaleFactor;

//        // Конвертируем в координаты пикселя
//        uint pixelX = (uint)Math.Clamp(offsetX * scaleX, 0, managerUI.canvas.Width - 1);
//        uint pixelY = (uint)Math.Clamp(offsetY * scaleY, 0, managerUI.canvas.Height - 1);

//        // Выводим координаты пикселя для отладки
//        Console.WriteLine($"{pixelX}, {pixelY}");

//        // Устанавливаем цвет пикселя на канвасе
//        managerUI.canvas.SetPixelColor(pixelX, pixelY, ManagerUI.ActiveColor);
//    }
//}

class Border
{
    static RenderWindow window;
    static RectangleShape leftPanel;
    static RectangleShape rightPanel;
    static RectangleShape border;
    static List<RectangleShape> cubes;
    static bool isDragging = false;
    static float borderWidth = 10; // Ширина границы
    static float borderX;
    static float minCubeSize = 50; // Минимальный размер кубика
    static float maxCubeSize = 100; // Максимальный размер кубика

    static void DAsdASDASD(string[] args)
    {
        window = new RenderWindow(new VideoMode(800, 600), "SFML.Net Split Window");
        window.Closed += (sender, e) => window.Close();
        window.MouseButtonPressed += OnMouseButtonPressed;
        window.MouseButtonReleased += OnMouseButtonReleased;
        window.MouseMoved += OnMouseMoved;

        leftPanel = new RectangleShape(new Vector2f(400, 600)) { FillColor = Color.Transparent };
        rightPanel = new RectangleShape(new Vector2f(390, 600)) { FillColor = Color.Black };
        border = new RectangleShape(new Vector2f(borderWidth, 600)) { FillColor = Color.Blue };
        borderX = leftPanel.Size.X;

        cubes = new List<RectangleShape>();
        for (int i = 0; i < 4; i++)
        {
            var cube = new RectangleShape(new Vector2f(maxCubeSize, maxCubeSize))
            {
                FillColor = Color.Green
            };
            cubes.Add(cube);
        }

        UpdatePanelSizes(); // Инициализируем размеры панелей и границы

        while (window.IsOpen)
        {
            window.DispatchEvents();
            Update();
            Render();
        }
    }

    static void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left && e.X >= borderX && e.X <= borderX + borderWidth)
        {
            isDragging = true;
        }
    }

    static void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
        {
            isDragging = false;
        }
    }

    static void OnMouseMoved(object sender, MouseMoveEventArgs e)
    {
        if (isDragging)
        {
            // Ограничим перемещение границы
            if (e.X > 10 && e.X < window.Size.X - 10)
            {
                borderX = e.X;
                UpdatePanelSizes();
            }
        }
    }

    static void UpdatePanelSizes()
    {
        leftPanel.Size = new Vector2f(borderX, window.Size.Y);
        rightPanel.Size = new Vector2f(window.Size.X - borderX - borderWidth, window.Size.Y);
        rightPanel.Position = new Vector2f(borderX + borderWidth, 0);
        border.Position = new Vector2f(borderX, 0);

        // Обновляем размеры и положение кубиков
        float cubeSize = maxCubeSize;
        int columns = 2; // Начнем с двух столбцов
        if (borderX / 2 < minCubeSize + 10) // Учитываем минимальный размер и отступ
        {
            columns = 1; // Переходим на один столбец при достижении минимума
            cubeSize = minCubeSize;
        }
        else
        {
            cubeSize = Math.Max(minCubeSize, Math.Min(maxCubeSize, borderX / columns - 10));
        }

        float xOffset = (borderX - (cubeSize * columns)) / (columns + 1);
        float yOffset = 10; // Отступ между строками

        for (int i = 0; i < cubes.Count; i++)
        {
            cubes[i].Size = new Vector2f(cubeSize, cubeSize);
            cubes[i].Position = new Vector2f((i % columns) * (cubeSize + xOffset) + xOffset, (i / columns) * (cubeSize + yOffset) + yOffset);
        }
    }

    static void Update()
    {
        // Update logic if needed
    }

    static void Render()
    {
        window.Clear();
        window.Draw(leftPanel);
        window.Draw(rightPanel);
        window.Draw(border);

        // Рисуем кубики
        foreach (var cube in cubes)
        {
            window.Draw(cube);
        }

        window.Display();
    }
}
