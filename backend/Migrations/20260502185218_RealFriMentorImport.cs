using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MentorReservation.Api.Migrations
{
    /// <inheritdoc />
    public partial class RealFriMentorImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MentorProfiles_Users_UserId",
                table: "MentorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorshipRequests_MentorProfiles_MentorId",
                table: "MentorshipRequests");

            migrationBuilder.RenameColumn(
                name: "MentorId",
                table: "MentorshipRequests",
                newName: "MentorProfileId");

            migrationBuilder.RenameColumn(
                name: "MentorComment",
                table: "MentorshipRequests",
                newName: "MentorResponse");

            migrationBuilder.RenameIndex(
                name: "IX_MentorshipRequests_StudentId_MentorId_Status",
                table: "MentorshipRequests",
                newName: "IX_MentorshipRequests_StudentId_MentorProfileId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_MentorshipRequests_MentorId",
                table: "MentorshipRequests",
                newName: "IX_MentorshipRequests_MentorProfileId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "DecidedAt",
                table: "MentorshipRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MentorProfiles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Laboratory",
                table: "MentorProfiles",
                type: "character varying(180)",
                maxLength: 180,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MentorProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "MentorProfiles",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "MentorProfiles",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportedAt",
                table: "MentorProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "MentorProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "MentorProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Office",
                table: "MentorProfiles",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "MentorProfiles",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileUrl",
                table: "MentorProfiles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "MentorProfiles",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SourceExternalId",
                table: "MentorProfiles",
                type: "character varying(240)",
                maxLength: 240,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MentorProfiles",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "MentorProfiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "EmailOutbox",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipientEmail = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Subject = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailOutbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MentorImportRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ImportedCount = table.Column<int>(type: "integer", nullable: false),
                    UpdatedCount = table.Column<int>(type: "integer", nullable: false),
                    SkippedCount = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorImportRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    RelatedRequestId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_MentorshipRequests_RelatedRequestId",
                        column: x => x.RelatedRequestId,
                        principalTable: "MentorshipRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResearchAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResearchAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EnrollmentNumber = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    StudyProgram = table.Column<string>(type: "text", nullable: false),
                    YearOfStudy = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MentorResearchAreas",
                columns: table => new
                {
                    MentorProfileId = table.Column<int>(type: "integer", nullable: false),
                    ResearchAreaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorResearchAreas", x => new { x.MentorProfileId, x.ResearchAreaId });
                    table.ForeignKey(
                        name: "FK_MentorResearchAreas_MentorProfiles_MentorProfileId",
                        column: x => x.MentorProfileId,
                        principalTable: "MentorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentorResearchAreas_ResearchAreas_ResearchAreaId",
                        column: x => x.ResearchAreaId,
                        principalTable: "ResearchAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MentorProfiles_Email",
                table: "MentorProfiles",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_MentorProfiles_ProfileUrl",
                table: "MentorProfiles",
                column: "ProfileUrl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MentorProfiles_SourceExternalId",
                table: "MentorProfiles",
                column: "SourceExternalId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorResearchAreas_ResearchAreaId",
                table: "MentorResearchAreas",
                column: "ResearchAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedRequestId",
                table: "Notifications",
                column: "RelatedRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResearchAreas_Name",
                table: "ResearchAreas",
                column: "Name",
                unique: true);

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET
                    "FirstName" = CASE
                        WHEN position(' ' in trim("FullName")) > 0 THEN split_part(trim("FullName"), ' ', 1)
                        ELSE trim("FullName")
                    END,
                    "LastName" = CASE
                        WHEN position(' ' in trim("FullName")) > 0 THEN regexp_replace(trim("FullName"), '^[^ ]+\s+', '')
                        ELSE ''
                    END,
                    "IsActive" = true,
                    "CreatedAt" = CURRENT_TIMESTAMP,
                    "UpdatedAt" = CURRENT_TIMESTAMP;
                """);

            migrationBuilder.Sql("""
                UPDATE "MentorProfiles" AS mp
                SET
                    "FirstName" = u."FirstName",
                    "LastName" = u."LastName",
                    "Email" = u."Email",
                    "Source" = 'Legacy seed data',
                    "IsAvailable" = true,
                    "CreatedAt" = CURRENT_TIMESTAMP,
                    "UpdatedAt" = CURRENT_TIMESTAMP
                FROM "Users" AS u
                WHERE mp."UserId" = u."Id";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "ResearchAreas" ("Name", "CreatedAt")
                SELECT DISTINCT btrim(area), CURRENT_TIMESTAMP
                FROM "MentorProfiles" AS mp, unnest(mp."ResearchAreas") AS area
                WHERE area IS NOT NULL AND btrim(area) <> ''
                ON CONFLICT ("Name") DO NOTHING;
                """);

            migrationBuilder.Sql("""
                INSERT INTO "MentorResearchAreas" ("MentorProfileId", "ResearchAreaId")
                SELECT mp."Id", ra."Id"
                FROM "MentorProfiles" AS mp
                CROSS JOIN LATERAL unnest(mp."ResearchAreas") AS area
                INNER JOIN "ResearchAreas" AS ra ON ra."Name" = btrim(area)
                ON CONFLICT DO NOTHING;
                """);

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResearchAreas",
                table: "MentorProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_EnrollmentNumber",
                table: "StudentProfiles",
                column: "EnrollmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_UserId",
                table: "StudentProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorProfiles_Users_UserId",
                table: "MentorProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorshipRequests_MentorProfiles_MentorProfileId",
                table: "MentorshipRequests",
                column: "MentorProfileId",
                principalTable: "MentorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MentorProfiles_Users_UserId",
                table: "MentorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorshipRequests_MentorProfiles_MentorProfileId",
                table: "MentorshipRequests");

            migrationBuilder.DropTable(
                name: "EmailOutbox");

            migrationBuilder.DropTable(
                name: "MentorImportRuns");

            migrationBuilder.DropTable(
                name: "MentorResearchAreas");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "ResearchAreas");

            migrationBuilder.DropIndex(
                name: "IX_MentorProfiles_Email",
                table: "MentorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_MentorProfiles_ProfileUrl",
                table: "MentorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_MentorProfiles_SourceExternalId",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DecidedAt",
                table: "MentorshipRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "ImportedAt",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Office",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileUrl",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "SourceExternalId",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "MentorProfiles");

            migrationBuilder.RenameColumn(
                name: "MentorResponse",
                table: "MentorshipRequests",
                newName: "MentorComment");

            migrationBuilder.RenameColumn(
                name: "MentorProfileId",
                table: "MentorshipRequests",
                newName: "MentorId");

            migrationBuilder.RenameIndex(
                name: "IX_MentorshipRequests_StudentId_MentorProfileId_Status",
                table: "MentorshipRequests",
                newName: "IX_MentorshipRequests_StudentId_MentorId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_MentorshipRequests_MentorProfileId",
                table: "MentorshipRequests",
                newName: "IX_MentorshipRequests_MentorId");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MentorProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Laboratory",
                table: "MentorProfiles",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(180)",
                oldMaxLength: 180,
                oldNullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "ResearchAreas",
                table: "MentorProfiles",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorProfiles_Users_UserId",
                table: "MentorProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorshipRequests_MentorProfiles_MentorId",
                table: "MentorshipRequests",
                column: "MentorId",
                principalTable: "MentorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
