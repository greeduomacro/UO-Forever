using System;
using Server;
using Server.Engines.Harvest;

namespace Server.Items
{
	public class OrcishPickaxe : BaseAxe
	{
		public override string DefaultName{ get{ return "orcish pickaxe"; } }
		public override HarvestSystem HarvestSystem{ get{ return Mining.System; } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int OldStrengthReq{ get{ return 25; } }
		public override int NewMinDamage{ get{ return 1; } }
		public override int NewMaxDamage{ get{ return 15; } }
		public override int OldSpeed{ get{ return 35; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash1H; } }

		[Constructable]
		public OrcishPickaxe() : this( Utility.RandomMinMax( 300, 500 ) )
		{
		}

		[Constructable]
		public OrcishPickaxe( int uses ) : base( 0xE86 )
		{
			Weight = 11.0;
			Hue = 2115;
			UsesRemaining = uses;
			ShowUsesRemaining = true;
		}

		public OrcishPickaxe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}