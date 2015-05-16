using System;
using Server;
using Server.Engines.Craft;
using Server.Misc;
 
namespace Server.Items
{
	public class RunicSmithHammerWeapon : SmithHammerWeapon, IRunicTool
	{
		[Constructable]
		public RunicSmithHammerWeapon( CraftResource resource, int uses ) : base( uses )
		{
			Resource = resource;
			Identified = true;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			int index = CraftResources.GetIndex( Resource );

			if ( index >= 1 && index <= 8 )
				list.Add( 1049019 + index );
			else
				list.Add( 1045128 ); // runic smithy hammer
		}

		public override int LabelNumber { get { int index = CraftResources.GetIndex( Resource ); return ( index >= 1 && index <= 8 ) ? 1049019 + index : 1045128; } } // runic smithy hammer

		public RunicSmithHammerWeapon( Serial serial ) : base( serial )
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