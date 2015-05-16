using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseLethargyPotion : BasePotion
	{
		public abstract int Loss{ get; }

		public BaseLethargyPotion( PotionEffect effect ) : base( 0xE24, effect )
		{
			Hue = 1278;
		}

		public BaseLethargyPotion( Serial serial ) : base( serial )
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
		}

		public void DoStatLoss( Mobile from )
		{
			from.RawDex -= Loss;
		}

		public override bool Drink( Mobile from )
		{
			if ( from.RawDex > 10 )
			{
				if ( from.BeginAction( typeof( BaseLethargyPotion ) ) )
				{
					DoStatLoss( from );

					BasePotion.PlayDrinkEffect( from );

					this.Consume();

					new DelayTimer( from ).Start();

					return true;
				}
				else
					from.LocalOverheadMessage( MessageType.Regular, 0x22, true, "You must wait for your body to adjust to the potion." );
			}
			else
				from.SendMessage( "You decide against drinking this potion, as you are already fairly slow." );

			return false;
		}

		private class DelayTimer : Timer
		{
			private Mobile m_Mobile;

			public DelayTimer( Mobile m ) : base( TimeSpan.FromSeconds( 3.0 ) )
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( BaseLethargyPotion ) );
			}
		}
	}
}