using System;
using Server;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Regions;

namespace Server.Items
{
	[Flags]
	public enum CraftSkillType
	{
		None		= 0x00,
		Smithing	= 0x01,
		Tailoring	= 0x02,
		Tinkering	= 0x04,
		Carpentry	= 0x08,
		Fletching	= 0x10
	}

	public class RepairDeed : Item
	{
		private class RepairSkillInfo
		{
			private CraftSystem m_System;
			//private Type[] m_NearbyTypes;
			private TextDefinition m_NotNearbyMessage, m_Name;
			private CraftSkillType m_Skill;

			public TextDefinition NotNearbyMessage{	get { return m_NotNearbyMessage; } }
			public TextDefinition Name { get { return m_Name; } }

			public CraftSystem System { get { return m_System; } }
			//public Type[] NearbyTypes { get { return m_NearbyTypes; } }

			public CraftSkillType RepairSkill{ get { return m_Skill; } }


			public RepairSkillInfo( CraftSystem system, Type[] nearbyTypes, TextDefinition notNearbyMessage, TextDefinition name, CraftSkillType skilltype )
			{
				m_System = system;
				//m_NearbyTypes = nearbyTypes;
				m_NotNearbyMessage = notNearbyMessage;
				m_Name = name;
				m_Skill = skilltype;
			}

			public RepairSkillInfo( CraftSystem system, Type nearbyType, TextDefinition notNearbyMessage, TextDefinition name, CraftSkillType skilltype )
				: this( system, new Type[] { nearbyType }, notNearbyMessage, name, skilltype )
			{
			}

			public static RepairSkillInfo[] Table { get { return m_Table; } }
			private static RepairSkillInfo[] m_Table = new RepairSkillInfo[]
				{
					new RepairSkillInfo( DefBlacksmithy.CraftSystem, typeof( Blacksmith ), 1047013, 1023015, CraftSkillType.Smithing ),
					new RepairSkillInfo( DefTailoring.CraftSystem, typeof( Tailor ), 1061132, 1022981, CraftSkillType.Tailoring ),
					new RepairSkillInfo( DefTinkering.CraftSystem, typeof( Tinker ), 1061166, 1022983, CraftSkillType.Tinkering ),
					new RepairSkillInfo( DefCarpentry.CraftSystem, typeof( Carpenter ), 1061135, 1060774, CraftSkillType.Carpentry ),
					new RepairSkillInfo( DefBowFletching.CraftSystem, typeof( Bowyer ), 1061134, 1023005, CraftSkillType.Fletching )
				};

			public static RepairSkillInfo GetInfo( CraftSkillType type )
			{
				for( int i = 0; i < m_Table.Length; i++ )
					if( m_Table[i].RepairSkill == type )
						return m_Table[i];

				return m_Table[0];
			}
		}

		public override bool DisplayLootType { get { return false; } }

		private CraftSkillType m_Skill;
		private double m_SkillLevel;

		private Mobile m_Crafter;

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftSkillType RepairSkill
		{
			get { return m_Skill; }
			set { m_Skill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double SkillLevel
		{
			get { return m_SkillLevel; }
			set { m_SkillLevel = Math.Max( Math.Min( value, 120.0 ), 0 ) ; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get { return m_Crafter; }
			set { m_Crafter = value; InvalidateProperties(); }
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			string message = String.Empty;

			if ( m_Crafter != null )
				message = String.Format( "{0}, {1}\t{2}", m_Crafter.RawName, GetSkillTitle( m_SkillLevel ).ToString(), RepairSkillInfo.GetInfo( m_Skill ).Name );
			else
				message = String.Format( "{0}\t{1}", GetSkillTitle( m_SkillLevel ).ToString(), RepairSkillInfo.GetInfo( m_Skill ).Name );

			list.Add( 1061133, message ); // A repair service contract from ~1_SKILL_TITLE~ ~2_SKILL_NAME~.
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if( m_Crafter != null )
				list.Add( 1050043, m_Crafter.RawName ); // crafted by ~1_NAME~

			//On OSI it says it's exceptional.  Intentional difference.
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelToExpansion(from);

			string message = String.Empty;

			if ( m_Crafter != null )
				message = String.Format( "{0}, {1}\t{2}", m_Crafter.RawName, GetSkillTitle( m_SkillLevel ).ToString(), RepairSkillInfo.GetInfo( m_Skill ).Name );
			else
				message = String.Format( "{0}\t{1}", GetSkillTitle( m_SkillLevel ).ToString(), RepairSkillInfo.GetInfo( m_Skill ).Name );

			LabelTo( from, 1061133, message );
		}

		[Constructable]
		public RepairDeed() : this( CraftSkillType.Smithing, 100.0, null, true )
		{
		}

		[Constructable]
		public RepairDeed( CraftSkillType skill, double level ) : this( skill, level, null, true )
		{
		}

		[Constructable]
		public RepairDeed( CraftSkillType skill, double level, bool normalizeLevel ) : this( skill, level, null, normalizeLevel )
		{
		}

		public RepairDeed( CraftSkillType skill, double level, Mobile crafter ) : this( skill, level, crafter, true )
		{
		}

		public RepairDeed( CraftSkillType skill, double level, Mobile crafter, bool normalizeLevel ) : base( 0x14F0 )
		{
			if( normalizeLevel )
				SkillLevel = (int)(level/10)*10;
			else
				SkillLevel = level;

			m_Skill = skill;
			m_Crafter = crafter;
			Hue = 0x1BC;
			LootType = LootType.Blessed;
		}

		public RepairDeed( Serial serial ) : base( serial )
		{
		}

		private static TextDefinition GetSkillTitle( double skillLevel )
		{
			int skill = (int)(skillLevel/10);
/*
			if( skill >= 11 )
				return (1062008 + skill-11);
			else if( skill >= 5 )
				return (1061123 + skill-5);
*/
			switch( skill )
			{
				case 12:
					return "a Legendary";
				case 11:
					return "an Elder";
				case 10:
					return "a Grandmaster";
				case 9:
					return "a Master";
				case 8:
					return "an Adept";
				case 7:
					return "an Expert";
				case 6:
					return "a Journeyman";
				case 5:
					return "an Apprentice";
				case 4:
					return "a Novice";
				case 3:
					return "a Neophyte";
				default:
					return "a Newbie";		//On OSI, it shouldn't go below 50, but, this is for 'custom' support.
			}
		}

		public static CraftSkillType GetTypeFor( CraftSystem s )
		{
			for( int i = 0; i < RepairSkillInfo.Table.Length; i++ )
				if( RepairSkillInfo.Table[i].System == s )
					return RepairSkillInfo.Table[i].RepairSkill;

			return CraftSkillType.Smithing;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( Check( from ) )
				Repair.Do( from, RepairSkillInfo.GetInfo( m_Skill ).System, this );
		}

		public bool Check( Mobile from )
		{
			if( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1047012 ); // The contract must be in your backpack to use it.
			else if( !VerifyRegion( from ) )
				TextDefinition.SendMessageTo( from, RepairSkillInfo.GetInfo( m_Skill ).NotNearbyMessage );
			else
				return true;

			return false;
		}

		public bool VerifyRegion( Mobile m )
		{
			CraftShopRegion region = m.Region.GetRegion( typeof( CraftShopRegion ) ) as CraftShopRegion;
			return region != null && region.HasSkill( m_Skill );

			//TODO: When the entire region system data is in, convert to that instead of a proximity thing.
/*
			if( !m.Region.IsPartOf( typeof( TownRegion ) ) )
				return false;

			return Server.Factions.Faction.IsNearType( m, RepairSkillInfo.GetInfo( m_Skill ).NearbyTypes, 6 );
*/
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version

			writer.Write( (int)m_Skill );
			writer.Write( m_SkillLevel );
			writer.Write( m_Crafter );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch( version )
			{
				case 0:
				{
					m_Skill = (CraftSkillType)reader.ReadInt();
					m_SkillLevel = reader.ReadDouble();
					m_Crafter = reader.ReadMobile();

					break;
				}
			}
		}
	}
}