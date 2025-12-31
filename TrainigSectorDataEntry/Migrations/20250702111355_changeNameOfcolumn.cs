using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainigSectorDataEntry.Migrations
{
    /// <inheritdoc />
    public partial class changeNameOfcolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmpPhotoPath",
                table: "Employees",
                newName: "PhotoPath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoPath",
                table: "Employees",
                newName: "EmpPhotoPath");
        }
    }
}
