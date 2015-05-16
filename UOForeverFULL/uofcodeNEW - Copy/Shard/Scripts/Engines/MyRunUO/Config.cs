#region References
using System;
using System.IO;
using System.Linq;
using System.Threading;

using Server.Gumps;
using Server.Network;

using VitaNex;
using VitaNex.IO;
using VitaNex.MySQL;
using VitaNex.Text;
#endregion

namespace Server.Engines.MyRunUO
{
	public class Config
	{
		public static MyRunUOOptions Options { get; private set; }

		public static bool DatabaseNonLocal
		{
			get
			{
				return Options.ConnectionInfo.IP != "localhost" && Options.ConnectionInfo.IP != "127.0.0.1" &&
					   Listener.EndPoints.All(ip => ip.Address.ToString() != Options.ConnectionInfo.IP);
			}
		}

		private static FileInfo PersistenceFile { get { return IOUtility.EnsureFile(VitaNexCore.SavesDirectory + "/MyRunUO/Options.bin"); } }

		static Config()
		{
			Options = new MyRunUOOptions();

			LoadState();
		}

		private static bool _Configured;

		public static void Configure()
		{
			if (_Configured)
			{
				return;
			}

			_Configured = true;

			CommandUtility.Register(
				"MyRunUOConfig",
				AccessLevel.Administrator,
				e =>
				{
					if (e.Mobile != null)
					{
						e.Mobile.SendGump(new PropertiesGump(e.Mobile, Options));
					}
				});

			EventSink.WorldSave += e => SaveState();
		}

		private static void SaveState()
		{
			PersistenceFile.Serialize(Options.Serialize);
		}

		private static void LoadState()
		{
			PersistenceFile.Deserialize(Options.Deserialize);
		}

		public static string CompileConnectionString()
		{
			return Options.ConnectionInfo.GetConnectionString();
		}
	}

	public sealed class MyRunUOOptions : PropertyObject
	{
		private TimeSpan _CharUpdateInterval;
		private TimeSpan _StatusUpdateInterval;

		[CommandProperty(AccessLevel.Administrator)]
		public MySQLConnectionInfo ConnectionInfo { get; set; }

		/// <summary>
		///     Is MyRunUO enabled?
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool Enabled { get; set; }

		/// <summary>
		///     Should the database use transactions? This is recommended
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool UseTransactions { get; set; }

		/// <summary>
		///     Use optimized table loading techniques? (LOAD DATA INFILE)
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool LoadDataInFile { get; set; }

		/// <summary>
		///     Text encoding used
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public EncodingType Encoding { get; set; }

		/// <summary>
		///     Database communication is done in a separate thread. This value is the 'priority' of that thread, or, how much CPU it will try to use
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public ThreadPriority Priority { get; set; }

		/// <summary>
		///     Any character with an AccessLevel equal to or higher than this will not be displayed
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public AccessLevel HiddenAccessLevel { get; set; }

		/// <summary>
		///     Export character database interval
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan CharUpdateInterval
		{
			get { return _CharUpdateInterval; }
			set
			{
				_CharUpdateInterval = value;

				if (MyRunUO.UpdateTimer != null)
				{
					MyRunUO.UpdateTimer.Interval = _CharUpdateInterval;
				}
			}
		}

		/// <summary>
		///     Export online list database interval
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan StatusUpdateInterval
		{
			get { return _StatusUpdateInterval; }
			set
			{
				_StatusUpdateInterval = value;

				if (MyRunUOStatus.UpdateTimer != null)
				{
					MyRunUOStatus.UpdateTimer.Interval = _StatusUpdateInterval;
				}
			}
		}

		//Defaults should be changed via the [MyRunUOConfig command.
		public MyRunUOOptions()
		{
			ConnectionInfo = new MySQLConnectionInfo(
				"localhost",
				3306,
				"ultima",
				"vista2k3",
				ODBCVersion.V_5_3_UNICODE,
				ShardInfo.IsTestCenter ? "myrunuo_test" : "myrunuo");

			Enabled = false;

			UseTransactions = true;
			LoadDataInFile = false;

			Encoding = EncodingType.ASCII;
			Priority = ThreadPriority.Normal;
			HiddenAccessLevel = AccessLevel.Counselor;

			CharUpdateInterval = TimeSpan.FromMinutes(30.0);
			StatusUpdateInterval = TimeSpan.FromMinutes(5.0);
		}

		public MyRunUOOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			ConnectionInfo.Clear();

			Enabled = false;

			UseTransactions = true;
			LoadDataInFile = false;

			Encoding = EncodingType.ASCII;
			Priority = ThreadPriority.Normal;
			HiddenAccessLevel = AccessLevel.Counselor;

			CharUpdateInterval = TimeSpan.FromMinutes(30.0);
			StatusUpdateInterval = TimeSpan.FromMinutes(5.0);
		}

		public override void Reset()
		{
			ConnectionInfo.Reset();

			Enabled = false;

			UseTransactions = true;
			LoadDataInFile = false;

			Encoding = EncodingType.ASCII;
			Priority = ThreadPriority.Normal;
			HiddenAccessLevel = AccessLevel.Counselor;

			CharUpdateInterval = TimeSpan.FromMinutes(30.0);
			StatusUpdateInterval = TimeSpan.FromMinutes(5.0);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
						ConnectionInfo.Serialize(writer);

						writer.Write(Enabled);
						writer.Write(UseTransactions);
						writer.Write(LoadDataInFile);

						writer.WriteFlag(Encoding);
						writer.WriteFlag(Priority);
						writer.WriteFlag(HiddenAccessLevel);

						writer.Write(CharUpdateInterval);
						writer.Write(StatusUpdateInterval);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
						ConnectionInfo = new MySQLConnectionInfo(reader);

						Enabled = reader.ReadBool();
						UseTransactions = reader.ReadBool();
						LoadDataInFile = reader.ReadBool();

						Encoding = reader.ReadFlag<EncodingType>();
						Priority = reader.ReadFlag<ThreadPriority>();
						HiddenAccessLevel = reader.ReadFlag<AccessLevel>();

						CharUpdateInterval = reader.ReadTimeSpan();
						StatusUpdateInterval = reader.ReadTimeSpan();
					}
					break;
			}
		}
	}
}