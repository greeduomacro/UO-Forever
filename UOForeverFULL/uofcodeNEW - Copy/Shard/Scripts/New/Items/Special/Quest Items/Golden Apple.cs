using System;
using Server;

namespace Server.Items
{
   public class GoldenApple : Item
   {
      [Constructable]
      public GoldenApple() : base( 0x9D0 )
      {
      	Name = "A Golden Apple";
		Hue = 0x499;
        Weight = 0.1;
      }
      
      public GoldenApple( Serial serial ) : base( serial )
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