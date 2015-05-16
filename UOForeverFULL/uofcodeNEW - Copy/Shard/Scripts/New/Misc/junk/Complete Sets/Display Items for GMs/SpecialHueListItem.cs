using Server;
using System;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public class SpecialHueListItemTarget : Target
	{
		private Item m_Tub;
		private int theHue;

		public SpecialHueListItemTarget( SpecialHueListItem dyetub ) : base( 12, false, TargetFlags.None )
		{
			m_Tub = dyetub;
			theHue = (int) dyetub.Hue;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( targeted is Item )
			{
				Item item = (Item) targeted;
				item.Hue = theHue;
			}
			else from.SendMessage("You cannot dye that.");
		}
	}

	public class SpecialHueListItem : Item
	{
		[Constructable]
		public SpecialHueListItem(int gen1hue) : this( gen1hue, 0xFAB ) {}

		[Constructable]
		public SpecialHueListItem(int genhue, int itemid) : base( itemid )
		{
			Weight = 0.0;
			ItemID = itemid;
			Hue = genhue;
			Name = "Special Hue List Item " + Convert.ToString(Hue);
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.SendMessage( "Select the item to dye" );
				from.Target = new SpecialHueListItemTarget( this );
			}
			else from.SendLocalizedMessage( 500446 );
		}

		public SpecialHueListItem( Serial serial ) : base( serial ) { }
		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( (int) 0 ); }
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt(); }
	}
}