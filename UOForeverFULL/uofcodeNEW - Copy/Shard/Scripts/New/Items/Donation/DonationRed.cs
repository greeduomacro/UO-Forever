using System;
using Server;

namespace Server.Items
{
	public class DonationRed : Sandals
	{
		//public override int ArtifactRarity{ get{ return 11; } }

		[Constructable]
		public DonationRed()
		{
			Name = "I support ultima online forever";
			LootType = LootType.Blessed;
			Hue = 1157;
		}

		public DonationRed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (Hue != 1157)
                Hue = 1157;
		}
	}
}
