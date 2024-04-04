using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakery.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewDeliveryDate",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
            
            migrationBuilder.Sql("UPDATE [Order] SET NewDeliveryDate = FORMAT(DeliveryDate, 'ddMMyyyy HHmm', 'en-US')");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Order");
            
            migrationBuilder.RenameColumn(
                name: "NewDeliveryDate",
                table: "Order",
                newName: "DeliveryDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "Order",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
