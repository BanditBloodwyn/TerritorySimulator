using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GUI.Main.SidePanel;
using GUI.Main.WorldPanel;

namespace GUI.Main.MainWindow
{
    public partial class MainWindow : UserControl
    {
        private readonly SidePanelGUI sidePanelGUI;
        private readonly WorldPanelGUI worldPanelGUI;

        public MainWindow()
        {
            InitializeComponent();

            sidePanelGUI = new SidePanelGUI();
            sidePanelGUI.Dock = DockStyle.Fill;
            splitContainer.Panel1.Controls.Add(sidePanelGUI);

            worldPanelGUI = new WorldPanelGUI();
            worldPanelGUI.Dock = DockStyle.Fill;
            splitContainer.Panel2.Controls.Add(worldPanelGUI);
        }
    }
}
