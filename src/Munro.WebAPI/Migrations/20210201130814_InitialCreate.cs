using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventManager.WebAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Jobs",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>("TEXT", nullable: true),
                    UserName = table.Column<string>("TEXT", nullable: true),
                    Data = table.Column<string>("TEXT", nullable: true),
                    TimeStamp = table.Column<DateTime>("TEXT", nullable: false),
                    Duration = table.Column<long>("INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>("INTEGER", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Jobs", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Jobs");
        }
    }
}