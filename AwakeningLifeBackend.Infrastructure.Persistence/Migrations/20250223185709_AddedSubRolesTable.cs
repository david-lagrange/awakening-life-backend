using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubRolesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionRoles",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionRoles", x => new { x.ProductId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_SubscriptionRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionRoles_RoleId",
                table: "SubscriptionRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionRoles");
        }
    }
}
