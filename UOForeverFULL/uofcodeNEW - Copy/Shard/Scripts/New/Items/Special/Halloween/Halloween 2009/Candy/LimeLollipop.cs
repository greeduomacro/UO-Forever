using System;
using Server;

namespace Server.Items
{
	public class LimeLollipop : BaseCandy
	{
		public override int LabelNumber { get{ return 1096935; } } // lime lollipop

		[Constructable]
		public LimeLollipop() : this( 1 )
		{
		}

		[Constructable]
		public LimeLollipop( int amount ) : base( amount, 0x468F )
		{
		}

		public LimeLollipop( Serial serial ) : base( serial )
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