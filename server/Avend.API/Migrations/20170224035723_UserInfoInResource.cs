using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Avend.API.Migrations
{
    public partial class UserInfoInResource : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "resources",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_resources_user_id",
                table: "resources",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_resources_events_event_id",
                table: "resources",
                column: "event_id",
                principalTable: "events",
                principalColumn: "event_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_resources_subscription_members_user_id",
                table: "resources",
                column: "user_id",
                principalTable: "subscription_members",
                principalColumn: "subscription_member_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_resources_events_event_id",
                table: "resources");

            migrationBuilder.DropForeignKey(
                name: "FK_resources_subscription_members_user_id",
                table: "resources");

            migrationBuilder.DropIndex(
                name: "IX_resources_user_id",
                table: "resources");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "resources");
        }
    }
}
