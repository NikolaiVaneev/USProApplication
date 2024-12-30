﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using USProApplication.DataBase;

#nullable disable

namespace USProApplication.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("USProApplication.DataBase.Entities.Сounterparty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasComment("Уникальный идентификатор записи.");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT")
                        .HasComment("Адрес");

                    b.Property<string>("BIK")
                        .HasMaxLength(9)
                        .HasColumnType("TEXT")
                        .HasComment("Банковский идентификационный код");

                    b.Property<string>("Bank")
                        .HasColumnType("TEXT")
                        .HasComment("Банк");

                    b.Property<string>("CorrAccount")
                        .HasMaxLength(20)
                        .HasColumnType("TEXT")
                        .HasComment("Корреспондентский счет");

                    b.Property<string>("Director")
                        .HasColumnType("TEXT")
                        .HasComment("Руководитель");

                    b.Property<int>("DirectorPosition")
                        .HasColumnType("INTEGER")
                        .HasComment("Должность руководителя");

                    b.Property<bool>("Executor")
                        .HasColumnType("INTEGER")
                        .HasComment("Является исполнителем");

                    b.Property<string>("INN")
                        .HasMaxLength(10)
                        .HasColumnType("TEXT")
                        .HasComment("ИНН");

                    b.Property<string>("KPP")
                        .HasMaxLength(9)
                        .HasColumnType("TEXT")
                        .HasComment("КПП");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasComment("Наименование");

                    b.Property<string>("OGRN")
                        .HasMaxLength(13)
                        .HasColumnType("TEXT")
                        .HasComment("ОГРН");

                    b.Property<string>("PaymentAccount")
                        .HasColumnType("TEXT")
                        .HasComment("Номер счета");

                    b.HasKey("Id");

                    b.ToTable("Сounterparties", t =>
                        {
                            t.HasComment("Контрагенты");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
