using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OSItemIndex.Data
{
    public class OsrsBoxItem : ItemEntity
    {
        /// <summary>
        /// The last time (UTC) the item was updated (in ISO8601 date format).
        /// </summary>
        [JsonPropertyName("last_updated")]
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// The Grand Exchange buy limit of the item.
        /// </summary>
        [JsonPropertyName("buy_limit")]
        public int? BuyLimit { get; set; }

        /// <summary>
        /// The store price of an item.
        /// </summary>
        [JsonPropertyName("cost")]
        public int? Cost { get; set; }

        /// <summary>
        /// If the item is a duplicate.
        /// </summary>
        [JsonPropertyName("duplicate")]
        public bool Duplicate { get; set; }

        /// <summary>
        /// If the item is equipable (based on right-click menu entry).
        /// </summary>
        [JsonPropertyName("equipable")]
        public bool Equipable { get; set; }

        /// <summary>
        /// If the item is equipable in-game by a player.
        /// </summary>
        [JsonPropertyName("equipable_by_player")]
        public bool EquipableByPlayer { get; set; }

        /// <summary>
        /// If the item is an equipable weapon.
        /// </summary>
        [JsonPropertyName("equipable_weapon")]
        public bool EquipableWeapon { get; set; }

        /// <summary>
        /// The equipment bonuses of equipable armour/weapons.
        /// </summary>
        [JsonPropertyName("equipment")]
        [Column(TypeName = "json")]
        public Equipment? Equipment { get; set; }

        /// <summary>
        /// The examine text for the item.
        /// </summary>
        [JsonPropertyName("examine")]
        public string Examine { get; set; }

        /// <summary>
        /// The high alchemy value of the item (cost * 0.6).
        /// </summary>
        [JsonPropertyName("highalch")]
        public int? Highalch { get; set; }

        /// <summary>
        /// The item icon (in base64 encoding).
        /// </summary>
        [JsonPropertyName("icon")]
        public byte[] Icon { get; set; }

        /// <summary>
        /// If the item has incomplete wiki data.
        /// </summary>
        [JsonPropertyName("incomplete")]
        public bool Incomplete { get; set; }

        /// <summary>
        /// The linked ID of the actual item (if noted/placeholder).
        /// </summary>
        [JsonPropertyName("linked_id_item")]
        public int? LinkedIdItem { get; set; }

        /// <summary>
        /// The linked ID of an item in noted form.
        /// </summary>
        [JsonPropertyName("linked_id_noted")]
        public int? LinkedIdNoted { get; set; }

        /// <summary>
        /// The linked ID of an item in placeholder form.
        /// </summary>
        [JsonPropertyName("linked_id_placeholder")]
        public int? LinkedIdPlaceholder { get; set; }

        /// <summary>
        /// The low alchemy value of the item (cost * 0.4).
        /// </summary>
        [JsonPropertyName("lowalch")]
        public int? Lowalch { get; set; }

        /// <summary>
        /// If the item is a members-only.
        /// </summary>
        [JsonPropertyName("members")]
        public bool Members { get; set; }

        /// <summary>
        /// If the item is noteable.
        /// </summary>
        [JsonPropertyName("noteable")]
        public bool Noteable { get; set; }

        /// <summary>
        /// If the item is noted.
        /// </summary>
        [JsonPropertyName("noted")]
        public bool Noted { get; set; }

        /// <summary>
        /// If the item is a placeholder.
        /// </summary>
        [JsonPropertyName("placeholder")]
        public bool Placeholder { get; set; }

        /// <summary>
        /// If the item is associated with a quest.
        /// </summary>
        [JsonPropertyName("quest_item")]
        public bool QuestItem { get; set; }

        /// <summary>
        /// Date the item was released (in ISO8601 format).
        /// </summary>
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        /// <summary>
        /// If the item is stackable (in inventory).
        /// </summary>
        [JsonPropertyName("stackable")]
        public bool Stackable { get; set; }

        /// <summary>
        /// If the item is stacked, indicated by the stack count.
        /// </summary>
        [JsonPropertyName("stacked")]
        public int? Stacked { get; set; }

        /// <summary>
        /// If the item is tradeable (between players and on the GE).
        /// </summary>
        [JsonPropertyName("tradeable")]
        public bool Tradeable { get; set; }

        /// <summary>
        /// If the item is tradeable (only on GE).
        /// </summary>
        [JsonPropertyName("tradeable_on_ge")]
        public bool TradeableOnGe { get; set; }

        /// <summary>
        /// The weapon bonuses including attack speed, type and stance.
        /// </summary>
        [JsonPropertyName("weapon")]
        [Column(TypeName = "json")]
        public Equipment.WeaponInfo? Weapon { get; set; }

        /// <summary>
        /// The weight (in kilograms) of the item.
        /// </summary>
        [JsonPropertyName("weight")]
        public double? Weight { get; set; }

        /// <summary>
        /// The OSRS Wiki Exchange URL.
        /// </summary>
        [JsonPropertyName("wiki_exchange")]
        public string WikiExchange { get; set; }

        /// <summary>
        /// The OSRS Wiki name for the item.
        /// </summary>
        [JsonPropertyName("wiki_name")]
        public string WikiName { get; set; }

        /// <summary>
        /// The OSRS Wiki URL (possibly including anchor link).
        /// </summary>
        [JsonPropertyName("wiki_url")]
        public string WikiUrl { get; set; }
    }

    /// <summary>
    /// The equipment bonuses of equipable armour/weapons.
    /// </summary>
    public class Equipment
    {
        /// <summary>
        /// The attack crush bonus of the item.
        /// </summary>
        [JsonPropertyName("attack_crush")]
        public int AttackCrush { get; set; }

        /// <summary>
        /// The attack magic bonus of the item.
        /// </summary>
        [JsonPropertyName("attack_magic")]
        public int AttackMagic { get; set; }

        /// <summary>
        /// The attack ranged bonus of the item.
        /// </summary>
        [JsonPropertyName("attack_ranged")]
        public int AttackRanged { get; set; }

        /// <summary>
        /// The attack slash bonus of the item.
        /// </summary>
        [JsonPropertyName("attack_slash")]
        public int AttackSlash { get; set; }

        /// <summary>
        /// The attack stab bonus of the item.
        /// </summary>
        [JsonPropertyName("attack_stab")]
        public int AttackStab { get; set; }

        /// <summary>
        /// The defence crush bonus of the item.
        /// </summary>
        [JsonPropertyName("defence_crush")]
        public int DefenceCrush { get; set; }

        /// <summary>
        /// The defence magic bonus of the item.
        /// </summary>
        [JsonPropertyName("defence_magic")]
        public int DefenceMagic { get; set; }

        /// <summary>
        /// The defence ranged bonus of the item.
        /// </summary>
        [JsonPropertyName("defence_ranged")]
        public int DefenceRanged { get; set; }

        /// <summary>
        /// The defence slash bonus of the item.
        /// </summary>
        [JsonPropertyName("defence_slash")]
        public int DefenceSlash { get; set; }

        /// <summary>
        /// The defence stab bonus of the item.
        /// </summary>
        [JsonPropertyName("defence_stab")]
        public int DefenceStab { get; set; }

        /// <summary>
        /// The magic damage bonus of the item.
        /// </summary>
        [JsonPropertyName("magic_damage")]
        public int MagicDamage { get; set; }

        /// <summary>
        /// The melee strength bonus of the item.
        /// </summary>
        [JsonPropertyName("melee_strength")]
        public int MeleeStrength { get; set; }

        /// <summary>
        /// The prayer bonus of the item.
        /// </summary>
        [JsonPropertyName("prayer")]
        public int Prayer { get; set; }

        /// <summary>
        /// The ranged strength bonus of the item.
        /// </summary>
        [JsonPropertyName("ranged_strength")]
        public int RangedStrength { get; set; }

        /// <summary>
        /// An object of requirements {skill: level}.
        /// </summary>
        [JsonPropertyName("requirements")]
        public Dictionary<string, object>? Requirements { get; set; }

        /// <summary>
        /// The equipment slot associated with the item (e.g., head).
        /// </summary>
        [JsonPropertyName("slot")]
        [Column(TypeName = "json")]
        public EquipmentSlot? Slot { get; set; }

        /// <summary>
        /// The weapon bonuses including attack speed, type and stance.
        /// </summary>
        public class WeaponInfo
        {
            /// <summary>
            /// The attack speed of a weapon (in game ticks).
            /// </summary>
            [JsonPropertyName("attack_speed")]
            public int AttackSpeed { get; set; }

            /// <summary>
            /// An array of weapon stance information.
            /// </summary>
            [JsonPropertyName("stances")]
            public WeaponStance[]? Stances { get; set; }

            /// <summary>
            /// The weapon classification (e.g., axes)
            /// </summary>
            [JsonPropertyName("weapon_type")]
            public WeaponClassType? WeaponType { get; set; }
        }

        public class WeaponStance
        {
            [JsonPropertyName("attack_style")] public WeaponAttackStyle? AttackStyle { get; set; }

            [JsonPropertyName("attack_type")] public WeaponAttackType? AttackType { get; set; }

            [JsonPropertyName("boosts")] public WeaponBoosts? Boosts { get; set; }

            [JsonPropertyName("combat_style")] public WeaponCombatStyle? CombatStyle { get; set; }

            [JsonPropertyName("experience")] public WeaponExperienceStyle? Experience { get; set; }
        }

        /// <summary>
        /// The equipment slot associated with the item (e.g., head).
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum EquipmentSlot
        {
            [EnumMember(Value = "ammo")] Ammo,
            [EnumMember(Value = "body")] Body,
            [EnumMember(Value = "cape")] Cape,
            [EnumMember(Value = "feet")] Feet,
            [EnumMember(Value = "hands")] Hands,
            [EnumMember(Value = "head")] Head,
            [EnumMember(Value = "legs")] Legs,
            [EnumMember(Value = "neck")] Neck,
            [EnumMember(Value = "ring")] Ring,
            [EnumMember(Value = "shield")] Shield,
            [EnumMember(Value = "2h")] TwoHand,
            [EnumMember(Value = "weapon")] Weapon
        }

        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum WeaponAttackStyle
        {
            [EnumMember(Value = "accurate")] Accurate,
            [EnumMember(Value = "aggressive")] Aggressive,
            [EnumMember(Value = "controlled")] Controlled,
            [EnumMember(Value = "defensive")] Defensive,
            [EnumMember(Value = "magic")] Magic
        }

        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum WeaponAttackType
        {
            [EnumMember(Value = "crush")] Crush,

            [EnumMember(Value = "defensive casting")]
            DefensiveCasting,
            [EnumMember(Value = "magic")] Magic,
            [EnumMember(Value = "ranged")] Ranged,
            [EnumMember(Value = "slash")] Slash,
            [EnumMember(Value = "spellcasting")] Spellcasting,
            [EnumMember(Value = "stab")] Stab
        }

        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum WeaponBoosts
        {
            [EnumMember(Value = "accuracy and damage")]
            AccuracyAndDamage,

            [EnumMember(Value = "attack range by 2 squares")]
            AttackRangeBy2Squares,

            [EnumMember(Value = "attack speed by 1 tick")]
            AttackSpeedBy1Tick
        }

        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum WeaponCombatStyle
        {
            [EnumMember(Value = "accurate")] Accurate,
            [EnumMember(Value = "aim and fire")] AimAndFire,
            [EnumMember(Value = "bash")] Bash,
            [EnumMember(Value = "blaze")] Blaze,
            [EnumMember(Value = "block")] Block,
            [EnumMember(Value = "chop")] Chop,
            [EnumMember(Value = "deflect")] Deflect,
            [EnumMember(Value = "fend")] Fend,
            [EnumMember(Value = "flare")] Flare,
            [EnumMember(Value = "flick")] Flick,
            [EnumMember(Value = "focus")] Focus,
            [EnumMember(Value = "hack")] Hack,
            [EnumMember(Value = "impale")] Impale,
            [EnumMember(Value = "jab")] Jab,
            [EnumMember(Value = "kick")] Kick,
            [EnumMember(Value = "lash")] Lash,
            [EnumMember(Value = "long fuse")] LongFuse,
            [EnumMember(Value = "longrange")] Longrange,
            [EnumMember(Value = "lunge")] Lunge,
            [EnumMember(Value = "medium fuse")] MediumFuse,
            [EnumMember(Value = "pound")] Pound,
            [EnumMember(Value = "pummel")] Pummel,
            [EnumMember(Value = "punch")] Punch,
            [EnumMember(Value = "rapid")] Rapid,
            [EnumMember(Value = "reap")] Reap,
            [EnumMember(Value = "scorch")] Scorch,
            [EnumMember(Value = "short fuse")] ShortFuse,
            [EnumMember(Value = "slash")] Slash,
            [EnumMember(Value = "smash")] Smash,
            [EnumMember(Value = "spell")] Spell,
            [EnumMember(Value = "spell (defensive)")] SpellDefensive,
            [EnumMember(Value = "spike")] Spike,
            [EnumMember(Value = "stab")] Stab,
            [EnumMember(Value = "swipe")] Swipe
        }

        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum WeaponExperienceStyle
        {
            [EnumMember(Value = "attack")] Attack,
            [EnumMember(Value = "defence")] Defence,
            [EnumMember(Value = "magic")] Magic,

            [EnumMember(Value = "magic and defence")]
            MagicAndDefence,
            [EnumMember(Value = "ranged")] Ranged,

            [EnumMember(Value = "ranged and defence")]
            RangedAndDefence,
            [EnumMember(Value = "shared")] Shared,
            [EnumMember(Value = "strength")] Strength
        }

        /// <summary>
        /// The weapon classification (e.g., axes)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumMemberConverter))]
        public enum WeaponClassType
        {
            [EnumMember(Value = "axe")] Axe,
            [EnumMember(Value = "banner")] Banner,
            [EnumMember(Value = "bladed_staff")] BladedStaff,
            [EnumMember(Value = "blaster")] Blaster,
            [EnumMember(Value = "bludgeon")] Bludgeon,
            [EnumMember(Value = "blunt")] Blunt,
            [EnumMember(Value = "bow")] Bow,
            [EnumMember(Value = "bulwark")] Bulwark,
            [EnumMember(Value = "chinchompas")] Chinchompas,
            [EnumMember(Value = "claw")] Claw,
            [EnumMember(Value = "crossbow")] Crossbow,
            [EnumMember(Value = "gun")] Gun,
            [EnumMember(Value = "pickaxe")] Pickaxe,
            [EnumMember(Value = "polearm")] Polearm,
            [EnumMember(Value = "polestaff")] Polestaff,
            [EnumMember(Value = "powered_staff")] PoweredStaff,
            [EnumMember(Value = "salamander")] Salamander,
            [EnumMember(Value = "scythe")] Scythe,
            [EnumMember(Value = "slash_sword")] SlashSword,
            [EnumMember(Value = "spear")] Spear,
            [EnumMember(Value = "spiked")] Spiked,
            [EnumMember(Value = "stab_sword")] StabSword,
            [EnumMember(Value = "staff")] Staff,
            [EnumMember(Value = "2h_sword")] TwoHandedSword,
            [EnumMember(Value = "thrown")] Thrown,
            [EnumMember(Value = "unarmed")] Unarmed,
            [EnumMember(Value = "whip")] Whip
        };
    }
}
