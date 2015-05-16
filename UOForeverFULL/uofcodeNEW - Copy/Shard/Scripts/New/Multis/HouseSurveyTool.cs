using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Regions;

namespace Server.Items
{
	public class HouseSurveyTool : Item, IUsesRemaining
	{
		public override string DefaultName{ get{ return "a house survey tool"; } }

		private int m_UsesRemaining;
		public bool ShowUsesRemaining{ get{ return m_UsesRemaining > -1; } set{} }

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[Constructable]
		public HouseSurveyTool() : this( 10 )
		{
		}

		[Constructable]
		public HouseSurveyTool( int usesRemaining ) : base( 0x14F6 )
		{
			Weight = 3.0;
			Hue = 1072;
			LootType = LootType.Blessed;
			m_UsesRemaining = usesRemaining;
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_UsesRemaining == 0 )
				from.SendMessage( "This tool has lost its charge." );
			else if ( IsChildOf( from.Backpack ) )
			{
				from.CloseGump( typeof( HouseSurveyCategoryGump ) );
				from.SendGump( new HouseSurveyCategoryGump( from, this ) );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( ShowUsesRemaining )
				list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public HouseSurveyTool( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 1 ); // version

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
				{
					m_UsesRemaining = reader.ReadInt();
					break;
				}
			}
		}
	}

	public class HouseSurveyCategoryGump : Gump
	{
		private Mobile m_From;
		private HouseSurveyTool m_Tool;

		private const int LabelColor = 0x7FFF;
		private const int LabelColorDisabled = 0x4210;

		public HouseSurveyCategoryGump( Mobile from, HouseSurveyTool tool ) : base( 50, 50 )
		{
			m_From = from;
			m_Tool = tool;

			from.CloseGump( typeof( HouseSurveyCategoryGump ) );
			from.CloseGump( typeof( HouseSurveyListGump ) );

			AddPage( 0 );

			AddBackground( 0, 0, 270, 145, 5054 );

			AddImageTiled( 10, 10, 250, 125, 2624 );
			AddAlphaRegion( 10, 10, 250, 125 );

			AddHtmlLocalized( 10, 10, 250, 20, 1060239, LabelColor, false, false ); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

			AddButton( 10, 110, 4017, 4019, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 110, 150, 20, 3000363, LabelColor, false, false ); // Close

			AddButton( 10, 40, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 40, 200, 20, 1060390, LabelColor, false, false ); // Classic Houses

			AddButton( 10, 60, 4005, 4007, 2, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 60, 200, 20, 1060391, LabelColor, false, false ); // 2-Story Customizable Houses

			AddButton( 10, 80, 4005, 4007, 3, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 80, 200, 20, 1060392, LabelColor, false, false ); // 3-Story Customizable Houses
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( !m_From.CheckAlive() || m_From.Backpack == null || m_Tool == null || m_Tool.UsesRemaining == 0 || !m_Tool.IsChildOf( m_From.Backpack )  )
				return;

			switch ( info.ButtonID )
			{
				case 1: // Classic Houses
				{
					m_From.SendGump( new HouseSurveyListGump( m_From, m_Tool, HouseSurveyEntry.ClassicHouses ) );
					break;
				}
				case 2: // 2-Story Customizable Houses
				{
					m_From.SendGump( new HouseSurveyListGump( m_From, m_Tool, HouseSurveyEntry.TwoStoryFoundations ) );
					break;
				}
				case 3: // 3-Story Customizable Houses
				{
					m_From.SendGump( new HouseSurveyListGump( m_From, m_Tool, HouseSurveyEntry.ThreeStoryFoundations ) );
					break;
				}
			}
		}
	}

	public class HouseSurveyListGump : Gump
	{
		private Mobile m_From;
		private HouseSurveyTool m_Tool;
		private HouseSurveyEntry[] m_Entries;

		private const int LabelColor = 0x7FFF;
		private const int LabelHue = 0x480;

		public HouseSurveyListGump( Mobile from, HouseSurveyTool tool, HouseSurveyEntry[] entries ) : base( 50, 50 )
		{
			m_From = from;
			m_Tool = tool;
			m_Entries = entries;

			from.CloseGump( typeof( HouseSurveyCategoryGump ) );
			from.CloseGump( typeof( HouseSurveyListGump ) );

			AddPage( 0 );

			AddBackground( 0, 0, 520, 420, 5054 );

			AddImageTiled( 10, 10, 500, 20, 2624 );
			AddAlphaRegion( 10, 10, 500, 20 );

			AddHtmlLocalized( 10, 10, 500, 20, 1060239, LabelColor, false, false ); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

			AddImageTiled( 10, 40, 500, 20, 2624 );
			AddAlphaRegion( 10, 40, 500, 20 );

			AddHtmlLocalized( 50, 40, 225, 20, 1060235, LabelColor, false, false ); // House Description
			AddHtmlLocalized( 275, 40, 75, 20, 1060236, LabelColor, false, false ); // Storage
			AddHtmlLocalized( 350, 40, 75, 20, 1060237, LabelColor, false, false ); // Lockdowns
			//AddHtmlLocalized( 425, 40, 75, 20, 1060034, LabelColor, false, false ); // Cost

			AddImageTiled( 10, 70, 500, 280, 2624 );
			AddAlphaRegion( 10, 70, 500, 280 );

			AddImageTiled( 10, 360, 500, 20, 2624 );
			AddAlphaRegion( 10, 360, 500, 20 );

			//AddHtmlLocalized( 10, 360, 250, 20, 1060645, LabelColor, false, false ); // Bank Balance:
			//AddLabel( 250, 360, LabelHue, Banker.GetBalance( from ).ToString() );

			AddImageTiled( 10, 390, 500, 20, 2624 );
			AddAlphaRegion( 10, 390, 500, 20 );

			AddButton( 10, 390, 4017, 4019, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 50, 390, 100, 20, 3000363, LabelColor, false, false ); // Close

			for ( int i = 0; i < entries.Length; ++i )
			{
				int page = 1 + (i / 14);
				int index = i % 14;

				if ( index == 0 )
				{
					if ( page > 1 )
					{
						AddButton( 450, 390, 4005, 4007, 0, GumpButtonType.Page, page );
						AddHtmlLocalized( 400, 390, 100, 20, 3000406, LabelColor, false, false ); // Next
					}

					AddPage( page );

					if ( page > 1 )
					{
						AddButton( 200, 390, 4014, 4016, 0, GumpButtonType.Page, page - 1 );
						AddHtmlLocalized( 250, 390, 100, 20, 3000405, LabelColor, false, false ); // Previous
					}
				}

				HouseSurveyEntry entry = entries[i];

				int y = 70 + (index * 20);

				AddButton( 10, y, 4005, 4007, 1 + i, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 50, y, 225, 20, entry.Description, LabelColor, false, false );
				AddLabel( 275, y, LabelHue, entry.Storage.ToString() );
				AddLabel( 350, y, LabelHue, entry.Lockdowns.ToString() );
				AddLabel( 425, y, LabelHue, entry.Cost.ToString() );
			}
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( !m_From.CheckAlive() || m_From.Backpack == null || m_Tool == null || m_Tool.UsesRemaining == 0 || !m_Tool.IsChildOf( m_From.Backpack )  )
				return;

			int index = info.ButtonID - 1;

			if ( index >= 0 && index < m_Entries.Length )
				m_From.Target = new NewHouseSurveyTarget( m_Tool, m_Entries, m_Entries[index] );
			else
				m_From.SendGump( new HouseSurveyCategoryGump( m_From, m_Tool ) );
		}
	}

	public class NewHouseSurveyTarget : MultiTarget
	{
		private HouseSurveyEntry m_Entry;
		private HouseSurveyEntry[] m_Entries;

		private bool m_Placed;
		private HouseSurveyTool m_Tool;

		public NewHouseSurveyTarget( HouseSurveyTool tool, HouseSurveyEntry[] entries, HouseSurveyEntry entry ) : base( entry.MultiID, entry.Offset )
		{
			m_Tool = tool;

			Range = 14;

			m_Entries = entries;
			m_Entry = entry;
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if ( !from.CheckAlive() || from.Backpack == null || m_Tool == null || m_Tool.UsesRemaining == 0 || !m_Tool.IsChildOf( from.Backpack )  )
				return;

			IPoint3D ip = o as IPoint3D;

			if ( ip != null )
			{
				if ( ip is Item )
					ip = ((Item)ip).GetWorldTop();

				Point3D p = new Point3D( ip );

				Region reg = Region.Find( new Point3D( p ), from.Map );

				if ( from.AccessLevel >= AccessLevel.GameMaster || reg.AllowHousing( from, p ) )
					m_Placed = m_Entry.OnPlacement( from, m_Tool, p );
				else if ( reg.IsPartOf( typeof( TreasureRegion ) ) || reg.IsPartOf( typeof( HouseRegion ) ) )
					from.SendLocalizedMessage( 1043287 ); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
				else
					from.SendLocalizedMessage( 501265 ); // Housing can not be created in this area.

				if ( m_Tool.UsesRemaining > 0 )
				{
					m_Tool.UsesRemaining--;
					from.SendMessage( "Your house survey tool lost part of its charge." );
				}
			}
		}

		protected override void OnTargetFinish( Mobile from )
		{
			if ( !from.CheckAlive() || from.Backpack == null || m_Tool == null || m_Tool.UsesRemaining == 0 || !m_Tool.IsChildOf( from.Backpack )  )
				return;

			if ( !m_Placed )
				from.SendGump( new HouseSurveyListGump( from, m_Tool, m_Entries ) );
		}
	}

	public class HouseSurveyEntry
	{
		private Type m_Type;
		private int m_Description;
		private int m_Storage;
		private int m_Lockdowns;
		private int m_NewStorage;
		private int m_NewLockdowns;
		private int m_Vendors;
		private int m_Cost;
		private int m_MultiID;
		private Point3D m_Offset;

		public Type Type{ get{ return m_Type; } }

		public int Description{ get{ return m_Description; } }
		public int Storage{ get{ return BaseHouse.NewVendorSystem ? m_NewStorage : m_Storage; } }
		public int Lockdowns{ get{ return BaseHouse.NewVendorSystem ? m_NewLockdowns : m_Lockdowns; } }
		public int Vendors{ get{ return m_Vendors; } }
		public int Cost{ get{ return m_Cost; } }

		public int MultiID{ get{ return m_MultiID; } }
		public Point3D Offset{ get{ return m_Offset; } }

		public HouseSurveyEntry( Type type, int description, int storage, int lockdowns, int newStorage, int newLockdowns, int vendors, int cost, int xOffset, int yOffset, int zOffset, int multiID )
		{
			m_Type = type;
			m_Description = description;
			m_Storage = storage;
			m_Lockdowns = lockdowns;
			m_NewStorage = newStorage;
			m_NewLockdowns = newLockdowns;
			m_Vendors = vendors;
			m_Cost = cost;

			m_Offset = new Point3D( xOffset, yOffset, zOffset );

			m_MultiID = multiID;
		}

		public void SurveyNotice_Callback( Mobile from, object state )
		{
			SurveyPreviewHouse prev = state as SurveyPreviewHouse;
			if ( state != null )
				prev.Delete();
		}

		public bool OnPlacement( Mobile from, HouseSurveyTool tool, Point3D p )
		{
			if ( !from.CheckAlive() || from.Backpack == null || tool == null || tool.UsesRemaining == 0 || !tool.IsChildOf( from.Backpack )  )
				return false;

			if ( SurveyPreviewHouse.PreviewHouseList.Contains( from ) )
			{
				from.SendMessage( "You are already previewing a potential house location." );
				return false;
			}

			ArrayList toMove;
			Point3D center = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );
			HousePlacementResult res = HousePlacement.Check( from, m_MultiID, center, out toMove, false );

			switch ( res )
			{
				case HousePlacementResult.Valid:
				{
					from.SendMessage( "This is valid terrain to place a house." ); // This is a valid location.

					SurveyPreviewHouse prev = new SurveyPreviewHouse( m_MultiID, from );

					MultiComponentList mcl = prev.Components;

					prev.MoveToWorld( center, from.Map );

					string message = "You have found valid terrain to place a house.  This preview will stay visible for 20 seconds.<BR><BR>Press OKAY to stop previewing this house.";

					from.SendGump( new NoticeGump( 1060635, 30720, message, 0xFFC000, 420, 280, new NoticeGumpCallback( SurveyNotice_Callback ), prev ) );

					return true;
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
					from.SendLocalizedMessage( 1043287 ); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
					break;
				}
				case HousePlacementResult.BadRegion:
				{
					from.SendLocalizedMessage( 501265 ); // Housing cannot be created in this area.
					break;
				}
				case HousePlacementResult.BadRegionTemp:
				{
					from.SendLocalizedMessage( 501270 ); //Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
					break;
				}
			}

			return false;
		}

		private static Hashtable m_Table;

		static HouseSurveyEntry()
		{
			m_Table = new Hashtable();

			FillTable( m_ClassicHouses );
			FillTable( m_TwoStoryFoundations );
			FillTable( m_ThreeStoryFoundations );
		}

		public static HouseSurveyEntry Find( BaseHouse house )
		{
			object obj = m_Table[house.GetType()];

			if ( obj is HouseSurveyEntry )
			{
				return ((HouseSurveyEntry)obj);
			}
			else if ( obj is ArrayList )
			{
				ArrayList list = (ArrayList)obj;

				for ( int i = 0; i < list.Count; ++i )
				{
					HouseSurveyEntry e = (HouseSurveyEntry)list[i];

					if ( e.m_MultiID == house.ItemID )
						return e;
				}
			}
			else if ( obj is Hashtable )
			{
				Hashtable table = (Hashtable)obj;

				obj = table[house.ItemID];

				if ( obj is HouseSurveyEntry )
					return (HouseSurveyEntry)obj;
			}

			return null;
		}

		private static void FillTable( HouseSurveyEntry[] entries )
		{
			for ( int i = 0; i < entries.Length; ++i )
			{
				HouseSurveyEntry e = entries[i];

				object obj = m_Table[e.m_Type];

				if ( obj == null )
				{
					m_Table[e.m_Type] = e;
				}
				else if ( obj is HouseSurveyEntry )
				{
					ArrayList list = new ArrayList();

					list.Add( obj );
					list.Add( e );

					m_Table[e.m_Type] = list;
				}
				else if ( obj is ArrayList )
				{
					ArrayList list = (ArrayList)obj;

					if ( list.Count == 8 )
					{
						Hashtable table = new Hashtable();

						for ( int j = 0; j < list.Count; ++j )
							table[((HouseSurveyEntry)list[j]).m_MultiID] = list[j];

						table[e.m_MultiID] = e;

						m_Table[e.m_Type] = table;
					}
					else
					{
						list.Add( e );
					}
				}
				else if ( obj is Hashtable )
				{
					((Hashtable)obj)[e.m_MultiID] = e;
				}
			}
		}

		private static HouseSurveyEntry[] m_ClassicHouses = new HouseSurveyEntry[]
			{
				new HouseSurveyEntry( typeof( SmallOldHouse ),		1011303,	425,	212,	489,	244,	10,	37000,		0,	4,	0,	0x0064	),
				new HouseSurveyEntry( typeof( SmallOldHouse ),		1011304,	425,	212,	489,	244,	10,	37000,		0,	4,	0,	0x0066	),
				new HouseSurveyEntry( typeof( SmallOldHouse ),		1011305,	425,	212,	489,	244,	10,	36750,		0,	4,	0,	0x0068	),
				new HouseSurveyEntry( typeof( SmallOldHouse ),		1011306,	425,	212,	489,	244,	10,	35250,		0,	4,	0,	0x006A	),
				new HouseSurveyEntry( typeof( SmallOldHouse ),		1011307,	425,	212,	489,	244,	10,	36750,		0,	4,	0,	0x006C	),
				new HouseSurveyEntry( typeof( SmallOldHouse ),		1011308,	425,	212,	489,	244,	10,	36750,		0,	4,	0,	0x006E	),
				new HouseSurveyEntry( typeof( SmallShop ),			1011321,	425,	212,	489,	244,	10,	50500,	   -1,	4,	0,	0x00A0	),
				new HouseSurveyEntry( typeof( SmallShop ),			1011322,	425,	212,	489,	244,	10,	52500,		0,	4,	0,	0x00A2	),
				new HouseSurveyEntry( typeof( SmallTower ),			1011317,	580,	290,	667,	333,	14,	73500,		3,	4,	0,	0x0098	),
				new HouseSurveyEntry( typeof( TwoStoryVilla ),		1011319,	1100,	550,	1265,	632,	24,	113750,		3,	6,	0,	0x009E	),
				new HouseSurveyEntry( typeof( SandStonePatio ),		1011320,	850,	425,	1265,	632,	24,	76500,	   -1,	4,	0,	0x009C	),
				new HouseSurveyEntry( typeof( LogCabin ),			1011318,	1100,	550,	1265,	632,	24,	81750,		1,	6,	0,	0x009A	),
				new HouseSurveyEntry( typeof( GuildHouse ),			1011309,	1370,	685,	1576,	788,	28,	131500,	   -1,	7,	0,	0x0074	),
				new HouseSurveyEntry( typeof( TwoStoryHouse ),		1011310,	1370,	685,	1576,	788,	28,	162750,	   -3,	7,	0,	0x0076	),
				new HouseSurveyEntry( typeof( TwoStoryHouse ),		1011311,	1370,	685,	1576,	788,	28,	162000,	   -3,	7,	0,	0x0078	),
				new HouseSurveyEntry( typeof( LargePatioHouse ),		1011315,	1370,	685,	1576,	788,	28,	129250,	   -4,	7,	0,	0x008C	),
				new HouseSurveyEntry( typeof( LargeMarbleHouse ),	1011316,	1370,	685,	1576,	788,	28,	160500,	   -4,	7,	0,	0x0096	),
				new HouseSurveyEntry( typeof( Tower ),				1011312,	2119,	1059,	2437,	1218,	42,	366500,		0,	7,	0,	0x007A	),
				new HouseSurveyEntry( typeof( Keep ),				1011313,	2625,	1312,	3019,	1509,	52,	572750,		0, 11,	0,	0x007C	),
				new HouseSurveyEntry( typeof( Castle ),				1011314,	4076,	2038,	4688,	2344,	78,	865250,		0, 16,	0,	0x007E	)
			};

		public static HouseSurveyEntry[] ClassicHouses{ get{ return m_ClassicHouses; } }





		private static HouseSurveyEntry[] m_TwoStoryFoundations = new HouseSurveyEntry[]
			{
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060241,	425,	212,	489,	244,	10,	30500,		0,	4,	0,	0x13EC	), // 7x7 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060242,	580,	290,	667,	333,	14,	34500,		0,	5,	0,	0x13ED	), // 7x8 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060243,	650,	325,	748,	374,	16,	38500,		0,	5,	0,	0x13EE	), // 7x9 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060244,	700,	350,	805,	402,	16,	42500,		0,	6,	0,	0x13EF	), // 7x10 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060245,	750,	375,	863,	431,	16,	46500,		0,	6,	0,	0x13F0	), // 7x11 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060246,	800,	400,	920,	460,	18,	50500,		0,	7,	0,	0x13F1	), // 7x12 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060253,	580,	290,	667,	333,	14,	34500,		0,	4,	0,	0x13F8	), // 8x7 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060254,	650,	325,	748,	374,	16,	39000,		0,	5,	0,	0x13F9	), // 8x8 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060255,	700,	350,	805,	402,	16,	43500,		0,	5,	0,	0x13FA	), // 8x9 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060256,	750,	375,	863,	431,	16,	48000,		0,	6,	0,	0x13FB	), // 8x10 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060257,	800,	400,	920,	460,	18,	52500,		0,	6,	0,	0x13FC	), // 8x11 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060258,	850,	425,	1265,	632,	24,	57000,		0,	7,	0,	0x13FD	), // 8x12 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060259,	1100,	550,	1265,	632,	24,	61500,		0,	7,	0,	0x13FE	), // 8x13 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060265,	650,	325,	748,	374,	16,	38500,		0,	4,	0,	0x1404	), // 9x7 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060266,	700,	350,	805,	402,	16,	43500,		0,	5,	0,	0x1405	), // 9x8 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060267,	750,	375,	863,	431,	16,	48500,		0,	5,	0,	0x1406	), // 9x9 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060268,	800,	400,	920,	460,	18,	53500,		0,	6,	0,	0x1407	), // 9x10 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060269,	850,	425,	1265,	632,	24,	58500,		0,	6,	0,	0x1408	), // 9x11 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060270,	1100,	550,	1265,	632,	24,	63500,		0,	7,	0,	0x1409	), // 9x12 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060271,	1100,	550,	1265,	632,	24,	68500,		0,	7,	0,	0x140A	), // 9x13 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060277,	700,	350,	805,	402,	16,	42500,		0,	4,	0,	0x1410	), // 10x7 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060278,	750,	375,	863,	431,	16,	48000,		0,	5,	0,	0x1411	), // 10x8 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060279,	800,	400,	920,	460,	18,	53500,		0,	5,	0,	0x1412	), // 10x9 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060280,	850,	425,	1265,	632,	24,	59000,		0,	6,	0,	0x1413	), // 10x10 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060281,	1100,	550,	1265,	632,	24,	64500,		0,	6,	0,	0x1414	), // 10x11 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060282,	1100,	550,	1265,	632,	24,	70000,		0,	7,	0,	0x1415	), // 10x12 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060283,	1150,	575,	1323,	661,	24,	75500,		0,	7,	0,	0x1416	), // 10x13 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060289,	750,	375,	863,	431,	16,	46500,		0,	4,	0,	0x141C	), // 11x7 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060290,	800,	400,	920,	460,	18,	52500,		0,	5,	0,	0x141D	), // 11x8 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060291,	850,	425,	1265,	632,	24,	58500,		0,	5,	0,	0x141E	), // 11x9 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060292,	1100,	550,	1265,	632,	24,	64500,		0,	6,	0,	0x141F	), // 11x10 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060293,	1100,	550,	1265,	632,	24,	70500,		0,	6,	0,	0x1420	), // 11x11 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060294,	1150,	575,	1323,	661,	24,	76500,		0,	7,	0,	0x1421	), // 11x12 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060295,	1200,	600,	1380,	690,	26,	82500,		0,	7,	0,	0x1422	), // 11x13 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060301,	800,	400,	920,	460,	18,	50500,		0,	4,	0,	0x1428	), // 12x7 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060302,	850,	425,	1265,	632,	24,	57000,		0,	5,	0,	0x1429	), // 12x8 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060303,	1100,	550,	1265,	632,	24,	63500,		0,	5,	0,	0x142A	), // 12x9 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060304,	1100,	550,	1265,	632,	24,	70000,		0,	6,	0,	0x142B	), // 12x10 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060305,	1150,	575,	1323,	661,	24,	76500,		0,	6,	0,	0x142C	), // 12x11 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060306,	1200,	600,	1380,	690,	26,	83000,		0,	7,	0,	0x142D	), // 12x12 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060307,	1250,	625,	1438,	719,	26,	89500,		0,	7,	0,	0x142E	), // 12x13 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060314,	1100,	550,	1265,	632,	24,	61500,		0,	5,	0,	0x1435	), // 13x8 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060315,	1100,	550,	1265,	632,	24,	68500,		0,	5,	0,	0x1436	), // 13x9 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060316,	1150,	575,	1323,	661,	24,	75500,		0,	6,	0,	0x1437	), // 13x10 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060317,	1200,	600,	1380,	690,	26,	82500,		0,	6,	0,	0x1438	), // 13x11 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060318,	1250,	625,	1438,	719,	26,	89500,		0,	7,	0,	0x1439	), // 13x12 2-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060319,	1300,	650,	1495,	747,	28,	96500,		0,	7,	0,	0x143A	)  // 13x13 2-Story Customizable House
			};

		public static HouseSurveyEntry[] TwoStoryFoundations{ get{ return m_TwoStoryFoundations; } }





		private static HouseSurveyEntry[] m_ThreeStoryFoundations = new HouseSurveyEntry[]
			{
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060272,	1150,	575,	1323,	661,	24,	73500,		0,	8,	0,	0x140B	), // 9x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060284,	1200,	600,	1380,	690,	26,	81000,		0,	8,	0,	0x1417	), // 10x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060285,	1250,	625,	1438,	719,	26,	86500,		0,	8,	0,	0x1418	), // 10x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060296,	1250,	625,	1438,	719,	26,	88500,		0,	8,	0,	0x1423	), // 11x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060297,	1300,	650,	1495,	747,	28,	94500,		0,	8,	0,	0x1424	), // 11x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060298,	1350,	675,	1553,	776,	28,	100500,		0,	9,	0,	0x1425	), // 11x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060308,	1300,	650,	1495,	747,	28,	96000,		0,	8,	0,	0x142F	), // 12x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060309,	1350,	675,	1553,	776,	28,	102500,		0,	8,	0,	0x1430	), // 12x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060310,	1370,	685,	1576,	788,	28,	109000,		0,	9,	0,	0x1431	), // 12x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060311,	1370,	685,	1576,	788,	28,	115500,		0,	9,	0,	0x1432	), // 12x17 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060320,	1350,	675,	1553,	776,	28,	103500,		0,	8,	0,	0x143B	), // 13x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060321,	1370,	685,	1576,	788,	28,	110500,		0,	8,	0,	0x143C	), // 13x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060322,	1370,	685,	1576,	788,	28,	117500,		0,	9,	0,	0x143D	), // 13x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060323,	2119,	1059,	2437,	1218,	42,	124500,		0,	9,	0,	0x143E	), // 13x17 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060324,	2119,	1059,	2437,	1218,	42,	131500,		0,	10,	0,	0x143F	), // 13x18 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060327,	1150,	575,	1323,	661,	24,	73500,		0,	5,	0,	0x1442	), // 14x9 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060328,	1200,	600,	1380,	690,	26,	81000,		0,	6,	0,	0x1443	), // 14x10 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060329,	1250,	625,	1438,	719,	26,	88500,		0,	6,	0,	0x1444	), // 14x11 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060330,	1300,	650,	1495,	747,	28,	96000,		0,	7,	0,	0x1445	), // 14x12 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060331,	1350,	675,	1553,	776,	28,	103500,		0,	7,	0,	0x1446	), // 14x13 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060332,	1370,	685,	1576,	788,	28,	111000,		0,	8,	0,	0x1447	), // 14x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060333,	1370,	685,	1576,	788,	28,	118500,		0,	8,	0,	0x1448	), // 14x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060334,	2119,	1059,	2437,	1218,	42,	126000,		0,	9,	0,	0x1449	), // 14x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060335,	2119,	1059,	2437,	1218,	42,	133500,		0,	9,	0,	0x144A	), // 14x17 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060336,	2119,	1059,	2437,	1218,	42,	141000,		0,	10,	0,	0x144B	), // 14x18 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060340,	1250,	625,	1438,	719,	26,	86500,		0,	6,	0,	0x144F	), // 15x10 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060341,	1300,	650,	1495,	747,	28,	94500,		0,	6,	0,	0x1450	), // 15x11 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060342,	1350,	675,	1553,	776,	28,	102500,		0,	7,	0,	0x1451	), // 15x12 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060343,	1370,	685,	1576,	788,	28,	110500,		0,	7,	0,	0x1452	), // 15x13 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060344,	1370,	685,	1576,	788,	28,	118500,		0,	8,	0,	0x1453	), // 15x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060345,	2119,	1059,	2437,	1218,	42,	126500,		0,	8,	0,	0x1454	), // 15x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060346,	2119,	1059,	2437,	1218,	42,	134500,		0,	9,	0,	0x1455	), // 15x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060347,	2119,	1059,	2437,	1218,	42,	142500,		0,	9,	0,	0x1456	), // 15x17 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060348,	2119,	1059,	2437,	1218,	42,	150500,		0,	10,	0,	0x1457	), // 15x18 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060353,	1350,	675,	1553,	776,	28,	100500,		0,	6,	0,	0x145C	), // 16x11 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060354,	1370,	685,	1576,	788,	28,	109000,		0,	7,	0,	0x145D	), // 16x12 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060355,	1370,	685,	1576,	788,	28,	117500,		0,	7,	0,	0x145E	), // 16x13 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060356,	2119,	1059,	2437,	1218,	42,	126000,		0,	8,	0,	0x145F	), // 16x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060357,	2119,	1059,	2437,	1218,	42,	134500,		0,	8,	0,	0x1460	), // 16x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060358,	2119,	1059,	2437,	1218,	42,	143000,		0,	9,	0,	0x1461	), // 16x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060359,	2119,	1059,	2437,	1218,	42,	151500,		0,	9,	0,	0x1462	), // 16x17 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060360,	2119,	1059,	2437,	1218,	42,	160000,		0,	10,	0,	0x1463	), // 16x18 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060366,	1370,	685,	1576,	788,	28,	115500,		0,	7,	0,	0x1469	), // 17x12 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060367,	2119,	1059,	2437,	1218,	42,	124500,		0,	7,	0,	0x146A	), // 17x13 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060368,	2119,	1059,	2437,	1218,	42,	133500,		0,	8,	0,	0x146B	), // 17x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060369,	2119,	1059,	2437,	1218,	42,	142500,		0,	8,	0,	0x146C	), // 17x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060370,	2119,	1059,	2437,	1218,	42,	151500,		0,	9,	0,	0x146D	), // 17x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060371,	2119,	1059,	2437,	1218,	42,	160500,		0,	9,	0,	0x146E	), // 17x17 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060372,	2119,	1059,	2437,	1218,	42,	169500,		0,	10,	0,	0x146F	), // 17x18 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060379,	2119,	1059,	2437,	1218,	42,	131500,		0,	7,	0,	0x1476	), // 18x13 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060380,	2119,	1059,	2437,	1218,	42,	141000,		0,	8,	0,	0x1477	), // 18x14 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060381,	2119,	1059,	2437,	1218,	42,	150500,		0,	8,	0,	0x1478	), // 18x15 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060382,	2119,	1059,	2437,	1218,	42,	160000,		0,	9,	0,	0x1479	), // 18x16 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060383,	2119,	1059,	2437,	1218,	42,	169500,		0,	9,	0,	0x147A	), // 18x17 3-Story Customizable House
				new HouseSurveyEntry( typeof( HouseFoundation ),		1060384,	2119,	1059,	2437,	1218,	42,	179000,		0,	10,	0,	0x147B	)  // 18x18 3-Story Customizable House
			};

		public static HouseSurveyEntry[] ThreeStoryFoundations{ get{ return m_ThreeStoryFoundations; } }
	}
}