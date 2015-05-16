using System;
using Server;

namespace Server.Items
{
	public class FullSpellbook : Spellbook
	{
		[Constructable]
		public FullSpellbook()
		{
			this.Content = ulong.MaxValue;
		}
		
		public FullSpellbook( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}

