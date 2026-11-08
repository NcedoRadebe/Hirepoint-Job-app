using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePoint.Migrations
{
    /// <inheritdoc />
    public partial class AddRepresentativeVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RepresentativeVerificationExpiresAt",
                table: "RecruiterProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeVerificationStatus",
                table: "RecruiterProfiles",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeVerificationTokenHash",
                table: "RecruiterProfiles",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepresentativeVerificationExpiresAt",
                table: "RecruiterProfiles");

            migrationBuilder.DropColumn(
                name: "RepresentativeVerificationStatus",
                table: "RecruiterProfiles");

            migrationBuilder.DropColumn(
                name: "RepresentativeVerificationTokenHash",
                table: "RecruiterProfiles");
        }
    }
}
