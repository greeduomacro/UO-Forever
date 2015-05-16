using System;
using Server.Network;
using Server.Prompts;
using Server.Targeting;

namespace Server.Items
{
	public class SpellKnowledgeScrollTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private SpellKnowledgeScroll m_Deed;

		public SpellKnowledgeScrollTarget( SpellKnowledgeScroll deed ) : base( 1, false, TargetFlags.None )
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

				ulong maxcontent = 0;

				if ( item.BookCount == 64 )
					maxcontent = ulong.MaxValue;
				else
					maxcontent = (1ul << item.BookCount) - 1;

				if ( item.Content == maxcontent )
					from.SendMessage( "The item already has much magical knowledge." );
				else if ( item.LootType == LootType.Cursed )
					from.SendMessage( "The evil within prevents imbuing." );
				else if ( item.RootParent != from )
					from.SendMessage( "You cannot imbue that object." );
				else
				{
					item.Content = maxcontent;
					from.SendMessage( "You imbue the item with the knowledge from a Grandmaster of the art of magic." );

					m_Deed.Consume(); // Delete the bless deed
				}
			}
			else
				from.SendMessage( "You cannot imbue that object." );
		}
	}

	public class SpellKnowledgeScroll : Item // Create the item class which is derived from the base item class
	{
		public override string DefaultName
		{
			get { return "a scroll of magical knowledge"; }
		}

		[Constructable]
		public SpellKnowledgeScroll() : base( 0xE34 )
		{
			Hue = 144;
			Weight = 1.0;
		}

		public SpellKnowledgeScroll( Serial serial ) : base( serial )
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

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "What would you like to imbue? (Spellbooks Only)" );
				from.Target = new SpellKnowledgeScrollTarget( this ); // Call our target
			}
		}
	}
}