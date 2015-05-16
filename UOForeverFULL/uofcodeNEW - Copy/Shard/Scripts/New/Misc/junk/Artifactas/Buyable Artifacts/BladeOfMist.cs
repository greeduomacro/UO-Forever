using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x26C1, 0x26CB )]
	public class BladeOfMist : BaseSword
	{
public override int ArtifactRarity{ get{ return 10; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int OldStrengthReq{ get{ return 55; } }
		public override int NewMinDamage{ get{ return 11; } }
		public override int NewMaxDamage{ get{ return 14; } }
		public override int OldSpeed{ get{ return 47; } }

		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 51; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public BladeOfMist() : base( 0x26C1 )
		{
			Name = "Blade Of Mist";
			Weight = 1.0;
			Hue = 1892;
		}
		

		public BladeOfMist( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}