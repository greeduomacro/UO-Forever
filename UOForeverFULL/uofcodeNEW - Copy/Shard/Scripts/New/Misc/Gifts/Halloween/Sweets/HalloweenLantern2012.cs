// Original Author Unknown
// Updated to be halloween 2012 by boba

using System;
using Server;
using Server.Items;

namespace Server.Items
{  
	public class HalloweenLantern2012 : Lantern
	{
           	[Constructable]
           	public HalloweenLantern2012()
           	{
           		Name = "a Spooky Lantern";
			Hue = Utility.RandomList( m_Hues );
			LootType = LootType.Blessed;
           	}

           	[Constructable]
           	public HalloweenLantern2012(int amount)
           	{
           	}

           	public HalloweenLantern2012(Serial serial) : base( serial )
           	{
           	}

		private static int[] m_Hues = new int[]
		{
			0xF3,
			0x497,
		};

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( "Halloween 2012" );
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
