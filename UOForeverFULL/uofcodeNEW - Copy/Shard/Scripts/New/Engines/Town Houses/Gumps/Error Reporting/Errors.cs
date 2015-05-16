#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Network;

#endregion

namespace Knives.TownHouses
{
    public class Errors
    {
        private static readonly List<string> s_ErrorLog = new List<string>();
        private static readonly List<Mobile> s_Checked = new List<Mobile>();

        public static List<string> ErrorLog
        {
            get { return s_ErrorLog; }
        }

        public static List<Mobile> Checked
        {
            get { return s_Checked; }
        }

        public static void Initialize()
        {
            RUOVersion.AddCommand("TownHouseErrors", AccessLevel.Counselor, OnErrors);
            RUOVersion.AddCommand("the", AccessLevel.Counselor, OnErrors);

            EventSink.Login += OnLogin;
        }

        private static void OnErrors(CommandInfo e)
        {
            if (string.IsNullOrEmpty(e.ArgString))
            {
                new ErrorsGump(e.Mobile);
            }
            else
            {
                Report(e.ArgString + " - " + e.Mobile.Name);
            }
        }

        private static void OnLogin(LoginEventArgs e)
        {
            if (e.Mobile.AccessLevel != AccessLevel.Player
                && s_ErrorLog.Count != 0
                && !s_Checked.Contains(e.Mobile))
            {
                new ErrorsNotifyGump(e.Mobile);
            }
        }

        public static void Report(string error)
        {
            s_ErrorLog.Add(String.Format("<B>{0}</B><BR>{1}<BR>", DateTime.Now, error));

            s_Checked.Clear();

            Notify();
        }

        private static void Notify()
        {
            foreach (
                NetState state in
                    NetState.Instances.Where(state => state.Mobile != null)
                        .Where(state => state.Mobile.AccessLevel != AccessLevel.Player))
            {
                Notify(state.Mobile);
            }
        }

        private static void Notify(Mobile m)
        {
            if (m.HasGump(typeof (ErrorsGump)))
            {
                new ErrorsGump(m);
            }
            else
            {
                new ErrorsNotifyGump(m);
            }
        }
    }
}