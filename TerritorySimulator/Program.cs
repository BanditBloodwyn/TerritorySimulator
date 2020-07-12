using System;
using System.Windows.Forms;
using GUI.Main.MainWindow;

namespace TerritorySimulator.Core.Start
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Dock = DockStyle.Fill;

            MainForm mainForm = new MainForm();
            mainForm.Controls.Add(mainWindow);

            Application.Run(mainForm);
        }
    }
}
