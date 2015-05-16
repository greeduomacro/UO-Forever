using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
	public class CarpetAddon : BaseAddon
	{
		private int m_Width;
		private int m_Height;
		private CarpetType m_Type;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Width{ get{ return m_Width; } set{ m_Width = value; UpdateComponents(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Height{ get{ return m_Height; } set{ m_Height = value; UpdateComponents(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public CarpetType Type{ get{ return m_Type; } set{ m_Type = value; UpdateComponents(); } }

		public override BaseAddonDeed Deed{ get{ return new CarpetAddonDeed( m_Type, m_Width, m_Height ); } }

		[Constructable]
		public CarpetAddon( CarpetType type, int width, int height ) : base()
		{
			m_Type = type;
			m_Width = width;
			m_Height = height;

			UpdateComponents();
		}

		public void UpdateComponents()
		{
			for ( int i = Components.Count-1; i >= 0; i-- )
				if ( Components[i] != null && !Components[i].Deleted )
					Components[i].Delete();

			Components.Clear();

			CarpetInfo info = CarpetInfo.GetInfo( m_Type );

			AddComponent( new AddonComponent( info.GetCarpetPart( Position.Top ).ItemID ), 0, 0, 0 );
			AddComponent( new AddonComponent( info.GetCarpetPart( Position.Right ).ItemID ), m_Width, 0, 0 );
			AddComponent( new AddonComponent( info.GetCarpetPart( Position.Left ).ItemID ), 0, m_Height, 0 );
			AddComponent( new AddonComponent( info.GetCarpetPart( Position.Bottom ).ItemID ), m_Width, m_Height, 0 );

			int w = m_Width - 1;
			int h = m_Height - 1;

			for ( int y = 1; y <= h; ++y )
			{
				AddComponent( new AddonComponent( info.GetCarpetPart( Position.West ).ItemID ), 0, y, 0 );
				AddComponent( new AddonComponent( info.GetCarpetPart( Position.East ).ItemID ), m_Width, y, 0 );
			}

			for ( int x = 1; x <= w; ++x )
			{
				AddComponent( new AddonComponent( info.GetCarpetPart( Position.North ).ItemID ), x, 0, 0 );
				AddComponent( new AddonComponent( info.GetCarpetPart( Position.South ).ItemID ), x, m_Height, 0 );
			}

			for ( int x = 1; x <= w; ++x )
				for ( int y = 1; y <= h; ++y )
					AddComponent( new AddonComponent( info.GetCarpetPart( Position.Center ).ItemID ), x, y, 0 );
		}

		public CarpetAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int)m_Type );
			writer.Write( m_Width );
			writer.Write( m_Height );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Type = (CarpetType)reader.ReadEncodedInt();
			m_Width = reader.ReadInt();
			m_Height = reader.ReadInt();
		}
	}

	public enum CarpetType
	{
		PlainBlue,
		FloweredBlueNorth,
		FloweredBlueEast,
		PlainRed,
		FloweredRedNorth,
		FloweredRedEast,
		OrangeBlue1,
		OrangeBlue2,
		GoldBlue,
		BrownRed,
		GoldRed
	}

	public enum Position
	{
		Top,
		Bottom,
		Left,
		Right,
		West,
		North,
		East,
		South,
		Center
	}

	public class CarpetInfo
	{
		private string m_Name;
		private CarpetPart[] m_Entries;

		public string Name{ get{ return m_Name; } }
		public CarpetPart[] Entries{ get{ return m_Entries; } }

		public CarpetInfo( string name, params CarpetPart[] entries )
		{
			m_Name = name;
			m_Entries = entries;
		}

		public CarpetPart GetCarpetPart( Position pos )
		{
			int i = (int)pos;

			if ( i < 0 || i >= m_Entries.Length )
				i = 0;

			return m_Entries[i];
		}

		public static CarpetInfo GetInfo( CarpetType type )
		{
			int index = (int)type;

			if ( index < 0 || index >= m_Infos.Length )
				index = 0;

			return m_Infos[index];
		}

		private static CarpetInfo[] m_Infos = new CarpetInfo[] {
					new CarpetInfo( "plain blue carpet",
						new CarpetPart( 0xAC3, Position.Top ),
						new CarpetPart( 0xAC2, Position.Bottom ),
						new CarpetPart( 0xAC4, Position.Left ),
						new CarpetPart( 0xAC5, Position.Right ),
						new CarpetPart( 0xAF6, Position.West ),
						new CarpetPart( 0xAF7, Position.North ),
						new CarpetPart( 0xAF8, Position.East ),
						new CarpetPart( 0xAF9, Position.South ),
						new CarpetPart( 0xABE, Position.Center )
					),
					new CarpetInfo( "blue flower pattern carpet (north)",
						new CarpetPart( 0xAC3, Position.Top ),
						new CarpetPart( 0xAC2, Position.Bottom ),
						new CarpetPart( 0xAC4, Position.Left ),
						new CarpetPart( 0xAC5, Position.Right ),
						new CarpetPart( 0xAF6, Position.West ),
						new CarpetPart( 0xAF7, Position.North ),
						new CarpetPart( 0xAF8, Position.East ),
						new CarpetPart( 0xAF9, Position.South ),
						new CarpetPart( 0xABD, Position.Center )
					),
					new CarpetInfo( "blue flower pattern carpet (east)",
						new CarpetPart( 0xAC3, Position.Top ),
						new CarpetPart( 0xAC2, Position.Bottom ),
						new CarpetPart( 0xAC4, Position.Left ),
						new CarpetPart( 0xAC5, Position.Right ),
						new CarpetPart( 0xAF6, Position.West ),
						new CarpetPart( 0xAF7, Position.North ),
						new CarpetPart( 0xAF8, Position.East ),
						new CarpetPart( 0xAF9, Position.South ),
						new CarpetPart( 0xABF, Position.Center )
					),
					new CarpetInfo( "plain red carpet",
						new CarpetPart( 0xACA, Position.Top ),
						new CarpetPart( 0xAC9, Position.Bottom ),
						new CarpetPart( 0xACB, Position.Left ),
						new CarpetPart( 0xACC, Position.Right ),
						new CarpetPart( 0xACD, Position.West ),
						new CarpetPart( 0xACE, Position.North ),
						new CarpetPart( 0xACF, Position.East ),
						new CarpetPart( 0xAD0, Position.South ),
						new CarpetPart( 0xAC8, Position.Center )
					),
					new CarpetInfo( "red flower pattern carpet (north)",
						new CarpetPart( 0xACA, Position.Top ),
						new CarpetPart( 0xAC9, Position.Bottom ),
						new CarpetPart( 0xACB, Position.Left ),
						new CarpetPart( 0xACC, Position.Right ),
						new CarpetPart( 0xACD, Position.West ),
						new CarpetPart( 0xACE, Position.North ),
						new CarpetPart( 0xACF, Position.East ),
						new CarpetPart( 0xAD0, Position.South ),
						new CarpetPart( 0xAC7, Position.Center )
					),
					new CarpetInfo( "red flower pattern carpet (east)",
						new CarpetPart( 0xACA, Position.Top ),
						new CarpetPart( 0xAC9, Position.Bottom ),
						new CarpetPart( 0xACB, Position.Left ),
						new CarpetPart( 0xACC, Position.Right ),
						new CarpetPart( 0xACD, Position.West ),
						new CarpetPart( 0xACE, Position.North ),
						new CarpetPart( 0xACF, Position.East ),
						new CarpetPart( 0xAD0, Position.South ),
						new CarpetPart( 0xAC6, Position.Center )
					),
					new CarpetInfo( "orange cross pattern carpet",
						new CarpetPart( 0xAEF, Position.Top ),
						new CarpetPart( 0xAEE, Position.Bottom ),
						new CarpetPart( 0xAF0, Position.Left ),
						new CarpetPart( 0xAF1, Position.Right ),
						new CarpetPart( 0xAF2, Position.West ),
						new CarpetPart( 0xAF3, Position.North ),
						new CarpetPart( 0xAF4, Position.East ),
						new CarpetPart( 0xAF5, Position.South ),
						new CarpetPart( 0xAEC, Position.Center )
					),
					new CarpetInfo( "orange star pattern carpet",
						new CarpetPart( 0xAEF, Position.Top ),
						new CarpetPart( 0xAEE, Position.Bottom ),
						new CarpetPart( 0xAF0, Position.Left ),
						new CarpetPart( 0xAF1, Position.Right ),
						new CarpetPart( 0xAF2, Position.West ),
						new CarpetPart( 0xAF3, Position.North ),
						new CarpetPart( 0xAF4, Position.East ),
						new CarpetPart( 0xAF5, Position.South ),
						new CarpetPart( 0xAED, Position.Center )
					),
					new CarpetInfo( "gold pattern blue carpet",
						new CarpetPart( 0xAD3, Position.Top ),
						new CarpetPart( 0xAD2, Position.Bottom ),
						new CarpetPart( 0xAD4, Position.Left ),
						new CarpetPart( 0xAD5, Position.Right ),
						new CarpetPart( 0xAD6, Position.West ),
						new CarpetPart( 0xAD7, Position.North ),
						new CarpetPart( 0xAD8, Position.East ),
						new CarpetPart( 0xAD9, Position.South ),
						new CarpetPart( 0xAD1, Position.Center )
					),
					new CarpetInfo( "brown pattern carpet",
						new CarpetPart( 0xAE4, Position.Top ),
						new CarpetPart( 0xAE3, Position.Bottom ),
						new CarpetPart( 0xAE5, Position.Left ),
						new CarpetPart( 0xAE6, Position.Right ),
						new CarpetPart( 0xAE7, Position.West ),
						new CarpetPart( 0xAE8, Position.North ),
						new CarpetPart( 0xAE9, Position.East ),
						new CarpetPart( 0xAEA, Position.South ),
						new CarpetPart( 0xAEB, Position.Center )
					),
					new CarpetInfo( "gold pattern red carpet",
						new CarpetPart( 0xADC, Position.Top ),
						new CarpetPart( 0xADB, Position.Bottom ),
						new CarpetPart( 0xADD, Position.Left ),
						new CarpetPart( 0xADE, Position.Right ),
						new CarpetPart( 0xADF, Position.West ),
						new CarpetPart( 0xAE0, Position.North ),
						new CarpetPart( 0xAE1, Position.East ),
						new CarpetPart( 0xAE2, Position.South ),
						new CarpetPart( 0xADA, Position.Center )
					)
			};

		public static CarpetInfo[] Infos{ get{ return m_Infos; } }
	}

	public class CarpetPart
	{
		private int m_ItemID;
		private  Position m_Info;

		public int ItemID{ get{ return m_ItemID; } }
		public  Position Position{ get{ return m_Info; } }

		public CarpetPart( int itemID,  Position info )
		{
			m_ItemID = itemID;
			m_Info = info;
		}
	}

	public class CarpetAddonDeed : BaseAddonDeed
	{
		private int m_Width;
		private int m_Height;
		private CarpetType m_Type;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Width{ get{ return m_Width; } set{ m_Width = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Height{ get{ return m_Height; } set{ m_Height = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public CarpetType Type{ get{ return m_Type; } set{ m_Type = value; } }

		public override BaseAddon Addon{ get{ return new CarpetAddon( m_Type, m_Width, m_Height ); } }
		public override string DefaultName{ get{ return String.Format( "a deed for a {0}x{1} {2}", m_Width, m_Height, CarpetInfo.GetInfo( m_Type ).Name ); } }

		[Constructable]
		public CarpetAddonDeed( CarpetType type, int width, int height )
		{
			m_Type = type;
			m_Width = width;
			m_Height = height;
		}

		public CarpetAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.WriteEncodedInt( (int)m_Type );
			writer.Write( m_Width );
			writer.Write( m_Height );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Type = (CarpetType)reader.ReadEncodedInt();
			m_Width = reader.ReadInt();
			m_Height = reader.ReadInt();
		}
	}
}