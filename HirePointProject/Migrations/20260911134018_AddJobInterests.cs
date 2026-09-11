using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HirePoint.Migrations
{
    /// <inheritdoc />
    public partial class AddJobInterests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobInterests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobInterests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobseekerInterests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobseekerId = table.Column<int>(type: "int", nullable: false),
                    JobInterestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobseekerInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobseekerInterests_JobInterests_JobInterestId",
                        column: x => x.JobInterestId,
                        principalTable: "JobInterests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobseekerInterests_Users_JobseekerId",
                        column: x => x.JobseekerId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "JobInterests",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Software Developer" },
                    { 2, "Web Developer" },
                    { 3, "Mobile App Developer" },
                    { 4, "Data Analyst" },
                    { 5, "Database Administrator" },
                    { 6, "Network Administrator" },
                    { 7, "Cybersecurity" },
                    { 8, "IT Support" },
                    { 9, "Graphic Designer" },
                    { 10, "UI/UX Designer" },
                    { 11, "Accountant" },
                    { 12, "Human Resources" },
                    { 13, "Marketing" },
                    { 14, "Sales" },
                    { 15, "Project Manager" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobInterests_Name",
                table: "JobInterests",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobseekerInterests_JobInterestId",
                table: "JobseekerInterests",
                column: "JobInterestId");

            migrationBuilder.CreateIndex(
                name: "IX_JobseekerInterests_JobseekerId_JobInterestId",
                table: "JobseekerInterests",
                columns: new[] { "JobseekerId", "JobInterestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobseekerInterests");

            migrationBuilder.DropTable(
                name: "JobInterests");
        }
    }
}
