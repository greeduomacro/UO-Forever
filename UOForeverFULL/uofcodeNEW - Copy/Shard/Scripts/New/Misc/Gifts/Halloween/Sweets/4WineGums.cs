using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class WineGums : Item
  {


      
      [Constructable]
		public WineGums()
			: base(9451)
		{
          Name = "BubbleGum";
          Hue = 1172;
		}
		
		public override void OnDoubleClick( Mobile m )
		{
		  m.SendMessage( 48, "You feel more energetic as you eat the sweet." );
		  m.Stam = m.Stam + 35;
		  this.Delete();
		}

		public WineGums( Serial serial ) : base( serial )
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
