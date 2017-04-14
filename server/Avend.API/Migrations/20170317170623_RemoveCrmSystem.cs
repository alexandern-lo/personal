using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class RemoveCrmSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_crm_configurations_crm_systems_crm_system_id",
                table: "user_crm_configurations");

            migrationBuilder.DropForeignKey(
                name: "FK_user_settings_user_crm_configurations_default_user_crm_configuration_id",
                table: "user_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_crm_configurations",
                table: "user_crm_configurations");

            migrationBuilder.DropIndex(
                name: "IX_user_crm_configurations_crm_system_id",
                table: "user_crm_configurations");

            migrationBuilder.DropColumn(
                name: "crm_system_id",
                table: "user_crm_configurations");

            migrationBuilder.RenameTable(
                name: "user_crm_configurations",
                newName: "crms");

            migrationBuilder.RenameColumn(
                name: "user_crm_configuration_uid",
                table: "crms",
                newName: "uid");

            migrationBuilder.RenameColumn(
                name: "field_mappings",
                table: "crms",
                newName: "sync_fields");

            migrationBuilder.RenameColumn(
                name: "user_crm_configuration_id",
                table: "crms",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_user_crm_configurations_user_uid_user_crm_configuration_uid",
                table: "crms",
                newName: "IX_crms_user_uid_uid");

            migrationBuilder.RenameIndex(
                name: "IX_user_crm_configurations_user_uid_name",
                table: "crms",
                newName: "IX_crms_user_uid_name");

            migrationBuilder.RenameIndex(
                name: "IX_user_crm_configurations_user_uid",
                table: "crms",
                newName: "IX_crms_user_uid");

            migrationBuilder.RenameIndex(
                name: "IX_user_crm_configurations_user_crm_configuration_uid",
                table: "crms",
                newName: "IX_crms_uid");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "crms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_crms",
                table: "crms",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_settings_crms_default_user_crm_configuration_id",
                table: "user_settings",
                column: "default_user_crm_configuration_id",
                principalTable: "crms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_settings_crms_default_user_crm_configuration_id",
                table: "user_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_crms",
                table: "crms");

            migrationBuilder.DropColumn(
                name: "type",
                table: "crms");

            migrationBuilder.RenameTable(
                name: "crms",
                newName: "user_crm_configurations");

            migrationBuilder.RenameColumn(
                name: "uid",
                table: "user_crm_configurations",
                newName: "user_crm_configuration_uid");

            migrationBuilder.RenameColumn(
                name: "sync_fields",
                table: "user_crm_configurations",
                newName: "field_mappings");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user_crm_configurations",
                newName: "user_crm_configuration_id");

            migrationBuilder.RenameIndex(
                name: "IX_crms_user_uid_uid",
                table: "user_crm_configurations",
                newName: "IX_user_crm_configurations_user_uid_user_crm_configuration_uid");

            migrationBuilder.RenameIndex(
                name: "IX_crms_user_uid_name",
                table: "user_crm_configurations",
                newName: "IX_user_crm_configurations_user_uid_name");

            migrationBuilder.RenameIndex(
                name: "IX_crms_user_uid",
                table: "user_crm_configurations",
                newName: "IX_user_crm_configurations_user_uid");

            migrationBuilder.RenameIndex(
                name: "IX_crms_uid",
                table: "user_crm_configurations",
                newName: "IX_user_crm_configurations_user_crm_configuration_uid");

            migrationBuilder.AddColumn<long>(
                name: "crm_system_id",
                table: "user_crm_configurations",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_crm_configurations",
                table: "user_crm_configurations",
                column: "user_crm_configuration_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_crm_configurations_crm_system_id",
                table: "user_crm_configurations",
                column: "crm_system_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_crm_configurations_crm_systems_crm_system_id",
                table: "user_crm_configurations",
                column: "crm_system_id",
                principalTable: "crm_systems",
                principalColumn: "crm_system_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_settings_user_crm_configurations_default_user_crm_configuration_id",
                table: "user_settings",
                column: "default_user_crm_configuration_id",
                principalTable: "user_crm_configurations",
                principalColumn: "user_crm_configuration_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
