using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePoint.Migrations
{
    /// <inheritdoc />
    public partial class AddQualifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Company_CompanyID",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
            name: "QualificationID",
            table: "JobseekerProfiles",
            type: "int",
            nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Jobs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "QualificationID",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Jobs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Qualifications",
                columns: table => new
                {
                    QualificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Qualifications", x => x.QualificationID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobseekerProfiles_QualificationID",
                table: "JobseekerProfiles",
                column: "QualificationID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_QualificationID",
                table: "Jobs",
                column: "QualificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Company_CompanyID",
                table: "Jobs",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Qualifications_QualificationID",
                table: "Jobs",
                column: "QualificationID",
                principalTable: "Qualifications",
                principalColumn: "QualificationID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobseekerProfiles_Qualifications_QualificationID",
                table: "JobseekerProfiles",
                column: "QualificationID",
                principalTable: "Qualifications",
                principalColumn: "QualificationID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Company_CompanyID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Qualifications_QualificationID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobseekerProfiles_Qualifications_QualificationID",
                table: "JobseekerProfiles");

            migrationBuilder.DropTable(
                name: "Qualifications");

            migrationBuilder.DropIndex(
                name: "IX_JobseekerProfiles_QualificationID",
                table: "JobseekerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_QualificationID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "QualificationID",
                table: "JobseekerProfiles");

            migrationBuilder.DropColumn(
                name: "QualificationID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Jobs");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Company_CompanyID",
                table: "Jobs",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
