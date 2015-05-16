#region References
using System;
using System.Linq;

using VitaNex;
using VitaNex.MySQL;
using VitaNex.Reflection;
#endregion

namespace Server.Engines.Conquests
{
	public static partial class Conquests
	{
		private static MySQLConnection _Connection;

		public static void ExportMySQL()
		{
			if (!CMOptions.ModuleEnabled || !CMOptions.MySQLEnabled || !CMOptions.MySQLInfo.IsValid())
			{
				if (_Connection != null)
				{
					_Connection.Dispose();
					_Connection = null;
				}

				return;
			}

			if (_Connection != null && !_Connection.IsDisposed)
			{
				return;
			}

			CMOptions.ToConsole("Updating MySQL database...");
			
			VitaNexCore.TryCatch(
				() =>
				{
					_Connection = new MySQLConnection(CMOptions.MySQLInfo);
					_Connection.ConnectAsync(0, true,() =>
					{
						var a = new Action(UpdateMySQL);

						a.BeginInvoke(
							r =>
							{
								a.EndInvoke(r);
								UpdateMySQL();
							},
							null);
					});
				},
				x =>
				{
					if (_Connection != null)
					{
						_Connection.Dispose();
						_Connection = null;
					}

					CMOptions.ToConsole(x);
				});
		}

		private static void UpdateMySQL()
		{
			if (!CMOptions.ModuleEnabled || !CMOptions.MySQLEnabled || !CMOptions.MySQLInfo.IsValid())
			{
				if (_Connection != null)
				{
					_Connection.Dispose();
					_Connection = null;
				}

				return;
			}

			if (_Connection == null || _Connection.IsDisposed)
			{
				_Connection = null;
				return;
			}

			if (!_Connection.Connected)
			{
				_Connection.Dispose();
				_Connection = null;
				return;
			}

			using (_Connection)
			{
				#region Conquests
				CMOptions.ToConsole("Exporting conquests...");

				Conquest[] cList = ConquestRegistry.Values.Where(c => c != null && !c.Deleted).ToArray();

				_Connection.NonQuery(GetCreateQuery("conquests"));
				_Connection.Truncate("conquests");

				int cCount = 0;

				foreach (Conquest c in cList)
				{
					var sData = new MySQLData("id_con", c.UID.Value);

					using (var props = new PropertyList<Conquest>(c))
					{
						props.RemoveKeyRange("UID", "");

						int pc =
							props.Count(
								kv =>
								{
									CMOptions.ToConsole("Prop: {0} = {1}", kv.Key, kv.Value);
									
									return _Connection.Insert("conquests", new[] {sData, new MySQLData("key", kv.Key), new MySQLData("val", kv.Value)});
								});

						if (pc == props.Count)
						{
							++cCount;
						}
					}
				}

				CMOptions.ToConsole("Exported {0:#,0} conquests.", cCount);
				#endregion

				#region Profiles
				CMOptions.ToConsole("Exporting profiles...");

				ConquestProfile[] pList = Profiles.Values.Where(p => p != null && p.Owner != null).ToArray();

				_Connection.NonQuery(GetCreateQuery("profiles"));
				_Connection.Truncate("profiles");

				int pCount =
					pList.Count(
						p =>
						_Connection.Insert(
							"profiles",
							new[]
							{
								new MySQLData("id_owner", p.Owner.Serial.Value), new MySQLData("owner", p.Owner.RawName),
								new MySQLData("points", p.GetPointsTotal()), new MySQLData("credit", p.Credit)
							}));

				CMOptions.ToConsole("Exported {0:#,0} profiles.", pCount);
				#endregion

				#region States
				CMOptions.ToConsole("Exporting states...");

				_Connection.NonQuery(GetCreateQuery("states"));
				_Connection.Truncate("states");

				int sCount = 0;

				foreach (var p in pList)
				{
					foreach (var s in p)
					{
						var sData = new MySQLData("id_state", s.UID.Value);
						var cData = new MySQLData("id_con", s.Conquest.UID.Value);
						var oData = new MySQLData("id_owner", s.Owner.Serial.Value);

						using (var props = new PropertyList<ConquestState>(s))
						{
							props.Remove("UID");
							props.Remove("Conquest");
							props.Remove("Owner");
							props.Remove("ConquestExists");

							int pc =
								props.Count(
									kv =>
									_Connection.Insert(
										"states", new[] {sData, cData, oData, new MySQLData("key", kv.Key), new MySQLData("val", kv.Value)}));

							if (pc == props.Count)
							{
								++sCount;
							}
						}
					}
				}

				CMOptions.ToConsole("Exported {0:#,0} states.", sCount);
				#endregion
			}

			_Connection = null;
		}

		private static string GetCreateQuery(string table)
		{
			switch (table)
			{
				case "conquests":
					return "CREATE TABLE IF NOT EXISTS `conquests` " + //
						   "(" + //
						   "`id` INT UNSIGNED NOT NULL AUTO_INCREMENT, " + //
						   "`id_con` VARCHAR(255) NOT NULL, " + //
						   "`key` VARCHAR(255) NOT NULL, " + //
						   "`val` TEXT NULL, " + //
						   "PRIMARY KEY (`id`), " + //
						   "UNIQUE INDEX `id_UNIQUE` (`id` ASC), " + //
						   "INDEX `id_con_INDEX` (`id_con` ASC) " + //
						   ")";
				case "profiles":
					return "CREATE TABLE IF NOT EXISTS `profiles` " + //
						   "(" + //
						   "`id` INT UNSIGNED NOT NULL AUTO_INCREMENT, " + //
						   "`id_owner` INT NOT NULL, " + //
						   "`owner` VARCHAR(255) NOT NULL, " + //
						   "`points` BIGINT UNSIGNED NULL DEFAULT 0, " + //
						   "`credit` BIGINT UNSIGNED NULL DEFAULT 0, " + //
						   "PRIMARY KEY (`id`), " + //
						   "UNIQUE INDEX `id_UNIQUE` (`id` ASC), " + //
						   "INDEX `id_owner_INDEX` (`id_owner` ASC) " + //
						   ")";
				case "states":
					return "CREATE TABLE IF NOT EXISTS `states` " + //
						   "(" + //
						   "`id` INT UNSIGNED NOT NULL AUTO_INCREMENT, " + //
						   "`id_state` VARCHAR(255) NOT NULL, " + //
						   "`id_con` VARCHAR(255) NOT NULL, " + //
						   "`id_owner` INT NOT NULL, " + //
						   "`key` VARCHAR(255) NOT NULL, " + //
						   "`val` TEXT NULL, " + //
						   "PRIMARY KEY (`id`), " + //
						   "UNIQUE INDEX `id_UNIQUE` (`id` ASC), " + //
						   "INDEX `id_state_INDEX` (`id_state` ASC), " + //
						   "INDEX `id_con_INDEX` (`id_con` ASC), " + //
						   "INDEX `id_owner_INDEX` (`id_owner` ASC) " + //
						   ")";
			}

			return String.Empty;
		}
	}
}