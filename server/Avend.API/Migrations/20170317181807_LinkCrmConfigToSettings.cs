using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class LinkCrmConfigToSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_settings_default_user_crm_configuration_id",
                table: "user_settings");

            migrationBuilder.AddColumn<long>(
                name: "settings_id",
                table: "crms",
                nullable: true);

            migrationBuilder.Sql("update crms set sync_fields = ''");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "settings_id",
                table: "crms");

            migrationBuilder.CreateIndex(
                name: "IX_user_settings_default_user_crm_configuration_id",
                table: "user_settings",
                column: "default_user_crm_configuration_id");
        }
    }
}
