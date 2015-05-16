using System;
using Server; 

namespace Server.Items 
{ 
	public class EggCase : Item
	{ 
		[Constructable]
		public EggCase() : base( 0x10D9 )
		
		{
			Weight = 5;
		     Name = "egg case";

		} 

		public EggCase ( Serial serial ) : base( serial )
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
