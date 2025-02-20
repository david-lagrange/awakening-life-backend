using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesIpRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5be06b26-d163-4027-84b9-31155c61393b", null, "View IP", "VIEW IP" },
                    { "6345c99d-8cce-45f1-a876-8a99fe91cd94", null, "View Roles", "VIEW ROLES" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "5be06b26-d163-4027-84b9-31155c61393b", "c78cca8c-efa3-4524-ae21-98904dadf303" },
                    { "6345c99d-8cce-45f1-a876-8a99fe91cd94", "c78cca8c-efa3-4524-ae21-98904dadf303" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5be06b26-d163-4027-84b9-31155c61393b", "c78cca8c-efa3-4524-ae21-98904dadf303" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "6345c99d-8cce-45f1-a876-8a99fe91cd94", "c78cca8c-efa3-4524-ae21-98904dadf303" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5be06b26-d163-4027-84b9-31155c61393b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6345c99d-8cce-45f1-a876-8a99fe91cd94");
        }
    }
}
