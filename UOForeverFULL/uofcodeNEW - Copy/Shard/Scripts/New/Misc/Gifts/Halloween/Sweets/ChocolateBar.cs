using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class ChocolateBar : Item
  {


      
      [Constructable]
		public ChocolateBar()
			: base(3978)
		{
          Name = "A Bar of Chocolate";
          Hue = 1504;
		}
		
		public override void OnDoubleClick( Mobile m )
		{
		  m.SendMessage( 48, "You feel more energetic as you eat the sweet." );
		  m.Stam = m.Stam + 85;
		  this.Delete();
		}

		public ChocolateBar( Serial serial ) : base( serial )
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
