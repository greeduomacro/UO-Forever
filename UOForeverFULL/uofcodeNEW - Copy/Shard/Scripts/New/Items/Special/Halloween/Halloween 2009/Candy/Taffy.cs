using System;
using Server;

namespace Server.Items
{
	public class Taffy : BaseCandy
	{
		public override int LabelNumber { get{ return 1096949; } } // taffy

		[Constructable]
		public Taffy() : this( 1 )
		{
		}

		[Constructable]
		public Taffy( int amount ) : base( amount, 0x469D )
		{
		}

		public Taffy( Serial serial ) : base( serial )
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