using System;
using Microsoft.EntityFrameworkCore.Migrations;

public partial class AddCreatedAtToFriendRequests : Migration
{
    protected override void Up( MigrationBuilder migrationBuilder )
    {
        // Læg kolonnen på med default i SQL Server
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAtUtc",
            table: "FriendRequests",
            type: "datetime2",
            nullable: false,
            defaultValueSql: "SYSUTCDATETIME()" );

        // (valgfrit) defensiv opdatering, hvis nogle rækker har "0001-01-01"
        migrationBuilder.Sql(
            "UPDATE dbo.FriendRequests SET CreatedAtUtc = SYSUTCDATETIME() WHERE CreatedAtUtc = '00010101'" );
    }

    protected override void Down( MigrationBuilder migrationBuilder )
    {
        // Dropper kolonnen (SQL Server fjerner evt. default constraint automatisk sammen med kolonnen)
        migrationBuilder.DropColumn(
            name: "CreatedAtUtc",
            table: "FriendRequests" );
    }
}
