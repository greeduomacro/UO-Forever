#region References
using System;
using System.Collections.Generic;

using VitaNex;
using VitaNex.IO;
#endregion

namespace Server
{
	[CoreService("Player Scores", "1.0.0", TaskPriority.High)]
	public static partial class PlayerScores
	{
		static PlayerScores()
		{
			CSOptions = new PlayerScoresOptions();

			Registry = new BinaryDataStore<IEntity, Dictionary<Mobile, double>>(
				VitaNexCore.SavesDirectory + "/PlayerScores", "Scores")
			{
				Async = true,
				OnSerialize = SerializeScores,
				OnDeserialize = DeserializePlayerScores
			};
		}

		private static void CSSave()
		{
			//DefragmentPlayerScores();
			VitaNexCore.TryCatchGet(Registry.Export, x => x.ToConsole());
		}

		private static void CSLoad()
		{
			if (VitaNexCore.TryCatchGet(Registry.Import, x => x.ToConsole()) == DataStoreResult.OK)
			{
				//DefragmentPlayerScores();
			}
		}

		private static bool SerializeScores(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteBlockDictionary(
				Registry,
				(e, list) =>
				{
					writer.WriteEntity(e);

					writer.WriteBlockDictionary(
						list,
						(k, v) =>
						{
							writer.Write(k);
							writer.Write(v);
						});
				});

			return true;
		}

		private static bool DeserializePlayerScores(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadBlockDictionary(
				() =>
				{
					IEntity e = reader.ReadEntity();

					Dictionary<Mobile, double> list = reader.ReadBlockDictionary(
						() =>
						{
							Mobile k = reader.ReadMobile();
							double v = reader.ReadDouble();

							return new KeyValuePair<Mobile, double>(k, v);
						});

					return new KeyValuePair<IEntity, Dictionary<Mobile, double>>(e, list);
				},
				Registry);

			return true;
		}
	}
}