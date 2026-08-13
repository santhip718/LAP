using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "LAP");

            migrationBuilder.CreateTable(
                name: "features",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    method = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_features", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ref_sets",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ref_sets", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ref_terms",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ref_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_ref_terms", x => x.id);
                    table.ForeignKey(
                        name: "f_k_ref_terms_ref_sets_ref_set_id",
                        column: x => x.ref_set_id,
                        principalSchema: "LAP",
                        principalTable: "ref_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "persons",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    mobile_number = table.Column<string>(type: "text", nullable: false),
                    designation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_persons", x => x.id);
                    table.ForeignKey(
                        name: "f_k_persons__ref_terms_designation_id",
                        column: x => x.designation_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_persons__ref_terms_gender_id",
                        column: x => x.gender_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "role_feature_mappings",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_role_feature_mappings", x => x.id);
                    table.ForeignKey(
                        name: "f_k_role_feature_mappings_features_feature_id",
                        column: x => x.feature_id,
                        principalSchema: "LAP",
                        principalTable: "features",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_role_feature_mappings_ref_terms_role_id",
                        column: x => x.role_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_tier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                    table.ForeignKey(
                        name: "f_k_users_persons_person_id",
                        column: x => x.person_id,
                        principalSchema: "LAP",
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_users_ref_terms_current_tier_id",
                        column: x => x.current_tier_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "courses",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sub_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    overall_rating = table.Column<decimal>(type: "numeric", nullable: false),
                    difficulty_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    duration_minute = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_url = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_courses", x => x.id);
                    table.ForeignKey(
                        name: "f_k_courses__ref_terms_category_id",
                        column: x => x.category_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_courses__ref_terms_difficulty_level_id",
                        column: x => x.difficulty_level_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_courses__ref_terms_sub_category_id",
                        column: x => x.sub_category_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_courses__users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalSchema: "LAP",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_role_mappings",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_role_mappings", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_role_mappings_ref_terms_role_id",
                        column: x => x.role_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_user_role_mappings_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "LAP",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_secrets",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    password_salt = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_secrets", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_secrets_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "LAP",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "assessments",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    total_mark = table.Column<int>(type: "integer", nullable: false),
                    passing_mark = table.Column<int>(type: "integer", nullable: false),
                    duration_minute = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_assessments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_assessments__courses_course_id",
                        column: x => x.course_id,
                        principalSchema: "LAP",
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "course_meta_topics",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence_order = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    duration_minute = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_course_meta_topics", x => x.id);
                    table.ForeignKey(
                        name: "f_k_course_meta_topics_courses_course_id",
                        column: x => x.course_id,
                        principalSchema: "LAP",
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "enrollments",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    enrolled_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    completed_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    progress_percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    enrollment_status = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_enrollments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_enrollments__users_user_id",
                        column: x => x.user_id,
                        principalSchema: "LAP",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_enrollments_courses_course_id",
                        column: x => x.course_id,
                        principalSchema: "LAP",
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "forum_messages",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_text = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_forum_messages", x => x.id);
                    table.ForeignKey(
                        name: "f_k_forum_messages__users_user_id",
                        column: x => x.user_id,
                        principalSchema: "LAP",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_forum_messages_courses_course_id",
                        column: x => x.course_id,
                        principalSchema: "LAP",
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    review_text = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_reviews", x => x.id);
                    table.ForeignKey(
                        name: "f_k_reviews__users_user_id",
                        column: x => x.user_id,
                        principalSchema: "LAP",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_reviews_courses_course_id",
                        column: x => x.course_id,
                        principalSchema: "LAP",
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "assessment_histories",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    completed_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    score = table.Column<decimal>(type: "numeric", nullable: false),
                    weighted_score = table.Column<decimal>(type: "numeric", nullable: false),
                    tier_awarded_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_assessment_histories", x => x.id);
                    table.ForeignKey(
                        name: "f_k_assessment_histories__ref_terms_tier_awarded_id",
                        column: x => x.tier_awarded_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id"
                    );
                    table.ForeignKey(
                        name: "f_k_assessment_histories__users_user_id",
                        column: x => x.user_id,
                        principalSchema: "LAP",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_assessment_histories_assessments_assessment_id",
                        column: x => x.assessment_id,
                        principalSchema: "LAP",
                        principalTable: "assessments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "import_jobs",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_records = table.Column<int>(type: "integer", nullable: false),
                    processed_records = table.Column<int>(type: "integer", nullable: false),
                    failed_records = table.Column<int>(type: "integer", nullable: false),
                    started_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    completed_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_import_jobs", x => x.id);
                    table.ForeignKey(
                        name: "f_k_import_jobs__ref_terms_status_id",
                        column: x => x.status_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_import_jobs_assessments_assessment_id",
                        column: x => x.assessment_id,
                        principalSchema: "LAP",
                        principalTable: "assessments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "course_contents",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    meta_topic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    content_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    video_url = table.Column<string>(type: "text", nullable: true),
                    pdf_file_path = table.Column<string>(type: "text", nullable: true),
                    sequence_order = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_course_contents", x => x.id);
                    table.ForeignKey(
                        name: "f_k_course_contents__course_meta_topics_meta_topic_id",
                        column: x => x.meta_topic_id,
                        principalSchema: "LAP",
                        principalTable: "course_meta_topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_course_contents__ref_terms_content_type_id",
                        column: x => x.content_type_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meta_topic_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    option_list = table.Column<List<string>>(type: "jsonb", nullable: false),
                    answer = table.Column<string>(type: "text", nullable: false),
                    weight = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_questions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_questions__ref_terms_question_type_id",
                        column: x => x.question_type_id,
                        principalSchema: "LAP",
                        principalTable: "ref_terms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_questions_assessments_assessment_id",
                        column: x => x.assessment_id,
                        principalSchema: "LAP",
                        principalTable: "assessments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_questions_course_meta_topics_meta_topic_id",
                        column: x => x.meta_topic_id,
                        principalSchema: "LAP",
                        principalTable: "course_meta_topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "user_course_progresses",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    enrollment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_content_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_on = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_course_progresses", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_course_progresses_course_contents_course_content_id",
                        column: x => x.course_content_id,
                        principalSchema: "LAP",
                        principalTable: "course_contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_user_course_progresses_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalSchema: "LAP",
                        principalTable: "enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "assessment_answers",
                schema: "LAP",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_history_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    selected_answer = table.Column<string>(type: "text", nullable: false),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    obtained_score = table.Column<decimal>(type: "numeric", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    date_updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_assessment_answers", x => x.id);
                    table.ForeignKey(
                        name: "f_k_assessment_answers__assessment_histories_assessment_history_id",
                        column: x => x.assessment_history_id,
                        principalSchema: "LAP",
                        principalTable: "assessment_histories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "f_k_assessment_answers__questions_question_id",
                        column: x => x.question_id,
                        principalSchema: "LAP",
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_answers_assessment_history_id",
                schema: "LAP",
                table: "assessment_answers",
                column: "assessment_history_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_answers_question_id",
                schema: "LAP",
                table: "assessment_answers",
                column: "question_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_histories_assessment_id",
                schema: "LAP",
                table: "assessment_histories",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_histories_tier_awarded_id",
                schema: "LAP",
                table: "assessment_histories",
                column: "tier_awarded_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_histories_user_id",
                schema: "LAP",
                table: "assessment_histories",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_assessments_course_id",
                schema: "LAP",
                table: "assessments",
                column: "course_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "i_x_course_contents_content_type_id",
                schema: "LAP",
                table: "course_contents",
                column: "content_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_course_contents_meta_topic_id",
                schema: "LAP",
                table: "course_contents",
                column: "meta_topic_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_course_meta_topics_course_id",
                schema: "LAP",
                table: "course_meta_topics",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_courses_category_id",
                schema: "LAP",
                table: "courses",
                column: "category_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_courses_created_by_user_id",
                schema: "LAP",
                table: "courses",
                column: "created_by_user_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_courses_difficulty_level_id",
                schema: "LAP",
                table: "courses",
                column: "difficulty_level_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_courses_sub_category_id",
                schema: "LAP",
                table: "courses",
                column: "sub_category_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_enrollments_course_id",
                schema: "LAP",
                table: "enrollments",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_enrollments_user_id",
                schema: "LAP",
                table: "enrollments",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_forum_messages_course_id",
                schema: "LAP",
                table: "forum_messages",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_forum_messages_user_id",
                schema: "LAP",
                table: "forum_messages",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_import_jobs_assessment_id",
                schema: "LAP",
                table: "import_jobs",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_import_jobs_status_id",
                schema: "LAP",
                table: "import_jobs",
                column: "status_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_persons_designation_id",
                schema: "LAP",
                table: "persons",
                column: "designation_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_persons_gender_id",
                schema: "LAP",
                table: "persons",
                column: "gender_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_questions_assessment_id",
                schema: "LAP",
                table: "questions",
                column: "assessment_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_questions_meta_topic_id",
                schema: "LAP",
                table: "questions",
                column: "meta_topic_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_questions_question_type_id",
                schema: "LAP",
                table: "questions",
                column: "question_type_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_ref_terms_ref_set_id",
                schema: "LAP",
                table: "ref_terms",
                column: "ref_set_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_reviews_course_id",
                schema: "LAP",
                table: "reviews",
                column: "course_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_reviews_user_id",
                schema: "LAP",
                table: "reviews",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_role_feature_mappings_feature_id",
                schema: "LAP",
                table: "role_feature_mappings",
                column: "feature_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_role_feature_mappings_role_id",
                schema: "LAP",
                table: "role_feature_mappings",
                column: "role_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_user_course_progresses_course_content_id",
                schema: "LAP",
                table: "user_course_progresses",
                column: "course_content_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_user_course_progresses_enrollment_id",
                schema: "LAP",
                table: "user_course_progresses",
                column: "enrollment_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_user_role_mappings_role_id",
                schema: "LAP",
                table: "user_role_mappings",
                column: "role_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_user_role_mappings_user_id",
                schema: "LAP",
                table: "user_role_mappings",
                column: "user_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_user_secrets_user_id",
                schema: "LAP",
                table: "user_secrets",
                column: "user_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "i_x_users_current_tier_id",
                schema: "LAP",
                table: "users",
                column: "current_tier_id"
            );

            migrationBuilder.CreateIndex(
                name: "i_x_users_person_id",
                schema: "LAP",
                table: "users",
                column: "person_id",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "assessment_answers", schema: "LAP");

            migrationBuilder.DropTable(name: "forum_messages", schema: "LAP");

            migrationBuilder.DropTable(name: "import_jobs", schema: "LAP");

            migrationBuilder.DropTable(name: "reviews", schema: "LAP");

            migrationBuilder.DropTable(name: "role_feature_mappings", schema: "LAP");

            migrationBuilder.DropTable(name: "user_course_progresses", schema: "LAP");

            migrationBuilder.DropTable(name: "user_role_mappings", schema: "LAP");

            migrationBuilder.DropTable(name: "user_secrets", schema: "LAP");

            migrationBuilder.DropTable(name: "assessment_histories", schema: "LAP");

            migrationBuilder.DropTable(name: "questions", schema: "LAP");

            migrationBuilder.DropTable(name: "features", schema: "LAP");

            migrationBuilder.DropTable(name: "course_contents", schema: "LAP");

            migrationBuilder.DropTable(name: "enrollments", schema: "LAP");

            migrationBuilder.DropTable(name: "assessments", schema: "LAP");

            migrationBuilder.DropTable(name: "course_meta_topics", schema: "LAP");

            migrationBuilder.DropTable(name: "courses", schema: "LAP");

            migrationBuilder.DropTable(name: "users", schema: "LAP");

            migrationBuilder.DropTable(name: "persons", schema: "LAP");

            migrationBuilder.DropTable(name: "ref_terms", schema: "LAP");

            migrationBuilder.DropTable(name: "ref_sets", schema: "LAP");
        }
    }
}
