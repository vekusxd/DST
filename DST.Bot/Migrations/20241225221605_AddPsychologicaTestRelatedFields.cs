using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DST.Bot.Migrations
{
    /// <inheritdoc />
    public partial class AddPsychologicaTestRelatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DialogStateId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 11,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PsychologicalTestPoints",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PsychologicalType",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PsychologicalTestPoints",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PsychologicalType",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "DialogStateId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 11);
        }
    }
}
