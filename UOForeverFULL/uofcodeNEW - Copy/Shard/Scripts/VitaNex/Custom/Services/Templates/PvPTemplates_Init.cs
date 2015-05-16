#region References
using System.Collections.Generic;

using Server.Mobiles;

using VitaNex;
using VitaNex.IO;
#endregion

namespace Server.PvPTemplates
{
	[CoreService("PvP Templates", "1.1.0", TaskPriority.High)]
	public static partial class PvPTemplates
	{
		static PvPTemplates()
		{
			CSOptions = new PvPTemplatesOptions();

			Templates = new BinaryDataStore<PlayerMobile, TemplateProfile>(
				VitaNexCore.SavesDirectory + "/Templates", "PvPTemplates")
			{
				Async = true,
				OnSerialize = SerializeTemplates,
				OnDeserialize = DeserializeTemplates
			};
		}

		private static void CSConfig()
		{
			EventSink.Login += OnLogin;
		}

		private static void CSInvoke()
		{
			CommandUtility.Register(
				"Templates",
				AccessLevel.Player,
				e =>
				{
					if (e.Mobile is PlayerMobile)
					{
						DisplayManagerGump((PlayerMobile)e.Mobile);
					}
				});
		}

		private static void CSSave()
		{
			Save();
		}

		private static void CSLoad()
		{
			Load();
		}
		
		public static void Save()
		{
			VitaNexCore.TryCatch(SaveProfiles, CSOptions.ToConsole);
		}

		public static void SaveProfiles()
		{
			DataStoreResult result = Templates.Export();
			CSOptions.ToConsole("Result: {0}", result.ToString());

			switch (result)
			{
				case DataStoreResult.Null:
				case DataStoreResult.Busy:
				case DataStoreResult.Error:
					{
						if (Templates.HasErrors)
						{
							CSOptions.ToConsole("Profiles database has errors...");

							Templates.Errors.ForEach(CSOptions.ToConsole);
						}
					}
					break;
				case DataStoreResult.OK:
					CSOptions.ToConsole("Profile count: {0:#,0}", Templates.Count);
					break;
			}
		}

		public static void Load()
		{
			VitaNexCore.TryCatch(LoadProfiles, CSOptions.ToConsole);
		}

		public static void LoadProfiles()
		{
			DataStoreResult result = Templates.Import();
			CSOptions.ToConsole("Result: {0}", result.ToString());

			switch (result)
			{
				case DataStoreResult.Null:
				case DataStoreResult.Busy:
				case DataStoreResult.Error:
					{
						if (Templates.HasErrors)
						{
							CSOptions.ToConsole("Profiles database has errors...");

							Templates.Errors.ForEach(CSOptions.ToConsole);
						}
					}
					break;
				case DataStoreResult.OK:
					CSOptions.ToConsole("Profile count: {0:#,0}", Templates.Count);
					break;
			}
		}

		private static bool SerializeTemplates(GenericWriter writer)
		{
			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteBlockDictionary(
							Templates,
							(key, val) =>
							{
								writer.Write(key);
								val.Serialize(writer);
							});
					}
					break;
			}

			return true;
		}

		private static bool DeserializeTemplates(GenericReader reader)
		{
			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						reader.ReadBlockDictionary(
							() =>
							{
								var key = reader.ReadMobile<PlayerMobile>();
								var val = new TemplateProfile(reader);
								return new KeyValuePair<PlayerMobile, TemplateProfile>(key, val);
							},
							Templates);
					}
					break;
			}

			return true;
		}
	}
}