using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class EventOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_uid",
                table: "events");

            migrationBuilder.AddColumn<long>(
                name: "owner_id",
                table: "events",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_owner_id",
                table: "events",
                column: "owner_id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_subscription_members_owner_id",
                table: "events",
                column: "owner_id",
                principalTable: "subscription_members",
                principalColumn: "subscription_member_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_subscription_members_owner_id",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_owner_id",
                table: "events");

            migrationBuilder.DropColumn(
                name: "owner_id",
                table: "events");

            migrationBuilder.AddColumn<Guid>(
                name: "user_uid",
                table: "events",
                type: "UniqueIdentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
