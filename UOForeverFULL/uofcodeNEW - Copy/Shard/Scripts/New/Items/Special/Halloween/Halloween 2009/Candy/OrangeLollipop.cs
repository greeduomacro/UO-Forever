using System;
using Server;

namespace Server.Items
{
	public class OrangeLollipop : BaseCandy
	{
		public override int LabelNumber { get{ return 1096934; } } // orange lollipop

		[Constructable]
		public OrangeLollipop() : this( 1 )
		{
		}

		[Constructable]
		public OrangeLollipop( int amount ) : base( amount, 0x468E )
		{
		}

		public OrangeLollipop( Serial serial ) : base( serial )
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