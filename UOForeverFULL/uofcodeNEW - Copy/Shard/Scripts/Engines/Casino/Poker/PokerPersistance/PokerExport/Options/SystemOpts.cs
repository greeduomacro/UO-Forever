#region References
using System;

using Server;
using VitaNex;
using VitaNex.MySQL;
#endregion

namespace Server.Poker
{
	public sealed class PokerOptions : CoreModuleOptions
	{
		public static int DefQueueCapacity = 10000;
		public static int DefExportCapacity = 1000;
		public static bool DefUpdateTimer = true;
		public static TimeSpan DefUpdateInterval = TimeSpan.FromMinutes(60.0);

		[CommandProperty(PokerExport.Access)]
		public MySQLConnectionInfo MySQL { get; set; }

        [CommandProperty(PokerExport.Access)]
		public bool UseTransactions { get; set; }

        [CommandProperty(PokerExport.Access)]
		public int QueueCapacity { get; set; }

        [CommandProperty(PokerExport.Access)]
		public int ExportCapacity { get; set; }

        [CommandProperty(PokerExport.Access)]
		public bool UpdateTimer { get; set; }

        [CommandProperty(PokerExport.Access)]
		public TimeSpan UpdateInterval { get; set; }

        [CommandProperty(PokerExport.Access)]
        public bool Poker { get { return PokerExport.Connection.Connected; } }

        [CommandProperty(PokerExport.Access)]
		public bool Valid { get { return MySQL.IsValid(); } }

        [CommandProperty(PokerExport.Access)]
        public TimeSpan UpdateTimeAverage { get { return PokerExport.UpdateTimeAverage; } }

        [CommandProperty(PokerExport.Access)]
        public TimeSpan UpdateTimeMin { get { return PokerExport.UpdateTimeMin; } }

        [CommandProperty(PokerExport.Access)]
        public TimeSpan UpdateTimeMax { get { return PokerExport.UpdateTimeMax; } }

        [CommandProperty(PokerExport.Access)]
        public bool Updating { get { return PokerExport.Updating; } }

		public PokerOptions()
            : base(typeof(PokerExport))
		{
			MySQL = new MySQLConnectionInfo(
				"localhost", 3306, "root", "", ODBCVersion.V_5_1, "uof_poker");

			EnsureDefaults();
		}

        public PokerOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
			QueueCapacity = DefQueueCapacity;
			ExportCapacity = DefExportCapacity;
			UpdateTimer = DefUpdateTimer;
			UpdateInterval = DefUpdateInterval;
		}

		public override void Clear()
		{
			base.Clear();

			MySQL = new MySQLConnectionInfo("", 0, "", "", ODBCVersion.V_5_1, "");

			EnsureDefaults();
		}

		public override void Reset()
		{
			base.Reset();

			MySQL = new MySQLConnectionInfo(
				"localhost", 3306, "root", "", ODBCVersion.V_5_1, "uof_poker");

			EnsureDefaults();
		}

		public override string ToString()
		{
			return "UOF Poker Export Config";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(3);

			MySQL.Serialize(writer);

			switch (version)
			{
				case 3:
					{
						writer.Write(UpdateTimer);
						writer.Write(UpdateInterval);
					}
					goto case 2;
				case 2:
					writer.Write(ExportCapacity);
					goto case 1;
				case 1:
					writer.Write(QueueCapacity);
					goto case 0;
				case 0:
					writer.Write(UseTransactions);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			MySQL = new MySQLConnectionInfo(reader);

			switch (version)
			{
				case 3:
					{
						UpdateTimer = reader.ReadBool();
						UpdateInterval = reader.ReadTimeSpan();
					}
					goto case 2;
				case 2:
					ExportCapacity = reader.ReadInt();
					goto case 1;
				case 1:
					QueueCapacity = reader.ReadInt();
					goto case 0;
				case 0:
					UseTransactions = reader.ReadBool();
					break;
			}

			if (version < 3)
			{
				UpdateTimer = DefUpdateTimer;
				UpdateInterval = DefUpdateInterval;
			}

			if (version < 2)
			{
				ExportCapacity = DefExportCapacity;
			}

			if (version < 1)
			{
				QueueCapacity = DefQueueCapacity;
			}
		}
	}
}