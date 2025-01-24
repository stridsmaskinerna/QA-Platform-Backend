using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnotherNewEntities_Subjects_SubjectId",
                table: "AnotherNewEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnotherNewEntities",
                table: "AnotherNewEntities");

            migrationBuilder.RenameTable(
                name: "AnotherNewEntities",
                newName: "Topics");

            migrationBuilder.RenameIndex(
                name: "IX_AnotherNewEntities_SubjectId",
                table: "Topics",
                newName: "IX_Topics_SubjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Topics",
                table: "Topics",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Subjects_SubjectId",
                table: "Topics",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Subjects_SubjectId",
                table: "Topics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Topics",
                table: "Topics");

            migrationBuilder.RenameTable(
                name: "Topics",
                newName: "AnotherNewEntities");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_SubjectId",
                table: "AnotherNewEntities",
                newName: "IX_AnotherNewEntities_SubjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnotherNewEntities",
                table: "AnotherNewEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnotherNewEntities_Subjects_SubjectId",
                table: "AnotherNewEntities",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }
    }
}
