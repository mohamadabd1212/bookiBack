using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ruhanBack.Migrations
{
    /// <inheritdoc />
    public partial class Items : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rarity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CSFloatPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CSMoneyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Revenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BoughtAtPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BoughtAtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoldAtPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SoldAtDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
