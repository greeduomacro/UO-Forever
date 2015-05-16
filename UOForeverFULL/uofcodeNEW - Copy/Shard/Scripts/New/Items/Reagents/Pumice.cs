using System;
using Server;

namespace Server.Items
{
	public class Pumice : BaseReagent
	{
		[Constructable]
		public Pumice() : this( 1 )
		{
		}

		[Constructable]
		public Pumice( int amount ) : base( 0xf8b, amount )
		{
		}

		public Pumice( Serial serial ) : base( serial )
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