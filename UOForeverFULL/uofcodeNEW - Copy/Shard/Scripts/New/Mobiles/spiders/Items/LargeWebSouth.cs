using System;
using Server; 

namespace Server.Items 
{ 
	public class LargeWebSouth : Item
	{ 
		[Constructable]
		public LargeWebSouth() : base( 0x10D2 )
		
		{
			Weight = 5;
		     Name = "a web";

		} 

		public LargeWebSouth ( Serial serial ) : base( serial )
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
