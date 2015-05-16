using System;
using Server; 

namespace Server.Items 
{ 
	public class MediumWebSouth : Item
	{ 
		[Constructable]
		public MediumWebSouth() : base( 0x10D5 )
		
		{
			Weight = 5;
		     Name = "a web";

		} 

		public MediumWebSouth ( Serial serial ) : base( serial )
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
