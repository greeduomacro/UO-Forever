using System;
using Server; 

namespace Server.Items 
{ 
	public class HalloweenCat : Item
	{ 
		[Constructable]
		public HalloweenCat() : base( 0x4688 )
		
		{
			Weight = 5;
		     Name = "a rare Halloween Cat";

		} 

		public HalloweenCat ( Serial serial ) : base( serial )
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
