using System;
using Server; 

namespace Server.Items 
{ 
	public class BlackPearlOars : Item
	{ 
		[Constructable]
        public BlackPearlOars()
            : base(0x1E2B)
		
		{
			Weight = 5;
			Hue = 2075;
		     Name = "Oars of the Black Pearl";

		} 

		public BlackPearlOars ( Serial serial ) : base( serial )
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
