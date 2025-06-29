using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCQInterviews.Migrations
{
    /// <inheritdoc />
    public partial class AddResponseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResponseTypeId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ResponseTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ResponseTypeId",
                table: "Questions",
                column: "ResponseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ResponseTypes_ResponseTypeId",
                table: "Questions",
                column: "ResponseTypeId",
                principalTable: "ResponseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ResponseTypes_ResponseTypeId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "ResponseTypes");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ResponseTypeId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ResponseTypeId",
                table: "Questions");
        }
    }
}
