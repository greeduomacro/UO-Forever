#region References
using System;
using System.IO;
using System.Text;

using Server.Accounting;
using Server.Multis;
using Server.Network;

using VitaNex;
using VitaNex.IO;
#endregion

namespace Server.Commands
{
	public class CommandLogging
	{
		private static StreamWriter m_Output;
		private static bool m_Enabled = true;

		public static bool Enabled { get { return m_Enabled; } set { m_Enabled = value; } }

		public static StreamWriter Output { get { return m_Output; } }

		public static void Initialize()
		{
			CommandSystem.Register("LogBadPackets", AccessLevel.GameMaster, LogBadPackets_Command);

			EventSink.Command += EventSink_Command;

			if (!Directory.Exists("Logs"))
			{
				Directory.CreateDirectory("Logs");
			}

			const string directory = "Logs/Commands";

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			try
			{
				m_Output = new StreamWriter(
					Path.Combine(directory, String.Format("{0}.log", DateTime.UtcNow.ToLongDateString())), true)
				{
					AutoFlush = true
				};

				m_Output.WriteLine("##############################");
				m_Output.WriteLine("Log started on {0}", DateTime.UtcNow);
				m_Output.WriteLine();
			}
			catch
			{ }
		}

		public static void LogBadPackets_Command(CommandEventArgs e)
		{
			PacketReader.LogBadPackets = !PacketReader.LogBadPackets;

			e.Mobile.SendMessage("LogBadPackets now = " + PacketReader.LogBadPackets);
		}

		public static object Format(object o)
		{
			if (o is Mobile)
			{
				Mobile m = (Mobile)o;

				if (m.Account == null)
				{
					return String.Format("{0} (no account) at {1} ({2})", m, m.Location, m.Map);
				}

				return String.Format("{0} ('{1}') at {2} ({3})", m, m.Account.Username, m.Location, m.Map);
			}

			if (o is BaseHouse)
			{
				BaseHouse house = (BaseHouse)o;

				if (house.Owner != null)
				{
					Mobile m = house.Owner;

					if (m.Account == null)
					{
						return String.Format(
							"0x{0:X} ({1}) owned by {2} (no account) at {3} ({4})",
							house.Serial.Value,
							house.GetType().Name,
							m.Name,
							house.Location,
							house.Map);
					}

					return String.Format(
						"0x{0:X} ({1}) owned by {2} ('{3}') at {4} ({5})",
						house.Serial.Value,
						house.GetType().Name,
						m.Name,
						m.Account.Username,
						house.Location,
						house.Map);
				}

				return String.Format(
					"0x{0:X} ({1}) at {2} ({3})", house.Serial.Value, house.GetType().Name, house.Location, house.Map);
			}

			if (o is HouseSign)
			{
				HouseSign item = (HouseSign)o;
				if (item.Owner != null)
				{
					BaseHouse house = item.Owner;

					if (house.Owner != null)
					{
						Mobile m = house.Owner;

						if (m.Account == null)
						{
							return String.Format(
								"0x{0:X} (HouseSign) -> ({1}) owned by {2} (no account) at {3} ({4})",
								item.Serial.Value,
								house.GetType().Name,
								m.Name,
								item.Location,
								item.Map);
						}

						return String.Format(
							"0x{0:X} (HouseSign) -> ({1}) owned by {2} ('{3}') at {4} ({5})",
							item.Serial.Value,
							house.GetType().Name,
							m.Name,
							m.Account.Username,
							item.Location,
							item.Map);
					}

					return String.Format(
						"0x{0:X} (HouseSign) -> ({1}) at {2} ({3})", item.Serial.Value, house.GetType().Name, item.Location, item.Map);
				}

				return String.Format("0x{0:X} (null) at {1} ({2})", item.Serial.Value, item.Location, item.Map);
			}

			if (o is Item)
			{
				Item item = (Item)o;

				Mobile m = item.RootParent as Mobile;

				if (m != null)
				{
					if (m.Account == null)
					{
						return String.Format(
							"0x{0:X} ({1}) owned by {2} (no account) at {3} ({4})",
							item.Serial.Value,
							item.GetType().Name,
							m.Name,
							item.GetWorldLocation(),
							item.Map);
					}

					return String.Format(
						"0x{0:X} ({1}) owned by {2} ('{3}') at {4} ({5})",
						item.Serial.Value,
						item.GetType().Name,
						m.Name,
						m.Account.Username,
						item.GetWorldLocation(),
						item.Map);
				}

				return String.Format(
					"0x{0:X} ({1}) at {2} ({3})", item.Serial.Value, item.GetType().Name, item.GetWorldLocation(), item.Map);
			}

			return o;
		}

		public static void WriteLine(Mobile from, string format, params object[] args)
		{
			if (!m_Enabled)
			{
				return;
			}

			string text = String.Format(format, args);

			try
			{
				m_Output.WriteLine("{0}: {1}: {2}", DateTime.UtcNow, from.NetState, text);

				string path = Core.BaseDirectory;

				Account acct = from.Account as Account;

				string name = (acct == null ? from.Name : acct.Username);

				AppendPath(ref path, "Logs");
				AppendPath(ref path, "Commands");
				AppendPath(ref path, from.AccessLevel.ToString());
				path = Path.Combine(path, String.Format("{0}.log", name));

				using (StreamWriter sw = new StreamWriter(path, true))
				{
					sw.WriteLine("{0}: {1}: {2}", DateTime.UtcNow, from.NetState, text);
				}
			}
			catch
			{ }
		}

		private static readonly char[] m_NotSafe = new[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

		public static void AppendPath(ref string path, string toAppend)
		{
			path = Path.Combine(path, toAppend);

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static string Safe(string ip)
		{
			if (ip == null)
			{
				return "null";
			}

			ip = ip.Trim();

			if (ip.Length == 0)
			{
				return "empty";
			}

			bool isSafe = true;

			for (int i = 0; isSafe && i < m_NotSafe.Length; ++i)
			{
				isSafe = (ip.IndexOf(m_NotSafe[i]) == -1);
			}

			if (isSafe)
			{
				return ip;
			}

			StringBuilder sb = new StringBuilder(ip);

			foreach (char t in m_NotSafe)
			{
				sb.Replace(t, '_');
			}

			return sb.ToString();
		}

		public static void EventSink_Command(CommandEventArgs e)
		{
			if (e.Mobile.AccessLevel == AccessLevel.Player)
			{
				return;
			} // no need to log player commands... it's just a waste of resources - Alan
			WriteLine(e.Mobile, "{0} {1} used command '{2} {3}'", e.Mobile.AccessLevel, Format(e.Mobile), e.Command, e.ArgString);
		}

		public static void LogChangeProperty(Mobile from, object o, string name, string oldvalue, string value)
		{
			WriteLine(
				from,
				"{0} {1} set property '{2}' of {3} from '{4}' to '{5}'",
				from.AccessLevel,
				Format(from),
				name,
				Format(o),
				oldvalue,
				value);
		}
	}

	// Alan's Logging system 
	public class LoggingCustom
	{
		// Default: false
		private static bool _Enabled;
		private static bool _CommandDebug;
		private static bool _UberScript = true;

		public static bool Enabled
		{
			get { return _Enabled; }
			set
			{
				if (_Enabled == value)
				{
					return;
				}

				_Enabled = value;
				SaveState();
			}
		}

		public static bool CommandDebug
		{
			get { return _CommandDebug; }
			set
			{
				if (_CommandDebug == value)
				{
					return;
				}

				_CommandDebug = value;
				SaveState();
			}
		}

		public static bool UberScript
		{
			get { return _UberScript; }
			set
			{
				if (_UberScript == value)
				{
					return;
				}

				_UberScript = value;
				SaveState();
			}
		}

		private static void SaveState()
		{
			PersistenceFile.Serialize(
				w =>
				{
					w.SetVersion(0);

					w.Write(_Enabled);
					w.Write(_CommandDebug);
					w.Write(_UberScript);
				});
		}

		private static void LoadState()
		{
			PersistenceFile.Deserialize(
				r =>
				{
					r.GetVersion();

					_Enabled = r.ReadBool();
					_CommandDebug = r.ReadBool();
					_UberScript = r.ReadBool();
				});
		}

		private static FileInfo PersistenceFile { get { return IOUtility.EnsureFile("Logs/Custom/LoggingOptions.bin"); } }

		public static FileInfo CommandDebugFile { get { return IOUtility.EnsureFile("Logs/Custom/CommandDebug.txt"); } }

		public static FileInfo MiscFile { get { return IOUtility.EnsureFile("Logs/Custom/Misc.txt"); } }
		public static FileInfo DisguisesFile { get { return IOUtility.EnsureFile("Logs/Custom/Disguises.txt"); } }
		public static FileInfo SpeedExploitFile { get { return IOUtility.EnsureFile("Logs/Custom/SpeedExploit.txt"); } }
		public static FileInfo CounselorFile { get { return IOUtility.EnsureFile("Logs/Custom/Counselors.txt"); } }
		public static FileInfo PseudoseerFile { get { return IOUtility.EnsureFile("Logs/Custom/Pseudoseers.txt"); } }
		public static FileInfo CompanionFile { get { return IOUtility.EnsureFile("Logs/Custom/Companions.txt"); } }
		public static FileInfo HarvesterFile { get { return IOUtility.EnsureFile("Logs/Custom/Harvesting.txt"); } }
		public static FileInfo HouseSignFile { get { return IOUtility.EnsureFile("Logs/Custom/HouseSignClick.txt"); } }

		static LoggingCustom()
		{
			LoadState();
		}

		public static void Initialize()
		{
			CommandUtility.Register("CustomLogs", AccessLevel.Administrator, OnLoggingCommand);
			CommandUtility.Register("CommandDebug", AccessLevel.Administrator, OnDebugCommand);
			CommandUtility.Register("UberScriptLogs", AccessLevel.Administrator, OnUberScriptCommand);
		}

		public static void OnLoggingCommand(CommandEventArgs e)
		{
			Enabled = !Enabled;
			e.Mobile.SendMessage("Custom Logging has been {0}.", Enabled ? "enabled" : "disabled");
		}

		public static void OnDebugCommand(CommandEventArgs e)
		{
			CommandDebug = !CommandDebug;
			e.Mobile.SendMessage("Command Debug Logging has been {0}.", CommandDebug ? "enabled" : "disabled");
		}

		public static void OnUberScriptCommand(CommandEventArgs e)
		{
			UberScript = !UberScript;
			e.Mobile.SendMessage("UberScript Logging has been {0}.", UberScript ? "enabled" : "disabled");
		}

		public static void LogCommandDebug(params string[] lines)
		{
			if (_CommandDebug)
			{
				CommandDebugFile.AppendText(false, lines);
			}
		}

		public static void LogUberScript(string path, params string[] lines)
		{
			// Redirect file writes to the Logs/Custom/ directory if the path doesn't begin with // or <drive>:
			if (!Path.IsPathRooted(path) && !Insensitive.StartsWith(path, @"Logs/UberScript/") &&
				!Insensitive.StartsWith(path, @"Logs\UberScript\"))
			{
				path = "Logs" + IOUtility.SEPARATOR + "UberScript" + IOUtility.SEPARATOR + path;
			}

			FileInfo file = IOUtility.EnsureFile(path);

			LogUberScript(file, lines);
		}

		public static void LogUberScript(FileInfo file, params string[] lines)
		{
			if (UberScript && file != null && file.Exists)
			{
				file.AppendText(false, lines);
			}
		}

		public static void Log(string path, params string[] lines)
		{
			// Redirect file writes to the Logs/Custom/ directory if the path doesn't begin with // or <drive>:
			if (!Path.IsPathRooted(path) && !Insensitive.StartsWith(path, @"Logs/Custom/") &&
				!Insensitive.StartsWith(path, @"Logs\Custom\"))
			{
				path = "Logs" + IOUtility.SEPARATOR + "Custom" + IOUtility.SEPARATOR + path;
			}

			FileInfo file = IOUtility.EnsureFile(path);

			Log(file, lines);
		}

		public static void Log(FileInfo file, params string[] lines)
		{
			if (file != null && file.Exists)
			{
				file.AppendText(false, lines);
			}
		}

		public static void LogMisc(params string[] lines)
		{
			if (Enabled)
			{
				Log(MiscFile, lines);
			}
		}

		public static void LogDisguise(params string[] lines)
		{
			if (Enabled)
			{
				Log(DisguisesFile, lines);
			}
		}

		public static void LogSpeedExploit(params string[] lines)
		{
			if (Enabled)
			{
				Log(SpeedExploitFile, lines);
			}
		}

		public static void LogCounselor(params string[] lines)
		{
			if (Enabled)
			{
				Log(CounselorFile, lines);
			}
		}

		public static void LogPseudoseer(params string[] lines)
		{
			if (Enabled)
			{
				Log(PseudoseerFile, lines);
			}
		}

		public static void LogCompanion(params string[] lines)
		{
			if (Enabled)
			{
				Log(CompanionFile, lines);
			}
		}

		public static void LogHarvester(params string[] lines)
		{
			if (Enabled)
			{
				Log(HarvesterFile, lines);
			}
		}

		public static void LogHouseSign(params string[] lines)
		{
			if (Enabled)
			{
				Log(HouseSignFile, lines);
			}
		}
	}
}