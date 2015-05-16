using System;
using Server; 

namespace Server.Items 
{ 
	public class Bindingsofdavyjones : Item
	{ 
		[Constructable]
		public Bindingsofdavyjones() : base( 0x14F8 )
		
		{
			Weight = 5;
			Hue = 1150;
		     Name = "Davy Jones Bindings";

		} 

		public Bindingsofdavyjones ( Serial serial ) : base( serial )
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
