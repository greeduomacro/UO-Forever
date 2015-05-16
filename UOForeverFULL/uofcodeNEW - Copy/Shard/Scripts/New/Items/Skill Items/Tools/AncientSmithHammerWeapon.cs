using System;
using Server;
using Server.Engines.Craft;
using Server.Misc;

namespace Server.Items
{
	public class AncientSmithHammerWeapon : SmithHammerWeapon
	{
		private int m_Bonus;
		private SkillMod m_SkillMod;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Bonus
		{
			get
			{
				return m_Bonus;
			}
			set
			{
				m_Bonus = value;
				InvalidateProperties();

				if ( m_Bonus == 0 )
				{
					if ( m_SkillMod != null )
						m_SkillMod.Remove();

					m_SkillMod = null;
				}
				else if ( m_SkillMod == null && Parent is Mobile )
				{
					m_SkillMod = new DefaultSkillMod( SkillName.Blacksmith, true, m_Bonus );
					( (Mobile) Parent ).AddSkillMod( m_SkillMod );
				}
				else if ( m_SkillMod != null )
					m_SkillMod.Value = m_Bonus;
			}
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( m_Bonus != 0 && parent is Mobile )
			{
				if ( m_SkillMod != null )
					m_SkillMod.Remove();

				m_SkillMod = new DefaultSkillMod( SkillName.Blacksmith, true, m_Bonus );
				((Mobile)parent).AddSkillMod( m_SkillMod );
			}
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( m_SkillMod != null )
				m_SkillMod.Remove();

			m_SkillMod = null;
		}

		public override int LabelNumber { get { return 1045127; } } // ancient smithy hammer

		[Constructable]
		public AncientSmithHammerWeapon( int bonus ) : this( bonus, 600 )
		{
		}

		[Constructable]
		public AncientSmithHammerWeapon( int bonus, int uses ) : base( uses )
		{
			Identified = true;
			m_Bonus = bonus;
			Hue = 0x482;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Bonus != 0 )
				list.Add( 1060451, "#1042354\t{0}", m_Bonus.ToString() ); // ~1_skillname~ +~2_val~
		}

		public override string DefaultName{ get{ return String.Format( "+{0} ancient smithy hammer", m_Bonus ); } }

		public AncientSmithHammerWeapon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Bonus );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
					{
						m_Bonus = reader.ReadInt();
						break;
					}
			}

			if ( m_Bonus != 0 && Parent is Mobile )
			{
				if ( m_SkillMod != null )
					m_SkillMod.Remove();

				m_SkillMod = new DefaultSkillMod( SkillName.Blacksmith, true, m_Bonus );
				((Mobile)Parent).AddSkillMod( m_SkillMod );
			}

			if ( Hue == 0 )
				Hue = 0x482;
		}
	}
}