using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x143E, 0x143F )]
	public class HalberdOfTheForgottenKing : BasePoleArm
	{
		public override int ArtifactRarity{ get{ return 20; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int OldStrengthReq{ get{ return 45; } }
		public override int NewMinDamage{ get{ return 5; } }
		public override int NewMaxDamage{ get{ return 49; } }
		public override int OldSpeed{ get{ return 25; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public HalberdOfTheForgottenKing() : base( 0x143E )
		{	
			Name = "Halberd of the Forgotten King";
			Hue  = 1260;
			Weight = 16.0;

			Slayer = SlayerName.DragonSlaying;
		}

		public HalberdOfTheForgottenKing( Serial serial ) : base( serial )
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