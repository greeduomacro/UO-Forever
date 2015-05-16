/***************************************************************************
 *                               MultiData.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: MultiData.cs 644 2010-12-23 09:18:45Z asayre $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Server
{
	public static class MultiData
	{
		private static MultiComponentList[] m_Components;

		private static FileStream m_Index, m_Stream;
		private static BinaryReader m_IndexReader, m_StreamReader;

		public static MultiComponentList GetComponents( int multiID )
		{
			MultiComponentList mcl;

			if ( multiID >= 0 && multiID < m_Components.Length )
			{
				mcl = m_Components[multiID];

				if ( mcl == null )
					m_Components[multiID] = mcl = Load( multiID );
			}
			else
			{
				mcl = MultiComponentList.Empty;
			}

			return mcl;
		}
        public static int currentMultiID = 24; // ALAN TEMP MOD
		public static MultiComponentList Load( int multiID )
		{
			try
			{
				m_IndexReader.BaseStream.Seek( multiID * 12, SeekOrigin.Begin );

				int lookup = m_IndexReader.ReadInt32();
				int length = m_IndexReader.ReadInt32();

				if ( lookup < 0 || length <= 0 )
					return MultiComponentList.Empty;

				m_StreamReader.BaseStream.Seek( lookup, SeekOrigin.Begin );
                currentMultiID = multiID; // ALAN TEMP MOD
				return new MultiComponentList( m_StreamReader, length / ( MultiComponentList.PostHSFormat ? 16 : 12 ) );
			}
			catch
			{
				return MultiComponentList.Empty;
			}
		}

		static MultiData()
		{
			string idxPath = Core.FindDataFile( "multi.idx" );
			string mulPath = Core.FindDataFile( "multi.mul" );

			if ( File.Exists( idxPath ) && File.Exists( mulPath ) )
			{
				m_Index = new FileStream( idxPath, FileMode.Open, FileAccess.Read, FileShare.Read );
				m_IndexReader = new BinaryReader( m_Index );

				m_Stream = new FileStream( mulPath, FileMode.Open, FileAccess.Read, FileShare.Read );
				m_StreamReader = new BinaryReader( m_Stream );

				m_Components = new MultiComponentList[(int)(m_Index.Length / 12)];

				string vdPath = Core.FindDataFile( "verdata.mul" );

				if ( File.Exists( vdPath ) )
				{
					using ( FileStream fs = new FileStream( vdPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
					{
						BinaryReader bin = new BinaryReader( fs );

						int count = bin.ReadInt32();

						for ( int i = 0; i < count; ++i )
						{
							int file = bin.ReadInt32();
							int index = bin.ReadInt32();
							int lookup = bin.ReadInt32();
							int length = bin.ReadInt32();
							int extra = bin.ReadInt32();

							if ( file == 14 && index >= 0 && index < m_Components.Length && lookup >= 0 && length > 0 )
							{
								bin.BaseStream.Seek( lookup, SeekOrigin.Begin );

								m_Components[index] = new MultiComponentList( bin, length / 12 );

								bin.BaseStream.Seek( 24 + (i * 20), SeekOrigin.Begin );
							}
						}

						bin.Close();
					}
				}
			}
			else
			{
				Console.WriteLine( "Warning: Multi data files not found" );

				m_Components = new MultiComponentList[0];
			}
		}
	}

	public struct MultiTileEntry
	{
		public ushort m_ItemID;
		public short m_OffsetX, m_OffsetY, m_OffsetZ;
		public int m_Flags;

		public MultiTileEntry( ushort itemID, short xOffset, short yOffset, short zOffset, int flags )
		{
			m_ItemID = itemID;
			m_OffsetX = xOffset;
			m_OffsetY = yOffset;
			m_OffsetZ = zOffset;
			m_Flags = flags;
		}
	}

	public sealed class MultiComponentList
	{
		public static bool PostHSFormat {
			get { return _PostHSFormat; }
			set { _PostHSFormat = value; }
		}

		private static bool _PostHSFormat = false;

		private Point2D m_Min, m_Max, m_Center;
		private int m_Width, m_Height;
		private StaticTile[][][] m_Tiles;
		private MultiTileEntry[] m_List;

		public static readonly MultiComponentList Empty = new MultiComponentList();

		public Point2D Min{ get{ return m_Min; } }
		public Point2D Max{ get{ return m_Max; } }

		public Point2D Center{ get{ return m_Center; } }

		public int Width{ get{ return m_Width; } }
		public int Height{ get{ return m_Height; } }

		public StaticTile[][][] Tiles{ get{ return m_Tiles; } }
		public MultiTileEntry[] List{ get{ return m_List; } }

		public void Add( int itemID, int x, int y, int z )
		{
			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if ( vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height )
			{
				StaticTile[] oldTiles = m_Tiles[vx][vy];

				for ( int i = oldTiles.Length - 1; i >= 0; --i )
				{
					ItemData data = TileData.ItemTable[itemID & TileData.MaxItemValue];

					if ( oldTiles[i].Z == z && (oldTiles[i].Height > 0 == data.Height > 0 ) )
					{
						bool newIsRoof = ( data.Flags & TileFlag.Roof) != 0;
						bool oldIsRoof = (TileData.ItemTable[oldTiles[i].ID & TileData.MaxItemValue].Flags & TileFlag.Roof ) != 0;

						if ( newIsRoof == oldIsRoof )
							Remove( oldTiles[i].ID, x, y, z );
					}
				}

				oldTiles = m_Tiles[vx][vy];

				StaticTile[] newTiles = new StaticTile[oldTiles.Length + 1];

				for ( int i = 0; i < oldTiles.Length; ++i )
					newTiles[i] = oldTiles[i];

				newTiles[oldTiles.Length] = new StaticTile( (ushort)itemID, (sbyte)z );

				m_Tiles[vx][vy] = newTiles;

				MultiTileEntry[] oldList = m_List;
				MultiTileEntry[] newList = new MultiTileEntry[oldList.Length + 1];

				for ( int i = 0; i < oldList.Length; ++i )
					newList[i] = oldList[i];

				newList[oldList.Length] = new MultiTileEntry( (ushort)itemID, (short)x, (short)y, (short)z, 1 );

				m_List = newList;

				if ( x < m_Min.m_X )
					m_Min.m_X = x;

				if ( y < m_Min.m_Y )
					m_Min.m_Y = y;

				if ( x > m_Max.m_X )
					m_Max.m_X = x;

				if ( y > m_Max.m_Y )
					m_Max.m_Y = y;
			}
		}

		public void RemoveXYZH( int x, int y, int z, int minHeight )
		{
			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if ( vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height )
			{
				StaticTile[] oldTiles = m_Tiles[vx][vy];

				for ( int i = 0; i < oldTiles.Length; ++i )
				{
					StaticTile tile = oldTiles[i];

					if ( tile.Z == z && tile.Height >= minHeight )
					{
						StaticTile[] newTiles = new StaticTile[oldTiles.Length - 1];

						for ( int j = 0; j < i; ++j )
							newTiles[j] = oldTiles[j];

						for ( int j = i + 1; j < oldTiles.Length; ++j )
							newTiles[j - 1] = oldTiles[j];

						m_Tiles[vx][vy] = newTiles;

						break;
					}
				}

				MultiTileEntry[] oldList = m_List;

				for ( int i = 0; i < oldList.Length; ++i )
				{
					MultiTileEntry tile = oldList[i];

					if ( tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y && tile.m_OffsetZ == (short)z && TileData.ItemTable[tile.m_ItemID & TileData.MaxItemValue].Height >= minHeight )
					{
						MultiTileEntry[] newList = new MultiTileEntry[oldList.Length - 1];

						for ( int j = 0; j < i; ++j )
							newList[j] = oldList[j];

						for ( int j = i + 1; j < oldList.Length; ++j )
							newList[j - 1] = oldList[j];

						m_List = newList;

						break;
					}
				}
			}
		}

		public void Remove( int itemID, int x, int y, int z )
		{
			int vx = x + m_Center.m_X;
			int vy = y + m_Center.m_Y;

			if ( vx >= 0 && vx < m_Width && vy >= 0 && vy < m_Height )
			{
				StaticTile[] oldTiles = m_Tiles[vx][vy];

				for ( int i = 0; i < oldTiles.Length; ++i )
				{
					StaticTile tile = oldTiles[i];

					if ( tile.ID == itemID && tile.Z == z )
					{
						StaticTile[] newTiles = new StaticTile[oldTiles.Length - 1];

						for ( int j = 0; j < i; ++j )
							newTiles[j] = oldTiles[j];

						for ( int j = i + 1; j < oldTiles.Length; ++j )
							newTiles[j - 1] = oldTiles[j];

						m_Tiles[vx][vy] = newTiles;

						break;
					}
				}

				MultiTileEntry[] oldList = m_List;

				for ( int i = 0; i < oldList.Length; ++i )
				{
					MultiTileEntry tile = oldList[i];

					if ( tile.m_ItemID == itemID && tile.m_OffsetX == (short)x && tile.m_OffsetY == (short)y && tile.m_OffsetZ == (short)z )
					{
						MultiTileEntry[] newList = new MultiTileEntry[oldList.Length - 1];

						for ( int j = 0; j < i; ++j )
							newList[j] = oldList[j];

						for ( int j = i + 1; j < oldList.Length; ++j )
							newList[j - 1] = oldList[j];

						m_List = newList;

						break;
					}
				}
			}
		}

		public void Resize( int newWidth, int newHeight )
		{
			int oldWidth = m_Width, oldHeight = m_Height;
			StaticTile[][][] oldTiles = m_Tiles;

			int totalLength = 0;

			StaticTile[][][] newTiles = new StaticTile[newWidth][][];

			for ( int x = 0; x < newWidth; ++x )
			{
				newTiles[x] = new StaticTile[newHeight][];

				for ( int y = 0; y < newHeight; ++y )
				{
					if ( x < oldWidth && y < oldHeight )
						newTiles[x][y] = oldTiles[x][y];
					else
						newTiles[x][y] = new StaticTile[0];

					totalLength += newTiles[x][y].Length;
				}
			}

			m_Tiles = newTiles;
			m_List = new MultiTileEntry[totalLength];
			m_Width = newWidth;
			m_Height = newHeight;

			m_Min = Point2D.Zero;
			m_Max = Point2D.Zero;

			int index = 0;

			for ( int x = 0; x < newWidth; ++x )
			{
				for ( int y = 0; y < newHeight; ++y )
				{
					StaticTile[] tiles = newTiles[x][y];

					for ( int i = 0; i < tiles.Length; ++i )
					{
						StaticTile tile = tiles[i];

						int vx = x - m_Center.X;
						int vy = y - m_Center.Y;

						if ( vx < m_Min.m_X )
							m_Min.m_X = vx;

						if ( vy < m_Min.m_Y )
							m_Min.m_Y = vy;

						if ( vx > m_Max.m_X )
							m_Max.m_X = vx;

						if ( vy > m_Max.m_Y )
							m_Max.m_Y = vy;

						m_List[index++] = new MultiTileEntry( (ushort)tile.ID, (short)vx, (short)vy, (short)tile.Z, 1 );
					}
				}
			}
		}

		public MultiComponentList( MultiComponentList toCopy )
		{
			m_Min = toCopy.m_Min;
			m_Max = toCopy.m_Max;

			m_Center = toCopy.m_Center;

			m_Width = toCopy.m_Width;
			m_Height = toCopy.m_Height;

			m_Tiles = new StaticTile[m_Width][][];

			for ( int x = 0; x < m_Width; ++x )
			{
				m_Tiles[x] = new StaticTile[m_Height][];

				for ( int y = 0; y < m_Height; ++y )
				{
					m_Tiles[x][y] = new StaticTile[toCopy.m_Tiles[x][y].Length];

					for ( int i = 0; i < m_Tiles[x][y].Length; ++i )
						m_Tiles[x][y][i] = toCopy.m_Tiles[x][y][i];
				}
			}

			m_List = new MultiTileEntry[toCopy.m_List.Length];

			for ( int i = 0; i < m_List.Length; ++i )
				m_List[i] = toCopy.m_List[i];
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( (int) 1 ); // version;

			writer.Write( m_Min );
			writer.Write( m_Max );
			writer.Write( m_Center );

			writer.Write( (int) m_Width );
			writer.Write( (int) m_Height );

			writer.Write( (int) m_List.Length );

			for ( int i = 0; i < m_List.Length; ++i )
			{
				MultiTileEntry ent = m_List[i];

				writer.Write( (ushort) ent.m_ItemID );
				writer.Write( (short) ent.m_OffsetX );
				writer.Write( (short) ent.m_OffsetY );
				writer.Write( (short) ent.m_OffsetZ );
				writer.Write( (int) ent.m_Flags );
			}
		}

		public MultiComponentList( GenericReader reader )
		{
			int version = reader.ReadInt();

			m_Min = reader.ReadPoint2D();
			m_Max = reader.ReadPoint2D();
			m_Center = reader.ReadPoint2D();
			m_Width = reader.ReadInt();
			m_Height = reader.ReadInt();

			int length = reader.ReadInt();

			MultiTileEntry[] allTiles = m_List = new MultiTileEntry[length];

			if ( version == 0 ) {
				for ( int i = 0; i < length; ++i )
				{
					int id = reader.ReadShort();
					if ( id >= 0x4000 )
						id -= 0x4000;

					allTiles[i].m_ItemID = (ushort)id;
					allTiles[i].m_OffsetX = reader.ReadShort();
					allTiles[i].m_OffsetY = reader.ReadShort();
					allTiles[i].m_OffsetZ = reader.ReadShort();
					allTiles[i].m_Flags = reader.ReadInt();
				}
			} else {
				for ( int i = 0; i < length; ++i )
				{
					allTiles[i].m_ItemID = reader.ReadUShort();
					allTiles[i].m_OffsetX = reader.ReadShort();
					allTiles[i].m_OffsetY = reader.ReadShort();
					allTiles[i].m_OffsetZ = reader.ReadShort();
					allTiles[i].m_Flags = reader.ReadInt();
				}
			}

			TileList[][] tiles = new TileList[m_Width][];
			m_Tiles = new StaticTile[m_Width][][];

			for ( int x = 0; x < m_Width; ++x )
			{
				tiles[x] = new TileList[m_Height];
				m_Tiles[x] = new StaticTile[m_Height][];

				for ( int y = 0; y < m_Height; ++y )
					tiles[x][y] = new TileList();
			}

			for ( int i = 0; i < allTiles.Length; ++i )
			{
				if ( i == 0 || allTiles[i].m_Flags != 0 )
				{
					int xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					int yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;

					tiles[xOffset][yOffset].Add( (ushort)allTiles[i].m_ItemID, (sbyte)allTiles[i].m_OffsetZ );
				}
			}

			for ( int x = 0; x < m_Width; ++x )
				for ( int y = 0; y < m_Height; ++y )
					m_Tiles[x][y] = tiles[x][y].ToArray();
		}
        // ALAN TEMP MOD
        public static Dictionary<int, MultiEntry> MultiEntries = new Dictionary<int, MultiEntry>();
        public class MultiEntry
        {
            public static int[] CannonSpotBaseItemIDs = new int[] { 0x84fD, 0x8489, 0x84AA, 0x8516, 0x84FF, 0x848E, 0x84AC }; // north-facing undamaged boat cannon spot ids
            public static int[] RopeSpotBaseItemIDs = new int[] { 0x14FA };
            public static int[] HoldSpotBaseItemIDs = new int[] { }; // north-facing undamaged boat hold spot ids
            
            public int BaseMultiID = 0;
            public List<MultiEntryComponent> Components = new List<MultiEntryComponent>();

            public MultiEntry(int baseMultiID)
            {
                BaseMultiID = baseMultiID;
            }

            public void AddMultiEntryComponent(int damageLevel, int direction, int itemID, Point3D offset, int[,] transformation)
            {
                
                bool matchFound = false;
                foreach (MultiEntryComponent comp in Components)
                {
                    Point3D transformedOffset = new Point3D(transformation[0, 0] * comp.Offset.X + transformation[0, 1] * comp.Offset.Y, transformation[1, 0] * comp.Offset.X + transformation[1, 1] * comp.Offset.Y, comp.Offset.Z);
                    if (offset == transformedOffset)
                    {
                        if (matchFound)
                        {
                            Console.WriteLine("MultiBoatStuff error: " + transformedOffset + " has 2 components in it...!  New itemid=" + itemID.ToString("X4") + "   compOffset=" + comp.Offset + "   offset=" + transformedOffset);
                        }
                        else
                        {
                            comp.DirectionalItemIDs[damageLevel, direction] = itemID;
                            matchFound = true;
                        }
                    }
                }
                if (!matchFound)
                {
                    Console.WriteLine("BaseID: " + BaseMultiID + " adding base component, itemID=" + itemID.ToString("X4") + "  direction=" + direction + "   damageLevel=" + damageLevel + "    offset=" + offset);
                    if (damageLevel != 0 || direction != 0) { Console.WriteLine("MultiBoatStuff error: expected northern undamaged multi components first!"); }
                    Components.Add(new MultiEntryComponent(offset, itemID));
                }
            }
        }
        public class MultiEntryComponent
        {
            public Point3D Offset;
            public int[,] DirectionalItemIDs = new int[3,4]; // this is a 3 x 4 matrix, each row being a damage level, and each column being a direction, 0 = North, 1 = East, 2 = South, 3 = West

            public MultiEntryComponent(Point3D offset, int itemID)
            {
                // expecting it to be north for the first one
                DirectionalItemIDs[0, 0] = itemID;
                Offset = offset;
            }

            public override string ToString()
            {
                string output = "";
                for (int i = 0; i < DirectionalItemIDs.GetLength(0); i++)
                {
                    for (int j = 0; j < DirectionalItemIDs.GetLength(1); j++)
                    {
                        output += DirectionalItemIDs[i, j] + ",";
                    }
                }
                return Offset.ToString() + " " + output;
            }
        }
        // END ALAN MOD

		public MultiComponentList( BinaryReader reader, int count )
		{
            MultiTileEntry[] allTiles = m_List = new MultiTileEntry[count];

			for ( int i = 0; i < count; ++i )
			{
				allTiles[i].m_ItemID = reader.ReadUInt16();
				allTiles[i].m_OffsetX = reader.ReadInt16();
				allTiles[i].m_OffsetY = reader.ReadInt16();
				allTiles[i].m_OffsetZ = reader.ReadInt16();
				allTiles[i].m_Flags = reader.ReadInt32();

                /*
                // BEGIN ALAN PARSING OUT STUFF (THIS IS TEMPORARY) ==================================================
                if (allTiles[i].m_Flags == 0) // invisible
                {
                    int[,] transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; // transformation matrix

                    //Console.WriteLine(allTiles[i].m_ItemID + "\t" + allTiles[i].m_OffsetX + "\t" + allTiles[i].m_OffsetY + "\t" + allTiles[i].m_OffsetZ + "\t" + allTiles[i].m_Flags);
                    int baseMultiID = 0; // this is the "base" multi (northern undamaged boat) used to rotate from in order to connect the pieces
                    int damageLevel = 0;
                    int direction = 0;
                    if (MultiData.currentMultiID == 24) // orc north BASE
                    {
                        if (!MultiEntries.ContainsKey(24)) MultiEntries.Add(24, new MultiEntry(24));
                        baseMultiID = 24; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 0; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 25) // orc east
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 0; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 26) // orc south
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 0; direction =2 ;
                    }
                    else if (MultiData.currentMultiID == 27) // orc west
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 0; direction = 3;
                    }
                    else if (MultiData.currentMultiID == 28) // orc north damaged 1
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 1; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 29) // orc east damaged 1
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 1; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 30) // orc south damaged 1
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 1; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 31) // orc west damaged 1
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 1; direction = 3;
                    }
                    else if (MultiData.currentMultiID == 32) // orc north damaged 2
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 2; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 33) // orc east damaged 2
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 2; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 34) // orc south damaged 2
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 2; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 35) // orc west damaged 2
                    {
                        baseMultiID = 24; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 2; direction = 3;
                    }






                    else if (MultiData.currentMultiID == 36) // gargoyle north BASE
                    {
                        if (!MultiEntries.ContainsKey(36)) MultiEntries.Add(36, new MultiEntry(36));
                        baseMultiID = 36; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 0; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 37) // gargoyle east
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 0; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 38) // gargoyle south
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 0; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 39) // gargoyle west
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 0; direction = 3;
                    }
                    else if (MultiData.currentMultiID == 40) // gargoyle north damaged 1
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 1; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 41) // gargoyle east damaged 1
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 1; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 42) // gargoyle south damaged 1
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 1; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 43) // gargoyle west damaged 1
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 1; direction = 3;
                    }
                    else if (MultiData.currentMultiID == 44) // gargoyle north damaged 2
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 2; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 45) // gargoyle east damaged 2
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 2; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 46) // gargoyle south damaged 2
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 2; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 47) // gargoyle west damaged 2
                    {
                        baseMultiID = 36; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 2; direction = 3;
                    }




                    else if (MultiData.currentMultiID == 48) // tokuno north BASE
                    {
                        if (!MultiEntries.ContainsKey(48)) MultiEntries.Add(48, new MultiEntry(48));
                        baseMultiID = 48; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 0; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 49) // tokuno east
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 0; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 50) // tokuno south
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 0; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 51) // tokuno west
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 0; direction = 3;
                    }
                    else if (MultiData.currentMultiID == 52) // tokuno north damaged 1
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 1; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 53) // tokuno east damaged 1
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 1; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 54) // tokuno south damaged 1
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 1; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 55) // tokuno west damaged 1
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 1; direction = 3;
                    }
                    else if (MultiData.currentMultiID == 56) // tokuno north damaged 2
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 1, 0 }, { 0, 1 } }; damageLevel = 2; direction = 0;
                    }
                    else if (MultiData.currentMultiID == 57) // tokuno east damaged 2
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 0, -1 }, { 1, 0 } }; damageLevel = 2; direction = 1;
                    }
                    else if (MultiData.currentMultiID == 58) // tokuno south damaged 2
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { -1, 0 }, { 0, -1 } }; damageLevel = 2; direction = 2;
                    }
                    else if (MultiData.currentMultiID == 59) // tokuno west damaged 2
                    {
                        baseMultiID = 48; transformation = new int[2, 2] { { 0, 1 }, { -1, 0 } }; damageLevel = 2; direction = 3;
                    }



                    if (MultiEntries.ContainsKey(baseMultiID))
                    {
                        MultiEntry entry = MultiEntries[baseMultiID];
                        entry.AddMultiEntryComponent(damageLevel, direction, allTiles[i].m_ItemID, new Point3D(allTiles[i].m_OffsetX, allTiles[i].m_OffsetY, allTiles[i].m_OffsetZ), transformation);
                    }
                    else
                    {
                        Console.WriteLine("MultiEntries did not yet contain key " + baseMultiID);
                    }
                }

                // END ALAN PARSING OUT STUFF ===================================================================
                */
				if ( _PostHSFormat )
					reader.ReadInt32(); // ??

				MultiTileEntry e = allTiles[i];

				if ( i == 0 || e.m_Flags != 0 )
				{
					if ( e.m_OffsetX < m_Min.m_X )
						m_Min.m_X = e.m_OffsetX;

					if ( e.m_OffsetY < m_Min.m_Y )
						m_Min.m_Y = e.m_OffsetY;

					if ( e.m_OffsetX > m_Max.m_X )
						m_Max.m_X = e.m_OffsetX;

					if ( e.m_OffsetY > m_Max.m_Y )
						m_Max.m_Y = e.m_OffsetY;
				}
			}

			m_Center = new Point2D( -m_Min.m_X, -m_Min.m_Y );
			m_Width = (m_Max.m_X - m_Min.m_X) + 1;
			m_Height = (m_Max.m_Y - m_Min.m_Y) + 1;

			TileList[][] tiles = new TileList[m_Width][];
			m_Tiles = new StaticTile[m_Width][][];

			for ( int x = 0; x < m_Width; ++x )
			{
				tiles[x] = new TileList[m_Height];
				m_Tiles[x] = new StaticTile[m_Height][];

				for ( int y = 0; y < m_Height; ++y )
					tiles[x][y] = new TileList();
			}

			for ( int i = 0; i < allTiles.Length; ++i )
			{
				if ( i == 0 || allTiles[i].m_Flags != 0 )
				{
					int xOffset = allTiles[i].m_OffsetX + m_Center.m_X;
					int yOffset = allTiles[i].m_OffsetY + m_Center.m_Y;

					tiles[xOffset][yOffset].Add( (ushort)allTiles[i].m_ItemID, (sbyte)allTiles[i].m_OffsetZ );
				}
			}

			for ( int x = 0; x < m_Width; ++x )
				for ( int y = 0; y < m_Height; ++y )
					m_Tiles[x][y] = tiles[x][y].ToArray();
		}

		private MultiComponentList()
		{
			m_Tiles = new StaticTile[0][][];
			m_List = new MultiTileEntry[0];
		}
	}
}