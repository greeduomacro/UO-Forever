using System;
using Server; 

namespace Server.Items 
{ 
	public class SlipKnot : Item
	{ 
		[Constructable]
		public SlipKnot() : base( 0x1EA0 )
		
		{
			Weight = 5;
		     Name = "a SlipKnot";

		} 

		public SlipKnot ( Serial serial ) : base( serial )
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
