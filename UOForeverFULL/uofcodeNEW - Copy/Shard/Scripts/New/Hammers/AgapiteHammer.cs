using System;
using Server;
using Server.Engines.Craft;


namespace Server.Items
{
	public class AgapiteHammer : RunicHammer1
	{

		[Constructable]
		public AgapiteHammer() : this( 50 )
		{
		}		

		[Constructable]
		public AgapiteHammer( int uses ) : base( CraftResource.Agapite )
		{
			Weight = 1.0;
			UsesRemaining = uses;
			Name = "A runic hammer";
		}
		public AgapiteHammer( Serial serial ) : base( serial )
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