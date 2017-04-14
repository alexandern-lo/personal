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
    [Migration("20170210013339_UpdateLeadsAddStatus")]
    partial class UpdateLeadsAddStatus
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
                        .HasColumnName("event_attendee_category_id");

                    b.Property<long>("EventId")
                        .HasColumnName("event_id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<int>("Position")
                        .HasColumnName("position");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_attendee_category_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("event_attendee_categories");
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryOption", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_attendee_category_option_id");

                    b.Property<long>("AttendeeCategoryId")
                        .HasColumnName("event_attendee_category_id");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<int>("Position")
                        .HasColumnName("position");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_attendee_category_option_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeCategoryId");

                    b.ToTable("event_attendee_category_options");
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_attendee_category_value_id");

                    b.Property<long?>("AttendeeCategoryId")
                        .HasColumnName("event_attendee_category_id");

                    b.Property<long?>("AttendeeCategoryOptionId")
                        .HasColumnName("event_attendee_category_option_id");

                    b.Property<string>("CategoryValue")
                        .HasColumnName("category_value");

                    b.Property<long>("EventAttendeeId")
                        .HasColumnName("event_attendee_id");

                    b.Property<string>("OptionValue")
                        .HasColumnName("option_value");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_attendee_category_value_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeCategoryId");

                    b.HasIndex("AttendeeCategoryOptionId");

                    b.HasIndex("EventAttendeeId");

                    b.HasIndex("AttendeeCategoryId", "AttendeeCategoryOptionId");

                    b.HasIndex("EventAttendeeId", "AttendeeCategoryId", "AttendeeCategoryOptionId");

                    b.ToTable("event_attendee_category_values");
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

            modelBuilder.Entity("Avend.API.Model.EventAttendee", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_attendee_id");

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
                        .HasColumnName("event_attendee_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<string>("ZipCode")
                        .HasColumnName("zipcode");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("EventId", "Company", "Id");

                    b.HasIndex("EventId", "Email", "Id");

                    b.HasIndex("EventId", "FirstName", "Id");

                    b.HasIndex("EventId", "LastName", "Id");

                    b.ToTable("event_attendees");
                });

            modelBuilder.Entity("Avend.API.Model.EventQuestion", b =>
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

            modelBuilder.Entity("Avend.API.Model.EventQuestionAnswer", b =>
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

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("status")
                        .HasAnnotation("SqlServer:DefaultValueSql", "1");

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

            modelBuilder.Entity("Avend.API.Model.EventRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("event_id");

                    b.Property<string>("Address")
                        .HasColumnName("address");

                    b.Property<string>("City")
                        .HasColumnName("city");

                    b.Property<string>("Country")
                        .HasColumnName("country");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnName("end_date");

                    b.Property<Guid?>("GroupUid")
                        .HasColumnName("group_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<string>("Industry")
                        .HasColumnName("industry");

                    b.Property<string>("LogoUrl")
                        .HasColumnName("logo_url");

                    b.Property<string>("Name")
                        .HasColumnName("name");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnName("start_date");

                    b.Property<string>("State")
                        .HasColumnName("state");

                    b.Property<string>("Type")
                        .HasColumnName("event_type");

                    b.Property<Guid>("Uid")
                        .HasColumnName("event_uid")
                        .HasColumnType("UniqueIdentifier");

                    b.Property<DateTime?>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnName("updated_at")
                        .HasAnnotation("SqlServer:DefaultValueSql", "GETUTCDATE()");

                    b.Property<string>("VenueName")
                        .HasColumnName("venue_name");

                    b.Property<string>("WebsiteUrl")
                        .HasColumnName("website_url");

                    b.Property<string>("ZipCode")
                        .HasColumnName("zip_code");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("Uid");

                    b.HasIndex("UpdatedAt");

                    b.ToTable("events");
                });

            modelBuilder.Entity("Avend.API.Model.EventRecordShortView", b =>
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
                        .HasForeignKey("AttendeeCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.AttendeeCategoryValue", b =>
                {
                    b.HasOne("Avend.API.Model.AttendeeCategoryRecord", "AttendeeCategory")
                        .WithMany()
                        .HasForeignKey("AttendeeCategoryId");

                    b.HasOne("Avend.API.Model.AttendeeCategoryOption", "AttendeeCategoryOption")
                        .WithMany()
                        .HasForeignKey("AttendeeCategoryOptionId");

                    b.HasOne("Avend.API.Model.EventAttendee", "EventAttendee")
                        .WithMany("CategoryValues")
                        .HasForeignKey("EventAttendeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventAgendaItem", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventAttendee", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventQuestion", b =>
                {
                    b.HasOne("Avend.API.Model.EventRecord", "Event")
                        .WithMany("Questions")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Avend.API.Model.EventQuestionAnswer", b =>
                {
                    b.HasOne("Avend.API.Model.EventQuestion", "EventQuestion")
                        .WithMany("Answers")
                        .HasForeignKey("EventQuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
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
                    b.HasOne("Avend.API.Model.EventQuestionAnswer", "EventAnswer")
                        .WithMany()
                        .HasForeignKey("EventAnswerId");

                    b.HasOne("Avend.API.Model.EventQuestion", "EventQuestion")
                        .WithMany()
                        .HasForeignKey("EventQuestionId");

                    b.HasOne("Avend.API.Model.Lead", "Lead")
                        .WithMany("QuestionAnswers")
                        .HasForeignKey("LeadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
