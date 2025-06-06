﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedOfficeBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingTypeToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Bookings");
        }
    }
}
