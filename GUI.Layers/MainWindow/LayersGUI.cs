using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Configuration;

namespace GUI.Layers.MainWindow
{
    public partial class LayersGUI : UserControl
    {
        public LayersGUI()
        {
            InitializeComponent();
            InitializeTreeview();
        }

        private void InitializeTreeview()
        {
            tvFilter.BeginUpdate();
            tvFilter.Nodes.Add("Earth Texture");
            tvFilter.Nodes[0].Checked = LayerConfiguration.ShowEarthTexture;

            tvFilter.Nodes.Add("Clouds");
            tvFilter.Nodes.Add("Grid");
            tvFilter.Nodes[2].Nodes.Add("Lines");
            tvFilter.Nodes[2].Nodes.Add("Color coding");
            tvFilter.EndUpdate();
        }

        private void tvFilter_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }

        private void CheckTreeViewNode(TreeNode node, bool isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;
                if (item.Nodes.Count > 0)
                    CheckTreeViewNode(item, isChecked);
            }

            LayerConfiguration.ShowEarthTexture = tvFilter.Nodes[0].Checked;
        }
    }
}
