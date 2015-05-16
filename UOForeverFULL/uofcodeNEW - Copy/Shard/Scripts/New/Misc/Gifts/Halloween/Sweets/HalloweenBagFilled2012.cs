// Original Author Unknown
// Updated to be halloween 2012 by Boba

using System;
using Server;
using Server.Items;

namespace Server.Items
{  
	public class HalloweenBagFilled2012 : Bag
	{
           	[Constructable]
           	public HalloweenBagFilled2012()
           	{
           		Name = "Have A Spooky Halloween 2012";
			Hue = 243;

			DropItem (new HalloweenLantern2012() );

			switch ( Utility.Random( 4 ) )
			{      	
				case 0: DropItem(new DecoPumpkin1());
				break;

				case 1: DropItem(new DecoPumpkin2());
				break;

				case 2: DropItem(new CandyBag());
				break;

				case 3: DropItem(new DecoScareCrow());
				break;
			}

			if ( 0.1 > Utility.RandomDouble() )
			{
				DropItem( new HalloweenRobe2012() );
			}

           	}

           	[Constructable]
           	public HalloweenBagFilled2012(int amount)
           	{
           	}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( "Halloween 2012" );
		}

           	public HalloweenBagFilled2012(Serial serial) : base( serial )
           	{
           	}

          	public override void Serialize(GenericWriter writer)
          	{
           		base.Serialize(writer);

           		writer.Write((int)0); // version 
     		}

           	public override void Deserialize(GenericReader reader)
      	{
           		base.Deserialize(reader);

          		int version = reader.ReadInt();
           	}
	}
}
