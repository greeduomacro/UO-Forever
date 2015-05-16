using System;
using Server; 

namespace Server.Items 
{ 
	public class SmallWebEast : Item
	{ 
		[Constructable]
		public SmallWebEast() : base( 0x10D7 )
		
		{
			Weight = 5;
		     Name = "a web";

		} 

		public SmallWebEast ( Serial serial ) : base( serial )
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
