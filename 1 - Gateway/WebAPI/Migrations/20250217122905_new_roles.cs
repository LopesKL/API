using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class new_roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "373C7921-DF59-4D99-9827-521F7FBC97B6", null, "AppRole", "RoleProjectManager", "ROLEPROJECTMANAGER" },
                    { "B47591C2-1033-4543-B400-83B83C63B1BD", null, "AppRole", "RoleUser", "ROLEUSER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "373C7921-DF59-4D99-9827-521F7FBC97B6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B47591C2-1033-4543-B400-83B83C63B1BD");
        }
    }
}
