using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_answers__assessment_histories_assessment_history_id",
                schema: "LAP",
                table: "assessment_answers");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_answers__questions_question_id",
                schema: "LAP",
                table: "assessment_answers");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_histories__ref_terms_tier_awarded_id",
                schema: "LAP",
                table: "assessment_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_histories__users_user_id",
                schema: "LAP",
                table: "assessment_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_histories_assessments_assessment_id",
                schema: "LAP",
                table: "assessment_histories");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessments__courses_course_id",
                schema: "LAP",
                table: "assessments");

            migrationBuilder.DropForeignKey(
                name: "f_k_course_contents__course_meta_topics_meta_topic_id",
                schema: "LAP",
                table: "course_contents");

            migrationBuilder.DropForeignKey(
                name: "f_k_course_contents__ref_terms_content_type_id",
                schema: "LAP",
                table: "course_contents");

            migrationBuilder.DropForeignKey(
                name: "f_k_course_meta_topics_courses_course_id",
                schema: "LAP",
                table: "course_meta_topics");

            migrationBuilder.DropForeignKey(
                name: "f_k_courses__ref_terms_category_id",
                schema: "LAP",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "f_k_courses__ref_terms_difficulty_level_id",
                schema: "LAP",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "f_k_courses__ref_terms_sub_category_id",
                schema: "LAP",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "f_k_courses__users_created_by_user_id",
                schema: "LAP",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "f_k_enrollments__users_user_id",
                schema: "LAP",
                table: "enrollments");

            migrationBuilder.DropForeignKey(
                name: "f_k_enrollments_courses_course_id",
                schema: "LAP",
                table: "enrollments");

            migrationBuilder.DropForeignKey(
                name: "f_k_forum_messages__users_user_id",
                schema: "LAP",
                table: "forum_messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_forum_messages_courses_course_id",
                schema: "LAP",
                table: "forum_messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_import_jobs__ref_terms_status_id",
                schema: "LAP",
                table: "import_jobs");

            migrationBuilder.DropForeignKey(
                name: "f_k_import_jobs_assessments_assessment_id",
                schema: "LAP",
                table: "import_jobs");

            migrationBuilder.DropForeignKey(
                name: "f_k_persons__ref_terms_designation_id",
                schema: "LAP",
                table: "persons");

            migrationBuilder.DropForeignKey(
                name: "f_k_persons__ref_terms_gender_id",
                schema: "LAP",
                table: "persons");

            migrationBuilder.DropForeignKey(
                name: "f_k_questions__ref_terms_question_type_id",
                schema: "LAP",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "f_k_questions_assessments_assessment_id",
                schema: "LAP",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "f_k_questions_course_meta_topics_meta_topic_id",
                schema: "LAP",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "f_k_ref_terms_ref_sets_ref_set_id",
                schema: "LAP",
                table: "ref_terms");

            migrationBuilder.DropForeignKey(
                name: "f_k_refresh_tokens__users_user_id",
                schema: "LAP",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "f_k_reviews__users_user_id",
                schema: "LAP",
                table: "reviews");

            migrationBuilder.DropForeignKey(
                name: "f_k_reviews_courses_course_id",
                schema: "LAP",
                table: "reviews");

            migrationBuilder.DropForeignKey(
                name: "f_k_role_feature_mappings_features_feature_id",
                schema: "LAP",
                table: "role_feature_mappings");

            migrationBuilder.DropForeignKey(
                name: "f_k_role_feature_mappings_ref_terms_role_id",
                schema: "LAP",
                table: "role_feature_mappings");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_course_progresses_course_contents_course_content_id",
                schema: "LAP",
                table: "user_course_progresses");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_course_progresses_enrollments_enrollment_id",
                schema: "LAP",
                table: "user_course_progresses");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_role_mappings_ref_terms_role_id",
                schema: "LAP",
                table: "user_role_mappings");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_role_mappings_users_user_id",
                schema: "LAP",
                table: "user_role_mappings");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_secrets_users_user_id",
                schema: "LAP",
                table: "user_secrets");

            migrationBuilder.DropForeignKey(
                name: "f_k_users_persons_person_id",
                schema: "LAP",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "f_k_users_ref_terms_current_tier_id",
                schema: "LAP",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_users",
                schema: "LAP",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_secrets",
                schema: "LAP",
                table: "user_secrets");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_role_mappings",
                schema: "LAP",
                table: "user_role_mappings");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_course_progresses",
                schema: "LAP",
                table: "user_course_progresses");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_role_feature_mappings",
                schema: "LAP",
                table: "role_feature_mappings");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_reviews",
                schema: "LAP",
                table: "reviews");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_refresh_tokens",
                schema: "LAP",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_ref_terms",
                schema: "LAP",
                table: "ref_terms");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_ref_sets",
                schema: "LAP",
                table: "ref_sets");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_questions",
                schema: "LAP",
                table: "questions");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_persons",
                schema: "LAP",
                table: "persons");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_import_jobs",
                schema: "LAP",
                table: "import_jobs");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_forum_messages",
                schema: "LAP",
                table: "forum_messages");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_features",
                schema: "LAP",
                table: "features");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_enrollments",
                schema: "LAP",
                table: "enrollments");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_courses",
                schema: "LAP",
                table: "courses");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_course_meta_topics",
                schema: "LAP",
                table: "course_meta_topics");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_course_contents",
                schema: "LAP",
                table: "course_contents");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_assessments",
                schema: "LAP",
                table: "assessments");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_assessment_histories",
                schema: "LAP",
                table: "assessment_histories");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_assessment_answers",
                schema: "LAP",
                table: "assessment_answers");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "LAP",
                newName: "user",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "user_secrets",
                schema: "LAP",
                newName: "user_secret",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "user_role_mappings",
                schema: "LAP",
                newName: "user_role_mapping",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "user_course_progresses",
                schema: "LAP",
                newName: "user_course_progress",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "role_feature_mappings",
                schema: "LAP",
                newName: "role_feature_mapping",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "reviews",
                schema: "LAP",
                newName: "review",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                schema: "LAP",
                newName: "refresh_token",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "ref_terms",
                schema: "LAP",
                newName: "ref_term",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "ref_sets",
                schema: "LAP",
                newName: "ref_set",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "questions",
                schema: "LAP",
                newName: "question",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "persons",
                schema: "LAP",
                newName: "person",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "import_jobs",
                schema: "LAP",
                newName: "import_job",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "forum_messages",
                schema: "LAP",
                newName: "forum_message",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "features",
                schema: "LAP",
                newName: "feature",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "enrollments",
                schema: "LAP",
                newName: "enrollment",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "courses",
                schema: "LAP",
                newName: "course",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "course_meta_topics",
                schema: "LAP",
                newName: "course_meta_topic",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "course_contents",
                schema: "LAP",
                newName: "course_content",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "assessments",
                schema: "LAP",
                newName: "assessment",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "assessment_histories",
                schema: "LAP",
                newName: "assessment_history",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "assessment_answers",
                schema: "LAP",
                newName: "assessment_answer",
                newSchema: "LAP");

            migrationBuilder.RenameIndex(
                name: "i_x_users_person_id",
                schema: "LAP",
                table: "user",
                newName: "i_x_user_person_id");

            migrationBuilder.RenameIndex(
                name: "i_x_users_current_tier_id",
                schema: "LAP",
                table: "user",
                newName: "i_x_user_current_tier_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_secrets_user_id",
                schema: "LAP",
                table: "user_secret",
                newName: "i_x_user_secret_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_role_mappings_user_id",
                schema: "LAP",
                table: "user_role_mapping",
                newName: "i_x_user_role_mapping_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_role_mappings_role_id",
                schema: "LAP",
                table: "user_role_mapping",
                newName: "i_x_user_role_mapping_role_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_course_progresses_enrollment_id",
                schema: "LAP",
                table: "user_course_progress",
                newName: "i_x_user_course_progress_enrollment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_course_progresses_course_content_id",
                schema: "LAP",
                table: "user_course_progress",
                newName: "i_x_user_course_progress_course_content_id");

            migrationBuilder.RenameIndex(
                name: "i_x_role_feature_mappings_role_id",
                schema: "LAP",
                table: "role_feature_mapping",
                newName: "i_x_role_feature_mapping_role_id");

            migrationBuilder.RenameIndex(
                name: "i_x_role_feature_mappings_feature_id",
                schema: "LAP",
                table: "role_feature_mapping",
                newName: "i_x_role_feature_mapping_feature_id");

            migrationBuilder.RenameIndex(
                name: "i_x_reviews_user_id",
                schema: "LAP",
                table: "review",
                newName: "i_x_review_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_reviews_course_id",
                schema: "LAP",
                table: "review",
                newName: "i_x_review_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_refresh_tokens_user_id",
                schema: "LAP",
                table: "refresh_token",
                newName: "i_x_refresh_token_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_ref_terms_ref_set_id",
                schema: "LAP",
                table: "ref_term",
                newName: "i_x_ref_term_ref_set_id");

            migrationBuilder.RenameIndex(
                name: "i_x_questions_question_type_id",
                schema: "LAP",
                table: "question",
                newName: "i_x_question_question_type_id");

            migrationBuilder.RenameIndex(
                name: "i_x_questions_meta_topic_id",
                schema: "LAP",
                table: "question",
                newName: "i_x_question_meta_topic_id");

            migrationBuilder.RenameIndex(
                name: "i_x_questions_assessment_id",
                schema: "LAP",
                table: "question",
                newName: "i_x_question_assessment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_persons_gender_id",
                schema: "LAP",
                table: "person",
                newName: "i_x_person_gender_id");

            migrationBuilder.RenameIndex(
                name: "i_x_persons_designation_id",
                schema: "LAP",
                table: "person",
                newName: "i_x_person_designation_id");

            migrationBuilder.RenameIndex(
                name: "i_x_import_jobs_status_id",
                schema: "LAP",
                table: "import_job",
                newName: "i_x_import_job_status_id");

            migrationBuilder.RenameIndex(
                name: "i_x_import_jobs_assessment_id",
                schema: "LAP",
                table: "import_job",
                newName: "i_x_import_job_assessment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_forum_messages_user_id",
                schema: "LAP",
                table: "forum_message",
                newName: "i_x_forum_message_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_forum_messages_course_id",
                schema: "LAP",
                table: "forum_message",
                newName: "i_x_forum_message_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_enrollments_user_id",
                schema: "LAP",
                table: "enrollment",
                newName: "i_x_enrollment_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_enrollments_course_id",
                schema: "LAP",
                table: "enrollment",
                newName: "i_x_enrollment_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_courses_sub_category_id",
                schema: "LAP",
                table: "course",
                newName: "i_x_course_sub_category_id");

            migrationBuilder.RenameIndex(
                name: "i_x_courses_difficulty_level_id",
                schema: "LAP",
                table: "course",
                newName: "i_x_course_difficulty_level_id");

            migrationBuilder.RenameIndex(
                name: "i_x_courses_created_by_user_id",
                schema: "LAP",
                table: "course",
                newName: "i_x_course_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_courses_category_id",
                schema: "LAP",
                table: "course",
                newName: "i_x_course_category_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_meta_topics_course_id",
                schema: "LAP",
                table: "course_meta_topic",
                newName: "i_x_course_meta_topic_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_contents_meta_topic_id",
                schema: "LAP",
                table: "course_content",
                newName: "i_x_course_content_meta_topic_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_contents_content_type_id",
                schema: "LAP",
                table: "course_content",
                newName: "i_x_course_content_content_type_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessments_course_id",
                schema: "LAP",
                table: "assessment",
                newName: "i_x_assessment_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_histories_user_id",
                schema: "LAP",
                table: "assessment_history",
                newName: "i_x_assessment_history_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_histories_tier_awarded_id",
                schema: "LAP",
                table: "assessment_history",
                newName: "i_x_assessment_history_tier_awarded_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_histories_assessment_id",
                schema: "LAP",
                table: "assessment_history",
                newName: "i_x_assessment_history_assessment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_answers_question_id",
                schema: "LAP",
                table: "assessment_answer",
                newName: "i_x_assessment_answer_question_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_answers_assessment_history_id",
                schema: "LAP",
                table: "assessment_answer",
                newName: "i_x_assessment_answer_assessment_history_id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user",
                schema: "LAP",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_secret",
                schema: "LAP",
                table: "user_secret",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_role_mapping",
                schema: "LAP",
                table: "user_role_mapping",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_course_progress",
                schema: "LAP",
                table: "user_course_progress",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_role_feature_mapping",
                schema: "LAP",
                table: "role_feature_mapping",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_review",
                schema: "LAP",
                table: "review",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_refresh_token",
                schema: "LAP",
                table: "refresh_token",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_ref_term",
                schema: "LAP",
                table: "ref_term",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_ref_set",
                schema: "LAP",
                table: "ref_set",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_question",
                schema: "LAP",
                table: "question",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_person",
                schema: "LAP",
                table: "person",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_import_job",
                schema: "LAP",
                table: "import_job",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_forum_message",
                schema: "LAP",
                table: "forum_message",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_feature",
                schema: "LAP",
                table: "feature",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_enrollment",
                schema: "LAP",
                table: "enrollment",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_course",
                schema: "LAP",
                table: "course",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_course_meta_topic",
                schema: "LAP",
                table: "course_meta_topic",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_course_content",
                schema: "LAP",
                table: "course_content",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_assessment",
                schema: "LAP",
                table: "assessment",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_assessment_history",
                schema: "LAP",
                table: "assessment_history",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_assessment_answer",
                schema: "LAP",
                table: "assessment_answer",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment__course_course_id",
                schema: "LAP",
                table: "assessment",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "course",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_answer__assessment_history_assessment_history_id",
                schema: "LAP",
                table: "assessment_answer",
                column: "assessment_history_id",
                principalSchema: "LAP",
                principalTable: "assessment_history",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_answer__question_question_id",
                schema: "LAP",
                table: "assessment_answer",
                column: "question_id",
                principalSchema: "LAP",
                principalTable: "question",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_history__ref_term_tier_awarded_id",
                schema: "LAP",
                table: "assessment_history",
                column: "tier_awarded_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_history__user_user_id",
                schema: "LAP",
                table: "assessment_history",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_history_assessment_assessment_id",
                schema: "LAP",
                table: "assessment_history",
                column: "assessment_id",
                principalSchema: "LAP",
                principalTable: "assessment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course__ref_term_category_id",
                schema: "LAP",
                table: "course",
                column: "category_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course__ref_term_difficulty_level_id",
                schema: "LAP",
                table: "course",
                column: "difficulty_level_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course__ref_term_sub_category_id",
                schema: "LAP",
                table: "course",
                column: "sub_category_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course__user_created_by_user_id",
                schema: "LAP",
                table: "course",
                column: "created_by_user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course_content__course_meta_topic_meta_topic_id",
                schema: "LAP",
                table: "course_content",
                column: "meta_topic_id",
                principalSchema: "LAP",
                principalTable: "course_meta_topic",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course_content__ref_term_content_type_id",
                schema: "LAP",
                table: "course_content",
                column: "content_type_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course_meta_topic_course_course_id",
                schema: "LAP",
                table: "course_meta_topic",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "course",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_enrollment__user_user_id",
                schema: "LAP",
                table: "enrollment",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_enrollment_course_course_id",
                schema: "LAP",
                table: "enrollment",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "course",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_forum_message__user_user_id",
                schema: "LAP",
                table: "forum_message",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_forum_message_course_course_id",
                schema: "LAP",
                table: "forum_message",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "course",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_import_job__ref_term_status_id",
                schema: "LAP",
                table: "import_job",
                column: "status_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_import_job_assessment_assessment_id",
                schema: "LAP",
                table: "import_job",
                column: "assessment_id",
                principalSchema: "LAP",
                principalTable: "assessment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_person__ref_term_designation_id",
                schema: "LAP",
                table: "person",
                column: "designation_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_person__ref_term_gender_id",
                schema: "LAP",
                table: "person",
                column: "gender_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_question__ref_term_question_type_id",
                schema: "LAP",
                table: "question",
                column: "question_type_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_question_assessment_assessment_id",
                schema: "LAP",
                table: "question",
                column: "assessment_id",
                principalSchema: "LAP",
                principalTable: "assessment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_question_course_meta_topic_meta_topic_id",
                schema: "LAP",
                table: "question",
                column: "meta_topic_id",
                principalSchema: "LAP",
                principalTable: "course_meta_topic",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_ref_term_ref_set_ref_set_id",
                schema: "LAP",
                table: "ref_term",
                column: "ref_set_id",
                principalSchema: "LAP",
                principalTable: "ref_set",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_refresh_token__user_user_id",
                schema: "LAP",
                table: "refresh_token",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_review__user_user_id",
                schema: "LAP",
                table: "review",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_review_course_course_id",
                schema: "LAP",
                table: "review",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "course",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_role_feature_mapping_feature_feature_id",
                schema: "LAP",
                table: "role_feature_mapping",
                column: "feature_id",
                principalSchema: "LAP",
                principalTable: "feature",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_role_feature_mapping_ref_term_role_id",
                schema: "LAP",
                table: "role_feature_mapping",
                column: "role_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_person_person_id",
                schema: "LAP",
                table: "user",
                column: "person_id",
                principalSchema: "LAP",
                principalTable: "person",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_ref_term_current_tier_id",
                schema: "LAP",
                table: "user",
                column: "current_tier_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_user_course_progress_course_content_course_content_id",
                schema: "LAP",
                table: "user_course_progress",
                column: "course_content_id",
                principalSchema: "LAP",
                principalTable: "course_content",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_course_progress_enrollment_enrollment_id",
                schema: "LAP",
                table: "user_course_progress",
                column: "enrollment_id",
                principalSchema: "LAP",
                principalTable: "enrollment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_role_mapping_ref_term_role_id",
                schema: "LAP",
                table: "user_role_mapping",
                column: "role_id",
                principalSchema: "LAP",
                principalTable: "ref_term",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_role_mapping_user_user_id",
                schema: "LAP",
                table: "user_role_mapping",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_secret_user_user_id",
                schema: "LAP",
                table: "user_secret",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_assessment__course_course_id",
                schema: "LAP",
                table: "assessment");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_answer__assessment_history_assessment_history_id",
                schema: "LAP",
                table: "assessment_answer");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_answer__question_question_id",
                schema: "LAP",
                table: "assessment_answer");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_history__ref_term_tier_awarded_id",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_history__user_user_id",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropForeignKey(
                name: "f_k_assessment_history_assessment_assessment_id",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropForeignKey(
                name: "f_k_course__ref_term_category_id",
                schema: "LAP",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "f_k_course__ref_term_difficulty_level_id",
                schema: "LAP",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "f_k_course__ref_term_sub_category_id",
                schema: "LAP",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "f_k_course__user_created_by_user_id",
                schema: "LAP",
                table: "course");

            migrationBuilder.DropForeignKey(
                name: "f_k_course_content__course_meta_topic_meta_topic_id",
                schema: "LAP",
                table: "course_content");

            migrationBuilder.DropForeignKey(
                name: "f_k_course_content__ref_term_content_type_id",
                schema: "LAP",
                table: "course_content");

            migrationBuilder.DropForeignKey(
                name: "f_k_course_meta_topic_course_course_id",
                schema: "LAP",
                table: "course_meta_topic");

            migrationBuilder.DropForeignKey(
                name: "f_k_enrollment__user_user_id",
                schema: "LAP",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "f_k_enrollment_course_course_id",
                schema: "LAP",
                table: "enrollment");

            migrationBuilder.DropForeignKey(
                name: "f_k_forum_message__user_user_id",
                schema: "LAP",
                table: "forum_message");

            migrationBuilder.DropForeignKey(
                name: "f_k_forum_message_course_course_id",
                schema: "LAP",
                table: "forum_message");

            migrationBuilder.DropForeignKey(
                name: "f_k_import_job__ref_term_status_id",
                schema: "LAP",
                table: "import_job");

            migrationBuilder.DropForeignKey(
                name: "f_k_import_job_assessment_assessment_id",
                schema: "LAP",
                table: "import_job");

            migrationBuilder.DropForeignKey(
                name: "f_k_person__ref_term_designation_id",
                schema: "LAP",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "f_k_person__ref_term_gender_id",
                schema: "LAP",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "f_k_question__ref_term_question_type_id",
                schema: "LAP",
                table: "question");

            migrationBuilder.DropForeignKey(
                name: "f_k_question_assessment_assessment_id",
                schema: "LAP",
                table: "question");

            migrationBuilder.DropForeignKey(
                name: "f_k_question_course_meta_topic_meta_topic_id",
                schema: "LAP",
                table: "question");

            migrationBuilder.DropForeignKey(
                name: "f_k_ref_term_ref_set_ref_set_id",
                schema: "LAP",
                table: "ref_term");

            migrationBuilder.DropForeignKey(
                name: "f_k_refresh_token__user_user_id",
                schema: "LAP",
                table: "refresh_token");

            migrationBuilder.DropForeignKey(
                name: "f_k_review__user_user_id",
                schema: "LAP",
                table: "review");

            migrationBuilder.DropForeignKey(
                name: "f_k_review_course_course_id",
                schema: "LAP",
                table: "review");

            migrationBuilder.DropForeignKey(
                name: "f_k_role_feature_mapping_feature_feature_id",
                schema: "LAP",
                table: "role_feature_mapping");

            migrationBuilder.DropForeignKey(
                name: "f_k_role_feature_mapping_ref_term_role_id",
                schema: "LAP",
                table: "role_feature_mapping");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_person_person_id",
                schema: "LAP",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_ref_term_current_tier_id",
                schema: "LAP",
                table: "user");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_course_progress_course_content_course_content_id",
                schema: "LAP",
                table: "user_course_progress");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_course_progress_enrollment_enrollment_id",
                schema: "LAP",
                table: "user_course_progress");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_role_mapping_ref_term_role_id",
                schema: "LAP",
                table: "user_role_mapping");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_role_mapping_user_user_id",
                schema: "LAP",
                table: "user_role_mapping");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_secret_user_user_id",
                schema: "LAP",
                table: "user_secret");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_secret",
                schema: "LAP",
                table: "user_secret");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_role_mapping",
                schema: "LAP",
                table: "user_role_mapping");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_course_progress",
                schema: "LAP",
                table: "user_course_progress");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user",
                schema: "LAP",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_role_feature_mapping",
                schema: "LAP",
                table: "role_feature_mapping");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_review",
                schema: "LAP",
                table: "review");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_refresh_token",
                schema: "LAP",
                table: "refresh_token");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_ref_term",
                schema: "LAP",
                table: "ref_term");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_ref_set",
                schema: "LAP",
                table: "ref_set");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_question",
                schema: "LAP",
                table: "question");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_person",
                schema: "LAP",
                table: "person");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_import_job",
                schema: "LAP",
                table: "import_job");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_forum_message",
                schema: "LAP",
                table: "forum_message");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_feature",
                schema: "LAP",
                table: "feature");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_enrollment",
                schema: "LAP",
                table: "enrollment");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_course_meta_topic",
                schema: "LAP",
                table: "course_meta_topic");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_course_content",
                schema: "LAP",
                table: "course_content");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_course",
                schema: "LAP",
                table: "course");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_assessment_history",
                schema: "LAP",
                table: "assessment_history");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_assessment_answer",
                schema: "LAP",
                table: "assessment_answer");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_assessment",
                schema: "LAP",
                table: "assessment");

            migrationBuilder.RenameTable(
                name: "user_secret",
                schema: "LAP",
                newName: "user_secrets",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "user_role_mapping",
                schema: "LAP",
                newName: "user_role_mappings",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "user_course_progress",
                schema: "LAP",
                newName: "user_course_progresses",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "user",
                schema: "LAP",
                newName: "users",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "role_feature_mapping",
                schema: "LAP",
                newName: "role_feature_mappings",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "review",
                schema: "LAP",
                newName: "reviews",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "refresh_token",
                schema: "LAP",
                newName: "refresh_tokens",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "ref_term",
                schema: "LAP",
                newName: "ref_terms",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "ref_set",
                schema: "LAP",
                newName: "ref_sets",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "question",
                schema: "LAP",
                newName: "questions",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "person",
                schema: "LAP",
                newName: "persons",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "import_job",
                schema: "LAP",
                newName: "import_jobs",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "forum_message",
                schema: "LAP",
                newName: "forum_messages",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "feature",
                schema: "LAP",
                newName: "features",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "enrollment",
                schema: "LAP",
                newName: "enrollments",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "course_meta_topic",
                schema: "LAP",
                newName: "course_meta_topics",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "course_content",
                schema: "LAP",
                newName: "course_contents",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "course",
                schema: "LAP",
                newName: "courses",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "assessment_history",
                schema: "LAP",
                newName: "assessment_histories",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "assessment_answer",
                schema: "LAP",
                newName: "assessment_answers",
                newSchema: "LAP");

            migrationBuilder.RenameTable(
                name: "assessment",
                schema: "LAP",
                newName: "assessments",
                newSchema: "LAP");

            migrationBuilder.RenameIndex(
                name: "i_x_user_secret_user_id",
                schema: "LAP",
                table: "user_secrets",
                newName: "i_x_user_secrets_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_role_mapping_user_id",
                schema: "LAP",
                table: "user_role_mappings",
                newName: "i_x_user_role_mappings_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_role_mapping_role_id",
                schema: "LAP",
                table: "user_role_mappings",
                newName: "i_x_user_role_mappings_role_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_course_progress_enrollment_id",
                schema: "LAP",
                table: "user_course_progresses",
                newName: "i_x_user_course_progresses_enrollment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_course_progress_course_content_id",
                schema: "LAP",
                table: "user_course_progresses",
                newName: "i_x_user_course_progresses_course_content_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_person_id",
                schema: "LAP",
                table: "users",
                newName: "i_x_users_person_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_current_tier_id",
                schema: "LAP",
                table: "users",
                newName: "i_x_users_current_tier_id");

            migrationBuilder.RenameIndex(
                name: "i_x_role_feature_mapping_role_id",
                schema: "LAP",
                table: "role_feature_mappings",
                newName: "i_x_role_feature_mappings_role_id");

            migrationBuilder.RenameIndex(
                name: "i_x_role_feature_mapping_feature_id",
                schema: "LAP",
                table: "role_feature_mappings",
                newName: "i_x_role_feature_mappings_feature_id");

            migrationBuilder.RenameIndex(
                name: "i_x_review_user_id",
                schema: "LAP",
                table: "reviews",
                newName: "i_x_reviews_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_review_course_id",
                schema: "LAP",
                table: "reviews",
                newName: "i_x_reviews_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_refresh_token_user_id",
                schema: "LAP",
                table: "refresh_tokens",
                newName: "i_x_refresh_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_ref_term_ref_set_id",
                schema: "LAP",
                table: "ref_terms",
                newName: "i_x_ref_terms_ref_set_id");

            migrationBuilder.RenameIndex(
                name: "i_x_question_question_type_id",
                schema: "LAP",
                table: "questions",
                newName: "i_x_questions_question_type_id");

            migrationBuilder.RenameIndex(
                name: "i_x_question_meta_topic_id",
                schema: "LAP",
                table: "questions",
                newName: "i_x_questions_meta_topic_id");

            migrationBuilder.RenameIndex(
                name: "i_x_question_assessment_id",
                schema: "LAP",
                table: "questions",
                newName: "i_x_questions_assessment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_person_gender_id",
                schema: "LAP",
                table: "persons",
                newName: "i_x_persons_gender_id");

            migrationBuilder.RenameIndex(
                name: "i_x_person_designation_id",
                schema: "LAP",
                table: "persons",
                newName: "i_x_persons_designation_id");

            migrationBuilder.RenameIndex(
                name: "i_x_import_job_status_id",
                schema: "LAP",
                table: "import_jobs",
                newName: "i_x_import_jobs_status_id");

            migrationBuilder.RenameIndex(
                name: "i_x_import_job_assessment_id",
                schema: "LAP",
                table: "import_jobs",
                newName: "i_x_import_jobs_assessment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_forum_message_user_id",
                schema: "LAP",
                table: "forum_messages",
                newName: "i_x_forum_messages_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_forum_message_course_id",
                schema: "LAP",
                table: "forum_messages",
                newName: "i_x_forum_messages_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_enrollment_user_id",
                schema: "LAP",
                table: "enrollments",
                newName: "i_x_enrollments_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_enrollment_course_id",
                schema: "LAP",
                table: "enrollments",
                newName: "i_x_enrollments_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_meta_topic_course_id",
                schema: "LAP",
                table: "course_meta_topics",
                newName: "i_x_course_meta_topics_course_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_content_meta_topic_id",
                schema: "LAP",
                table: "course_contents",
                newName: "i_x_course_contents_meta_topic_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_content_content_type_id",
                schema: "LAP",
                table: "course_contents",
                newName: "i_x_course_contents_content_type_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_sub_category_id",
                schema: "LAP",
                table: "courses",
                newName: "i_x_courses_sub_category_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_difficulty_level_id",
                schema: "LAP",
                table: "courses",
                newName: "i_x_courses_difficulty_level_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_created_by_user_id",
                schema: "LAP",
                table: "courses",
                newName: "i_x_courses_created_by_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_course_category_id",
                schema: "LAP",
                table: "courses",
                newName: "i_x_courses_category_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_history_user_id",
                schema: "LAP",
                table: "assessment_histories",
                newName: "i_x_assessment_histories_user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_history_tier_awarded_id",
                schema: "LAP",
                table: "assessment_histories",
                newName: "i_x_assessment_histories_tier_awarded_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_history_assessment_id",
                schema: "LAP",
                table: "assessment_histories",
                newName: "i_x_assessment_histories_assessment_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_answer_question_id",
                schema: "LAP",
                table: "assessment_answers",
                newName: "i_x_assessment_answers_question_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_answer_assessment_history_id",
                schema: "LAP",
                table: "assessment_answers",
                newName: "i_x_assessment_answers_assessment_history_id");

            migrationBuilder.RenameIndex(
                name: "i_x_assessment_course_id",
                schema: "LAP",
                table: "assessments",
                newName: "i_x_assessments_course_id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_secrets",
                schema: "LAP",
                table: "user_secrets",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_role_mappings",
                schema: "LAP",
                table: "user_role_mappings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_course_progresses",
                schema: "LAP",
                table: "user_course_progresses",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_users",
                schema: "LAP",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_role_feature_mappings",
                schema: "LAP",
                table: "role_feature_mappings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_reviews",
                schema: "LAP",
                table: "reviews",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_refresh_tokens",
                schema: "LAP",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_ref_terms",
                schema: "LAP",
                table: "ref_terms",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_ref_sets",
                schema: "LAP",
                table: "ref_sets",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_questions",
                schema: "LAP",
                table: "questions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_persons",
                schema: "LAP",
                table: "persons",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_import_jobs",
                schema: "LAP",
                table: "import_jobs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_forum_messages",
                schema: "LAP",
                table: "forum_messages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_features",
                schema: "LAP",
                table: "features",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_enrollments",
                schema: "LAP",
                table: "enrollments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_course_meta_topics",
                schema: "LAP",
                table: "course_meta_topics",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_course_contents",
                schema: "LAP",
                table: "course_contents",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_courses",
                schema: "LAP",
                table: "courses",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_assessment_histories",
                schema: "LAP",
                table: "assessment_histories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_assessment_answers",
                schema: "LAP",
                table: "assessment_answers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_assessments",
                schema: "LAP",
                table: "assessments",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_answers__assessment_histories_assessment_history_id",
                schema: "LAP",
                table: "assessment_answers",
                column: "assessment_history_id",
                principalSchema: "LAP",
                principalTable: "assessment_histories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_answers__questions_question_id",
                schema: "LAP",
                table: "assessment_answers",
                column: "question_id",
                principalSchema: "LAP",
                principalTable: "questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_histories__ref_terms_tier_awarded_id",
                schema: "LAP",
                table: "assessment_histories",
                column: "tier_awarded_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_histories__users_user_id",
                schema: "LAP",
                table: "assessment_histories",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessment_histories_assessments_assessment_id",
                schema: "LAP",
                table: "assessment_histories",
                column: "assessment_id",
                principalSchema: "LAP",
                principalTable: "assessments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_assessments__courses_course_id",
                schema: "LAP",
                table: "assessments",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course_contents__course_meta_topics_meta_topic_id",
                schema: "LAP",
                table: "course_contents",
                column: "meta_topic_id",
                principalSchema: "LAP",
                principalTable: "course_meta_topics",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course_contents__ref_terms_content_type_id",
                schema: "LAP",
                table: "course_contents",
                column: "content_type_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_course_meta_topics_courses_course_id",
                schema: "LAP",
                table: "course_meta_topics",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_courses__ref_terms_category_id",
                schema: "LAP",
                table: "courses",
                column: "category_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_courses__ref_terms_difficulty_level_id",
                schema: "LAP",
                table: "courses",
                column: "difficulty_level_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_courses__ref_terms_sub_category_id",
                schema: "LAP",
                table: "courses",
                column: "sub_category_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_courses__users_created_by_user_id",
                schema: "LAP",
                table: "courses",
                column: "created_by_user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_enrollments__users_user_id",
                schema: "LAP",
                table: "enrollments",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_enrollments_courses_course_id",
                schema: "LAP",
                table: "enrollments",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_forum_messages__users_user_id",
                schema: "LAP",
                table: "forum_messages",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_forum_messages_courses_course_id",
                schema: "LAP",
                table: "forum_messages",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_import_jobs__ref_terms_status_id",
                schema: "LAP",
                table: "import_jobs",
                column: "status_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_import_jobs_assessments_assessment_id",
                schema: "LAP",
                table: "import_jobs",
                column: "assessment_id",
                principalSchema: "LAP",
                principalTable: "assessments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_persons__ref_terms_designation_id",
                schema: "LAP",
                table: "persons",
                column: "designation_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_persons__ref_terms_gender_id",
                schema: "LAP",
                table: "persons",
                column: "gender_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_questions__ref_terms_question_type_id",
                schema: "LAP",
                table: "questions",
                column: "question_type_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_questions_assessments_assessment_id",
                schema: "LAP",
                table: "questions",
                column: "assessment_id",
                principalSchema: "LAP",
                principalTable: "assessments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_questions_course_meta_topics_meta_topic_id",
                schema: "LAP",
                table: "questions",
                column: "meta_topic_id",
                principalSchema: "LAP",
                principalTable: "course_meta_topics",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_ref_terms_ref_sets_ref_set_id",
                schema: "LAP",
                table: "ref_terms",
                column: "ref_set_id",
                principalSchema: "LAP",
                principalTable: "ref_sets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_refresh_tokens__users_user_id",
                schema: "LAP",
                table: "refresh_tokens",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_reviews__users_user_id",
                schema: "LAP",
                table: "reviews",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_reviews_courses_course_id",
                schema: "LAP",
                table: "reviews",
                column: "course_id",
                principalSchema: "LAP",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_role_feature_mappings_features_feature_id",
                schema: "LAP",
                table: "role_feature_mappings",
                column: "feature_id",
                principalSchema: "LAP",
                principalTable: "features",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_role_feature_mappings_ref_terms_role_id",
                schema: "LAP",
                table: "role_feature_mappings",
                column: "role_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_course_progresses_course_contents_course_content_id",
                schema: "LAP",
                table: "user_course_progresses",
                column: "course_content_id",
                principalSchema: "LAP",
                principalTable: "course_contents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_course_progresses_enrollments_enrollment_id",
                schema: "LAP",
                table: "user_course_progresses",
                column: "enrollment_id",
                principalSchema: "LAP",
                principalTable: "enrollments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_role_mappings_ref_terms_role_id",
                schema: "LAP",
                table: "user_role_mappings",
                column: "role_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_role_mappings_users_user_id",
                schema: "LAP",
                table: "user_role_mappings",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_secrets_users_user_id",
                schema: "LAP",
                table: "user_secrets",
                column: "user_id",
                principalSchema: "LAP",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_users_persons_person_id",
                schema: "LAP",
                table: "users",
                column: "person_id",
                principalSchema: "LAP",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_users_ref_terms_current_tier_id",
                schema: "LAP",
                table: "users",
                column: "current_tier_id",
                principalSchema: "LAP",
                principalTable: "ref_terms",
                principalColumn: "id");
        }
    }
}
