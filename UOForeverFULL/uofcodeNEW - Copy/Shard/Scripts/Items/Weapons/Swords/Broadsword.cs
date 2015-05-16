using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF5E, 0xF5F )]
	public class Broadsword : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }

		public override int OldStrengthReq{ get{ return 25; } }
        public override int NewMinDamage { get { return WeaponDamageController._BroadswordDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._BroadswordDamageMax; } }
		public override int DiceDamage { get{ return Utility.Dice( 2, 13, 3 ); } } // 2d13+3 (5-29)
		public override int OldSpeed{ get{ return 45; } }

		public override int DefHitSound{ get{ return 0x237; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 100; } }

		[Constructable]
		public Broadsword() : base( 0xF5E )
		{
			Weight = 6.0;
		}

		public Broadsword( Serial serial ) : base( serial )
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