using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FeatureRolesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "03b4f3a4-d2a6-4e6f-86c1-d5a0b8d027f7", null, "Gratitude & Goals", "GRATITUDE & GOALS" },
                    { "310071fe-00bc-41f1-9161-10a9879fa91c", null, "Deepest Vision", "DEEPEST VISION" },
                    { "6bdc6e0c-dc95-4e90-8f4b-50bf684dff71", null, "Mind Clearing", "MIND CLEARING" },
                    { "902df002-5302-4055-a1c7-39bcdb14ea40", null, "Custom Journey", "CUSTOM JOURNEY" },
                    { "95746f8b-6570-4b53-914f-2d5639be416e", null, "Manifestation", "MANIFESTATION" },
                    { "b9b0109e-a82f-4934-a12c-dd1ba0c87f04", null, "Contemplation", "CONTEMPLATION" },
                    { "edda87cb-fe80-4b82-9a74-8e992f049423", null, "Technique Training", "TECHNIQUE TRAINING" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "03b4f3a4-d2a6-4e6f-86c1-d5a0b8d027f7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "310071fe-00bc-41f1-9161-10a9879fa91c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6bdc6e0c-dc95-4e90-8f4b-50bf684dff71");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "902df002-5302-4055-a1c7-39bcdb14ea40");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95746f8b-6570-4b53-914f-2d5639be416e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b9b0109e-a82f-4934-a12c-dd1ba0c87f04");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "edda87cb-fe80-4b82-9a74-8e992f049423");
        }
    }
}
