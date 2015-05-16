using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseManaRefreshPotion : BasePotion
	{
		public abstract double Refresh{ get; }

		public BaseManaRefreshPotion( PotionEffect effect ) : base( 0xF0D, effect )
		{
			Hue = 1072;
		}

		public BaseManaRefreshPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override bool Drink( Mobile from )
		{
			if ( from.Mana < from.ManaMax )
			{
				if ( from.BeginAction( typeof( BaseManaRefreshPotion ) ) )
				{
					from.Mana += Scale( from, (int)(Refresh * from.ManaMax) );

					BasePotion.PlayDrinkEffect( from );

					Delete();

					Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 20.0 ), new TimerStateCallback<Mobile>( ReleaseManaLock ), from );

					return true;
				}
				else
					from.LocalOverheadMessage( MessageType.Regular, 0x22, false, "You must wait 20 seconds before using another mana refresh potion." );

			}
			else
				from.SendMessage( "You decide against drinking this potion, as you already have full mana." );

			return false;
		}

		private static void ReleaseManaLock( Mobile from )
		{
			from.EndAction( typeof( BaseManaRefreshPotion ) );
		}
	}
}