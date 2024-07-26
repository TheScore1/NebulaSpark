using SFML.System;
using SFML.Graphics;
using Application;
using System.Threading.Channels;

namespace SFML
{
    public class ColorWheel
    {
        private RectangleShape DemoWindow;

        List<ChannelPanel> channels;

        private ChannelPanel RChannel;
        private ChannelPanel GChannel;
        private ChannelPanel BChannel;
        private ChannelPanel AChannel;

        public ColorWheel()
        {
            DemoWindow = new RectangleShape();

            channels = new List<ChannelPanel>();

            RChannel = new ChannelPanel();
            GChannel = new ChannelPanel();
            BChannel = new ChannelPanel();
            AChannel = new ChannelPanel();

            channels.Add(RChannel);
            channels.Add(GChannel);
            channels.Add(BChannel);
            channels.Add(AChannel);
        }

        public void Update()
        {
            DemoWindow.FillColor = ManagerUI.MainColor;

            RChannel.Value = ManagerUI.MainColor.R;
            GChannel.Value = ManagerUI.MainColor.G;
            BChannel.Value = ManagerUI.MainColor.B;
            AChannel.Value = ManagerUI.MainColor.A;
        }

        public void Draw(GameLoop gameLoop)
        {
            gameLoop.Window.Draw(DemoWindow);

            foreach (var channel in channels)
                channel.Draw(gameLoop);
        }
    }

    public class ChannelPanel
    {
        public int Value;
        public RectangleShape Shape;
        public RectangleShape ValueShape;

        public ChannelPanel(int value = 0)
        {
            Value = value;
            Shape = new RectangleShape();
            ValueShape = new RectangleShape();
        }

        public void Draw(GameLoop gameLoop)
        {
            gameLoop.Window.Draw(Shape);
            gameLoop.Window.Draw(ValueShape);
        }
    }
}
