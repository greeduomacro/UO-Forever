using System;

namespace Server.Items
{
	public interface IShipwreckedItem
	{
		bool IsShipwreckedItem { get; set; }
	}

	public class ShipwreckedItem : Item, IShipwreckedItem
	{
		public ShipwreckedItem( int itemID ) : base( itemID )
		{
			int weight = this.ItemData.Weight;

			if ( weight >= 255 )
				weight = 1;

			this.Weight = weight;
			Dyable = true;
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelToExpansion(from);

			this.LabelTo( from, 1050039, String.Format( "#{0}\t#1041645", LabelNumber ) );
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1041645 ); // recovered from a shipwreck
		}

		public ShipwreckedItem( Serial serial ) : base( serial )
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

		public override bool DisplayDyable{ get{ return false; } }

		public override bool Dye( Mobile from, IDyeTub sender )
		{
			if ( Deleted )
				return false;

			if ( ItemID >= 0x13A4 && ItemID <= 0x13AE )
				return base.Dye( from, sender );

			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		#region IShipwreckedItem Members

		public bool IsShipwreckedItem
		{
			get
			{
				return true;	//It's a ShipwreckedItem item.  'Course it's gonna be a Shipwreckeditem
			}
			set
			{
			}
		}

		#endregion
	}
}