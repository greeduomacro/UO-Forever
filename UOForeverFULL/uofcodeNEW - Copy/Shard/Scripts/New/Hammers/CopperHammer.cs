using System;
using Server;
using Server.Engines.Craft;


namespace Server.Items
{
	public class CopperHammer : RunicHammer1
	{

		[Constructable]
		public CopperHammer() : this( 50 )
		{
		}		

		[Constructable]
		public CopperHammer( int uses ) : base( CraftResource.Copper )
		{
			Weight = 1.0;
			UsesRemaining = uses;
			Name = "A runic hammer";
		}
		public CopperHammer( Serial serial ) : base( serial )
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