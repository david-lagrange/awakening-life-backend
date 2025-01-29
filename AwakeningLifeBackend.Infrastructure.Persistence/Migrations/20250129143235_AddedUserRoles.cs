using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44a35cf3-f804-483a-b9f1-a7ad7af9c8b6");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "62227b59-0b0f-43a3-834e-5e2cd3a60ea7", null, "Modify Users", "MODIFY USERS" },
                    { "77e345c0-86e5-4aa6-9744-54b83db0b53d", null, "View Users", "VIEW USERS" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c78cca8c-efa3-4524-ae21-98904dadf303",
                column: "IsDeleted",
                value: false);

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "62227b59-0b0f-43a3-834e-5e2cd3a60ea7", "c78cca8c-efa3-4524-ae21-98904dadf303" },
                    { "77e345c0-86e5-4aa6-9744-54b83db0b53d", "c78cca8c-efa3-4524-ae21-98904dadf303" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "62227b59-0b0f-43a3-834e-5e2cd3a60ea7", "c78cca8c-efa3-4524-ae21-98904dadf303" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "77e345c0-86e5-4aa6-9744-54b83db0b53d", "c78cca8c-efa3-4524-ae21-98904dadf303" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "62227b59-0b0f-43a3-834e-5e2cd3a60ea7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "77e345c0-86e5-4aa6-9744-54b83db0b53d");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "44a35cf3-f804-483a-b9f1-a7ad7af9c8b6", null, "Manager", "MANAGER" });
        }
    }
}
