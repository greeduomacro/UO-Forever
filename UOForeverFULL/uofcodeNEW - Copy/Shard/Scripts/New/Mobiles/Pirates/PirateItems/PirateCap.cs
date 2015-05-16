using System;
using Server; 

namespace Server.Items 
{ 
	public class PirateCap : Item
	{ 
		[Constructable]
		public PirateCap() : base( 0x1544)
		
		{
			Weight = 5;
			Hue= 2075;
		     Name = "a rare Pirates Cap";

		} 

		public PirateCap ( Serial serial ) : base( serial )
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
