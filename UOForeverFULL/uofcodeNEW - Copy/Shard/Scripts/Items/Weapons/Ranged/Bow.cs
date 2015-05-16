using System;

namespace Server.Items
{
	[FlipableAttribute( 0x13B2, 0x13B1 )]
	public class Bow : BaseRanged
	{
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int OldStrengthReq{ get{ return 20; } }
        public override int NewMinDamage { get { return WeaponDamageController._BowDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._BowDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 4, 9, 5 ); } } // 4d9+5 (9-41)
		public override int OldSpeed{ get{ return 25; } }

		public override int DefMaxRange{ get{ return 12; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		[Constructable]
		public Bow() : base( 0x13B2 )
		{
			Weight = 6.0;
			Layer = Layer.TwoHanded;
			Resource = CraftResource.RegularWood;
		}

		public Bow( Serial serial ) : base( serial )
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

			if ( Weight == 7.0 )
				Weight = 6.0;
		}
	}
}