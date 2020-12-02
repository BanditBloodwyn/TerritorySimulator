using System.Windows.Forms;
using GUI.Main.WorldPanel.RenderGUI;

namespace GUI.Main.WorldPanel
{
    public partial class WorldPanelGUI : UserControl
    {
        public WorldPanelGUI()
        {
            InitializeComponent();

            RenderingGUI renderControl = new RenderingGUI();
            renderControl.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(renderControl);
        }
    }
}
