using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Avend.API.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Remove all tables in database to make sure we start from scratch
            const string dbCleanup = @"
DECLARE @Sql NVARCHAR(500) 
DECLARE @Cursor CURSOR

SET @Cursor = CURSOR FAST_FORWARD FOR
    SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + ']'
    FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
    LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME =rc1.CONSTRAINT_NAME
    WHERE NOT tc2.TABLE_NAME LIKE '%__EFMigrationsHistory%'

OPEN @Cursor
FETCH NEXT FROM @Cursor INTO @Sql
WHILE (@@FETCH_STATUS = 0)
BEGIN
    Exec sp_executesql @Sql
    FETCH NEXT FROM @Cursor INTO @Sql
END
CLOSE @Cursor
DEALLOCATE @Cursor

SET @Cursor = CURSOR FAST_FORWARD FOR
	SELECT DISTINCT sql = 'DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']'
	FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_NAME != '__EFMigrationsHistory' AND TABLE_SCHEMA = 'dbo'

OPEN @Cursor
FETCH NEXT FROM @Cursor INTO @Sql
WHILE (@@FETCH_STATUS = 0)
BEGIN	
    Exec sp_executesql @Sql
    FETCH NEXT FROM @Cursor INTO @Sql
END

CLOSE @Cursor
DEALLOCATE @Cursor

DELETE FROM __EFMigrationsHistory;
";

            migrationBuilder.Sql(dbCleanup);

            migrationBuilder.CreateTable(
                name: "crm_systems",
                columns: table => new
                {
                    crm_system_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    abbreviation = table.Column<int>(nullable: false),
                    authorization_params = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    default_field_mappings = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    name = table.Column<string>(nullable: true),
                    token_request_params = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    token_request_url = table.Column<string>(nullable: true),
                    crm_system_uid = table.Column<Guid>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                { 
                    table.PrimaryKey("PK_crm_systems", x => x.crm_system_id);
                });

            migrationBuilder.CreateTable(
                name: "events_short_view",
                columns: table => new
                {
                    event_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    city = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    end_date = table.Column<DateTime>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    questions_count = table.Column<int>(nullable: false),
                    start_date = table.Column<DateTime>(nullable: true),
                    state = table.Column<string>(nullable: true),
                    event_type = table.Column<string>(nullable: true),
                    event_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events_short_view", x => x.event_id);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    resource_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    description = table.Column<string>(nullable: true),
                    event_id = table.Column<long>(nullable: true),
                    mime_type = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    opened_count = table.Column<int>(nullable: false),
                    sent_count = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    tenant_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true),
                    resource_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    url = table.Column<string>(nullable: true),
                    user_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resources", x => x.resource_id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    subscription_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    active_users_count = table.Column<int>(nullable: false),
                    additional_data = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    expires_at = table.Column<DateTime>(nullable: false),
                    external_uid = table.Column<string>(nullable: true),
                    max_users_count = table.Column<int>(nullable: false),
                    recurly_account_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    service_type = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    type = table.Column<string>(nullable: true),
                    subscription_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.subscription_id);
                });

            migrationBuilder.CreateTable(
                name: "terms",
                columns: table => new
                {
                    terms_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false),
                    release_date = table.Column<DateTime>(nullable: false),
                    terms_text = table.Column<string>(nullable: true),
                    terms_uid = table.Column<Guid>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_terms", x => x.terms_id);
                });

            migrationBuilder.CreateTable(
                name: "terms_acceptances",
                columns: table => new
                {
                    terms_acceptance_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    accepted_at = table.Column<DateTime>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    terms_id = table.Column<long>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false),
                    user_uid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_terms_acceptances", x => x.terms_acceptance_id);
                });

            migrationBuilder.CreateTable(
                name: "user_crm_configurations",
                columns: table => new
                {
                    user_crm_configuration_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    access_token = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    crm_system_id = table.Column<long>(nullable: false),
                    dynamics_365_url = table.Column<string>(type: "VARCHAR(2048)", nullable: true),
                    field_mappings = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    name = table.Column<string>(nullable: true),
                    refresh_token = table.Column<string>(nullable: true),
                    user_crm_configuration_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false),
                    user_uid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_crm_configurations", x => x.user_crm_configuration_id);
                    table.ForeignKey(
                        name: "FK_user_crm_configurations_crm_systems_crm_system_id",
                        column: x => x.crm_system_id,
                        principalTable: "crm_systems",
                        principalColumn: "crm_system_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    event_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    address = table.Column<string>(maxLength: 500, nullable: true),
                    city = table.Column<string>(maxLength: 200, nullable: true),
                    country = table.Column<string>(maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    description = table.Column<string>(maxLength: 2048, nullable: true),
                    end_date = table.Column<DateTime>(nullable: true),
                    industry = table.Column<string>(maxLength: 200, nullable: true),
                    logo_url = table.Column<string>(maxLength: 1024, nullable: true),
                    name = table.Column<string>(maxLength: 200, nullable: true),
                    start_date = table.Column<DateTime>(nullable: true),
                    state = table.Column<string>(maxLength: 200, nullable: true),
                    subscription_id = table.Column<long>(nullable: true),
                    event_type = table.Column<string>(maxLength: 20, nullable: true),
                    event_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    user_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    venue_name = table.Column<string>(maxLength: 200, nullable: true),
                    website_url = table.Column<string>(maxLength: 1024, nullable: true),
                    zip_code = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.event_id);
                    table.ForeignKey(
                        name: "FK_events_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "subscription_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscription_members",
                columns: table => new
                {
                    subscription_member_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    city = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    email = table.Column<string>(nullable: true),
                    first_name = table.Column<string>(nullable: true),
                    job_title = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    role = table.Column<int>(nullable: false),
                    state = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    subscription_id = table.Column<long>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    user_uid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_members", x => x.subscription_member_id);
                    table.ForeignKey(
                        name: "FK_subscription_members_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "subscription_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_transactions",
                columns: table => new
                {
                    user_transaction_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    additional_data = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    external_uid = table.Column<string>(nullable: true),
                    service_type = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    user_subscription_id = table.Column<long>(nullable: false),
                    user_transaction_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false),
                    user_uid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_transactions", x => x.user_transaction_id);
                    table.ForeignKey(
                        name: "FK_user_transactions_subscriptions_user_subscription_id",
                        column: x => x.user_subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "subscription_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_settings",
                columns: table => new
                {
                    user_settings_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false),
                    default_user_crm_configuration_id = table.Column<long>(nullable: true),
                    time_zone = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: false),
                    user_uid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_settings", x => x.user_settings_id);
                    table.ForeignKey(
                        name: "FK_user_settings_user_crm_configurations_default_user_crm_configuration_id",
                        column: x => x.default_user_crm_configuration_id,
                        principalTable: "user_crm_configurations",
                        principalColumn: "user_crm_configuration_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attendee_categories",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    event_id = table.Column<long>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    position = table.Column<int>(nullable: false),
                    uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendee_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendee_categories_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);                    
                });

            migrationBuilder.CreateTable(
                name: "attendees",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    avatar_url = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    company = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    event_id = table.Column<long>(nullable: false),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true),
                    state = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    zipcode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendees", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendees_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_agenda_items",
                columns: table => new
                {
                    event_agenda_item_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    date = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    details_url = table.Column<string>(nullable: true),
                    end_time_ticks = table.Column<long>(nullable: false),
                    event_id = table.Column<long>(nullable: false),
                    location = table.Column<string>(nullable: true),
                    location_url = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    start_time_ticks = table.Column<long>(nullable: false),
                    event_agenda_item_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_agenda_items", x => x.event_agenda_item_id);
                    table.ForeignKey(
                        name: "FK_event_agenda_items_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_questions",
                columns: table => new
                {
                    event_question_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    event_id = table.Column<long>(nullable: false),
                    position = table.Column<int>(nullable: false),
                    question_text = table.Column<string>(nullable: true),
                    event_question_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_questions", x => x.event_question_id);
                    table.ForeignKey(
                        name: "FK_event_questions_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_user_expenses",
                columns: table => new
                {
                    event_user_expense_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    amount = table.Column<decimal>(nullable: false),
                    comments = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    currency = table.Column<int>(nullable: false),
                    event_id = table.Column<long>(nullable: false),
                    spent_at = table.Column<DateTime>(nullable: false),
                    tenant_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true),
                    event_user_expense_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    user_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_user_expenses", x => x.event_user_expense_id);
                    table.ForeignKey(
                        name: "FK_event_user_expenses_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_lead_goals",
                columns: table => new
                {
                    event_lead_goal_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    event_id = table.Column<long>(nullable: false),
                    leads_acquired = table.Column<int>(nullable: false),
                    leads_goal = table.Column<int>(nullable: false),
                    tenant_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true),
                    event_lead_goal_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    user_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_lead_goals", x => x.event_lead_goal_id);
                    table.ForeignKey(
                        name: "FK_event_lead_goals_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "leads",
                columns: table => new
                {
                    lead_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    address = table.Column<string>(nullable: true),
                    business_card_back_thumbnail_url = table.Column<string>(nullable: true),
                    business_card_back_url = table.Column<string>(nullable: true),
                    business_card_front_thumbnail_url = table.Column<string>(nullable: true),
                    business_card_front_url = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    clientside_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true),
                    clientside_updated_at = table.Column<DateTime>(nullable: true),
                    company_name = table.Column<string>(nullable: true),
                    company_url = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    event_id = table.Column<long>(nullable: true),
                    location = table.Column<string>(nullable: true),
                    location_latitude = table.Column<double>(nullable: true),
                    location_longitude = table.Column<double>(nullable: true),
                    first_name = table.Column<string>(nullable: true),
                    job_title = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    notes = table.Column<string>(nullable: true),
                    photo_thumbnail_url = table.Column<string>(nullable: true),
                    photo_url = table.Column<string>(nullable: true),
                    qualification = table.Column<int>(nullable: false),
                    state = table.Column<string>(nullable: true),
                    tenant_uid = table.Column<Guid>(nullable: true),
                    lead_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    user_uid = table.Column<Guid>(nullable: false),
                    zip_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leads", x => x.lead_id);
                    table.ForeignKey(
                        name: "FK_leads_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscription_invites",
                columns: table => new
                {
                    invite_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    accepted = table.Column<bool>(nullable: false),
                    accepted_at = table.Column<DateTime>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    email = table.Column<string>(nullable: true),
                    invite_code = table.Column<string>(nullable: true),
                    subscription_id = table.Column<long>(nullable: false),
                    subscription_member_id = table.Column<long>(nullable: true),
                    invite_uid = table.Column<Guid>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true),
                    valid_till = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_invites", x => x.invite_id);
                    table.ForeignKey(
                        name: "FK_subscription_invites_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "subscription_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_subscription_invites_subscription_members_subscription_member_id",
                        column: x => x.subscription_member_id,
                        principalTable: "subscription_members",
                        principalColumn: "subscription_member_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attendee_category_options",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    attendee_category_id = table.Column<long>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    position = table.Column<int>(nullable: false),
                    uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendee_category_options", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendee_category_options_attendee_categories_attendee_category_id",
                        column: x => x.attendee_category_id,
                        principalTable: "attendee_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_question_answers",
                columns: table => new
                {
                    event_question_answer_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    event_question_id = table.Column<long>(nullable: false),
                    position = table.Column<int>(nullable: false),
                    answer_text = table.Column<string>(nullable: true),
                    event_question_answer_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_question_answers", x => x.event_question_answer_id);
                    table.ForeignKey(
                        name: "FK_event_question_answers_event_questions_event_question_id",
                        column: x => x.event_question_id,
                        principalTable: "event_questions",
                        principalColumn: "event_question_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lead_emails",
                columns: table => new
                {
                    lead_email_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false),
                    designation = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    lead_id = table.Column<long>(nullable: false),
                    lead_email_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_emails", x => x.lead_email_id);
                    table.ForeignKey(
                        name: "FK_lead_emails_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "lead_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lead_export_statuses",
                columns: table => new
                {
                    lead_export_status_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    exported_at = table.Column<DateTime>(nullable: true),
                    external_uid = table.Column<string>(nullable: true),
                    lead_id = table.Column<long>(nullable: true),
                    lead_export_status_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()"),
                    user_crm_configuration_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_export_statuses", x => x.lead_export_status_id);
                    table.ForeignKey(
                        name: "FK_lead_export_statuses_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "lead_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lead_phones",
                columns: table => new
                {
                    lead_phone_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    created_at = table.Column<DateTime>(nullable: false),
                    designation = table.Column<string>(nullable: true),
                    lead_id = table.Column<long>(nullable: false),
                    phone = table.Column<string>(nullable: true),
                    lead_phone_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_phones", x => x.lead_phone_id);
                    table.ForeignKey(
                        name: "FK_lead_phones_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "lead_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attendee_category_values",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    attendee_id = table.Column<long>(nullable: false),
                    attendee_category_id = table.Column<long>(nullable: true),
                    attendee_category_option_id = table.Column<long>(nullable: true),
                    category_value = table.Column<string>(nullable: true),
                    option_value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendee_category_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendee_category_values_attendees_attendee_id",
                        column: x => x.attendee_id,
                        principalTable: "attendees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attendee_category_values_attendee_categories_attendee_category_id",
                        column: x => x.attendee_category_id,
                        principalTable: "attendee_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attendee_category_values_attendee_category_options_attendee_category_option_id",
                        column: x => x.attendee_category_option_id,
                        principalTable: "attendee_category_options",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lead_question_answers",
                columns: table => new
                {
                    lead_question_answer_id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    answer_text = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    event_answer_id = table.Column<long>(nullable: false),
                    event_answer_uid = table.Column<Guid>(nullable: true),
                    event_question_id = table.Column<long>(nullable: false),
                    event_question_uid = table.Column<Guid>(nullable: true),
                    lead_id = table.Column<long>(nullable: false),
                    question_text = table.Column<string>(nullable: true),
                    lead_question_answer_uid = table.Column<Guid>(type: "UniqueIdentifier", nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lead_question_answers", x => x.lead_question_answer_id);
                    table.ForeignKey(
                        name: "FK_lead_question_answers_event_question_answers_event_answer_id",
                        column: x => x.event_answer_id,
                        principalTable: "event_question_answers",
                        principalColumn: "event_question_answer_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lead_question_answers_event_questions_event_question_id",
                        column: x => x.event_question_id,
                        principalTable: "event_questions",
                        principalColumn: "event_question_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lead_question_answers_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "lead_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendee_categories_event_id",
                table: "attendee_categories",
                column: "event_id");

            migrationBuilder.CreateIndex(
               name: "IX_attendee_categories_uid",
               table: "attendee_categories",
               column: "uid");

            migrationBuilder.CreateIndex(
                name: "IX_attendee_category_options_attendee_category_id",
                table: "attendee_category_options",
                column: "attendee_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendee_category_values_attendee_id",
                table: "attendee_category_values",
                column: "attendee_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendee_category_values_attendee_category_option_id",
                table: "attendee_category_values",
                column: "attendee_category_option_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendee_category_values_attendee_category_id_attendee_category_option_id",
                table: "attendee_category_values",
                columns: new[] { "attendee_category_id", "attendee_category_option_id" });

            migrationBuilder.CreateIndex(
                name: "IX_attendee_category_values_attendee_id_attendee_category_id_attendee_category_option_id",
                table: "attendee_category_values",
                columns: new[] { "attendee_id", "attendee_category_id", "attendee_category_option_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_attendees_event_id",
                table: "attendees",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendees_event_id_company_id",
                table: "attendees",
                columns: new[] { "event_id", "company", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_attendees_event_id_email_id",
                table: "attendees",
                columns: new[] { "event_id", "email", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_attendees_event_id_first_name_id",
                table: "attendees",
                columns: new[] { "event_id", "first_name", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_attendees_event_id_last_name_id",
                table: "attendees",
                columns: new[] { "event_id", "last_name", "id" });

            migrationBuilder.CreateIndex(
                name: "IX_crm_systems_created_at",
                table: "crm_systems",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_crm_systems_name",
                table: "crm_systems",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_crm_systems_crm_system_uid",
                table: "crm_systems",
                column: "crm_system_uid");

            migrationBuilder.CreateIndex(
                name: "IX_crm_systems_updated_at",
                table: "crm_systems",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_agenda_items_created_at",
                table: "event_agenda_items",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_agenda_items_event_agenda_item_uid",
                table: "event_agenda_items",
                column: "event_agenda_item_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_agenda_items_updated_at",
                table: "event_agenda_items",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_agenda_items_event_id_date_start_time_ticks",
                table: "event_agenda_items",
                columns: new[] { "event_id", "date", "start_time_ticks" });

            migrationBuilder.CreateIndex(
                name: "IX_event_agenda_items_event_id_location_date",
                table: "event_agenda_items",
                columns: new[] { "event_id", "location", "date" });

            migrationBuilder.CreateIndex(
                name: "IX_event_questions_created_at",
                table: "event_questions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_questions_event_id",
                table: "event_questions",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_questions_event_question_uid",
                table: "event_questions",
                column: "event_question_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_questions_updated_at",
                table: "event_questions",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_question_answers_created_at",
                table: "event_question_answers",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_question_answers_event_question_id",
                table: "event_question_answers",
                column: "event_question_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_question_answers_event_question_answer_uid",
                table: "event_question_answers",
                column: "event_question_answer_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_question_answers_updated_at",
                table: "event_question_answers",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_events_created_at",
                table: "events",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_events_subscription_id",
                table: "events",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_event_uid",
                table: "events",
                column: "event_uid");

            migrationBuilder.CreateIndex(
                name: "IX_events_updated_at",
                table: "events",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_created_at",
                table: "event_user_expenses",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_tenant_uid",
                table: "event_user_expenses",
                column: "tenant_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_event_user_expense_uid",
                table: "event_user_expenses",
                column: "event_user_expense_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_updated_at",
                table: "event_user_expenses",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_user_uid",
                table: "event_user_expenses",
                column: "user_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_event_id_spent_at",
                table: "event_user_expenses",
                columns: new[] { "event_id", "spent_at" });

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_user_uid_spent_at",
                table: "event_user_expenses",
                columns: new[] { "user_uid", "spent_at" });

            migrationBuilder.CreateIndex(
                name: "IX_event_user_expenses_event_id_user_uid_spent_at",
                table: "event_user_expenses",
                columns: new[] { "event_id", "user_uid", "spent_at" });

            migrationBuilder.CreateIndex(
                name: "IX_event_lead_goals_created_at",
                table: "event_lead_goals",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_lead_goals_tenant_uid",
                table: "event_lead_goals",
                column: "tenant_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_lead_goals_event_lead_goal_uid",
                table: "event_lead_goals",
                column: "event_lead_goal_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_lead_goals_updated_at",
                table: "event_lead_goals",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_event_lead_goals_user_uid",
                table: "event_lead_goals",
                column: "user_uid");

            migrationBuilder.CreateIndex(
                name: "IX_event_lead_goals_event_id_tenant_uid",
                table: "event_lead_goals",
                columns: new[] { "event_id", "tenant_uid" });

            migrationBuilder.CreateIndex(
                name: "IX_event_lead_goals_event_id_user_uid",
                table: "event_lead_goals",
                columns: new[] { "event_id", "user_uid" });

            migrationBuilder.CreateIndex(
                name: "IX_leads_created_at",
                table: "leads",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_leads_event_id",
                table: "leads",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_leads_lead_uid",
                table: "leads",
                column: "lead_uid");

            migrationBuilder.CreateIndex(
                name: "IX_leads_updated_at",
                table: "leads",
                column: "updated_at");

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

            migrationBuilder.CreateIndex(
                name: "IX_lead_emails_lead_id",
                table: "lead_emails",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_lead_export_statuses_created_at",
                table: "lead_export_statuses",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_lead_export_statuses_lead_id",
                table: "lead_export_statuses",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_lead_export_statuses_lead_export_status_uid",
                table: "lead_export_statuses",
                column: "lead_export_status_uid");

            migrationBuilder.CreateIndex(
                name: "IX_lead_export_statuses_updated_at",
                table: "lead_export_statuses",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_lead_phones_lead_id",
                table: "lead_phones",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_lead_question_answers_created_at",
                table: "lead_question_answers",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_lead_question_answers_event_answer_id",
                table: "lead_question_answers",
                column: "event_answer_id");

            migrationBuilder.CreateIndex(
                name: "IX_lead_question_answers_event_question_id",
                table: "lead_question_answers",
                column: "event_question_id");

            migrationBuilder.CreateIndex(
                name: "IX_lead_question_answers_lead_id",
                table: "lead_question_answers",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_lead_question_answers_lead_question_answer_uid",
                table: "lead_question_answers",
                column: "lead_question_answer_uid");

            migrationBuilder.CreateIndex(
                name: "IX_lead_question_answers_updated_at",
                table: "lead_question_answers",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_resources_created_at",
                table: "resources",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_resources_event_id",
                table: "resources",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_resources_tenant_uid",
                table: "resources",
                column: "tenant_uid");

            migrationBuilder.CreateIndex(
                name: "IX_resources_resource_uid",
                table: "resources",
                column: "resource_uid");

            migrationBuilder.CreateIndex(
                name: "IX_resources_updated_at",
                table: "resources",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_resources_url",
                table: "resources",
                column: "url");

            migrationBuilder.CreateIndex(
                name: "IX_resources_user_uid",
                table: "resources",
                column: "user_uid");

            migrationBuilder.CreateIndex(
                name: "IX_resources_event_id_status",
                table: "resources",
                columns: new[] { "event_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_resources_user_uid_status",
                table: "resources",
                columns: new[] { "user_uid", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_created_at",
                table: "subscriptions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_external_uid",
                table: "subscriptions",
                column: "external_uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_subscription_uid",
                table: "subscriptions",
                column: "subscription_uid");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_updated_at",
                table: "subscriptions",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_recurly_account_uid_status_expires_at",
                table: "subscriptions",
                columns: new[] { "recurly_account_uid", "status", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invites_email",
                table: "subscription_invites",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invites_invite_code",
                table: "subscription_invites",
                column: "invite_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invites_subscription_id",
                table: "subscription_invites",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invites_subscription_member_id",
                table: "subscription_invites",
                column: "subscription_member_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invites_invite_uid",
                table: "subscription_invites",
                column: "invite_uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_members_subscription_id",
                table: "subscription_members",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_members_user_uid_subscription_id",
                table: "subscription_members",
                columns: new[] { "user_uid", "subscription_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_crm_configurations_crm_system_id",
                table: "user_crm_configurations",
                column: "crm_system_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_crm_configurations_user_crm_configuration_uid",
                table: "user_crm_configurations",
                column: "user_crm_configuration_uid");

            migrationBuilder.CreateIndex(
                name: "IX_user_crm_configurations_user_uid",
                table: "user_crm_configurations",
                column: "user_uid");

            migrationBuilder.CreateIndex(
                name: "IX_user_crm_configurations_user_uid_name",
                table: "user_crm_configurations",
                columns: new[] { "user_uid", "name" });

            migrationBuilder.CreateIndex(
                name: "IX_user_crm_configurations_user_uid_user_crm_configuration_uid",
                table: "user_crm_configurations",
                columns: new[] { "user_uid", "user_crm_configuration_uid" });

            migrationBuilder.CreateIndex(
                name: "IX_user_settings_default_user_crm_configuration_id",
                table: "user_settings",
                column: "default_user_crm_configuration_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_settings_user_uid",
                table: "user_settings",
                column: "user_uid");

            migrationBuilder.CreateIndex(
                name: "IX_user_transactions_user_subscription_id",
                table: "user_transactions",
                column: "user_subscription_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendee_category_values");

            migrationBuilder.DropTable(
                name: "event_agenda_items");

            migrationBuilder.DropTable(
                name: "event_user_expenses");

            migrationBuilder.DropTable(
                name: "event_lead_goals");

            migrationBuilder.DropTable(
                name: "lead_emails");

            migrationBuilder.DropTable(
                name: "lead_export_statuses");

            migrationBuilder.DropTable(
                name: "lead_phones");

            migrationBuilder.DropTable(
                name: "lead_question_answers");

            migrationBuilder.DropTable(
                name: "events_short_view");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "subscription_invites");

            migrationBuilder.DropTable(
                name: "terms");

            migrationBuilder.DropTable(
                name: "terms_acceptances");

            migrationBuilder.DropTable(
                name: "user_settings");

            migrationBuilder.DropTable(
                name: "user_transactions");

            migrationBuilder.DropTable(
                name: "attendees");

            migrationBuilder.DropTable(
                name: "attendee_category_options");

            migrationBuilder.DropTable(
                name: "event_question_answers");

            migrationBuilder.DropTable(
                name: "leads");

            migrationBuilder.DropTable(
                name: "subscription_members");

            migrationBuilder.DropTable(
                name: "user_crm_configurations");

            migrationBuilder.DropTable(
                name: "attendee_categories");

            migrationBuilder.DropTable(
                name: "event_questions");

            migrationBuilder.DropTable(
                name: "crm_systems");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
