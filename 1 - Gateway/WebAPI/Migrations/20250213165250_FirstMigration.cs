using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Crud");

            migrationBuilder.DropColumn(
                name: "LojaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TutorId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VeterinarioId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxaFaturamento",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Alteracoes",
                columns: table => new
                {
                    IdAlteracao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataEHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomeTabela = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoOperacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresAntigos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNovos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alteracoes", x => x.IdAlteracao);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    IdCliente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Moeda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "Empresa",
                columns: table => new
                {
                    IdEmpresa = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.IdEmpresa);
                });

            migrationBuilder.CreateTable(
                name: "Habilidade",
                columns: table => new
                {
                    IdHabilidade = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilidade", x => x.IdHabilidade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    IdTag = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.IdTag);
                });

            migrationBuilder.CreateTable(
                name: "Projetos",
                columns: table => new
                {
                    IdProjetos = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEmpresa = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCliente = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrcamentoInicial = table.Column<int>(type: "int", nullable: false),
                    Gastos = table.Column<int>(type: "int", nullable: false),
                    SaldoFinal = table.Column<int>(type: "int", nullable: false),
                    Moeda = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorasEstimadas = table.Column<int>(type: "int", nullable: false),
                    HorasCobradas = table.Column<int>(type: "int", nullable: false),
                    HorasNaoCobradas = table.Column<int>(type: "int", nullable: false),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjetoConcluido = table.Column<bool>(type: "bit", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projetos", x => x.IdProjetos);
                    table.ForeignKey(
                        name: "FK_Projetos_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projetos_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioHabilidade",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdHabilidade = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioHabilidade", x => new { x.IdUsuario, x.IdHabilidade });
                    table.ForeignKey(
                        name: "FK_UsuarioHabilidade_Habilidade_IdHabilidade",
                        column: x => x.IdHabilidade,
                        principalTable: "Habilidade",
                        principalColumn: "IdHabilidade",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AtividadePai",
                columns: table => new
                {
                    IdAtividadePai = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorasEstimadas = table.Column<int>(type: "int", nullable: false),
                    HorasCobradas = table.Column<int>(type: "int", nullable: true),
                    HorasNaoCobradas = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadePai", x => x.IdAtividadePai);
                    table.ForeignKey(
                        name: "FK_AtividadePai_Projetos_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projetos",
                        principalColumn: "IdProjetos",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjetoUsuario",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjetoUsuario", x => new { x.IdUsuario, x.IdProjeto });
                    table.ForeignKey(
                        name: "FK_ProjetoUsuario_Projetos_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projetos",
                        principalColumn: "IdProjetos",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AtividadeFilho",
                columns: table => new
                {
                    IdAtividadeFilho = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAtividadePai = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Progresso = table.Column<int>(type: "int", nullable: false),
                    HorasEstimadas = table.Column<int>(type: "int", nullable: false),
                    HorasCobradas = table.Column<int>(type: "int", nullable: true),
                    HorasNaoCobradas = table.Column<int>(type: "int", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadeFilho", x => x.IdAtividadeFilho);
                    table.ForeignKey(
                        name: "FK_AtividadeFilho_AtividadePai_IdAtividadePai",
                        column: x => x.IdAtividadePai,
                        principalTable: "AtividadePai",
                        principalColumn: "IdAtividadePai",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Atividade",
                columns: table => new
                {
                    IdAtividade = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAtividadeFilho = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Progresso = table.Column<int>(type: "int", nullable: false),
                    HorasEstimadas = table.Column<int>(type: "int", nullable: false),
                    HorasTotais = table.Column<int>(type: "int", nullable: false),
                    Cobrado = table.Column<bool>(type: "bit", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividade", x => x.IdAtividade);
                    table.ForeignKey(
                        name: "FK_Atividade_AtividadeFilho_IdAtividadeFilho",
                        column: x => x.IdAtividadeFilho,
                        principalTable: "AtividadeFilho",
                        principalColumn: "IdAtividadeFilho",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lancamento",
                columns: table => new
                {
                    IdLancamento = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAtividadePai = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdAtividadeFilho = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    idTag = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorarioInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorarioFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Horas = table.Column<int>(type: "int", nullable: false),
                    Cobrado = table.Column<bool>(type: "bit", nullable: false),
                    Valor = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lancamento", x => x.IdLancamento);
                    table.ForeignKey(
                        name: "FK_Lancamento_AtividadeFilho_IdAtividadeFilho",
                        column: x => x.IdAtividadeFilho,
                        principalTable: "AtividadeFilho",
                        principalColumn: "IdAtividadeFilho",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lancamento_AtividadePai_IdAtividadePai",
                        column: x => x.IdAtividadePai,
                        principalTable: "AtividadePai",
                        principalColumn: "IdAtividadePai",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lancamento_Projetos_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projetos",
                        principalColumn: "IdProjetos",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lancamento_Tag_idTag",
                        column: x => x.idTag,
                        principalTable: "Tag",
                        principalColumn: "IdTag",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AtividadeUsuario",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAtividade = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AtividadeFilhoIdAtividadeFilho = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtividadeUsuario", x => new { x.IdUsuario, x.IdAtividade });
                    table.ForeignKey(
                        name: "FK_AtividadeUsuario_AtividadeFilho_AtividadeFilhoIdAtividadeFilho",
                        column: x => x.AtividadeFilhoIdAtividadeFilho,
                        principalTable: "AtividadeFilho",
                        principalColumn: "IdAtividadeFilho",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AtividadeUsuario_Atividade_IdAtividade",
                        column: x => x.IdAtividade,
                        principalTable: "Atividade",
                        principalColumn: "IdAtividade",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comentario",
                columns: table => new
                {
                    IdComentario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProjetos = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdAtividadePai = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdAtividadeFilho = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdAtividade = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdLancamento = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentario", x => x.IdComentario);
                    table.ForeignKey(
                        name: "FK_Comentario_AtividadeFilho_IdAtividadeFilho",
                        column: x => x.IdAtividadeFilho,
                        principalTable: "AtividadeFilho",
                        principalColumn: "IdAtividadeFilho",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentario_AtividadePai_IdAtividadePai",
                        column: x => x.IdAtividadePai,
                        principalTable: "AtividadePai",
                        principalColumn: "IdAtividadePai",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentario_Atividade_IdAtividade",
                        column: x => x.IdAtividade,
                        principalTable: "Atividade",
                        principalColumn: "IdAtividade",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentario_Projetos_IdProjetos",
                        column: x => x.IdProjetos,
                        principalTable: "Projetos",
                        principalColumn: "IdProjetos",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atividade_IdAtividadeFilho",
                table: "Atividade",
                column: "IdAtividadeFilho");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadeFilho_IdAtividadePai",
                table: "AtividadeFilho",
                column: "IdAtividadePai");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadePai_IdProjeto",
                table: "AtividadePai",
                column: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadeUsuario_AtividadeFilhoIdAtividadeFilho",
                table: "AtividadeUsuario",
                column: "AtividadeFilhoIdAtividadeFilho");

            migrationBuilder.CreateIndex(
                name: "IX_AtividadeUsuario_IdAtividade",
                table: "AtividadeUsuario",
                column: "IdAtividade");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_IdAtividade",
                table: "Comentario",
                column: "IdAtividade");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_IdAtividadeFilho",
                table: "Comentario",
                column: "IdAtividadeFilho");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_IdAtividadePai",
                table: "Comentario",
                column: "IdAtividadePai");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_IdProjetos",
                table: "Comentario",
                column: "IdProjetos");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_IdAtividadeFilho",
                table: "Lancamento",
                column: "IdAtividadeFilho");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_IdAtividadePai",
                table: "Lancamento",
                column: "IdAtividadePai");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_IdProjeto",
                table: "Lancamento",
                column: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_idTag",
                table: "Lancamento",
                column: "idTag");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_IdCliente",
                table: "Projetos",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_IdEmpresa",
                table: "Projetos",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_ProjetoUsuario_IdProjeto",
                table: "ProjetoUsuario",
                column: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioHabilidade_IdHabilidade",
                table: "UsuarioHabilidade",
                column: "IdHabilidade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alteracoes");

            migrationBuilder.DropTable(
                name: "AtividadeUsuario");

            migrationBuilder.DropTable(
                name: "Comentario");

            migrationBuilder.DropTable(
                name: "Lancamento");

            migrationBuilder.DropTable(
                name: "ProjetoUsuario");

            migrationBuilder.DropTable(
                name: "UsuarioHabilidade");

            migrationBuilder.DropTable(
                name: "Atividade");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Habilidade");

            migrationBuilder.DropTable(
                name: "AtividadeFilho");

            migrationBuilder.DropTable(
                name: "AtividadePai");

            migrationBuilder.DropTable(
                name: "Projetos");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Empresa");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TaxaFaturamento",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "LojaId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TutorId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VeterinarioId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Crud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Decimal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Integer = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MultiSelect = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Select = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crud", x => x.Id);
                });
        }
    }
}
