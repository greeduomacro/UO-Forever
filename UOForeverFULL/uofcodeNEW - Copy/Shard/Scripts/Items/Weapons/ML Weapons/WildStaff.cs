using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x2D25, 0x2D31 )]
	public class WildStaff : BaseStaff
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.Block; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ForceOfNature; } }

		public override int OldStrengthReq{ get{ return 15; } }
		public override int NewMinDamage{ get{ return 10; } }
		public override int NewMaxDamage{ get{ return 12; } }
		public override int OldSpeed{ get{ return 48; } }

		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public WildStaff() : base( 0x2D25 )
		{
			Weight = 8.0;
		}

		public WildStaff( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}