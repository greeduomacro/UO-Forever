using System;
using Server.Network;

namespace Server.Items
{
	public class MagicWand : BaseBashing
	{
		public override WeaponAbility PrimaryAbility { get { return WeaponAbility.Dismount; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.Disarm; } }

		public override int OldStrengthReq{ get{ return 0; } }
        public override int NewMinDamage { get { return WeaponDamageController._MagicWandDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._MagicWandDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 2, 3, 0 ); } } // 2d3 (2-6)
		public override int OldSpeed{ get{ return 35; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		[Constructable]
		public MagicWand() : base( 0xDF2 )
		{
			Weight = 1.0;
		}

		public MagicWand( Serial serial ) : base( serial )
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