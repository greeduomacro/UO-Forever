#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Gumps;
using Server.Items;
using Server.Mobiles;

using VitaNex;
using VitaNex.IO;
using VitaNex.Modules.AutoPvP;
using VitaNex.Notify;
#endregion

namespace Server.Twitch
{
	public static partial class PokerPersistance
	{
        static PokerPersistance()
		{
			_PersistenceFile = IOUtility.EnsureFile(VitaNexCore.SavesDirectory + "/ActionCams/Cameras.bin");

		}

		public static void Configure()
		{
			EventSink.WorldLoad += Load;
			EventSink.WorldSave += e => Save();
		}

		public static void Save()
		{
			_PersistenceFile.Serialize(SerializeCams);
		}

		public static void Load()
		{
			_PersistenceFile.Deserialize(DeserializeCams);
		}

		private static void SerializeCams(GenericWriter writer)
		{
			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
					}
					break;
			}
		}

		private static void DeserializeCams(GenericReader reader)
		{
			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
					}
					break;
			}
		}
	}
}