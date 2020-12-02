using System;
using System.Windows.Forms;
using GUI.Main.MainWindow;

namespace TerritorySimulator
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Dock = DockStyle.Fill;

            MainForm mainForm = new MainForm();
            mainForm.Text += " - v" + Application.ProductVersion;
            mainForm.Controls.Add(mainWindow);

            Application.Run(mainForm);
        }
    }
}
