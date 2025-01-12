using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USProApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddChildOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Orders",
                type: "TEXT",
                nullable: true,
                comment: "Идентификатор родительского договора");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ParentId",
                table: "Orders",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Orders_ParentId",
                table: "Orders",
                column: "ParentId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Orders_ParentId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ParentId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Orders");
        }
    }
}
