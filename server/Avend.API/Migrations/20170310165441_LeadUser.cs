using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class LeadUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_subscriptions_uid",
                table: "subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_subscription_members_user_uid_subscription_id",
                table: "subscription_members");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_subscription_members_user_uid",
                table: "subscription_members",
                column: "user_uid");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_uid",
                table: "subscriptions",
                column: "uid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_leads_subscription_members_user_uid",
                table: "leads",
                column: "user_uid",
                principalTable: "subscription_members",
                principalColumn: "user_uid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leads_subscription_members_user_uid",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_subscriptions_uid",
                table: "subscriptions");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_subscription_members_user_uid",
                table: "subscription_members");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_uid",
                table: "subscriptions",
                column: "uid");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_members_user_uid_subscription_id",
                table: "subscription_members",
                columns: new[] { "user_uid", "subscription_id" },
                unique: true);
        }
    }
}
