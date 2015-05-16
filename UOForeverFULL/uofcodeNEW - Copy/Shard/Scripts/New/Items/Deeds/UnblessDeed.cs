using System;
using Server.Prompts;
using Server.Targeting;

namespace Server.Items
{
	public class UnblessTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private UnblessDeed m_Deed;

		public UnblessTarget( UnblessDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( m_Deed == null || m_Deed.Deleted )
				return;

			BaseClothing item = target as BaseClothing;

			if (item == null || item.RootParent != from || item.LootType != LootType.Blessed || item.BlessedFor != from || (Mobile.InsuranceEnabled && item.EraAOS && item.Insured)) // Check if its already newbied (blessed)
				from.SendMessage( "You cannot remove the blessing from that object." ); // That item is already blessed
			else if ( !m_Deed.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1045156 );
			else
			{
				item.LootType = LootType.Regular;
				from.SendMessage( "You unblessed the item...." ); // You bless the item....

				m_Deed.Delete(); // Delete the bless deed
			}
		}
	}

	public class UnblessDeed : Item // Create the item class which is derived from the base item class
	{
		public override string DefaultName{ get{ return "a clothing bless removal deed"; } }

		[Constructable]
		public UnblessDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			//LootType = LootType.Newbied;
		}

		public UnblessDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "What item would you like to unbless?" ); // What item would you like to bless? (Clothes Only)
				from.Target = new UnblessTarget( this ); // Call our target
			}
		}
	}
}