using System;
//using Microsoft.EntityFrameworkCore;
using PhysicalMeasure;

//using KBL.Extensions;
using PhysicalUnit = PhysicalMeasure.Unit;
using PhysicalQuantity = PhysicalMeasure.Quantity;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogMeasurement
{
    // Pseudo Typedef 
    // public using IdInt = System.Int16;
    public struct IdInt
    {
        private Int16 id;

        public static implicit operator IdInt(Int16 i)
        {
            return new IdInt { id = i };
        }

        public static implicit operator Int16(IdInt i)
        {
            return i.id;
        }
    }

    interface IDataItem
    {
        // IdInt ID
        Int32 Id
        {
            get;
            set;
        }
    }

    interface INamedDataItem : IDataItem
    {
        String Name
        {
            get;
            set;
        }
    }

    interface IDisplayableDataItem : IDataItem
    {
        String Text
        {
            get;
            set;
        }
    }

    interface IMeasurementDataItem : IDataItem
    {
        String EventText
        {
            get;
            set;
        }
    }

    interface IListDataItem : IDataItem
    {
    }

    interface IDataItemList : IDataItem
    {
    }

    interface IDataItemListElement : IListDataItem
    {
        Int32 ListId
        {
            get;
            set;
        }
    }

    public abstract class DatabaseItem : IDataItem
    {
        // public virtual IdInt ID { get; set; } 
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // dbEventLog.SaveChanges(); -> Entity framwork core SqlException: Cannot insert explicit value for identity column in table when IDENTITY_INSERT is set to OFF.
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual Int32 Id { get; set; }
    }

    public abstract class DisplayableItem : DatabaseItem , IDisplayableDataItem
    {
        public virtual String Text { get; set; }
    }

    public abstract class DataItemList : DatabaseItem, IDataItemList
    {

    }

    public abstract class DataItemListElement : DatabaseItem, IDataItemList
    {
        public virtual Int32 ListId { get; set; }
    }

    interface IUnit : IDataItem
    {
        String Name { get; set; }
        String Symbol { get; set; }
        SByte[] Exponents { get; set; }
        UnitKind KindOfUnit { get; }

        Double? ConversionFactor { get; set; }
        Double? ConversionOffset { get; set; }
    }


    interface IUnitListElement : IDataItem
    {
        Unit Unit { get; set; }
    }

    interface IMeasurement : IDataItem
    {
        Double MeasuredValue { get; set; }
        UnitDBItem MeasuredUnit { get; set; }
    }

    interface IInternalError : IDataItem
    {
        DateTime LogTime { get; set; }
        DateTime EventTime { get; set; }

        String ErrorMessage { get; set; }
    }

    public abstract class Unit : DisplayableItem, IUnit
    {
        public virtual string Name { get; set; }
        public virtual string Symbol { get; set; }
        public virtual SByte[] Exponents { get; set; }

        public virtual Double? ConversionFactor { get; set; }
        public virtual Double? ConversionOffset { get; set; }

        public override string Text { get { return Name + "  " + (!string.IsNullOrWhiteSpace(Symbol) ? Symbol : ExponentsText); } set { /* Name = value; */ } }

        protected UnitKind _kindofUnit = UnitKind.BaseUnit;

        public UnitKind KindOfUnit { get { return _kindofUnit; } }

        public string GetUnitItemText(UnitItemViewKind UnitViewKind)
        {
            StringBuilder sb = new StringBuilder("");

            if ((UnitViewKind & UnitItemViewKind.DBId) != 0)
            {
                sb.Append($"[{Id.ToString()}]");
            }

            if ((UnitViewKind & UnitItemViewKind.Name) != 0)
            {
                sb.AppendSeparated(Name);
            }

            if ((UnitViewKind & UnitItemViewKind.Symbol) != 0)
            {
                sb.AppendSeparated(Symbol);
            }

            if ((((UnitViewKind & UnitItemViewKind.Symbol) != 0) && string.IsNullOrWhiteSpace(Symbol))
                || ((UnitViewKind & UnitItemViewKind.BaseUnits) != 0))
            {
                sb.AppendSeparated(ExponentsText);
            }

            return sb.ToString();
        }

        public string ExponentsText
        {
            get
            {
                if (!ConversionOffset.HasValue || ConversionOffset.Value == 0)
                {
                    return PhysicalUnit.MakePhysicalUnit(Exponents, ConversionFactor ?? 1.0, ConversionOffset ?? 0.0).ToString();
                }
                else
                {
                    return PhysicalUnit.MakePhysicalUnit(Exponents, ConversionFactor ?? 1.0, ConversionOffset ?? 0.0).ToString();
                }
            }
            set { /*  Name = value; */  }
        }

        // Define FillUnitsTable base numbers and class sizes
        public const int BaseUnitBaseNumber = 1;
        public const int BaseUnitsClassSize = 16;
        public const int NamedDerivedUnitBaseNumber = BaseUnitBaseNumber + BaseUnitsClassSize;
        public const int NamedDerivedUnitsClassSize = 32;
        public const int NamedConvertibleUnitBaseNumber = NamedDerivedUnitBaseNumber + NamedDerivedUnitsClassSize;
        public const int NamedConvertibleClassSize = 16;
        public const int OtherDerivedUnitBaseNumber = NamedConvertibleUnitBaseNumber + NamedConvertibleClassSize;

        public static List<string> OtherDerivedUnitStrings = new List<string> { "m/s", "Km/h", "Kg/h", "l/h", "KWh", "ml" };
        public static int OtherDerivedUnitClassSize = OtherDerivedUnitStrings.Count;
    }
    
    public abstract class UnitListElement : DataItemListElement, IUnitListElement
    {
        public int ElementIndex{ get; set; }
        public Unit Unit { get; set; }
    }

    public abstract class UnitList : DataItemList
    {
        public const int UnitListBaseNumber = 1;
        public const int UnitListElementBaseNumber = 1;

        public static readonly List<string> FavoritUnitStrings = new List<string> { "m/s", "Km/h", "ml", "l/h" };

        /*
        public static void FillUnitListsTable()
        {
        }
        */
    }

    public abstract class TimedEvent : DisplayableItem // , ITimedEvent
    {
        public DateTime EventTime { get; set; }

        public override string Text { get { return this.Id + " " + this.EventTime.ToString(); } set { /* Name = value; */ } }
    }

    public abstract class LoggedTimedEvent : TimedEvent // , ILoggedTimedEvent
    {
        public DateTime LogTime { get; set; }

        public override string Text { get { return this.Id + " " + this.LogTime.ToString() + " " + this.EventTime.ToString(); } set { /* Name = value; */ } }
    }


    public abstract class Measurement : LoggedTimedEvent, IMeasurement
    {
        public Double MeasuredValue { get; set; }

        public UnitDBItem MeasuredUnit { get; set; }

        public override string Text { get { return base.Text + " " + this.MeasuredValue.ToString() + " " + this.MeasuredUnit.ToString(); } set { /* Name = value; */ } }

        public string GetMeasurementItemText(MeasurementItemViewKind viewKind)
        {
            StringBuilder sb = new StringBuilder("");

            if ((viewKind & MeasurementItemViewKind.DBId) != 0)
            {
                sb.Append($"[{Id.ToString()}]");
            }

            if ((viewKind & MeasurementItemViewKind.LogTime) != 0)
            {
                sb.AppendSeparated(LogTime.ToString());
            }

            if ((viewKind & MeasurementItemViewKind.EventTime) != 0)
            {
                sb.AppendSeparated(EventTime.ToString());
            }

            if ((viewKind & MeasurementItemViewKind.Value) != 0)
            {
                sb.AppendSeparated(MeasuredValue.ToString());
            }

            if ((viewKind & MeasurementItemViewKind.Unit) != 0)
            {
                sb.AppendSeparated(MeasuredUnit.ToString());
            }

            return sb.ToString();
        }
    }

    public abstract class InternalError : LoggedTimedEvent, IInternalError
    {
        public String ErrorMessage { get; set; }

        public override string Text { get { return base.Text + " " + this.ErrorMessage; } set { /* Name = value; */ } }

        public string GetErrorItemText(ErrorItemViewKind errorViewKind)
        {
            StringBuilder sb = new StringBuilder("");

            if ((errorViewKind & ErrorItemViewKind.DBId) != 0)
            {
                sb.Append($"[{Id.ToString()}]");
            }

            if ((errorViewKind & ErrorItemViewKind.LogTime) != 0)
            {
                sb.AppendSeparated(LogTime.ToString());
            }

            if ((errorViewKind & ErrorItemViewKind.EventTime) != 0)
            {
                sb.AppendSeparated(EventTime.ToString());
            }

            if ((errorViewKind & ErrorItemViewKind.ErrorMessage) != 0)
            {
                sb.AppendSeparated(ErrorMessage);
            }

            return sb.ToString();
        }
    }
}