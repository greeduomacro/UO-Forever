#region References

using System.Collections.Generic;
using Server.Guilds;
using Server.Targeting;

#endregion

namespace Server.Commands
{
    public class ListenToAlliance
    {
        private static Dictionary<Mobile, List<Guild>> _table = new Dictionary<Mobile, List<Guild>>();

        public static Dictionary<Mobile, List<Guild>> Table { get { return _table; } }

        [Command("ListenToAlliance", AccessLevel.GameMaster)]
        public static void ListenToAlliance_OnCommand(CommandEventArgs args)
        {
            args.Mobile.SendMessage("Target an aligned guild member.");
            args.Mobile.BeginTarget(-1, false, TargetFlags.None, delegate(Mobile from, object obj)
            {
                if (obj is Mobile)
                {
                    var m = obj as Mobile;

                    if (m.Guild != null)
                    {
                        var g = m.Guild as Guild;

                        if (g != null && g.Alliance != null)
                        {
                            List<Guild> value;
                            if (_table.TryGetValue(@from, out value) && value.Contains(g))
                            {
                                value.Remove(g);

                                if (value.Count < 1)
                                {
                                    _table.Remove(from);
                                }

                                from.SendMessage("You are no longer listening to that alliance\'s private chat.");
                            }
                            else
                            {
                                if (_table.ContainsKey(from))
                                {
                                    value.Add(g);
                                }
                                else
                                {
                                    var list = new List<Guild> {g};

                                    _table.Add(from, list);
                                }

                                from.SendMessage("You are now listening to that alliance\'s private chat.");
                            }
                        }
                        else
                        {
                            from.SendMessage("Their guild is not part of an alliance.");
                        }
                    }
                    else
                    {
                        from.SendMessage("They are not in a guild.");
                    }
                }
            });
        }

        [Command("ClearAllianceListeners", AccessLevel.GameMaster)]
        public static void ClearAllianceListeners_OnCommand(CommandEventArgs args)
        {
            if (_table.ContainsKey(args.Mobile))
            {
                _table.Remove(args.Mobile);
            }

            args.Mobile.SendMessage("You are no longer listening to any private alliance chat.");
        }
    }
}