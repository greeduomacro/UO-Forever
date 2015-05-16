using System;
using Server; 

namespace Server.Items 
{ 
	public class KrakenStatue: Item
	{ 
		[Constructable]
		public KrakenStatue() : base( 0x25A2)
		
		{
			Weight = 5;
			Hue= 0;
		     Name = "the Kracken";

		} 

		public KrakenStatue( Serial serial ) : base( serial )
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
