using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LAP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameThumbnailUrlToThumbnailImgPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "thumbnail_url",
                schema: "LAP",
                table: "courses",
                newName: "thumbnail_img_path"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "thumbnail_img_path",
                schema: "LAP",
                table: "courses",
                newName: "thumbnail_url"
            );
        }
    }
}
