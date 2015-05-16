using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x1441, 0x1440 )]
	public class Cutlass : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ShadowStrike; } }

		public override int OldStrengthReq{ get{ return 10; } }
        public override int NewMinDamage { get { return WeaponDamageController._CutlassDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._CutlassDamageMax; } }
		public override int DiceDamage { get{ return Utility.Dice( 2, 12, 4 ); } } // 2d12+4 (6-28)
		public override int OldSpeed{ get{ return 45; } }

		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }

		[Constructable]
		public Cutlass() : base( 0x1441 )
		{
			Weight = 8.0;
		}

		public Cutlass( Serial serial ) : base( serial )
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