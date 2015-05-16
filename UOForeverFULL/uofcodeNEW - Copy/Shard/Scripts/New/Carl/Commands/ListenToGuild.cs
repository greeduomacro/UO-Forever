#region References

using System.Collections.Generic;
using Server.Guilds;
using Server.Targeting;

#endregion

namespace Server.Commands
{
    public class ListenToGuild
    {
        private static Dictionary<Mobile, List<BaseGuild>> _table = new Dictionary<Mobile, List<BaseGuild>>();

        public static Dictionary<Mobile, List<BaseGuild>> Table { get { return _table; } }

        [Command("ListenToGuild", AccessLevel.GameMaster)]
        public static void ListenToGuild_OnCommand(CommandEventArgs args)
        {
            args.Mobile.SendMessage("Target a guild member.");
            args.Mobile.BeginTarget(-1, false, TargetFlags.None, delegate(Mobile from, object obj)
            {
                if (obj is Mobile)
                {
                    var m = obj as Mobile;

                    if (m.Guild == null)
                    {
                        from.SendMessage("They are not in a guild.");
                    }
                    else
                    {
                        List<BaseGuild> value;
                        if (_table.TryGetValue(@from, out value) && value.Contains(m.Guild))
                        {
                            value.Remove(m.Guild);

                            if (value.Count < 1)
                            {
                                _table.Remove(@from);
                            }

                            @from.SendMessage("You are no longer listening to that guild\'s private chat.");
                        }
                        else
                        {
                            if (_table.ContainsKey(@from))
                            {
                                value.Add(m.Guild);
                            }
                            else
                            {
                                var list = new List<BaseGuild> {m.Guild};

                                _table.Add(@from, list);
                            }

                            @from.SendMessage("You are now listening to that guild\'s private chat.");
                        }
                    }
                }
            });
        }

        [Command("ClearGuildListeners", AccessLevel.GameMaster)]
        public static void ClearGuildListeners_OnCommand(CommandEventArgs args)
        {
            if (_table.ContainsKey(args.Mobile))
            {
                _table.Remove(args.Mobile);
            }

            args.Mobile.SendMessage("You are no longer listening to any private guild chat.");
        }
    }
}