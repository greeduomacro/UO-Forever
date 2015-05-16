using System;
using Server;

namespace Server.Items
{
	public class RobeOfOwnage : BaseOuterTorso
	{
		[Constructable]
		public RobeOfOwnage() : base( 0x1F03 )
		{
			Weight = 3.0;
			Name = "Robe Of Ownage";
		}

		public RobeOfOwnage( Serial serial ) : base( serial )
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
