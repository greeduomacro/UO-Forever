using System;
using Server;

namespace Server.Items
{
	public class Blood : Item
	{
		[Constructable]
		public Blood() : this( 0 )
		{
		}

		[Constructable]
		public Blood( bool longduration ) : this( 0, longduration )
		{
		}

		[Constructable]
		public Blood( int hue ) : this( hue, false )
		{
		}

		[Constructable]
		public Blood( int hue, bool longduration ) : this( Utility.RandomList( 0x1645, 0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F ), hue, longduration )
		{
		}

		[Constructable]
		public Blood( int itemID, int hue ) : this( itemID, hue, false )
		{
		}

		[Constructable]
		public Blood( int itemID, int hue, bool longduration ) : base( itemID )
		{
			Hue = hue;
			Movable = false;

			new InternalTimer( this, TimeSpan.FromSeconds( 3.0 + (Utility.RandomDouble() * (longduration ? 25.0 : 3.0) ) ) ).Start();
		}

		public Blood( Serial serial ) : base( serial )
		{
			new InternalTimer( this, TimeSpan.FromSeconds( 3.0 + (Utility.RandomDouble() * 3.0) ) ).Start();
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
		}

		private class InternalTimer : Timer
		{
			private Item m_Blood;

			public InternalTimer( Item blood, TimeSpan duration ) : base( duration )
			{
				Priority = TimerPriority.OneSecond;

				m_Blood = blood;
			}

			protected override void OnTick()
			{
                if (m_Blood.DoesNotDecay) return;
                m_Blood.Delete();
			}
		}
	}
}