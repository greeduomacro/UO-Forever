using System;
using Server;
using Server.Engines.Craft;


namespace Server.Items
{
	public class GoldHammer : RunicHammer1
	{

		[Constructable]
		public GoldHammer() : this( 50 )
		{
		}		

		[Constructable]
		public GoldHammer( int uses ) : base( CraftResource.Gold )
		{
			Weight = 1.0;
			UsesRemaining = uses;
			Name = "A runic hammer";
		}
		public GoldHammer( Serial serial ) : base( serial )
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