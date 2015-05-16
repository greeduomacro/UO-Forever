using System;
using Server;

namespace Server.Items
{
	public class QuiverOfShadows : BaseQuiver
	{
		public override string DefaultName{ get{ return "a quiver of the shadows"; } }
		public override int DefaultMaxWeight{ get{ return 100; } }

		[Constructable]
		public QuiverOfShadows() : base()
		{
			Hue = 1175;

			DamageIncrease = 0;
			WeightReduction = 70;
			Capacity = 1000;
			LootType = LootType.Blessed;
		}

		public QuiverOfShadows( Serial serial ) : base( serial )
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