#region References
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


using Server.Gumps;
using Server.Mobiles;
using Server.Poker;
using VitaNex;
using VitaNex.Modules.UOFLegends;
using VitaNex.MySQL;

using SQL = VitaNex.MySQL.MySQL;
using ThreadState = System.Threading.ThreadState;
#endregion

namespace Server.Poker
{
	public static partial class PokerExport
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static PokerOptions CMOptions { get; private set; }

		private static MySQLConnection _Connection;

        public static List<PokerHandObject> HandsToExport = new List<PokerHandObject>();
        public static List<PokerActionObject> ActionsToExport = new List<PokerActionObject>();
        public static List<PokerPlayerObject> PlayersToExport = new List<PokerPlayerObject>();

		public static MySQLConnection Connection
		{
			get
			{
				/*if (_Connection != null && !_Connection.Credentials.Equals(CMOptions.MySQL))
				{
					_Connection.Credentials = CMOptions.MySQL;
				}*/

				return _Connection ?? (_Connection = new MySQLConnection(CMOptions.MySQL));
			}
		}

		public static ConcurrentDictionary<string, ConcurrentStack<QueuedData>> ExportQueue { get; private set; }

		public static Type[] ObjectStateTypes { get; private set; }
		public static LegendState[] ObjectStates { get; private set; }

		public static DateTime LastUpdate { get; private set; }
		public static PollTimer UpdateTimer { get; private set; }

		public static Queue<TimeSpan> UpdateTimes { get; private set; }

		public static TimeSpan UpdateTimeAverage
		{
			//
			get { return TimeSpan.FromMilliseconds(UpdateTimes.Count > 0 ? UpdateTimes.Average(ts => ts.TotalMilliseconds) : 0.0); }
		}

		public static TimeSpan UpdateTimeMin
		{
			//
			get { return TimeSpan.FromMilliseconds(UpdateTimes.Count > 0 ? UpdateTimes.Min(ts => ts.TotalMilliseconds) : 0.0); }
		}

		public static TimeSpan UpdateTimeMax
		{
			//
			get { return TimeSpan.FromMilliseconds(UpdateTimes.Count > 0 ? UpdateTimes.Max(ts => ts.TotalMilliseconds) : 0.0); }
		}

		public static Thread ExportThread { get; private set; }

		private static readonly EventWaitHandle _Sync = new EventWaitHandle(false, EventResetMode.ManualReset);

		private static volatile bool _Updating;
		private static volatile int _UpdateCount;

		public static bool Updating { get { return _Updating; } }

		public static void ConnectAsync(Action callback)
		{
			Connection.ConnectAsync(0, true, callback);
		}

		private static void Cancel()
		{
			if (!_Updating)
			{
				ExportQueue.Clear();
				return;
			}

			if (ExportThread != null)
			{
				if (ExportThread.IsAlive || ExportThread.ThreadState == ThreadState.Running)
				{
					ExportThread.Abort();
					ExportThread.Join(3000);
				}

				ExportThread = null;
			}

			if (_Connection != null)
			{
				_Connection.Close();
				_Connection = null;
			}

			_Updating = false;
		}

		private static void Update()
		{
		    if (ActionsToExport.Count > 0 && PlayersToExport.Count > 0 && HandsToExport.Count > 0)
		    {
		        if (ExportThread != null)
		        {
		            if (ExportThread.IsAlive || ExportThread.ThreadState == ThreadState.Running)
		            {
		                ExportThread.Abort();
		                ExportThread.Join(3000);
		            }

		            ExportThread = null;
		        }

		        VitaNexCore.TryCatch<Action>(
		            ConnectAsync,
		            () =>
		            {
		                ExportThread = new Thread(UpdateCallback)
		                {
		                    Name = CMOptions.ModuleName,
		                    Priority = ThreadPriority.BelowNormal
		                };

		                ExportThread.Start();
		            },
		            x =>
		            {
		                CMOptions.ToConsole(x);
		                Cancel();
		            });
		    }
		}

		[STAThread]
		private static void UpdateCallback()
		{
			if (_Updating || !Connection.Connected)
			{
				return;
			}

			_Updating = true;

			LastUpdate = DateTime.Now;

			Connection.NonQuery(
				@"CREATE DATABASE IF NOT EXISTS `{0}` DEFAULT CHARSET `utf8` DEFAULT COLLATE `utf8_bin`",
				CMOptions.MySQL.Database);

			Connection.UseDatabase(CMOptions.MySQL.Database);
		
			var watch = new Stopwatch();
			watch.Start();

			VitaNexCore.TryCatch(
				() =>
				{
					Process();
					Flush();
				},
				x =>
				{
					CMOptions.ToConsole(x);
					OnAbort();
				});

			watch.Stop();

			CMOptions.ToConsole("Updated {0:#,0} objects in {1:F2} seconds.", _UpdateCount, watch.Elapsed.TotalSeconds);

			if (UpdateTimes.Count >= 10)
			{
				UpdateTimes.Dequeue();
			}

			UpdateTimes.Enqueue(watch.Elapsed);

			ExportQueue.Clear();

			if (_Connection != null)
			{
				_Connection.Close();
				_Connection = null;
			}

			//GC.Collect();

			_UpdateCount = 0;
			_Updating = false;
		}

		public static void Process()
		{
            Process("Poker Hands", HandsToExport.ToList());
            HandsToExport.Clear();
            HandsToExport.TrimExcess();

            Process("Poker Actions", ActionsToExport.ToList());
            ActionsToExport.Clear();
            ActionsToExport.TrimExcess();

            Process("Poker Players", PlayersToExport.ToList());
            PlayersToExport.Clear();
            PlayersToExport.TrimExcess();
		}

		public static void Process<T>(string opName, IEnumerable<T> opQueue)
		{
			Process(opName, opQueue.AsParallel());
		}

		public static void Process<T>(string opName, ParallelQuery<T> opQueue)
		{
			if (!_Updating)
			{
				OnAbort();
				return;
			}

			CMOptions.ToConsole("Processing {0}...", opName);


            int c = opQueue.Count();

            List<T> list = VitaNexCore.TryCatchGet(opQueue.Take(c).ToList);

			while (list.Count > 0)
			{
				if (World.Loading)
				{
					Thread.Sleep(10);
					continue;
				}

				if (World.Saving)
				{
					World.WaitForWriteCompletion();
					Thread.Sleep(10);
					continue;
				}

				if (!_Updating)
				{
					OnAbort();
					break;
				}

				int cur = ExportQueue.Values.Sum(l => l.Count);
				int count = Math.Max(0, Math.Min(CMOptions.QueueCapacity - cur, list.Count));

				Parallel.ForEach(list.Take(count), Enqueue);

				list.RemoveRange(0, count);
				list.TrimExcess();

				cur += count;

				if (cur >= CMOptions.QueueCapacity)
				{
					Flush();
				}

				Thread.Sleep(0);
			}

			list.Clear();
			list.TrimExcess();
		}

		private static void Enqueue<T>(T obj)
		{
			if (!_Updating)
			{
				OnAbort();
				return;
			}

			IDictionary<string, SimpleType> data = null;
			LegendState state = ObjectStates.FirstOrDefault(s => s.Compile(obj, out data));

			if (state != null && data != null && data.Count > 0)
			{
				ExportQueue.GetOrAdd(state.TableName, new ConcurrentStack<QueuedData>()).Push(
					new QueuedData
					{
						RawData = data,
						SqlData = data.Select(kd => new MySQLData(kd.Key, SQL.Encode(kd.Value, true))).ToArray()
					});
			}
		}

		private static void Flush()
		{
			if (!_Updating)
			{
				OnAbort();
				return;
			}

			if (CMOptions.UseTransactions)
			{
				Connection.BeginTransaction(FlushCallback, true);
				_Sync.WaitOne();
			}
			else
			{
				FlushCallback(false);
			}
		}

		private static void FlushCallback(bool transactional)
		{
			int total = ExportQueue.Values.Sum(d => d.Count);

			if (!_Updating || ExportQueue.Count == 0 || total == 0)
			{
				if (transactional)
				{
					Connection.EndTransaction();
					_Sync.Set();
				}

				OnAbort();
				return;
			}

			CMOptions.ToConsole("{0:#,0} objects in {1:#,0} tables pending update...", total, ExportQueue.Count);

			int updated = 0;

			ConcurrentStack<QueuedData> stack;
			QueuedData[] pop;
			QueuedData peek;
			MySQLData[][] batch;
			int count;

			var watch = new Stopwatch();

			watch.Start();

			foreach (string table in ExportQueue.Keys.TakeWhile(table => _Updating && updated < CMOptions.ExportCapacity))
			{
				stack = ExportQueue[table];

				if (stack == null || stack.IsEmpty)
				{
					ExportQueue.TryRemove(table, out stack);
					continue;
				}

				count = Math.Max(0, Math.Min(CMOptions.ExportCapacity - updated, stack.Count));

				if (count == 0)
				{
					continue;
				}

				pop = new QueuedData[count];

				if (stack.TryPopRange(pop) > 0)
				{
					peek = pop.FirstOrDefault(p => p != null && p.RawData != null && p.RawData.Count > 0);

					if (peek == null || !Connection.CreateTable(table, peek.RawData))
					{
						ExportQueue.TryRemove(table, out stack);
						continue;
					}

					batch = pop.AsParallel().Select(qd => qd.SqlData).ToMultiArray();

					if (batch.Length > 0)
					{
						if (CMOptions.ModuleDebug)
						{
							CMOptions.ToConsole("Update {0:#,0} objects in '{1}'...", batch.Length, table);
						}

						    Connection.InsertMany(table, batch);

						updated += batch.Length;
					}
				}

				if (stack.IsEmpty)
				{
					ExportQueue.TryRemove(table, out stack);
				}
			}

			if (transactional)
			{
				VitaNexCore.TryCatch(Connection.EndTransaction, CMOptions.ToConsole);
			}

			watch.Stop();

			CMOptions.ToConsole("Updated {0:#,0} objects in {1:F2} seconds.", updated, watch.Elapsed.TotalSeconds);

			_UpdateCount += updated;

			//GC.Collect();

			_Sync.Set();

			if (!ExportQueue.IsEmpty)
			{
				Flush();
			}
		}

		private static void OnAbort()
		{
			ExportQueue.Clear();

			if (ExportThread != null)
			{
				if (ExportThread.IsAlive || ExportThread.ThreadState == ThreadState.Running)
				{
					ExportThread.Abort();
					ExportThread.Join(3000);
				}

				ExportThread = null;
			}

			if (_Connection != null && _Connection.Connected)
			{
				_Connection.Close();
				_Connection = null;
			}

			CMOptions.ToConsole("Update aborted.");

			_Updating = false;
		}

		public static void Config(PlayerMobile user)
		{
			if (user != null && !user.Deleted && user.AccessLevel >= Access)
			{
				user.SendGump(new PropertiesGump(user, CMOptions));
			}
		}

		public sealed class QueuedData
		{
			public IDictionary<string, SimpleType> RawData { get; set; }
			public MySQLData[] SqlData { get; set; }
		}
	}
}