using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePoint.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQualificationToText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobseekerProfiles_Qualifications_QualificationID",
                table: "JobseekerProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "QualificationID",
                table: "JobseekerProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Qualification",
                table: "JobseekerProfiles",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_JobseekerProfiles_Qualifications_QualificationID",
                table: "JobseekerProfiles",
                column: "QualificationID",
                principalTable: "Qualifications",
                principalColumn: "QualificationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobseekerProfiles_Qualifications_QualificationID",
                table: "JobseekerProfiles");

            migrationBuilder.DropColumn(
                name: "Qualification",
                table: "JobseekerProfiles");

            migrationBuilder.AlterColumn<int>(
                name: "QualificationID",
                table: "JobseekerProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobseekerProfiles_Qualifications_QualificationID",
                table: "JobseekerProfiles",
                column: "QualificationID",
                principalTable: "Qualifications",
                principalColumn: "QualificationID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
