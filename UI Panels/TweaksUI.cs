using SFML.Window;
using SFML.Graphics;

namespace SFML
{
    public static class TweaksUI
    {
        public static SFML.Graphics.Color BarsColors = SFML.Graphics.Color.White;
        public static SFML.Graphics.Color OutlineColor = SFML.Graphics.Color.Black;
        public static int OutlineThickness = -6;

        // курсоры это задел на будущее для интерфейса
        private static SFML.Window.Cursor cursorArrow = new SFML.Window.Cursor(SFML.Window.Cursor.CursorType.Arrow);
        private static SFML.Window.Cursor cursorHand = new SFML.Window.Cursor(SFML.Window.Cursor.CursorType.Hand);
        private static SFML.Window.Cursor cursorSizeHorizontal = new SFML.Window.Cursor(SFML.Window.Cursor.CursorType.SizeHorizontal);
    }
}
