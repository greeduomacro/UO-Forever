using System;
using Server; 

namespace Server.Items 
{ 
	public class Fountainofyouth : Item
	{ 
		[Constructable]
		public Fountainofyouth() : base( 0x3F10 )
		
		{
			Weight = 5;
		     Name = "Fountain of Youth";

		} 

		public Fountainofyouth ( Serial serial ) : base( serial )
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
