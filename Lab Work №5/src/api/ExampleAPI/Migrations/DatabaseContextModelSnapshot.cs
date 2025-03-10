﻿// <auto-generated />
using System;
using ExampleAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExampleAPI.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExampleAPI.Model.Filial", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("filials", (string)null);
                });

            modelBuilder.Entity("ExampleAPI.Model.HousingType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("housing_types", (string)null);
                });

            modelBuilder.Entity("ExampleAPI.Model.Settlement", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<int?>("FilialID")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("FilialID");

                    b.ToTable("settlements", (string)null);
                });

            modelBuilder.Entity("ExampleAPI.Model.Teo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("FilialID")
                        .HasColumnType("integer");

                    b.Property<int?>("HousingTypeID")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("FilialID");

                    b.HasIndex("HousingTypeID");

                    b.ToTable("teos", (string)null);
                });

            modelBuilder.Entity("SettlementTeo", b =>
                {
                    b.Property<int>("SettlementsID")
                        .HasColumnType("integer");

                    b.Property<int>("TeoID")
                        .HasColumnType("integer");

                    b.HasKey("SettlementsID", "TeoID");

                    b.HasIndex("TeoID");

                    b.ToTable("SettlementTeo");
                });

            modelBuilder.Entity("ExampleAPI.Model.Settlement", b =>
                {
                    b.HasOne("ExampleAPI.Model.Filial", "Filial")
                        .WithMany("Settlements")
                        .HasForeignKey("FilialID");

                    b.Navigation("Filial");
                });

            modelBuilder.Entity("ExampleAPI.Model.Teo", b =>
                {
                    b.HasOne("ExampleAPI.Model.Filial", "Filial")
                        .WithMany()
                        .HasForeignKey("FilialID");

                    b.HasOne("ExampleAPI.Model.HousingType", "HousingType")
                        .WithMany()
                        .HasForeignKey("HousingTypeID");

                    b.Navigation("Filial");

                    b.Navigation("HousingType");
                });

            modelBuilder.Entity("SettlementTeo", b =>
                {
                    b.HasOne("ExampleAPI.Model.Settlement", null)
                        .WithMany()
                        .HasForeignKey("SettlementsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExampleAPI.Model.Teo", null)
                        .WithMany()
                        .HasForeignKey("TeoID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ExampleAPI.Model.Filial", b =>
                {
                    b.Navigation("Settlements");
                });
#pragma warning restore 612, 618
        }
    }
}
