using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x2D27, 0x2D33 )]
	public class RadiantWarSword : BaseSword
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int NewMinDamage{ get{ return 6; } }
		public override int NewMaxDamage{ get{ return 34; } }
		public override int OldSpeed{ get{ return 30; } }

		public override int DefHitSound{ get{ return 0x237; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override string DefaultName{ get{ return "radiant war axe"; } }

		[Constructable]
		public RadiantWarSword() : base( 0x2D27 )
		{
			Weight = 6.0;
			Hue = 2407;
		}

		public RadiantWarSword( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}