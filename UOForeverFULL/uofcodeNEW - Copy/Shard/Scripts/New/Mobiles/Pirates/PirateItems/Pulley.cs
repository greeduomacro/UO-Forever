using System;
using Server; 

namespace Server.Items 
{ 
	public class Pulley : Item
	{ 
		[Constructable]
		public Pulley() : base( 0x1E9C )
		
		{
			Weight = 5;
		     Name = "a Pulley";

		} 

		public Pulley ( Serial serial ) : base( serial )
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
