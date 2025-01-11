using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace USProApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalServiceColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalService",
                table: "Orders",
                type: "TEXT",
                nullable: true,
                comment: "Дополнительная услуга");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalService",
                table: "Orders");
        }
    }
}
