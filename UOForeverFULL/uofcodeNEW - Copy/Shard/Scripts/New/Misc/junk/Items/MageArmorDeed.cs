using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class MageArmorTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private MageArmorDeed m_Deed;

		public MageArmorTarget( MageArmorDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			/*if ( target is BaseArmor )
			{
				Item item = (Item)target;

				
				if ( ((BaseArmor)item).ArmorAttributes.MageArmor == 1 )
				{
					from.SendMessage( "That already is mage armor!" ); // Check to see if item already has mage armor
				}
				else
				{
					if( item.RootParent != from ) // Make sure its in their pack or they are wearing it
					{
						from.SendMessage( "You cannot make that mage armor there!" ); 
					}
					else
					{
						((BaseArmor)item).ArmorAttributes.MageArmor = 1;
						from.SendMessage( "You make your armor mage armor..." );

						m_Deed.Delete(); // Delete the deed
					}
				}
			}
			else
			{
				from.SendMessage( "You cannot make that mage armor!" );
			}*/
		}
	}

	public class MageArmorDeed : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public MageArmorDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "a mage armor deed";
			LootType = LootType.Blessed;
			Hue = 0x492;
		}

		public MageArmorDeed( Serial serial ) : base( serial )
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
                from.SendMessage("This deed appears to be a counterfeit.  You been had.");
                //from.SendMessage("What item would you like to make mage armor?"  );
				//from.Target = new MageArmorTarget( this ); // Call our target
			 }
		}	
	}
}


