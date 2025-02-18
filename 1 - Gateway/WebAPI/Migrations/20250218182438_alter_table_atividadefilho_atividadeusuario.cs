using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class alter_table_atividadefilho_atividadeusuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorasCobradas",
                table: "AtividadeFilho");

            migrationBuilder.DropColumn(
                name: "HorasNaoCobradas",
                table: "AtividadeFilho");

            migrationBuilder.DropColumn(
                name: "Progresso",
                table: "AtividadeFilho");

            migrationBuilder.AlterColumn<bool>(
                name: "estrela",
                table: "AtividadeUsuario",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "estrela",
                table: "AtividadeUsuario",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HorasCobradas",
                table: "AtividadeFilho",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HorasNaoCobradas",
                table: "AtividadeFilho",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Progresso",
                table: "AtividadeFilho",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
