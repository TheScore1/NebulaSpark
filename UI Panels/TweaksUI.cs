using SFML.Window;
using SFML.Graphics;

namespace SFML
{
    public static class TweaksUI
    {
        public static Color BarsColors = Color.White;
        public static Color OutlineColor = Color.Black;
        public static int OutlineThickness = -6;

        // курсоры это задел на будущее для интерфейса
        private static Cursor cursorArrow = new Cursor(Cursor.CursorType.Arrow);
        private static Cursor cursorHand = new Cursor(Cursor.CursorType.Hand);
        private static Cursor cursorSizeHorizontal = new Cursor(Cursor.CursorType.SizeHorizontal);
    }
}
