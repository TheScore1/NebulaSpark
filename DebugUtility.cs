using System;
using SFML.System;
using SFML.Graphics;
using Application;
using OpenTK.Graphics.OpenGL;
using System;

namespace SFML
{
    public static class DebugUtility
    {
        public const string CONSOLE_FONT_PATH = "arialmt.ttf";

        public static SFML.Graphics.Font ConsoleFont;

        public static void LoadContent()
        {
            ConsoleFont = new SFML.Graphics.Font(CONSOLE_FONT_PATH);
        }

        public static void DrawPerformanceData(GameLoop gameLoop, SFML.Graphics.Color fontColor)
        {
            if (ConsoleFont == null)
                return;

            string totalTimeElapsedStr = gameLoop.GameTime.TotalTimeElapsed.ToString("0.000");
            string deltaTimeStr = gameLoop.GameTime.DeltaTime.ToString("0.00000");
            float fps = 1f / gameLoop.GameTime.DeltaTime;
            string fpsStr = fps.ToString("0.00");

            Text totalTimeStat = new Text("Total time elapsed: " + totalTimeElapsedStr, ConsoleFont, 14);
            totalTimeStat.Position = new Vector2f(4f, 8f);
            totalTimeStat.FillColor = fontColor;

            Text deltaTimeStat = new Text("Delta time: " + deltaTimeStr, ConsoleFont, 14);
            deltaTimeStat.Position = new Vector2f(4f, 28f);
            deltaTimeStat.FillColor = fontColor;

            Text fpsStat = new Text("FPS: " + fpsStr, ConsoleFont, 14);
            fpsStat.Position = new Vector2f(4f, 48f);
            fpsStat.FillColor = fontColor;

            gameLoop.Window.Draw(totalTimeStat);
            gameLoop.Window.Draw(deltaTimeStat);
            gameLoop.Window.Draw(fpsStat);
        }

        public static void Log(string str)
        {
            string filePath = "logs.txt";

            // возможно добавить вывод в файл
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now}] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(str);
            Console.ResetColor();

            if (!File.Exists(filePath))
            {
                using (var stream = File.CreateText(filePath))
                {
                    stream.Close();
                }
            }

            File.AppendAllText(filePath, $"[{DateTime.Now}] {str + "\n"}");
        }

        public static void LogCanvas()
        {
            throw new NotImplementedException();
        }

        public static void LogStart()
        {
            throw new NotImplementedException();
        }
        
        public static void LogClose()
        {
            throw new NotImplementedException();
        }
    }
}
