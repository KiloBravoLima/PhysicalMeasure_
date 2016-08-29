using System;
using System.Windows.Forms;

namespace LogMeasurement
{
    enum ListViewFormWindowState  // Extends FormWindowState
    {
        // Inherited from FormWindowState
        Normal = FormWindowState.Normal,
        Minimized = FormWindowState.Minimized,
        Maximized = FormWindowState.Maximized,

        // Extents from FormWindowState to ListViewFormWindowState with:
        Unspecified = 3
    }

    [Flags]
    public enum ListViewClass : byte 
    {
        Unit = 1,  // All units
          BaseUnit = 2,
          NamedDerivedUnit = 4,
          ConvertedUnit = 8,
          DerivedUnit = 0x10,
          FavoriteUnit = 0x20,

        Measurement = 0x40,

        InternalError = 0x80,

        All = 0xff
    }

    public static class ListViewClassMethodes
    {
        public static String GetClassName(ListViewClass ViewClass)
        {
            // Set window title
            switch (ViewClass)
            {
                case ListViewClass.Unit:
                    return "Units";
                case ListViewClass.BaseUnit:
                    return "Base Units";
                case ListViewClass.NamedDerivedUnit:
                    return "Named Derivated Units";
                case ListViewClass.ConvertedUnit:
                    return "Convertible Units";
                case ListViewClass.DerivedUnit:
                    return "Other Derivated Units";
                case ListViewClass.FavoriteUnit:
                    return "Favorite units";
                case ListViewClass.Measurement:
                    return "Measurements";
                case ListViewClass.InternalError:
                    return "Internal Errors";
                case ListViewClass.All:
                    // this.Text = "Units and Measurements";
                    return "All";
                default:
                    return "Unhandled class : " + ViewClass.ToString();
            }
        }
    }


    public enum ListViewClassIconIndexes
    {
        II_FolderAll,       // Folder all items (units, Measurements, InternalErrors)
          II_FolderUnit,    // Folder all units
                    II_Unit,      // Generic Unit item
                II_FolderBaseUnit,  // Folder all base units
                    II_BaseUnit,      // Base Unit item
                II_FolderNamedDerivedUnit,  // Folder all named derived units
                    II_NamedDerivedUnit,  // Named derived Unit item
                II_FolderConvertibleUnit,
                    II_ConvertibleUnit,
                II_FolderDerivedUnit,
                    II_DerivedUnit,
                II_FolderFavoriteUnit,
                    II_FavoriteUnit,

          II_FolderMeasurement,
            II_Measurement,

          II_FolderInternalError,
            II_InternalError
    }


    [Flags]
    public enum ItemViewKind : byte
    {
        FixedLabels = 1,
        DBId = 2,

        All = 0xff
    }

    [Flags]
    public enum UnitItemViewKind : byte // Extents ItemViewKind
    {
        // Inherited from ItemViewKind
        FixedLabels = ItemViewKind.FixedLabels,
        DBId = ItemViewKind.DBId,

        // Extents from ItemViewKind to UnitItemViewKind with:
        Name = 4,
        Symbol = 8,
        BaseUnits = 16,

        // Inherited from ItemViewKind
        All = ItemViewKind.All
    }

    [Flags]
    public enum LoggedEventItemViewKind : byte // Extents ItemViewKind
    {
        // Inherited from ItemViewKind
        FixedLabels = ItemViewKind.FixedLabels,
        DBId = ItemViewKind.DBId,

        // Extents from ItemViewKind to LoggedEventItemViewKind with:
        LogTime = 4,
        EventTime = 8,

        // Inherited from ItemViewKind
        All = ItemViewKind.All
    }

    [Flags]
    public enum MeasurementItemViewKind : byte // Extents LoggedEventItemViewKind
    {
        // Inherited from ItemViewKind
        FixedLabels = ItemViewKind.FixedLabels,
        DBId = ItemViewKind.DBId,

        // Inherited from LoggedEventItemViewKind
        LogTime = 4,
        EventTime = 8,

        // Extents from ItemViewKind to MeasurementItemViewKind with:
        Value = 16,
        Unit = 32,

        // Inherited from ItemViewKind
        All = ItemViewKind.All
    }

    [Flags]
    public enum ErrorItemViewKind : byte  // Extents LoggedEventItemViewKind
    {
        // Inherited from ItemViewKind
        FixedLabels = ItemViewKind.FixedLabels,
        DBId = ItemViewKind.DBId,

        // Inherited from LoggedEventItemViewKind
        LogTime = 4,
        EventTime = 8,

        // Extents from ItemViewKind to ErrorItemViewKind with:
        ErrorMessage = 16,

        // Inherited from ItemViewKind
        All = ItemViewKind.All
    }

    [Flags]
    public enum TreeNodeViewMode : byte
    {
        TNVM_Normal = 0,  // Default =  Unselected and unexpanded
        TNVM_Selected = 1,  // Default =  Unselected
        TNVM_Expanded = 2,  // Default =  Unexpanded
        TNVM_SelectedExpanded = 3,  // Selected and expanded

        NoOfViewModes = 4
    }

    public class ListViewTreeView : TreeView
    {
        //static public byte ImageIndex(ListViewClassIconIndexes IconIndex, TreeNodeViewMode ViewMode) 
        public byte ViewModeImageIndex(ListViewClassIconIndexes IconIndex, TreeNodeViewMode ViewMode)
        {
            // return IconIndex * NoOfTreeNodeViewModes + ViewMode;
            int index = (Byte)IconIndex * (Byte)TreeNodeViewMode.NoOfViewModes + (Byte)ViewMode;
            return (byte)Math.Min(index, this.ImageList.Images.Count - 1);
        }
    }

}
