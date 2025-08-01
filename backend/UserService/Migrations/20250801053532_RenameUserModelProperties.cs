using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserModelProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Registration_date",
                table: "UserModel",
                newName: "RegistrationDate");

            migrationBuilder.RenameColumn(
                name: "Last_name",
                table: "UserModel",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "First_name",
                table: "UserModel",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "User_id",
                table: "UserModel",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "UserModel",
                newName: "Registration_date");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "UserModel",
                newName: "Last_name");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "UserModel",
                newName: "First_name");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserModel",
                newName: "User_id");
        }
    }
}
