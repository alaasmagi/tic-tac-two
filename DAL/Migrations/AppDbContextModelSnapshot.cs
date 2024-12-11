﻿// <auto-generated />
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Domain.ConfigurationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("GameConfigName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SerializedJsonString")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Configurations", (string)null);
                });

            modelBuilder.Entity("Domain.SaveGameEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameMode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlayerAName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PlayerBName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SaveGameName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SerializedJsonString")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SaveGames", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
