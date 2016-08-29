namespace LogMeasurement
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Data.Linq;
    using PhysicalMeasure;
    using PhysicalCalculator;

    using System;
    //using KBL.Extensions;
    using PhysicalUnit = PhysicalMeasure.Unit;
    using PhysicalQuantity = PhysicalMeasure.Quantity;

    
    partial class MeasurementsDataContext
    {
    }

    public partial class Unit_LINQ : Unit // DatabaseItem
    {
        //protected UnitKind _kindofUnit = UnitKind.BaseUnit;

        //public UnitKind KindOfUnit { get { return _kindofUnit; } }
        //public override string Text { get { return Name + "  " + (!string.IsNullOrWhiteSpace(Symbol) ? Symbol : ExponentsText); } set { /* Name = value; */ } }

        /*
        public string GetUnitItemText(UnitItemViewKind UnitViewKind) 
        { 
            string  str ="";

            if ((UnitViewKind & UnitItemViewKind.DBId) != 0)
            {
                str += "[" + Id.ToString() + "]";
            }

            if ((UnitViewKind & UnitItemViewKind.Name) != 0)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    str += " ";
                }
                str += Name;
            }

            if ((UnitViewKind & UnitItemViewKind.Symbol) != 0)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    str += " ";
                }
                str += Symbol;
            }

            if (   (((UnitViewKind & UnitItemViewKind.Symbol) != 0) && string.IsNullOrWhiteSpace(Symbol))
                || ((UnitViewKind & UnitItemViewKind.BaseUnits) != 0))
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    str += " ";
                }
                str += ExponentsText;
            }

            return str;
        }


        // string.Format(); 
        //public string ExponentsText { get { return PhysicalUnit.MakePhysicalUnit(DimensionExponents.Exponents(Exponents.ToArray()), ConvertionFactor ?? 1.0).ToString(); } set { /*  Name = value; * /  } }
        //public string ExponentsText { get { return PhysicalUnit.MakePhysicalUnit(DimensionExponents.Exponents(Exponents.ToArray()), 1.0).ToString(); } set { /*  Name = value; * /  } }
        //public string ExponentsText { get { return PhysicalUnit.MakePhysicalUnit(DimensionExponents.Exponents(Exponents.ToArray()), ConversionFactor ?? 1.0, ConversionOffset ?? 0.0).ToString(); } set { /*  Name = value; * /  } }
        public string ExponentsText 
        { 
            get 
            {
                if (!ConversionOffset.HasValue || ConversionOffset.Value == 0)
                {
                    return PhysicalUnit.MakePhysicalUnit(Exponents.ToArray().ToSBytes(), ConversionFactor ?? 1.0, ConversionOffset ?? 0.0).ToString();
                }
                else
                {
                    return PhysicalUnit.MakePhysicalUnit(Exponents.ToArray().ToSBytes(), ConversionFactor ?? 1.0, ConversionOffset ?? 0.0).ToString();
                }
            } 
            set { / *  Name = value; * /  } 
        }
        **/

        /*
        // Define FillUnitsTable base numbers and class sizes
        public const int BaseUnitBaseNumber = 1;
        public const int BaseUnitsClassSize = 16;
        public const int NamedDerivedUnitBaseNumber = BaseUnitBaseNumber + BaseUnitsClassSize;
        public const int NamedDerivedUnitsClassSize = 32;
        public const int NamedConvertibleUnitBaseNumber = NamedDerivedUnitBaseNumber + NamedDerivedUnitsClassSize;
        public const int NamedConvertibleClassSize = 16;
        public const int OtherDerivedUnitBaseNumber = NamedConvertibleUnitBaseNumber + NamedConvertibleClassSize;

        public static List<string> OtherDerivedUnitStrings = new List<string> { "m/s", "Km/h", "Kg/h", "l/h" , "KWh"};

        public static int OtherDerivedUnitClassSize = OtherDerivedUnitStrings.Count;
        ***/

        public static void FillUnitsTable()
        {
            //Obtaining the data source 
            MeasurementsDataContext dbEventLog = new MeasurementsDataContext();

            //List<string> OtherDerivedUnitStrings = new List<string> { "m/s", "Km/h", "Kg/h", "l/h" };

            int OtherDerivedUnitClassSize = OtherDerivedUnitStrings.Count;

            //int DeleteClassSize = BaseUnitsClassSize + NamedDerivedUnitsClassSize + OtherDerivedUnitClassSize;
            int DeleteClassSize = OtherDerivedUnitBaseNumber + OtherDerivedUnitClassSize;

            // Remove all units from dbEventLog.Units
            //dbEventLog.Units.DeleteAllOnSubmit(dbEventLog.Units);
            //dbEventLog.SubmitChanges();

            //Create compiled query
            var fnUnitsOfClass = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, int UnitClass, int ClassSize) =>
                from c in dbEventLog1.Units
                where c.Id >= UnitClass
                where c.Id < (UnitClass + ClassSize)
                select c);

            // Execute the query 
            // Remove all units from dbEventLog.Units
            // foreach (Unit u in dbEventLog.Units)
            ////foreach (Unit u in fnUnitsOfClass(dbEventLog, BaseUnitBaseNumber - 1, DeleteClassSize))
            ////{
            ////    dbEventLog.Units.DeleteOnSubmit(u);
            ////}
            // Remove all unit lists from dbEventLog.Units
            dbEventLog.Units.DeleteAllOnSubmit(dbEventLog.Units);


            // Fill base units into dbEventLog.Units
            foreach (PhysicalMeasure.BaseUnit pu in PhysicalMeasure.Physics.SI_Units.BaseUnits)
            {
                Unit_LINQ u = new Unit_LINQ() { Id = BaseUnitBaseNumber + pu.BaseUnitNumber, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.Exponents };
                dbEventLog.Units.InsertOnSubmit(u);
            }


            // Fill named derived units into dbEventLog.Units
            int NamedDerivedUnitIndex = 0;
            foreach (PhysicalMeasure.NamedDerivedUnit pu in PhysicalMeasure.Physics.SI_Units.NamedDerivedUnits)
            {
                Unit_LINQ u = new Unit_LINQ() { Id = NamedDerivedUnitBaseNumber + NamedDerivedUnitIndex, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.Exponents };
                dbEventLog.Units.InsertOnSubmit(u);

                NamedDerivedUnitIndex++;
            }


            // Fill Convertible units into dbEventLog.Units
            int NamedConvertibleUnitIndex = 0;
            foreach (PhysicalMeasure.ConvertibleUnit pu in PhysicalMeasure.Physics.SI_Units.ConvertibleUnits)
            {
                //Unit u = new Unit() { Id = NamedDerivedUnitBaseNumber + NamedDerivedUnitIndex + NamedConvertibleUnitIndex, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.UnsignedExponents, ConvertionFactor = 1 / pu.ConvertToPrimaryUnit().Value };
                //Unit u = new Unit() { Id = NamedConvertibleUnitBaseNumber + NamedConvertibleUnitIndex, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.UnsignedExponents, ConvertionFactor = 1 / pu.ConvertToPrimaryUnit().Value, ConvertionOffset =  };
                Unit_LINQ u = new Unit_LINQ() { Id = NamedConvertibleUnitBaseNumber + NamedConvertibleUnitIndex, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.Exponents, ConversionFactor = pu.Conversion.LinearScale, ConversionOffset = pu.Conversion.LinearOffset  };
                dbEventLog.Units.InsertOnSubmit(u);

                NamedConvertibleUnitIndex++;
            }


            // Fill named derived units into dbEventLog.Units
            int OtherDerivedUnitIndex = 0;
            foreach (string unitStr in OtherDerivedUnitStrings)
            {
                PhysicalUnit pu = PhysicalUnit.Parse(unitStr);
                PhysicalQuantity pq = pu.ConvertToSystemUnit().ConvertToDerivedUnit();

                //Unit u = new Unit() { Id = OtherDerivedUnitBaseNumber + OtherDerivedUnitIndex, Name = pu.ToPrintString(), Exponents = pu.UnsignedExponents};
                Unit_LINQ u = new Unit_LINQ() { Id = OtherDerivedUnitBaseNumber + OtherDerivedUnitIndex, Name = pu.ToPrintString(), Exponents = pq.Unit.Exponents, ConversionFactor = 1 / pq.Value };
                dbEventLog.Units.InsertOnSubmit(u);

                OtherDerivedUnitIndex++;
            }

            dbEventLog.SubmitChanges();
        }
    }

    partial class UnitList_LINQ : DatabaseItem
    {
        public const int UnitListBaseNumber = 1;
        public const int UnitListElementBaseNumber = 1;

        public static void FillUnitListsTable()
        {
            //Obtaining the data source 
            MeasurementsDataContext dbEventLog = new MeasurementsDataContext();

            List<string> FavoritUnitStrings = new List<string> { "m/s", "Km/h", "ml", "l/h" };

            //Create compiled query
            var fnUnitIdFromName = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, string UnitName) =>
                from u in dbEventLog.Units
                where u.Name == UnitName
                select u.Id);
            
            // Execute the query 
            // Remove all unit lists from dbEventLog.UnitLists
            dbEventLog.UnitLists.DeleteAllOnSubmit(dbEventLog.UnitLists);

            dbEventLog.SubmitChanges();

            // Fill named derived units into dbEventLog.Units
            int UnitListId = UnitListBaseNumber;

            int UnitListElementIndex = 0;
            foreach (string unitStr in FavoritUnitStrings)
            {
                // IPhysicalUnit pu = PhysicalMeasure.PhysicalUnit.Parse(unitStr);
                // IPhysicalQuantity pq = pu.ConvertToSystemUnit();

                var q = fnUnitIdFromName(dbEventLog, unitStr);

                int UnitListElementUnitId = q.ElementAtOrDefault(0);
                Debug.Assert(UnitListElementUnitId != 0);

                if (UnitListElementUnitId != 0)
                {
                    UnitList_LINQ ul_element = new UnitList_LINQ() { Id = UnitListElementBaseNumber + UnitListElementIndex, UnitId = UnitListElementUnitId, ListId = UnitListId };
                    //UnitList ul_element = new UnitList() { UnitId = UnitListElementUnitId, ListId = UnitListId };
                    dbEventLog.UnitLists.InsertOnSubmit(ul_element);
                }
                else
                {
                   InternalError_LINQ.ApplicationInternalErrorLog( "Error when adding '" + unitStr + "' as favorite unit. Found no unit named '" + unitStr + "'");
                }

                UnitListElementIndex++;
            }

            dbEventLog.SubmitChanges();
        }

    }

    partial class Measurement_LINQ : DisplayableItem
    {
        public override string Text { get { return  this.MeasuredValue.ToString() + " " + this.MeasuredUnit.ToString(); } set { /* Name = value; */ } }
    }


    partial class InternalError_LINQ: DisplayableItem
    {
        public override string Text { get { return  this.EventTime.ToString()+ " " + this.ErrorMessage; } set { /* Name = value; */ } }

        public string GetErrorItemText(ErrorItemViewKind UnitViewKind) 
        { 
            string  str ="";

            if ((UnitViewKind & ErrorItemViewKind.DBId) != 0)
            {
                str += "[" + Id.ToString() + "]";
            }

            if ((UnitViewKind & ErrorItemViewKind.LogTime) != 0)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    str += " ";
                }
                str += LogTime.ToString();
            }

            if ((UnitViewKind & ErrorItemViewKind.EventTime) != 0)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    str += " ";
                }
                str += EventTime.ToString();
            }

            if ((UnitViewKind & ErrorItemViewKind.ErrorMessage) != 0)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    str += " ";
                }
                str += ErrorMessage.ToString();
            }

            return str;
        }


        public static void ClearApplicationInternalErrorLog()
        {
            //Obtaining the data source 
            MeasurementsDataContext dbEventLog = new MeasurementsDataContext();

            // Remove all Internal errors 
            dbEventLog.InternalErrors.DeleteAllOnSubmit(dbEventLog.InternalErrors);

            dbEventLog.SubmitChanges();
        }

        // ApplicationInternalErrorLog("Error when adding '" + unitStr + " as favorite unit");
        public static void ApplicationInternalErrorLog(string errorMessage)
        {
            //Obtaining the data source 
            MeasurementsDataContext dbEventLog = new MeasurementsDataContext();

            int nextId = dbEventLog.InternalErrors.Count() +1;
            InternalError_LINQ ie = new InternalError_LINQ() { Id = nextId, LogTime = System.DateTime.Now, EventTime = System.DateTime.Now, ErrorMessage = errorMessage };
            dbEventLog.InternalErrors.InsertOnSubmit(ie);

            dbEventLog.SubmitChanges();
        }
    }


    

}
