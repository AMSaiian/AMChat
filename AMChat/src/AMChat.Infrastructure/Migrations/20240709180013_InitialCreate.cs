using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "amchat");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "users",
                schema: "amchat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chats",
                schema: "amchat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chats", x => x.id);
                    table.ForeignKey(
                        name: "fk_chats_users_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "amchat",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                schema: "amchat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    fullname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    description = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_profiles", x => x.id);
                    table.CheckConstraint("chk_messages_posted_time_not_in_future", "AGE(CURRENT_DATE, birthdate) >= INTERVAL '14 years'");
                    table.ForeignKey(
                        name: "fk_profiles_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "amchat",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chat_user",
                schema: "amchat",
                columns: table => new
                {
                    joined_chats_id = table.Column<Guid>(type: "uuid", nullable: false),
                    joined_users_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_user", x => new { x.joined_chats_id, x.joined_users_id });
                    table.ForeignKey(
                        name: "fk_chat_user_chats_joined_chats_id",
                        column: x => x.joined_chats_id,
                        principalSchema: "amchat",
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chat_user_users_joined_users_id",
                        column: x => x.joined_users_id,
                        principalSchema: "amchat",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                schema: "amchat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    posted_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.CheckConstraint("chk_messages_posted_time_not_in_future", "posted_time <= CURRENT_TIMESTAMP");
                    table.ForeignKey(
                        name: "fk_messages_chats_chat_id",
                        column: x => x.chat_id,
                        principalSchema: "amchat",
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_users_author_id",
                        column: x => x.author_id,
                        principalSchema: "amchat",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chat_user_joined_users_id",
                schema: "amchat",
                table: "chat_user",
                column: "joined_users_id");

            migrationBuilder.CreateIndex(
                name: "ix_chats_owner_id",
                schema: "amchat",
                table: "chats",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_author_id",
                schema: "amchat",
                table: "messages",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_chat_id",
                schema: "amchat",
                table: "messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_profiles_user_id",
                schema: "amchat",
                table: "profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_name",
                schema: "amchat",
                table: "users",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_user",
                schema: "amchat");

            migrationBuilder.DropTable(
                name: "messages",
                schema: "amchat");

            migrationBuilder.DropTable(
                name: "profiles",
                schema: "amchat");

            migrationBuilder.DropTable(
                name: "chats",
                schema: "amchat");

            migrationBuilder.DropTable(
                name: "users",
                schema: "amchat");
        }
    }
}
