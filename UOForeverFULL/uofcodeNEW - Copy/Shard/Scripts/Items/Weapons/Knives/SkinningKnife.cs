using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xEC4, 0xEC5 )]
	public class SkinningKnife : BaseKnife
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int OldStrengthReq{ get{ return 5; } }
        public override int NewMinDamage { get { return WeaponDamageController._SkinningKnifeDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._SkinningKnifeDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 1, 10, 0 ); } } // 1d10 (1-10)
		public override int OldSpeed{ get{ return 40; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 40; } }

		[Constructable]
		public SkinningKnife() : base( 0xEC4 )
		{
			Weight = 1.0;
		}

		public SkinningKnife( Serial serial ) : base( serial )
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