using System;
using Server;
using Server.Gumps;
using Server.Network;
using System.Collections;
using Server.Multis;
using Server.Mobiles;


namespace Server.Items
{
	public class CompleteEurystheusBox : Item
	{
		[Constructable]
		public CompleteEurystheusBox() : this( null )
		{
		}

		[Constructable]
		public CompleteEurystheusBox ( string name ) : base ( 0x1EBA )
		{
			Name = "A Complete Eurystheus Collection Box";
			LootType = LootType.Blessed;
			Hue = 0x137;
		}

		public CompleteEurystheusBox ( Serial serial ) : base ( serial )
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