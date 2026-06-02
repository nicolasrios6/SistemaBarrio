using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaBarrio.Migrations
{
    /// <inheritdoc />
    public partial class AddAutorizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DomicilioId",
                table: "Autorizaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoAutorizacion",
                table: "Autorizaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Autorizaciones_DomicilioId",
                table: "Autorizaciones",
                column: "DomicilioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Autorizaciones_Domicilios_DomicilioId",
                table: "Autorizaciones",
                column: "DomicilioId",
                principalTable: "Domicilios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Autorizaciones_Domicilios_DomicilioId",
                table: "Autorizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Autorizaciones_DomicilioId",
                table: "Autorizaciones");

            migrationBuilder.DropColumn(
                name: "DomicilioId",
                table: "Autorizaciones");

            migrationBuilder.DropColumn(
                name: "TipoAutorizacion",
                table: "Autorizaciones");
        }
    }
}
