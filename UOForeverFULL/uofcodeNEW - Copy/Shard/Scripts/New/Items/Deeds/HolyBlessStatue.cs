using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class HolyBlessStatueTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private HolyBlessStatue m_Deed;

		public HolyBlessStatueTarget( HolyBlessStatue deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( m_Deed == null || m_Deed.Deleted || m_Deed.RootParent != from )
				return;

			BaseClothing item = target as BaseClothing;

			if ( item == null || item.RootParent != from || item is HoodedShroudOfShadows || item is ElvenRobe )
				from.SendLocalizedMessage( 500509 ); // You cannot bless that object
			else if (item.LootType != LootType.Regular || item.BlessedFor != null || (Mobile.InsuranceEnabled && item.EraAOS && item.Insured)) // Check if its already newbied (blessed)
				from.SendLocalizedMessage( 1045113 ); // That item is already blessed
			else if ( !item.CanBeBlessed || item.RootParent != from )
				from.SendLocalizedMessage( 500509 ); // You cannot bless that object
			else
			{
				item.LootType = LootType.Blessed;
				item.Hue = 1154;
				item.Name = String.Format( "{0} blessed by a holy angel", String.IsNullOrEmpty( item.Name ) ? ( String.IsNullOrEmpty( item.DefaultName ) ? item.ItemData.Name : item.DefaultName ) : item.Name );
				from.SendLocalizedMessage( 1010026 );

				m_Deed.Consume(); // Delete the bless deed
			}
		}
	}

	public class HolyBlessStatue : Item
	{
		public override string DefaultName{ get{ return "statue of a holy angel"; } }

		[Constructable]
		public HolyBlessStatue() : base( 0x1226 )
		{
			Hue = 1154;
			Weight = 15.0;
			LootType = LootType.Cursed;
		}

		public HolyBlessStatue( Serial serial ) : base( serial )
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

		public override bool DisplayLootType{ get{ return true; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendLocalizedMessage( 1005018 ); // What item would you like to bless? (Clothes Only)
				from.Target = new HolyBlessStatueTarget( this ); // Call our target
			 }
		}
	}
}