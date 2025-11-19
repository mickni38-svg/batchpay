using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultOnCreatedAtUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.Sql( @"
IF COL_LENGTH('dbo.FriendRequests','CreatedAtUtc') IS NULL
BEGIN
    ALTER TABLE dbo.FriendRequests
      ADD [CreatedAtUtc] datetime2 NOT NULL
          CONSTRAINT DF_FriendRequests_CreatedAtUtc DEFAULT (SYSUTCDATETIME());
END
" );
        }

        protected override void Down( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.Sql( @"
IF COL_LENGTH('dbo.FriendRequests','CreatedAtUtc') IS NOT NULL
BEGIN
    DECLARE @df nvarchar(128);
    SELECT @df = d.name
    FROM sys.default_constraints d
    JOIN sys.columns c ON c.default_object_id = d.object_id
    WHERE d.parent_object_id = OBJECT_ID('dbo.FriendRequests') AND c.name = 'CreatedAtUtc';

    IF @df IS NOT NULL EXEC('ALTER TABLE dbo.FriendRequests DROP CONSTRAINT ' + QUOTENAME(@df));
    ALTER TABLE dbo.FriendRequests DROP COLUMN [CreatedAtUtc];
END
" );
        }

    }
}
