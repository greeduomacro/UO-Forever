using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x13FF, 0x13FE )]
	public class Katana : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }

		public override int OldStrengthReq{ get{ return 10; } }
        public override int NewMinDamage { get { return WeaponDamageController._KatanaDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._KatanaDamageMax; } }
		public override int DiceDamage { get{ return Utility.Dice( 3, 8, 2 ); } } // 3d8+2 (5-26)
		public override int OldSpeed{ get{ return 58; } }

		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 90; } }

		[Constructable]
		public Katana() : base( 0x13FF )
		{
			Weight = 6.0;
		}

		public Katana( Serial serial ) : base( serial )
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