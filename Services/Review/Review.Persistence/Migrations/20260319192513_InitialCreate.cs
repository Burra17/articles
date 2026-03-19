using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Review.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetTypeDefinition",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    DefaultFileExtension = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false, defaultValue: "pdf"),
                    MaxFileSizeInMB = table.Column<byte>(type: "tinyint", nullable: false),
                    MaxAssetCount = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    AllowedFileExtensions = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypeDefinition", x => x.Id);
                    table.UniqueConstraint("AK_AssetTypeDefinition_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Journals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Honorific = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Affiliation = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Institution or organization they are associated with when they conduct their research."),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    TypeDiscriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "The highest academic degree of the author, e.g., PhD, MD, etc."),
                    Discipline = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true, comment: "The scientific discipline of the author, e.g., Computer Science, Biology, etc.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedById = table.Column<int>(type: "int", nullable: true),
                    Stage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JournalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articles_Journals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "Journals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Articles_Persons_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewerSpecialization",
                columns: table => new
                {
                    ReviewerId = table.Column<int>(type: "int", nullable: false),
                    JournalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewerSpecialization", x => new { x.JournalId, x.ReviewerId });
                    table.ForeignKey(
                        name: "FK_ReviewerSpecialization_Journals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "Journals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewerSpecialization_Persons_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticleActor",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    TypeDiscriminator = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "AUT"),
                    ContributionAreas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleActor", x => new { x.ArticleId, x.PersonId, x.TypeDiscriminator });
                    table.ForeignKey(
                        name: "FK_ArticleActor_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleActor_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    File_FileServerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    File_OriginalName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, comment: "Original full file name, with extension"),
                    File_Size = table.Column<long>(type: "bigint", nullable: false, comment: "Size of the file in kilobytes"),
                    File_Extension = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    File_Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "Final name of the file after renaming"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assets_AssetTypeDefinition_Type",
                        column: x => x.Type,
                        principalTable: "AssetTypeDefinition",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SentOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentById = table.Column<int>(type: "int", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitations_Persons_SentById",
                        column: x => x.SentById,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleActor_ArticleId_PersonId_Role",
                table: "ArticleActor",
                columns: new[] { "ArticleId", "PersonId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleActor_PersonId",
                table: "ArticleActor",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_JournalId",
                table: "Articles",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_SubmittedById",
                table: "Articles",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ArticleId",
                table: "Assets",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_Type",
                table: "Assets",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypeDefinition_Name",
                table: "AssetTypeDefinition",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ArticleId",
                table: "Invitations",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_SentById",
                table: "Invitations",
                column: "SentById");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_UserId",
                table: "Persons",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewerSpecialization_ReviewerId",
                table: "ReviewerSpecialization",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleActor");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "ReviewerSpecialization");

            migrationBuilder.DropTable(
                name: "AssetTypeDefinition");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Journals");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
