#region References
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Server.Commands;
using Server.Mobiles;

using VitaNex;
using VitaNex.IO;
#endregion

namespace Server.Misc
{
	public class AutoSave : Timer
	{
		public static AutoSave Instance { get; private set; }

		private static readonly TimeSpan _SaveInterval = TimeSpan.FromMinutes(ShardInfo.IsTestCenter ? 120.0 : 90.0);
		private static readonly TimeSpan _SaveWarning = ShardInfo.IsTestCenter ? TimeSpan.Zero : TimeSpan.FromSeconds(60.0);

		private static readonly TimeSpan _BackupInterval = TimeSpan.FromHours(3.0);
		private static readonly TimeSpan _BackupExpire = TimeSpan.FromDays(7.0);

		private static DateTime _LastBackup = DateTime.MinValue;

		public static bool SavesEnabled { get; set; }

		public static DirectoryInfo BackupSource { get; private set; }
		public static DirectoryInfo BackupTarget { get; private set; }

		static AutoSave()
		{
			Instance = new AutoSave();

			SavesEnabled = true;

			BackupSource = IOUtility.EnsureDirectory(Core.BaseDirectory + "/Saves/");
			BackupTarget = IOUtility.EnsureDirectory(Core.BaseDirectory + "/Backups/Automatic/");
		}

		public static void Initialize()
		{
			Instance.Start();

			CommandSystem.Register("SetSaves", AccessLevel.Administrator, OnSetSavesCommand);
			CommandSystem.Register("SaveBackup", AccessLevel.Administrator, OnSaveBackupCommand);
		}

		[Usage("SetSaves [true | false]")]
		[Description("Enables or disables automatic shard saving.")]
		public static void OnSetSavesCommand(CommandEventArgs e)
		{
			if (e.Length == 1)
			{
				SavesEnabled = e.GetBoolean(0);
				e.Mobile.SendMessage("Saves have been {0}.", SavesEnabled ? "enabled" : "disabled");
			}
			else
			{
				e.Mobile.SendMessage("Format: SetSaves <true | false>");
			}
		}

		[Usage("SaveBackup")]
		[Description("Backs up the current shard saves.")]
		public static void OnSaveBackupCommand(CommandEventArgs e)
		{
			if (World.Saving)
			{
				World.WaitForWriteCompletion();
			}

			try
			{
				Backup();
				e.Mobile.SendMessage("Backup complete!");
			}
			catch (Exception x)
			{
				e.Mobile.SendMessage("Backup failed: {0}", x);
				Console.WriteLine("WARNING: Automatic backup FAILED: {0}", x);
			}
		}

		public AutoSave()
			: base(_SaveInterval - _SaveWarning, _SaveInterval)
		{
			//Console.WriteLine( "World: Autosave initiated." );
			Priority = TimerPriority.OneMinute;
		}

		protected override void OnTick()
		{
			if (!SavesEnabled || AutoRestart.Restarting)
			{
				return;
			}

			if (_SaveWarning <= TimeSpan.Zero)
			{
				Save(true);
				return;
			}

			string n = ServerTime.ShortTimeNow;
			var s = (int)_SaveWarning.TotalSeconds;
			int m = s / 60;
			s %= 60;

			if (m > 0 && s > 0)
			{
				World.Broadcast(
					0x35,
					true,
					"[{0}]: The world will save in {1} minute{2} and {3} second{4}.",
					n,
					m,
					m != 1 ? "s" : "",
					s,
					s != 1 ? "s" : "");
			}
			else if (m > 0)
			{
				World.Broadcast(0x35, true, "[{0}]: The world will save in {1} minute{2}.", n, m, m != 1 ? "s" : "");
			}
			else
			{
				World.Broadcast(0x35, true, "[{0}]: The world will save in {1} second{2}.", n, s, s != 1 ? "s" : "");
			}

			DelayCall(_SaveWarning, Save);
		}

		public static void Save()
		{
			Save(false);
		}

		public static void Save(bool permitBackgroundWrite)
		{
			if (AutoRestart.Restarting || AutoRestart.ServerWars)
			{
				return;
			}

			World.WaitForWriteCompletion();

			if (DateTime.UtcNow > _LastBackup + _BackupInterval)
			{
				try
				{
					Backup();
					_LastBackup = DateTime.UtcNow;
				}
				catch (Exception e)
				{
					Console.WriteLine("WARNING: Automatic backup FAILED: {0}", e);
				}
			}

			foreach (PlayerMobile m in World.Mobiles.Values.OfType<PlayerMobile>().Where(pm => pm.Account == null).ToArray())
			{
				Console.WriteLine("WARNING: Orphan player deleted - {0} [{1}]: {2}", m.Location, m.Map, m.Name);
				m.Delete();
			}

			World.Save(true, permitBackgroundWrite);
		}

		private static void Backup()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					DateTime expire = DateTime.UtcNow.Subtract(_BackupExpire);

					Parallel.ForEach(
						BackupTarget.EnumerateDirectories(),
						dir => VitaNexCore.TryCatch(
							() =>
							{
								if (dir.CreationTimeUtc < expire)
								{
									dir.Delete(true);
								}
							}));
				});

			VitaNexCore.TryCatch(
				() =>
				{
					DirectoryInfo target =
						IOUtility.EnsureDirectory(BackupTarget + "/" + DateTime.Now.ToSimpleString("D d M y - t@h-m@"));

					Parallel.ForEach(
						BackupSource.EnumerateDirectories(),
						dir =>
						VitaNexCore.TryCatch(
							() => dir.CopyDirectory(IOUtility.EnsureDirectory(dir.FullName.Replace(BackupSource.FullName, target.FullName)))));
				});
		}
	}
}