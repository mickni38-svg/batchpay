using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchPay.Data.Migrations
{
    public partial class AddReceiverTypeToFriendRequestIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old unique index on (RequesterUserId, ReceiverUserId)
            migrationBuilder.DropIndex(
                name: "IX_FriendRequest_RequesterUserId_ReceiverUserId",
                table: "FriendRequest");

            // Create new unique index including ReceiverType
            migrationBuilder.CreateIndex(
                name: "IX_FriendRequest_RequesterUserId_ReceiverUserId_ReceiverType",
                table: "FriendRequest",
                columns: new[] { "RequesterUserId", "ReceiverUserId", "ReceiverType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new index
            migrationBuilder.DropIndex(
                name: "IX_FriendRequest_RequesterUserId_ReceiverUserId_ReceiverType",
                table: "FriendRequest");

            // Re-create old index
            migrationBuilder.CreateIndex(
                name: "IX_FriendRequest_RequesterUserId_ReceiverUserId",
                table: "FriendRequest",
                columns: new[] { "RequesterUserId", "ReceiverUserId" },
                unique: true);
        }
    }
}
