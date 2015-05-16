using System;
using Server;
using Server.Misc;
using Server.Mobiles;
using Server.Multis.Deeds;

namespace Server.Items
{
	public class ForeverWelcomeBag : Bag
	{
		[Constructable]
		public ForeverWelcomeBag() : base()
		{
			Name = "Forever Welcome Bag";
			Hue = 196;

			Item item = new MiniatureHorse();
			DropItem( item );
			item.X = 30;
			item.Y = 76;

			item = new BagOfReagents( 50 );
			DropItem( item );
			item.X = 71;
			item.Y = 55;

			Container bag = new Bag();
			bag.DropItem( new LeatherCap() );
			bag.DropItem( new LeatherChest() );
			bag.DropItem( new LeatherLegs() );
			bag.DropItem( new LeatherGloves() );
			bag.DropItem( new LeatherArms() );
			bag.DropItem( new LeatherGorget() );
			DropItem( bag );
			bag.X = 63;
			bag.Y = 75;

				item = new FireworksWand();
				item.Name = "Launch Day 2013";
				DropItem( item );
				item.X = 94;
				item.Y = 34;
			
				/*item = new BankCheck( 1000 );
				DropItem( item );
				item.X = 52;
				item.Y = 36;*/

				item = new HalfApron(); 
				item.Name = "Launch Day 2013";
				//item.loottype = Blessed;
				DropItem( item );
				item.X = 23;
				item.Y = 53;

			/*	if ( 1.00 > Utility.RandomDouble() )// 2 percent - multipy number x 100 to get percent

                switch (Utility.Random(5))
                {
				
				case 0:
                PackItem = new Skirt(); 
				Name = "Launch Day 2013";

				case 1:
                Item = new Boots(); 
				Name = "Launch Day 2013";
				DropItem( item );
				item.X = 23;
				item.Y = 53;

			    case 2:
                Item = new StrawHat(); 
				Name = "Launch Day 2013";
				DropItem( item );
				item.X = 23;
				item.Y = 53;

				case 3:
                Item = new Bandana(); 
				Name = "Launch Day 2013";
				DropItem( item );
				item.X = 23;
				item.Y = 53;

		        case 4:*/
               
                }
	

		public ForeverWelcomeBag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}