#region References

using System.Linq;
using System.Net;
using Server.Accounting;
using Server.Network;
using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

#endregion

namespace Server.Scripts.New.Adam.NewGuild
{
    public abstract class NewPlayerGuildAutoJoin
    {
        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        private static void OnLogin(LoginEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;
            if (pm == null || !pm.IsYoung() || pm.IsYoung() && pm.Guild != null)
            {
                return;
            }

            if (NewGuildPersistence.Instance == null)
            {
                var NGP = new NewGuildPersistence();
            }

            if (NewGuildPersistence.Instance != null && NewGuildPersistence.JoinedIPs != null)
            {
                var loginIPs = ((Account) e.Mobile.Account).LoginIPs.ToArray();
                if (loginIPs.Any(ip => NewGuildPersistence.JoinedIPs.Contains(ip)))
                {
                    return;
                }
            }

            var g = (Guild) BaseGuild.FindByAbbrev("New");
            if (g != null)
            {
                e.Mobile.SendGump(new NewPlayerGuildJoinGump(g, e.Mobile));
            }
        }
    }
}