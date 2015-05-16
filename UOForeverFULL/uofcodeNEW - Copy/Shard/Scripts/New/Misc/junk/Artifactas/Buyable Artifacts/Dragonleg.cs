using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xE89, 0xE8a )]
	public class DragonsLeg : BaseStaff
	{
public override int ArtifactRarity{ get{ return 10; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int NewMinDamage{ get{ return 8; } }
		public override int NewMaxDamage{ get{ return 28; } }
		public override int OldSpeed{ get{ return 48; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public DragonsLeg() : base( 0xE89 )
		{
			Name = "Dragon's Leg";
			Weight = 4.0;
            Hue = 62;
		}

		public DragonsLeg( Serial serial ) : base( serial )
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