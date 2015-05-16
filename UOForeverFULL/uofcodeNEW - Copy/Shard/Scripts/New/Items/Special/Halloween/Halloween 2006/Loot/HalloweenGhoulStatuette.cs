using System;
using Server;

namespace Server.Items
{
	public class HalloweenGhoulStatuette : MonsterStatuette
	{
		public override int LabelNumber{ get{ return 1076782; } } // Halloween Ghoul Statuette

		[Constructable]
		public HalloweenGhoulStatuette() : base( MonsterStatuetteType.Ghoul_Old )
		{
			LootType = LootType.Regular;
			Hue = Utility.RandomList( 47, 997 );
		}

		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled && EraAOS; } }

		public HalloweenGhoulStatuette( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}