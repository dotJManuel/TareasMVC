using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class SubtareasYPapelera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Tareas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TareaPadreId",
                table: "Tareas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_TareaPadreId",
                table: "Tareas",
                column: "TareaPadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_Tareas_TareaPadreId",
                table: "Tareas",
                column: "TareaPadreId",
                principalTable: "Tareas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_Tareas_TareaPadreId",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_TareaPadreId",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "TareaPadreId",
                table: "Tareas");
        }
    }
}
