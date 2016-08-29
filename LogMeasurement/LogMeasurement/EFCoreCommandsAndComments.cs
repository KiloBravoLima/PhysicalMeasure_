

/**
Create database migration (database schema definitions)
    •Run "Add-Migration MyFirstMigration" to scaffold a migration to create the initial set of tables for your model.

    PM> Add-Migration CreateDatabaseInstanceMigration
An operation was scaffolded that may result in the loss of data. Please review the migration for accuracy.
To undo this action, use Remove-Migration.

    PM> Add-Migration CreateDatabaseInstanceMigration_2016_08_05_01_20

    When Add-Migration fails with "Project 'Default' is not found.", try remove the Migrations folder completely:
    PM> Add-Migration CreateDatabaseInstanceMigration_2016_08_05_01_20
    Project 'Default' is not found.
    
    Remove the folder "Migrations", and then retry to create a migration:
    PM> Add-Migration CreateDatabaseInstanceMigration_2016_08_05_01_30
    To undo this action, use Remove-Migration.
    PM> Update-Database
    Applying migration '20160804232957_CreateDatabaseInstanceMigration_2016_08_05_01_30'.
    Done.
    PM> 

Create database (actual SQL database instance)
    •Run "Update-Database" to apply the new migration to the database. Because your database doesn’t exist yet, it will be created for you before the migration is applied



    PM> Remove-Migration



SqlException: Cannot insert explicit value for identity column in table 'Units' when IDENTITY_INSERT is set to OFF.
// [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // dbEventLog.SaveChanges(); -> Entity framwork core SqlException: Cannot insert explicit value for identity column in table when IDENTITY_INSERT is set to OFF.

 
 **/


/*

        private void itemsTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = e.Item as TreeNode;

            TreeView treeView = sender as TreeView;


            // Move the dragged node when the left mouse button is used.
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

            // Copy the dragged node when the right mouse button is used.
            else if (e.Button == MouseButtons.Right)
            {
                DoDragDrop(e.Item, DragDropEffects.Copy);
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

            // Confirm that the node at the drop location is not 
            // the dragged node or a descendant of the dragged node.
            if (!draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
            {
                // If it is a move operation, remove the node from its current 
                // location and add it to the node at the drop location.
                if (e.Effect == DragDropEffects.Move)
                {
                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                }

                // If it is a copy operation, clone the dragged node 
                // and add it to the node at the drop location.
                else if (e.Effect == DragDropEffects.Copy)
                {
                    targetNode.Nodes.Add((TreeNode)draggedNode.Clone());
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
 

 */
