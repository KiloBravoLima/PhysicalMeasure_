using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LogMeasurement
{

    public partial class ListViewForm : Form
    {

        public abstract class TreeItemTreeNode : TreeNode
        {
            public TreeItemTreeNode(String itemText)
                : base(itemText)
            {
            }

            public TreeItemTreeNode(String itemText, int imageIndex,	int selectedImageIndex)
                : base(itemText, imageIndex, selectedImageIndex)
            {
            }
            public TreeItemTreeNode(Object tag, String itemText, int imageIndex, int selectedImageIndex)
                : base(itemText, imageIndex, selectedImageIndex)
            {
                this.Tag = tag;
            }

            public TreeItemTreeNode(String itemText, ListViewClassIconIndexes iconIndex, ListViewTreeView ItemTreeView)
                : this(itemText, ItemTreeView.ViewModeImageIndex(iconIndex, TreeNodeViewMode.TNVM_Normal), ItemTreeView.ViewModeImageIndex(iconIndex, TreeNodeViewMode.TNVM_Selected))
            {
            }
            public TreeItemTreeNode(Object tag, String itemText, ListViewClassIconIndexes iconIndex, ListViewTreeView ItemTreeView)
                : this(tag, itemText, ItemTreeView.ViewModeImageIndex(iconIndex, TreeNodeViewMode.TNVM_Normal), ItemTreeView.ViewModeImageIndex(iconIndex, TreeNodeViewMode.TNVM_Selected))
            {
            }
        }

        public class GroupItemTreeNode : TreeItemTreeNode
        {
            public GroupItemTreeNode(String itemText, ListViewClassIconIndexes iconIndex, ListViewTreeView ItemTreeView)
                : base(itemText, iconIndex, ItemTreeView)
            {
            }
        }

        public class UnitItemGroupTreeNode : GroupItemTreeNode
        {
            public UnitItemGroupTreeNode(String itemText, ListViewTreeView ItemTreeView)
                : base(itemText, ListViewClassIconIndexes.II_FolderUnit, ItemTreeView)
            {
            }
        }

        public class UnitItemTreeNode : TreeItemTreeNode
        {
            public UnitItemTreeNode(UnitItemViewKind viewKind, Unit u, ListViewTreeView LogItemTreeView)
                : base(u, u.GetUnitItemText(viewKind & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.Symbol)), ListViewClassIconIndexes.II_Unit, LogItemTreeView)
            {
            }
        }

        public class BaseUnitItemTreeNode : TreeItemTreeNode
        {
            public BaseUnitItemTreeNode(UnitItemViewKind viewKind, Unit u, ListViewTreeView LogItemTreeView)
                : base(u, u.GetUnitItemText(viewKind & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.Symbol)), ListViewClassIconIndexes.II_BaseUnit, LogItemTreeView)
            {
            }
        }
    
        public class NamedDerivatedUnitItemTreeNode : TreeItemTreeNode
        {
            public NamedDerivatedUnitItemTreeNode(UnitItemViewKind viewKind, Unit u, ListViewTreeView LogItemTreeView = null)
                : base(u, u.GetUnitItemText(viewKind & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.Symbol)), ListViewClassIconIndexes.II_NamedDerivedUnit, LogItemTreeView)
            {
            }
        }

        public class ConvertibleUnitItemTreeNode : TreeItemTreeNode
        {
            public ConvertibleUnitItemTreeNode(UnitItemViewKind viewKind, Unit u, ListViewTreeView LogItemTreeView = null)
                : base(u, u.GetUnitItemText(viewKind & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.Symbol | UnitItemViewKind.BaseUnits)), ListViewClassIconIndexes.II_ConvertibleUnit, LogItemTreeView)
            {
            }
        }

        public class DerivatedUnitItemTreeNode : TreeItemTreeNode
        {
            public DerivatedUnitItemTreeNode(UnitItemViewKind viewKind, Unit u, ListViewTreeView LogItemTreeView = null)
                : base(u, u.GetUnitItemText(viewKind & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.BaseUnits)), ListViewClassIconIndexes.II_DerivedUnit, LogItemTreeView)
            {
            }
        }

        public class FavoriteUnitItemTreeNode : TreeItemTreeNode
        {
            public FavoriteUnitItemTreeNode(UnitItemViewKind viewKind, Unit u, ListViewTreeView LogItemTreeView = null)
                : base(u, u.GetUnitItemText(viewKind & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.BaseUnits)), ListViewClassIconIndexes.II_DerivedUnit, LogItemTreeView)
            {
            }
        }

        public class MeasurementItemTreeNode : TreeItemTreeNode
        {
            public MeasurementItemTreeNode(MeasurementItemViewKind viewKind, MeasurementDBItem m, ListViewTreeView LogItemTreeView = null)
                : base(m, m.GetMeasurementItemText(viewKind & (MeasurementItemViewKind.DBId | MeasurementItemViewKind.LogTime | MeasurementItemViewKind.EventTime | MeasurementItemViewKind.Value | MeasurementItemViewKind.Unit)), ListViewClassIconIndexes.II_Measurement, LogItemTreeView)
            {
            }

        }

        public class ErrorItemTreeNode : TreeItemTreeNode
        {
            public ErrorItemTreeNode(ErrorItemViewKind viewKind, InternalErrorDBItem e, ListViewTreeView LogItemTreeView = null)
                : base(e.GetErrorItemText(viewKind & (ErrorItemViewKind.DBId | ErrorItemViewKind.LogTime | ErrorItemViewKind.EventTime | ErrorItemViewKind.ErrorMessage)), ListViewClassIconIndexes.II_InternalError, LogItemTreeView)
            {
            }

        }

        public ListViewClass ViewClass = ListViewClass.All;

        // public UnitItemViewKind UnitViewKind = UnitItemViewKind.All;
        public ItemViewKind ViewKind = (ItemViewKind)((ItemViewKind)ItemViewKind.DBId | (ItemViewKind)UnitItemViewKind.Name | (ItemViewKind)UnitItemViewKind.Symbol | (ItemViewKind)UnitItemViewKind.BaseUnits);

        public int FavoriteUnitListID = UnitList_LINQ.UnitListBaseNumber; // First (shared) favorite list
        /*
        public var function GetQuery()
        {
            //Obtaining the data source 
            MeasurementsDataContext dbEventLog = new MeasurementsDataContext();
        
             var queryAllItems = from u in dbEventLog.Units select u;
        }

        // public var queryAllItems;
        */

        public ListViewForm()
        {
            InitializeComponent();
        }


        private void ListViewForm_Load(object sender, EventArgs e)
        {
            // Set window title
            this.Text = ListViewClassMethodes.GetClassName(ViewClass);

            Load_Data();
        }


        public static void FillUnitsTable()
        {
#if USE_DB_MAPPING_LINQ_TO_SQL
            LogMeasurement.Unit_LINQ.FillUnitsTable();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK
            LogMeasurement.Unit_LINQ.FillUnitsTable();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK_CORE
            LogMeasurement.UnitDBItem.FillUnitsTable();
#endif
        }

        public static void FillUnitListsTable()
        {
#if USE_DB_MAPPING_LINQ_TO_SQL
            LogMeasurement.UnitList_LINQ.FillUnitListsTable();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK
            LogMeasurement.UnitList_LINQ.FillUnitListsTable();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK_CORE
            LogMeasurement.UnitListDBItem.FillUnitListsTable();
#endif
        }
        public static void ClearUnitListsTable()
        {
#if USE_DB_MAPPING_LINQ_TO_SQL
            LogMeasurement.UnitList_LINQ.ClearUnitListsTable();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK
            LogMeasurement.UnitList_LINQ.ClearUnitListsTable();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK_CORE
            LogMeasurement.UnitListDBItem.ClearUnitListsTable();
#endif
        }

        public static void ClearApplicationInternalErrorLog()
        {
#if USE_DB_MAPPING_LINQ_TO_SQL
            LogMeasurement.InternalError_LINQ.ClearApplicationInternalErrorLog();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK
            LogMeasurement.InternalError_LINQ.ClearApplicationInternalErrorLog();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK_CORE
            LogMeasurement.InternalErrorDBItem.ClearApplicationInternalErrorLog();
#endif
        }

        public void Load_Data()
        {
            //Obtaining the data source 
#if USE_DB_MAPPING_LINQ_TO_SQL
            MeasurementsDataContext dbEventLog = new MeasurementsDataContext();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK
            MeasurementsDataContext dbEventLog = new MeasurementsDataContext();
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK_CORE
            // MEASUREMENTSDataSet dbEventLog = new MEASUREMENTSDataSet();
            LoggingContext dbEventLog = new LoggingContext();
#endif
            // Display a wait cursor while the TreeNodes are being created.
            //Cursor.Current = new Cursor("MyWait.cur");

            // Suppress repainting the TreeView until all the objects have been created.
            itemsTreeView.BeginUpdate();

            // Clear the TreeView each time the method is called.
            itemsTreeView.Nodes.Clear();

            try
            {
                Stack<TreeNodeCollection> TreeNodeStack = new Stack<TreeNodeCollection>();
                TreeNodeStack.Push(itemsTreeView.Nodes);
                TreeNodeCollection TNC = TreeNodeStack.Peek();

                int TopViewClassCounts = 0;
                if ((ViewClass & ListViewClass.Unit) != 0)
                    TopViewClassCounts++;
                if ((ViewClass & ListViewClass.Measurement) != 0)
                    TopViewClassCounts++;
                if ((ViewClass & ListViewClass.InternalError) != 0)
                    TopViewClassCounts++;

                if (TopViewClassCounts > 1)
                {   // Place top view class nodes under AllRoot when more than one
                    TreeNode AllRootNode = new GroupItemTreeNode(ListViewClassMethodes.GetClassName(ListViewClass.All), ListViewClassIconIndexes.II_FolderAll, itemsTreeView);
                    

                    TNC.Add(AllRootNode);

                    TreeNodeStack.Push(AllRootNode.Nodes);
                    TNC = TreeNodeStack.Peek();
                }

                if ((ViewClass & (ListViewClass.Unit | ListViewClass.FavoriteUnit)) != 0)
                {   // Some units to show

                    /***
                    // Create the query 
                    var queryAllItems = from u in dbEventLog.Units select u;
                    ***/

#if USE_DB_MAPPING_LINQ_TO_SQL

                    //Create compiled query
                    var fnUnitsOfClass = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, int UnitClass, int ClassSize) =>
                        from u in dbEventLog1.Units
                        where u.Id >= UnitClass
                        where u.Id < (UnitClass + ClassSize)
                        select u);
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK

                //Create compiled query
                var fnUnitsOfClass = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, int UnitClass, int ClassSize) =>
                    from u in dbEventLog1.Units
                    where u.Id >= UnitClass
                    where u.Id < (UnitClass + ClassSize)
                    select u);
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK_CORE

                    /*
                    //Create compiled query
                var fnUnitsOfClass = CompiledQuery.Compile((LoggingContext dbEventLog1, int UnitClass, int ClassSize) =>
                    from u in dbEventLog1.Units
                    where u.Id >= UnitClass
                    where u.Id < (UnitClass + ClassSize)
                    select u);
                    */

                    List<UnitDBItem> fnUnitsOfClass(LoggingContext dbEventLog1, int UnitClass, int ClassSize)
                    {
                        var temp = from u in dbEventLog1.Units
                                   where u.Id >= UnitClass
                                   where u.Id < (UnitClass + ClassSize)
                                   select u;
                        return temp.ToList();
                    }

#endif

                    TreeNode UnitRootNode = null;
                    if (  (((ViewClass & ListViewClass.Unit) != 0) && (   ViewClass != ListViewClass.Unit 
                                                                    || (ViewKind & ItemViewKind.FixedLabels) != 0))
                        || (ViewClass & ListViewClass.FavoriteUnit) != 0 && ViewClass != ListViewClass.FavoriteUnit)
                    {
                        UnitRootNode = new UnitItemGroupTreeNode(ListViewClassMethodes.GetClassName(ListViewClass.Unit), itemsTreeView);
                        TNC.Add(UnitRootNode);
                        TreeNodeStack.Push(UnitRootNode.Nodes);
                        TNC = TreeNodeStack.Peek();
                    }

                    if ((ViewClass & ListViewClass.Unit) != 0)
                    {
                        TreeNode subnode1 = null;
                        if (ViewClass != ListViewClass.Unit || (ViewKind & ItemViewKind.FixedLabels) != 0)
                        {
                            subnode1 = new UnitItemGroupTreeNode(ListViewClassMethodes.GetClassName(ListViewClass.BaseUnit), itemsTreeView);
                            TNC.Add(subnode1);

                            TreeNodeStack.Push(subnode1.Nodes);
                            TNC = TreeNodeStack.Peek();
                        }

                        // Execute the query for base units
                        foreach (Unit u in fnUnitsOfClass(dbEventLog, Unit.BaseUnitBaseNumber, Unit.BaseUnitsClassSize))
                        {
                            //TreeNode subnode2 = new TreeNode(u.GetUnitItemText(((UnitItemViewKind)ViewKind) & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.Symbol)));
                            TreeNode subnode2 = new BaseUnitItemTreeNode((UnitItemViewKind)ViewKind, u, itemsTreeView);

                            TNC.Add(subnode2);
                        }

                        if (ViewClass != ListViewClass.Unit || (ViewKind & ItemViewKind.FixedLabels) != 0)
                        {
                            TreeNodeStack.Pop();
                            TNC = TreeNodeStack.Peek();

                            subnode1 = new UnitItemGroupTreeNode(ListViewClassMethodes.GetClassName(ListViewClass.NamedDerivedUnit), itemsTreeView);
                            TNC.Add(subnode1);

                            TreeNodeStack.Push(subnode1.Nodes);
                            TNC = TreeNodeStack.Peek();
                        }

                        // Execute the query named derivated units
                        foreach (Unit u in fnUnitsOfClass(dbEventLog, Unit.NamedDerivedUnitBaseNumber, Unit.NamedDerivedUnitsClassSize))
                        {
                            // TreeNode subnode2 = new TreeNode(u.GetUnitItemText(((UnitItemViewKind)ViewKind) & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.Symbol)));
                            TreeNode subnode2 = new NamedDerivatedUnitItemTreeNode((UnitItemViewKind)ViewKind, u, itemsTreeView);
                            TNC.Add(subnode2);
                        }

                        if (ViewClass != ListViewClass.Unit || (ViewKind & ItemViewKind.FixedLabels) != 0)
                        {
                            TreeNodeStack.Pop();
                            TNC = TreeNodeStack.Peek();

                            subnode1 = new UnitItemGroupTreeNode(ListViewClassMethodes.GetClassName(ListViewClass.ConvertedUnit), itemsTreeView);
                            TNC.Add(subnode1);

                            TreeNodeStack.Push(subnode1.Nodes);
                            TNC = TreeNodeStack.Peek();
                        }

                        // Execute the query named convertible units
                        foreach (Unit u in fnUnitsOfClass(dbEventLog, Unit.NamedConvertibleUnitBaseNumber, Unit.NamedConvertibleClassSize))
                        {
                            //TreeNode subnode2 = new TreeNode(u.GetUnitItemText(((UnitItemViewKind)ViewKind) & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.Symbol | UnitItemViewKind.BaseUnits)));
                            TreeNode subnode2 = new ConvertibleUnitItemTreeNode((UnitItemViewKind)ViewKind, u, itemsTreeView);
                            TNC.Add(subnode2);
                        }

                        if (ViewClass != ListViewClass.Unit || (((UnitItemViewKind)ViewKind) & UnitItemViewKind.FixedLabels) != 0)
                        {
                            TreeNodeStack.Pop();
                            TNC = TreeNodeStack.Peek();

                            subnode1 = new UnitItemGroupTreeNode(ListViewClassMethodes.GetClassName(ListViewClass.DerivedUnit), itemsTreeView);
                            TNC.Add(subnode1);

                            TreeNodeStack.Push(subnode1.Nodes);
                            TNC = TreeNodeStack.Peek();
                        }
                        // Execute the query for other derivated units
                        foreach (Unit u in fnUnitsOfClass(dbEventLog, Unit.OtherDerivedUnitBaseNumber, int.MaxValue - 256))
                        {
                            //TreeNode subnode2 = new TreeNode(u.GetUnitItemText(((UnitItemViewKind)ViewKind) & (UnitItemViewKind.DBId | UnitItemViewKind.Name | UnitItemViewKind.BaseUnits)));
                            TreeNode subnode2 = new DerivatedUnitItemTreeNode((UnitItemViewKind)ViewKind, u, itemsTreeView);
                            TNC.Add(subnode2);
                        }

                        if (ViewClass != ListViewClass.Unit || (((UnitItemViewKind)ViewKind) & UnitItemViewKind.FixedLabels) != 0)
                        {
                            TreeNodeStack.Pop();
                            TNC = TreeNodeStack.Peek();
                        }
                    }


                    if ((ViewClass & ListViewClass.FavoriteUnit) != 0)
                    {
                        TreeNode subnode1 = null;
                        if (ViewClass != ListViewClass.FavoriteUnit || (((UnitItemViewKind)ViewKind) & UnitItemViewKind.FixedLabels) != 0)
                        {
                            subnode1 = new UnitItemGroupTreeNode(ListViewClassMethodes.GetClassName(ListViewClass.FavoriteUnit), itemsTreeView);
                            TNC.Add(subnode1);
                            TreeNodeStack.Push(subnode1.Nodes);
                            TNC = TreeNodeStack.Peek();
                        }


#if USE_DB_MAPPING_LINQ_TO_SQL
                        //Create compiled query
                        var fnUnitsFromFavoritList = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, int FavoritUnitListID) =>
                            from fu in dbEventLog1.UnitLists
                            where fu.ListId == FavoritUnitListID
                            select fu.Unit);
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK
                        //Create compiled query
                        var fnUnitsFromFavoritList = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, int FavoritUnitListID) =>
                            from fu in dbEventLog1.UnitLists
                            where fu.ListId == FavoritUnitListID
                            select fu.Unit);
#elif USE_DB_MAPPING_LINQ_TO_ENTITY_FRAMEWORK_CORE

                        Unit fnUnitsOfUnitDbItem(UnitDBItem udbi)
                        {
                            return udbi as Unit;
                        }

                        //Create compiled query
                        List<Unit> fnUnitsFromFavoritList(LoggingContext dbEventLog1, int FavoritUnitListID)
                        {

                            /** */
                            var temp = from fu in dbEventLog1.UnitListElements 
                                       join u in dbEventLog1.Units on fu.UnitId equals u.Id
                                       where fu.ListId == FavoritUnitListID
                                       orderby fu.ElementIndex
                                       // select (u as Unit);
                                       select u ;

                            return temp.ToList().Select(ui => fnUnitsOfUnitDbItem(ui)).ToList();
                        }

#endif
                        List<Unit> favoritUnits = fnUnitsFromFavoritList(dbEventLog, FavoriteUnitListID);
                        // Execute the query favorite units
                        foreach (Unit u in favoritUnits)
                        {
                            TreeNode subnode2 = new FavoriteUnitItemTreeNode((UnitItemViewKind)ViewKind, u, itemsTreeView);
                            TNC.Add(subnode2);
                        }

                        if (ViewClass != ListViewClass.FavoriteUnit || (((UnitItemViewKind)ViewKind) & UnitItemViewKind.FixedLabels) != 0)
                        {
                            TreeNodeStack.Pop();
                            TNC = TreeNodeStack.Peek();
                        }
                    }

                    if (UnitRootNode != null)
                    {
                        TreeNodeStack.Pop();
                        TNC = TreeNodeStack.Peek();
                    }
                }

                if ((ViewClass & ListViewClass.Measurement) != 0)
                {
                    /***
                    // Create the query 
                    queryAllItems = null;
                    var queryAllItems2 = from m in dbEventLog.Measurements select m;
                    ***/


                    if (ViewClass != ListViewClass.Measurement || (ViewKind & ItemViewKind.FixedLabels) != 0)
                    {
                        TreeNode node = new TreeNode(ListViewClassMethodes.GetClassName(ListViewClass.Measurement));
                        TNC.Add(node);
                        TreeNodeStack.Push(node.Nodes);
                        TNC = TreeNodeStack.Peek();
                    }

                    // Execute the query 
                    //foreach (var m in queryAllItems2)
                    foreach (MeasurementDBItem m in dbEventLog.Measurements)
                    {
                        // ItemsListView.Items.Add(u.Name);
                        TreeNode subnode = new MeasurementItemTreeNode((MeasurementItemViewKind)ViewKind, m, itemsTreeView);
                        TNC.Add(subnode);
                    }

                    if (ViewClass != ListViewClass.Measurement || (ViewKind & ItemViewKind.FixedLabels) != 0)
                    {
                        TreeNodeStack.Pop();
                        TNC = TreeNodeStack.Peek();
                    }
                }

                if ((ViewClass & ListViewClass.InternalError) != 0)
                {

                    /***
                    // Create the query 
                    queryAllItems = null;
                    var queryAllItems2 = from m in dbEventLog.Measurements select m;
                    ***/

                    if (ViewClass != ListViewClass.InternalError || (ViewKind & ItemViewKind.FixedLabels) != 0)
                    {
                        TreeNode node = new TreeNode(ListViewClassMethodes.GetClassName(ListViewClass.InternalError));
                        TNC.Add(node);
                        TreeNodeStack.Push(node.Nodes);
                        TNC = TreeNodeStack.Peek();
                    }

                    // Execute the query 
                    //foreach (var m in queryAllItems2)
                    foreach (var ie in dbEventLog.InternalErrors)
                    {
                        // ItemsListView.Items.Add(u.Name);
                        TreeNode subnode = new TreeNode(ie.GetErrorItemText(((ErrorItemViewKind)ViewKind)));
                        TNC.Add(subnode);
                    }

                    if (ViewClass != ListViewClass.InternalError || (ViewKind & ItemViewKind.FixedLabels) != 0)
                    {
                        TreeNodeStack.Pop();
                        TNC = TreeNodeStack.Peek();
                    }
                }
            }
            finally 
            {
                itemsTreeView.ExpandAll();

                // Reset the cursor to the default for all controls.
                //Cursor.Current = Cursors.Default;

                // Begin repainting the TreeView.
                itemsTreeView.EndUpdate();
            }
        }

        private void itemsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (ViewClass)
            {
                case ListViewClass.Unit:
                    // this.Text = "Units";
                    //e.Node.Tag;
                    break;
                case ListViewClass.FavoriteUnit:
                    // this.Text = "Favorite units";
                    break;

                case ListViewClass.Measurement:
                    // this.Text = "Measurements";
                    break;

                case ListViewClass.InternalError:
                    // this.Text = "Internal Errors";
                    break;

                case ListViewClass.All:
                    // this.Text = "Units and Measurements";
                    // this.Text = "All";
                    break;
                default:
                    // this.Text = "Unhandled class : " + ViewClass.ToString();
                    break;
            }
        }

        private void itemsTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode draggedNode = e.Item as TreeNode;

            TreeView treeView = sender as TreeView;

            var type = draggedNode.GetType();

            // Move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left && type == typeof(FavoriteUnitItemTreeNode))
            {   // FavoriteUnitItemTreeNodes can be reordered
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

            // Copy the dragged node when the right mouse button is used.
            else // if (e.Button == MouseButtons.Right)
            {
                Boolean draggedNodeTypeCanBeCopied
                    = (    type == typeof(BaseUnitItemTreeNode)
                        || type == typeof(NamedDerivatedUnitItemTreeNode)
                        || type == typeof(ConvertibleUnitItemTreeNode)
                        || type == typeof(DerivatedUnitItemTreeNode)
                        || type == typeof(FavoriteUnitItemTreeNode));

                if (draggedNodeTypeCanBeCopied)
                {   // Unit tree items can be 'copied' to favorite list
                    DoDragDrop(e.Item, DragDropEffects.Copy);
                }
            }
        }

        // Set the target drop effect to the effect 
        // specified in the ItemDrag event handler.
        private void itemsTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void itemsTreeView_DragLeave(object sender, EventArgs e)
        {

        }

        // Select the node under the mouse pointer to indicate the 
        // expected drop location.
        private void itemsTreeView_DragOver(object sender, DragEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

            // Select the node at the mouse position.
            treeView.SelectedNode = treeView.GetNodeAt(targetPoint);
        }

        private void itemsTreeView_DragDrop(object sender, DragEventArgs e)
        {
            TreeView treeView = sender as TreeView;

            // Retrieve the client coordinates of the drop location.
            Point targetPoint = treeView.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = treeView.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            if (draggedNode == null) { draggedNode = (TreeNode)e.Data.GetData(typeof(BaseUnitItemTreeNode)); }
            if (draggedNode == null) { draggedNode = (TreeNode)e.Data.GetData(typeof(NamedDerivatedUnitItemTreeNode)); }
            if (draggedNode == null) { draggedNode = (TreeNode)e.Data.GetData(typeof(ConvertibleUnitItemTreeNode)); }
            if (draggedNode == null) { draggedNode = (TreeNode)e.Data.GetData(typeof(DerivatedUnitItemTreeNode)); }
            if (draggedNode == null) { draggedNode = (TreeNode)e.Data.GetData(typeof(FavoriteUnitItemTreeNode)); }

            if (draggedNode == null)
            {
                // Gets the original data formats in the data object by setting the automatic
                // conversion parameter to false.
                String[] myFormatsArray = e.Data.GetFormats(false);

                Debug.Assert(draggedNode != null);

                return;
            }

            // Confirm that the node at the drop location is not 
            // the dragged node or a descendant of the dragged node.
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current 
                // location and add it to the node at the drop location.
                if (e.Effect == DragDropEffects.Move)
                {
                    //UnitDBItem draggedUnit = (UnitDBItem)draggedNode.Tag;
                    // UnitDBItem targetUnit;
                    int fromIndex = draggedNode.Parent.Nodes.IndexOf(draggedNode)+1;
                    int toIndex = -1; 

                    var targetNodeType = targetNode.GetType();
                    if (targetNodeType == typeof(FavoriteUnitItemTreeNode))
                    {
                        // targetUnit = (UnitDBItem)targetNode.Tag;
                        toIndex = targetNode.Parent.Nodes.IndexOf(targetNode)+1;

                        targetNode = targetNode.Parent;
                    }
                    else
                    if (targetNodeType == typeof(UnitItemGroupTreeNode) && targetNode.Text == ListViewClassMethodes.GetClassName(ListViewClass.FavoriteUnit))
                    {
                        toIndex = targetNode.Nodes.Count +1;
                    }

                    if (toIndex >= 0)
                    {
                        // draggedNode.Remove();
                        // targetNode.Nodes.Add(draggedNode);

                        LoggingContext dbEventLog = new LoggingContext();

                        if (fromIndex < toIndex)
                        {
                            draggedNode.Remove();
                            targetNode.Nodes.Insert(toIndex-2, draggedNode);

                            foreach (UnitListElementDBItem uli in dbEventLog.UnitListElements.Where(eli => eli.ListId == 1 && eli.ElementIndex >= fromIndex))
                            {
                                int index = uli.ElementIndex;
                                if (index == fromIndex)
                                {
                                    uli.ElementIndex = toIndex;
                                }
                                else
                                if (index > fromIndex && index <= toIndex)
                                {
                                    uli.ElementIndex = uli.ElementIndex - 1;
                                }
                            }
                        }
                        else
                        {
                            draggedNode.Remove();
                            targetNode.Nodes.Insert(toIndex-1, draggedNode);

                            foreach (UnitListElementDBItem uli in dbEventLog.UnitListElements.Where(eli => eli.ListId == 1 && eli.ElementIndex >= toIndex))
                            {
                                int index = uli.ElementIndex;
                                if (index >= toIndex && index < fromIndex)
                                {
                                    uli.ElementIndex = uli.ElementIndex + 1;
                                }
                                else
                                if (index == fromIndex)
                                {
                                    uli.ElementIndex = toIndex;
                                }
                            }
                        }

                        dbEventLog.SaveChangesAsync();
                    }
                }

                // If it is a copy operation, clone the dragged node 
                // and add it to the node at the drop location.
                else if (e.Effect == DragDropEffects.Copy)
                {
                    //targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
                    int toIndex = -1;
                    var type = targetNode.GetType();
                    if (targetNode.GetType() == typeof(FavoriteUnitItemTreeNode))
                    {
                        toIndex = targetNode.Parent.Nodes.IndexOf(targetNode) + 1;

                        targetNode = targetNode.Parent;
                    }
                    else
                    {
                        toIndex = targetNode.Nodes.Count + 1;
                    }
                    if (targetNode.GetType() == typeof(UnitItemGroupTreeNode) && targetNode.Text == ListViewClassMethodes.GetClassName(ListViewClass.FavoriteUnit))
                    {
                        Boolean allreadyHasdraggedUnitAsFavorite = false;
                        foreach (TreeNode subNode in targetNode.Nodes)
                        {
                            if (subNode.Tag == draggedNode.Tag)
                            {
                                allreadyHasdraggedUnitAsFavorite = true;
                                break;
                            }
                        }
                        if (!allreadyHasdraggedUnitAsFavorite)
                        {
                            // tn = draggedNode.Clone();
                            Unit unit = (Unit)draggedNode.Tag;

                            TreeNode tn = new FavoriteUnitItemTreeNode((UnitItemViewKind)ViewKind, unit, itemsTreeView);
                            targetNode.Nodes.Insert(toIndex - 1, tn);

                            LoggingContext dbEventLog = new LoggingContext();

                            foreach (UnitListElementDBItem uli in dbEventLog.UnitListElements.Where(eli => eli.ListId == 1 && eli.ElementIndex >= toIndex))
                            {
                                uli.ElementIndex = uli.ElementIndex + 1;
                            }

                            dbEventLog.UnitListElements.Add(new UnitListElementDBItem() { ElementIndex = toIndex,  ListId = 1, UnitId = unit.Id });
                            dbEventLog.SaveChangesAsync();
                        }
                    }
                }

                // Expand the node at the location 
                // to show the dropped node.
                targetNode.Expand();
            }
        }


        // Determine whether one node is a parent 
        // or ancestor of a second node.
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            // Check the parent node of the second node.
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            // If the parent node is not null or equal to the first node, 
            // call the ContainsNode method recursively using the parent of 
            // the second node.
            return ContainsNode(node1, node2.Parent);
        }


    }
}
