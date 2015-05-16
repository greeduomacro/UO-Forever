using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class BandageBag : Bag 
   { 
      [Constructable] 
      public BandageBag() : this( 1 ) 
      { 
		  Movable = true; 
		  Hue = 1150; 
		  Name = "a bandage bag";
      } 
	   [Constructable]
	   public BandageBag( int amount )
	   {
		   DropItem( new Bandage( 200 ) );
	   }
		

      public BandageBag( Serial serial ) : base( serial ) 
      { 
      } 

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 

         writer.Write( (int) 0 ); // version 
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 

         int version = reader.ReadInt(); 
      } 
   } 
} 
