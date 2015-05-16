using System;
using Server;

namespace Server.Items
{
	public class TotalManaRefreshPotion : BaseManaRefreshPotion
	{
		public override double Refresh{ get{ return 1.0; } }
		public override string DefaultName{ get{ return "total mana refresh potion"; } }

		[Constructable]
		public TotalManaRefreshPotion() : base( PotionEffect.TotalManaRefresh )
		{
		}

		public TotalManaRefreshPotion( Serial serial ) : base( serial )
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