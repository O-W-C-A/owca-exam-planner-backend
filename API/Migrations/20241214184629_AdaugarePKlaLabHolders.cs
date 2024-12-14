using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AdaugarePKlaLabHolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LabHolders",
                table: "LabHolders");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LabHolders",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabHolders",
                table: "LabHolders",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LabHolders_CourseID",
                table: "LabHolders",
                column: "CourseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LabHolders",
                table: "LabHolders");

            migrationBuilder.DropIndex(
                name: "IX_LabHolders_CourseID",
                table: "LabHolders");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LabHolders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabHolders",
                table: "LabHolders",
                columns: new[] { "CourseID", "ProfessorID" });
        }
    }
}
