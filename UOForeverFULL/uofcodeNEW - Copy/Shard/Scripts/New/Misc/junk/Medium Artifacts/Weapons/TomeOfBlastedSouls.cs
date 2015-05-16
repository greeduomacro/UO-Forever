using System;
using Server;

namespace Server.Items
{
	public class TomeOfBlastedSouls : Spellbook
	{
		[Constructable]
		public TomeOfBlastedSouls()
		{
			this.Content = ulong.MaxValue;
			Movable = true;
			Name = "Tome Of Blasted Souls";
			Hue = 810;
			LootType = LootType.Regular;
			Attributes.LowerRegCost = 25;
			Attributes.SpellDamage = 8;
			Attributes.CastRecovery = 2;
		}
		
		public TomeOfBlastedSouls( Serial serial ) : base( serial )
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

