
namespace LogMeasurement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Data.Linq;

    using Microsoft.EntityFrameworkCore;

    using PhysicalMeasure;
    using PhysicalCalculator;

    //using KBL.Extensions;
    using PhysicalUnit = PhysicalMeasure.Unit;
    using PhysicalQuantity = PhysicalMeasure.Quantity;
    using Microsoft.EntityFrameworkCore.Storage;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LoggingContext : DbContext
    {
        public DbSet<UnitDBItem> Units { get; set; }

        public DbSet<UnitListElementDBItem> UnitListElements { get; set; }

        public DbSet<MeasurementDBItem> Measurements { get; set; }

        public DbSet<InternalErrorDBItem> InternalErrors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.ConsoleApp.NewDb;Trusted_Connection=True;");
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MeasurementLog;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UnitDBItem>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<UnitDBItem>().Property(e => e.Id).UseSqlServerIdentityColumn();
            // modelBuilder.Entity<UnitDBItem>().Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<UnitDBItem>().Ignore(x => x.Text);
            modelBuilder.Entity<UnitDBItem>().Ignore(x => x.Exponents);
            modelBuilder.Entity<UnitDBItem>().Ignore(x => x.ExponentsText);
            modelBuilder.Entity<UnitDBItem>().Property(x => x.ExponentsBin).HasColumnName("Exponents");

            modelBuilder.Entity<UnitListElementDBItem>().Ignore(x => x.Unit);
            modelBuilder.Entity<UnitListElementDBItem>()
                .HasOne(unitListElement => unitListElement.UnitItem)
                .WithMany(unitItem => unitItem.UnitListElements)
                .HasForeignKey(unitListElement => unitListElement.UnitId)
                .HasPrincipalKey(unitItem => unitItem.Id);

            modelBuilder.Entity<MeasurementDBItem>().Ignore(x => x.Text);

            modelBuilder.Entity<InternalErrorDBItem>().Ignore(x => x.Text);
        }
    }

    public class UnitDBItem : Unit
    {
        // [Required]
        //public override int Id { get; set; }

        public virtual Byte[] ExponentsBin { get; set; }
        public override SByte[] Exponents { get { return ExponentsBin.ToSBytes(); } set { ExponentsBin = value.ToBytes(); } }

        public List<UnitListElementDBItem> UnitListElements { get; set; }

        public static void FillUnitsTable()
        {
            //Obtaining the data source 
            LoggingContext dbEventLog = new LoggingContext();

            int OtherDerivedUnitClassSize = OtherDerivedUnitStrings.Count;

            //int DeleteClassSize = BaseUnitsClassSize + NamedDerivedUnitsClassSize + OtherDerivedUnitClassSize;
            int DeleteClassSize = OtherDerivedUnitBaseNumber + OtherDerivedUnitClassSize;

            // Remove all units from dbEventLog.Units
            //Create compiled query
            var fnUnitsOfClass = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, int UnitClass, int ClassSize) =>
                from c in dbEventLog1.Units
                where c.Id >= UnitClass
                where c.Id < (UnitClass + ClassSize)
                select c);

            using (IDbContextTransaction ts = dbEventLog.Database.BeginTransaction())
            {
                try
                {
                    // Execute the query 
                    // Remove all units from dbEventLog.Units
                    dbEventLog.Units.RemoveRange(dbEventLog.Units);

                    dbEventLog.SaveChanges();


                    // SET IDENTITY_INSERT Units ON  to try to avoid exception in final dbEventLog.SaveChanges(); Entity framwork core SqlException: Cannot insert explicit value for identity column in table when IDENTITY_INSERT is set to OFF.
                    //dbEventLog.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Units ON");

                    // Fill base units into dbEventLog.Units
                    foreach (PhysicalMeasure.BaseUnit pu in PhysicalMeasure.Physics.SI_Units.BaseUnits)
                    {
                        UnitDBItem u = new UnitDBItem() { Id = BaseUnitBaseNumber + pu.BaseUnitNumber, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.Exponents };
                        dbEventLog.Units.Add(u);
                    }


                    // Fill named derived units into dbEventLog.Units
                    int NamedDerivedUnitIndex = 0;
                    foreach (PhysicalMeasure.NamedDerivedUnit pu in PhysicalMeasure.Physics.SI_Units.NamedDerivedUnits)
                    {
                        UnitDBItem u = new UnitDBItem() { Id = NamedDerivedUnitBaseNumber + NamedDerivedUnitIndex, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.Exponents };
                        dbEventLog.Units.Add(u);

                        NamedDerivedUnitIndex++;
                    }


                    // Fill Convertible units into dbEventLog.Units
                    int NamedConvertibleUnitIndex = 0;
                    foreach (PhysicalMeasure.ConvertibleUnit pu in PhysicalMeasure.Physics.SI_Units.ConvertibleUnits)
                    {
                        UnitDBItem u = new UnitDBItem() { Id = NamedConvertibleUnitBaseNumber + NamedConvertibleUnitIndex, Name = pu.Name, Symbol = pu.Symbol, Exponents = pu.Exponents, ConversionFactor = pu.Conversion.LinearScale, ConversionOffset = pu.Conversion.LinearOffset };
                        dbEventLog.Units.Add(u);

                        NamedConvertibleUnitIndex++;
                    }


                    // Fill named derived units into dbEventLog.Units
                    int OtherDerivedUnitIndex = 0;
                    foreach (string unitStr in OtherDerivedUnitStrings)
                    {
                        PhysicalUnit pu = PhysicalUnit.Parse(unitStr);
                        PhysicalQuantity pq = pu.ConvertToSystemUnit().ConvertToDerivedUnit();

                        UnitDBItem u = new UnitDBItem() { Id = OtherDerivedUnitBaseNumber + OtherDerivedUnitIndex, Name = pu.ToPrintString(), Exponents = pq.Unit.Exponents, ConversionFactor = 1 / pq.Value };
                        dbEventLog.Units.Add(u);

                        OtherDerivedUnitIndex++;
                    }

                    // Entity framwork core SqlException: Cannot insert explicit value for identity column in table when IDENTITY_INSERT is set to OFF.
                    // dbEventLog.Database.GetDbConnection();
                    //dbEventLog.ChangeTracker.Context.
                    // dbEventLog.ChangeTracker.QueryTrackingBehavior
                    // dbEventLog.
                    dbEventLog.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Units ON");
                    dbEventLog.SaveChanges();

                    ts.Commit();
                }
                catch (Exception ex)
                {
                    InternalErrorDBItem.ApplicationInternalErrorLog("Error when FillUnitsTable. " + ex.Message);
                    ts.Rollback();
                    // throw;
                }
            }
        }
    }

    public class UnitListElementDBItem : UnitListElement // DatabaseItem
    {
        public int UnitId { get; set; }

        [ForeignKey("UnitId")]
        public UnitDBItem UnitItem { get {return Unit as UnitDBItem; } set { Unit = value; } }
    }
    
    public class UnitListDBItem : UnitList // , DataItemList
    {
        public static void FillUnitListsTable()
        {
            //Obtaining the data source 
            LoggingContext dbEventLog = new LoggingContext();


            //Create compiled query
            /*
            var fnUnitIdFromName = CompiledQuery.Compile((MeasurementsDataContext dbEventLog1, string UnitName) =>
                from u in dbEventLog.Units
                where u.Name == UnitName
                select u.Id);
            */
            List<int> fnUnitIdFromName(LoggingContext dbEventLog1, string UnitName)
            {
                var temp = from u in dbEventLog.Units
                           where u.Name == UnitName
                           select u.Id;

                return temp.ToList();
            }

            // Execute the query 
            // Remove all unit lists from dbEventLog.UnitLists
            dbEventLog.UnitListElements.RemoveRange(dbEventLog.UnitListElements);

            dbEventLog.SaveChanges();

            // Fill named derived units into dbEventLog.Units
            int UnitListId = UnitListBaseNumber;

            int UnitListElementIndex = 0;
            foreach (string unitStr in UnitList.FavoritUnitStrings)
            {
                // IPhysicalUnit pu = PhysicalMeasure.PhysicalUnit.Parse(unitStr);
                // IPhysicalQuantity pq = pu.ConvertToSystemUnit();

                var q = fnUnitIdFromName(dbEventLog, unitStr);

                int UnitListElementUnitId = q.ElementAtOrDefault(0);
                // Debug.Assert(UnitListElementUnitId != 0);

                if (UnitListElementUnitId != 0)
                {
                    UnitListElementDBItem ul_element = new UnitListElementDBItem() { UnitId = UnitListElementUnitId, ListId = UnitListId, ElementIndex = UnitListElementIndex +1 };
                    dbEventLog.UnitListElements.Add(ul_element);
                }
                else
                {
                    InternalErrorDBItem.ApplicationInternalErrorLog("Error when adding '" + unitStr + "' as favorite unit. Found no unit named '" + unitStr + "'");
                }

                UnitListElementIndex++;
            }

            dbEventLog.SaveChanges();
        }

        
        public static void ClearUnitListsTable()
        {
            //Obtaining the data source 
            LoggingContext dbEventLog = new LoggingContext();

            // Remove all Internal errors 
            dbEventLog.UnitListElements.RemoveRange(dbEventLog.UnitListElements);

            dbEventLog.SaveChanges();
        }
    }

    public class MeasurementDBItem  : Measurement // DatabaseItem
    {
        //public override int Id { get; set; }

    }

    public class InternalErrorDBItem : InternalError // DatabaseItem
    {
        //public override int Id { get; set; }

        public static void ClearApplicationInternalErrorLog()
        {
            //Obtaining the data source 
            LoggingContext dbEventLog = new LoggingContext();

            // Remove all Internal errors 
            dbEventLog.InternalErrors.RemoveRange(dbEventLog.InternalErrors);

            dbEventLog.SaveChanges();
        }

        public static void ApplicationInternalErrorLog(string errorMessage)
        {
            //Obtaining the data source 
            LoggingContext dbEventLog = new LoggingContext();

            int nextId = dbEventLog.InternalErrors.Count() + 1;
            // InternalErrorDBItem ie = new InternalErrorDBItem() { Id = nextId, LogTime = System.DateTime.Now, EventTime = System.DateTime.Now, ErrorMessage = errorMessage };
            InternalErrorDBItem ie = new InternalErrorDBItem() { LogTime = System.DateTime.Now, EventTime = System.DateTime.Now, ErrorMessage = errorMessage };
            dbEventLog.InternalErrors.Add(ie);

            dbEventLog.SaveChanges();
        }
    }

}
