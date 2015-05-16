using System; 
using Server.Network; 
using Server.Prompts; 
using Server.Items; 
using Server.Mobiles;
using Server.Targeting; 

namespace Server.Items 
{ 
   public class InvulTarget : Target 
   { 
      private PetInvulDeed m_Deed; 

      public InvulTarget( PetInvulDeed deed ) : base( 1, false, TargetFlags.None ) 
      { 
         m_Deed = deed; 
      } 

      protected override void OnTarget( Mobile from, object target ) 
      { 
         if ( target is BaseCreature ) 
         { 
            BaseCreature t = ( BaseCreature ) target; 

            if ( t.Blessed == true )
            { 
               from.SendMessage( "That pet is already Invulnerable!" );
            } 
            else if ( t.ControlMaster != from ) 
            { 
               from.SendMessage( "That is not your pet!" ); 
            } 
            else  
             
               { 

			t.Blessed = !t.Blessed;
			from.SendMessage( "You make the pet Invulnerablet!" );

                  m_Deed.Delete(); // Delete the deed 
               } 
            
         } 
         else 
         { 
            from.SendMessage( "That is not a valid traget." );  
         } 
      } 
   } 

   public class PetInvulDeed : Item // Create the item class which is derived from the base item class 
   { 
      [Constructable] 
      public PetInvulDeed() : base( 0x14F0 ) 
      { 
         Weight = 1.0; 
         Name = "a pet invuln deed"; 
         LootType = LootType.Blessed; 
	   Hue = 1174;
      } 

      public PetInvulDeed( Serial serial ) : base( serial ) 
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
         LootType = LootType.Blessed; 

         int version = reader.ReadInt(); 
      } 

      public override bool DisplayLootType{ get{ return false; } } 

      public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target 
      { 
         if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack 
         { 
             from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it. 
         } 
         else 
         { 
            from.SendMessage( "Choose the pet you wish to make invulnerable." );  
            from.Target = new InvulTarget( this ); // Call our target 
          } 
      }    
   } 
}