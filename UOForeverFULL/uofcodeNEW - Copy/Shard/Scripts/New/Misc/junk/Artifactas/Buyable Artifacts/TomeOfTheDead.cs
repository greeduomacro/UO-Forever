using System;
using Server;

namespace Server.Items
{
	public class TomeOfTheDead : Spellbook
	{
	

		[Constructable]
		public TomeOfTheDead()
		{
			this.Content = ulong.MaxValue;
			Movable = true;
			Name = "Tome Of The Dead";
			Hue = 1156;
			LootType = LootType.Regular;
			Attributes.RegenMana = 10;
			Attributes.LowerManaCost = 30;
		}
		
		public TomeOfTheDead( Serial serial ) : base( serial )
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

