using System;
using Server;

namespace Server.Items
{
	public class EnchantedWood : BaseReagent
	{
		public override string DefaultName{ get{ return "an enchanted wood fragment"; } }

		[Constructable]
		public EnchantedWood() : this( 1 )
		{
		}

		[Constructable]
		public EnchantedWood( int amount ) : base( 0xF90, amount )
		{
			//LootType = LootType.Newbied;
		}

		public EnchantedWood( Serial serial ) : base( serial )
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