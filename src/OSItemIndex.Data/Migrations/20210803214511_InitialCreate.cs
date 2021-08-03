using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OSItemIndex.Data;

namespace OSItemIndex.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false),
                    details = table.Column<object>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_p_k_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    buy_limit = table.Column<int>(type: "integer", nullable: true),
                    cost = table.Column<int>(type: "integer", nullable: true),
                    duplicate = table.Column<bool>(type: "boolean", nullable: false),
                    equipable = table.Column<bool>(type: "boolean", nullable: false),
                    equipable_by_player = table.Column<bool>(type: "boolean", nullable: false),
                    equipable_weapon = table.Column<bool>(type: "boolean", nullable: false),
                    equipment = table.Column<Equipment>(type: "jsonb", nullable: true),
                    examine = table.Column<string>(type: "text", nullable: true),
                    highalch = table.Column<int>(type: "integer", nullable: true),
                    icon = table.Column<byte[]>(type: "bytea", nullable: false),
                    incomplete = table.Column<bool>(type: "boolean", nullable: false),
                    linked_id_item = table.Column<int>(type: "integer", nullable: true),
                    linked_id_noted = table.Column<int>(type: "integer", nullable: true),
                    linked_id_placeholder = table.Column<int>(type: "integer", nullable: true),
                    lowalch = table.Column<int>(type: "integer", nullable: true),
                    members = table.Column<bool>(type: "boolean", nullable: false),
                    noteable = table.Column<bool>(type: "boolean", nullable: false),
                    noted = table.Column<bool>(type: "boolean", nullable: false),
                    placeholder = table.Column<bool>(type: "boolean", nullable: false),
                    quest_item = table.Column<bool>(type: "boolean", nullable: false),
                    release_date = table.Column<string>(type: "text", nullable: true),
                    stackable = table.Column<bool>(type: "boolean", nullable: false),
                    stacked = table.Column<int>(type: "integer", nullable: true),
                    tradeable = table.Column<bool>(type: "boolean", nullable: false),
                    tradeable_on_ge = table.Column<bool>(type: "boolean", nullable: false),
                    weapon = table.Column<Equipment.WeaponInfo>(type: "jsonb", nullable: true),
                    weight = table.Column<double>(type: "double precision", nullable: true),
                    wiki_name = table.Column<string>(type: "text", nullable: true),
                    wiki_url = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_p_k_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prices_realtime_five_minutes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    average_high_price = table.Column<int>(type: "integer", nullable: true),
                    high_price_volume = table.Column<int>(type: "integer", nullable: true),
                    average_low_price = table.Column<int>(type: "integer", nullable: true),
                    low_price_volume = table.Column<int>(type: "integer", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_p_k_prices_realtime_five_minutes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prices_realtime_latest",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    high = table.Column<int>(type: "integer", nullable: true),
                    high_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    low = table.Column<int>(type: "integer", nullable: true),
                    low_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_p_k_prices_realtime_latest", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prices_realtime_one_hour",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    average_high_price = table.Column<int>(type: "integer", nullable: true),
                    high_price_volume = table.Column<int>(type: "integer", nullable: true),
                    average_low_price = table.Column<int>(type: "integer", nullable: true),
                    low_price_volume = table.Column<int>(type: "integer", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_p_k_prices_realtime_one_hour", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "prices_realtime_five_minutes");

            migrationBuilder.DropTable(
                name: "prices_realtime_latest");

            migrationBuilder.DropTable(
                name: "prices_realtime_one_hour");
        }
    }
}
