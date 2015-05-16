using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x13B6, 0x13B5 )]
	public class Scimitar : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
        
		public override int OldStrengthReq{ get{ return 10; } }
        public override int NewMinDamage { get { return WeaponDamageController._ScimitarDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._ScimitarDamageMax; } }
		public override int DiceDamage{ get{ return Utility.Dice( 2, 14, 2 ); } } // 2d14+2 (4-30)
		public override int OldSpeed{ get{ return 43; } }

		public override int DefHitSound{ get{ return 0x23B; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 90; } }

		[Constructable]
		public Scimitar() : base( 0x13B6 )
		{
			Weight = 5.0;
		}

		public Scimitar( Serial serial ) : base( serial )
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