using System;
using Server; 

namespace Server.Items 
{ 
	public class Cocoon : Item
	{ 
		[Constructable]
		public Cocoon() : base( 0x10DA )
		
		{
			Weight = 5;
		     Name = "a cocoon";

		} 

		public Cocoon ( Serial serial ) : base( serial )
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
