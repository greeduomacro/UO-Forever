using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
	public class SpearOfTheDying : BaseSpear
	{
        public override int ArtifactRarity{ get{ return 15; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int NewMinDamage{ get{ return 2; } }
		public override int NewMaxDamage{ get{ return 34; } }
		public override int OldSpeed{ get{ return 46; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public SpearOfTheDying() : base( 0xF62 )
		{
			Name = "Spear Of The Dying";
			Weight = 7.0;
			Hue = 1143;
		}

		public SpearOfTheDying( Serial serial ) : base( serial )
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