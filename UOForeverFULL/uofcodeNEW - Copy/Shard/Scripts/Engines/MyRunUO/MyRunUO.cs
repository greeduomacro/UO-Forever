#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Server.Accounting;
using Server.Commands;
using Server.Guilds;
using Server.Misc;
using Server.Mobiles;

using VitaNex;
#endregion

namespace Server.Engines.MyRunUO
{
	public class MyRunUO : Timer
	{
		// Processor runs every 0.1 seconds
		private const double CpuInterval = 0.1;

		// Processor runs for 25% of Interval, or ~25ms. This should take around 25% cpu
		private const double CpuPercent = 0.25;

		public static PollTimer UpdateTimer { get; private set; }

		public static void Initialize()
		{
			UpdateTimer = PollTimer.CreateInstance(Config.Options.CharUpdateInterval, Begin, () => Config.Options.Enabled);

			CommandSystem.Register("UpdateMyRunUO", AccessLevel.Administrator, UpdateMyRunUO_OnCommand);

			CommandSystem.Register("PublicChar", AccessLevel.Player, PublicChar_OnCommand);
			CommandSystem.Register("PrivateChar", AccessLevel.Player, PrivateChar_OnCommand);
		}

		[Usage("PublicChar")]
		[Description("Enables showing extended character stats and skills in MyRunUO.")]
		public static void PublicChar_OnCommand(CommandEventArgs e)
		{
			if (!Config.Options.Enabled)
			{
				return;
			}

			var pm = e.Mobile as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			if (pm.PublicMyRunUO)
			{
				pm.SendMessage("You have already chosen to show your skills and stats.");
			}
			else
			{
				pm.PublicMyRunUO = true;
				pm.SendMessage("All of your skills and stats will now be shown publicly in MyRunUO.");
			}
		}

		[Usage("PrivateChar")]
		[Description("Disables showing extended character stats and skills in MyRunUO.")]
		public static void PrivateChar_OnCommand(CommandEventArgs e)
		{
			if (!Config.Options.Enabled)
			{
				return;
			}

			var pm = e.Mobile as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			if (!pm.PublicMyRunUO)
			{
				pm.SendMessage("You have already chosen to not show your skills and stats.");
			}
			else
			{
				pm.PublicMyRunUO = false;
				pm.SendMessage("Only a general level of your top three skills will be shown in MyRunUO.");
			}
		}

		[Usage("UpdateMyRunUO")]
		[Description("Starts the process of updating the MyRunUO character and guild database.")]
		public static void UpdateMyRunUO_OnCommand(CommandEventArgs e)
		{
			if (!Config.Options.Enabled)
			{
				return;
			}

			if (m_Command != null && m_Command.HasCompleted)
			{
				m_Command = null;
			}

			if (m_Timer == null && m_Command == null)
			{
				Begin();
				e.Mobile.SendMessage("MyRunUO update process has been started.");
			}
			else
			{
				e.Mobile.SendMessage("MyRunUO database is already being updated.");
			}
		}

		public static void Begin()
		{
			if (m_Command != null && m_Command.HasCompleted)
			{
				m_Command = null;
			}

			if (m_Timer != null || m_Command != null)
			{
				return;
			}

			m_Timer = new MyRunUO();
			m_Timer.Start();
		}

		private static Timer m_Timer;

		private Stage m_Stage;
		private List<object> m_List;
		private List<IAccount> m_Collecting;
		private int m_Index;

		private static DatabaseCommandQueue m_Command;

		private string m_SkillsPath;
		private string m_LayersPath;
		private string m_MobilesPath;

		private StreamWriter m_OpSkills;
		private StreamWriter m_OpLayers;
		private StreamWriter m_OpMobiles;

		private readonly DateTime m_StartTime;

		public MyRunUO()
			: base(TimeSpan.FromSeconds(CpuInterval), TimeSpan.FromSeconds(CpuInterval))
		{
			m_List = new List<object>();
			m_Collecting = new List<IAccount>();

			m_StartTime = DateTime.UtcNow;

			Console.WriteLine("MyRunUO: Updating character database");
		}

		protected override void OnTick()
		{
			bool shouldExit;

			try
			{
				shouldExit = Process(DateTime.UtcNow + TimeSpan.FromSeconds(CpuInterval * CpuPercent));

				if (shouldExit)
				{
					Console.WriteLine(
						"MyRunUO: Database statements compiled in {0:F2} seconds", (DateTime.UtcNow - m_StartTime).TotalSeconds);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("MyRunUO: {0}: Exception caught while processing", m_Stage);
				Console.WriteLine(e);
				shouldExit = true;
			}

			if (!shouldExit)
			{
				return;
			}

			m_Command.Enqueue(null);

			Stop();
			m_Timer = null;
		}

		private enum Stage
		{
			CollectingMobiles,
			DumpingMobiles,
			CollectingGuilds,
			DumpingGuilds,
			Complete
		}

		public bool Process(DateTime endTime)
		{
			switch (m_Stage)
			{
				case Stage.CollectingMobiles:
					CollectMobiles(endTime);
					break;
				case Stage.DumpingMobiles:
					DumpMobiles(endTime);
					break;
				case Stage.CollectingGuilds:
					CollectGuilds(endTime);
					break;
				case Stage.DumpingGuilds:
					DumpGuilds(endTime);
					break;
			}

			return (m_Stage == Stage.Complete);
		}

		private static List<Mobile> m_Mobiles = new List<Mobile>();

		public static void QueueMobileUpdate(Mobile m)
		{
			if (Config.Options.Enabled && !Config.Options.LoadDataInFile)
			{
				m_Mobiles.Add(m);
			}
		}

		public void CollectMobiles(DateTime endTime)
		{
			if (Config.Options.LoadDataInFile)
			{
				if (m_Index == 0)
				{
					m_Collecting.AddRange(Accounts.GetAccounts());
				}

				for (int i = m_Index; i < m_Collecting.Count; ++i)
				{
					IAccount acct = m_Collecting[i];

					for (int j = 0; j < acct.Length; ++j)
					{
						Mobile mob = acct[j];

						if (mob != null && mob.AccessLevel < Config.Options.HiddenAccessLevel)
						{
							m_List.Add(mob);
						}
					}

					++m_Index;

					if (DateTime.UtcNow >= endTime)
					{
						break;
					}
				}

				if (m_Index == m_Collecting.Count)
				{
					m_Collecting = new List<IAccount>();
					m_Stage = Stage.DumpingMobiles;
					m_Index = 0;
				}
			}
			else
			{
				m_List = new List<object>(m_Mobiles);
				m_Mobiles = new List<Mobile>();
				m_Stage = Stage.DumpingMobiles;
				m_Index = 0;
			}
		}

		public void CheckConnection()
		{
			if (m_Command == null)
			{
				m_Command = new DatabaseCommandQueue(
					"MyRunUO: Character database updated in {0:F1} seconds", "MyRunUO Character Database Thread");

				if (Config.Options.LoadDataInFile)
				{
					m_OpSkills = GetUniqueWriter("skills", out m_SkillsPath);
					m_OpLayers = GetUniqueWriter("layers", out m_LayersPath);
					m_OpMobiles = GetUniqueWriter("mobiles", out m_MobilesPath);

					m_Command.Enqueue("TRUNCATE TABLE myrunuo_characters");
					m_Command.Enqueue("TRUNCATE TABLE myrunuo_characters_layers");
					m_Command.Enqueue("TRUNCATE TABLE myrunuo_characters_skills");
				}

				m_Command.Enqueue("TRUNCATE TABLE myrunuo_guilds");
				m_Command.Enqueue("TRUNCATE TABLE myrunuo_guilds_wars");
			}
		}

		public void ExecuteNonQuery(string format, params object[] args)
		{
			m_Command.Enqueue(String.Format(format, args));
		}

		public void ExecuteNonQueryIfNull(string select, string insert)
		{
			m_Command.Enqueue(new[] {select, insert});
		}

		private static void AppendCharEntity(string input, int charIndex, ref StringBuilder sb, char c)
		{
			if (sb == null)
			{
				sb = charIndex > 0
						 ? new StringBuilder(input, 0, charIndex, input.Length + 20)
						 : new StringBuilder(input.Length + 20);
			}

			sb.Append("&#");
			sb.Append((int)c);
			sb.Append(";");
		}

		private static void AppendEntityRef(string input, int charIndex, ref StringBuilder sb, string ent)
		{
			if (sb == null)
			{
				sb = charIndex > 0
						 ? new StringBuilder(input, 0, charIndex, input.Length + 20)
						 : new StringBuilder(input.Length + 20);
			}

			sb.Append(ent);
		}

		private static string SafeString(string input)
		{
			if (input == null)
			{
				return "";
			}

			//return WebUtility.HtmlEncode(input);

			StringBuilder sb = null;

			for (int i = 0; i < input.Length; ++i)
			{
				char c = input[i];

				if (c < 0x20 || c >= 0x7F)
				{
					AppendCharEntity(input, i, ref sb, c);
				}
				else
				{
					switch (c)
					{
						case '&':
							AppendEntityRef(input, i, ref sb, "&amp;");
							break;
						case '>':
							AppendEntityRef(input, i, ref sb, "&gt;");
							break;
						case '<':
							AppendEntityRef(input, i, ref sb, "&lt;");
							break;
						case '"':
							AppendEntityRef(input, i, ref sb, "&quot;");
							break;
						case '`':
						case ',':
						case '/':
						case '\'':
						case '\\':
							AppendCharEntity(input, i, ref sb, c);
							break;
						default:
							{
								if (sb != null)
								{
									sb.Append(c);
								}

								break;
							}
					}
				}
			}

			if (sb != null)
			{
				return sb.ToString();
			}

			return input;
		}

		public const char LineStart = '\"';
		public const string EntrySep = "\",\"";
		public const string LineEnd = "\"\n";

		public void InsertMobile(Mobile mob)
		{
			string guildTitle = mob.GuildTitle;

			if (guildTitle == null || (guildTitle = guildTitle.Trim()).Length == 0)
			{
				guildTitle = "NULL";
			}
			else
			{
				guildTitle = SafeString(guildTitle);
			}

			string notoTitle = SafeString(Titles.ComputeTitle(null, mob));
			string female = (mob.Female ? "1" : "0");

			bool pubBool = (mob is PlayerMobile) && (((PlayerMobile)mob).PublicMyRunUO);

			string pubString = (pubBool ? "1" : "0");

			string guildId = (mob.Guild == null ? "NULL" : mob.Guild.Id.ToString(CultureInfo.InvariantCulture));

			if (Config.Options.LoadDataInFile)
			{
				m_OpMobiles.Write(LineStart);
				m_OpMobiles.Write(mob.Serial.Value);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(SafeString(mob.Name));
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(mob.RawStr);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(mob.RawDex);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(mob.RawInt);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(female);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(mob.Kills);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(guildId);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(guildTitle);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(notoTitle);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(mob.Hue);
				m_OpMobiles.Write(EntrySep);
				m_OpMobiles.Write(pubString);
				m_OpMobiles.Write(LineEnd);
			}
			else
			{
				ExecuteNonQuery(
					"REPLACE INTO myrunuo_characters (char_id, char_name, char_str, char_dex, char_int, char_female, char_counts, char_guild, char_guildtitle, char_nototitle, char_bodyhue, char_public ) VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}, {11})",
					mob.Serial.Value.ToString(CultureInfo.InvariantCulture),
					SafeString(mob.Name),
					mob.RawStr.ToString(CultureInfo.InvariantCulture),
					mob.RawDex.ToString(CultureInfo.InvariantCulture),
					mob.RawInt.ToString(CultureInfo.InvariantCulture),
					female,
					mob.Kills.ToString(CultureInfo.InvariantCulture),
					guildId,
					guildTitle,
					notoTitle,
					mob.Hue.ToString(CultureInfo.InvariantCulture),
					pubString);
			}
		}

		public void InsertSkills(Mobile mob)
		{
			Skills skills = mob.Skills;
			string serial = mob.Serial.Value.ToString(CultureInfo.InvariantCulture);

			for (int i = 0; i < skills.Length; ++i)
			{
				Skill skill = skills[i];

				if (skill.BaseFixedPoint <= 0)
				{
					continue;
				}

				if (Config.Options.LoadDataInFile)
				{
					m_OpSkills.Write(LineStart);
					m_OpSkills.Write(serial);
					m_OpSkills.Write(EntrySep);
					m_OpSkills.Write(i);
					m_OpSkills.Write(EntrySep);
					m_OpSkills.Write(skill.BaseFixedPoint);
					m_OpSkills.Write(LineEnd);
				}
				else
				{
					ExecuteNonQuery(
						"REPLACE INTO myrunuo_characters_skills (char_id, skill_id, skill_value) VALUES ({0}, {1}, {2})",
						serial,
						i.ToString(CultureInfo.InvariantCulture),
						skill.BaseFixedPoint.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		private readonly List<Item> m_Items = new List<Item>();

		private void InsertItem(string serial, int index, int itemID, int hue)
		{
			if (Config.Options.LoadDataInFile)
			{
				m_OpLayers.Write(LineStart);
				m_OpLayers.Write(serial);
				m_OpLayers.Write(EntrySep);
				m_OpLayers.Write(index);
				m_OpLayers.Write(EntrySep);
				m_OpLayers.Write(itemID);
				m_OpLayers.Write(EntrySep);
				m_OpLayers.Write(hue);
				m_OpLayers.Write(LineEnd);
			}
			else
			{
				ExecuteNonQuery(
					"REPLACE INTO myrunuo_characters_layers (char_id, layer_id, item_id, item_hue) VALUES ({0}, {1}, {2}, {3})",
					serial,
					index.ToString(CultureInfo.InvariantCulture),
					itemID.ToString(CultureInfo.InvariantCulture),
					hue.ToString(CultureInfo.InvariantCulture));
			}
		}

		public void InsertItems(Mobile mob)
		{
			m_Items.AddRange(mob.Items);

			string serial = mob.Serial.Value.ToString(CultureInfo.InvariantCulture);

			m_Items.Sort(LayerComparer.Instance);

			int index = 0;

			bool hidePants = false;
			bool alive = mob.Alive;
			bool hideHair = !alive;

			int i;
			for (i = 0; i < m_Items.Count; ++i)
			{
				Item item = m_Items[i];

				if (!LayerComparer.IsValid(item))
				{
					break;
				}

				if (!alive && item.ItemID != 8270)
				{
					continue;
				}

				if (item.ItemID == 0x1411 || item.ItemID == 0x141A) // plate legs
				{
					hidePants = true;
				}
				else if (hidePants && item.Layer == Layer.Pants)
				{
					continue;
				}

				if (!hideHair && item.Layer == Layer.Helm)
				{
					hideHair = true;
				}

				InsertItem(serial, index++, item.ItemID, item.Hue);
			}

			if (mob.FacialHairItemID != 0 && alive)
			{
				InsertItem(serial, index++, mob.FacialHairItemID, mob.FacialHairHue);
			}

			if (mob.HairItemID != 0 && !hideHair)
			{
				InsertItem(serial, index, mob.HairItemID, mob.HairHue);
			}

			m_Items.Clear();
		}

		public void DeleteMobile(Mobile mob)
		{
			ExecuteNonQuery(
				"DELETE FROM myrunuo_characters WHERE char_id = {0}", mob.Serial.Value.ToString(CultureInfo.InvariantCulture));
			ExecuteNonQuery(
				"DELETE FROM myrunuo_characters_skills WHERE char_id = {0}", mob.Serial.Value.ToString(CultureInfo.InvariantCulture));
			ExecuteNonQuery(
				"DELETE FROM myrunuo_characters_layers WHERE char_id = {0}", mob.Serial.Value.ToString(CultureInfo.InvariantCulture));
		}

		public StreamWriter GetUniqueWriter(string type, out string filePath)
		{
			filePath =
				Path.Combine(Core.BaseDirectory, String.Format("myrunuodb_{0}.txt", type))
					.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			try
			{
				return new StreamWriter(filePath);
			}
			catch
			{
				for (int i = 0; i < 100; ++i)
				{
					try
					{
						filePath =
							Path.Combine(Core.BaseDirectory, String.Format("myrunuodb_{0}_{1}.txt", type, i))
								.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
						return new StreamWriter(filePath);
					}
					catch
					{ }
				}
			}

			return null;
		}

		public void DumpMobiles(DateTime endTime)
		{
			CheckConnection();

			for (int i = m_Index; i < m_List.Count; ++i)
			{
				var mob = (Mobile)m_List[i];

				if (!mob.Deleted && mob.AccessLevel < Config.Options.HiddenAccessLevel)
				{
					if (!Config.Options.LoadDataInFile)
					{
						DeleteMobile(mob);
					}

					InsertMobile(mob);
					InsertSkills(mob);
					InsertItems(mob);
				}
				else if (!Config.Options.LoadDataInFile)
				{
					DeleteMobile(mob);
				}

				++m_Index;

				if (DateTime.UtcNow >= endTime)
				{
					break;
				}
			}

			if (m_Index != m_List.Count)
			{
				return;
			}

			m_List.Clear();
			m_Stage = Stage.CollectingGuilds;
			m_Index = 0;

			if (!Config.Options.LoadDataInFile)
			{
				return;
			}

			m_OpSkills.Close();
			m_OpLayers.Close();
			m_OpMobiles.Close();

			ExecuteNonQuery(
				"LOAD DATA {0}INFILE '{1}' INTO TABLE myrunuo_characters FIELDS TERMINATED BY ',' ENCLOSED BY '\"' LINES TERMINATED BY '\n'",
				Config.DatabaseNonLocal ? "LOCAL " : "",
				m_MobilesPath);
			ExecuteNonQuery(
				"LOAD DATA {0}INFILE '{1}' INTO TABLE myrunuo_characters_skills FIELDS TERMINATED BY ',' ENCLOSED BY '\"' LINES TERMINATED BY '\n'",
				Config.DatabaseNonLocal ? "LOCAL " : "",
				m_SkillsPath);
			ExecuteNonQuery(
				"LOAD DATA {0}INFILE '{1}' INTO TABLE myrunuo_characters_layers FIELDS TERMINATED BY ',' ENCLOSED BY '\"' LINES TERMINATED BY '\n'",
				Config.DatabaseNonLocal ? "LOCAL " : "",
				m_LayersPath);
		}

		public void CollectGuilds(DateTime endTime)
		{
			m_List.AddRange(BaseGuild.List.Values);
			m_Stage = Stage.DumpingGuilds;
			m_Index = 0;
		}

		public void InsertGuild(Guild guild)
		{
			string guildType = "Standard";

			switch (guild.Type)
			{
				case GuildType.Chaos:
					guildType = "Chaos";
					break;
				case GuildType.Order:
					guildType = "Order";
					break;
			}

			ExecuteNonQuery(
				"INSERT INTO myrunuo_guilds (guild_id, guild_name, guild_abbreviation, guild_website, guild_charter, guild_type, guild_wars, guild_members, guild_master) VALUES ({0}, '{1}', {2}, {3}, {4}, '{5}', {6}, {7}, {8})",
				guild.Id.ToString(CultureInfo.InvariantCulture),
				SafeString(guild.Name),
				guild.Abbreviation == "none" ? "NULL" : "'" + SafeString(guild.Abbreviation) + "'",
				guild.Website == null ? "NULL" : "'" + SafeString(guild.Website) + "'",
				guild.Charter == null ? "NULL" : "'" + SafeString(guild.Charter) + "'",
				guildType,
				guild.Enemies.Count.ToString(CultureInfo.InvariantCulture),
				guild.Members.Count.ToString(CultureInfo.InvariantCulture),
				guild.Leader.Serial.Value.ToString(CultureInfo.InvariantCulture));
		}

		public void InsertWars(Guild guild)
		{
			string ourId = guild.Id.ToString(CultureInfo.InvariantCulture);

			foreach (string theirId in guild.Enemies.Select(them => them.Id.ToString(CultureInfo.InvariantCulture)))
			{
				ExecuteNonQueryIfNull(
					String.Format(
						"SELECT guild_1 FROM myrunuo_guilds_wars WHERE (guild_1={0} AND guild_2={1}) OR (guild_1={1} AND guild_2={0})",
						ourId,
						theirId),
					String.Format("INSERT INTO myrunuo_guilds_wars (guild_1, guild_2) VALUES ({0}, {1})", ourId, theirId));
			}
		}

		public void DumpGuilds(DateTime endTime)
		{
			CheckConnection();

			for (int i = m_Index; i < m_List.Count; ++i)
			{
				var guild = (Guild)m_List[i];

				if (!guild.Disbanded)
				{
					InsertGuild(guild);
					InsertWars(guild);
				}

				++m_Index;

				if (DateTime.UtcNow >= endTime)
				{
					break;
				}
			}

			if (m_Index != m_List.Count)
			{
				return;
			}

			m_List.Clear();
			m_Stage = Stage.Complete;
			m_Index = 0;
		}
	}
}