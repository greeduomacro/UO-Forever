using System;
using Server; 

namespace Server.Items 
{ 
	public class SmallWebSouth : Item
	{ 
		[Constructable]
		public SmallWebSouth() : base( 0x10D6 )
		
		{
			Weight = 5;
		     Name = "a web";

		} 

		public SmallWebSouth ( Serial serial ) : base( serial )
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
