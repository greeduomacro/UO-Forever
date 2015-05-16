using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x13FB, 0x13FA )]
	public class LargeBattleAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int OldStrengthReq{ get{ return 40; } }
        public override int NewMinDamage { get { return WeaponDamageController._LargeBattleAxeDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._LargeBattleAxeDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 2, 17, 4 ); } } // 2d17+4 (6-38)
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		[Constructable]
		public LargeBattleAxe() : base( 0x13FB )
		{
			Weight = 6.0;
		}

		public LargeBattleAxe( Serial serial ) : base( serial )
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