using System;

namespace Server.Items
{
	public class RareStoneSculpture : Item
	{
		public override string DefaultName{ get{ return "a rare stone sculpture"; } }

		[Constructable]
		public RareStoneSculpture() : base( 0x2848 )
		{
		}

		public RareStoneSculpture( Serial serial ) : base( serial )
		{
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
	}

}