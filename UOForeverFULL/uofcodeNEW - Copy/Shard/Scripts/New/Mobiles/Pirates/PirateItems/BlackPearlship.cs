using System;
using Server; 

namespace Server.Items 
{ 
	public class BlackPearlShip : Item
	{ 
		[Constructable]
		public BlackPearlShip() : base( 0x14F3 )
		
		{
			Weight = 5;
			Hue = 2075;
		     Name = "Black Pearl (Deco)";

		} 

		public BlackPearlShip ( Serial serial ) : base( serial )
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
