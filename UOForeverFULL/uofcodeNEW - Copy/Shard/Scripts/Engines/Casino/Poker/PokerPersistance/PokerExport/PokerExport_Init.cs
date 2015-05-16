#region References
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Server.Mobiles;
using VitaNex;
using VitaNex.Modules.UOFLegends;

#endregion

namespace Server.Poker
{
	[CoreModule("PokerExport", "1.0.0", false, TaskPriority.High)]
	public static partial class PokerExport
	{
        static PokerExport()
		{
			CMOptions = new PokerOptions();

			ObjectStateTypes = typeof(LegendState).GetConstructableChildren();

			ObjectStates = new LegendState[ObjectStateTypes.Length];

			ExportQueue = new ConcurrentDictionary<string, ConcurrentStack<QueuedData>>();

			UpdateTimes = new Queue<TimeSpan>(10);
		}

		private static void CMConfig()
		{
			ObjectStates.SetAll(i => ObjectStates[i] = ObjectStateTypes[i].CreateInstanceSafe<LegendState>());

			Array.Sort(
				ObjectStates,
				(l, r) =>
				{
					int result = 0;

					if (l.CompareNull(r, ref result))
					{
						return result;
					}

					return l.SupportedType.CompareTo(r.SupportedType);
				});
		}

		private static void CMInvoke()
		{
            LastUpdate = DateTime.Now;

		    UpdateTimer = PollTimer.FromMinutes(
		        1.0,
		        () =>
		        {
		            Update();

		            if (CMOptions.UpdateInterval < UpdateTimeMax)
		            {
		                //CMOptions.UpdateInterval = UpdateTimeMax;
		            }
		        },
		        () =>
		            CMOptions.ModuleEnabled && CMOptions.UpdateTimer && !_Updating &&
		            (DateTime.Now - LastUpdate) > CMOptions.UpdateInterval);
		}

		private static void CMEnabled()
		{
			UpdateTimer.Start();
		}

		private static void CMDisabled()
		{
			UpdateTimer.Stop();
		}

		private static void CMSave()
		{ }

		private static void CMLoad()
		{ }
	}
}