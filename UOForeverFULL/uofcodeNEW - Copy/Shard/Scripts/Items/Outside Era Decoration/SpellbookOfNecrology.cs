using System;
using Server;

namespace Server.Items
{
	public class SpellbookOfNecrology : NecromancerSpellbook
	{
		public override string DefaultName{ get{ return "spellbook of necrology"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public SpellbookOfNecrology() : this( (ulong)0 )
		{
		}

		[Constructable]
		public SpellbookOfNecrology( ulong content ) : base( content )
		{
			Hue = 1437;
		}

		public SpellbookOfNecrology( Serial serial ) : base( serial )
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