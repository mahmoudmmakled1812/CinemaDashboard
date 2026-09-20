using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using CinemaDashboard.Data;

#nullable disable

namespace CinemaDashboard.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260920222000_RestoreApplicationUserOTP")]
public partial class RestoreApplicationUserOTP : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ApplicationUserOTPs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                OTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsUsed = table.Column<bool>(type: "bit", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApplicationUserOTPs", x => x.Id);
                table.ForeignKey(
                    name: "FK_ApplicationUserOTPs_AspNetUsers_ApplicationUserId",
                    column: x => x.ApplicationUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ApplicationUserOTPs_ApplicationUserId",
            table: "ApplicationUserOTPs",
            column: "ApplicationUserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ApplicationUserOTPs");
    }
}
