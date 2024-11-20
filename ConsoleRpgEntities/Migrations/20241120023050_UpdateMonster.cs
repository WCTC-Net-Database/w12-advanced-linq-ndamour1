using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    public partial class UpdateMonster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "Monsters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_EquipmentId",
                table: "Monsters",
                column: "EquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Monsters_Equipments_EquipmentId",
                table: "Monsters",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monsters_Equipments_EquipmentId",
                table: "Monsters");

            migrationBuilder.DropIndex(
                name: "IX_Monsters_EquipmentId",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Monsters");
        }
    }
}
