using SFML.System;
using SFML.Graphics;
using Application;

namespace SFML
{
    public class ColorWheel
    {
        private RectangleShape background;

        public ColorWheel()
        {
            background = new RectangleShape();
        }

        public void Resize(Vector2f size)
        {
            background.Size = size;
        }

        public void Draw(GameLoop gameLoop)
        {
            background.FillColor = new SFML.Graphics.Color(255, 255, 255);
            gameLoop.Window.Draw(background);
        }
    }
}
