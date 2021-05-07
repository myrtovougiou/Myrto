using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeesDemographics.API.Migrations
{
    public partial class start : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Surname = table.Column<string>(nullable: false),
                    HiringDate = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<string>(nullable: false),
                    Age = table.Column<int>(nullable: false),
                    Ethnicity = table.Column<string>(nullable: false),
                    Education = table.Column<string>(nullable: false),
                    MaritalStatus = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
