using System;
using Server; 

namespace Server.Items 
{ 
	public class PirateCoffin : Item
	{ 
		[Constructable]
		public PirateCoffin() : base( 0x3F0E )
		
		{
			Weight = 5;
			Hue = 2075;
		     Name = "a Unknown Pirate Coffin";

		} 

		public PirateCoffin ( Serial serial ) : base( serial )
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
