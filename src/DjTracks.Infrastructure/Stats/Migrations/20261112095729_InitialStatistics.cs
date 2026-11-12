using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DjTracks.Infrastructure.Stats.Migrations
{
    /// <inheritdoc />
    public partial class InitialStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "playback_events",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Library = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    RelativePath = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Year = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Style = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SessionFolder = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Title = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Artist = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PlayedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playback_events", x => x.EventId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_playback_events_Library_RelativePath",
                table: "playback_events",
                columns: new[] { "Library", "RelativePath" });

            migrationBuilder.CreateIndex(
                name: "IX_playback_events_PlayedAtUtc",
                table: "playback_events",
                column: "PlayedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "playback_events");
        }
    }
}
