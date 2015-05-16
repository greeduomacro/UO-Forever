using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Network;
using VitaNex.FX;
using VitaNex.Items;
using VitaNex.Network;

namespace Server.Items
{
	[Furniture]
	public class DonationChest : BaseContainer
	{
        public override int DefaultMaxWeight { get { return 2000000000; } }

        public override bool IsDecoContainer
        {
            get { return false; }
        }

		[Constructable]
		public DonationChest() : base( 0x4102 )
		{
		    Name = "Donation Chest";
			Weight = 1.0;
			Dyable = false;
		    Movable = false;
		    GumpID = 266;
		}

		public override void DisplayTo( Mobile m )
		{
			if ( DynamicFurniture.Open( this, m ) )
				base.DisplayTo( m );
		}

		public DonationChest( Serial serial ) : base( serial )
		{
		}

	    public override void OnDoubleClick(Mobile from)
	    {
	        if (from.AccessLevel < AccessLevel.Administrator)
	        {
	            from.SendGump(new DonationChestGump());
	            DynamicFurniture.Open(this, from); 
	            return;
	        }
	        base.OnDoubleClick(from);
	    }

	    public override bool OnDragDrop(Mobile from, Item dropped)
	    {
	        if (dropped is Gold || dropped is BankCheck || dropped is DonationCoin)
	        {
                from.SendMessage(54, "Thank you for your donation!");
	            dropped.Movable = false;
	            return base.OnDragDrop(from, dropped);
	        }
	        else
	        {
                from.SendMessage(54, "You may only donate gold, bank checks or donation coins to this chest.");
	            return false;
	        }
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

			DynamicFurniture.Close( this );
		}
	}

}