using System;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Items
{
	[Flags]
	public enum WrestlingSkillScrollFlags
	{
		None	= 0x00,
		Newbie	= 0x01,
		Expires	= 0x02
	}

	public class WrestlingSkillScroll : Item
	{
		private int m_SkillBonus;
		private WrestlingSkillScrollFlags m_Flags;
		private DateTime m_ExpireDate;

		public WrestlingSkillScrollFlags Flags{ get{ return m_Flags; } set{ m_Flags = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime ExpireDate{ get{ return m_ExpireDate; } set{ m_ExpireDate = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int SkillBonus{ get{ return m_SkillBonus; } set{ m_SkillBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool NewbieBall{ get{ return GetFlag( WrestlingSkillScrollFlags.Newbie ); } set{ SetFlag( WrestlingSkillScrollFlags.Newbie, value ); UpdateHue(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Expires{ get{ return GetFlag( WrestlingSkillScrollFlags.Expires ); } set{ SetFlag( WrestlingSkillScrollFlags.Expires, value ); InvalidateProperties(); } }

		public bool GetFlag( WrestlingSkillScrollFlags flag )
		{
			return ( m_Flags & flag ) != 0;
		}

		public void SetFlag( WrestlingSkillScrollFlags flag, bool value )
		{
			if ( value )
				m_Flags |= flag;
			else
				m_Flags &= ~flag;
		}

		[Constructable]
		public WrestlingSkillScroll( int bonus, bool newbied, int days ) : base( 8800 )
		{
			m_SkillBonus = bonus;
			NewbieBall = newbied;
			if ( days > -1 )
			{
				ExpireDate = DateTime.UtcNow.AddDays( days );
				Expires = true;
			}
			UpdateName();
		}

		[Constructable]
		public WrestlingSkillScroll( int bonus, bool newbied ) : this( bonus, false, -1 )
		{
		}

		[Constructable]
		public WrestlingSkillScroll( int bonus ) : this( bonus, false )
		{
		}

		[Constructable]
		public WrestlingSkillScroll() : this( 1 )
		{
            Weight = 1;
		}

		public WrestlingSkillScroll( Serial serial ) : base( serial )
		{
		}

		public override bool Nontransferable{ get { return false; } }

		public override string DefaultName{ get{ return "a Wrestling Skill Scroll"; } }
		public override int LabelNumber{ get{ return 0; } }
		public override bool DisplayLootType{ get{ return false; } }
		public virtual bool Rechargable{ get{ return false; } }

		public virtual void UpdateHue()
		{
			Hue = NewbieBall ? 33 : 1161;
		}

		public virtual void UpdateName()
		{
			if ( Expires )
				Name = "an unstable skill ball";
			else
				Name = "a +1 Wrestling skill scroll";
		}

		public virtual string GetLabelName()
		{
			if ( Expires )
				return "a +{0} unstable skill ball";
			return "a +{0} Wrestling Skill Scroll";
		}

		public virtual SkillName[] GetAllowedSkills()
		{
			if (EraML)
				return m_MLSkills;
			else if (EraSE)
				return m_SESkills;
			else if (EraAOS)
				return m_AOSSkills;
			else
				return m_OldSkills;
		}

		private static readonly SkillName[] m_OldSkills = new SkillName[]
			{
	    SkillName.Wrestling
			};

		private static readonly SkillName[] m_AOSSkills = new SkillName[]
			{
				
				SkillName.Wrestling
			};

		private static readonly SkillName[] m_SESkills = new SkillName[]
			{
				SkillName.Wrestling

			};

		private static readonly SkillName[] m_MLSkills = new SkillName[]
			{
				SkillName.Wrestling
			
			};

		public override bool CheckNewbied( Mobile from )
		{
			return base.CheckNewbied( from ) || NewbieBall;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060658, "bonus\t{0}", m_SkillBonus );

			if ( Expires )
			{
				if ( DateTime.UtcNow < m_ExpireDate )
				{
					int hours = (int)(m_ExpireDate - DateTime.UtcNow).TotalHours;
					if ( hours > 24 )
						list.Add( 1150814, "{0}\t{1}", hours / 24, hours % 24 );
					else if ( hours > 0 )
						list.Add( 1150813, "{0}", hours );
					else
						list.Add( 1150829 );
				}
				else
					list.Add( 1150487 );
			}
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Deleted || !from.CanSee( this ) )
				return;

			LabelToExpansion(from);

			LabelTo( from, String.Format( GetLabelName(), m_SkillBonus ) );

			if ( Expires )
			{
				if ( DateTime.UtcNow < m_ExpireDate )
				{
					int hours = (int)(m_ExpireDate - DateTime.UtcNow).TotalHours;
					if ( hours > 24 )
						LabelTo( from, String.Format( "Remaining Time: about {0} day(s) {1} hour(s)", hours / 24, hours % 24 ) );
					else if ( hours > 0 )
						LabelTo( from, String.Format( "Remaining Time: about {0} hour(s)", hours ) );
					else
						LabelTo( from, "Remaining Time: within 1 hour" );
				}
				else
					LabelTo( from, 1150487 );
			}
		}

		public virtual void SendGump( Mobile from )
		{
			from.SendGump( new WrestlingSkillScrollGump( from, from, this ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( SkillBonus <= 0 || ( Expires && DateTime.UtcNow >= m_ExpireDate ) )
			{
				if ( from.AccessLevel >= AccessLevel.GameMaster )
					from.SendGump( new PropertiesGump( from, this ) );
				SendLocalizedMessageTo( from, 1042544 ); // This item is out of charges.
			}
			else if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( !from.HasGump( typeof(WrestlingSkillScroll) ) )
				SendGump( from );
			else
				from.SendMessage( "You are already using a skill ball." );
		}

		public virtual List<Skill> GetDecreasableSkills( Mobile from, Mobile target, double count, double cap, out double decreaseamount )
		{
			Skills skills = from.Skills;
			decreaseamount = 0.0;

			List<Skill> decreased = new List<Skill>();
			double bonus = m_SkillBonus;

			if ( (count + bonus) > cap )
			{
				for ( int i = 0; i < skills.Length; i++ )
				{
					if ( skills[i].Lock == SkillLock.Down && skills[i].Base > 0.0 )
					{
						decreased.Add( skills[i] );
						decreaseamount += skills[i].Base;
					}
				}
			}

			return decreased;
		}

		public virtual void DecreaseSkills( Mobile from, Mobile target, List<Skill> decreased, double count, double cap, double decreaseamount )
		{
			Skills skills = from.Skills;

			double freepool = cap - count;
			double bonus = m_SkillBonus;

			if ( freepool < bonus )
			{
				bonus -= freepool;

				foreach( Skill s in decreased )
				{
					if ( s.Base >= bonus )
					{
						s.Base -= bonus;
						bonus = 0;
					}
					else
					{
						bonus -= s.Base;
						s.Base = 0;
					}

					if ( bonus == 0 )
						break;
				}
			}
		}

		public virtual void IncreaseSkill( Mobile from, Mobile target, Skill skill )
		{
			skill.Base += m_SkillBonus;
			SendConfirmMessage( from, target, skill );
			m_SkillBonus = 0;
		}

		public virtual void SendConfirmMessage( Mobile from, Mobile target, Skill skill )
		{
			from.SendMessage( "Your skill in {0} has been raised by {1}", skill.Name, m_SkillBonus );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)1 ); // version

			writer.WriteEncodedInt( (int)m_Flags );
			writer.Write( m_ExpireDate );

			writer.Write( m_SkillBonus );

			//We save it just in case, then delete it afterwards
			if ( Expires && DateTime.UtcNow >= m_ExpireDate )
				AddToCleanup( this );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
				{
					m_Flags = (WrestlingSkillScrollFlags)reader.ReadEncodedInt();
					m_ExpireDate = reader.ReadDateTime();
					goto case 0;
				}
				case 0:
				{
					m_SkillBonus = reader.ReadInt();
					break;
				}
			}

			//We save it just in case, then delete it afterwards
			if ( Expires && DateTime.UtcNow >= m_ExpireDate )
				AddToCleanup( this );
            SkillScroll.ConvertSkillScroll(this, m_OldSkills[0]);
		}

		public static void Configure()
		{
			m_Cleanup = new List<Item>();
			EventSink.WorldLoad += new WorldLoadEventHandler( PurgeList );
			EventSink.WorldSave += new WorldSaveEventHandler( Save );
		}

		public static List<Item> m_Cleanup;

		public static void PurgeList()
		{
			if ( m_Cleanup != null && m_Cleanup.Count > 0 )
				for ( int i = 0; i < m_Cleanup.Count; ++i )
					m_Cleanup[i].Delete();

			m_Cleanup.Clear();
		}

		public static void Save( WorldSaveEventArgs e )
		{
			Timer.DelayCall( TimeSpan.Zero, new TimerCallback( PurgeList ) );
		}

		public static void AddToCleanup( Item item )
		{
			if ( m_Cleanup == null )
				m_Cleanup = new List<Item>();

			m_Cleanup.Add( item );
		}

	}
}

namespace Server.Gumps
{
	public class WrestlingSkillScrollGump : Gump
	{
		private const int FieldsPerPage = 1;

		private Skill m_Skill;
		private WrestlingSkillScroll m_WrestlingSkillScroll;
		private Mobile m_Target;

		public WrestlingSkillScrollGump ( Mobile from, Mobile target, WrestlingSkillScroll ball ) : base ( 20, 20 )
		{
			m_WrestlingSkillScroll = ball;
			m_Target = target;

			AddPage ( 0 );
			AddBackground( 0, 0, 260, 351, 5054 );

			AddImageTiled( 10, 10, 240, 23, 0x52 );
			AddImageTiled( 11, 11, 238, 21, 0xBBC );

			AddLabel( 45, 11, 0, "Forever Skill Scroll" );

			AddPage( 1 );

			int page = 1;
			int index = 0;

			Skills skills = m_Target.Skills;
			SkillName[] allowedskills = m_WrestlingSkillScroll.GetAllowedSkills();

			for ( int i = 0; i < allowedskills.Length; ++i )
			{
				if ( index >= FieldsPerPage )
				{
					AddButton( 231, 13, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1 );

					++page;
					index = 0;

					AddPage( page );

					AddButton( 213, 13, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1 );
				}

				Skill skill = skills[allowedskills[i]];

				if ( (!m_WrestlingSkillScroll.NewbieBall || skill.Base == 0) && (skill.Base + m_WrestlingSkillScroll.SkillBonus) <= skill.Cap && skill.Lock == SkillLock.Up )
				{
					AddImageTiled( 10, 32 + (index * 22), 240, 23, 0x52 );
					AddImageTiled( 11, 33 + (index * 22), 238, 21, 0xBBC );

					AddLabelCropped( 13, 33 + (index * 22), 150, 21, 0, skill.Name );
					AddImageTiled( 180, 34 + (index * 22), 50, 19, 0x52 );
					AddImageTiled( 181, 35 + (index * 22), 48, 17, 0xBBC );
					AddLabelCropped( 182, 35 + (index * 22), 234, 21, 0, skill.Base.ToString( "F1" ) );

					AddButton( 231, 35 + (index * 22), 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0 );

					++index;
				}
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( from == null || m_WrestlingSkillScroll.Deleted )
				return;

			if ( m_WrestlingSkillScroll.Expires && DateTime.UtcNow >= m_WrestlingSkillScroll.ExpireDate )
				m_WrestlingSkillScroll.SendLocalizedMessageTo( from, 1042544 ); // This item is out of charges.
			else if ( !m_WrestlingSkillScroll.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if (info.ButtonID == 1)
			{
				SkillName skillname = (m_WrestlingSkillScroll.GetAllowedSkills())[info.ButtonID-1];
				m_Skill = m_Target.Skills[skillname];

				if ( m_Skill == null )
					return;

				double count = m_Target.Skills.Total / 10;
				double cap = m_Target.SkillsCap / 10;
				double decreaseamount;
				int bonus = m_WrestlingSkillScroll.SkillBonus;

				List<Skill> decreased = m_WrestlingSkillScroll.GetDecreasableSkills( from, m_Target, count, cap, out decreaseamount );

				if ( decreased.Count > 0 && decreaseamount <= 0 )
					from.SendMessage( "You have exceeded the skill cap and do not have a skill set to be decreased." );
				else if ( m_WrestlingSkillScroll.NewbieBall && m_Skill.Base > 0 )
					from.SendMessage( "You may only choose skills which are at zero." );
				else if ( (m_Skill.Base + bonus) > m_Skill.Cap )
					from.SendMessage( "Your skill is too high to raise it further." );
				else if ( m_Skill.Lock != SkillLock.Up )
					from.SendMessage( "You must set the skill to be increased in order to raise it further." );
				else
				{
					if ( ( cap - count + decreaseamount ) >= bonus )
					{
						m_WrestlingSkillScroll.DecreaseSkills( from, m_Target, decreased, count, cap, decreaseamount );
						m_WrestlingSkillScroll.IncreaseSkill( from, m_Target, m_Skill );

						if ( !m_WrestlingSkillScroll.Rechargable )
							m_WrestlingSkillScroll.Delete();
					}
					else
						from.SendMessage( "You have exceeded the skill cap and do not have enough skill set to be decreased." );
				}
			}
		}
	}
}