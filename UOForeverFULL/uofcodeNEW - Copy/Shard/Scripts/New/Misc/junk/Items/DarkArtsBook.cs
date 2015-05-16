using System;
using Server;

namespace Server.Items
{
	public class DarkArtsBook : Spellbook
	{
		[Constructable]
		public DarkArtsBook()
		{
			this.Content = ulong.MaxValue;
			Movable = true;
			Name = "Book of the Dark Arts";
			Hue = 1152;
			LootType = LootType.Regular;
			Attributes.BonusInt = 5;
			Attributes.RegenMana = 10;
			Attributes.SpellDamage = 10;
		}
		
		public DarkArtsBook( Serial serial ) : base( serial )
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

