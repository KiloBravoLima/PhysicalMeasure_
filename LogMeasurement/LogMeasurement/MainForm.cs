using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace LogMeasurement
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Let the first MDI child fill out parent area.

            // ListViewForm MDIChild_Units = CreateMDIChild(ListViewClass.Unit, ListViewFormWindowState.Maximized);

            // ListViewForm MDIChild_Measurements = CreateMDIChild(ListViewClass.Measurement, ListViewFormWindowState.Maximized);

            // Let the first MDI child fill out parent area.
            // ListViewForm firstMDIChild = CreateMDIChild(ListViewClass.All, ListViewFormWindowState.Maximized);
            ListViewForm firstMDIChild = CreateMDIChild(ListViewClass.Measurement | ListViewClass.InternalError | ListViewClass.Unit, ListViewFormWindowState.Maximized);

            // Display the new form.
            // MDIChild_Units.Show();
            // Display the new form.
            // MDIChild_Measurements.Show();
            // Display the new form.
            firstMDIChild.Show();
        }

        private ListViewForm CreateMDIChild(ListViewClass viewClass, ListViewFormWindowState windowState)
        {
            ListViewForm newMDIChild = new ListViewForm();
            // Set the Parent Form of the Child window.
            newMDIChild.MdiParent = this;

            if (windowState != ListViewFormWindowState.Unspecified)
            {
                // Let the first MDI child fill out parent area.
                newMDIChild.WindowState = (FormWindowState)windowState;
            }

            newMDIChild.MeasurementsViewClass = viewClass & (ListViewClass.Measurement | ListViewClass.InternalError);
            newMDIChild.UnitsViewClass = viewClass & ListViewClass.AnyUnitMask;

            return newMDIChild;
        }

        private void ShowMDIChild(ListViewClass viewClass, ListViewFormWindowState windowState = ListViewFormWindowState.Unspecified)
        {
            foreach (ListViewForm lwf in this.MdiChildren)
            {
                if (lwf.UnitsViewClass == viewClass)
                {
                    if (windowState != ListViewFormWindowState.Unspecified)
                    {
                        lwf.WindowState = (FormWindowState)windowState;
                    }
                    lwf.Activate();

                    return;
                }
            }

            ListViewForm newMDIChild = CreateMDIChild( viewClass, windowState);
            // Display the new form.
            newMDIChild.Show();
        }

        #region  Menu functions

        #region  File Menu functions
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion File Menu functions

        #region  Fill Menu functions

        private void UpdateListViewFormsForClass(ListViewClass lwc)
        {
            foreach (ListViewForm lwf in this.MdiChildren.Where(f => (((ListViewForm)f).UnitsViewClass & lwc) != 0))
            {
                Debug.Assert((lwf.UnitsViewClass & lwc) != 0);
                lwf.Load_Data();
            }
        }
        
        private void FillUnitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogMeasurement.ListViewForm.FillUnitsTable();
            UpdateListViewFormsForClass(ListViewClass.Unit);
        }

        private void FillFavoritUnitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogMeasurement.ListViewForm.FillUnitListsTable();
            UpdateListViewFormsForClass(ListViewClass.FavoriteUnit);
        }

        private void clearFavoriteUnitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogMeasurement.ListViewForm.ClearUnitListsTable();
            UpdateListViewFormsForClass(ListViewClass.FavoriteUnit);
        }


        private void clearInternalErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogMeasurement.ListViewForm.ClearApplicationInternalErrorLog();
            UpdateListViewFormsForClass(ListViewClass.FavoriteUnit);
        }

        #endregion  Fill Menu functions

        #region  Window Menu functions

        private void showUnitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMDIChild(ListViewClass.Unit);
        }

        private void openFavoritUnitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMDIChild(ListViewClass.FavoriteUnit);
        }

        private void openMeasurementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMDIChild(ListViewClass.Measurement);
        }

        private void openInternalErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMDIChild(ListViewClass.InternalError);
        }

        private void openAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMDIChild(ListViewClass.All);
        }

        #endregion Window Menu functions

        #region  About Menu functions
        private void AboutBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();

            // Display the new form.
            ab.Show();
        }
        #endregion About Menu functions

        #endregion  Menu functions

    }
}
