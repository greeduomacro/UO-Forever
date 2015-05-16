#region Header
//   Vorspire    _,-'/-'/  Notify_Init.cs
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
using System.Reflection;

using Server;

using VitaNex.IO;
#endregion

namespace VitaNex.Notify
{
	[CoreService("Notify", "1.0.0", TaskPriority.Highest)]
	public static partial class Notify
	{
		static Notify()
		{
			CSOptions = new CoreServiceOptions(typeof(Notify));

			GumpTypes = typeof(NotifyGump).GetConstructableChildren(t => !t.IsEqualOrChildOf<WorldNotifyGump>());
			WorldGumpSubTypes = typeof(WorldNotifyGump).GetConstructableChildren(t => t.IsNested);

			Settings = new BinaryDataStore<Type, NotifySettings>(VitaNexCore.SavesDirectory + "/Notify", "Settings") {
				OnSerialize = Serialize,
				OnDeserialize = Deserialize
			};
		}

		private static void CSConfig()
		{
			/*foreach (Type t in GumpTypes)
			{
				var init = t.GetMethod("InitSettings", BindingFlags.Static | BindingFlags.NonPublic);

				if (init == null)
				{
					continue;
				}

				var settings = EnsureSettings(t);

				if (settings != null)
				{
					init.Invoke(null, new object[] {settings});
				}
			}*/
		}

		private static void CSInvoke()
		{
			CommandUtility.Register("Notify", AccessLevel.Seer, HandleNotify);
			CommandUtility.Register("NotifyAC", AccessLevel.Seer, HandleNotifyAC);
			CommandUtility.Register("NotifyNA", AccessLevel.Seer, HandleNotifyNA);
			CommandUtility.Register("NotifyACNA", AccessLevel.Seer, HandleNotifyACNA);
		}

		private static void CSLoad()
		{
			Settings.Import();
		}

		private static void CSSave()
		{
			Settings.Export();
		}

		private static bool Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteBlockDictionary(
				Settings,
				(k, v) =>
				{
					writer.WriteType(k);
					v.Serialize(writer);
				});

			return true;
		}

		private static bool Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			reader.ReadBlockDictionary(
				() =>
				{
					var k = reader.ReadType();
					var v = EnsureSettings(k);

					if (v != null)
					{
						v.Deserialize(reader);
					}

					return new KeyValuePair<Type, NotifySettings>(k, v);
				},
				Settings);

			return true;
		}
	}
}