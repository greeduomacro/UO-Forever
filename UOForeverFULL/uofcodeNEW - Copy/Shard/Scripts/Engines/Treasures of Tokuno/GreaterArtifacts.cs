using Server;
using System;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	public class DarkenedSky : Kama
	{
		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		public override int LabelNumber { get { return 1070966; } } // Darkened Sky

		[Constructable]
		public DarkenedSky() : base()
		{
		}

		public DarkenedSky( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}

	public class KasaOfTheRajin : Kasa
	{
		public override int LabelNumber { get { return 1070969; } } // Kasa of the Raj-in

		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		[Constructable]
		public KasaOfTheRajin() : base()
		{
		}

		public KasaOfTheRajin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)2 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version <= 1 )
			{
				MaxHitPoints = 255;
				HitPoints = 255;
			}

			if( version == 0 )
				LootType = LootType.Regular;
		}

	}

	public class RuneBeetleCarapace : PlateDo
	{
		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		public override int LabelNumber{ get{ return 1070968; } } // Rune Beetle Carapace

		[Constructable]
		public RuneBeetleCarapace() : base()
		{
		}

		public RuneBeetleCarapace( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}

	public class Stormgrip : LeatherNinjaMitts
	{
		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		public override int LabelNumber{ get{ return 1070970; } } // Stormgrip

		[Constructable]
		public Stormgrip() : base()
		{
		}

		public Stormgrip( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}

	public class SwordOfTheStampede : NoDachi
	{
		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		public override int LabelNumber { get { return 1070964; } } // Sword of the Stampede

		[Constructable]
		public SwordOfTheStampede() : base()
		{
		}

		public SwordOfTheStampede( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}

	public class SwordsOfProsperity : Daisho
	{
		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		public override int LabelNumber { get { return 1070963; } } // Swords of Prosperity

		[Constructable]
		public SwordsOfProsperity() : base()
		{
		}

		public SwordsOfProsperity( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

	}

	public class TheHorselord : Yumi
	{
		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }

		public override int LabelNumber { get { return 1070967; } } // The Horselord

		[Constructable]
		public TheHorselord() : base()
		{
			Slayer = SlayerName.ElementalBan;
			Slayer2 = SlayerName.ReptilianDeath;
		}

		public TheHorselord( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class TomeOfLostKnowledge : Spellbook
	{
		public override int LabelNumber { get { return 1070971; } } // Tome of Lost Knowledge

		[Constructable]
		public TomeOfLostKnowledge() : base()
		{
			LootType = LootType.Regular;
			Hue = 0x530;
		}

		public TomeOfLostKnowledge( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class WindsEdge : Tessen
	{
		public override int LabelNumber { get { return 1070965; } } // Wind's Edge

		[Constructable]
		public WindsEdge() : base()
		{
		}

		public WindsEdge( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override int InitMinHits { get { return 255; } }
		public override int InitMaxHits { get { return 255; } }
	}

	public enum PigmentType
	{
		None,
		ParagonGold,
		VioletCouragePurple,
		InvulnerabilityBlue,
		LunaWhite,
		DryadGreen,
		ShadowDancerBlack,
		BerserkerRed,
		NoxGreen,
		RumRed,
		FireOrange,
		FadedCoal,
		Coal,
		FadedGold,
		StormBronze,
		Rose,
		MidnightCoal,
		FadedBronze,
		FadedRose,
		DeepRose
	}

	public class PigmentsOfTokuno : BasePigmentsOfTokuno
	{
		public class PigmentInfo
		{
			//private PigmentType m_PigmentType;
			private int m_Hue;
			private TextDefinition m_Label;

			public int Hue { get { return m_Hue; } }
			public TextDefinition Label { get { return m_Label; } }

			public PigmentInfo( int hue, TextDefinition label )
			{
				m_Hue = hue;
				m_Label = label;
			}

			private static PigmentInfo[] m_Table = new PigmentInfo[]
				{
					new PigmentInfo( /*PigmentType.None,*/ 0, -1 ),
					new PigmentInfo( /*PigmentType.ParagonGold,*/ 0x501, 1070987 ),
					new PigmentInfo( /*PigmentType.VioletCouragePurple,*/ 0x486, 1070988 ),
					new PigmentInfo( /*PigmentType.InvulnerabilityBlue,*/ 0x4F2, 1070989 ),
					new PigmentInfo( /*PigmentType.LunaWhite,*/ 0x47E, 1070990 ),
					new PigmentInfo( /*PigmentType.DryadGreen,*/ 0x48F, 1070991 ),
					new PigmentInfo( /*PigmentType.ShadowDancerBlack,*/ 0x455, 1070992 ),
					new PigmentInfo( /*PigmentType.BerserkerRed,*/ 0x21, 1070993 ),
					new PigmentInfo( /*PigmentType.NoxGreen,*/ 0x58C, 1070994 ),
					new PigmentInfo( /*PigmentType.RumRed,*/ 0x66C, 1070995 ),
					new PigmentInfo( /*PigmentType.FireOrange,*/ 0x54F, 1070996 ),
					new PigmentInfo( /*PigmentType.Fadedcoal,*/ 0x96A, 1079579 ),
					new PigmentInfo( /*PigmentType.Coal,*/ 0x96B, 1079580 ),
					new PigmentInfo( /*PigmentType.FadedGold,*/ 0x972, 1079581 ),
					new PigmentInfo( /*PigmentType.StormBronze,*/ 0x977, 1079582 ),
					new PigmentInfo( /*PigmentType.Rose,*/ 0x97C, 1079583 ),
					new PigmentInfo( /*PigmentType.MidnightCoal,*/ 0x96C, 1079584 ),
					new PigmentInfo( /*PigmentType.FadedBronze,*/ 0x975, 1079585 ),
					new PigmentInfo( /*PigmentType.FadedRose,*/ 0x97B, 1079586 ),
					new PigmentInfo( /*PigmentType.DeepRose,*/ 0x97E, 1079587 )
				};

			public static PigmentType GetRandomPigment()
			{
				return GetRandomPigment( false );
			}

			public static PigmentType GetRandomPigment( bool lesser )
			{
				if (lesser)
					return (PigmentType)Utility.Random( m_Table.Length );
				else return (PigmentType)Utility.RandomMinMax(1, m_Table.Length - 1);
			}

			public static PigmentInfo GetInfo( PigmentType type )
			{
				int v = (int)type;

				if( v < 0 || v >= m_Table.Length )
					v = 0;

				return m_Table[v];
			}
		}

		private PigmentType m_Type;

		[CommandProperty( AccessLevel.GameMaster )]
		public PigmentType Type
		{
			get { return m_Type; }
			set
			{
				m_Type = value;

				PigmentInfo p = PigmentInfo.GetInfo( m_Type );
				Hue = p.Hue;
				InvalidateProperties();
			}
		}

		public override int LabelNumber { get { return 1070933; } } // Pigments of Tokuno

		[Constructable]
		public PigmentsOfTokuno() : this( PigmentType.None, 10 )
		{
		}

		[Constructable]
		public PigmentsOfTokuno( PigmentType type ) : this( type, (type == PigmentType.None||type >= PigmentType.FadedCoal)? 10 : 50 )
		{
		}

		[Constructable]
		public PigmentsOfTokuno( bool randomwithlesser, int uses ) : this( PigmentInfo.GetRandomPigment( randomwithlesser ), uses )
		{
		}

		[Constructable]
		public PigmentsOfTokuno( PigmentType type, int uses ) : base( uses )
		{
			Weight = 1.0;
			Type = type;
		}

		public PigmentsOfTokuno( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 );

			writer.WriteEncodedInt( (int)m_Type );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = ( InheritsItem ? 0 : reader.ReadInt() ); // Required for BasePigmentsOfTokuno insertion

			switch ( version )
			{
				case 1: Type = (PigmentType)reader.ReadEncodedInt(); break;
				case 0: break;
			}
		}
	}
}