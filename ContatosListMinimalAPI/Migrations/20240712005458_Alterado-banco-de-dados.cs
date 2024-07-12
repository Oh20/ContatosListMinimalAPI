using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContatosListMinimalAPI.Migrations
{
    /// <inheritdoc />
    public partial class Alteradobancodedados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contatos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contatos_Nome",
                table: "Contatos",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contatos_Telefone",
                table: "Contatos",
                column: "Telefone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contatos");
        }
    }
}
