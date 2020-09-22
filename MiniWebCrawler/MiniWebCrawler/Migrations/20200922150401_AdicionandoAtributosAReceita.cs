using Microsoft.EntityFrameworkCore.Migrations;

namespace MiniWebCrawler.Migrations
{
    public partial class AdicionandoAtributosAReceita : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "Receita",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Porcao",
                table: "Receita",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tempo",
                table: "Receita",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Likes",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "Porcao",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "Tempo",
                table: "Receita");
        }
    }
}
