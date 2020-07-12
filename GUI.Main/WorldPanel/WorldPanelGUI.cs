using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rendering.Core.RenderGUI;

namespace GUI.Main.WorldPanel
{
    public partial class WorldPanelGUI : UserControl
    {
        public WorldPanelGUI()
        {
            InitializeComponent();

            var renderControl = new RenderGUI();
            renderControl.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(renderControl);
        }
    }
}
