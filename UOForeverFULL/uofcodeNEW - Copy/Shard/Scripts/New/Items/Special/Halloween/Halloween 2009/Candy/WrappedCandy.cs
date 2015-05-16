using System;
using Server;

namespace Server.Items
{
	public class WrappedCandy : BaseCandy
	{
		public override int LabelNumber { get{ return 1096950; } } // wrapped candy

		[Constructable]
		public WrappedCandy() : this( 1 )
		{
		}

		[Constructable]
		public WrappedCandy( int amount ) : base( amount, 0x469E )
		{
		}

		public WrappedCandy( Serial serial ) : base( serial )
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