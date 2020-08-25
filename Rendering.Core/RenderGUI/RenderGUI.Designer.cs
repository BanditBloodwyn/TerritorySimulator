namespace Rendering.Core.RenderGUI
{
    partial class RenderGUI
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlGL = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlGL
            // 
            this.pnlGL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGL.Location = new System.Drawing.Point(0, 0);
            this.pnlGL.Name = "pnlGL";
            this.pnlGL.Size = new System.Drawing.Size(630, 466);
            this.pnlGL.TabIndex = 0;
            // 
            // RenderGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlGL);
            this.Name = "RenderGUI";
            this.Size = new System.Drawing.Size(630, 466);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlGL;
    }
}
