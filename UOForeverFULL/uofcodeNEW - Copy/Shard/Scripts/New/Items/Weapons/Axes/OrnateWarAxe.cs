using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x2D28, 0x2D34 )]
	public class OrnateWarAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int OldStrengthReq{ get{ return 35; } }
		public override int NewMinDamage{ get{ return 6; } }
		public override int NewMaxDamage{ get{ return 33; } }
		public override int OldSpeed{ get{ return 37; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }

		public override string DefaultName{ get{ return "ornate war axe"; } }

		[Constructable]
		public OrnateWarAxe() : base( 0x2D28 )
		{
			Weight = 8.0;
			Hue = 2407;
		}

		public OrnateWarAxe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}