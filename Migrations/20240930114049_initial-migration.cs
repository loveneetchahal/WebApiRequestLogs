using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiRequestLogs.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Node = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TraceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestDateTimeUtcActionLevel = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestQuery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestQueries = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestScheme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestHost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestHeaders = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseDateTimeUtcActionLevel = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseHeaders = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsExceptionActionLevel = table.Column<bool>(type: "bit", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExceptionStackTrace = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestLogs");
        }
    }
}
