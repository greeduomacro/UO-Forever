using System;
using Server; 

namespace Server.Items 
{ 
	public class DavyJonesOar : Item
	{ 
		[Constructable]
		public DavyJonesOar() : base( 0x1E29)
		
		{
			Weight = 5;
			Hue = 1150;
		     Name = "Davy Jones Oar";

		} 

		public DavyJonesOar ( Serial serial ) : base( serial )
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
