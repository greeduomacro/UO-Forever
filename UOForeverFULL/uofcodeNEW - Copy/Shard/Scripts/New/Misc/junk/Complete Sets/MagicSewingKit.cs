using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Engines.Craft;
using System.Collections;
using Server;


namespace Server.Items
{
	public class MagicSewingKitTarget : Target
	{
		private MagicSewingKit m_Deed;

		public MagicSewingKitTarget( MagicSewingKit deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}
		
		protected override void OnTarget( Mobile from, object target )
		{
			if ( m_Deed.Deleted )
				return;
            from.SendMessage("It didn't do anything! You were ripped off!");
            /*
			if ( target is Robe || target is Cloak || target is HoodedShroudOfShadows )
			{
				Item item = (Item)target;

				if ( item.PhysicalResistance >= 1 ) // Check if its has resists
				{
					from.SendMessage( "That is not possible" );
				}
				else
				{
					if( item.RootParent != from ) // Make sure its in their pack or they are wearing it
					{
						from.SendMessage( "Make sure the item is in your backpack" );
					}
					else 
					{
						BaseClothing clothing = target as BaseClothing;

						clothing.Resistances.Physical += 3;
						from.SendMessage( "You use your kit!" );
						m_Deed.Delete(); // Delete the sewing kit
					}
					
				}
			}
			else
			{
				from.SendMessage( "You cannot sew that object" );
			}*/
		}
	}

	public class MagicSewingKit : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public MagicSewingKit() : base( 0xF9D )
		{
			Weight = 1.0;
			Name = "a magic sewing kit";
			LootType = LootType.Cursed;
			Hue = 1266;	
		}

		public MagicSewingKit( Serial serial ) : base( serial )
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
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage( "What item would you like to sew?" ); 
				from.Target = new MagicSewingKitTarget( this ); // Call our target
			 }
		}	
	}
}