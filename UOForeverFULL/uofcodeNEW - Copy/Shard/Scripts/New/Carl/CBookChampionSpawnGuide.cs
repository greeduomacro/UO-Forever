using System;
using Server;

namespace Server.Items
{
	public class CBookChampionSpawnGuide : BaseBook
	{
		public static readonly BookContent Content = new BookContent
		(
			"Champion Spawn Guide", "Trystan",

			new BookPageInfo
			(
				"General Info - Champion",
				"spawns are on a 3-4 hour",
				"timer meaning if you do",
				"one that same one can't",
				"be done for another 3-4",
				"hours. - To activate a",
				"Champion Spawn you must",
				"stand on the Altar with"
			),
			new BookPageInfo
			(
				"3 other players. You",
				"will get a warning and",
				"30 seconds later the",
				"mobs will start",
				"spawning. - Champion",
				"Spawns will release",
				"waves of monsters that",
				"progressively gets"
			),
			new BookPageInfo
			(
				"harder over time,",
				"eventually leading to a",
				"boss that must be",
				"killed. - As monsters",
				"are killed Red candle",
				"skulls will appear",
				"around the Altar",
				"indicating your"
			),
			new BookPageInfo
			(
				"progress, if you kill",
				"too slowly or leave the",
				"area the skulls progress",
				"will be reversed. - The",
				"rewards for doing a",
				"champion are 3 power",
				"scrolls, the chance at a",
				"clothing bless deed, a"
			),
			new BookPageInfo
			(
				"champion skull",
				"(eventually used to",
				"spawn The Harrower) and",
				"over 50k gold will",
				"explode onto the ground",
				"in the area around the",
				"boss. General Tips -",
				"When doing a champion"
			),
			new BookPageInfo
			(
				"spawn place your group",
				"near the edge of the",
				"spawn so you can handle",
				"it bit by bit. Do not",
				"set up camp right in the",
				"middle of the altar",
				"unless you have safely",
				"cleared to it. - Summons"
			),
			new BookPageInfo
			(
				"such as energy vortex",
				"are useful for clearing",
				"champs, even better you",
				"can craft some EV",
				"scrolls. - Use field",
				"spells such as fire",
				"field and poison field",
				"onto big groups of"
			),
			new BookPageInfo
			(
				"monsters to clear them",
				"out faster. - PvM",
				"templates such as",
				"provokers and tamers are",
				"ideal champion spawn",
				"killers. The Champion",
				"Spawns Vermin Horde",
				"Location: Despise Level"
			),
			new BookPageInfo
			(
				"3 1 to 5 Red Candles:",
				"Giant Rats and Slimes 6",
				"to 9 Red Candles: Dire",
				"Wolves and Ratmen 10 to",
				"13 Red Candles: Hell",
				"Hounds and Ratman Mages",
				"14 to 16 Red Candles :",
				"Ratman Archers and"
			),
			new BookPageInfo
			(
				"Silver Serpents The",
				"Champion: Barracoon",
				"Abyss Location: Abyss",
				"Portal Gate - Enter on",
				"top of FIre Temple 1 to",
				"5 Red Candles: Mongbats",
				"and Imps 6 to 9 Red",
				"Candles: Gargoyles and"
			),
			new BookPageInfo
			(
				"Harpies 10 to 13 Red",
				"Candles: Fire Gargoyles",
				"and Stone Gargoyles 14",
				"to 16 Red Candles :",
				"Daemons and Succubi The",
				"Champion: Semidar Unholy",
				"Terror Location: Deceit",
				"Level 3 1 to 5 Red"
			),
			new BookPageInfo
			(
				"Candles: Bogles, Ghouls,",
				"Shades, Spectres and",
				"Wraiths 6 to 9 Red",
				"Candles: Bone Magi,",
				"Mummies and Skeletal",
				"Mages 10 to 13 Red",
				"Candles: Bone Knights,",
				"Liches and Skeletal"
			),
			new BookPageInfo
			(
				"Knights 14 to 16 Red",
				"Candles : Lich Lords and",
				"Rotting Corpses The",
				"Champion: Neira the",
				"Necromancer Arachnid",
				"Location: Island with",
				"Valor Shrine 1 to 5 Red",
				"Candles: Scorpions and"
			),
			new BookPageInfo
			(
				"Giant Spiders 6 to 9 Red",
				"Candles: Terathan Drones",
				"and Terathan Warriors 10",
				"to 13 Red Candles: Dread",
				"Spiders and Terathan",
				"Matriarchs 14 to 16 Red",
				"Candles : Poison",
				"Elementals and Terathan"
			),
			new BookPageInfo
			(
				"Avengers The Champion:",
				"Mephitis"
			)
		);

		public override BookContent DefaultContent{ get{ return Content; } }

		[Constructable]
		public CBookChampionSpawnGuide() : base( 0x1C13, false )
		{
			Hue = 723;
		}

		public CBookChampionSpawnGuide( Serial serial ) : base( serial )
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
