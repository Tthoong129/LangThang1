using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniMap.Migrations
{
    /// <inheritdoc />
    public partial class SeparateProposals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaceProposals_Places_TargetPlaceId",
                table: "PlaceProposals");

            migrationBuilder.DropIndex(
                name: "IX_PlaceProposals_TargetPlaceId",
                table: "PlaceProposals");

            migrationBuilder.DropColumn(
                name: "ProposalType",
                table: "PlaceProposals");

            migrationBuilder.DropColumn(
                name: "TargetPlaceId",
                table: "PlaceProposals");

            migrationBuilder.CreateTable(
                name: "PlaceEditProposals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaceId = table.Column<long>(type: "bigint", nullable: false),
                    ProposedBy = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinPrice = table.Column<decimal>(type: "decimal(12,0)", nullable: true),
                    MaxPrice = table.Column<decimal>(type: "decimal(12,0)", nullable: true),
                    OpeningHours = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RejectReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewedBy = table.Column<long>(type: "bigint", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceEditProposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceEditProposals_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceEditProposals_Users_ProposedBy",
                        column: x => x.ProposedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceEditProposals_Users_ReviewedBy",
                        column: x => x.ReviewedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlaceEditProposalMedia",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaceEditProposalId = table.Column<long>(type: "bigint", nullable: false),
                    MediaType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceEditProposalMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceEditProposalMedia_PlaceEditProposals_PlaceEditProposalId",
                        column: x => x.PlaceEditProposalId,
                        principalTable: "PlaceEditProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceEditProposalMedia_PlaceEditProposalId",
                table: "PlaceEditProposalMedia",
                column: "PlaceEditProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceEditProposals_PlaceId",
                table: "PlaceEditProposals",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceEditProposals_ProposedBy",
                table: "PlaceEditProposals",
                column: "ProposedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceEditProposals_ReviewedBy",
                table: "PlaceEditProposals",
                column: "ReviewedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceEditProposalMedia");

            migrationBuilder.DropTable(
                name: "PlaceEditProposals");

            migrationBuilder.AddColumn<string>(
                name: "ProposalType",
                table: "PlaceProposals",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "TargetPlaceId",
                table: "PlaceProposals",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaceProposals_TargetPlaceId",
                table: "PlaceProposals",
                column: "TargetPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceProposals_Places_TargetPlaceId",
                table: "PlaceProposals",
                column: "TargetPlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
