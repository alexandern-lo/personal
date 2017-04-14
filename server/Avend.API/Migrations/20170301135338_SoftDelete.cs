using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class SoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_leads_subscription_id_status_qualification", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_status_company_name", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_status_created_at", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_status_first_name", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_status_job_title", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_status_last_name", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_status_qualification", "leads");

            migrationBuilder.DropColumn(
                name: "status",
                table: "leads");

            migrationBuilder.AddColumn<bool>(
                name: "deleted",
                table: "resources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "deleted",
                table: "leads",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "deleted",
                table: "events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_leads_subscription_id_qualification",
                table: "leads",
                columns: new[] {"subscription_id", "qualification"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_company_name",
                table: "leads",
                columns: new[] {"user_uid", "company_name"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_created_at",
                table: "leads",
                columns: new[] {"user_uid", "created_at"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_first_name",
                table: "leads",
                columns: new[] {"user_uid", "first_name"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_job_title",
                table: "leads",
                columns: new[] {"user_uid", "job_title"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_last_name",
                table: "leads",
                columns: new[] {"user_uid", "last_name"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_qualification",
                table: "leads",
                columns: new[] {"user_uid", "qualification"});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "deleted",
                table: "resources");

            migrationBuilder.DropColumn(
                name: "deleted",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "deleted",
                table: "events");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "leads",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropIndex("IX_leads_subscription_id_qualification", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_company_name", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_created_at", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_first_name", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_job_title", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_last_name", "leads");
            migrationBuilder.DropIndex("IX_leads_user_uid_qualification", "leads");

            migrationBuilder.CreateIndex(
                name: "IX_leads_subscription_id_status_qualification",
                table: "leads",
                columns: new[] {"subscription_id", "status", "qualification"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_company_name",
                table: "leads",
                columns: new[] {"user_uid", "status", "company_name"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_created_at",
                table: "leads",
                columns: new[] {"user_uid", "status", "created_at"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_first_name",
                table: "leads",
                columns: new[] {"user_uid", "status", "first_name"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_job_title",
                table: "leads",
                columns: new[] {"user_uid", "status", "job_title"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_last_name",
                table: "leads",
                columns: new[] {"user_uid", "status", "last_name"});

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_qualification",
                table: "leads",
                columns: new[] {"user_uid", "status", "qualification"});
        }
    }
}