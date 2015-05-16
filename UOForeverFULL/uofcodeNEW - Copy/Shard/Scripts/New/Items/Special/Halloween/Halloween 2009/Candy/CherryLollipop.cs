using System;
using Server;

namespace Server.Items
{
	public class CherryLollipop : BaseCandy
	{
		public override int LabelNumber { get{ return 1096933; } } // cherry lollipop

		[Constructable]
		public CherryLollipop() : this( 1 )
		{
		}

		[Constructable]
		public CherryLollipop( int amount ) : base( amount, 0x468D )
		{
		}

		public CherryLollipop( Serial serial ) : base( serial )
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