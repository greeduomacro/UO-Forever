#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Server.Commands;
using Server.Network;
#endregion

namespace Server.Misc
{
	public class IPLimiter
	{
		public const string ExemptionFileName = "IPExemptions.txt";

		public static Dictionary<IPAddress, string> Exemptions { get; private set; }

		public static bool Enabled { get; set; }
		public static bool SocketBlock { get; set; }
		public static int MaxAddresses { get; set; }

		static IPLimiter()
		{
			//For hosting services where there are cases where IPs can be proxied
			Exemptions = new Dictionary<IPAddress, string>();

			Enabled = true;

			// True to block at connection, false to block at login request
			SocketBlock = true;

			MaxAddresses = 4;
		}

		public static void Configure()
		{
			CommandSystem.Register("UniqueIP", AccessLevel.Administrator, OnUniqueIPCommand);
			CommandSystem.Register("WhiteList", AccessLevel.Administrator, OnWhiteListCommand);
		}

		public static void Initialize()
		{
			DeserializeExemptions();
		}

		private static void OnUniqueIPCommand(CommandEventArgs e)
		{
			if (e.Mobile != null)
			{
				e.Mobile.SendMessage("There are {0:#,0} unique IP's currently connected.", GetUniqueStates().Count);
			}
		}

		private static void OnWhiteListCommand(CommandEventArgs e)
		{
			if (e.Mobile == null)
			{
				return;
			}

			if (e.Arguments == null || e.Arguments.Length < 1)
			{
				e.Mobile.SendMessage("Usage: {0}WhiteList <add | remove | save | reload | clear>", CommandSystem.Prefix);
				return;
			}

			IPAddress ip;

			switch (e.Arguments[0].ToLower())
			{
				case "add":
					{
						if (e.Arguments.Length < 2 || !IPAddress.TryParse(e.Arguments[1], out ip))
						{
							e.Mobile.SendMessage("Usage: {0}WhiteList <add> <ip> [note]", CommandSystem.Prefix);
							return;
						}

						string note = e.Arguments.Length < 3 ? String.Empty : e.Arguments[2];

						if (Exemptions.ContainsKey(ip))
						{
							if (!String.IsNullOrEmpty(note))
							{
								Exemptions[ip] = note;
							}

							e.Mobile.SendMessage("IP '{0}' is already in the exemption list.", ip);
							return;
						}

						Exemptions.Add(ip, note);
						e.Mobile.SendMessage("IP '{0}' has been added to the exemption list.", ip);
						return;
					}
				case "remove":
					{
						if (e.Arguments.Length < 2 || !IPAddress.TryParse(e.Arguments[1], out ip))
						{
							e.Mobile.SendMessage("Usage: {0}WhiteList <remove> <ip>", CommandSystem.Prefix);
							return;
						}

						if (!Exemptions.Remove(ip))
						{
							e.Mobile.SendMessage("IP '{0}' does not exist in the exemption list.", ip);
							return;
						}

						e.Mobile.SendMessage("IP '{0}' has been removed from the exemption list.", ip);
						return;
					}
				case "save":
					{
						SerializeExemptions();
						e.Mobile.SendMessage("IP exemption list has been saved.");
						return;
					}
				case "reload":
					{
						DeserializeExemptions();
						e.Mobile.SendMessage("IP exemption list has been reloaded.");
						return;
					}
				case "clear":
					{
						Exemptions.Clear();
						e.Mobile.SendMessage("IP exemption list has been cleared.");
						return;
					}
			}
		}

		public static Dictionary<IPAddress, List<NetState>> GetUniqueStates()
		{
			var list = new Dictionary<IPAddress, List<NetState>>();

			foreach (var ns in NetState.Instances.AsParallel().Where(ns => ns != null && ns.Mobile != null && ns.Socket != null))
			{
				if (!list.ContainsKey(ns.Address))
				{
					list.Add(ns.Address, new List<NetState>());
				}

				list[ns.Address].Add(ns);
			}

			return list;
		}

		public static void SerializeExemptions()
		{
			if (File.Exists(ExemptionFileName))
			{
				File.Delete(ExemptionFileName);
			}

			File.WriteAllLines(
				ExemptionFileName, Exemptions.Select(kv => String.Format("{0}\t{1}", kv.Key, kv.Value)), Encoding.UTF8);
		}

		public static void DeserializeExemptions()
		{
			if (!File.Exists(ExemptionFileName))
			{
				return;
			}

			Exemptions.Clear();

			var lines = File.ReadAllLines(ExemptionFileName, Encoding.UTF8);
			var schar = new[] {' ', '\t'};

			foreach (var line in
				lines.Where(line => !String.IsNullOrWhiteSpace(line) && !line.StartsWith("#") && !line.StartsWith("//"))
					 .Select(line => line.Split(schar, StringSplitOptions.RemoveEmptyEntries)))
			{
				string addr = line.First();
				string note = String.Join(" ", line.Skip(1));

				IPAddress ip;

				if (!IPAddress.TryParse(addr, out ip))
				{
					continue;
				}

				if (!Exemptions.ContainsKey(ip))
				{
					Exemptions.Add(ip, note);
				}
				else
				{
					Exemptions[ip] = note;
				}
			}
		}

		public static bool IsExempt(IPAddress ip)
		{
			return !Enabled || Exemptions.ContainsKey(ip);
		}

		public static bool Verify(IPAddress ip)
		{
			return !Enabled || IsExempt(ip) || NetState.Instances.Count(ns => ns.Address == ip) < MaxAddresses;
		}
	}
}