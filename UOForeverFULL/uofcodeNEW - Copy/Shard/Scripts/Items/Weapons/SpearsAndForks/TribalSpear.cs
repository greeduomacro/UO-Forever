using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
	public class TribalSpear : BaseSpear
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int OldStrengthReq{ get{ return 30; } }
        public override int NewMinDamage { get { return WeaponDamageController._TribalSpearDamageMin; } }// equivalent to a force spear
        public override int NewMaxDamage { get { return WeaponDamageController._TribalSpearDamageMax; } }
		public override int DiceDamage { get { return Utility.Dice( 2, 14, 4 ); } } //2d14 + 4 (6-36)
		public override int OldSpeed{ get{ return 46; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		//public override int VirtualDamageBonus{ get{ return 25; } }

		public override string DefaultName{ get { return "a tribal spear"; } }

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get
			{	if ( (base.Hue % 16384) == 0 )
					return base.Hue + 837;
				else
					return base.Hue;
			}
			set
			{
				base.Hue = value;
			}
		}

		[Constructable]
		public TribalSpear() : base( 0xF62 )
		{
			Weight = 7.0;
		}

		public TribalSpear( Serial serial ) : base( serial )
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

			if ( !Identified && Hue == 837 )
				Hue = 0;
		}
	}
}