using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HardshipApp.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HardshipApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    MonthlyIncome = table.Column<long>(type: "INTEGER", nullable: false),
                    MonthlyExpenses = table.Column<long>(type: "INTEGER", nullable: false),
                    HardshipReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HardshipApplications", x => x.Id);
                    table.CheckConstraint("CK_HardshipApplications_MonthlyExpenses", "\"MonthlyExpenses\" >= 0");
                    table.CheckConstraint("CK_HardshipApplications_MonthlyIncome", "\"MonthlyIncome\" >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HardshipApplications_CreatedAt",
                table: "HardshipApplications",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HardshipApplications");
        }
    }
}
