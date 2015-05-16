using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xf45, 0xf46 )]
	public class ExecutionersAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int OldStrengthReq{ get{ return 35; } }
        public override int NewMinDamage { get { return WeaponDamageController._ExecutionersAxeDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._ExecutionersAxeDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 3, 10, 3 ); } } // 3d10+3 (6-33)
		public override int OldSpeed{ get{ return 37; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 90; } }

		[Constructable]
		public ExecutionersAxe() : base( 0xF45 )
		{
			Weight = 8.0;
		}

		public ExecutionersAxe( Serial serial ) : base( serial )
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