using System;
using Server;

namespace Server.Items
{
	public class DonationBlack : Sandals
	{
		//public override int ArtifactRarity{ get{ return 11; } }

		[Constructable]
		public DonationBlack()
		{
			Name = "i support ultima online forever";
			LootType = LootType.Blessed;
			Hue = 1175;
	/*		Attributes.SpellDamage = 10;
			Attributes.LowerManaCost = 5;
			Attributes.ReflectPhysical = 5;
            Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 3;
			Resistances.Physical = 10;
			Resistances.Fire = 10;
			Resistances.Cold = 10;
			Resistances.Poison = 10;
			Resistances.Energy = 10;
*/
		}

		public DonationBlack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Hue == 1175 )
				Hue = 1175;
		}
	}
}
