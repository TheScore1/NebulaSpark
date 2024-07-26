using SFML.Window;
using SFML.Graphics;

namespace SFML
{
    public static class TweaksUI
    {
        // Borders
        public static SFML.Graphics.Color BorderColor = SFML.Graphics.Color.White;
        public static int BorderMinSpace = 60;

        // Palette Items
        public static SFML.Graphics.Color PaletteItemOutlineColor = SFML.Graphics.Color.Black;
        public static SFML.Graphics.Color PaletteItemOutlineActiveColor = new SFML.Graphics.Color(99, 155, 255);

        public static int PaletteItemOutlineThickness = -3;
        public static int PaletteItemOutlineThicknessActiveColor = -5;

        public static int PaletteShapeItemMinSize = 20;
        public static int PaletteShapeItemMaxSize = 128;

        // Active Color Panels
        public static SFML.Graphics.Color OutlineColorActiveColorPanel = SFML.Graphics.Color.Black;
        public static int OutlineThicknessActiveColorPanels = -6;

        // Instruments

        // разрешить ли использовать пипетку по всему экрану и всему интерфейсу или только canvas
        // true = альфа канал с холста работать не будет! (хотя это можно и дополнить)
        public static bool EyedropperIsFullScreenActive = false;

        // курсоры это задел на будущее для интерфейса
        private static SFML.Window.Cursor cursorArrow = new SFML.Window.Cursor(SFML.Window.Cursor.CursorType.Arrow);
        private static SFML.Window.Cursor cursorHand = new SFML.Window.Cursor(SFML.Window.Cursor.CursorType.Hand);
        private static SFML.Window.Cursor cursorSizeHorizontal = new SFML.Window.Cursor(SFML.Window.Cursor.CursorType.SizeHorizontal);
    }
}
