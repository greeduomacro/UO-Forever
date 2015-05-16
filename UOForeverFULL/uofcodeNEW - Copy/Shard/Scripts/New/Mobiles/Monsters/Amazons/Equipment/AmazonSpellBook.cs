using System;
using Server;

namespace Server.Items
{
	public class AmazonSpellbook : Spellbook
	{
		public override string DefaultName{ get{ return "amazon spellbook"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public AmazonSpellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public AmazonSpellbook( ulong content ) : base( content )
		{
			LootType = LootType.Regular;
			Hue = 58;
		}

		public AmazonSpellbook( Serial serial ) : base( serial )
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