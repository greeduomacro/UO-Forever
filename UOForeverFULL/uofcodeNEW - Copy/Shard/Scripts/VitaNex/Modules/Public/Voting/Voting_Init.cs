#region Header
//   Vorspire    _,-'/-'/  Voting_Init.cs
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

using Server;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.Voting
{
	[CoreModule("Voting", "1.1.0", false)]
	public static partial class Voting
	{
		static Voting()
		{
			SiteTypes = typeof(IVoteSite).GetConstructableChildren();

			CMOptions = new VotingOptions();

			VoteSites = new BinaryDataStore<int, IVoteSite>(VitaNexCore.SavesDirectory + "/Voting", "Sites") {
				OnSerialize = SerializeVoteSites,
				OnDeserialize = DeserializeVoteSites
			};

			Profiles = new BinaryDataStore<PlayerMobile, VoteProfile>(VitaNexCore.SavesDirectory + "/Voting", "Profiles")
			{
				Async = true,
				OnSerialize = SerializeProfiles,
				OnDeserialize = DeserializeProfiles
			};
		}

		private static void CMConfig()
		{ }

		private static void CMEnabled()
		{ }

		private static void CMDisabled()
		{ }

		private static void CMInvoke()
		{
			if (VoteSites.Count != 0)
			{
				return;
			}

			var sites = new List<IVoteSite>();

			SiteTypes.ForEach(
				type => VitaNexCore.TryCatch(
					() =>
					{
						var site = type.CreateInstance<IVoteSite>();

						if (site == null)
						{
							return;
						}

						if (site.Name == "Vita-Nex")
						{
							site.Enabled = true;
						}

						sites.Add(site);
						CMOptions.ToConsole(
							"Created site ({0}) '{1}', '{2}'", site.GetType().Name, site.Name, site.Enabled ? "Enabled" : "Disabled");
					},
					CMOptions.ToConsole));

			sites.ForEach(
				s =>
				{
					if (!VoteSites.ContainsKey(s.UID))
					{
						VoteSites.Add(s.UID, s);
					}
					else
					{
						VoteSites[s.UID] = s;
					}
				});

			InternalSiteSort();
		}

		private static void CMSave()
		{
			DataStoreResult result = VitaNexCore.TryCatchGet(VoteSites.Export, CMOptions.ToConsole);
			CMOptions.ToConsole("{0} sites saved, {1}", VoteSites.Count.ToString("#,0"), result);

			result = VitaNexCore.TryCatchGet(Profiles.Export, CMOptions.ToConsole);
			CMOptions.ToConsole("{0} profiles saved, {1}", Profiles.Count.ToString("#,0"), result);
		}

		private static void CMLoad()
		{
			DataStoreResult result = VitaNexCore.TryCatchGet(VoteSites.Import, CMOptions.ToConsole);
			CMOptions.ToConsole("{0} sites loaded, {1}.", VoteSites.Count.ToString("#,0"), result);

			result = VitaNexCore.TryCatchGet(Profiles.Import, CMOptions.ToConsole);
			CMOptions.ToConsole("{0} profiles loaded, {1}.", Profiles.Count.ToString("#,0"), result);
		}

		private static bool SerializeVoteSites(GenericWriter writer)
		{
			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteBlockDictionary(
							VoteSites,
							(k, v) => writer.WriteType(
								v,
								t =>
								{
									if (t != null)
									{
										v.Serialize(writer);
									}
								}));
					}
					break;
			}

			return true;
		}

		private static bool DeserializeVoteSites(GenericReader reader)
		{
			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						reader.ReadBlockDictionary(
							() =>
							{
								var v = reader.ReadTypeCreate<IVoteSite>(reader);

								return new KeyValuePair<int, IVoteSite>(v.UID, v);
							},
							VoteSites);
					}
					break;
			}

			return true;
		}

		private static bool SerializeProfiles(GenericWriter writer)
		{
			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.WriteBlockDictionary(
							Profiles,
							(k, v) =>
							{
								writer.Write(k);
								v.Serialize(writer);
							});
					}
					break;
			}

			return true;
		}

		private static bool DeserializeProfiles(GenericReader reader)
		{
			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						reader.ReadBlockDictionary(
							() =>
							{
								var k = reader.ReadMobile<PlayerMobile>();
								var v = new VoteProfile(reader);

								return new KeyValuePair<PlayerMobile, VoteProfile>(k, v);
							},
							Profiles);
					}
					break;
			}

			return true;
		}
	}
}