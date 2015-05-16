using System;
using Server;
using Server.Engines.Conquests;
using Server.Mobiles;
using Server.Regions;

namespace Server.Items
{
	public abstract class BaseRefreshPotion : BasePotion
	{
		public abstract double Refresh{ get; }

		public BaseRefreshPotion( PotionEffect effect ) : base( 0xF0B, effect )
		{
		}

		public BaseRefreshPotion( Serial serial ) : base( serial )
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

		public override bool Drink( Mobile from )
		{
			if ( from.Stam < from.StamMax )
			{
                CustomRegion region1 = from.Region as CustomRegion;

				from.Stam += Scale( from, (int)(Refresh * from.StamMax) );

				BasePotion.PlayDrinkEffect( from );

                if (!Engines.ConPVP.DuelContext.IsFreeConsume(from) && (region1 == null || !region1.PlayingGame(from)))
					this.Consume();
			}
			else
			{
				from.SendMessage( "You decide against drinking this potion, as you are already at full stamina." );
				return false;
			}

			return true;
		}
	}
}