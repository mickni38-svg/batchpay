using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

public partial class RemoveRequesterType : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RequesterType",
            table: "FriendRequests");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte>(
            name: "RequesterType",
            table: "FriendRequests",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);
    }
}