using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupergirlShrine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReadingProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndYear",
                table: "Comics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastReadChapterId",
                table: "Comics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadDate",
                table: "Comics",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastReadPageNumber",
                table: "Comics",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndYear",
                table: "Comics");

            migrationBuilder.DropColumn(
                name: "LastReadChapterId",
                table: "Comics");

            migrationBuilder.DropColumn(
                name: "LastReadDate",
                table: "Comics");

            migrationBuilder.DropColumn(
                name: "LastReadPageNumber",
                table: "Comics");
        }
    }
}
