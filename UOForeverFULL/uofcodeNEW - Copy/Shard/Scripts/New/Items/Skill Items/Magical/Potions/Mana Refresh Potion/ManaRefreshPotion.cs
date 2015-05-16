using System;
using Server;

namespace Server.Items
{
	public class ManaRefreshPotion : BaseManaRefreshPotion
	{
		public override double Refresh{ get{ return 0.25; } }
		public override string DefaultName{ get{ return "mana refresh potion"; } }

		[Constructable]
		public ManaRefreshPotion() : base( PotionEffect.ManaRefresh )
		{
		}

		public ManaRefreshPotion( Serial serial ) : base( serial )
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