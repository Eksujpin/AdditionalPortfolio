using Microsoft.EntityFrameworkCore.Migrations;

namespace BDSA2020.Assignment03.Migrations
{
    public partial class SeedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Email = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Title = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    AssignedTo = table.Column<int>(nullable: true),
                    Description = table.Column<string>(unicode: false, nullable: true),
                    State = table.Column<int>(unicode: false, maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_tasks",
                        column: x => x.AssignedTo,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskTag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    task = table.Column<int>(nullable: false),
                    tag = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TaskTag__tag__2E1BDC42",
                        column: x => x.tag,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TaskTag__task__2D27B809",
                        column: x => x.task,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Tag",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Easy" },
                    { 2, "Medium" },
                    { 3, "Hard" },
                    { 4, "Programming" },
                    { 5, "Admin work" },
                    { 6, "Setup" },
                    { 7, "Cleaning" }
                });

            migrationBuilder.InsertData(
                table: "Task",
                columns: new[] { "Id", "AssignedTo", "Description", "State", "Title" },
                values: new object[,]
                {
                    { 2, null, "deploy best-of-breed niches", 0, "Program good stuff" },
                    { 3, null, "disintermediate 24/365 niches", 1, "Make citations" },
                    { 4, null, "productize integrated infomediaries", 1, "Cook dinner" },
                    { 5, null, "monetize customized experiences", 2, "Save the world" },
                    { 6, null, "incubate killer web-readiness", 2, "Destroy the world" },
                    { 8, null, null, 3, "Delete the sun" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "tpoxson0@gnu.org", "Tania Poxson" },
                    { 2, "jlarraway1@huffingtonpost.com", "Janos Larraway" },
                    { 3, "ibyre2@dyndns.org", "Ingram Byre" }
                });

            migrationBuilder.InsertData(
                table: "Task",
                columns: new[] { "Id", "AssignedTo", "Description", "State", "Title" },
                values: new object[,]
                {
                    { 1, 1, "deploy global platforms", 0, "Clean up code" },
                    { 7, 2, "deploy visionary e-business", 3, "Become famous" },
                    { 9, 2, "enable 24/7 architectures", 4, "Assassinate trump" },
                    { 10, 3, null, 4, "Make america great again" }
                });

            migrationBuilder.InsertData(
                table: "TaskTag",
                columns: new[] { "Id", "tag", "task" },
                values: new object[,]
                {
                    { 4, 1, 2 },
                    { 5, 4, 2 },
                    { 6, 1, 5 }
                });

            migrationBuilder.InsertData(
                table: "TaskTag",
                columns: new[] { "Id", "tag", "task" },
                values: new object[,]
                {
                    { 1, 3, 1 },
                    { 2, 4, 1 },
                    { 3, 7, 1 },
                    { 7, 3, 9 },
                    { 8, 7, 9 },
                    { 9, 5, 9 },
                    { 10, 1, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Tag__737584F6A119F308",
                table: "Tag",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_AssignedTo",
                table: "Task",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTag_tag",
                table: "TaskTag",
                column: "tag");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTag_task",
                table: "TaskTag",
                column: "task");

            migrationBuilder.CreateIndex(
                name: "UQ__User__A9D1053435401C23",
                table: "User",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskTag");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
