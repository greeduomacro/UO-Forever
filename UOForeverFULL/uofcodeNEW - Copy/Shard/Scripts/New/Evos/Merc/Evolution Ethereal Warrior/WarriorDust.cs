//Script Modified By: Crows Kingdom Shard
using System; 
using Server.Items; 

namespace Server.Items 
{ 
   	public class WarriorDust: Item 
   	{ 
		[Constructable]
		public WarriorDust() : this( 1 )
		{
		}

		[Constructable]
		public WarriorDust( int amount ) : base( 0x26B8 )
		{
			Stackable = true;
			Weight = 0.0;
			Amount = amount;
			Name = "warrior's dust";
			Hue = 48;
		}

            	public WarriorDust( Serial serial ) : base ( serial ) 
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