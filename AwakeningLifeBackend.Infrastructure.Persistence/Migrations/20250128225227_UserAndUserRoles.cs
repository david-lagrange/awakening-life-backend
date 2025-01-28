using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserAndUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c78cca8c-efa3-4524-ae21-98904dadf303", 0, "b8baa1c8-b7eb-4909-bbd5-a3e305a4381e", "admin@equanimity-solutions.com", true, "Equanimity", "Admin", false, null, "ADMIN@EQUANIMITY-SOLUTIONS.COM", "EQUANIMITY-ADMIN", "AQAAAAIAAYagAAAAEBabxi2j4ViaUyJTawKOoncZJcJUvtnGSMQStB+D9Qqz8Yc/HfbNvCIW6FUvDM2tPw==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "388fe995-3763-4c99-848b-8699c3131d0d", false, "equanimity-admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1fca31b0-e385-46b3-98f2-13f47864589b", "c78cca8c-efa3-4524-ae21-98904dadf303" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1fca31b0-e385-46b3-98f2-13f47864589b", "c78cca8c-efa3-4524-ae21-98904dadf303" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c78cca8c-efa3-4524-ae21-98904dadf303");
        }
    }
}
