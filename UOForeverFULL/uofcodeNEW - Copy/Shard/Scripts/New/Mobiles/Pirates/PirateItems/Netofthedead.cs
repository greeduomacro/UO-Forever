using System;
using Server; 

namespace Server.Items 
{ 
	public class Netofthedead : Item
	{ 
		[Constructable]
		public Netofthedead() : base( 0x1EA6 )
		
		{
			Weight = 5;
			Hue = 2075;
		     Name = "Net of the Dead";

		} 

		public Netofthedead ( Serial serial ) : base( serial )
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
