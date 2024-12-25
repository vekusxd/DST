using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DST.Bot.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DialogStateId = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "FrontPageData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Course = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Profile = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Theme = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Initials = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Group = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    SupervisorInitials = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SupervisorAcademicTitle = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SupervisorAcademicDegree = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SupervisorJobTitle = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontPageData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrontPageData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FrontPageData_UserId",
                table: "FrontPageData",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrontPageData");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
