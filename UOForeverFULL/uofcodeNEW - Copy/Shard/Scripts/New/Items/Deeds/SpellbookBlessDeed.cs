using System;
using Server.Network;
using Server.Prompts;
using Server.Targeting;

namespace Server.Items
{
	public class SpellbookBlessTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private SpellbookBlessDeed m_Deed;

		public SpellbookBlessTarget( SpellbookBlessDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( m_Deed.Deleted || m_Deed.RootParent != from )
				return;

			if ( target is Spellbook )
			{
				Spellbook item = (Spellbook)target;

				if (item.LootType == LootType.Blessed || item.BlessedFor == from || (Mobile.InsuranceEnabled && item.EraAOS && item.Insured)) // Check if its already newbied (blessed)
					from.SendLocalizedMessage( 1045113 ); // That item is already blessed
				else if ( item.LootType == LootType.Cursed )
					from.SendLocalizedMessage( 1045114 ); // You can not bless that item
				else if ( item.RootParent != from )
					from.SendLocalizedMessage( 500509 ); // You cannot bless that object
				else
				{
					item.LootType = LootType.Blessed;
					from.SendLocalizedMessage( 1010026 ); // You bless the item....

					m_Deed.Consume(); // Delete the bless deed
				}
			}
			else
				from.SendLocalizedMessage( 500509 ); // You cannot bless that object
		}
	}

	public class SpellbookBlessDeed : Item // Create the item class which is derived from the base item class
	{
		public override string DefaultName{ get { return "a spellbook bless deed"; } }

		[Constructable]
		public SpellbookBlessDeed() : base( 0x14F0 )
		{
			Hue = 184;
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public SpellbookBlessDeed( Serial serial ) : base( serial )
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
			LootType = LootType.Blessed;

			int version = reader.ReadEncodedInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "What would you like to bless? (Spellbooks Only)" );
				from.Target = new SpellbookBlessTarget( this ); // Call our target
			}
		}
	}
}