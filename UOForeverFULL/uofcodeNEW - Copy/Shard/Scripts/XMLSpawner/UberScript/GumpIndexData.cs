#region References
using System;
using System.Collections;
using System.IO;

using Server.Commands;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class GumpData
	{
		private readonly int m_Width;
		public int Width { get { return m_Width; } }
		private readonly int m_Height;
		public int Height { get { return m_Height; } }

		public GumpData(int width, int height)
		{
			m_Width = width;
			m_Height = height;
		}
	}

	public class GumpIndexData
	{
		public static ArrayList ParsedGumpData = new ArrayList(61243); // the number of gumps

		public static GumpData GetGumpData(int gumpID)
		{
			if (gumpID < ParsedGumpData.Count)
			{
				return (GumpData)ParsedGumpData[gumpID];
			}
			return null;
		}

		public static void Initialize()
		{
			try
			{
				string filePath = Core.FindDataFile("gumpidx.mul");

				if (File.Exists(filePath))
				{
					using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						BinaryReader bin = new BinaryReader(fs);
						// http://wpdev.sourceforge.net/docs/formats/csharp/index.html
						//To find data for a certain entry, you seek to (EntryID * 12), and read the data.
						//Then, in the actual MUL file (not the index file), you seek to (lookup), and read (length) number of bytes.
						//Some data relies on the extra field.
						long test = fs.Length - 1;
						long position = 0;
						while (position < test)
						{
							int lookup = bin.ReadInt32();
							int length = bin.ReadInt32();
							int extra = bin.ReadInt32();
							position += 12;
							short width = (short)((extra >> 16) & 0xFFFF);
							short height = (short)(extra & 0xFFFF);
							ParsedGumpData.Add(new GumpData(width, height));
							//Console.WriteLine(width + " " + height);
							//LoggingCustom.Log("ERROR_GumpIndexParser.txt", width + " " + height);
						}

						fs.Close();
					}
				}
			}
			catch (Exception e)
			{
				LoggingCustom.Log("ERROR_GumpIndexParser.txt", DateTime.Now + "\t" + e.Message + "\n" + e.StackTrace);
			}
		}
	}
}