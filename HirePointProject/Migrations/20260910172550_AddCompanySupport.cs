using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePoint.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Provinces_ProvinceID",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Company_CompanyID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Company_CompanyID",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");

            migrationBuilder.RenameIndex(
                name: "IX_Company_ProvinceID",
                table: "Companies",
                newName: "IX_Companies_ProvinceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Provinces_ProvinceID",
                table: "Companies",
                column: "ProvinceID",
                principalTable: "Provinces",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Companies_CompanyID",
                table: "Jobs",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyID",
                table: "Users",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Provinces_ProvinceID",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Companies_CompanyID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyID",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");

            migrationBuilder.RenameIndex(
                name: "IX_Companies_ProvinceID",
                table: "Company",
                newName: "IX_Company_ProvinceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Company",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Provinces_ProvinceID",
                table: "Company",
                column: "ProvinceID",
                principalTable: "Provinces",
                principalColumn: "ProvinceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Company_CompanyID",
                table: "Jobs",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Company_CompanyID",
                table: "Users",
                column: "CompanyID",
                principalTable: "Company",
                principalColumn: "CompanyID");
        }
    }
}
