using System;
using SFML.System;
using SFML.Graphics;
using Application;

namespace SFML
{
    public static class DebugUtility
    {
        public const string CONSOLE_FONT_PATH = "arialmt.ttf";

        public static Font consoleFont;

        public static void LoadContent()
        {
            consoleFont = new Font(CONSOLE_FONT_PATH);
        }

        public static void DrawPerformanceData(GameLoop gameLoop, Color fontColor)
        {
            if (consoleFont == null)
                return;

            string totalTimeElapsedStr = gameLoop.GameTime.TotalTimeElapsed.ToString("0.000");
            string deltaTimeStr = gameLoop.GameTime.DeltaTime.ToString("0.00000");
            float fps = 1f / gameLoop.GameTime.DeltaTime;
            string fpsStr = fps.ToString("0.00");

            Text totalTimeStat = new Text("Total time elapsed: " + totalTimeElapsedStr, consoleFont, 14);
            totalTimeStat.Position = new Vector2f(4f, 8f);
            totalTimeStat.Color = fontColor;

            Text deltaTimeStat = new Text("Delta time: " + deltaTimeStr, consoleFont, 14);
            deltaTimeStat.Position = new Vector2f(4f, 28f);
            deltaTimeStat.Color = fontColor;

            Text fpsStat = new Text("FPS: " + fpsStr, consoleFont, 14);
            fpsStat.Position = new Vector2f(4f, 48f);
            fpsStat.Color = fontColor;

            gameLoop.Window.Draw(totalTimeStat);
            gameLoop.Window.Draw(deltaTimeStat);
            gameLoop.Window.Draw(fpsStat);
        }
    }
}
