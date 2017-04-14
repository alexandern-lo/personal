using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class QuestionsAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_question_answers_event_questions_event_question_id",
                table: "event_question_answers");

            migrationBuilder.RenameColumn(
                name: "event_question_uid",
                table: "event_questions",
                newName: "uid");

            migrationBuilder.RenameColumn(
                name: "question_text",
                table: "event_questions",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "event_question_id",
                table: "event_questions",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_event_questions_event_question_uid",
                table: "event_questions",
                newName: "IX_event_questions_uid");

            migrationBuilder.RenameColumn(
                name: "event_question_answer_uid",
                table: "event_question_answers",
                newName: "uid");

            migrationBuilder.RenameColumn(
                name: "answer_text",
                table: "event_question_answers",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "event_question_id",
                table: "event_question_answers",
                newName: "question_id");

            migrationBuilder.RenameColumn(
                name: "event_question_answer_id",
                table: "event_question_answers",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_event_question_answers_event_question_answer_uid",
                table: "event_question_answers",
                newName: "IX_event_question_answers_uid");

            migrationBuilder.RenameIndex(
                name: "IX_event_question_answers_event_question_id",
                table: "event_question_answers",
                newName: "IX_event_question_answers_question_id");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "event_questions",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_event_questions_user_id_event_id",
                table: "event_questions",
                columns: new[] { "user_id", "event_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_event_question_answers_event_questions_question_id",
                table: "event_question_answers",
                column: "question_id",
                principalTable: "event_questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_event_questions_subscription_members_user_id",
                table: "event_questions",
                column: "user_id",
                principalTable: "subscription_members",
                principalColumn: "subscription_member_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_question_answers_event_questions_question_id",
                table: "event_question_answers");

            migrationBuilder.DropForeignKey(
                name: "FK_event_questions_subscription_members_user_id",
                table: "event_questions");

            migrationBuilder.DropIndex(
                name: "IX_event_questions_user_id_event_id",
                table: "event_questions");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "event_questions");

            migrationBuilder.RenameColumn(
                name: "uid",
                table: "event_questions",
                newName: "event_question_uid");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "event_questions",
                newName: "question_text");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "event_questions",
                newName: "event_question_id");

            migrationBuilder.RenameIndex(
                name: "IX_event_questions_uid",
                table: "event_questions",
                newName: "IX_event_questions_event_question_uid");

            migrationBuilder.RenameColumn(
                name: "uid",
                table: "event_question_answers",
                newName: "event_question_answer_uid");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "event_question_answers",
                newName: "answer_text");

            migrationBuilder.RenameColumn(
                name: "question_id",
                table: "event_question_answers",
                newName: "event_question_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "event_question_answers",
                newName: "event_question_answer_id");

            migrationBuilder.RenameIndex(
                name: "IX_event_question_answers_uid",
                table: "event_question_answers",
                newName: "IX_event_question_answers_event_question_answer_uid");

            migrationBuilder.RenameIndex(
                name: "IX_event_question_answers_question_id",
                table: "event_question_answers",
                newName: "IX_event_question_answers_event_question_id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_question_answers_event_questions_event_question_id",
                table: "event_question_answers",
                column: "event_question_id",
                principalTable: "event_questions",
                principalColumn: "event_question_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
