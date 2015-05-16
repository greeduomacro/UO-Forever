using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF49, 0xF4a )]
	public class Axe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int OldStrengthReq{ get{ return 35; } }
		public override int NewMinDamage{ get{ return WeaponDamageController._AxeDamageMin; } }
		public override int NewMaxDamage{ get{ return WeaponDamageController._AxeDamageMax; } }

        public override int DiceDamage { get { return Utility.Dice(3, 10, 3); } }

        public override int OldSpeed { get { return 37; } }
		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 100; } }

		[Constructable]
		public Axe() : base( 0xF49 )
		{
			Weight = 4.0;
		}

		public Axe( Serial serial ) : base( serial )
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