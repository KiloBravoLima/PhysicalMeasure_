namespace LogMeasurement
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FillUnitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fillUnitsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fillFavoritUnitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearInternalErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showUnitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFavoritUnitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMeasurementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openInternalErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.clearFavoriteUnitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExitToolStripMenuItem,
            this.FillUnitsToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.AboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(968, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // FillUnitsToolStripMenuItem
            // 
            this.FillUnitsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fillUnitsToolStripMenuItem1,
            this.toolStripSeparator2,
            this.fillFavoritUnitsToolStripMenuItem,
            this.clearFavoriteUnitsToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearInternalErrorsToolStripMenuItem});
            this.FillUnitsToolStripMenuItem.Name = "FillUnitsToolStripMenuItem";
            this.FillUnitsToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.FillUnitsToolStripMenuItem.Text = "Fill Tables";
            // 
            // fillUnitsToolStripMenuItem1
            // 
            this.fillUnitsToolStripMenuItem1.Name = "fillUnitsToolStripMenuItem1";
            this.fillUnitsToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.fillUnitsToolStripMenuItem1.Text = "Fill Units";
            this.fillUnitsToolStripMenuItem1.Click += new System.EventHandler(this.FillUnitsToolStripMenuItem_Click);
            // 
            // fillFavoritUnitsToolStripMenuItem
            // 
            this.fillFavoritUnitsToolStripMenuItem.Name = "fillFavoritUnitsToolStripMenuItem";
            this.fillFavoritUnitsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.fillFavoritUnitsToolStripMenuItem.Text = "Fill Favorit units";
            this.fillFavoritUnitsToolStripMenuItem.Click += new System.EventHandler(this.FillFavoritUnitsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // clearInternalErrorsToolStripMenuItem
            // 
            this.clearInternalErrorsToolStripMenuItem.Name = "clearInternalErrorsToolStripMenuItem";
            this.clearInternalErrorsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.clearInternalErrorsToolStripMenuItem.Text = "Clear Internal Errors";
            this.clearInternalErrorsToolStripMenuItem.Click += new System.EventHandler(this.clearInternalErrorsToolStripMenuItem_Click);
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showUnitsToolStripMenuItem,
            this.openFavoritUnitsToolStripMenuItem,
            this.openMeasurementsToolStripMenuItem,
            this.openInternalErrorsToolStripMenuItem,
            this.openAllToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // showUnitsToolStripMenuItem
            // 
            this.showUnitsToolStripMenuItem.Name = "showUnitsToolStripMenuItem";
            this.showUnitsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.showUnitsToolStripMenuItem.Text = "Open Units";
            this.showUnitsToolStripMenuItem.Click += new System.EventHandler(this.showUnitsToolStripMenuItem_Click);
            // 
            // openFavoritUnitsToolStripMenuItem
            // 
            this.openFavoritUnitsToolStripMenuItem.Name = "openFavoritUnitsToolStripMenuItem";
            this.openFavoritUnitsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openFavoritUnitsToolStripMenuItem.Text = "Open Favorit Units";
            this.openFavoritUnitsToolStripMenuItem.Click += new System.EventHandler(this.openFavoritUnitsToolStripMenuItem_Click);
            // 
            // openMeasurementsToolStripMenuItem
            // 
            this.openMeasurementsToolStripMenuItem.Name = "openMeasurementsToolStripMenuItem";
            this.openMeasurementsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openMeasurementsToolStripMenuItem.Text = "Open Measurements";
            this.openMeasurementsToolStripMenuItem.Click += new System.EventHandler(this.openMeasurementsToolStripMenuItem_Click);
            // 
            // openInternalErrorsToolStripMenuItem
            // 
            this.openInternalErrorsToolStripMenuItem.Name = "openInternalErrorsToolStripMenuItem";
            this.openInternalErrorsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openInternalErrorsToolStripMenuItem.Text = "Open Internal Errors";
            this.openInternalErrorsToolStripMenuItem.Click += new System.EventHandler(this.openInternalErrorsToolStripMenuItem_Click);
            // 
            // openAllToolStripMenuItem
            // 
            this.openAllToolStripMenuItem.Name = "openAllToolStripMenuItem";
            this.openAllToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openAllToolStripMenuItem.Text = "Open All";
            this.openAllToolStripMenuItem.Click += new System.EventHandler(this.openAllToolStripMenuItem_Click);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.AboutToolStripMenuItem.Text = "About...";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutBoxToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 554);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(968, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // clearFavoriteUnitsToolStripMenuItem
            // 
            this.clearFavoriteUnitsToolStripMenuItem.Name = "clearFavoriteUnitsToolStripMenuItem";
            this.clearFavoriteUnitsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.clearFavoriteUnitsToolStripMenuItem.Text = "Clear Favorite units";
            this.clearFavoriteUnitsToolStripMenuItem.Click += new System.EventHandler(this.clearFavoriteUnitsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(174, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 576);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "LogEverything";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FillUnitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showUnitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMeasurementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fillUnitsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fillFavoritUnitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFavoritUnitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearInternalErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInternalErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem clearFavoriteUnitsToolStripMenuItem;
    }
}

