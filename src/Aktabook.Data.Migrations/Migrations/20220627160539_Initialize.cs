using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aktabook.Data.Migrations.Migrations;

public partial class Initialize : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.CreateTable(
            name: "Author",
            columns: table => new
            {
                AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Author", x => x.AuthorId);
            });

        migrationBuilder.CreateTable(
            name: "Book",
            columns: table => new
            {
                BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Isbn = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Book", x => x.BookId);
            });

        migrationBuilder.CreateTable(
            name: "BookInfoRequest",
            columns: table => new
            {
                BookInfoRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Isbn = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BookInfoRequest", x => x.BookInfoRequestId);
            });

        migrationBuilder.CreateTable(
            name: "AuthorBook",
            columns: table => new
            {
                AuthorsAuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BooksBookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuthorBook", x => new { x.AuthorsAuthorId, x.BooksBookId });
                table.ForeignKey(
                    name: "FK_AuthorBook_Author_AuthorsAuthorId",
                    column: x => x.AuthorsAuthorId,
                    principalTable: "Author",
                    principalColumn: "AuthorId",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AuthorBook_Book_BooksBookId",
                    column: x => x.BooksBookId,
                    principalTable: "Book",
                    principalColumn: "BookId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "BookInfoRequestLogEntry",
            columns: table => new
            {
                BookInfoRequestLogEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BookInfoRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Created = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BookInfoRequestLogEntry", x => x.BookInfoRequestLogEntryId);
                table.ForeignKey(
                    name: "FK_BookInfoRequestLogEntry_BookInfoRequest_BookInfoRequestId",
                    column: x => x.BookInfoRequestId,
                    principalTable: "BookInfoRequest",
                    principalColumn: "BookInfoRequestId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Author_Name",
            table: "Author",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_AuthorBook_BooksBookId",
            table: "AuthorBook",
            column: "BooksBookId");

        migrationBuilder.CreateIndex(
            name: "IX_Book_Isbn",
            table: "Book",
            column: "Isbn",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BookInfoRequest_Isbn",
            table: "BookInfoRequest",
            column: "Isbn");

        migrationBuilder.CreateIndex(
            name: "IX_BookInfoRequestLogEntry_BookInfoRequestId",
            table: "BookInfoRequestLogEntry",
            column: "BookInfoRequestId");

        migrationBuilder.CreateIndex(
            name: "IX_BookInfoRequestLogEntry_Status",
            table: "BookInfoRequestLogEntry",
            column: "Status");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.DropTable(
            name: "AuthorBook");

        migrationBuilder.DropTable(
            name: "BookInfoRequestLogEntry");

        migrationBuilder.DropTable(
            name: "Author");

        migrationBuilder.DropTable(
            name: "Book");

        migrationBuilder.DropTable(
            name: "BookInfoRequest");
    }
}
