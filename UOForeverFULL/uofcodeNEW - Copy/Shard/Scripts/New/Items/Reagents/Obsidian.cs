using System;
using Server;

namespace Server.Items
{
	public class Obsidian : BaseReagent
	{
		[Constructable]
		public Obsidian() : this( 1 )
		{
		}

		[Constructable]
		public Obsidian( int amount ) : base( 0xF89, amount )
		{
		}

		public Obsidian( Serial serial ) : base( serial )
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