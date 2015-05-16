using System;

namespace Server.Items
{
	public class ThreeCiderJugs : Item
	{
		[Constructable]
		public ThreeCiderJugs() : base( 0x98D )
		{
		}

		public ThreeCiderJugs( Serial serial ) : base( serial )
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