using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VacationsModule.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("c6f97274-d239-444a-b96e-6d5f3fa97a1d"), new Guid("8e445865-a24d-4543-a6c6-9443d048cdb9") });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("8e445865-a24d-4543-a6c6-9443d048cdb9"),
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash" },
                values: new object[] { "314ba4db-4947-4349-af0f-49b9aa0e3e4d", "MANAGERUSER01", "AQAAAAIAAYagAAAAEDOsffpKAN6tSQP+0Qahiv8ZtZhI3fu1SR93jZzFioa7aolQ5IiVcQISqOnEpgUi9g==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("c6f97274-d239-444a-b96e-6d5f3fa97a1d"), new Guid("8e445865-a24d-4543-a6c6-9443d048cdb9") });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("8e445865-a24d-4543-a6c6-9443d048cdb9"),
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash" },
                values: new object[] { "be9c842d-845a-4450-b1e3-c1fb78b05e59", "manageruser01", "AQAAAAIAAYagAAAAEGjN4sGQ14RHO/8YcsYQ9aUzLGL8/vVS4LbpRigIF9cXmXk4UELr4eFlFxKG2rJ3Ug==" });
        }
    }
}
