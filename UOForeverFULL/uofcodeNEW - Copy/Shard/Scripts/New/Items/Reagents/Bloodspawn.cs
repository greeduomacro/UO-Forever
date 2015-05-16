using System;
using Server;

namespace Server.Items
{
	public class Bloodspawn : BaseReagent
	{
		[Constructable]
		public Bloodspawn() : this( 1 )
		{
		}

		[Constructable]
		public Bloodspawn( int amount ) : base( 0xf7c, amount )
		{
		}

		public Bloodspawn( Serial serial ) : base( serial )
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