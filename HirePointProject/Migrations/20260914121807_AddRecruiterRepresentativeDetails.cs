using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePoint.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruiterRepresentativeDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RepresentativeEmail",
                table: "RecruiterProfiles",
                type: "nvarchar(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeName",
                table: "RecruiterProfiles",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepresentativeEmail",
                table: "RecruiterProfiles");

            migrationBuilder.DropColumn(
                name: "RepresentativeName",
                table: "RecruiterProfiles");
        }
    }
}
