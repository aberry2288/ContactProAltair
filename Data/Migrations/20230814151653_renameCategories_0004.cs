using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactProAltair.Data.Migrations
{
    /// <inheritdoc />
    public partial class renameCategories_0004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryContact_Categories_CatagoriesId",
                table: "CategoryContact");

            migrationBuilder.RenameColumn(
                name: "CatagoriesId",
                table: "CategoryContact",
                newName: "CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryContact_Categories_CategoriesId",
                table: "CategoryContact",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryContact_Categories_CategoriesId",
                table: "CategoryContact");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryContact",
                newName: "CatagoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryContact_Categories_CatagoriesId",
                table: "CategoryContact",
                column: "CatagoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
