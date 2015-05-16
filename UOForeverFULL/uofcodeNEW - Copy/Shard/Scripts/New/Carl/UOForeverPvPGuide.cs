using System;
using Server;

namespace Server.Items
{
	public class UOForeverPvPGuide : BaseBook
	{
		public static readonly BookContent Content = new BookContent
		(
			"UO Forever PvP Guide", "Trystan",

			new BookPageInfo
			(
				"1. Combat mechanics and",
				"skills. 2. Templates 3.",
				"Equipment 4. Important",
				"Spells 5. Hot-keys 6.",
				"Basic Tactics 7.",
				"Advanced Group Fighting",
				"Tactics 1. Combat",
				"Mechanics and Skills."
			),
			new BookPageInfo
			(
				"Important Combat Skills",
				"and what they do. The",
				"combat mechanics of the",
				"era most accurately",
				"represent the Ultima",
				"Online Renaissance",
				"combat mechanics.",
				"Resisting Spells -"
			),
			new BookPageInfo
			(
				"Increases your chance to",
				"resist magic spells,",
				"needed to survive versus",
				"mages and all other",
				"spell casters. This is",
				"the most common skill",
				"found in any pvp",
				"template and whether"
			),
			new BookPageInfo
			(
				"you're a mage or warrior",
				"you want it. Magery -",
				"Allows you to cast a",
				"variety of different",
				"spells that are",
				"extremely useful in pvp,",
				"It is highly recommended",
				"to have magery for the"
			),
			new BookPageInfo
			(
				"spells you will be able",
				"to cast, important",
				"spells and their uses",
				"will be expanded upon.",
				"Evaluating Intelligence",
				"- This skill will up the",
				"amount of damage your",
				"spells do, very"
			),
			new BookPageInfo
			(
				"important for a mage",
				"template. Meditation -",
				"Increases the rate in",
				"which you will recover",
				"mana, needed spell for",
				"any mage template. You",
				"have a passive regen",
				"component and an"
			),
			new BookPageInfo
			(
				"'active' regen when you",
				"use the skill",
				"meditation. Wrestling -",
				"Reduces your chance to",
				"be hit by meele attacks,",
				"very important for",
				"making sure your spells",
				"do not get interrupted,"
			),
			new BookPageInfo
			(
				"another staple combat",
				"skill. Inscription -",
				"Increases the protection",
				"of your beneficial buff",
				"spells such as magic",
				"reflect, protection, and",
				"reactive armor. Alchemy",
				"- Increases the amount"
			),
			new BookPageInfo
			(
				"of damage an explosion",
				"potion will do.",
				"Poisoning - Allows you",
				"the ability to cast",
				"higher level poison such",
				"as deadly poison with",
				"less of a resist rate.",
				"Tactics - Increases the"
			),
			new BookPageInfo
			(
				"damage done by your",
				"meele weapons, having",
				"100 tactics is needed if",
				"you plan to use any type",
				"of weapon. Swords -",
				"Increases your chance to",
				"hit with Swordsmanship",
				"weapons. Special hit"
			),
			new BookPageInfo
			(
				"with a 2h: Reduces your",
				"targets intelligence by",
				"50%. Macing - Increases",
				"your chance to hit with",
				"Mace Fighting weapons.",
				"Special hit with a 2h:",
				"Chance to deal a",
				"'crushing' blow for"
			),
			new BookPageInfo
			(
				"double damage. Fencing -",
				"Increases your chance to",
				"hit with Fencing",
				"weapons. Special hit",
				"with a 2h: Delivers a",
				"paralyzing blow to your",
				"target for 2.0 seconds.",
				"Archery - Increases your"
			),
			new BookPageInfo
			(
				"chance to hit with",
				"ranged weapons. Healing",
				"- Increases your chance",
				"and the amount you heal",
				"while using bandages,",
				"also allows you to cure",
				"poison and resurrect",
				"others with high skill."
			),
			new BookPageInfo
			(
				"Needed for any warrior",
				"template. Anatomy -",
				"Increases your ability",
				"to heal with bandages",
				"(synergy with healing)",
				"and also increases the",
				"amount of damage you",
				"deal with physical"
			),
			new BookPageInfo
			(
				"weapons. Needed for any",
				"warrior template.",
				"Lumberjacking - Gives",
				"you bonus damage with",
				"using axes, (synergy",
				"with swords). Parrying -",
				"Gives you a 30% chance",
				"to block all damage from"
			),
			new BookPageInfo
			(
				"the next incoming attack",
				"and reduces the damage",
				"you take by a flat",
				"amount. Not the most",
				"ideal combat skill as",
				"you can not drink a",
				"potion or use a 2h with",
				"shields. Combining the"
			),
			new BookPageInfo
			(
				"use of two skills",
				"together can unlock",
				"certain special",
				"abilities. Anatomy +",
				"Wrestling - Allows you",
				"to deliver a 2 second",
				"stun punch Evaluating",
				"Int + Anatomy - Gives"
			),
			new BookPageInfo
			(
				"you 'defensive",
				"wrestling' if you have",
				"anatomy and eval int on",
				"a mage template it will",
				"effectively give you the",
				"same benefit as GM",
				"wrestling would, this",
				"makes tank (weapon"
			),
			new BookPageInfo
			(
				"mages) more viable. 2.0",
				"PvP Templates Using the",
				"proper PvP based",
				"template is going to be",
				"the first step in being",
				"successful. There are",
				"many builds used by",
				"players however this"
			),
			new BookPageInfo
			(
				"will show some of the",
				"more common templates to",
				"give an idea of what",
				"works. Follow this link",
				"to a detailed write up",
				"on many of the builds",
				"found on the shard.",
				"viewtopic.php?f=13&t=1640"
			),
			new BookPageInfo
			(
				" Scribe/Stun Mage Stats:",
				"90 str, 35 dex, 100 int",
				"- Magery, Resisting",
				"spells, Evaluating int,",
				"Meditation, Wrestling,",
				"Anatomy, Inscription.",
				"This is a very good",
				"group fighting mage,"
			),
			new BookPageInfo
			(
				"inscription will buff",
				"your protective line of",
				"spells, while having",
				"anatomy and wrestling",
				"will let you deliver a",
				"stun punch and protect",
				"you from physical",
				"damage. The 4 mage"
			),
			new BookPageInfo
			(
				"skills (magery, resist,",
				"eval int, meditation)",
				"are needed in every mage",
				"template. Swords Tank",
				"Mage Stats: 90 str, 35",
				"dex, 100 int - Magery,",
				"Resisting spells,",
				"Evaluating int,"
			),
			new BookPageInfo
			(
				"Meditation, Anatomy,",
				"Tactics, Swordsmanship",
				"Same set up as before",
				"however this mage is a",
				"'tank' mage meaning that",
				"you will be using a",
				"weapon in conjunction",
				"with your spells. We"
			),
			new BookPageInfo
			(
				"dropped anatomy and",
				"inscription for tactics",
				"and swords and we",
				"removed wrestling and",
				"picked up anatomy as",
				"having eval int +",
				"anatomy gives you the",
				"same bonus as wrestling."
			),
			new BookPageInfo
			(
				"Alchemy Fencer Stats: 90",
				"str, 95 dex, 40 int -",
				"Tactics, Fencing,",
				"Anatomy, Healing,",
				"Resisting spells,",
				"Alchemy, Magery Very",
				"potent dexer template",
				"that allows you to toss"
			),
			new BookPageInfo
			(
				"stronger explosion",
				"potions while providing",
				"constant pressure with",
				"fencing weapons.",
				"Lumberjacker Stats: 90",
				"str, 95 dex, 40 int -",
				"Tactics, Swords,",
				"Anatomy, Healing,"
			),
			new BookPageInfo
			(
				"Resisting spells,",
				"Lumberjacking, Magery",
				"Good template vs mages",
				"as you can deliver a",
				"concussion blow with a",
				"2h axe and lumberjacking",
				"will up its damage. 3.",
				"Equipment There are many"
			),
			new BookPageInfo
			(
				"items that you are",
				"always going to want to",
				"carry. The biggest",
				"downfall you can have is",
				"not properly equipping",
				"yourself. By doing so,",
				"you are just setting",
				"yourself up for failure"
			),
			new BookPageInfo
			(
				"right from the start.",
				"Here are some supplies",
				"that you should carry",
				"when stocking for a",
				"fight. Main importance",
				"is having an abundance",
				"of potions to use.",
				"Ideally potions should"
			),
			new BookPageInfo
			(
				"not be a limiting",
				"factor, you want to be",
				"extremely liberal with",
				"them using them as much",
				"as possible. If you are",
				"not then your opponent",
				"more then likely is",
				"which is giving him the"
			),
			new BookPageInfo
			(
				"upper hand. Greater",
				"Strength potions are",
				"extremely undervalued by",
				"a lot of people. You",
				"always want to keep",
				"yourself under the",
				"effect of a greater",
				"strength potion as it"
			),
			new BookPageInfo
			(
				"will up your health",
				"pool. Using potions will",
				"eventually become second",
				"nature and you will",
				"start instinctively",
				"using potions when you",
				"need to. Mage Stock 75",
				"of each reagent Full"
			),
			new BookPageInfo
			(
				"suit of exceptional",
				"leather armor (Armor",
				"increases in value",
				"depending on hide types)",
				"normal - spined - horned",
				"- barbed 10 Greater",
				"Healing Potions 10 Total",
				"Refresh Potions 10"
			),
			new BookPageInfo
			(
				"Greater Cure Potions 5",
				"Greater Strength Potions",
				"5 Greater Agility Potion",
				"(Stun Punch) 5-10",
				"Greater Explosion",
				"potions (useful with",
				"alchemy) 10 Trapped",
				"Pouches Weapons and"
			),
			new BookPageInfo
			(
				"Bandages (if applicable",
				"to your template) Dexer",
				"Stock Your weapon:",
				"Ideally you want to use",
				"a force + weapon that",
				"has deadly poison",
				"applied to the blade. 75",
				"each reagent 200"
			),
			new BookPageInfo
			(
				"bandages Full suit of",
				"exceptional chain/plate",
				"mixed dexer suit. 10",
				"Greater Healing Potions",
				"10 Total Refresh Potions",
				"10 Greater Cure Potions",
				"5 Greater Strength",
				"Potions 5 Greater"
			),
			new BookPageInfo
			(
				"Agility Potion 5-10",
				"Greater Explosion",
				"potions (useful with",
				"alchemy) 10 Trapped",
				"Pouches When it comes to",
				"equipping your dexer you",
				"are going to use many of",
				"the same items that a"
			),
			new BookPageInfo
			(
				"mage would equip with",
				"however there is a",
				"greater importance added",
				"to the quality of the",
				"weapon you wield.",
				"Ideally a dexer looking",
				"to pvp is using a",
				"magical weapon that is"
			),
			new BookPageInfo
			(
				"force + or stronger.",
				"Weapons go Ruin - Might",
				"- Fencing - Power -",
				"Vanq, An exceptionally",
				"crafted weapon has the",
				"same damage bonus of a",
				"magical wep that is",
				"might. Success with a"
			),
			new BookPageInfo
			(
				"dexer often relies on",
				"the type of weapons you",
				"are using. Here's an",
				"example of some commonly",
				"used weapons for each",
				"weapon skill. With that",
				"said, you generally want",
				"to use whatever your"
			),
			new BookPageInfo
			(
				"best magic is. For",
				"example a power scimitar",
				"(swords wep) will be far",
				"better to use then a",
				"might or exceptional",
				"Katana. However, a power",
				"katana would be ideal",
				"over the power scimitar."
			),
			new BookPageInfo
			(
				"Swordsmanship: Katana,",
				"Broadsword, Halbred. If",
				"LJ: Large Battle Axe,",
				"Double Axe Fencing: Long",
				"spear, Short spear,",
				"Kryss, War Fork (only",
				"spears can para blow)",
				"Macing: War hammer,"
			),
			new BookPageInfo
			(
				"Q-staff, War axe (only",
				"war hammer can crushing",
				"blow) Archery: Bow,",
				"Heavy X-bow 4. Important",
				"PvP Spells As stated",
				"before the skill magery",
				"provides a whole range",
				"of different spells that"
			),
			new BookPageInfo
			(
				"will be needed on nearly",
				"every template. Whether",
				"you are a mage or a",
				"dexer based template",
				"9/10 times you are going",
				"to want to have GM",
				"magery to cast spells.",
				"There are 64 spells"
			),
			new BookPageInfo
			(
				"available total and this",
				"section will take you",
				"through the ones you are",
				"going to want to use in",
				"PvP. First Circle",
				"Clumsy: Temporary debuff",
				"that reduces your",
				"targets dexterity by 11"
			),
			new BookPageInfo
			(
				"points with GM magery.",
				"Feeblemind: Temporary",
				"debuff that reduces your",
				"targets intelligence by",
				"11 points with GM",
				"magery. Weaken:",
				"Temporary debuff that",
				"reduces your targets"
			),
			new BookPageInfo
			(
				"intelligence by 11",
				"points with GM magery.",
				"Heal: Low mana cost,",
				"fast casting heal spell,",
				"heals for 11-15 with GM",
				"magery. Magic Arrow:",
				"Lowest damage nuke but",
				"fastest casting."
			),
			new BookPageInfo
			(
				"Reactive Armor: Provides",
				"a buff that absorbs",
				"meele damage, the amount",
				"it can reflect is upped",
				"with inscription. Second",
				"Circle Cure: Cures you",
				"of all poisons, very",
				"much needed as you can"
			),
			new BookPageInfo
			(
				"not heal through poison.",
				"Harm: Does damage",
				"depending on the",
				"distance you are from",
				"your target, very good",
				"interrupt spell, can do",
				"9-15 damage if next to",
				"your target for very"
			),
			new BookPageInfo
			(
				"little mana. Magic Trap:",
				"Needed spell to trap",
				"your pouches, not used",
				"in battle but in",
				"preparation before the",
				"fight. Protection: Self",
				"buff that reduces your",
				"chance to be interrupted"
			),
			new BookPageInfo
			(
				"while casting,",
				"inscription will up this",
				"bonus. Third Circle",
				"Bless: Increases your",
				"strength, dex, and int",
				"by 11 points with GM",
				"magery. Very good",
				"self-buff spell, use"
			),
			new BookPageInfo
			(
				"before battle. Poison:",
				"Cast a poison on your",
				"target, blocks heals (no",
				"heal through poison).",
				"Teleport: Allows you to",
				"cast a teleport up to 10",
				"tiles away, useful when",
				"chasing or escaping (can"
			),
			new BookPageInfo
			(
				"tele over barriers or",
				"across rivers). Wall of",
				"Stone: Cast a 3 tile",
				"long wall of stone at",
				"your target, lets you",
				"block runners or provide",
				"you with an opportunity",
				"to escape. Fourth Circle"
			),
			new BookPageInfo
			(
				"Greater Heal: Your bread",
				"and butter heal spell.",
				"Will heal for approx",
				"40-45 health points.",
				"Very important spell",
				"that you will get the",
				"most use out of to save",
				"yourself or cross heal a"
			),
			new BookPageInfo
			(
				"friend. Lightning: Solid",
				"medium damage nuke,",
				"versatile spell, very",
				"good follow up spell",
				"after an explosion-ebolt",
				"combo, Arch Protection:",
				"An area of effect buff",
				"spell that increases"
			),
			new BookPageInfo
			(
				"your armor. Will hit",
				"anyone around you",
				"(including enemies) -",
				"apply to friendlies",
				"before battle. Mana",
				"Drain: Lets you drain",
				"mana from your target,",
				"mana drain is best use"
			),
			new BookPageInfo
			(
				"to break a players magic",
				"reflect. It deals no",
				"damage to you, is low",
				"mana cost and fast",
				"casting. Hotkey this as",
				"your opening move to",
				"check for reflect.",
				"Recall: Travel spell,"
			),
			new BookPageInfo
			(
				"recall is needed to move",
				"around the world. Try to",
				"break line of sight by",
				"running behind a wall or",
				"obstacle then recall",
				"away to avoid death.",
				"Firth Circle Magic",
				"Reflection: Self buff"
			),
			new BookPageInfo
			(
				"spell that allows you to",
				"reflect magic spells.",
				"Inscription will let you",
				"reflect more spells.",
				"Paralyze: Zero damage",
				"offensive spell that",
				"roots your target in",
				"place for an amount of"
			),
			new BookPageInfo
			(
				"time. If the target",
				"takes damage the",
				"paralyze effect is",
				"broken. This is why",
				"Trapped pouches are",
				"needed as you use them",
				"to deal insignificant",
				"damage to yourself to"
			),
			new BookPageInfo
			(
				"break the paralyze",
				"effect. Mind Blast:",
				"Offensive spell that",
				"deals damage depending",
				"on the stats of the",
				"player it hits. Players",
				"with less balanced stats",
				"(such as 100 str, 100"
			),
			new BookPageInfo
			(
				"dex, 25 int) will",
				"receive more damage.",
				"Dispel Field: Cast this",
				"spell to remove walls of",
				"stone and summoned",
				"gates. Important if you",
				"need to stop a runner",
				"from taking a gate, or"
			),
			new BookPageInfo
			(
				"if you need to dispel a",
				"wall to save a friend.",
				"Sixth Circle Explosion:",
				"Main nuke spell. After",
				"you cast it with your",
				"target will be hit with",
				"the damage after a 2",
				"second delay. Energy"
			),
			new BookPageInfo
			(
				"Bolt: Main nuke spell.",
				"Energy bolt is the bread",
				"and butter damage spell",
				"used by any mage, it",
				"simply releases a bolt",
				"of energy at your target",
				"that deals high damage.",
				"Combo by casting"
			),
			new BookPageInfo
			(
				"explosion followed by an",
				"energy bolt to do a",
				"large amount of damage",
				"at one time. Mark: Lets",
				"you mark a rune, then",
				"you can recall off of",
				"it. Marking is important",
				"because you want to"
			),
			new BookPageInfo
			(
				"rulebooks that have good",
				"locations to cut people",
				"off as well as having",
				"safe Gate-In points for",
				"attacking others.",
				"Invisibility: Cast this",
				"spell to hide yourself,",
				"only lasts a few"
			),
			new BookPageInfo
			(
				"minutes. Reveal: Cast",
				"this spell to reveal",
				"hidden players. Sixth",
				"Circle Flame Strike:",
				"Heavy hitting, high mana",
				"cost, high cast time",
				"spell. Gate Travel:",
				"Travel spell used to"
			),
			new BookPageInfo
			(
				"open a gate anyone can",
				"travel through. Eighth",
				"Circle Resurrection:",
				"Cast this spell to",
				"resurrect your fallen",
				"allies. Hotkey Layout It",
				"is really important that",
				"if you want to hold your"
			),
			new BookPageInfo
			(
				"own in any sort of pvp",
				"encounter that you have",
				"a good hotkey layout",
				"that you are well",
				"adjusted to, and can",
				"easily remember off of",
				"the top of your head.",
				"There are a lot of"
			),
			new BookPageInfo
			(
				"spells out there,",
				"however, you don't",
				"really need to hotkey",
				"all of them just the",
				"most used. The less used",
				"spells can be clicked",
				"through a spell gump",
				"from your spell book."
			),
			new BookPageInfo
			(
				"You want to have a good",
				"hot-key layout as anyone",
				"spending time clicking",
				"spells or items is",
				"surely toast! Here's an",
				"example of my specific",
				"spell layout, it has",
				"always worked for me and"
			),
			new BookPageInfo
			(
				"I have been using",
				"similar for years. All",
				"of these hotkeys are set",
				"through razor however",
				"the same hotkey options",
				"are available through UO",
				"assist. How to set your",
				"hotkeys through Razor:"
			),
			new BookPageInfo
			(
				"shift + space -",
				"Enable/Disable Razor",
				"HotKeys (lets you turn",
				"off the hotkey",
				"functions, so you can",
				"set hotkeys to basic",
				"keyboard commands such a",
				"(1, 2, 3, a, b, c) but"
			),
			new BookPageInfo
			(
				"when you want to use",
				"these keys to talk to a",
				"player in game you can",
				"toggle off the hotkeys",
				"to razor no longer",
				"transmits to your UO",
				"clients letting you type",
				"in game. Targeting -"
			),
			new BookPageInfo
			(
				"Generally easiest to",
				"have your targeting",
				"functions tied to your",
				"mouse. Side mouse button",
				"1 - Last Target (after",
				"casting a spell, hit",
				"this key to drop your",
				"pre-loaded spell on your"
			),
			new BookPageInfo
			(
				"last target) Mouse wheel",
				"up - Set last target",
				"(use this to bring up a",
				"cursor to set your last",
				"target, alternatively,",
				"tagging someone with a",
				"spell by clicking them",
				"or their bar will set"
			),
			new BookPageInfo
			(
				"them as your last",
				"target) Mouse wheel down",
				"- Target Self (same deal",
				"as Last target but you",
				"will always target",
				"yourself using this)",
				"Items Ctrl + 1 - Use",
				"Greater Healing Potion"
			),
			new BookPageInfo
			(
				"Ctrl + 2 - Use Greater",
				"Cure Potion Ctrl + 3 -",
				"Use Total Refresh Potion",
				"Ctrl + 4 - Use Greater",
				"Strength Potion Ctrl + 5",
				"- Use Greater Agility",
				"potion Ctrl + e - Use",
				"Greater Explosion potion"
			),
			new BookPageInfo
			(
				"Alt + 1 - Use Bandages t",
				"- use Trapped Pouches -",
				"Create a macro through",
				"razor or assist uo that",
				"has your player say",
				"[pouch , then set a",
				"hotkey to activate that",
				"macro. The [pouch"
			),
			new BookPageInfo
			(
				"command will pop a",
				"trapped pouch in your",
				"bag. ` (tilde) -",
				"Arm/Disarm Right Hand -",
				"Arm/Disarms a 2h weapon",
				"(needed if you use a",
				"weapon) alt + ` (tilde)",
				"- Arm/Disarm Left Hand -"
			),
			new BookPageInfo
			(
				"Arm/Disarms a 1h weapon",
				"(needed if you use a",
				"weapon) Spell Layout F1",
				"- Magic Reflect F2 -",
				"Reactive Armor F3 -",
				"Protection F4 - Arch",
				"Protection F5 - Bless 1",
				"- Greater Heal 2 - Mini"
			),
			new BookPageInfo
			(
				"Heal 3 - Cure 4 -",
				"Paralyze 5 - Teleport 6",
				"- Wall of stone q -",
				"Explosion w - EnergyBolt",
				"e - Harm r - Flamestrike",
				"Shift + r -Recall a -",
				"Weaken s - Feeblemind d",
				"- Clumsy f - Poison g -"
			),
			new BookPageInfo
			(
				"Lghtning z -",
				"Invisibility x - Reveal",
				"c - Dispel field v -",
				"Mana drain Special",
				"Moves/Misc/Skills b -",
				"Toggle Stun (lets you",
				"toggle your stun punch",
				"for anatomy/wrestling) m"
			),
			new BookPageInfo
			(
				"- show/hide map p -",
				"accept party invitation",
				"ctrl + space - use skill",
				"meditation ctrl + b-",
				"show all names on screen",
				"shift + b -",
				"resynchronize client",
				"(sometimes client can"
			),
			new BookPageInfo
			(
				"get out of synch showing",
				"you on the wrong tile)",
				"That covers nearly",
				"everything important you",
				"would want to hotkey.",
				"Keep in mind this is",
				"just an example of what",
				"works for me, and I feel"
			),
			new BookPageInfo
			(
				"this layout is a bit",
				"more user-friendly then",
				"most others. 6. Basic",
				"Tactics Pre-Casting When",
				"you cast a spell you can",
				"hold the spell on your",
				"cursor letting you use",
				"it at a later time. Pre"
			),
			new BookPageInfo
			(
				"casting allows you to",
				"hold a spell 'pre cast'",
				"on your cursor letting",
				"you drop the spell at",
				"the moment you want.",
				"Lets say for example you",
				"are on a LJ dexer",
				"fighting a mage, you can"
			),
			new BookPageInfo
			(
				"precast a greater heal",
				"spell, then equip your",
				"weapon and attack him",
				"with the heal cursor up.",
				"Once he nukes you a",
				"couple times you can",
				"then drop your",
				"pre-casted heal on"
			),
			new BookPageInfo
			(
				"yourself without fear of",
				"it being interrupted mid",
				"cast. This tactic is",
				"also important for mages",
				"when doing group",
				"fighting. You will want",
				"to pre-cast important",
				"opener spells such as"
			),
			new BookPageInfo
			(
				"Explosion or Mana Drain",
				"before encountering",
				"others allowing you to",
				"get the first spells in.",
				"Getting and setting your",
				"targets. The first thing",
				"you are going to need to",
				"do is get the target of"
			),
			new BookPageInfo
			(
				"whoever you are",
				"attempting to kill.",
				"There are a few",
				"different ways in which",
				"you can set your last",
				"target. The most common",
				"way that this is done is",
				"by 'pulling the bar' of"
			),
			new BookPageInfo
			(
				"whoever your target is.",
				"Using Ctrl + Shift can",
				"make pulling bars easier",
				"at times. This simply",
				"comes down to being fast",
				"and precise with your",
				"mouse movements in order",
				"to grab the health bar"
			),
			new BookPageInfo
			(
				"of whoever you are",
				"targeting. Once you have",
				"the players HP bar on",
				"your screen you can set",
				"them to your last target",
				"by using mouse wheel up",
				"to activate your set",
				"last target hotkey."
			),
			new BookPageInfo
			(
				"Alternatively you can",
				"cast on the player and",
				"it will by default set",
				"him as your last target.",
				"Tip: Once you have the",
				"pulled a persons health",
				"bar it will be 'sticky'",
				"and not drop until that"
			),
			new BookPageInfo
			(
				"player dies or you click",
				"off the bar. Combo your",
				"spells Being good in PvP",
				"on a mage largely comes",
				"down to knowing what",
				"spells to cast at the",
				"right moments. This",
				"means knowing how to"
			),
			new BookPageInfo
			(
				"apply pressure",
				"offensively using your",
				"spells. The simplest",
				"combo you can do is an",
				"explode-energy bolt",
				"combo. This means",
				"precast your explosion",
				"then drop the spell"
			),
			new BookPageInfo
			(
				"immediately followed up",
				"by casting an energy",
				"bolt. As stated before",
				"the delay on the",
				"explosion times up with",
				"your energy bolt",
				"delivering a powerful",
				"burst of damage. Further"
			),
			new BookPageInfo
			(
				"adding to that you can",
				"explode-energybolt-lightn",
				"ing as lightning is a",
				"good finisher. Time your",
				"Interrupts By dropping",
				"your spell on a player",
				"while they are casting a",
				"spell you will interrupt"
			),
			new BookPageInfo
			(
				"that persons casting",
				"completely meaning they",
				"will have to recast that",
				"spell. Interrupting your",
				"opponents from casting",
				"at the right moment is a",
				"basic tactic that is",
				"used by everyone. Harm"
			),
			new BookPageInfo
			(
				"is a very good tool used",
				"for the purpose of",
				"interrupting. Higher",
				"level spells are poor",
				"for interrupting. For",
				"example, the cast speed",
				"on a harm will always",
				"beat out the cast on a"
			),
			new BookPageInfo
			(
				"greater heal, however if",
				"you go for an energy",
				"bolt and the person your",
				"fighting goes for a G",
				"heal he will be able to",
				"get that spell off",
				"before your energy bolt",
				"hits him. Focus Your"
			),
			new BookPageInfo
			(
				"Damage This is basic",
				"group fighting tactics,",
				"if you are up against",
				"somebody who knows how",
				"to keep himself alive,",
				"or has friends that are",
				"good at cross healing",
				"with greater heal, you"
			),
			new BookPageInfo
			(
				"are absolutely going to",
				"need to focus your",
				"damage on one target. At",
				"this most basic level",
				"this means having your",
				"guild master/leader call",
				"a 'target' that everyone",
				"is supposed to go after."
			),
			new BookPageInfo
			(
				"At a more advanced level",
				"this means using voice",
				"chat to synch the",
				"casting of your spells",
				"to drop at the same time",
				"(will be explained",
				"further). Not focusing",
				"your damage will lead to"
			),
			new BookPageInfo
			(
				"a bunch of random,",
				"non-burst damage on a",
				"bunch of targets which",
				"is easy to heal. Heal",
				"yourself and teammates!",
				"This part can not be",
				"stressed enough! Keeping",
				"yourself and your"
			),
			new BookPageInfo
			(
				"friends healed is super",
				"important, you should",
				"always be making sure",
				"you are not going to die",
				"and your buddy(s) aren't",
				"going to die before",
				"attempting any sort of",
				"offensive action. As"
			),
			new BookPageInfo
			(
				"stated before Greater",
				"Healing (4th circle) is",
				"your bread and butter",
				"heal spell, be very",
				"quick to hit your GH key",
				"if you see any sort of",
				"damage incoming. Mini",
				"heal and greater healing"
			),
			new BookPageInfo
			(
				"potions can work well in",
				"a pinch but greater",
				"healing does all the",
				"heavy lifting. Use Your",
				"Potions When you",
				"encounter somebody on",
				"the FIELD anything goes,",
				"you have to expect that"
			),
			new BookPageInfo
			(
				"the player you are about",
				"to fight is going to do",
				"everything they possibly",
				"can to win the battle.",
				"This will likely mean a",
				"liberal use of options.",
				"It should be second",
				"nature for you to be"
			),
			new BookPageInfo
			(
				"using your potions very",
				"frequently. For example,",
				"you pretty much always",
				"want to keep yourself",
				"under the effects of a",
				"Greater Strength Potion",
				"as it adds total health",
				"points. Enter a fight?"
			),
			new BookPageInfo
			(
				"Use a G strength, get",
				"weakened? G strength",
				"immediately. Same goes",
				"for all other pots. Buff",
				"Yourself Before Battle",
				"Before heading into a",
				"fight, or before you",
				"move out to even begin"
			),
			new BookPageInfo
			(
				"looking for a fight you",
				"want to make sure your",
				"are always keeping up as",
				"many of your defensive",
				"buffs as possible. You",
				"can either choose to use",
				"Magic Reflect, Reactive",
				"Armor, or Protection, as"
			),
			new BookPageInfo
			(
				"the three do not stack.",
				"Generally you want to",
				"cast Magic Reflect most",
				"of the time, however, if",
				"you come across a",
				"physical damage user you",
				"would want to switch",
				"that up to reactive"
			),
			new BookPageInfo
			(
				"armor once your reflect",
				"drops. Ready to get into",
				"a fight? Buff yourself",
				"with magic reflect,",
				"bless, arch protection",
				"(stacks with magic",
				"reflect), and drink a",
				"greater strength/agility"
			),
			new BookPageInfo
			(
				"potion. Running away and",
				"chasing Mounted combat",
				"is very fast paced and",
				"there is quite a bit of",
				"skill involved in the",
				"way you travel on your",
				"mount. You should always",
				"be looking at whats"
			),
			new BookPageInfo
			(
				"coming up in-front of",
				"you so you do not snag",
				"yourself on any rocks,",
				"trees, or other",
				"obstacles. Doing so for",
				"a moment can give vital",
				"ground to the people",
				"that are trying to kill"
			),
			new BookPageInfo
			(
				"you. At the same time if",
				"the person you are",
				"chasing gets caught up",
				"or hits something for a",
				"moment this gives you a",
				"chance to catch them.",
				"When running on a mount",
				"you want to try to"
			),
			new BookPageInfo
			(
				"anticipate where people",
				"are going to move and",
				"use shorter paths to cut",
				"them off. The straighter",
				"you run the better, too",
				"many zig-zagging",
				"movements really slows",
				"down your chasing and"
			),
			new BookPageInfo
			(
				"fleeing. If you are in a",
				"situation where you are",
				"trying to run for your",
				"life your survivability",
				"will be increased 10",
				"fold by having the right",
				"equipment. This means",
				"having trapped boxes to"
			),
			new BookPageInfo
			(
				"deal with any paralyze",
				"that might be cast on",
				"you and plenty of",
				"potions. Total refresh",
				"potions refill your",
				"stamina completely",
				"allowing you to 'shove",
				"them out of the way'"
			),
			new BookPageInfo
			(
				"letting you run through",
				"someone completely.",
				"Without full stam",
				"running into and PC or",
				"NPC will flat out block",
				"you making refresh pots",
				"very important. Besides",
				"that greater heal"
			),
			new BookPageInfo
			(
				"potions should be",
				"chugged on cooldown as",
				"well as greater strength",
				"if they weaken you. 6.",
				"Advanced Group Tactics",
				"Synch your spell drops",
				"Many of the pvp groups",
				"can easily heal through"
			),
			new BookPageInfo
			(
				"the damage dealt by",
				"players unless the",
				"damage is delivered by",
				"everyone at the exact",
				"same time. This is what",
				"is known as a 'synch",
				"dump, or drop' the",
				"basics behind a synch"
			),
			new BookPageInfo
			(
				"dump is that you are",
				"using voice chat to",
				"coordinate everyone",
				"casting their spells at",
				"the same time. This",
				"means having a lead or",
				"caller who is going to",
				"let everyone know who"
			),
			new BookPageInfo
			(
				"the target is and then",
				"call down the drop so",
				"everyone's spells hit at",
				"generally the same",
				"moment. Done by having",
				"everyone in your group",
				"pre-casting explosion",
				"and then once ready the"
			),
			new BookPageInfo
			(
				"caller will call down",
				"the synch such as '2 1",
				"drop' or 'ready now'.",
				"Everyone will then drops",
				"their explosion on the",
				"same target and follows",
				"up with an energy bolts.",
				"Time your poison As you"
			),
			new BookPageInfo
			(
				"can not heal through",
				"poison in this era one",
				"important group fighting",
				"tactic is to have one",
				"player cast",
				"explosion-poison instead",
				"of casting the standard",
				"explosion-energy bolt."
			),
			new BookPageInfo
			(
				"By having one player",
				"cast poison you can",
				"throw off the healing on",
				"the person you're",
				"targeting if they are",
				"not fast enough to cure",
				"their poison. This is a",
				"good tactic for throwing"
			),
			new BookPageInfo
			(
				"people off and keeping",
				"people on their toes, if",
				"they do not get the cure",
				"off fast enough they",
				"will not get healed in",
				"time and die. The trick",
				"to doing this is by",
				"delaying the poison and"
			),
			new BookPageInfo
			(
				"landing it right before",
				"the damage hits. As",
				"poison is a third level",
				"spell and everyone else",
				"is casting level six",
				"spells whoever is going",
				"to be casting poison",
				"will have to hold the"
			),
			new BookPageInfo
			(
				"spell and drop it right",
				"at the last moment.",
				"Requires properly timing",
				"your poison to be",
				"effective. Timing stuns",
				"or paralyze before",
				"dropping As many players",
				"use the tactics of"
			),
			new BookPageInfo
			(
				"'off-screening' a dump,",
				"this can be combated",
				"through the use of",
				"paralyze and stun punch.",
				"Stun punch (anatomy +",
				"wrestling) mages are",
				"very good group fighters",
				"because of this reason."
			),
			new BookPageInfo
			(
				"One thing you can do is",
				"stun the player that",
				"everyone is going to",
				"cast on so that he",
				"doesn't have time to",
				"offscreen the dump. It's",
				"very useful when chasing",
				"others as well because"
			),
			new BookPageInfo
			(
				"of how fast mount speed",
				"is. If you stun punch",
				"someone you can drop on",
				"him right away and often",
				"times his own group will",
				"offscreen him if the",
				"player doesn't call out",
				"he's stunned fast"
			),
			new BookPageInfo
			(
				"enough. Paralyze is",
				"another important spell",
				"that can be used in",
				"conjunction with your",
				"dump. In this situation",
				"you want to have",
				"everyone pre-cast",
				"explosion but you have"
			),
			new BookPageInfo
			(
				"one person pre cast a",
				"paralyze do paralyze-eb",
				"instead. When your",
				"caller calls down the",
				"drop the person will be",
				"paralyzed the exact",
				"second that all of the",
				"explosions are casted on"
			),
			new BookPageInfo
			(
				"them. It can be a highly",
				"effective group fighting",
				"tactic as just a second",
				"or two of paralyze can",
				"be enough for your group",
				"to get all their spells",
				"off. Quick dumping or a",
				"'reverse' dump This type"
			),
			new BookPageInfo
			(
				"of dump is called a",
				"quick dump because you",
				"do not want to have any",
				"spells precasted when",
				"your caller calls this",
				"down. For example if you",
				"are being chased by a",
				"group of players and"
			),
			new BookPageInfo
			(
				"they are all strung out",
				"with 1 player clearly in",
				"the lead you can do a",
				"quick dump on them by",
				"everyone clearing their",
				"pre-casts and then",
				"stopping and all casting",
				"an energy bolt right at"
			),
			new BookPageInfo
			(
				"the same time. This",
				"works great as well if",
				"you're outnumbered, one",
				"guy is chasing in the",
				"lead who is down to",
				"around half hp because",
				"you're outnumbered and",
				"they are being cocky."
			),
			new BookPageInfo
			(
				"Well at that point you",
				"can do quick dump energy",
				"bolts on that player if",
				"they do not realize",
				"RIGHT away then they are",
				"likely dead. Kite",
				"them/String them out",
				"Works only versus the"
			),
			new BookPageInfo
			(
				"groups who do not",
				"mobilize well. If you",
				"are with 3 or so of your",
				"friends and are fighting",
				"a larger group if your",
				"smaller group has better",
				"mobility and the larger",
				"group does not you can"
			),
			new BookPageInfo
			(
				"eventually string out",
				"the fight and try to",
				"pull a dump on whoever",
				"is in the lead as the",
				"larger group will be",
				"many screens apart. This",
				"goes hand and hand in",
				"the staying mobile as a"
			),
			new BookPageInfo
			(
				"group. Disorganized",
				"groups can eventually be",
				"picked apart by smaller",
				"ones if the larger one",
				"has poor",
				"mobility/discipline",
				"(runs off screen from",
				"the caller, runs ahead"
			),
			new BookPageInfo
			(
				"etc). Follow the next",
				"section to avoid falling",
				"victim to this. Staying",
				"mobile as a group",
				"Another important aspect",
				"of group fighting is",
				"making sure that",
				"everyone in your"
			),
			new BookPageInfo
			(
				"party/group/guild are",
				"all generally on the",
				"same page and not",
				"getting strung out",
				"everywhere. This",
				"requires a degree of",
				"discipline within your",
				"group and will come with"
			),
			new BookPageInfo
			(
				"practice but",
				"mobilization is very",
				"important. You do not",
				"want a situation where",
				"you are chasing a group",
				"and your group is split",
				"up/strung out chasing",
				"them in a line. Ideally"
			),
			new BookPageInfo
			(
				"you want to be no more",
				"then 5 tiles away from",
				"your caller at all",
				"times. This is important",
				"for synching your spells",
				"and cross healing your",
				"allies. If you run ahead",
				"of your group out of"
			),
			new BookPageInfo
			(
				"cross heal range unless",
				"you know you are 100%",
				"safe you are just being",
				"a liability.",
				"Offscreening synch-dumps",
				"and not being out of",
				"position This is one way",
				"to combat the"
			),
			new BookPageInfo
			(
				"effectiveness of a synch",
				"dump, if you have a",
				"bunch of people casting",
				"on you then you want to",
				"try to run off screen so",
				"you don't get the full",
				"force of the drop. There",
				"is no honor in sitting"
			),
			new BookPageInfo
			(
				"there on screen taking a",
				"full spell dump, if you",
				"know it is on you then",
				"you want to run, just be",
				"careful not to run away",
				"from your group, run",
				"behind them. Another",
				"thing is not finding"
			),
			new BookPageInfo
			(
				"yourself out in a weird",
				"position, follows the",
				"staying mobile as a",
				"group principle. If you",
				"are caught out in the",
				"open in a bad spot you",
				"are going to be an easy",
				"target. Always be wary"
			),
			new BookPageInfo
			(
				"of where you are moving.",
				"How to read who the dump",
				"is on and faking your",
				"own dump People who are",
				"experienced in Ultima",
				"Online combat can easily",
				"develop a sense of who a",
				"synch-dump is going to"
			),
			new BookPageInfo
			(
				"land on because of the",
				"direction people face",
				"when casting a spell. If",
				"you cast a spell on",
				"someone your chars",
				"feet/body will be",
				"pointed towards that",
				"player. So you can watch"
			),
			new BookPageInfo
			(
				"the movement of a player",
				"to get a read of who",
				"they're dropping on. To",
				"counter this you need to",
				"'fake' the drop. You can",
				"fake who you are",
				"targeting by",
				"purposefully pointing"
			),
			new BookPageInfo
			(
				"yourself to the person",
				"who you are NOT casting",
				"on the moment you do",
				"cast your spell. This",
				"makes it look like",
				"you're about to hit a",
				"target who you really",
				"aren't casting on."
			),
			new BookPageInfo
			(
				"Eventually this leads to",
				"a lot of mind games. A",
				"group that does not",
				"fake/turn when casting",
				"is very predictable."
			)
		);

		public override BookContent DefaultContent{ get{ return Content; } }

		[Constructable]
		public UOForeverPvPGuide() : base( 0x1C13, false )
		{
			Hue = 723;
		}

		public UOForeverPvPGuide( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
} 
