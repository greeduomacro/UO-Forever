using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x13B9, 0x13Ba )]
	public class VikingSword : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int OldStrengthReq{ get{ return 40; } }
        public override int NewMinDamage { get { return WeaponDamageController._VikingSwordDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._VikingSwordDamageMax; } }
		public override int DiceDamage{ get{ return Utility.Dice( 4, 8, 2 ); } } // 4d8+2 (6-34)  // WAS 3d8?? what?
		public override int OldSpeed{ get{ return 36; } }

		public override int DefHitSound{ get{ return 0x237; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 100; } }

		[Constructable]
		public VikingSword() : base( 0x13B9 )
		{
			Weight = 6.0;
		}

		public VikingSword( Serial serial ) : base( serial )
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