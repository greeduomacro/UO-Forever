using System;
using Server; 

namespace Server.Items 
{ 
	public class Hook : Item
	{ 
		[Constructable]
		public Hook() : base( 0x1E9A )
		
		{
			Weight = 5;
		     Name = "a Hook";

		} 

		public Hook ( Serial serial ) : base( serial )
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
