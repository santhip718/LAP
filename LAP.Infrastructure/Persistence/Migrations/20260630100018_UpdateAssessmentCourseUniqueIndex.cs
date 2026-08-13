using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssessmentCourseUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_assessment_course_id",
                schema: "LAP",
                table: "assessment");

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_course_id",
                schema: "LAP",
                table: "assessment",
                column: "course_id",
                unique: true,
                filter: "is_active = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_assessment_course_id",
                schema: "LAP",
                table: "assessment");

            migrationBuilder.CreateIndex(
                name: "i_x_assessment_course_id",
                schema: "LAP",
                table: "assessment",
                column: "course_id",
                unique: true);
        }
    }
}
