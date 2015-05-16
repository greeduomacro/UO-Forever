using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF47, 0xF48 )]
	public class BattleAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int NewMinDamage{ get{ return WeaponDamageController._BattleAxeDamageMin; } }
        public override int NewMaxDamage { get { return WeaponDamageController._BattleAxeDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 2, 17, 4 ); } } // 2d17+4 (6-38)
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public BattleAxe() : base( 0xF47 )
		{
			Weight = 4.0;
			Layer = Layer.TwoHanded;
		}

		public BattleAxe( Serial serial ) : base( serial )
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