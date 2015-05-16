using System;
using Server;

namespace Server.Items
{
	public class JellyBeans : BaseCandy
	{
		public override int LabelNumber { get{ return 1096932; } } // jellybeans

		[Constructable]
		public JellyBeans() : this( 1 )
		{
		}

		[Constructable]
		public JellyBeans( int amount ) : base( amount, 0x468C )
		{
		}

		public JellyBeans( Serial serial ) : base( serial )
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