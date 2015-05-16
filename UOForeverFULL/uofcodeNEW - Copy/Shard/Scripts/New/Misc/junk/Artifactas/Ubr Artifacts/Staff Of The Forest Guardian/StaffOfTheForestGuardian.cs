using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13F8, 0x13F9 )]
	public class Staffoftheforestguardian : BaseStaff
	{
		public override int ArtifactRarity{ get{ return 20; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int OldStrengthReq{ get{ return 20; } }
		public override int NewMinDamage{ get{ return 10; } }
		public override int NewMaxDamage{ get{ return 30; } }
		public override int OldSpeed{ get{ return 33; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 50; } }

		[Constructable]
		public Staffoftheforestguardian() : base( 0x13F8 )
		{
			Name = "Staff of the Forest Guardian";
			Hue = 1289;
			Weight = 3.0;
			Slayer = SlayerName.ElementalHealth;
		}

		public Staffoftheforestguardian( Serial serial ) : base( serial )
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