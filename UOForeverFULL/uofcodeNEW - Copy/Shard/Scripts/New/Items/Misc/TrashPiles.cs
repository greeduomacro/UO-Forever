// Lootable Trash Piles
// a RunUO ver 2.0 Script
// Script by David, Suggestion by Macil

using System;
using System.Collections.Generic;
using Server;

namespace Server.Items
{
	public class BaseTrash : BaseAddon
	{
		#region Variables/Properties
		private List<TrashLooter> Looters = new List<TrashLooter>();
		private TimeSpan m_LootWait = TimeSpan.FromMinutes(90);
		private int m_usesCount;
		private int m_MaxUses = 0; // use zero for trash pile never decays
		private int m_Sound = 64; // 62-66 or 324 are good sounds, use 0 for no sound
		private bool m_MakesCriminal = false;
		private double m_ItemChance = 0.005;
		private double m_RareChance = 0.00005;

		public virtual TimeSpan LootWait { get { return m_LootWait; } set { m_LootWait = value; } }
		public virtual int MaxUses { get { return m_MaxUses; } set { m_MaxUses = value; } }
		public virtual int UsesCount { get { return m_usesCount; } }
		public virtual int Sound { get { return m_Sound; } set { m_Sound = value; } }
		public virtual bool MakesCriminal { get { return m_MakesCriminal; } set { m_MakesCriminal = value; } }
		public virtual double ItemChance { get { return m_ItemChance; } set { m_ItemChance = value % 1; } }
		public virtual double RareChance { get { return m_RareChance; } set { m_RareChance = value % 1; } }
		#endregion

		#region Loot Tables
		//-----------------------------------------------------------------------------------------------
		// >>>>> NOTE! Review these lists--they may introduce new player owned items to your shard! <<<<<
		//-----------------------------------------------------------------------------------------------
		private static Type[] m_TrashCommonTypes = new Type[]
			{
				typeof( Bandage ),              typeof( Bandage ),              typeof( Kindling )
			};

		private static Type[] m_TrashItemTypes = new Type[]
			{
				typeof( Torch ),				typeof( Torch ),			    typeof( Torch ),
				typeof( Torch ),				typeof( Torch ),			    typeof( Torch ),
				typeof( Torch ),				typeof( Torch ),			    typeof( Torch ),
				typeof( Torch ),				typeof( Torch ),			    typeof( Torch ),
				typeof( Torch ),				typeof( Torch ),			    typeof( Torch ),
				typeof( DriedOnions ),			typeof( Eggshells ),			typeof( EmptyWoodenBowl ),
				typeof( Dagger ),				typeof( DirtyPan ),			    /*typeof( DiseasedBark ),*/
				typeof( Buckler ),				typeof( Carrot ),			    typeof( Club ),
				typeof( AxleGears ),			typeof( Axe ),			        /*typeof( BarkFragment ),*/
				typeof( Candle ),				typeof( Shoes ),			    typeof( Bag ),
				typeof( Beads ),				typeof( BarrelHoops ),			typeof( BarrelTap ),
				typeof( ClockParts ),			typeof( Springs ),				typeof( Gears ),
				typeof( Skillet ),				typeof( RollingPin ),			typeof( Shaft ),
				typeof( Hammer ),				typeof( Saw ),				    typeof( Nails ),
				typeof( Bottle ),				typeof( EmptyJar ),				typeof( GreenBottle ),
				typeof( PewterMug ),			typeof( CandleShort ),			typeof( FishingPole ),
				typeof( BlankMap ),				typeof( BlankScroll ),			typeof( Dices ),
				/*typeof( HorseShoes ),*/			typeof( SpoolOfThread ),		typeof( Scissors ),
				typeof( DirtyFrypan ),			typeof( DirtyPot ),				typeof( DirtySmallPot ),
				typeof( Fork ),		            typeof( Spoon ),				typeof( Knife ),
				typeof( Board ),				typeof( Boots ),				typeof( ButcherKnife ),
				typeof( RuinedClock ),			typeof( RuinedDrawers ),		typeof( RuinedFallenChairA ),
				typeof( Bandana ),	            typeof( WoodDebris ),		    typeof( RuinedFallenChairB ),
				typeof( FloppyHat ),			typeof( Sausage ),			    typeof( Garlic ),
				typeof( Banana ),			    typeof( Apple ),			    typeof( Cabbage ),
				typeof( BodySash ),             typeof( Doublet ),              typeof( Kilt ),
				typeof( Bonnet ),               typeof( Cap ),		            typeof( HalfApron ),
				typeof( BoneArms ),				typeof( BoneChest ),			typeof( BoneGloves ),
				typeof( LesserCurePotion ),		typeof( LesserHealPotion ),		typeof( LesserPoisonPotion ),
				typeof( TanBook ),				typeof( BrownBook ),			typeof( BlueBook ),
				typeof( Plate ),				typeof( KnifeRight ),			typeof( KnifeLeft ),
				typeof( SpoonRight ),			typeof( SpoonLeft ),			typeof( ForkRight ),
				typeof( ForkLeft ),				typeof( Pitcher ),				typeof( Keg ),
				typeof( WoodenBox ),			typeof( MetalBox ),				typeof( SmallCrate ),
				typeof( Pouch ),				typeof( IronOre ),				typeof( IronIngot ),
				typeof( Log ),					typeof( Hides ),				typeof( EmptyVial ),
				// scripted books...
				typeof( BirdsOfBritannia ),		typeof( BritannianFlora ),		typeof( ChildrenTalesVol2 ),
				typeof( QuestOfVirtues ),		typeof( RegardingLlamas ),		typeof( BoldStranger ),
				typeof( RankingsOfTrades ),		typeof( WildGirlOfTheForest ),	typeof( VirtueBook ),
				typeof( GrammarOfOrcish ),		typeof( CallToAnarchy ),		typeof( ArmsAndWeaponsPrimer ),
				typeof( TheFight ),				typeof( TamingDragons ),		typeof( LifeOfATravellingMinstrel )
			};

		private static Type[] m_TrashRareTypes = new Type[]
			{
				typeof( Amber ),				typeof( Amethyst ),				typeof( Citrine ),
				typeof( Rope ),				    typeof( GoldRing ),				typeof( GoldBracelet ),
				typeof( TrulyRareFish ),	    typeof( SilverRing ),			typeof( SilverBracelet ),
				typeof( MouldyCheese ),			typeof( SmallFlask ),			typeof( MediumFlask ),
				typeof( LargeFlask ),			typeof( CurvedFlask ),			typeof( LongFlask ),
				typeof( RedBottle ),			typeof( SmallBrownBottle ),		typeof( SmallGreenBottle ),
				typeof( SmallVioletBottle ),	typeof( TinyYellowBottle ),		typeof( SmallBlueFlask ),
				typeof( SmallYellowFlask ),		typeof( SmallRedFlask ),		typeof( SmallEmptyFlask ),
				typeof( YellowBeaker ),			typeof( RedBeaker ),			typeof( BlueBeaker ),
				typeof( GreenBeaker ),			typeof( EmptyCurvedFlaskW ),	typeof( RedCurvedFlask ),
				typeof( LtBlueCurvedFlask ),	typeof( EmptyCurvedFlaskE ),	typeof( BlueCurvedFlask ),
				typeof( GreenCurvedFlask ),		typeof( RedRibbedFlask ),		typeof( VioletRibbedFlask ),
				typeof( EmptyRibbedFlask ),		typeof( LargeYellowFlask ),		typeof( LargeVioletFlask ),
				typeof( LargeEmptyFlask ),		typeof( AniRedRibbedFlask ),	typeof( AniLargeVioletFlask ),
				typeof( AniSmallBlueFlask ),	typeof( SmallBlueBottle ),		typeof( SmallGreenBottle2 ),
				typeof( EmptyVialsWRack ),		typeof( FullVialsWRack ),		typeof( FullJar ),
				typeof( HalfEmptyJar ),			typeof( VioletStemmedBottle ),	typeof( EmptyJars3 ),
				typeof( EmptyJars4 ),			typeof( FullJars3 ),			typeof( DecoFullJars4 ),
				typeof( EmptyJars2 ),			/*typeof( FullJars2 ),*/			typeof( HalfEmptyJars2 ),
				typeof( TinyRedBottle ),		/*typeof( CandleLong ),*/			typeof( CandleShort ),
				typeof( HeatingStand )//,			typeof( CandleSkull )
			};

		private static Type[] m_TrashVeryRareTypes = new Type[]
			{
				typeof( DullCopperOre ),		typeof( CopperOre ),			typeof( BronzeOre ),
				typeof( GoldOre ),				typeof( AgapiteOre ),			typeof( VeriteOre ),
				typeof( ValoriteOre ),			typeof( MagnetiteOre ),			typeof( Amber ),
				typeof( Amethyst ),				typeof( Citrine ),
			};
		#endregion

		#region Methods
		public BaseTrash() : base()
		{
		}

		public string GetName(Item item)
		{
			string[] splitname;
			string[] splitplural;
			string name = String.Empty;

			if ( String.IsNullOrEmpty( name ) )
			{
				name = item.ItemData.Name;

				splitname = name.Split('%'); // 0 is the main item, 1 is the conjugation, 2+ is the rest of the word
				name = splitname[0].Trim().ToLower();
				if ( splitname.Length > 1 )
				{
					splitplural = splitname[1].Split('/'); //0 is Plural, 1 is Singular (if exists)
					if ( splitplural.Length > 1 )
						name += splitplural[1].Trim().ToLower();

					for ( int i = 2; i < splitname.Length; i++ ) //Add the rest of the name to the end
						name += splitname[i].Trim().ToLower();
				}

				if ( (item.ItemData.Flags & TileFlag.ArticleA) != 0 )
					name = "a " + name;
				else if ( (item.ItemData.Flags & TileFlag.ArticleAn) != 0 )
					name = "an " + name;
			}
			else
				name = item.Name;

			return name;
		}

		public virtual void OnLoot(Mobile from)
		{
			Item m_Loot;
			string m_Collected = "You have recovered ";

			DefragLooters();
			if (!FindLooter(from))
			{
				List<Item> loots = new List<Item>();

				bool rare = false;

				// improved chance of finding good loot based on certian skills
				// Calculated out of 200%
				double useskills = (from.Skills.Begging.Value + from.Skills.ItemID.Value) / 5000;

				// always one
				m_Loot = Loot.Construct( m_TrashCommonTypes );
				m_Loot.Movable = true;
				from.AddToBackpack(m_Loot);
				loots.Add( m_Loot );

				if ( Utility.RandomBool() ) // maybe two
				{
					m_Loot = Loot.Construct(m_TrashCommonTypes);
					m_Loot.Movable = true;
					from.AddToBackpack(m_Loot);
					loots.Add( m_Loot );
				}

				if ( ( m_ItemChance + ( useskills * 2 ) ) > Utility.RandomDouble() )// plus chance for a random item
				{
					m_Loot = Loot.Construct(m_TrashItemTypes);
					m_Loot.Movable = true;
					from.AddToBackpack(m_Loot);
					loots.Add( m_Loot );
				}
				else if ( m_RareChance + useskills > Utility.RandomDouble() )// plus chance for a random prize
				{
					m_Loot = Loot.Construct(m_TrashRareTypes);
					m_Loot.Movable = true;
					from.AddToBackpack(m_Loot);
					loots.Add( m_Loot );
					rare = true;
				}
				else if ( ( m_RareChance + useskills ) / 100.0 > Utility.RandomDouble() )
				{
					m_Loot = Loot.Construct(m_TrashVeryRareTypes);
					m_Loot.Movable = true;
					from.AddToBackpack(m_Loot);
					loots.Add( m_Loot );
					rare = true;
				}

				if ( m_Sound > 0 )
					Effects.PlaySound(from.Location, from.Map, m_Sound);

				for ( int i = 0;i < loots.Count; i++ )
				{
					if ( i == 0 ) //First
						m_Collected += GetName( loots[i] );
					else if ( i == loots.Count-1 ) //Last
					{
						m_Collected += String.Format( ", and {0}{1}", GetName( loots[i] ), rare ? "!" : "." );

					}
					else
						m_Collected += String.Format( ", {0}", GetName( loots[i] ) );
				}

				from.SendMessage(m_Collected);

				Looters.Add(new TrashLooter(from));
				m_usesCount++;

				if (m_MaxUses > 0 && m_usesCount >= m_MaxUses)
				{
					from.SendMessage("You have recovered the last useable item!");
					this.Delete();
				}

				if (m_MakesCriminal)
				{
					from.CriminalAction(false);
					from.SendLocalizedMessage(1010630); // Taking someone else's treasure is a criminal offense!
				}
			}
			else
				from.SendMessage("You find nothing of value at this time.");
		}

		public bool FindLooter(Mobile from)
		{
			bool rtn = false;

			if (Looters.Count == 0)
				return rtn;

			foreach (TrashLooter looter in Looters)
			{
				if (looter.Looter == from)
				{
					rtn = true;
					break;
				}
			}
			return rtn;
		}

		public void DefragLooters()
		{
			if (Looters.Count == 0)
				return;

			for (int i = 0; i < Looters.Count; i++)
			{
				try
				{
					TrashLooter looter = Looters[i];

					if ( looter.Time + m_LootWait < DateTime.UtcNow || looter.Looter == null )
						Looters.RemoveAt(i--);
				}
				catch (Exception e)
				{
					Console.WriteLine("/nException Caught in TrashPiles Defrag: /n{0}/n", e);
				}
			}
		}

		public BaseTrash(Serial serial)	: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)1);

			writer.Write(m_MakesCriminal);
			writer.Write(m_Sound);
			writer.Write(m_LootWait);
			writer.Write(m_ItemChance);
			writer.Write(m_RareChance);
			writer.Write(m_MaxUses);
			writer.Write(m_usesCount);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					m_MakesCriminal = reader.ReadBool();
					m_Sound = reader.ReadInt();
					m_LootWait = reader.ReadTimeSpan();
					m_ItemChance = reader.ReadDouble();
					m_RareChance = reader.ReadDouble();
					m_MaxUses = reader.ReadInt();
					m_usesCount = reader.ReadInt();
					break;
			}
		}
		#endregion
	}

	#region TrashLooter struct
	public struct TrashLooter
	{
		public TrashLooter(Mobile from)
		{
			Looter = from;
			Time = DateTime.UtcNow;
		}

		public Mobile Looter;
		public DateTime Time;
	}
	#endregion

	#region TrashComponent class
	public class TrashComponent : AddonComponent
	{
		// Note: will crash if used in Addon other than BaseTrash.
		[CommandProperty(AccessLevel.GameMaster)]
		public virtual TimeSpan LootWait
		{
			get { return ((BaseTrash)base.Addon).LootWait; }
			set { ((BaseTrash)base.Addon).LootWait = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual double ItemChance
		{
			get { return ((BaseTrash)base.Addon).ItemChance; }
			set { ((BaseTrash)base.Addon).ItemChance = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual double RareChance
		{
			get { return ((BaseTrash)base.Addon).RareChance; }
			set { ((BaseTrash)base.Addon).RareChance = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int MaxUses
		{
			get { return ((BaseTrash)base.Addon).MaxUses; }
			set { ((BaseTrash)base.Addon).MaxUses = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int UsesCount
		{
			get { return ((BaseTrash)base.Addon).UsesCount; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Sound
		{
			get { return ((BaseTrash)base.Addon).Sound; }
			set { ((BaseTrash)base.Addon).Sound = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool MakesCriminal
		{
			get { return ((BaseTrash)base.Addon).MakesCriminal; }
			set { ((BaseTrash)base.Addon).MakesCriminal = value; }
		}

		public TrashComponent(int itemID) : base(itemID)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.InRange(this, 2))
			{
				if (base.Addon is BaseTrash)
					((BaseTrash)base.Addon).OnLoot(from);
			}
		}

		public TrashComponent(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	#endregion

	#region TrashPileSmall class
	public class TrashPileSmall : BaseTrash
	{
		[Constructable]
		public TrashPileSmall()
		{
			AddComponent(new TrashComponent(0x1ba1), 0, 1, 0);
			AddComponent(new TrashComponent(0x1ba5), 0, 0, 0);
			AddComponent(new TrashComponent(0x1bb1), 1, 1, 0);
			AddComponent(new TrashComponent(0x1bb0), 1, 0, 0);
		}

		public TrashPileSmall(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	#endregion

	#region TrashPileMedium class
	public class TrashPileMedium : BaseTrash
	{
		[Constructable]
		public TrashPileMedium()
		{
			AddComponent(new TrashComponent(0x1bb5), 0, 1, 0);
			AddComponent(new TrashComponent(0x1bb6), 0, 0, 0);
			AddComponent(new TrashComponent(0x1bb7), 0, -1, 0);
			AddComponent(new TrashComponent(0x1bbc), 1, 0, 0);
			AddComponent(new TrashComponent(0x1bbb), 1, -1, 0);
		}

		public TrashPileMedium(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	#endregion

	#region TrashPileLarge class
	public class TrashPileLarge : BaseTrash
	{
		[Constructable]
		public TrashPileLarge()
		{
			AddComponent(new TrashComponent(0x1bb5), 0, 2, 0);
			AddComponent(new TrashComponent(0x1ba6), 0, 1, 0);
			AddComponent(new TrashComponent(0x1ba7), 0, 0, 0);
			AddComponent(new TrashComponent(0x1ba8), 0, -1, 0);
			AddComponent(new TrashComponent(0x1ba9), 0, -2, 0);
			AddComponent(new TrashComponent(0x1ba2), -1, 2, 0);
			AddComponent(new TrashComponent(0x1ba3), -1, 1, 0);
			AddComponent(new TrashComponent(0x1ba4), -1, 0, 0);
			AddComponent(new TrashComponent(0x1ba5), -1, -1, 0);
			AddComponent(new TrashComponent(0x1bb1), 1, 1, 0);
			AddComponent(new TrashComponent(0x1bb0), 1, 0, 0);
			AddComponent(new TrashComponent(0x1baf), 1, -1, 0);
		}

		public TrashPileLarge(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	#endregion

	#region TrashPileXLarge
	public class TrashPileXLarge : BaseTrash
	{
		[Constructable]
		public TrashPileXLarge()
		{
			AddComponent(new TrashComponent(0x1bb2), 0, 0, 0);
			AddComponent(new TrashComponent(0x1ba2), -2, 1, 0);
			AddComponent(new TrashComponent(0x1ba3), -2, 0, 0);
			AddComponent(new TrashComponent(0x1ba4), -2, -1, 0);
			AddComponent(new TrashComponent(0x1ba5), -2, -2, 0);
			AddComponent(new TrashComponent(0x1bb5), -1, 1, 0);
			AddComponent(new TrashComponent(0x1bb8), -1, -2, 0);
			AddComponent(new TrashComponent(0x1bb9), -1, -3, 0);
			AddComponent(new TrashComponent(0x1bb4), 0, 1, 0);
			AddComponent(new TrashComponent(0x1baa), 0, -2, 0);
			AddComponent(new TrashComponent(0x1bbc), 1, 0, 0);
			AddComponent(new TrashComponent(0x1bbb), 1, -1, 0);
			AddComponent(new TrashComponent(0x1bba), 1, -2, 0);
		}

		public TrashPileXLarge(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
	#endregion
}