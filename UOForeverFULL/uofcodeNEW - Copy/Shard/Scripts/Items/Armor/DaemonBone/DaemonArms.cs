using System;

namespace Server.Items
{
	[FlipableAttribute( 0x144e, 0x1453 )]
	public class DaemonArms : BaseArmor
	{
		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int OldStrReq{ get{ return 40; } }

		public override int OldDexBonus{ get{ return -1; } }

		public override int ArmorBase{ get{ return 46; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Bone; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override int LabelNumber{ get{ return 1041371; } } // daemon bone arms

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get
			{	if ( (base.Hue % 16384) == 0 )
					return base.Hue + 0x648;
				else
					return base.Hue;
			}
			set
			{
				base.Hue = value;
			}
		}

		[Constructable]
		public DaemonArms() : base( 0x144E )
		{
			Weight = 2.0;
		}

		public DaemonArms( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}