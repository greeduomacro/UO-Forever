using System;

namespace Server.Items
{
	public class DirtyFryPan : Item
	{
		[Constructable]
		public DirtyFryPan() : base( 0x9DE )
		{
				Weight = 1.0;
			Stackable = false;
			Name = "Dirty Frying Pan";
		}

		public DirtyFryPan( Serial serial ) : base( serial )
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