using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x13b4, 0x13b3 )]
	public class Club : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int OldStrengthReq{ get{ return 10; } }
        public override int NewMinDamage { get { return WeaponDamageController._ClubDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._ClubDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 4, 5, 4 ); } } // 4d5+4 (8-24)
		public override int OldSpeed{ get{ return 40; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 40; } }

		[Constructable]
		public Club() : base( 0x13B4 )
		{
			Weight = 9.0;
			Resource = CraftResource.RegularWood;
		}

		public Club( Serial serial ) : base( serial )
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