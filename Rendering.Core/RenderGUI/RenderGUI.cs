using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;

namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        private RenderControl renderControl;
        private RenderTargetView renderView;
        private DeviceContext context;
        private SwapChain swapChain;    // referring to the MSDN and Wikipedia, a virtual Framebuffer used by the graphics card (where the render results are going to be stored in)
        private SwapChainDescription swapChainDesc;
        private Device device;
        private Texture2D backBuffer;

        public RenderGUI()
        {
            BackColor = System.Drawing.Color.Black;
            
            InitializeComponent();
            InitializeRendering();

            //RenderLoop.Run(renderControl, () => RenderLoopFunction());

            DisposeRendering();
        }

        private void InitializeRendering()
        {
            CreateRenderControl();
            DefineSwapChainDescription();

            // create the device with the defined SwapChain description
            Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.None,
                swapChainDesc,
                out device,
                out swapChain);

            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);

            context = device.ImmediateContext;
            context.Rasterizer.SetViewport(new Viewport(0, 0, renderControl.Width, renderControl.Height));
            context.OutputMerger.SetRenderTargets(renderView);
        }

        private void CreateRenderControl()
        {
            renderControl = new RenderControl();
            renderControl.Dock = DockStyle.Fill;

            Controls.Add(renderControl);
        }

        private void DefineSwapChainDescription()
        {
            swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(renderControl.Width, renderControl.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = false,
                OutputHandle = renderControl.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
        }

        private void DisposeRendering()
        {
            renderView.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            context.Dispose();
            swapChain.Dispose();
        }

        private void RenderLoopFunction()
        {
            context.ClearRenderTargetView(renderView, SharpDX.Color.CornflowerBlue);

            // Render anything here

            // Present the rendering on screen
            swapChain.Present(0, 0);
        }
    }
}
