using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USProApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddСounterpartiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Сounterparties",
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
                    table.PrimaryKey("PK_Сounterparties", x => x.Id);
                },
                comment: "Контрагенты");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Сounterparties");
        }
    }
}
