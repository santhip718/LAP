using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUrlFromFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url",
                schema: "LAP",
                table: "feature");

            migrationBuilder.CreateIndex(
                name: "i_x_user_secret_is_active",
                schema: "LAP",
                table: "user_secret",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_user_role_mapping_is_active",
                schema: "LAP",
                table: "user_role_mapping",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_user_role_mapping_user_id_role_id",
                schema: "LAP",
                table: "user_role_mapping",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_user_course_progress_enrollment_id_course_content_id",
                schema: "LAP",
                table: "user_course_progress",
                columns: new[] { "enrollment_id", "course_content_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_user_course_progress_is_active",
                schema: "LAP",
                table: "user_course_progress",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_user_course_progress_is_completed",
                schema: "LAP",
                table: "user_course_progress",
                column: "is_completed");

            migrationBuilder.CreateIndex(
                name: "i_x_user_is_active",
                schema: "LAP",
                table: "user",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_role_feature_mapping_is_active",
                schema: "LAP",
                table: "role_feature_mapping",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_role_feature_mapping_role_id_feature_id",
                schema: "LAP",
                table: "role_feature_mapping",
                columns: new[] { "role_id", "feature_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_review_is_active",
                schema: "LAP",
                table: "review",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_review_user_id_course_id",
                schema: "LAP",
                table: "review",
                columns: new[] { "user_id", "course_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_refresh_token_is_active",
                schema: "LAP",
                table: "refresh_token",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_refresh_token_token",
                schema: "LAP",
                table: "refresh_token",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_ref_term_is_active",
                schema: "LAP",
                table: "ref_term",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_ref_set_is_active",
                schema: "LAP",
                table: "ref_set",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_question_assessment_id_meta_topic_id",
                schema: "LAP",
                table: "question",
                columns: new[] { "assessment_id", "meta_topic_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_question_is_active",
                schema: "LAP",
                table: "question",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_person_email",
                schema: "LAP",
                table: "person",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_person_is_active",
                schema: "LAP",
                table: "person",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_import_job_is_active",
                schema: "LAP",
                table: "import_job",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_forum_message_course_id_user_id",
                schema: "LAP",
                table: "forum_message",
                columns: new[] { "course_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_forum_message_is_active",
                schema: "LAP",
                table: "forum_message",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_feature_is_active",
                schema: "LAP",
                table: "feature",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_feature_method",
                schema: "LAP",
                table: "feature",
                column: "method");

            migrationBuilder.CreateIndex(
                name: "i_x_enrollment_completed_on",
                schema: "LAP",
                table: "enrollment",
                column: "completed_on");

            migrationBuilder.CreateIndex(
                name: "i_x_enrollment_enrolled_on",
                schema: "LAP",
                table: "enrollment",
                column: "enrolled_on");

            migrationBuilder.CreateIndex(
                name: "i_x_enrollment_is_active",
                schema: "LAP",
                table: "enrollment",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_enrollment_user_id_course_id",
                schema: "LAP",
                table: "enrollment",
                columns: new[] { "user_id", "course_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_course_meta_topic_course_id_sequence_order",
                schema: "LAP",
                table: "course_meta_topic",
                columns: new[] { "course_id", "sequence_order" });

            migrationBuilder.CreateIndex(
                name: "i_x_course_meta_topic_is_active",
                schema: "LAP",
                table: "course_meta_topic",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_course_content_is_active",
                schema: "LAP",
                table: "course_content",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_course_content_meta_topic_id_sequence_order",
                schema: "LAP",
                table: "course_content",
                columns: new[] { "meta_topic_id", "sequence_order" });

            migrationBuilder.CreateIndex(
                name: "i_x_course_is_active",
                schema: "LAP",
                table: "course",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_course_title",
                schema: "LAP",
                table: "course",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_history_completed_on",
                schema: "LAP",
                table: "assessment_history",
                column: "completed_on");

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_history_is_active",
                schema: "LAP",
                table: "assessment_history",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_history_started_on",
                schema: "LAP",
                table: "assessment_history",
                column: "started_on");

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_history_user_id_assessment_id",
                schema: "LAP",
                table: "assessment_history",
                columns: new[] { "user_id", "assessment_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_answer_is_active",
                schema: "LAP",
                table: "assessment_answer",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_is_active",
                schema: "LAP",
                table: "assessment",
                column: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_user_secret_is_active",
                schema: "LAP",
                table: "user_secret");

            migrationBuilder.DropIndex(
                name: "i_x_user_role_mapping_is_active",
                schema: "LAP",
                table: "user_role_mapping");

            migrationBuilder.DropIndex(
                name: "i_x_user_role_mapping_user_id_role_id",
                schema: "LAP",
                table: "user_role_mapping");

            migrationBuilder.DropIndex(
                name: "i_x_user_course_progress_enrollment_id_course_content_id",
                schema: "LAP",
                table: "user_course_progress");

            migrationBuilder.DropIndex(
                name: "i_x_user_course_progress_is_active",
                schema: "LAP",
                table: "user_course_progress");

            migrationBuilder.DropIndex(
                name: "i_x_user_course_progress_is_completed",
                schema: "LAP",
                table: "user_course_progress");

            migrationBuilder.DropIndex(
                name: "i_x_user_is_active",
                schema: "LAP",
                table: "user");

            migrationBuilder.DropIndex(
                name: "i_x_role_feature_mapping_is_active",
                schema: "LAP",
                table: "role_feature_mapping");

            migrationBuilder.DropIndex(
                name: "i_x_role_feature_mapping_role_id_feature_id",
                schema: "LAP",
                table: "role_feature_mapping");

            migrationBuilder.DropIndex(
                name: "i_x_review_is_active",
                schema: "LAP",
                table: "review");

            migrationBuilder.DropIndex(
                name: "i_x_review_user_id_course_id",
                schema: "LAP",
                table: "review");

            migrationBuilder.DropIndex(
                name: "i_x_refresh_token_is_active",
                schema: "LAP",
                table: "refresh_token");

            migrationBuilder.DropIndex(
                name: "i_x_refresh_token_token",
                schema: "LAP",
                table: "refresh_token");

            migrationBuilder.DropIndex(
                name: "i_x_ref_term_is_active",
                schema: "LAP",
                table: "ref_term");

            migrationBuilder.DropIndex(
                name: "i_x_ref_set_is_active",
                schema: "LAP",
                table: "ref_set");

            migrationBuilder.DropIndex(
                name: "i_x_question_assessment_id_meta_topic_id",
                schema: "LAP",
                table: "question");

            migrationBuilder.DropIndex(
                name: "i_x_question_is_active",
                schema: "LAP",
                table: "question");

            migrationBuilder.DropIndex(
                name: "i_x_person_email",
                schema: "LAP",
                table: "person");

            migrationBuilder.DropIndex(
                name: "i_x_person_is_active",
                schema: "LAP",
                table: "person");

            migrationBuilder.DropIndex(
                name: "i_x_import_job_is_active",
                schema: "LAP",
                table: "import_job");

            migrationBuilder.DropIndex(
                name: "i_x_forum_message_course_id_user_id",
                schema: "LAP",
                table: "forum_message");

            migrationBuilder.DropIndex(
                name: "i_x_forum_message_is_active",
                schema: "LAP",
                table: "forum_message");

            migrationBuilder.DropIndex(
                name: "i_x_feature_is_active",
                schema: "LAP",
                table: "feature");

            migrationBuilder.DropIndex(
                name: "i_x_feature_method",
                schema: "LAP",
                table: "feature");

            migrationBuilder.DropIndex(
                name: "i_x_enrollment_completed_on",
                schema: "LAP",
                table: "enrollment");

            migrationBuilder.DropIndex(
                name: "i_x_enrollment_enrolled_on",
                schema: "LAP",
                table: "enrollment");

            migrationBuilder.DropIndex(
                name: "i_x_enrollment_is_active",
                schema: "LAP",
                table: "enrollment");

            migrationBuilder.DropIndex(
                name: "i_x_enrollment_user_id_course_id",
                schema: "LAP",
                table: "enrollment");

            migrationBuilder.DropIndex(
                name: "i_x_course_meta_topic_course_id_sequence_order",
                schema: "LAP",
                table: "course_meta_topic");

            migrationBuilder.DropIndex(
                name: "i_x_course_meta_topic_is_active",
                schema: "LAP",
                table: "course_meta_topic");

            migrationBuilder.DropIndex(
                name: "i_x_course_content_is_active",
                schema: "LAP",
                table: "course_content");

            migrationBuilder.DropIndex(
                name: "i_x_course_content_meta_topic_id_sequence_order",
                schema: "LAP",
                table: "course_content");

            migrationBuilder.DropIndex(
                name: "i_x_course_is_active",
                schema: "LAP",
                table: "course");

            migrationBuilder.DropIndex(
                name: "i_x_course_title",
                schema: "LAP",
                table: "course");

            migrationBuilder.DropIndex(
                name: "i_x_assessment_history_completed_on",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropIndex(
                name: "i_x_assessment_history_is_active",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropIndex(
                name: "i_x_assessment_history_started_on",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropIndex(
                name: "i_x_assessment_history_user_id_assessment_id",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropIndex(
                name: "i_x_assessment_answer_is_active",
                schema: "LAP",
                table: "assessment_answer");

            migrationBuilder.DropIndex(
                name: "i_x_assessment_is_active",
                schema: "LAP",
                table: "assessment");

            migrationBuilder.AddColumn<string>(
                name: "url",
                schema: "LAP",
                table: "feature",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
