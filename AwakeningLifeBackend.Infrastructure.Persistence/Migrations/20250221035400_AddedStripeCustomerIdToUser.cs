using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedStripeCustomerIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c78cca8c-efa3-4524-ae21-98904dadf303",
                column: "StripeCustomerId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "AspNetUsers");
        }
    }
}
