using System;
using Server;

namespace Server.Items
{
   	public class CerberusBone : Item
   	{
    	[Constructable]
    	public CerberusBone() : base( 0xF7E )
    	{
	 		Name = "Cerberus's Bone";
	 		Hue = 0x497;
     	    Weight = 0.1; 
    	}
      	
   		public CerberusBone( Serial serial ) : base( serial )
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