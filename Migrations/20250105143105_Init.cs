using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USProApplication.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Counterparties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Уникальный идентификатор записи."),
                    Name = table.Column<string>(type: "TEXT", nullable: false, comment: "Наименование"),
                    INN = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true, comment: "ИНН"),
                    OGRN = table.Column<string>(type: "TEXT", maxLength: 13, nullable: true, comment: "ОГРН"),
                    KPP = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true, comment: "КПП"),
                    Address = table.Column<string>(type: "TEXT", nullable: true, comment: "Адрес"),
                    PaymentAccount = table.Column<string>(type: "TEXT", nullable: true, comment: "Номер счета"),
                    Bank = table.Column<string>(type: "TEXT", nullable: true, comment: "Банк"),
                    BIK = table.Column<string>(type: "TEXT", maxLength: 9, nullable: true, comment: "Банковский идентификационный код"),
                    CorrAccount = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true, comment: "Корреспондентский счет"),
                    Director = table.Column<string>(type: "TEXT", nullable: true, comment: "Руководитель"),
                    DirectorPosition = table.Column<int>(type: "INTEGER", nullable: false, comment: "Должность руководителя"),
                    Executor = table.Column<bool>(type: "INTEGER", nullable: false, comment: "Является исполнителем")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counterparties", x => x.Id);
                },
                comment: "Контрагенты");

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Уникальный идентификатор записи."),
                    Name = table.Column<string>(type: "TEXT", nullable: false, comment: "Наименование"),
                    Abbreviation = table.Column<string>(type: "TEXT", nullable: false, comment: "Аббревиатура"),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false, comment: "Cтоимость"),
                    Description = table.Column<string>(type: "TEXT", nullable: true, comment: "Описание")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            // Добавление данных в таблицу Services
            migrationBuilder.InsertData(
                table: "Services",
                columns: ["Id", "Name", "Abbreviation", "Price"],
                values: new object[,]
                {
                    { Guid.Parse("a7e16db6-0a9b-4378-bf59-abdf2befad5d"), "Архитектурно-планировочные решения", "АР", 0.00m },
                    { Guid.Parse("b2f8e87d-c05a-4b83-b1ef-2a655b0b5cdd"), "Отопление, вентиляция и кондиционирование", "ОВиК", 0.00m },
                    { Guid.Parse("c5a02d9b-7eb3-4426-b4a9-65d19d3adabc"), "Электрооборудование и электроосвещение", "ЭОМ", 0.00m },
                    { Guid.Parse("d9b7e8ab-6c94-4d9a-a3d1-579bc44a725b"), "Водоснабжение и канализация", "ВК", 0.00m },
                    { Guid.Parse("e8abf5a3-4827-47a1-bd32-a34bcb321fbc"), "Структурированная кабельная система", "СКС", 0.00m },
                    { Guid.Parse("f6b2cd2a-8f2d-4539-9a8c-536df6a2560d"), "Видеонаблюдение", "В", 0.00m },
                    { Guid.Parse("08c174a6-54b3-4b37-8fd7-d93f3b91a4e2"), "Система контроля и управление доступом", "СКУД", 0.00m },
                    { Guid.Parse("13c8e596-80a1-4fbc-b4c2-c972a1be063f"), "Автоматическая пожарная сигнализация", "АПС", 0.00m },
                    { Guid.Parse("207b939d-f86f-4207-bc2a-d456c7c47aef"), "Система оповещения и управления эвакуацией", "СОУЭ", 0.00m },
                    { Guid.Parse("326dab1f-9281-4e7c-a29d-24e816c5ae14"), "Автоматическое пожаротушение", "АПТ", 0.00m },
                    { Guid.Parse("417bfc5b-7de3-4783-8d63-951cd7f46d7c"), "Газовое пожаротушение", "ГП", 0.00m },
                    { Guid.Parse("51d63a17-5d64-4c37-9017-8bd7e1a8a54c"), "Сети связи", "СС", 0.00m },
                    { Guid.Parse("623afc8d-6a3e-4c79-8c56-9bfed7f41ba9"), "Конструктивные решения", "КР", 0.00m },
                    { Guid.Parse("72bfecad-9278-44ab-a379-78f0bd2c1945"), "Расчет аварийного освещения", "РАО", 0.00m },
                    { Guid.Parse("83cdbe1b-a94d-4b18-a17d-2a8f5e3c65d2"), "Расчет общего освещения", "РО", 0.00m },
                    { Guid.Parse("94d6e3cf-5b8d-4c83-b8b2-94e6cd4e3b5f"), "Технологические решения", "ТХ", 0.00m },
                    { Guid.Parse("a5c7de84-7b7d-4a59-bcd9-b6f8c7d4e3ab"), "Визуализация", "ВИЗ", 0.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Counterparties");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
