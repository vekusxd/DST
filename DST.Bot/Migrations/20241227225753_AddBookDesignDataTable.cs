using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DST.Bot.Migrations
{
    /// <inheritdoc />
    public partial class AddBookDesignDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookDesignData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthorSurname = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    AuthorInitials = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    BookTitle = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PublicationPlace = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Publisher = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    YearOfPublication = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfPages = table.Column<int>(type: "INTEGER", nullable: false),
                    PublicationDetails = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Isbn = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookDesignData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookDesignData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookDesignData_UserId",
                table: "BookDesignData",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookDesignData");
        }
    }
}
