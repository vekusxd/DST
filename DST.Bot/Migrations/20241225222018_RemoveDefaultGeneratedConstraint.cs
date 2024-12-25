using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DST.Bot.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDefaultGeneratedConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DialogStateId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 11);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DialogStateId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 11,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
