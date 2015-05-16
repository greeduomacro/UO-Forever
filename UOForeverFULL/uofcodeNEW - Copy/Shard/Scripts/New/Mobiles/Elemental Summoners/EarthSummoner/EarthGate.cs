using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class EarthGate : Item, ICarvable
	{
		private SpawnTimer m_Timer;
		private EarthSummoner m_Summoner;

		public EarthSummoner Summoner{ get{ return m_Summoner; } }

		[Constructable]
		public EarthGate( EarthSummoner summoner ) : base( 8148 )
		{
			Movable = false;
			Name = "Earth Gate";
			Hue = 2311;

			m_Timer = new SpawnTimer( this );
			m_Timer.Start();

			m_Summoner = summoner;
		}

		public void Carve( Mobile from, Item item )
		{
			Effects.PlaySound( GetWorldLocation(), Map, 86 );
			Effects.SendLocationEffect( GetWorldLocation(), Map, 0x3728, 10, 10, 0, 0 );

			if ( 0.3 > Utility.RandomDouble() )
			{
				if ( ItemID == 0x2809 )
					from.SendMessage( "You destroy the gate." );
				else
					from.SendMessage( "You destroy the earth gate." );

				Gold gold = new Gold( 25, 100 );

				gold.MoveToWorld( GetWorldLocation(), Map );

				Delete();

				m_Timer.Stop();
			}
			else
			{
				if ( ItemID == 0x2809 )
					from.SendMessage( "You damage the gate." );
				else
					from.SendMessage( "You damage the earthcalling gate." );
			}
		}

		public EarthGate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Timer = new SpawnTimer( this );
			m_Timer.Start();
		}

		private class SpawnTimer : Timer
		{
			private EarthGate m_Item;

			public SpawnTimer( EarthGate item ) : base( TimeSpan.FromSeconds( Utility.RandomMinMax( 5, 10 ) ) )
			{
				Priority = TimerPriority.OneSecond;

				m_Item = item;
			}

			protected override void OnTick()
			{
				if ( m_Item.Deleted )
					return;

				if ( m_Item.Summoner != null && !m_Item.Summoner.Deleted )
				{
					EarthElemental spawn = new EarthElemental();
					spawn.MoveToWorld( m_Item.Location, m_Item.Map );
					m_Item.Summoner.Summons.Add( spawn );
				}

				m_Item.Delete();
			}
		}
	}
}