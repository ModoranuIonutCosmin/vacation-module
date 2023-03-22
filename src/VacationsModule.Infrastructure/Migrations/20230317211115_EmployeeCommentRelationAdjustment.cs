using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacationsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeCommentRelationAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_VacationRequests_VacationRequestId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_VacationRequests_AspNetUsers_EmployeeId",
                table: "VacationRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "VacationRequestId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_VacationRequests_VacationRequestId",
                table: "Comments",
                column: "VacationRequestId",
                principalTable: "VacationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VacationRequests_AspNetUsers_EmployeeId",
                table: "VacationRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_VacationRequests_VacationRequestId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_VacationRequests_AspNetUsers_EmployeeId",
                table: "VacationRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "VacationRequestId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_VacationRequests_VacationRequestId",
                table: "Comments",
                column: "VacationRequestId",
                principalTable: "VacationRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationRequests_AspNetUsers_EmployeeId",
                table: "VacationRequests",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
