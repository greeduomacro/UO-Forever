using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x143E, 0x143F )]
	public class Halberd : BasePoleArm
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int OldStrengthReq{ get{ return 45; } }
        public override int NewMinDamage { get { return WeaponDamageController._HalberdDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._HalberdDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 2, 23, 3 ); } } // 2d23+3 (5-49)
		public override int OldSpeed{ get{ return 25; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public Halberd() : base( 0x143E )
		{
			Weight = 16.0;
		}

		public Halberd( Serial serial ) : base( serial )
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