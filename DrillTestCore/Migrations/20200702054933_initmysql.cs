using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DrillTestCore.Migrations
{
    public partial class initmysql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holerecs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SerialNo = table.Column<string>(nullable: true),
                    HoleNumber = table.Column<short>(nullable: false),
                    TestTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    MaxPressure = table.Column<float>(nullable: true),
                    MacId = table.Column<short>(nullable: true),
                    LayerNo = table.Column<short>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holerecs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workrecs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SerialNo = table.Column<string>(nullable: true),
                    Layer = table.Column<short>(nullable: false),
                    HoleCount = table.Column<short>(nullable: false),
                    LastTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workrecs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holerecs");

            migrationBuilder.DropTable(
                name: "Workrecs");
        }
    }
}
