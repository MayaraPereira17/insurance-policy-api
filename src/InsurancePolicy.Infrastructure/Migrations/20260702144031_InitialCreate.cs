using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsurancePolicy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apolices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NumeroApolice = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CpfCnpjSegurado = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    PlacaVeiculo = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    ValorPremio = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DataInicioVigencia = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DataFimVigencia = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apolices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apolices_NumeroApolice",
                table: "Apolices",
                column: "NumeroApolice",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Apolices");
        }
    }
}
