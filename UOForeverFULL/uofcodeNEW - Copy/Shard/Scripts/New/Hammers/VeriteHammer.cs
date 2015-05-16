using System;
using Server;
using Server.Engines.Craft;


namespace Server.Items
{
	public class VeriteHammer : RunicHammer1
	{

		[Constructable]
		public VeriteHammer() : this( 50 )
		{
		}		

		[Constructable]
		public VeriteHammer( int uses ) : base( CraftResource.Verite )
		{
			Weight = 1.0;
			UsesRemaining = uses;
			Name = "A runic hammer";
		}
		public VeriteHammer( Serial serial ) : base( serial )
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