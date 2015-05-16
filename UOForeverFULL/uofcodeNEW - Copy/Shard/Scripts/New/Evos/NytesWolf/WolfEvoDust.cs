//Script Modified By: Sexy-Vampire
using System; 
using Server.Items; 

namespace Server.Items 
{ 
   	public class NytesWolfDust: Item 
   	{ 
		[Constructable]
		public NytesWolfDust() : this( 1 )
		{
		}

		[Constructable]
		public NytesWolfDust( int amount ) : base( 0x26B8 )
		{
			Stackable = true;
			Weight = 0.0;
			Amount = amount;
			Name = "Wolf Dust";
			Hue = 1150;
		}

            	public NytesWolfDust( Serial serial ) : base ( serial ) 
            	{             
           	} 

           	public override void Serialize( GenericWriter writer ) 
           	{ 
              		base.Serialize( writer ); 
              		writer.Write( (int) 0 ); 
           	} 
            
           	public override void Deserialize( GenericReader reader ) 
           	{ 
              		base.Deserialize( reader ); 
              		int version = reader.ReadInt(); 
           	} 
        } 
} 