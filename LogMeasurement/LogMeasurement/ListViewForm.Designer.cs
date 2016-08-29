namespace LogMeasurement
{
    partial class ListViewForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListViewForm));
            this.ListItemSplitContainer = new System.Windows.Forms.SplitContainer();
            this.itemsTreeView = new LogMeasurement.ListViewTreeView();
            this.ListViewItemImageList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ListItemSplitContainer)).BeginInit();
            this.ListItemSplitContainer.Panel1.SuspendLayout();
            this.ListItemSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListItemSplitContainer
            // 
            this.ListItemSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListItemSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.ListItemSplitContainer.Name = "ListItemSplitContainer";
            // 
            // ListItemSplitContainer.Panel1
            // 
            this.ListItemSplitContainer.Panel1.Controls.Add(this.itemsTreeView);
            this.ListItemSplitContainer.Size = new System.Drawing.Size(622, 474);
            this.ListItemSplitContainer.SplitterDistance = 207;
            this.ListItemSplitContainer.TabIndex = 0;
            // 
            // itemsTreeView
            // 
            this.itemsTreeView.AllowDrop = true;
            this.itemsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsTreeView.ImageIndex = 0;
            this.itemsTreeView.ImageList = this.ListViewItemImageList;
            this.itemsTreeView.Location = new System.Drawing.Point(0, 0);
            this.itemsTreeView.Name = "itemsTreeView";
            this.itemsTreeView.SelectedImageIndex = 0;
            this.itemsTreeView.Size = new System.Drawing.Size(207, 474);
            this.itemsTreeView.StateImageList = this.ListViewItemImageList;
            this.itemsTreeView.TabIndex = 0;
            this.itemsTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.itemsTreeView_ItemDrag);
            this.itemsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.itemsTreeView_AfterSelect);
            this.itemsTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.itemsTreeView_DragDrop);
            this.itemsTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.itemsTreeView_DragEnter);
            this.itemsTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.itemsTreeView_DragOver);
            this.itemsTreeView.DragLeave += new System.EventHandler(this.itemsTreeView_DragLeave);
            // 
            // ListViewItemImageList
            // 
            this.ListViewItemImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ListViewItemImageList.ImageStream")));
            this.ListViewItemImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ListViewItemImageList.Images.SetKeyName(0, "Unit_2.ico");
            this.ListViewItemImageList.Images.SetKeyName(1, "Unit_2.ico");
            this.ListViewItemImageList.Images.SetKeyName(2, "Unit_2.ico");
            this.ListViewItemImageList.Images.SetKeyName(3, "Unit_2.ico");
            this.ListViewItemImageList.Images.SetKeyName(4, "BaseUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(5, "BaseUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(6, "BaseUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(7, "BaseUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(8, "NamedDerivedUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(9, "NamedDerivedUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(10, "NamedDerivedUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(11, "NamedDerivedUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(12, "ConvertibleUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(13, "ConvertibleUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(14, "ConvertibleUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(15, "ConvertibleUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(16, "DerivedUnit_.ico");
            this.ListViewItemImageList.Images.SetKeyName(17, "DerivedUnit_.ico");
            this.ListViewItemImageList.Images.SetKeyName(18, "DerivedUnit_.ico");
            this.ListViewItemImageList.Images.SetKeyName(19, "DerivedUnit_.ico");
            this.ListViewItemImageList.Images.SetKeyName(20, "FavoriteUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(21, "FavoriteUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(22, "FavoriteUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(23, "FavoriteUnit.ico");
            this.ListViewItemImageList.Images.SetKeyName(24, "Measurement.ico");
            this.ListViewItemImageList.Images.SetKeyName(25, "Measurement.ico");
            this.ListViewItemImageList.Images.SetKeyName(26, "Measurement.ico");
            this.ListViewItemImageList.Images.SetKeyName(27, "Measurement.ico");
            this.ListViewItemImageList.Images.SetKeyName(28, "InternalError.ico");
            this.ListViewItemImageList.Images.SetKeyName(29, "InternalError.ico");
            this.ListViewItemImageList.Images.SetKeyName(30, "InternalError.ico");
            this.ListViewItemImageList.Images.SetKeyName(31, "InternalError.ico");
            // 
            // ListViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 474);
            this.Controls.Add(this.ListItemSplitContainer);
            this.Name = "ListViewForm";
            this.Text = "Measurements";
            this.Load += new System.EventHandler(this.ListViewForm_Load);
            this.ListItemSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ListItemSplitContainer)).EndInit();
            this.ListItemSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList ListViewItemImageList;
        private System.Windows.Forms.SplitContainer ListItemSplitContainer;
        //private System.Windows.Forms.TreeView itemsTreeView;
        ListViewTreeView itemsTreeView;

    }
}