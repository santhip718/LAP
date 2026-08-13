using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "overall_score",
                schema: "LAP",
                table: "users",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "overall_score",
                schema: "LAP",
                table: "users");
        }
    }
}
