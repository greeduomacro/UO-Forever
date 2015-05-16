/*
Script Name: GoldBar.cs
Author: Shadow1980
Version: 1.0
Public Release: 05/07/05
Purpose:  A more effective way of storing huge amounts of gold.

Description: 		Gold bars are an effective way of storing large amounts of gold for players.
			Basically when attempting to create a bank check larger then 500,000 gp, a
			gold bar will be created for every 500,000 gp. Gold bars when double clicked
			in a players bank box will change into a 500,000 gp check again. The advantage
			of gold bars to players is that gold bars are stackable, where checks are not.
			(Because checks have variable values you can't really make them stackable)
			Rather then increasing the size checks can be, this system provides a more 
			realistic storage method.

Acknowledgements: 	Thanks to Admin Vorspire for releasing an entirely different gold bar system that 
			brought this idea into my head. 

Installation:		Put GoldBar.cs in your Custom folder.
			Replace your Scripts\Mobiles\Townfolk\Banker.cs with the one below.
			(For those who already modified their bankers, the changes have been commented and
			are very easely inserted.)
*/ 

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x1BE9, 0x1BEC )]
	public class GoldBar : Item 
	{
		[Constructable]
		public GoldBar() : this (1)
		{
		}

		[Constructable]
		public GoldBar(int amount) : base( 0x1BE9 )
		{
			Stackable = true;
			Amount = amount;
			Weight = 5.0;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			list.Add( Amount == 1 ? "{0} Gold Bar" : "{0} Gold Bars", Amount );
		}
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties( list );

			list.Add( 1060738, "{0}", Amount * 500000 ); // value: ~1_val~
		}
		public override void OnSingleClick( Mobile from )
		{
			LabelToExpansion(from);

			LabelTo( from, Amount == 1 ? "{0} Gold Bar" : "{0} Gold Bars", Amount );
		}

		/*public override Item Dupe( int amount )
		{
			return base.Dupe( new GoldBar( amount ), amount );
		}here for ruo rc1 and maybe rc2*/

		public override void OnDoubleClick( Mobile from )
		{
			BankBox box = from.BankBox;

			if ( box != null && IsChildOf( box ) )
			{
				PlayerMobile pm = from as PlayerMobile;

				base.OnDoubleClick( from );
				
				BankCheck check;

				if( this.Amount == 1 )
				{
					check = new BankCheck( 500000 );
					
					this.Delete();

					if ( box.TryDropItem( pm, check, false ) )
					{
						pm.SendMessage("You return your gold bar to the bank and receive a 500,000 gp check.");
					}
					else
					{
						check.Delete();
						pm.AddToBackpack( new BankCheck( 500000 ) );
						pm.SendMessage("You return your gold bar to the bank and receive a 500,000 gp check.");
					}
				}
				else if( this.Amount >= 2 )
					{
					check = new BankCheck( 500000 );
                                
					if ( box.TryDropItem( pm, check, false ) )
					{
						this.Amount -= 1;
						pm.SendMessage("You return your gold bar to the bank and receive a 500,000 gp check.");
					}
					else
					{
						check.Delete();
						pm.SendMessage("There is not enough room in your bank box for the check.");
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 1047026 ); // That must be in your bank box to use it.
			}
		}

		public GoldBar(Serial serial) : base (serial)
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
	}
}