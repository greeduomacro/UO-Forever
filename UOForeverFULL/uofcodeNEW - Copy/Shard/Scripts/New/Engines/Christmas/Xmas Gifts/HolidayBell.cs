using System;
using Server.Multis;
using Server.Mobiles;

namespace Server.Items 
{
   	public class HolidayBell : BaseHolidayGift 
   	{
		public override bool IsAccessibleTo( Mobile m )
		{
			if ( !BaseHouse.CheckAccessible( m, this ) )
				return true;

			return base.IsAccessibleTo( m );
		}

		private int m_BellSound;

		public static T RandomList<T>( T[] list )
		{
     			return list[Utility.Random( list.Length )];
		}

      	[Constructable] 
      	public HolidayBell(int year) 
      	{
            ItemID = 0x1C12;

            HolidayName = "Christmas";
            HolidayYear = year;

      		m_BellSound = Utility.RandomList( m_BellSoundList ); 
      		Stackable = false; 

      		Weight = 0.1;
 
      		Name = "a holiday bell from " + RandomList<string>( m_BellNames );

			Hue = Utility.RandomList( m_Hues );
      		LootType = LootType.Blessed; 
      	}

		private static string[] m_BellNames = new string[]
		{
			"Owner Shane",
			"Dev Alan",
            "Dev Adam",
			"Admin Carl",

		};

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
			0x47E,
			0x497,
		};

		private static int[] m_BellSoundList = new int[]
		{
      		245,
			246,
			247,
			248,
			249,
			250,
			251,
			252,
			253,
			254,
			255,
			256,
			257,
			258,
			259,
			260,
			261,
			262,
		};

      	public HolidayBell( Serial serial ) : base( serial ) 
      	{ 
      	}

      	public override void OnDoubleClick( Mobile from ) 
      	{ 
         		Effects.PlaySound( from, from.Map, m_BellSound ); 
      	} 

      	public override void Serialize( GenericWriter writer ) 
      	{ 
         		base.Serialize( writer ); 

         		writer.Write( (int) 0 ); // version 

         		writer.Write( (int) m_BellSound ); 
      	} 

      	public override void Deserialize( GenericReader reader ) 
      	{ 
         		base.Deserialize( reader ); 

         		int version = reader.ReadInt(); 

         		m_BellSound = reader.ReadInt(); 
      	} 
   	} 
} 
