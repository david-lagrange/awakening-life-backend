using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubscriptionCancelationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubscriptionCancelations",
                columns: table => new
                {
                    SubscriptionCancelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<string>(type: "text", nullable: false),
                    CancelationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionCancelations", x => x.SubscriptionCancelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionCancelations");
        }
    }
}
