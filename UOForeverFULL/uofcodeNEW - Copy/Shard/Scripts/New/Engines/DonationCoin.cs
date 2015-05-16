using System;
using Server;

namespace Server.Items
{
	public class DonationCoin : BaseMarkCoin
	{
		public override string DefaultName{ get{ return " Forever Donation Coins"; } }

		[Constructable(AccessLevel.Administrator)]
		public DonationCoin() : this( 1 )
		{
		}

		[Constructable(AccessLevel.Administrator)]
		public DonationCoin( int amount ) : base( amount )
		{
		}

		public DonationCoin( Serial serial ) : base( serial )
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
	}
}