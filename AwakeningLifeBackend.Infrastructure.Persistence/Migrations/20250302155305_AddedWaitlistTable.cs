using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedWaitlistTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Waitlists",
                columns: table => new
                {
                    WaitlistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waitlists", x => x.WaitlistId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Waitlists");
        }
    }
}
