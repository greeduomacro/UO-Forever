#region References

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Server;
using Server.Accounting;
using Server.Mobiles;

#endregion

namespace VitaNex.Modules.UOFLegends
{
    [CoreModule("UOF Legends", "1.0.0", false, TaskPriority.High)]
    public static partial class UOFLegends
    {
        static UOFLegends()
        {
            CMOptions = new UOFLegendsOptions();

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

            if (CMOptions.ModuleDebug)
            {
                foreach (LegendState o in ObjectStates)
                {
                    CMOptions.ToConsole("'{0}' handles '{1}'", o.GetType().Name, o.SupportedType.Name);
                }
            }

            CommandUtility.Register("UOFLConfig", Access, e => Config(e.Mobile as PlayerMobile));

            CommandUtility.Register(
                "UOFLUpdate",
                Access,
                e =>
                {
                    if (_Updating)
                    {
                        e.Mobile.SendMessage(0x22, "Legends is currently updating the database, please wait.");
                        return;
                    }

                    Update();

                    e.Mobile.SendMessage(0x55, "Legends update started...");
                });

            CommandUtility.Register(
                "UOFLCancel",
                Access,
                e =>
                {
                    if (!_Updating)
                    {
                        e.Mobile.SendMessage(0x22, "Legends is not updating and can't be cancelled.");
                        return;
                    }

                    Cancel();

                    e.Mobile.SendMessage(0x55, "Legends update cancelled.");
                });

            CommandUtility.Register(
                "UOFLForceTotalUpdate",
                Access,
                e =>
                {
                    if (_Updating)
                    {
                        e.Mobile.SendMessage(0x22, "Legends is already updating.");
                        return;
                    }

                    Update();

                    _TotalUpdate = true;

                    e.Mobile.SendMessage(0x55, "Legends update started...");
                });

            CommandUtility.Register(
                "UnbanAll",
                Access,
                e =>
                {
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

            EventSink.MobileInvalidate += EventSink_InvalidateMobile;
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
        {}

        private static void CMLoad()
        {}
    }
}