using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GUI.Layers.MainWindow;

namespace GUI.Main.SidePanel
{
    public partial class SidePanelGUI : UserControl
    {
        private readonly LayersGUI layersGUI;

        public SidePanelGUI()
        {
            InitializeComponent();

            layersGUI = new LayersGUI();
            layersGUI.Dock = DockStyle.Fill;
            splitContainer.Panel2.Controls.Add(layersGUI);
        }
    }
}
