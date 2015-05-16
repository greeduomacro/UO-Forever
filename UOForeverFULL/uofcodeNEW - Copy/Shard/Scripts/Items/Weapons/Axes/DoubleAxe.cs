using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xf4b, 0xf4c )]
	public class DoubleAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }

		public override int OldStrengthReq{ get{ return 45; } }
        public override int NewMinDamage { get { return WeaponDamageController._DoubleAxeDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._DoubleAxeDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 1, 31, 4 ); } } // 1d31+4 (5-35)
		public override int OldSpeed{ get{ return 37; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		[Constructable]
		public DoubleAxe() : base( 0xF4B )
		{
			Weight = 8.0;
		}

		public DoubleAxe( Serial serial ) : base( serial )
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