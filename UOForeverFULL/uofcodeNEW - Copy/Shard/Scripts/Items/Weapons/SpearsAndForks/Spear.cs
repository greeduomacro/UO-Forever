using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
	public class Spear : BaseSpear
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int OldStrengthReq{ get{ return 30; } }
        public override int NewMinDamage { get { return WeaponDamageController._SpearDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._SpearDamageMax; } }
		public override int DiceDamage{ get{ return Utility.Dice( 2, 18, 0 ); } } // 2d16 (2-32)
		public override int OldSpeed{ get{ return 46; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public Spear() : base( 0xF62 )
		{
			Weight = 7.0;
		}

		public Spear( Serial serial ) : base( serial )
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