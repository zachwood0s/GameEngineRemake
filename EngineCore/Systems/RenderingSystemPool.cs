using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ECS.Systems
{
    public class RenderingSystemPool: ThreadedSystemPool
    {
        private Form _renderForm;
        private GraphicsDevice _graphicsDevics;

        public RenderingSystemPool(string poolName, int fps):base(poolName, fps)
        {
            _renderForm = new Form();
            _renderForm.ClientSize = new Size(800, 600);
            _renderForm.MainMenuStrip = null;

            _renderForm.Show();
        }

        public override void Initialize()
        {
            PresentationParameters pp = new PresentationParameters();
            pp.DeviceWindowHandle = _renderForm.Handle;

            pp.BackBufferFormat = SurfaceFormat.Color;
            pp.BackBufferWidth = 1280;
            pp.BackBufferHeight = 1024;
            pp.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            pp.IsFullScreen = false;

            pp.MultiSampleCount = 16;

            pp.DepthStencilFormat = DepthFormat.Depth24Stencil8;

            _graphicsDevics = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
                                                      GraphicsProfile.HiDef,
                                                      pp);
            Application.Idle += new EventHandler(_ApplicationIdle);
            Application.Run(_renderForm);
            base.Initialize();
        }
        public override void Execute()
        {
            _ResetTiming();
        }

        private void _ApplicationIdle(object pSender, EventArgs pEventArgs)
        {
            while (!PeekMessage(out Message message, IntPtr.Zero, 0, 0, 0))
            {
                _ThreadUpdate();
            }
        }


        //Got this from a stackoverflow post explaining how to make your own custom monogame form thing
        //https://stackoverflow.com/questions/6361691/custom-xna-game-loop-in-windows
        [StructLayout(LayoutKind.Sequential)]
        private struct Message
        {
            public IntPtr hWnd;
            public int msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint
            messageFilterMin, uint messageFilterMax, uint flags);
    }
}
