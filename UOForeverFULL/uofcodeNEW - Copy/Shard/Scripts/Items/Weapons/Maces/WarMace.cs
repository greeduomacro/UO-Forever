using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x1407, 0x1406 )]
	public class WarMace : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int OldStrengthReq{ get{ return 30; } }
        public override int NewMinDamage { get { return WeaponDamageController._WarMaceDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._WarMaceDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 5, 5, 5 ); } } // 5d5+5 (10-30)
		public override int OldSpeed{ get{ return 32; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		[Constructable]
		public WarMace() : base( 0x1407 )
		{
			Weight = 17.0;
		}

		public WarMace( Serial serial ) : base( serial )
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