#region References
using System;

using Server.Commands;
using Server.Guilds;
using Server.Items;
using Server.Multis;
using Server.Network;
#endregion

namespace Server.Gumps
{
	public class HouseDemolishGump : Gump
	{
		private readonly Mobile m_Mobile;
		private readonly BaseHouse m_House;

		public HouseDemolishGump(Mobile mobile, BaseHouse house)
			: base(110, 100)
		{
			m_Mobile = mobile;
			m_House = house;

			mobile.CloseGump(typeof(HouseDemolishGump));

			Closable = false;

			AddPage(0);

			AddBackground(0, 0, 420, 280, 5054);

			AddImageTiled(10, 10, 400, 20, 2624);
			AddAlphaRegion(10, 10, 400, 20);

			AddHtmlLocalized(10, 10, 400, 20, 1060635, 30720, false, false); // <CENTER>WARNING</CENTER>

			AddImageTiled(10, 40, 400, 200, 2624);
			AddAlphaRegion(10, 40, 400, 200);

			//AddHtmlLocalized( 10, 40, 400, 200, 1061795, 32512, false, true );
			/* You are about to demolish your house.
																				* You will be refunded the house's value directly to your bank box.
																				* All items in the house will remain behind and can be freely picked up by anyone.
																				* Once the house is demolished, anyone can attempt to place a new house on the vacant land.
																				* This action will not un-condemn any other houses on your account, nor will it end your 7-day waiting period (if it applies to you).
																				* Are you sure you wish to continue?
																				*/
			AddHtml(
				10,
				40,
				400,
				200,
				@"WARNING! If your house is a Classic House, a deed will be placed in your bank. If it is a custom house, you will be given a check for a FRACTION of the buying cost back.

Deeds on UO Forever are worth FAR less than the buying price to real estate broker NPCs.  If you want to sell it for a decent price, you may be able to sell to other players.

All items in the house will remain behind and can be freely picked up by anyone.  Once the house is demolished, anyone can attempt to place a new house on the vacant land.  Are you sure you wish to continue?",
				false,
				true);

			AddImageTiled(10, 250, 400, 20, 2624);
			AddAlphaRegion(10, 250, 400, 20);

			AddButton(10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(40, 250, 170, 20, 1011036, 32767, false, false); // OKAY

			AddButton(210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(240, 250, 170, 20, 1011012, 32767, false, false); // CANCEL
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info.ButtonID != 1 || m_House.Deleted)
			{
				return;
			}

			if (m_House.IsOwner(m_Mobile))
			{
				if (m_House.MovingCrate != null || m_House.InternalizedVendors.Count > 0)
				{
					return;
				}

				if (!Guild.NewGuildSystem && m_House.FindGuildstone() != null)
				{
					m_Mobile.SendLocalizedMessage(501389); // You cannot redeed a house with a guildstone inside.
					return;
				}
				/*
				if ( m_House.PlayerVendors.Count > 0 )
				{
					m_Mobile.SendLocalizedMessage( 503236 ); // You need to collect your vendor's belongings before moving.
					return;
				}
				*/
				if (m_House.HasRentedVendors && m_House.VendorInventories.Count > 0)
				{
					m_Mobile.SendLocalizedMessage(1062679);
						// You cannot do that that while you still have contract vendors or unclaimed contract vendor inventory in your house.
					return;
				}

				if (m_House.HasRentedVendors)
				{
					m_Mobile.SendLocalizedMessage(1062680);
						// You cannot do that that while you still have contract vendors in your house.
					return;
				}

				if (m_House.VendorInventories.Count > 0)
				{
					m_Mobile.SendLocalizedMessage(1062681);
						// You cannot do that that while you still have unclaimed contract vendor inventory in your house.
					return;
				}

				if (m_Mobile.AccessLevel >= AccessLevel.GameMaster)
				{
					m_Mobile.SendMessage("You do not get a refund for your house as you are not a player");
					m_House.RemoveKeys(m_Mobile);
					m_House.Delete();
				}
				else
				{
					Item toGive = m_House.GetDeed();

					if (toGive == null && m_House.Price > 0)
					{
						toGive = new BankCheck(m_House.Price);
					}

					if (toGive != null)
					{
						BankBox box = m_Mobile.FindBank(m_House.Expansion) ?? m_Mobile.BankBox;

						if (box != null)
						{
							box.AddItem(toGive);

							if (toGive is BankCheck)
							{
								m_Mobile.LocalOverheadMessage(
									MessageType.Regular,
									0x38,
									false,
									"A check for " + ((BankCheck)toGive).Worth + " gold has been placed in your bank.");
							}
							else // it's a deed
							{
								m_Mobile.LocalOverheadMessage(MessageType.Regular, 0x38, false, "A deed has been placed in your bank.");
							}
						}
						else if (m_Mobile.Backpack != null)
						{
							m_Mobile.Backpack.AddItem(toGive);
							m_Mobile.SendMessage(
								38,
								"The house deed was placed IN YOUR BACKPACK because there was an error accessing your bank. Contact staff asap!");
						}
						else
						{
							LoggingCustom.Log("ERROR_HouseDemolish.txt", DateTime.Now + "\t" + m_Mobile + "\t" + m_House);
						}
						m_House.RemoveKeys(m_Mobile);
						m_House.Delete();
					}
					else
					{
						m_Mobile.SendMessage("Unable to refund house.");
					}
				}
			}
			else
			{
				m_Mobile.SendLocalizedMessage(501320); // Only the house owner may do this.
			}
		}
	}
}