using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Application;
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFMLApp.User32;

namespace SFMLApp
{
    static class Program
    {
        const string MainWndClassName = "Main window";
        const string ContentWndClassName = "Content window (child)";
        const int MinContentWidth = 300;
        const int MinContentHeight = 300;
        const int MaxContentWidth = 2560;
        const int MaxContentHeight = 1440;

        static volatile IntPtr g_hMainWnd = IntPtr.Zero;
        static volatile IntPtr g_hContentWnd = IntPtr.Zero;
        static volatile int g_ContentWndWidth = (MaxContentWidth + MinContentWidth) / 2;
        static volatile int g_ContentWndHeight = (MaxContentHeight + MinContentHeight) / 2;

        static App appInstance;

        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            if (!CreateMainWindow())
                return;

            Thread contentWndThread = new Thread(ContentThreadMain);
            contentWndThread.Priority = ThreadPriority.AboveNormal;
            contentWndThread.Start();

            System.Windows.Forms.Application.Run();
            DestroyWindow(g_hMainWnd);
        }

        static bool CreateMainWindow()
        {
            Form mainForm = new Form
            {
                Text = MainWndClassName,
                FormBorderStyle = FormBorderStyle.Sizable,
                StartPosition = FormStartPosition.CenterScreen,
                MinimumSize = new System.Drawing.Size(MinContentWidth, MinContentHeight),
                MaximumSize = new System.Drawing.Size(MaxContentWidth, MaxContentHeight),
                Size = new System.Drawing.Size(500, 500)
            };

            mainForm.Load += (sender, e) =>
            {
                g_hMainWnd = mainForm.Handle;
                CreateContentWindow();
            };

            mainForm.Resize += (sender, e) =>
            {
                UpdateContentWindowSize();
            };

            mainForm.FormClosed += (sender, e) => System.Windows.Forms.Application.Exit();
            mainForm.Show();

            return true;
        }

        static void CreateContentWindow()
        {
            Form contentForm = new Form
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Black
            };

            var mainForm = Form.FromHandle(g_hMainWnd) as Form;
            mainForm.Controls.Add(contentForm);
            contentForm.Show();
            g_hContentWnd = contentForm.Handle;
        }

        static void UpdateContentWindowSize()
        {
            if (g_hContentWnd != IntPtr.Zero)
            {
                User32.GetClientRect(g_hMainWnd, out RECT rect);
                User32.MoveWindow(g_hContentWnd, 0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top, true);
            }
        }

        static void ContentThreadMain()
        {
            appInstance = new App(g_hContentWnd);
            appInstance.Run();
        }

        static void DestroyWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                appInstance?.OnWindowClosed(null, EventArgs.Empty);
                User32.DestroyWindow(hWnd);
            }
        }
    }

    static class User32
    {
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);

        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
    }
}