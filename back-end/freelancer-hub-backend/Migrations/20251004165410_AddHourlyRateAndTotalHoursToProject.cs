using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace freelancer_hub_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddHourlyRateAndTotalHoursToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalHours",
                table: "Projects",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TotalHours",
                table: "Projects");
        }
    }
}
