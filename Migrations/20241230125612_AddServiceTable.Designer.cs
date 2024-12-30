﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using USProApplication.DataBase;

#nullable disable

namespace USProApplication.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241230125612_AddServiceTable")]
    partial class AddServiceTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("USProApplication.DataBase.Entities.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasComment("Уникальный идентификатор записи.");

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasComment("Аббревиатура");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT")
                        .HasComment("Описание");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasComment("Наименование");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 2)
                        .HasColumnType("TEXT")
                        .HasComment("Cтоимость");

                    b.HasKey("Id");

                    b.ToTable("Services");
                });
#pragma warning restore 612, 618
        }
    }
}
