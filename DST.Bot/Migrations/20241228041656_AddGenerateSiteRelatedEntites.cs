using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DST.Bot.Migrations
{
    /// <inheritdoc />
    public partial class AddGenerateSiteRelatedEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteArticleData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SiteTitle = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteArticleData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteArticleData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SiteArticleData_UserId",
                table: "SiteArticleData",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteData_UserId",
                table: "SiteData",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteArticleData");

            migrationBuilder.DropTable(
                name: "SiteData");
        }
    }
}
