using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Avend.API.Migrations
{
    public partial class SubscriptionMembersViewFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[subscription_members_and_invites]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT')) DROP FUNCTION subscription_members_and_invites");
            migrationBuilder.Sql(@"
CREATE FUNCTION subscription_members_and_invites ()
RETURNS TABLE
AS
RETURN
    SELECT
        [subscription_member_id],
        [user_uid],
        [status],
        [subscription_id],
        [role],
        [first_name],
        [last_name],
        [email],
        [job_title],
        [city],
        [state],
        [created_at],
        [updated_at]
    FROM subscription_members
    UNION ALL
    SELECT 
        0 as [subscription_member_id],
        invite_uid as [user_uid],
        2 as [status],
        [subscription_id],
        0 as [role],
        NULL as [first_name],
        NULL as [last_name],
        [email],
        NULL as [job_title],
        NULL as [city],
        NULL as [state],
        [created_at],
        [updated_at] 
    FROM subscription_invites WHERE accepted = 0
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION subscription_members_and_invites");
        }
    }
}
