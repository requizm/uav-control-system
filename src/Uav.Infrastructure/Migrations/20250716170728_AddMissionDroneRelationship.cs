using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uav.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissionDroneRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drones_CurrentMissionId",
                table: "Drones");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedDroneId",
                table: "Missions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drones_CurrentMissionId",
                table: "Drones",
                column: "CurrentMissionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Drones_CurrentMissionId",
                table: "Drones");

            migrationBuilder.DropColumn(
                name: "AssignedDroneId",
                table: "Missions");

            migrationBuilder.CreateIndex(
                name: "IX_Drones_CurrentMissionId",
                table: "Drones",
                column: "CurrentMissionId");
        }
    }
}
