using System.Windows.Forms;
using GUI.Layers.MainWindow;

namespace GUI.Main.SidePanel
{
    public partial class SidePanelGUI : UserControl
    {
        public SidePanelGUI()
        {
            InitializeComponent();

            LayersGUI layersGui = new LayersGUI();
            layersGui.Dock = DockStyle.Fill;
            splitContainer.Panel2.Controls.Add(layersGui);
        }
    }
}
