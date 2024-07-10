using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMessageToHandleSystemMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "author_id",
                schema: "amchat",
                table: "messages",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "kind",
                schema: "amchat",
                table: "messages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kind",
                schema: "amchat",
                table: "messages");

            migrationBuilder.AlterColumn<Guid>(
                name: "author_id",
                schema: "amchat",
                table: "messages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
