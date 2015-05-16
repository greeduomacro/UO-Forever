using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class CamelSpiderEgg : Item, ICarvable
	{
		private EggTimer m_Timer;

		[Constructable]
		public CamelSpiderEgg() : base( 0x10D9 )
		{
			Movable = false;
			m_Timer = new EggTimer(this);
			m_Timer.Start();
		}

//carvable
		public void Carve( Mobile from, Item item )
		{
			from.SendMessage( "You destroy the egg." );

			if (m_Timer != null)
			{
				m_Timer.Stop();
				m_Timer = null;
			}

			Delete();
		}
//carvable

		private class EggTimer : Timer
		{
			private CamelSpiderEgg m_Egg;

			public EggTimer(CamelSpiderEgg egg) : base ( TimeSpan.FromSeconds(15) )
			{
				m_Egg = egg;
			}

			protected override void OnTick()
			{
				Map map = m_Egg.Map;

				if ( map == null )
					return;

				CamelSpiderHatchling spawned = new CamelSpiderHatchling();

				bool validLocation = false;
				Point3D loc = m_Egg.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = loc.X + Utility.Random( 3 ) - 1;
					int y = loc.Y + Utility.Random( 3 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, loc.Z, 16, false, false ) )
						loc = new Point3D( x, y, loc.Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				spawned.MoveToWorld( loc, map );

				Stop();
				m_Egg.Delete();
			}
		}

		public CamelSpiderEgg(Serial serial) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			new EggTimer(this).Start();
		}
	}
}
