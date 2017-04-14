using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class UpdateLeadsReplaceTenantUidWithSubscriptionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_leads_tenant_uid_status_qualification",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "tenant_uid",
                table: "leads");

            migrationBuilder.AddColumn<long>(
                name: "subscription_id",
                table: "leads",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_leads_subscription_id_status_qualification",
                table: "leads",
                columns: new[] { "subscription_id", "status", "qualification" });

            migrationBuilder.AddForeignKey(
                name: "FK_leads_subscriptions_subscription_id",
                table: "leads",
                column: "subscription_id",
                principalTable: "subscriptions",
                principalColumn: "subscription_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leads_subscriptions_subscription_id",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_subscription_id_status_qualification",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "subscription_id",
                table: "leads");

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_uid",
                table: "leads",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_leads_tenant_uid_status_qualification",
                table: "leads",
                columns: new[] { "tenant_uid", "status", "qualification" });
        }
    }
}
