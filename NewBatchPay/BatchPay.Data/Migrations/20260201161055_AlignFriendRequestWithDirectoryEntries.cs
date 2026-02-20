using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignFriendRequestWithDirectoryEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up( MigrationBuilder migrationBuilder )
        {
            // Ensure the table name is pluralized once (old schema used singular)
            migrationBuilder.Sql( """
IF OBJECT_ID(N'dbo.FriendRequest', N'U') IS NOT NULL
    EXEC sp_rename N'dbo.FriendRequest', N'FriendRequests';
""" );

            // Rename FK columns only if the legacy names still exist
            migrationBuilder.Sql( """
IF COL_LENGTH('dbo.FriendRequests','RequesterUserId') IS NOT NULL
    EXEC sp_rename N'dbo.FriendRequests.RequesterUserId', N'RequesterId', N'COLUMN';
""" );

            migrationBuilder.Sql( """
IF COL_LENGTH('dbo.FriendRequests','ReceiverUserId') IS NOT NULL
    EXEC sp_rename N'dbo.FriendRequests.ReceiverUserId', N'ReceiverId', N'COLUMN';
""" );

            // Drop legacy ReceiverType column if it is still present
            migrationBuilder.Sql( """
IF COL_LENGTH('dbo.FriendRequests','ReceiverType') IS NOT NULL
    ALTER TABLE dbo.FriendRequests DROP COLUMN ReceiverType;
""" );

            // Drop the old unique index regardless of its legacy name
            migrationBuilder.Sql( """
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FriendRequests_Requester_Receiver')
    DROP INDEX [IX_FriendRequests_Requester_Receiver] ON dbo.FriendRequests;
""" );

            migrationBuilder.Sql( """
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FriendRequest_Requester_Receiver')
    DROP INDEX [IX_FriendRequest_Requester_Receiver] ON dbo.FriendRequests;
""" );

            // Drop obsolete foreign keys only if they still exist
            migrationBuilder.Sql( """
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_FriendRequest_Users_RequesterUserId')
    ALTER TABLE dbo.FriendRequests DROP CONSTRAINT FK_FriendRequest_Users_RequesterUserId;
""" );

            migrationBuilder.Sql( """
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_FriendRequest_Users_ReceiverUserId')
    ALTER TABLE dbo.FriendRequests DROP CONSTRAINT FK_FriendRequest_Users_ReceiverUserId;
""" );

            // Ensure the composite unique index matches the new column names
            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_RequesterId_ReceiverId",
                table: "FriendRequests",
                columns: new[] { "RequesterId", "ReceiverId" },
                unique: true );

            // Add new FK constraints pointing to DirectoryEntries (Requester + Receiver can be any entry)
            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_DirectoryEntries_RequesterId",
                table: "FriendRequests",
                column: "RequesterId",
                principalTable: "DirectoryEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict );

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_DirectoryEntries_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId",
                principalTable: "DirectoryEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
