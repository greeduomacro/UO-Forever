using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x143D, 0x143C )]
	public class HammerPick : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int OldStrengthReq{ get{ return 35; } }
        public override int NewMinDamage { get { return WeaponDamageController._HammerPickDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._HammerPickDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 3, 10, 3 ); } } // 3d10+3 (6-33)
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }

		[Constructable]
		public HammerPick() : base( 0x143D )
		{
			Weight = 9.0;
			Layer = Layer.OneHanded;
		}

		public HammerPick( Serial serial ) : base( serial )
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