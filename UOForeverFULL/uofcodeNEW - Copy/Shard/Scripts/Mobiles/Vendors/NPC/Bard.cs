using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Bard : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.BardsGuild; } }
		private static readonly TimeSpan m_MusicDelay = TimeSpan.FromSeconds( 60.0 );

		private BaseInstrument m_Instrument;
		private DateTime m_NextMusic;

		[Constructable]
		public Bard() : base( "the bard" )
		{
			SetSkill( SkillName.Discordance, 64.0, 100.0 );
			SetSkill( SkillName.Musicianship, 64.0, 100.0 );
			SetSkill( SkillName.Peacemaking, 65.0, 88.0 );
			SetSkill( SkillName.Provocation, 60.0, 83.0 );
			SetSkill( SkillName.Archery, 36.0, 68.0 );
			SetSkill( SkillName.Swords, 36.0, 68.0 );
		}

		public override void OnThink()
		{
			if ( DateTime.UtcNow >= m_NextMusic )
			{
				if ( m_Instrument != null && m_Instrument.IsChildOf( Backpack ) )
					m_Instrument.PlayInstrumentWell( this );

				m_NextMusic = DateTime.UtcNow + m_MusicDelay + TimeSpan.FromSeconds( Utility.Random( 2 ) );
			}
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			if ( 0.0025 > Utility.RandomDouble() )
				m_Instrument = new BambooFlute();
			else
				switch ( Utility.Random( 6 ) )
				{
					case 0: m_Instrument = new Drums(); break;
					case 1: m_Instrument = new Harp(); break;
					case 2: m_Instrument = new LapHarp(); break;
					case 3: m_Instrument = new Lute(); break;
					case 4: m_Instrument = new Tambourine(); break;
					case 5: m_Instrument = new TambourineTassel(); break;
				}

			if ( m_Instrument != null )
				PackItem( m_Instrument );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBBard() );
		}

		public Bard( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Backpack != null )
				m_Instrument = Backpack.FindItemByType( typeof( BaseInstrument ) ) as BaseInstrument;
		}
	}
}