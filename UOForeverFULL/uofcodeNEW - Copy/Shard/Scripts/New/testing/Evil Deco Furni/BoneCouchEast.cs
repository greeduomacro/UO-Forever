

using System;
using Server;

namespace Server.Items
{
   public class BoneCouchEast : BaseAddon
   {
      public override BaseAddonDeed Deed{ get{ return new BoneCouchEastDeed(); } }
public override bool RetainDeedHue{ get{ return true; } }
      [Constructable]
      public BoneCouchEast()
      {

         AddComponent( new AddonComponent( 10880 ), 0, 0, 0 );
         AddComponent( new AddonComponent( 10879 ), 0, 1, 0 );

                     
           
      }

      public BoneCouchEast( Serial serial ) : base( serial )
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

   public class BoneCouchEastDeed : BaseAddonDeed
   {
      public override BaseAddon Addon{ get{ return new BoneCouchEast(); } }
      
      [Constructable]
      public BoneCouchEastDeed()
      {
Name = "Bone Couch East Deed";
   
}

      public BoneCouchEastDeed( Serial serial ) : base( serial )
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
