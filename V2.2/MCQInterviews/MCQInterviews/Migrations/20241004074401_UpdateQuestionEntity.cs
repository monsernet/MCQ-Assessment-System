using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCQInterviews.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuestionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioResponseType",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoResponseType",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioResponseType",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "VideoResponseType",
                table: "Questions");
        }
    }
}
