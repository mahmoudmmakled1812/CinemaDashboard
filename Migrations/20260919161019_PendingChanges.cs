using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaDashboard.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieActors_Actors_ActorId",
                table: "MovieActors");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieActors_Movies_MovieId",
                table: "MovieActors");

            migrationBuilder.DropTable(
                name: "ApplicationUserOTPs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieActors",
                table: "MovieActors");

            migrationBuilder.RenameTable(
                name: "MovieActors",
                newName: "MovieActor");

            migrationBuilder.RenameIndex(
                name: "IX_MovieActors_ActorId",
                table: "MovieActor",
                newName: "IX_MovieActor_ActorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieActor",
                table: "MovieActor",
                columns: new[] { "MovieId", "ActorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActor_Actors_ActorId",
                table: "MovieActor",
                column: "ActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActor_Movies_MovieId",
                table: "MovieActor",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieActor_Actors_ActorId",
                table: "MovieActor");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieActor_Movies_MovieId",
                table: "MovieActor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieActor",
                table: "MovieActor");

            migrationBuilder.RenameTable(
                name: "MovieActor",
                newName: "MovieActors");

            migrationBuilder.RenameIndex(
                name: "IX_MovieActor_ActorId",
                table: "MovieActors",
                newName: "IX_MovieActors_ActorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieActors",
                table: "MovieActors",
                columns: new[] { "MovieId", "ActorId" });

            migrationBuilder.CreateTable(
                name: "ApplicationUserOTPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActors_Actors_ActorId",
                table: "MovieActors",
                column: "ActorId",
                principalTable: "Actors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActors_Movies_MovieId",
                table: "MovieActors",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
