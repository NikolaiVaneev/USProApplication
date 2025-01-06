using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USProApplication.Migrations
{
    /// <inheritdoc />
    public partial class OrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Уникальный идентификатор записи."),
                    Name = table.Column<string>(type: "TEXT", nullable: true, comment: "Наименование"),
                    Address = table.Column<string>(type: "TEXT", nullable: true, comment: "Адрес"),
                    Number = table.Column<string>(type: "TEXT", nullable: true, comment: "Номер"),
                    Term = table.Column<int>(type: "INTEGER", nullable: true, comment: "Срок выполнения"),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: true, comment: "Дата договора"),
                    СompletionDate = table.Column<DateOnly>(type: "TEXT", nullable: true, comment: "Дата завершения"),
                    Price = table.Column<decimal>(type: "TEXT", nullable: true, comment: "Цена"),
                    PriceToMeter = table.Column<decimal>(type: "TEXT", nullable: true, comment: "Цена за метр"),
                    Square = table.Column<int>(type: "INTEGER", nullable: true, comment: "Площадь"),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "Завершено"),
                    Phone = table.Column<string>(type: "TEXT", nullable: true, comment: "Телефон"),
                    Email = table.Column<string>(type: "TEXT", nullable: true, comment: "Электронная почта"),
                    PrepaymentBillDate = table.Column<DateOnly>(type: "TEXT", nullable: true, comment: "Дата счета предоплаты"),
                    PrepaymentBillNumber = table.Column<string>(type: "TEXT", nullable: true, comment: "Номер счета предоплаты"),
                    PrepaymentPercent = table.Column<int>(type: "INTEGER", nullable: true, comment: "Процент предоплаты"),
                    ExecutionBillDate = table.Column<DateOnly>(type: "TEXT", nullable: true, comment: "Дата счета выполнения"),
                    ExecutionBillNumber = table.Column<string>(type: "TEXT", nullable: true, comment: "Номер счета выполнения"),
                    ExecutionPercent = table.Column<int>(type: "INTEGER", nullable: true, comment: "Процент оплаты выполнения"),
                    ApprovalBillDate = table.Column<DateOnly>(type: "TEXT", nullable: true, comment: "Дата счета согласования"),
                    ApprovalBillNumber = table.Column<string>(type: "TEXT", nullable: true, comment: "Номер счета согласования"),
                    ApprovalPercent = table.Column<int>(type: "INTEGER", nullable: true, comment: "Процент оплаты согласования"),
                    ExecutorId = table.Column<Guid>(type: "TEXT", nullable: true, comment: "Идентификатор исполнителя"),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: true, comment: "Идентификатор заказчика")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Counterparties_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Counterparties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Counterparties_ExecutorId",
                        column: x => x.ExecutorId,
                        principalTable: "Counterparties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Заказы");

            migrationBuilder.CreateTable(
                name: "OrderServices",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServiceId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderServices", x => new { x.OrderId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_OrderServices_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ExecutorId",
                table: "Orders",
                column: "ExecutorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderServices_ServiceId",
                table: "OrderServices",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderServices");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
