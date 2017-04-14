using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class EventFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "recurring",
                table: "events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.RenameColumn(
                name: "subscription_uid",
                table: "subscriptions",
                newName: "uid");

            migrationBuilder.RenameColumn(
                name: "subscription_id",
                table: "subscriptions",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_subscriptions_subscription_uid",
                table: "subscriptions",
                newName: "IX_subscriptions_uid");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "subscriptions",
                type: "VARCHAR(1024)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "recurring",
                table: "events");

            migrationBuilder.DropColumn(
                name: "name",
                table: "subscriptions");

            migrationBuilder.RenameColumn(
                name: "uid",
                table: "subscriptions",
                newName: "subscription_uid");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "subscriptions",
                newName: "subscription_id");

            migrationBuilder.RenameIndex(
                name: "IX_subscriptions_uid",
                table: "subscriptions",
                newName: "IX_subscriptions_subscription_uid");
        }
    }
}
