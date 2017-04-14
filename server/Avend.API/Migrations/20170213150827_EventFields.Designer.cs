using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Avend.API.Services;
using Avend.API.Model;

namespace Avend.API.Migrations
{
    [DbContext(typeof(AvendDbContext))]
    [Migration("20170213150827_EventFields")]
    partial class EventFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("EventId")
                        .HasColumnName("event_id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<int>("Position")
                        .HasColumnName("position");

                    b.Property<Guid>("Uid")
                        .HasColumnName("uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("Uid");

                    b.ToTable("attendee_categories");
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryOption", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("CategoryId")
                        .HasColumnName("attendee_category_id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<int>("Position")
                        .HasColumnName("position");

                    b.Property<Guid>("Uid")
                        .HasColumnName("uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("attendee_category_options");
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<long>("AttendeeId")
                        .HasColumnName("attendee_id");

                    b.Property<long?>("CategoryId")
                        .HasColumnName("attendee_category_id");

                    b.Property<long?>("CategoryOptionId")
                        .HasColumnName("attendee_category_option_id");

                    b.Property<string>("CategoryValue")
                        .HasColumnName("category_value");

                    b.Property<string>("OptionValue")
                        .HasColumnName("option_value");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex("CategoryOptionId");

                    b.HasIndex("CategoryId", "CategoryOptionId");

                    b.HasIndex("AttendeeId", "CategoryId", "CategoryOptionId")
                        .IsUnique();

                    b.ToTable("attendee_category_values");
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("AvatarUrl")
                        .HasColumnName("avatar_url");

                    b.Property<string>("City")
                        .HasColumnName("city");

                    b.Property<string>("Company")
                        .HasColumnName("company");

                    b.Property<string>("Country")
                        .HasColumnName("country");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<long>("EventId")
                        .HasColumnName("event_id");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.Property<string>("Phone")
                        .HasColumnName("phone");

                    b.Property<string>("State")
                        .HasColumnName("state");

                    b.Property<string>("Title")
                        .HasColumnName("title");

                    b.Property<Guid>("Uid")
                        .HasColumnName("uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<string>("ZipCode")
                        .HasColumnName("zipcode");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("EventId", "Company", "Id");

                    b.HasIndex("EventId", "Email", "Id");

                    b.HasIndex("EventId", "FirstName", "Id");

                    b.HasIndex("EventId", "LastName", "Id");

                    b.ToTable("attendees");
                });

            modelBuilder.Entity("Avend.API.Model.CrmSystem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("crm_system_id");

                    b.Property<int>("Abbreviation")
                        .HasColumnName("abbreviation");

                    b.Property<string>("AuthorizationParams")
                        .HasColumnName("authorization_params")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("DefaultFieldMappings")
                        .HasColumnName("default_field_mappings")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("TokenRequestParams")
                        .HasColumnName("token_request_params")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<string>("TokenRequestUrl")
                        .HasColumnName("token_request_url");

                    b.Property<Guid>("Uid")
                        .HasColumnName("crm_system_uid");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Name");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("crm_systems");
                });

            modelBuilder.Entity("Avend.API.Model.EventAgendaItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_agenda_item_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<DateTime>("Date")
                        .HasColumnName("date");

                    b.Property<string>("Description")
                        .HasColumnName("description");

                    b.Property<string>("DetailsUrl")
                        .HasColumnName("details_url");

                    b.Property<long>("EndTimeTicks")
                        .HasColumnName("end_time_ticks");

                    b.Property<long>("EventId")
                        .HasColumnName("event_id");

                    b.Property<string>("Location")
                        .HasColumnName("location");

                    b.Property<string>("LocationUrl")
                        .HasColumnName("location_url");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<long>("StartTimeTicks")
                        .HasColumnName("start_time_ticks");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_agenda_item_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("EventId", "Date", "StartTimeTicks");

                    b.HasIndex("EventId", "Location", "Date");

                    b.ToTable("event_agenda_items");
                });

            modelBuilder.Entity("Avend.API.Model.EventQuestionRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_question_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<long>("EventId")
                        .HasColumnName("event_id");

                    b.Property<int>("Position")
                        .HasColumnName("position");

                    b.Property<string>("Text")
                        .HasColumnName("question_text");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_question_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("EventId");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("event_questions");
                });

            modelBuilder.Entity("Avend.API.Model.AnswerChoiceRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_question_answer_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<long>("EventQuestionId")
                        .HasColumnName("event_question_id");

                    b.Property<int>("Position")
                        .HasColumnName("position");

                    b.Property<string>("Text")
                        .HasColumnName("answer_text");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_question_answer_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("EventQuestionId");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("event_question_answers");
                });

            modelBuilder.Entity("Avend.API.Model.EventRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_id");

                    b.Property<string>("Address")
                        .HasColumnName("address")
                        .HasMaxLength(500);

                    b.Property<string>("City")
                        .HasColumnName("city")
                        .HasMaxLength(200);

                    b.Property<string>("Country")
                        .HasColumnName("country")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasMaxLength(2048);

                    b.Property<DateTime?>("EndDate")
                        .HasColumnName("end_date");

                    b.Property<string>("Industry")
                        .HasColumnName("industry")
                        .HasMaxLength(200);

                    b.Property<string>("LogoUrl")
                        .HasColumnName("logo_url")
                        .HasMaxLength(1024);

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasMaxLength(200);

                    b.Property<long?>("OwnerId")
                        .HasColumnName("owner_id");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnName("start_date");

                    b.Property<string>("State")
                        .HasColumnName("state")
                        .HasMaxLength(200);

                    b.Property<long?>("SubscriptionId")
                        .HasColumnName("subscription_id");

                    b.Property<string>("Type")
                        .HasColumnName("event_type")
                        .HasMaxLength(20);

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("VenueName")
                        .HasColumnName("venue_name")
                        .HasMaxLength(200);

                    b.Property<string>("WebsiteUrl")
                        .HasColumnName("website_url")
                        .HasMaxLength(1024);

                    b.Property<string>("ZipCode")
                        .HasColumnName("zip_code")
                        .HasMaxLength(200);

                    b.Property<bool>("Recurring")
                        .HasColumnName("recurring");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("OwnerId");

                    b.HasIndex("SubscriptionId");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("events");
                });

            modelBuilder.Entity("Avend.API.Model.EventUserExpense", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_user_expense_id");

                    b.Property<decimal>("Amount")
                        .HasColumnName("amount");

                    b.Property<string>("Comments")
                        .HasColumnName("comments");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<int>("Currency")
                        .HasColumnName("currency");

                    b.Property<long>("EventId")
                        .HasColumnName("event_id");

                    b.Property<DateTime>("SpentAt")
                        .HasColumnName("spent_at");

                    b.Property<Guid?>("TenantUid")
                        .HasColumnName("tenant_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_user_expense_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<Guid?>("UserUid")
                        .HasColumnName("user_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("TenantUid");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("UserUid");

                    b.HasIndex("EventId", "SpentAt");

                    b.HasIndex("UserUid", "SpentAt");

                    b.HasIndex("EventId", "UserUid", "SpentAt");

                    b.ToTable("event_user_expenses");
                });

            modelBuilder.Entity("Avend.API.Model.EventUserGoal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_lead_goal_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<long>("EventId")
                        .HasColumnName("event_id");

                    b.Property<int>("LeadsAcquired")
                        .HasColumnName("leads_acquired");

                    b.Property<int>("LeadsGoal")
                        .HasColumnName("leads_goal");

                    b.Property<Guid?>("TenantUid")
                        .HasColumnName("tenant_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_lead_goal_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<Guid?>("UserUid")
                        .HasColumnName("user_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("TenantUid");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("UserUid");

                    b.HasIndex("EventId", "TenantUid");

                    b.HasIndex("EventId", "UserUid");

                    b.ToTable("event_lead_goals");
                });

            modelBuilder.Entity("Avend.API.Model.Lead", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("lead_id");

                    b.Property<string>("Address")
                        .HasColumnName("address");

                    b.Property<string>("BusinessCardBackThumbnailUrl")
                        .HasColumnName("business_card_back_thumbnail_url");

                    b.Property<string>("BusinessCardBackUrl")
                        .HasColumnName("business_card_back_url");

                    b.Property<string>("BusinessCardFrontThumbnailUrl")
                        .HasColumnName("business_card_front_thumbnail_url");

                    b.Property<string>("BusinessCardFrontUrl")
                        .HasColumnName("business_card_front_url");

                    b.Property<string>("City")
                        .HasColumnName("city");

                    b.Property<Guid?>("ClientsideUid")
                        .HasColumnName("clientside_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("ClientsideUpdatedAt")
                        .HasColumnName("clientside_updated_at");

                    b.Property<string>("CompanyName")
                        .HasColumnName("company_name");

                    b.Property<string>("CompanyUrl")
                        .HasColumnName("company_url");

                    b.Property<string>("Country")
                        .HasColumnName("country");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<long?>("EventId")
                        .HasColumnName("event_id");

                    b.Property<string>("FirstEntryLocation")
                        .HasColumnName("location");

                    b.Property<double?>("FirstEntryLocationLatitude")
                        .HasColumnName("location_latitude");

                    b.Property<double?>("FirstEntryLocationLongitude")
                        .HasColumnName("location_longitude");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("JobTitle")
                        .HasColumnName("job_title");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.Property<string>("Notes")
                        .HasColumnName("notes");

                    b.Property<string>("PhotoThumbnailUrl")
                        .HasColumnName("photo_thumbnail_url");

                    b.Property<string>("PhotoUrl")
                        .HasColumnName("photo_url");

                    b.Property<int>("Qualification")
                        .HasColumnName("qualification");

                    b.Property<string>("State")
                        .HasColumnName("state");

                    b.Property<Guid?>("TenantUid")
                        .HasColumnName("tenant_uid");

                    b.Property<Guid>("Uid")
                        .HasColumnName("lead_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<Guid>("UserUid")
                        .HasColumnName("user_uid");

                    b.Property<string>("ZipCode")
                        .HasColumnName("zip_code");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("EventId");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("TenantUid", "Qualification");

                    b.HasIndex("UserUid", "CompanyName");

                    b.HasIndex("UserUid", "CreatedAt");

                    b.HasIndex("UserUid", "FirstName");

                    b.HasIndex("UserUid", "JobTitle");

                    b.HasIndex("UserUid", "LastName");

                    b.HasIndex("UserUid", "Qualification");

                    b.ToTable("leads");
                });

            modelBuilder.Entity("Avend.API.Model.LeadEmail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("lead_email_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<string>("Designation")
                        .HasColumnName("designation");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<long>("LeadId")
                        .HasColumnName("lead_id");

                    b.Property<Guid>("Uid")
                        .HasColumnName("lead_email_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("LeadId");

                    b.ToTable("lead_emails");
                });

            modelBuilder.Entity("Avend.API.Model.LeadExportStatus", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("lead_export_status_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<DateTime?>("ExportedAt")
                        .HasColumnName("exported_at");

                    b.Property<string>("ExternalUid")
                        .HasColumnName("external_uid");

                    b.Property<long?>("LeadId")
                        .HasColumnName("lead_id");

                    b.Property<Guid>("Uid")
                        .HasColumnName("lead_export_status_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<Guid?>("UserCrmConfigurationUid")
                        .HasColumnName("user_crm_configuration_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("LeadId");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("lead_export_statuses");
                });

            modelBuilder.Entity("Avend.API.Model.LeadPhone", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("lead_phone_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<string>("Designation")
                        .HasColumnName("designation");

                    b.Property<long>("LeadId")
                        .HasColumnName("lead_id");

                    b.Property<string>("Phone")
                        .HasColumnName("phone");

                    b.Property<Guid>("Uid")
                        .HasColumnName("lead_phone_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("LeadId");

                    b.ToTable("lead_phones");
                });

            modelBuilder.Entity("Avend.API.Model.LeadQuestionAnswer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("lead_question_answer_id");

                    b.Property<string>("AnswerText")
                        .HasColumnName("answer_text");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<long>("EventAnswerId")
                        .HasColumnName("event_answer_id");

                    b.Property<Guid?>("EventAnswerUid")
                        .HasColumnName("event_answer_uid");

                    b.Property<long>("EventQuestionId")
                        .HasColumnName("event_question_id");

                    b.Property<Guid?>("EventQuestionUid")
                        .HasColumnName("event_question_uid");

                    b.Property<long>("LeadId")
                        .HasColumnName("lead_id");

                    b.Property<string>("QuestionText")
                        .HasColumnName("question_text");

                    b.Property<Guid>("Uid")
                        .HasColumnName("lead_question_answer_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("EventAnswerId");

                    b.HasIndex("EventQuestionId");

                    b.HasIndex("LeadId");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("lead_question_answers");
                });

            modelBuilder.Entity("Avend.API.Model.MeetingEventShortView", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_id");

                    b.Property<string>("City")
                        .HasColumnName("city");

                    b.Property<string>("Country")
                        .HasColumnName("country");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnName("end_date");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<int>("QuestionsCount")
                        .HasColumnName("questions_count");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnName("start_date");

                    b.Property<string>("State")
                        .HasColumnName("state");

                    b.Property<string>("Type")
                        .HasColumnName("event_type");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.ToTable("events_short_view");
                });

            modelBuilder.Entity("Avend.API.Model.Resource", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("resource_id");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("Description")
                        .HasColumnName("description");

                    b.Property<long?>("EventId")
                        .HasColumnName("event_id");

                    b.Property<string>("MimeType")
                        .HasColumnName("mime_type");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<int>("OpenedCount")
                        .HasColumnName("opened_count");

                    b.Property<int>("SentCount")
                        .HasColumnName("sent_count");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<Guid?>("TenantUid")
                        .HasColumnName("tenant_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<Guid>("Uid")
                        .HasColumnName("resource_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("Url")
                        .HasColumnName("url");

                    b.Property<Guid?>("UserUid")
                        .HasColumnName("user_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("EventId");

                    b.HasIndex("TenantUid");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("Url");

                    b.HasIndex("UserUid");

                    b.HasIndex("EventId", "Status");

                    b.HasIndex("UserUid", "Status");

                    b.ToTable("resources");
                });

            modelBuilder.Entity("Avend.API.Model.SubscriptionInvite", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("invite_id");

                    b.Property<bool>("Accepted")
                        .HasColumnName("accepted");

                    b.Property<DateTime?>("AcceptedAt")
                        .HasColumnName("accepted_at");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("InviteCode")
                        .HasColumnName("invite_code");

                    b.Property<long>("SubscriptionId")
                        .HasColumnName("subscription_id");

                    b.Property<long?>("SubscriptionMemberId")
                        .HasColumnName("subscription_member_id");

                    b.Property<Guid>("Uid")
                        .HasColumnName("invite_uid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.Property<DateTime>("ValidTill")
                        .HasColumnName("valid_till");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.HasIndex("InviteCode")
                        .IsUnique();

                    b.HasIndex("SubscriptionId");

                    b.HasIndex("SubscriptionMemberId");

                    b.HasIndex("Uid")
                        .IsUnique();

                    b.ToTable("subscription_invites");
                });

            modelBuilder.Entity("Avend.API.Model.SubscriptionMember", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("subscription_member_id");

                    b.Property<string>("City")
                        .HasColumnName("city");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("Email")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("JobTitle")
                        .HasColumnName("job_title");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.Property<int>("Role")
                        .HasColumnName("role");

                    b.Property<string>("State")
                        .HasColumnName("state");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<long?>("SubscriptionId")
                        .HasColumnName("subscription_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<Guid>("UserUid")
                        .HasColumnName("user_uid");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionId");

                    b.HasIndex("UserUid", "SubscriptionId")
                        .IsUnique();

                    b.ToTable("subscription_members");
                });

            modelBuilder.Entity("Avend.API.Model.SubscriptionRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("ActiveUsersCount")
                        .HasColumnName("active_users_count");

                    b.Property<string>("AdditionalData")
                        .HasColumnName("additional_data")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnName("expires_at");

                    b.Property<string>("ExternalUid")
                        .HasColumnName("external_uid");

                    b.Property<int>("MaximumUsersCount")
                        .HasColumnName("max_users_count");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("VARCHAR(1024)");

                    b.Property<Guid>("RecurlyAccountUid")
                        .HasColumnName("recurly_account_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<int>("Service")
                        .HasColumnName("service_type");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<string>("Type")
                        .HasColumnName("type");

                    b.Property<Guid>("Uid")
                        .HasColumnName("uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("ExternalUid")
                        .IsUnique();

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.HasIndex("RecurlyAccountUid", "Status", "ExpiresAt");

                    b.ToTable("subscriptions");
                });

            modelBuilder.Entity("Avend.API.Model.Terms", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("terms_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnName("release_date");

                    b.Property<string>("TermsText")
                        .HasColumnName("terms_text");

                    b.Property<Guid>("Uid")
                        .HasColumnName("terms_uid");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("terms");
                });

            modelBuilder.Entity("Avend.API.Model.TermsAcceptance", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("terms_acceptance_id");

                    b.Property<DateTime>("AcceptedAt")
                        .HasColumnName("accepted_at");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<long>("TermsId")
                        .HasColumnName("terms_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserUid")
                        .HasColumnName("user_uid");

                    b.HasKey("Id");

                    b.ToTable("terms_acceptances");
                });

            modelBuilder.Entity("Avend.API.Model.CrmRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("user_crm_configuration_id");

                    b.Property<string>("AccessToken")
                        .HasColumnName("access_token");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<long>("CrmSystemId")
                        .HasColumnName("crm_system_id");

                    b.Property<string>("Url")
                        .HasColumnName("dynamics_365_url")
                        .HasColumnType("VARCHAR(2048)");

                    b.Property<string>("SyncFields")
                        .HasColumnName("field_mappings")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<string>("RefreshToken")
                        .HasColumnName("refresh_token");

                    b.Property<Guid>("Uid")
                        .HasColumnName("user_crm_configuration_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserUid")
                        .HasColumnName("user_uid");

                    b.HasKey("Id");

                    b.HasIndex("CrmSystemId");

                    b.HasIndex("Uid");

                    b.HasIndex("UserUid");

                    b.HasIndex("UserUid", "Name");

                    b.HasIndex("UserUid", "Uid");

                    b.ToTable("user_crm_configurations");
                });

            modelBuilder.Entity("Avend.API.Model.SettingsRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("user_settings_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<long?>("DefaultCrmId")
                        .HasColumnName("default_user_crm_configuration_id");

                    b.Property<string>("TimeZone")
                        .HasColumnName("time_zone");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserUid")
                        .HasColumnName("user_uid");

                    b.HasKey("Id");

                    b.HasIndex("DefaultCrmId");

                    b.HasIndex("UserUid");

                    b.ToTable("user_settings");
                });

            modelBuilder.Entity("Avend.API.Model.UserTransaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("user_transaction_id");

                    b.Property<string>("AdditionalData")
                        .HasColumnName("additional_data")
                        .HasColumnType("VARCHAR(MAX)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<string>("ExternalUid")
                        .HasColumnName("external_uid");

                    b.Property<int>("Service")
                        .HasColumnName("service_type");

                    b.Property<int>("Status")
                        .HasColumnName("status");

                    b.Property<long>("SubscriptionId")
                        .HasColumnName("user_subscription_id");

                    b.Property<Guid>("Uid")
                        .HasColumnName("user_transaction_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.Property<Guid>("UserUid")
                        .HasColumnName("user_uid");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionId");

                    b.ToTable("user_transactions");
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryRecord", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany("AttendeeCategories")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryOption", b =>
                {
                    b.HasOne("Avend.API.Model.AttendeeCategoryRecord", "AttendeeCategory")
                        .WithMany("Options")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryValue", b =>
                {
                    b.HasOne("Avend.API.Model.AttendeeRecord", "Attendee")
                        .WithMany("Values")
                        .HasForeignKey("AttendeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Avend.API.Model.AttendeeCategoryRecord", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("Avend.API.Model.AttendeeCategoryOption", "AttendeeCategoryOption")
                        .WithMany()
                        .HasForeignKey("CategoryOptionId");
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeRecord", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventAgendaItem", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventQuestionRecord", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany("Questions")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.AnswerChoiceRecord", b =>
                {
                    b.HasOne("Avend.API.Model.EventQuestionRecord", "EventQuestionRecord")
                        .WithMany("Choices")
                        .HasForeignKey("EventQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventRecord", b =>
                {
                    b.HasOne("Avend.API.Model.SubscriptionMember", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("Avend.API.Model.SubscriptionRecord", "Subscription")
                        .WithMany()
                        .HasForeignKey("SubscriptionId");
                });

            modelBuilder.Entity("Avend.API.Model.EventUserExpense", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventUserGoal", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.Lead", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany()
                        .HasForeignKey("EventId");
                });

            modelBuilder.Entity("Avend.API.Model.LeadEmail", b =>
                {
                    b.HasOne("Avend.API.Model.Lead", "Lead")
                        .WithMany("Emails")
                        .HasForeignKey("LeadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.LeadExportStatus", b =>
                {
                    b.HasOne("Avend.API.Model.Lead", "Lead")
                        .WithMany("ExportStatuses")
                        .HasForeignKey("LeadId");
                });

            modelBuilder.Entity("Avend.API.Model.LeadPhone", b =>
                {
                    b.HasOne("Avend.API.Model.Lead", "Lead")
                        .WithMany("Phones")
                        .HasForeignKey("LeadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.LeadQuestionAnswer", b =>
                {
                    b.HasOne("Avend.API.Model.AnswerChoiceRecord", "Answer")
                        .WithMany()
                        .HasForeignKey("EventAnswerId");

                    b.HasOne("Avend.API.Model.EventQuestionRecord", "EventQuestionRecord")
                        .WithMany()
                        .HasForeignKey("EventQuestionId");

                    b.HasOne("Avend.API.Model.Lead", "Lead")
                        .WithMany("QuestionAnswers")
                        .HasForeignKey("LeadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.SubscriptionInvite", b =>
                {
                    b.HasOne("Avend.API.Model.SubscriptionRecord", "Subscription")
                        .WithMany()
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Avend.API.Model.SubscriptionMember", "SubscriptionMember")
                        .WithMany()
                        .HasForeignKey("SubscriptionMemberId");
                });

            modelBuilder.Entity("Avend.API.Model.SubscriptionMember", b =>
                {
                    b.HasOne("Avend.API.Model.SubscriptionRecord", "Subscription")
                        .WithMany()
                        .HasForeignKey("SubscriptionId");
                });

            modelBuilder.Entity("Avend.API.Model.CrmRecord", b =>
                {
                    b.HasOne("Avend.API.Model.CrmSystem", "CrmSystem")
                        .WithMany()
                        .HasForeignKey("CrmSystemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.SettingsRecord", b =>
                {
                    b.HasOne("Avend.API.Model.CrmRecord", "DefaultCrm")
                        .WithMany()
                        .HasForeignKey("DefaultCrmId");
                });

            modelBuilder.Entity("Avend.API.Model.UserTransaction", b =>
                {
                    b.HasOne("Avend.API.Model.SubscriptionRecord", "Subscription")
                        .WithMany()
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
