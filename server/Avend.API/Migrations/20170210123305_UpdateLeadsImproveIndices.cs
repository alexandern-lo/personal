using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class UpdateLeadsImproveIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_leads_tenant_uid_qualification",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_company_name",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_created_at",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_first_name",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_job_title",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_last_name",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_qualification",
                table: "leads");

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_updated_at",
                table: "leads",
                columns: new[] { "user_uid", "updated_at" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_tenant_uid_status_qualification",
                table: "leads",
                columns: new[] { "tenant_uid", "status", "qualification" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_company_name",
                table: "leads",
                columns: new[] { "user_uid", "status", "company_name" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_created_at",
                table: "leads",
                columns: new[] { "user_uid", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_first_name",
                table: "leads",
                columns: new[] { "user_uid", "status", "first_name" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_job_title",
                table: "leads",
                columns: new[] { "user_uid", "status", "job_title" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_last_name",
                table: "leads",
                columns: new[] { "user_uid", "status", "last_name" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_status_qualification",
                table: "leads",
                columns: new[] { "user_uid", "status", "qualification" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_updated_at",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_tenant_uid_status_qualification",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_status_company_name",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_status_created_at",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_status_first_name",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_status_job_title",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_status_last_name",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_user_uid_status_qualification",
                table: "leads");

            migrationBuilder.CreateIndex(
                name: "IX_leads_tenant_uid_qualification",
                table: "leads",
                columns: new[] { "tenant_uid", "qualification" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_company_name",
                table: "leads",
                columns: new[] { "user_uid", "company_name" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_created_at",
                table: "leads",
                columns: new[] { "user_uid", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_first_name",
                table: "leads",
                columns: new[] { "user_uid", "first_name" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_job_title",
                table: "leads",
                columns: new[] { "user_uid", "job_title" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_last_name",
                table: "leads",
                columns: new[] { "user_uid", "last_name" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_user_uid_qualification",
                table: "leads",
                columns: new[] { "user_uid", "qualification" });
        }
    }
}
