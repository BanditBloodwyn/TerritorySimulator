using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rendering.Core.RenderGUI
{
    public partial class RenderGUI : UserControl
    {
        public RenderGUI()
        {
            BackColor = Color.DarkBlue;
            
            InitializeComponent();
            InitializeRendering();

            DisposeRendering();
        }

        private void InitializeRendering()
        {
            CreateRenderControl();
            DefineSwapChainDescription();
        }

        private void CreateRenderControl()
        {

        }

        private void DefineSwapChainDescription()
        {

        }

        private void DisposeRendering()
        {

        }

        private void RenderLoopFunction()
        {

        }
    }
}
