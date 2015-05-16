using System;
using Server;

namespace Server.Items
{
   public class GeryonCowHide : Item
   {
      [Constructable]
      public GeryonCowHide() : base( 0x1078 )
      {
 		Name = "Geryon's Cow Hide";
 		Hue = 0x26;
        Weight = 0.1;
      }
      
      public GeryonCowHide( Serial serial ) : base( serial )
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