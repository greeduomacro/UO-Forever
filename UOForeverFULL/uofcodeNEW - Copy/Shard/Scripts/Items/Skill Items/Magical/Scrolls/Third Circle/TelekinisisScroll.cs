using System;
using Server;

namespace Server.Items
{
	public class TelekinesisScroll : SpellScroll
	{
		[Constructable]
		public TelekinesisScroll() : this( 1 )
		{
		}

		[Constructable]
		public TelekinesisScroll( int amount ) : base( 20, 0x1F41, amount )
		{
		}

		public TelekinesisScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}