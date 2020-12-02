using System.Windows.Forms;
using GUI.Main.SidePanel;
using GUI.Main.WorldPanel;

namespace GUI.Main.MainWindow
{
    public partial class MainWindow : UserControl
    {
        public MainWindow()
        {
            InitializeComponent();

            SidePanelGUI sidePanelGui = new SidePanelGUI();
            sidePanelGui.Dock = DockStyle.Fill;
            splitContainer.Panel1.Controls.Add(sidePanelGui);

            WorldPanelGUI worldPanelGui = new WorldPanelGUI();
            worldPanelGui.Dock = DockStyle.Fill;
            splitContainer.Panel2.Controls.Add(worldPanelGui);
        }
    }
}
