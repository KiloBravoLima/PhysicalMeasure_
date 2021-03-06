﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LogMeasurement;

namespace LogMeasurement.Migrations
{
    [DbContext(typeof(LoggingContext))]
    [Migration("20160804232957_CreateDatabaseInstanceMigration_2016_08_05_01_30")]
    partial class CreateDatabaseInstanceMigration_2016_08_05_01_30
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("LogMeasurement.InternalErrorDBItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ErrorMessage");

                    b.Property<DateTime>("EventTime");

                    b.Property<DateTime>("LogTime");

                    b.HasKey("Id");

                    b.ToTable("InternalErrors");
                });

            modelBuilder.Entity("LogMeasurement.MeasurementDBItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("MeasuredUnitId");

                    b.Property<double>("MeasuredValue");

                    b.HasKey("Id");

                    b.HasIndex("MeasuredUnitId");

                    b.ToTable("Measurements");
                });

            modelBuilder.Entity("LogMeasurement.UnitDBItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double?>("ConversionFactor");

                    b.Property<double?>("ConversionOffset");

                    b.Property<string>("Name");

                    b.Property<string>("Symbol");

                    b.HasKey("Id");

                    b.ToTable("Units");

                    b.HasDiscriminator().HasValue("UnitDBItem");
                });

            modelBuilder.Entity("LogMeasurement.UnitListItemDBItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ListId");

                    b.Property<int>("UnitId");

                    b.HasKey("Id");

                    b.ToTable("UnitListItems");
                });

            modelBuilder.Entity("LogMeasurement.MeasurementDBItem", b =>
                {
                    b.HasOne("LogMeasurement.UnitDBItem", "MeasuredUnit")
                        .WithMany()
                        .HasForeignKey("MeasuredUnitId")
                        .HasPrincipalKey("Id");
                });
        }
    }
}
