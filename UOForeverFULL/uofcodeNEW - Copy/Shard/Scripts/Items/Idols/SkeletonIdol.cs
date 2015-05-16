using System; 
using Server.Network; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Items 
{ 
   public class SkeletonIdol : Item 
   { 
      [Constructable] 
      public SkeletonIdol() : base( 9660 ) 
      { 
         Name = "Skeleton Idol"; 
		 this.Movable = true;
		 this.Stackable = false; 
      } 
	  	
      public SkeletonIdol( Serial serial ) : base( serial ) 
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
      }}}