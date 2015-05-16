#region Header
//   Vorspire    _,-'/-'/  TrashCollection_Init.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Server;

using VitaNex.IO;
using VitaNex.Network;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	[CoreModule("Trash Collection", "1.1.0", false, TaskPriority.Low)]
	public static partial class TrashCollection
	{
		static TrashCollection()
		{
			HandlerTypes = typeof(BaseTrashHandler).GetConstructableChildren();

			CMOptions = new TrashCollectionOptions();

			Handlers = new BinaryDataStore<string, BaseTrashHandler>(VitaNexCore.SavesDirectory + "/TrashCollection", "Handlers")
			{
				OnSerialize = SerializeHandlers,
				OnDeserialize = DeserializeHandlers
			};

			Profiles = new BinaryDataStore<Mobile, TrashProfile>(VitaNexCore.SavesDirectory + "/TrashCollection", "Profiles")
			{
				Async = true,
				OnSerialize = SerializeProfiles,
				OnDeserialize = DeserializeProfiles
			};
		}

		private static void CMConfig()
		{
			var handlers = new List<BaseTrashHandler>();

			int count = 0;

			HandlerTypes.ForEach(
				type =>
				{
					BaseTrashHandler handler = type.CreateInstance<BaseTrashHandler>();

					if (handler == null)
					{
						return;
					}

					handlers.Add(handler);

					if (CMOptions.ModuleDebug)
					{
						CMOptions.ToConsole(
							"Created trash handler '{0}' ({1})", handler.GetType().Name, handler.Enabled ? "Enabled" : "Disabled");
					}

					++count;
				});

			CMOptions.ToConsole("Created {0:#,0} trash handler{1}", count, count != 1 ? "s" : String.Empty);

			handlers.ForEach(
				h =>
				{
					if (!Handlers.ContainsKey(h.UID))
					{
						Handlers.Add(h.UID, h);
					}
					else
					{
						Handlers[h.UID] = h;
					}
				});

			ExtendedOPL.OnItemOPLRequest += AddTrashProperties;
			ExtendedOPL.OnMobileOPLRequest += AddTrashProperties;
		}

		private static void CMEnabled()
		{
			ExtendedOPL.OnItemOPLRequest += AddTrashProperties;
			ExtendedOPL.OnMobileOPLRequest += AddTrashProperties;
		}

		private static void CMDisabled()
		{
			ExtendedOPL.OnItemOPLRequest -= AddTrashProperties;
			ExtendedOPL.OnMobileOPLRequest -= AddTrashProperties;
		}

		private static void CMInvoke()
		{
			InternalHandlerSort();
		}

		private static void CMSave()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					DataStoreResult result = Handlers.Export();
					CMOptions.ToConsole("{0:#,0} handlers saved, {1}", Handlers.Count, result);
				},
				CMOptions.ToConsole);

			VitaNexCore.TryCatch(
				() =>
				{
					DataStoreResult result = Profiles.Export();
					CMOptions.ToConsole("{0:#,0} profiles saved, {1}", Profiles.Count, result);
				},
				CMOptions.ToConsole);
		}

		private static void CMLoad()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					DataStoreResult result = Handlers.Import();
					CMOptions.ToConsole("{0:#,0} handlers loaded, {1}.", Handlers.Count, result);
				},
				CMOptions.ToConsole);

			VitaNexCore.TryCatch(
				() =>
				{
					DataStoreResult result = Profiles.Import();
					CMOptions.ToConsole("{0:#,0} profiles loaded, {1}.", Profiles.Count, result);
				},
				CMOptions.ToConsole);
		}

		private static bool SerializeHandlers(GenericWriter writer)
		{
			writer.WriteBlockList(
				new List<BaseTrashHandler>(Handlers.Values),
				obj => writer.WriteType(
					obj,
					t =>
					{
						if (t != null)
						{
							obj.Serialize(writer);
						}
					}));

			return true;
		}

		private static bool DeserializeHandlers(GenericReader reader)
		{
			var list = reader.ReadBlockArray(() => reader.ReadTypeCreate<BaseTrashHandler>(reader));

			list.ForEach(
				h =>
				{
					if (!Handlers.ContainsKey(h.UID))
					{
						Handlers.Add(h.UID, h);
					}
					else
					{
						Handlers[h.UID] = h;
					}
				});

			InternalHandlerSort();
			return true;
		}

		private static bool SerializeProfiles(GenericWriter writer)
		{
			writer.WriteBlockArray(
				Profiles.Values.ToArray(),
				obj =>
				{
					if (obj != null)
					{
						obj.Serialize(writer);
					}
				});

			return true;
		}

		private static bool DeserializeProfiles(GenericReader reader)
		{
			var list = reader.ReadBlockArray(() => new TrashProfile(reader));
			
			foreach (var p in list)
			{
				Profiles.AddOrReplace(p.Owner, p);
			}

			return true;
		}
	}
}