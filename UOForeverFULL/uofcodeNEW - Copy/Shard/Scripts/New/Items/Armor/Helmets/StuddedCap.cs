using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1db9, 0x1dba )]
	public class StuddedCap : BaseArmor
	{
		
		
		
		
		

		public override int InitMinHits{ get{ return 35; } }
		public override int InitMaxHits{ get{ return 45; } }

		
		public override int OldStrReq{ get{ return 20; } }

		public override int ArmorBase{ get{ return 16; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Studded; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.Half; } }

		public override string DefaultName
		{
			get
			{
				string name = "studded cap";

				if ( Resource >= CraftResource.SpinedLeather && Resource <= CraftResource.BarbedLeather )
					name = String.Format( "{0} {1}", CraftResources.GetInfo( Resource ).Name.ToLower(), name );

				return name;
			}
		}

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get
			{	if ( (base.Hue % 16384) == 0 )
					return base.Hue + 1533;
				else
					return base.Hue;
			}
			set
			{
				base.Hue = value;
			}
		}

		[Constructable]
		public StuddedCap() : base( 0x1DB9 )
		{
			Weight = 2.0;
			Dyable = true;
			//Hue = 1533;
		}

		public override void ScissorHelper( Mobile from, Item newItem, int amountPerOldItem, bool carryHue )
		{
			base.ScissorHelper( from, newItem, amountPerOldItem, carryHue );

			if ( newItem.Hue == 1533 )
				newItem.Hue = 0;
		}

		public StuddedCap( Serial serial ) : base( serial )
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

			if ( Hue == 0x1533 )
				Hue = 1533;
		}
	}
}