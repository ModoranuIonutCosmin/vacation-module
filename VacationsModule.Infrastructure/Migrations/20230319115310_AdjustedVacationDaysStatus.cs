using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacationsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdjustedVacationDaysStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VacationDays",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "VacationRequests",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "VacationRequests",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "VacationDaysStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeftVacationDays = table.Column<int>(type: "int", nullable: false),
                    TotalVacationDays = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    YearStartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    YearEndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationDaysStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacationDaysStatus_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacationDaysStatus_EmployeeId",
                table: "VacationDaysStatus",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacationDaysStatus");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "VacationRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "VacationRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddColumn<int>(
                name: "VacationDays",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
