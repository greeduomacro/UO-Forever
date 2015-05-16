using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x1441, 0x1440 )]
	public class EvilCutlass : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

		public override int OldStrengthReq{ get{ return 10; } }
		public override int NewMinDamage{ get{ return 16; } }
		public override int NewMaxDamage{ get{ return 28; } }
		//public override int DiceDamage { get{ return Utility.Dice( 6, 3, 10 ); } } //6d3+10 = 16-28
		public override int OldSpeed{ get{ return 42; } }

		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override string DefaultName{ get{ return "a brigand cutlass"; } }

		[Constructable]
		public EvilCutlass() : base( 0x1441 )
		{
			Weight = 8.0;
			Hue = 1899;
		}

		public EvilCutlass( Serial serial ) : base( serial )
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