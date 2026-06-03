using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MazadZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBidAuctionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_AuctionId1",
                table: "Bids");

            migrationBuilder.DropIndex(
                name: "IX_Bids_AuctionId1",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "AuctionId1",
                table: "Bids");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids");

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionId1",
                table: "Bids",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AuctionId1",
                table: "Bids",
                column: "AuctionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId1",
                table: "Bids",
                column: "AuctionId1",
                principalTable: "Auctions",
                principalColumn: "Id");
        }
    }
}
