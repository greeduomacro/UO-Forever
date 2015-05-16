using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xEC3, 0xEC2 )]
	public class Cleaver : BaseKnife
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.InfectiousStrike; } }

		public override int OldStrengthReq{ get{ return 10; } }
        public override int NewMinDamage { get { return WeaponDamageController._CleaverDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._CleaverDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 1, 12, 1 ); } } // 1d12+1 (2-13)
		public override int OldSpeed{ get{ return 40; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 50; } }

		[Constructable]
		public Cleaver() : base( 0xEC3 )
		{
			Weight = 2.0;
		}

		public Cleaver( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}