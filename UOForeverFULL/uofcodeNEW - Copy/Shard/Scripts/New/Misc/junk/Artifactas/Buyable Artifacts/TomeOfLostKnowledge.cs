using System;
using Server;

namespace Server.Items
{
	public class TomeOfLostKnowledge2 : Spellbook
	{

		[Constructable]
		public TomeOfLostKnowledge2()
		{
			this.Content = ulong.MaxValue;
			Movable = true;
			Name = "Tome Of Lost Knowledge";
			Hue = 1167;
			LootType = LootType.Regular;
			Attributes.BonusInt = 7;
			Attributes.LowerRegCost = 35;
		}
		
		public TomeOfLostKnowledge2( Serial serial ) : base( serial )
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

