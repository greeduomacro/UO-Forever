#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.Conquests;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public class HousePlacementTool : Item
	{
		public override int LabelNumber { get { return 1060651; } } // a house placement tool

		[Constructable]
		public HousePlacementTool()
			: base(0x14F6)
		{
			Weight = 3.0;
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from is BaseCreature)
			{
				from.SendMessage("You can't place a house as a monster!"); // pseudoseer is possessing a monster
				return;
			}

			if (!BaseHouse.CanOwnHouse(from))
			{
				from.SendMessage(
					38,
					"You do not meet the requirements to own a house. " +
					"You must have over {0} skill points total across all characters " +
					"on your account and also have accrued {1} of game time on your account.",
					HouseSystemController._OwnHouseMinSkillsOnAccount,
					UberScriptFunctions.Methods.TIMESPANSTRING(null, HouseSystemController._OwnHouseMinGameTime));

				return;
			}

			if (IsChildOf(from.Backpack))
			{
				//from.LocalOverheadMessage(MessageType.Regular, 38, false, "WARNING! After placement, houses lose SIGNIFICANT value to NPCs!");
				if (from.CanBeginAction(typeof(HousePlacementTool)))
				{
					from.SendGump(new HousePlacementCategoryGump(from));
					from.RevealingAction();
				}
				else
				{
					from.SendMessage("You must wait a moment before attempting to place another house!");
				}
			}
			else
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
		}

		public HousePlacementTool(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			if (Weight == 0.0)
			{
				Weight = 3.0;
			}
		}
	}

	public class HousePlacementCategoryGump : Gump
	{
		private readonly Mobile m_From;

		private const int LabelColor = 0x7FFF;
		private const int LabelColorDisabled = 0x4210;

		public HousePlacementCategoryGump(Mobile from)
			: base(50, 50)
		{
			m_From = from;

			from.CloseGump(typeof(HousePlacementCategoryGump));
			from.CloseGump(typeof(HousePlacementListGump));

			AddPage(0);

			AddBackground(0, 0, 270, 145, 5054);

			AddImageTiled(10, 10, 250, 125, 2624);
			AddAlphaRegion(10, 10, 250, 125);

			AddHtmlLocalized(10, 10, 250, 20, 1060239, LabelColor, false, false); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

			AddButton(10, 110, 4017, 4019, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, 110, 150, 20, 3000363, LabelColor, false, false); // Close

			AddButton(10, 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, 40, 200, 20, 1060390, LabelColor, false, false); // Classic Houses

			AddButton(10, 60, 4005, 4007, 2, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, 60, 200, 20, 1060391, LabelColor, false, false); // 2-Story Customizable Houses

			AddButton(10, 80, 4005, 4007, 3, GumpButtonType.Reply, 0);
			AddHtmlLocalized(45, 80, 200, 20, 1060392, LabelColor, false, false); // 3-Story Customizable Houses
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (!m_From.CheckAlive() || m_From.Backpack == null ||
				m_From.Backpack.FindItemByType(typeof(HousePlacementTool)) == null)
			{
				return;
			}

			switch (info.ButtonID)
			{
				case 1: // Classic Houses
					{
						m_From.SendGump(new HousePlacementListGump(m_From, HousePlacementEntry.ClassicHouses));
						break;
					}
				case 2: // 2-Story Customizable Houses
					{
						m_From.SendGump(new HousePlacementListGump(m_From, HousePlacementEntry.TwoStoryFoundations));
						break;
					}
				case 3: // 3-Story Customizable Houses
					{
						m_From.SendGump(new HousePlacementListGump(m_From, HousePlacementEntry.ThreeStoryFoundations));
						break;
					}
			}
		}
	}

	public class HousePlacementListGump : Gump
	{
		private readonly Mobile m_From;
		private readonly HousePlacementEntry[] m_Entries;

		private readonly Type m_Currency;

		private const int LabelColor = 0x7FFF;
		private const int LabelHue = 0x480;

		public HousePlacementListGump(Mobile from, HousePlacementEntry[] entries)
			: base(50, 50)
		{
			m_From = from;
			m_Entries = entries;

			m_Currency = from.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

			from.CloseGump(typeof(HousePlacementCategoryGump));
			from.CloseGump(typeof(HousePlacementListGump));

			AddPage(0);

			AddBackground(0, 0, 520, 420, 5054);

			AddImageTiled(10, 10, 500, 20, 2624);
			AddAlphaRegion(10, 10, 500, 20);

			AddHtmlLocalized(10, 10, 500, 20, 1060239, LabelColor, false, false); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

			AddImageTiled(10, 40, 500, 20, 2624);
			AddAlphaRegion(10, 40, 500, 20);

			AddHtmlLocalized(50, 40, 225, 20, 1060235, LabelColor, false, false); // House Description
			AddHtmlLocalized(275, 40, 75, 20, 1060236, LabelColor, false, false); // Storage
			AddHtmlLocalized(350, 40, 75, 20, 1060237, LabelColor, false, false); // Lockdowns
			AddHtmlLocalized(425, 40, 75, 20, 1060034, LabelColor, false, false); // Cost

			AddImageTiled(10, 70, 500, 280, 2624);
			AddAlphaRegion(10, 70, 500, 280);

			AddImageTiled(10, 360, 500, 20, 2624);
			AddAlphaRegion(10, 360, 500, 20);

			AddHtmlLocalized(10, 360, 250, 20, 1060645, LabelColor, false, false); // Bank Balance:
			AddLabel(250, 360, LabelHue, Banker.GetBalance(from, m_Currency).ToString("#,0"));

			AddImageTiled(10, 390, 500, 20, 2624);
			AddAlphaRegion(10, 390, 500, 20);

			AddButton(10, 390, 4017, 4019, 0, GumpButtonType.Reply, 0);
			AddHtmlLocalized(50, 390, 100, 20, 3000363, LabelColor, false, false); // Close

			for (int i = 0; i < entries.Length; ++i)
			{
				int page = 1 + (i / 14);
				int index = i % 14;

				if (index == 0)
				{
					if (page > 1)
					{
						AddButton(450, 390, 4005, 4007, 0, GumpButtonType.Page, page);
						AddHtmlLocalized(400, 390, 100, 20, 3000406, LabelColor, false, false); // Next
					}

					AddPage(page);

					if (page > 1)
					{
						AddButton(200, 390, 4014, 4016, 0, GumpButtonType.Page, page - 1);
						AddHtmlLocalized(250, 390, 100, 20, 3000405, LabelColor, false, false); // Previous
					}
				}

				HousePlacementEntry entry = entries[i];

				int y = 70 + (index * 20);

				AddButton(10, y, 4005, 4007, 1 + i, GumpButtonType.Reply, 0);
                if (String.IsNullOrEmpty(entry.StringDescription))
				    AddHtmlLocalized(50, y, 225, 20, entry.Description, LabelColor, false, false);
                else
                    AddHtml(50, y, 225, 20, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", LabelColor, entry.StringDescription), false, false);
				AddLabel(275, y, LabelHue, entry.Storage.ToString("#,0"));
				AddLabel(350, y, LabelHue, entry.Lockdowns.ToString("#,0"));
				AddLabel(425, y, LabelHue, entry.Cost.ToString("#,0"));
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (!m_From.CheckAlive() || m_From.Backpack == null ||
				m_From.Backpack.FindItemByType(typeof(HousePlacementTool)) == null)
			{
				return;
			}

			int index = info.ButtonID - 1;

			if (index >= 0 && index < m_Entries.Length)
			{
				if (m_From.AccessLevel < AccessLevel.GameMaster && BaseHouse.HasAccountHouse(m_From))
				{
					m_From.SendLocalizedMessage(501271); // You already own a house, you may not place another!
				}
				else
				{
					m_From.Target = new NewHousePlacementTarget(m_Entries, m_Entries[index]);
				}
			}
			else
			{
				m_From.SendGump(new HousePlacementCategoryGump(m_From));
			}
		}
	}

	public class NewHousePlacementTarget : MultiTarget
	{
		private readonly HousePlacementEntry m_Entry;
		private readonly HousePlacementEntry[] m_Entries;

		private bool m_Placed;

		public NewHousePlacementTarget(HousePlacementEntry[] entries, HousePlacementEntry entry)
			: base(entry.MultiID, entry.Offset)
		{
			Range = 14;

			m_Entries = entries;
			m_Entry = entry;
		}

		protected override void OnTarget(Mobile from, object o)
		{
			if (!from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType(typeof(HousePlacementTool)) == null)
			{
				return;
			}

			var ip = o as IPoint3D;

			if (ip != null)
			{
				if (ip is Item)
				{
					ip = ((Item)ip).GetWorldTop();
				}

				var p = new Point3D(ip);

				Region reg = Region.Find(new Point3D(p), from.Map);

				if (from.AccessLevel >= AccessLevel.GameMaster || reg.AllowHousing(from, p))
				{
					m_Placed = m_Entry.OnPlacement(from, p);
				}
				else if (reg.IsPartOf(typeof(TempNoHousingRegion)))
				{
					from.SendLocalizedMessage(501270);
					// Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
				}
				else if (reg.IsPartOf(typeof(TreasureRegion)) || reg.IsPartOf(typeof(HouseRegion)))
				{
					from.SendLocalizedMessage(1043287);
					// The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
				}
				else
				{
					from.SendLocalizedMessage(501265); // Housing can not be created in this area.
				}
			}
		}

		protected override void OnTargetFinish(Mobile from)
		{
			if (!from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType(typeof(HousePlacementTool)) == null)
			{
				return;
			}

			if (!m_Placed)
			{
				if (HouseSystemController._MaxPlaceDelay == 0)
				{
					from.SendGump(new HousePlacementListGump(from, m_Entries));
				}
				else
				{
					from.BeginAction(typeof(HousePlacementTool));
					Timer.DelayCall(
						TimeSpan.FromSeconds(
							Utility.RandomMinMax(HouseSystemController._MinPlaceDelay, HouseSystemController._MaxPlaceDelay)),
						ReleasePlacementLock,
						from);
				}
				from.RevealingAction();
			}
		}

		public static void ReleasePlacementLock(Mobile from)
		{
			from.EndAction(typeof(HousePlacementTool));
		}
	}

	public class HousePlacementEntry
	{
		private readonly int m_Storage;
		private readonly int m_Lockdowns;
		private readonly int m_NewStorage;
		private readonly int m_NewLockdowns;

		public Type Type { get; private set; }

		public int Description { get; private set; }
        public string StringDescription { get; private set; }
		public int Vendors { get; private set; }
		public int Cost { get; private set; }
		public int MultiID { get; private set; }
		public Point3D Offset { get; private set; }

		public int Storage { get { return BaseHouse.NewVendorSystem ? m_NewStorage : m_Storage; } }
		public int Lockdowns { get { return BaseHouse.NewVendorSystem ? m_NewLockdowns : m_Lockdowns; } }

		public HousePlacementEntry(
			Type type,
			int description,
			int storage,
			int lockdowns,
			int newStorage,
			int newLockdowns,
			int vendors,
			int cost,
			int xOffset,
			int yOffset,
			int zOffset,
			int multiID)
		{
			Type = type;
			Description = description;
			m_Storage = storage;
			m_Lockdowns = lockdowns;
			m_NewStorage = newStorage;
			m_NewLockdowns = newLockdowns;
			Vendors = vendors;
			Cost = cost;

			Offset = new Point3D(xOffset, yOffset, zOffset);

			MultiID = multiID;
		}

        public HousePlacementEntry(
        Type type,
        string description,
        int storage,
        int lockdowns,
        int newStorage,
        int newLockdowns,
        int vendors,
        int cost,
        int xOffset,
        int yOffset,
        int zOffset,
        int multiID)
        {
            Type = type;
            StringDescription = description;
            m_Storage = storage;
            m_Lockdowns = lockdowns;
            m_NewStorage = newStorage;
            m_NewLockdowns = newLockdowns;
            Vendors = vendors;
            Cost = cost;

            Offset = new Point3D(xOffset, yOffset, zOffset);

            MultiID = multiID;
        }

		public BaseHouse ConstructHouse(Mobile from)
		{
			try
			{
				object[] args;

				if (Type == typeof(HouseFoundation))
				{
					args = new object[] {from, MultiID, m_Storage, m_Lockdowns};
				}
				else if (Type == typeof(SmallOldHouse) || Type == typeof(SmallShop) || Type == typeof(TwoStoryHouse))
				{
					args = new object[] {from, MultiID};
				}
				else
				{
					args = new object[] {from};
				}

				return Activator.CreateInstance(Type, args) as BaseHouse;
			}
			catch
			{ }

			return null;
		}

		public void PlacementWarning_Callback(Mobile from, bool okay, object state)
		{
			if (!from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType(typeof(HousePlacementTool)) == null)
			{
				return;
			}

			var prevHouse = (PreviewHouse)state;

			if (!okay)
			{
				prevHouse.Delete();
				return;
			}

			if (prevHouse.Deleted)
			{
				/* Too much time has passed and the test house you created has been deleted.
				 * Please try again!
				 */
				from.SendGump(new NoticeGump(1060637, 30720, 1060647, 32512, 320, 180, null, null));

				return;
			}

			Point3D center = prevHouse.Location;
			//Map map = prevHouse.Map;

			prevHouse.Delete();

			ArrayList toMove;
			//Point3D center = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );
			HousePlacementResult res = HousePlacement.Check(from, MultiID, center, out toMove, false);

			switch (res)
			{
				case HousePlacementResult.Valid:
					{
						if (from.AccessLevel < AccessLevel.GameMaster && BaseHouse.HasAccountHouse(from))
						{
							from.SendLocalizedMessage(501271); // You already own a house, you may not place another!
						}
						else
						{
							BaseHouse house = ConstructHouse(from);

							if (house == null)
							{
								return;
							}

							house.Price = Cost;

							Type cType = from.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

							if (from.AccessLevel >= AccessLevel.GameMaster)
							{
								from.SendMessage(
									"{0:#,0} {1} would have been withdrawn from your bank if you were not a GM.", Cost.ToString("#,0"), cType.Name);
							}
							else
							{
								if (Banker.Withdraw(from, cType, Cost))
								{
									from.SendMessage("{0:#,0} {1} has been withdrawn from your bank box.", Cost.ToString("#,0"), cType.Name);
								}
								else
								{
									house.RemoveKeys(from);
									house.Delete();

									from.SendMessage(
										"You do not have the funds available in your bank box to purchase this house.  " +
										"Try placing a smaller house, or adding {0} or checks to your bank box.",
										cType.Name);
									return;
								}
							}

							house.MoveToWorld(center, from.Map);

                            Conquests.CheckProgress<HousePlacementConquest>(from as PlayerMobile, house);

							foreach (object o in toMove)
							{
								if (o is Mobile)
								{
									((Mobile)o).Location = house.BanLocation;
								}
								else if (o is Item)
								{
									((Item)o).Location = house.BanLocation;
								}
							}
						}

						break;
					}
				case HousePlacementResult.MobileBlocking:
					{
						from.SendMessage("There is somebody in the way, and you can't place on top of them!");
						break;
					}
				case HousePlacementResult.ItemBlocking:
					{
						from.SendMessage("There is an item in the way, and you can't place on top of them!");
						break;
					}
				case HousePlacementResult.BadItem:
				case HousePlacementResult.BadLand:
				case HousePlacementResult.BadStatic:
				case HousePlacementResult.BadRegionHidden:
				case HousePlacementResult.NoSurface:
					{
						from.SendLocalizedMessage(1043287);
						// The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
						break;
					}
				case HousePlacementResult.BadRegion:
					{
						from.SendLocalizedMessage(501265); // Housing cannot be created in this area.
						break;
					}
				case HousePlacementResult.BadRegionTemp:
					{
						from.SendLocalizedMessage(501270);
						// Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
						break;
					}
				case HousePlacementResult.BadRegionRaffle:
					{
						from.SendLocalizedMessage(1150493); // You must have a deed for this plot of land in order to build here.
						break;
					}
				case HousePlacementResult.InvalidCastleKeep:
					{
						from.SendLocalizedMessage(1061122); // Castles and keeps cannot be created here.
						break;
					}
			}
		}

		public bool OnPlacement(Mobile from, Point3D p)
		{
			if (!from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType(typeof(HousePlacementTool)) == null)
			{
				return false;
			}

			ArrayList toMove;
			var center = new Point3D(p.X - Offset.X, p.Y - Offset.Y, p.Z - Offset.Z);
			HousePlacementResult res = HousePlacement.Check(from, MultiID, center, out toMove);

			switch (res)
			{
				case HousePlacementResult.Valid:
					{
						if (from.AccessLevel < AccessLevel.GameMaster && BaseHouse.HasAccountHouse(from))
						{
							from.SendLocalizedMessage(501271); // You already own a house, you may not place another!
						}
						else
						{
							from.SendLocalizedMessage(1011576); // This is a valid location.

							var prev = new PreviewHouse(MultiID);

							MultiComponentList mcl = prev.Components;

							var banLoc = new Point3D(center.X + mcl.Min.X, center.Y + mcl.Max.Y + 1, center.Z);

							foreach (MultiTileEntry entry in mcl.List)
							{
								int itemID = entry.m_ItemID;

								if (itemID < 0xBA3 || itemID > 0xC0E)
								{
									continue;
								}

								banLoc = new Point3D(center.X + entry.m_OffsetX, center.Y + entry.m_OffsetY, center.Z);
								break;
							}

							foreach (object o in toMove)
							{
								if (o is Mobile)
								{
									((Mobile)o).Location = banLoc;
								}
								else if (o is Item)
								{
									((Item)o).Location = banLoc;
								}
							}

							prev.MoveToWorld(center, from.Map);

							/* You are about to place a new house.
						 * Placing this house will condemn any and all of your other houses that you may have.
						 * All of your houses on all shards will be affected.
						 *
						 * In addition, you will not be able to place another house or have one transferred to you for one (1) real-life week.
						 *
						 * Once you accept these terms, these effects cannot be reversed.
						 * Re-deeding or transferring your new house will not uncondemn your other house(s) nor will the one week timer be removed.
						 *
						 * If you are absolutely certain you wish to proceed, click the button next to OKAY below.
						 * If you do not wish to trade for this house, click CANCEL.
						 */
							//from.SendGump( new WarningGump( 1060635, 30720, 1049583, 32512, 420, 280, new WarningGumpCallback( PlacementWarning_Callback ), prev ) );

							from.SendGump(
								new WarningGump(
									1060635, 30720, @"!!! VERY IMPORTANT UOFOREVER WARNING !!!
IF you accept this house placement, be aware that while demolishing a CLASSIC house returns the house deed to you, the deed is WORTH MUCH LESS than the buying price to NPC real estate brokers! (i.e. you will NOT get back NEARLY what you paid for it from NPCs!). Demolishing a CUSTOM house gives you a check for MUCH LESS than the buying price!

You MAY be able to sell classic house deeds to other players, but be aware that the NPC buy-back is SIGNIFICANTLY reduced!", 16777215, 420, 280, PlacementWarning_Callback, prev));

							return true;
						}

						break;
					}
				case HousePlacementResult.MobileBlocking:
					{
						from.SendMessage("There is somebody in the way, and you can't place on top of them!");
						break;
					}
				case HousePlacementResult.ItemBlocking:
					{
						from.SendMessage("There is an item in the way, and you can't place on top of them!");
						break;
					}
				case HousePlacementResult.BadItem:
				case HousePlacementResult.BadLand:
				case HousePlacementResult.BadStatic:
				case HousePlacementResult.BadRegionHidden:
				case HousePlacementResult.NoSurface:
					{
						// The house could not be created here.  
						// Either something is blocking the house, or the house would not be on valid terrain.
						from.SendLocalizedMessage(1043287);
						break;
					}
				case HousePlacementResult.BadRegion:
					{
						from.SendLocalizedMessage(501265); // Housing cannot be created in this area.
						break;
					}
				case HousePlacementResult.BadRegionTemp:
					{
						//Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
						from.SendLocalizedMessage(501270);
						break;
					}
				case HousePlacementResult.BadRegionRaffle:
					{
						from.SendLocalizedMessage(1150493); // You must have a deed for this plot of land in order to build here.
						break;
					}
				case HousePlacementResult.InvalidCastleKeep:
					{
						from.SendLocalizedMessage(1061122); // Castles and keeps cannot be created here.
						break;
					}
			}

			return false;
		}

		private static readonly Hashtable m_Table;

		static HousePlacementEntry()
		{
			m_Table = new Hashtable();

			FillTable(m_ClassicHouses);
			FillTable(m_TwoStoryFoundations);
			FillTable(m_ThreeStoryFoundations);
		}

		public static HousePlacementEntry Find(BaseHouse house)
		{
			object obj = m_Table[house.GetType()];

			if (obj is HousePlacementEntry)
			{
				return ((HousePlacementEntry)obj);
			}

			if (obj is ArrayList)
			{
				var list = (ArrayList)obj;

				return list.Cast<HousePlacementEntry>().FirstOrDefault(e => e.MultiID == house.ItemID);
			}

			if (obj is Hashtable)
			{
				var table = (Hashtable)obj;

				obj = table[house.ItemID];

				if (obj is HousePlacementEntry)
				{
					return (HousePlacementEntry)obj;
				}
			}

			return null;
		}

		private static void FillTable(IEnumerable<HousePlacementEntry> entries)
		{
			foreach (HousePlacementEntry e in entries)
			{
				object obj = m_Table[e.Type];

				if (obj == null)
				{
					m_Table[e.Type] = e;
				}
				else if (obj is HousePlacementEntry)
				{
					var list = new ArrayList
					{
						obj,
						e
					};

					m_Table[e.Type] = list;
				}
				else if (obj is ArrayList)
				{
					var list = (ArrayList)obj;

					if (list.Count == 8)
					{
						var table = new Hashtable();

						foreach (object o in list)
						{
							table[((HousePlacementEntry)o).MultiID] = o;
						}

						table[e.MultiID] = e;

						m_Table[e.Type] = table;
					}
					else
					{
						list.Add(e);
					}
				}
				else if (obj is Hashtable)
				{
					((Hashtable)obj)[e.MultiID] = e;
				}
			}
		}

		private static readonly HousePlacementEntry[] m_ClassicHouses = new[]
		{
			new HousePlacementEntry(typeof(SmallOldHouse), 1011303, 425, 212, 489, 244, 10, 64505, 0, 4, 0, 0x0064),
			new HousePlacementEntry(typeof(SmallOldHouse), 1011304, 425, 212, 489, 244, 10, 64505, 0, 4, 0, 0x0066),
			new HousePlacementEntry(typeof(SmallOldHouse), 1011305, 425, 212, 489, 244, 10, 64505, 0, 4, 0, 0x0068),
			new HousePlacementEntry(typeof(SmallOldHouse), 1011306, 425, 212, 489, 244, 10, 64505, 0, 4, 0, 0x006A),
			new HousePlacementEntry(typeof(SmallOldHouse), 1011307, 425, 212, 489, 244, 10, 64505, 0, 4, 0, 0x006C),
			new HousePlacementEntry(typeof(SmallOldHouse), 1011308, 425, 212, 489, 244, 10, 64505, 0, 4, 0, 0x006E),
			new HousePlacementEntry(typeof(SmallShop), 1011321, 425, 212, 489, 244, 10, 123000, -1, 4, 0, 0x00A0),
			new HousePlacementEntry(typeof(SmallShop), 1011322, 425, 212, 489, 244, 10, 123000, 0, 4, 0, 0x00A2),
			new HousePlacementEntry(typeof(SmallTower), 1011317, 580, 290, 667, 333, 14, 171500, 3, 4, 0, 0x0098),
			new HousePlacementEntry(typeof(TwoStoryVilla), 1011319, 1100, 550, 1265, 632, 24, 266500, 3, 6, 0, 0x009E),
			new HousePlacementEntry(typeof(SandStonePatio), 1011320, 850, 425, 1265, 632, 24, 189900, -1, 4, 0, 0x009C),
			new HousePlacementEntry(typeof(LogCabin), 1011318, 1100, 550, 1265, 632, 24, 198800, 1, 6, 0, 0x009A),
			new HousePlacementEntry(typeof(GuildHouse), 1011309, 1370, 685, 1576, 788, 28, 294500, -1, 7, 0, 0x0074),
			new HousePlacementEntry(typeof(TwoStoryHouse), 1011310, 1370, 685, 1576, 788, 28, 392400, -3, 7, 0, 0x0076),
			new HousePlacementEntry(typeof(TwoStoryHouse), 1011311, 1370, 685, 1576, 788, 28, 392400, -3, 7, 0, 0x0078),
			new HousePlacementEntry(typeof(LargePatioHouse), 1011315, 1370, 685, 1576, 788, 28, 249250, -4, 7, 0, 0x008C),
			new HousePlacementEntry(typeof(LargeMarbleHouse), 1011316, 1370, 685, 1576, 788, 28, 392000, -4, 7, 0, 0x0096),
			new HousePlacementEntry(typeof(Tower), 1011312, 2119, 1059, 2437, 1218, 42, 873200, 0, 7, 0, 0x007A),
			new HousePlacementEntry(typeof(Keep), 1011313, 2625, 1312, 3019, 1509, 52, 1325200, 0, 11, 0, 0x007C),
			new HousePlacementEntry(typeof(Castle), 1011314, 4076, 2038, 4688, 2344, 78, 2122800, 0, 16, 0, 0x007E)
		};

		public static HousePlacementEntry[] ClassicHouses { get { return m_ClassicHouses; } }

		private static readonly HousePlacementEntry[] m_TwoStoryFoundations = new[]
		{
			new HousePlacementEntry(typeof(HouseFoundation), 1060241, 425, 212, 489, 244, 10, 90500, 0, 4, 0, 0x13EC),
			// 7x7 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060242, 580, 290, 667, 333, 14, 94500, 0, 5, 0, 0x13ED),
			// 7x8 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060243, 650, 325, 748, 374, 16, 98500, 0, 5, 0, 0x13EE),
			// 7x9 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060244, 700, 350, 805, 402, 16, 122500, 0, 6, 0, 0x13EF),
			// 7x10 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060245, 750, 375, 863, 431, 16, 126500, 0, 6, 0, 0x13F0),
			// 7x11 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060246, 800, 400, 920, 460, 18, 150500, 0, 7, 0, 0x13F1),
			// 7x12 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060253, 580, 290, 667, 333, 14, 94500, 0, 4, 0, 0x13F8),
			// 8x7 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060254, 650, 325, 748, 374, 16, 99000, 0, 5, 0, 0x13F9),
			// 8x8 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060255, 700, 350, 805, 402, 16, 123500, 0, 5, 0, 0x13FA),
			// 8x9 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060256, 750, 375, 863, 431, 16, 128000, 0, 6, 0, 0x13FB),
			// 8x10 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060257, 800, 400, 920, 460, 18, 152500, 0, 6, 0, 0x13FC),
			// 8x11 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060258, 850, 425, 1265, 632, 24, 157000, 0, 7, 0, 0x13FD),
			// 8x12 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060259, 1100, 550, 1265, 632, 24, 161500, 0, 7, 0, 0x13FE),
			// 8x13 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060265, 650, 325, 748, 374, 16, 98500, 0, 4, 0, 0x1404),
			// 9x7 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060266, 700, 350, 805, 402, 16, 123500, 0, 5, 0, 0x1405),
			// 9x8 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060267, 750, 375, 863, 431, 16, 128500, 0, 5, 0, 0x1406),
			// 9x9 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060268, 800, 400, 920, 460, 18, 123500, 0, 6, 0, 0x1407),
			// 9x10 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060269, 850, 425, 1265, 632, 24, 158500, 0, 6, 0, 0x1408),
			// 9x11 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060270, 1100, 550, 1265, 632, 24, 163500, 0, 7, 0, 0x1409),
			// 9x12 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060271, 1100, 550, 1265, 632, 24, 168500, 0, 7, 0, 0x140A),
			// 9x13 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060277, 700, 350, 805, 402, 16, 142500, 0, 4, 0, 0x1410),
			// 10x7 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060278, 750, 375, 863, 431, 16, 148000, 0, 5, 0, 0x1411),
			// 10x8 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060279, 800, 400, 920, 460, 18, 153500, 0, 5, 0, 0x1412),
			// 10x9 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060280, 850, 425, 1265, 632, 24, 159000, 0, 6, 0, 0x1413),
			// 10x10 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060281, 1100, 550, 1265, 632, 24, 164500, 0, 6, 0, 0x1414),
			// 10x11 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060282, 1100, 550, 1265, 632, 24, 170000, 0, 7, 0, 0x1415),
			// 10x12 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060283, 1150, 575, 1323, 661, 24, 175500, 0, 7, 0, 0x1416),
			// 10x13 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060289, 750, 375, 863, 431, 16, 146500, 0, 4, 0, 0x141C),
			// 11x7 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060290, 800, 400, 920, 460, 18, 152500, 0, 5, 0, 0x141D),
			// 11x8 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060291, 850, 425, 1265, 632, 24, 158500, 0, 5, 0, 0x141E),
			// 11x9 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060292, 1100, 550, 1265, 632, 24, 164500, 0, 6, 0, 0x141F),
			// 11x10 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060293, 1100, 550, 1265, 632, 24, 170500, 0, 6, 0, 0x1420),
			// 11x11 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060294, 1150, 575, 1323, 661, 24, 176500, 0, 7, 0, 0x1421),
			// 11x12 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060295, 1200, 600, 1380, 690, 26, 182500, 0, 7, 0, 0x1422),
			// 11x13 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060301, 800, 400, 920, 460, 18, 150500, 0, 4, 0, 0x1428),
			// 12x7 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060302, 850, 425, 1265, 632, 24, 157000, 0, 5, 0, 0x1429),
			// 12x8 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060303, 1100, 550, 1265, 632, 24, 163500, 0, 5, 0, 0x142A),
			// 12x9 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060304, 1100, 550, 1265, 632, 24, 170000, 0, 6, 0, 0x142B),
			// 12x10 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060305, 1150, 575, 1323, 661, 24, 176500, 0, 6, 0, 0x142C),
			// 12x11 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060306, 1200, 600, 1380, 690, 26, 183000, 0, 7, 0, 0x142D),
			// 12x12 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060307, 1250, 625, 1438, 719, 26, 189500, 0, 7, 0, 0x142E),
			// 12x13 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060314, 1100, 550, 1265, 632, 24, 161500, 0, 5, 0, 0x1435),
			// 13x8 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060315, 1100, 550, 1265, 632, 24, 168500, 0, 5, 0, 0x1436),
			// 13x9 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060316, 1150, 575, 1323, 661, 24, 175500, 0, 6, 0, 0x1437),
			// 13x10 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060317, 1200, 600, 1380, 690, 26, 182500, 0, 6, 0, 0x1438),
			// 13x11 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060318, 1250, 625, 1438, 719, 26, 189500, 0, 7, 0, 0x1439),
			// 13x12 2-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060319, 1300, 650, 1495, 747, 28, 196500, 0, 7, 0, 0x143A)
			// 13x13 2-Story Customizable House
		};

		public static HousePlacementEntry[] TwoStoryFoundations { get { return m_TwoStoryFoundations; } }

		private static readonly HousePlacementEntry[] m_ThreeStoryFoundations = new[]
		{
			new HousePlacementEntry(typeof(HouseFoundation), 1060272, 1150, 575, 1323, 661, 24, 173500, 0, 8, 0, 0x140B),
			// 9x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060284, 1200, 600, 1380, 690, 26, 181000, 0, 8, 0, 0x1417),
			// 10x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060285, 1250, 625, 1438, 719, 26, 186500, 0, 8, 0, 0x1418),
			// 10x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060296, 1250, 625, 1438, 719, 26, 188500, 0, 8, 0, 0x1423),
			// 11x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060297, 1300, 650, 1495, 747, 28, 194500, 0, 8, 0, 0x1424),
			// 11x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060298, 1350, 675, 1553, 776, 28, 210500, 0, 9, 0, 0x1425),
			// 11x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060308, 1300, 650, 1495, 747, 28, 196000, 0, 8, 0, 0x142F),
			// 12x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060309, 1350, 675, 1553, 776, 28, 202500, 0, 8, 0, 0x1430),
			// 12x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060310, 1370, 685, 1576, 788, 28, 209000, 0, 9, 0, 0x1431),
			// 12x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060311, 1370, 685, 1576, 788, 28, 215500, 0, 9, 0, 0x1432),
			// 12x17 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060320, 1350, 675, 1553, 776, 28, 203500, 0, 8, 0, 0x143B),
			// 13x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060321, 1370, 685, 1576, 788, 28, 210500, 0, 8, 0, 0x143C),
			// 13x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060322, 1370, 685, 1576, 788, 28, 217500, 0, 9, 0, 0x143D),
			// 13x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060323, 2119, 1059, 2437, 1218, 42, 224500, 0, 9, 0, 0x143E),
			// 13x17 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060324, 2119, 1059, 2437, 1218, 42, 231500, 0, 10, 0, 0x143F),
			// 13x18 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060327, 1150, 575, 1323, 661, 24, 173500, 0, 5, 0, 0x1442),
			// 14x9 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060328, 1200, 600, 1380, 690, 26, 181000, 0, 6, 0, 0x1443),
			// 14x10 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060329, 1250, 625, 1438, 719, 26, 188500, 0, 6, 0, 0x1444),
			// 14x11 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060330, 1300, 650, 1495, 747, 28, 196000, 0, 7, 0, 0x1445),
			// 14x12 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060331, 1350, 675, 1553, 776, 28, 203500, 0, 7, 0, 0x1446),
			// 14x13 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060332, 1370, 685, 1576, 788, 28, 211000, 0, 8, 0, 0x1447),
			// 14x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060333, 1370, 685, 1576, 788, 28, 218500, 0, 8, 0, 0x1448),
			// 14x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060334, 2119, 1059, 2437, 1218, 42, 226000, 0, 9, 0, 0x1449),
			// 14x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060335, 2119, 1059, 2437, 1218, 42, 233500, 0, 9, 0, 0x144A),
			// 14x17 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060336, 2119, 1059, 2437, 1218, 42, 241000, 0, 10, 0, 0x144B),
			// 14x18 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060340, 1250, 625, 1438, 719, 26, 186500, 0, 6, 0, 0x144F),
			// 15x10 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060341, 1300, 650, 1495, 747, 28, 194500, 0, 6, 0, 0x1450),
			// 15x11 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060342, 1350, 675, 1553, 776, 28, 202500, 0, 7, 0, 0x1451),
			// 15x12 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060343, 1370, 685, 1576, 788, 28, 210500, 0, 7, 0, 0x1452),
			// 15x13 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060344, 1370, 685, 1576, 788, 28, 218500, 0, 8, 0, 0x1453),
			// 15x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060345, 2119, 1059, 2437, 1218, 42, 226500, 0, 8, 0, 0x1454),
			// 15x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060346, 2119, 1059, 2437, 1218, 42, 234500, 0, 9, 0, 0x1455),
			// 15x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060347, 2119, 1059, 2437, 1218, 42, 242500, 0, 9, 0, 0x1456),
			// 15x17 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060348, 2119, 1059, 2437, 1218, 42, 250500, 0, 10, 0, 0x1457),
			// 15x18 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060353, 1350, 675, 1553, 776, 28, 200500, 0, 6, 0, 0x145C),
			// 16x11 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060354, 1370, 685, 1576, 788, 28, 209000, 0, 7, 0, 0x145D),
			// 16x12 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060355, 1370, 685, 1576, 788, 28, 217500, 0, 7, 0, 0x145E),
			// 16x13 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060356, 2119, 1059, 2437, 1218, 42, 226000, 0, 8, 0, 0x145F),
			// 16x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060357, 2119, 1059, 2437, 1218, 42, 234500, 0, 8, 0, 0x1460),
			// 16x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060358, 2119, 1059, 2437, 1218, 42, 243000, 0, 9, 0, 0x1461),
			// 16x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060359, 2119, 1059, 2437, 1218, 42, 251500, 0, 9, 0, 0x1462),
			// 16x17 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060360, 2119, 1059, 2437, 1218, 42, 260000, 0, 10, 0, 0x1463),
			// 16x18 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060366, 1370, 685, 1576, 788, 28, 215500, 0, 7, 0, 0x1469),
			// 17x12 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060367, 2119, 1059, 2437, 1218, 42, 224500, 0, 7, 0, 0x146A),
			// 17x13 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060368, 2119, 1059, 2437, 1218, 42, 233500, 0, 8, 0, 0x146B),
			// 17x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060369, 2119, 1059, 2437, 1218, 42, 242500, 0, 8, 0, 0x146C),
			// 17x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060370, 2119, 1059, 2437, 1218, 42, 251500, 0, 9, 0, 0x146D),
			// 17x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060371, 2119, 1059, 2437, 1218, 42, 260500, 0, 9, 0, 0x146E),
			// 17x17 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060372, 2119, 1059, 2437, 1218, 42, 269500, 0, 10, 0, 0x146F),
			// 17x18 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060379, 2119, 1059, 2437, 1218, 42, 231500, 0, 7, 0, 0x1476),
			// 18x13 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060380, 2119, 1059, 2437, 1218, 42, 241000, 0, 8, 0, 0x1477),
			// 18x14 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060381, 2119, 1059, 2437, 1218, 42, 250500, 0, 8, 0, 0x1478),
			// 18x15 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060382, 2119, 1059, 2437, 1218, 42, 260000, 0, 9, 0, 0x1479),
			// 18x16 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060383, 2119, 1059, 2437, 1218, 42, 269500, 0, 9, 0, 0x147A),
			// 18x17 3-Story Customizable House
			new HousePlacementEntry(typeof(HouseFoundation), 1060384, 2119, 1059, 2437, 1218, 42, 279000, 0, 10, 0, 0x147B),
            //new HousePlacementEntry( typeof( HouseFoundation ),		"30x30 Custom House",	2119,	1059,	2437,	1218,	42,	179000,		0,	10,	0,	72	)  // 18x18 3-Story Customizable House
		};

		public static HousePlacementEntry[] ThreeStoryFoundations { get { return m_ThreeStoryFoundations; } }
	}
}