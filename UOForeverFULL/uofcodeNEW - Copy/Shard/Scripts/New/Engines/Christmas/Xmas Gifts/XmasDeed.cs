using System;
using Server.Gumps;

namespace Server.Items
{
   	public class XmasDeed : BaseHolidayGift
   	{
      	[Constructable]
      	public XmasDeed(int year)
      	{
            ItemID = 0x14F0;
         	Movable = true;
			Hue = Utility.RandomList( m_Hues );
         	Name = "A Gift Certificate";
            HolidayName = "Christmas";
            HolidayYear = year;
			LootType = LootType.Blessed;
      	}

		private static int[] m_Hues = new int[]
		{
			0x6,
			0xB,
			0x10,
			0x15,
			0x1A,
			0x1F,
			0x24,
			0x29,
			0x2E,
			0x33,
			0x38,
			0x3D,
			0x42,
			0x47,
			0x4C,
			0x51,
			0x56,
			0x5B,
			0x60,
			0x65,
		};

      	public override void OnDoubleClick( Mobile from )
      	{
         		if (!IsChildOf(from.Backpack)) 
         		{ 
            		from.SendLocalizedMessage( 1042010 ); //You must have the object in your backpack to use it. 
            		return; 
         		}

			else
			{
      			from.SendGump( new XmasDeedGump(from, this) );
				//this.Delete();
			}
      	}

      	public XmasDeed( Serial serial ) : base( serial )
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
