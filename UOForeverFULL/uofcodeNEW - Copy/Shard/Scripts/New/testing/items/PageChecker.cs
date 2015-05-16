using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Help;


namespace Server.Items
{

	public class PageChecker : Item
	{
		[Constructable]
		public PageChecker() : this( null )
		{
		}

		[Constructable]
		public PageChecker ( string name ) : base ( 0x2258 )
		{
			Name = "Test Note";
			LootType = LootType.Blessed;
			Hue = 1265;
		}

                public override void OnDoubleClick( Mobile from )
		

                  {
	                 
                              from.SendGump( new PageQueueGump() );
                      
          
         
                 }


		public PageChecker ( Serial serial ) : base ( serial )
		{
		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}
       
