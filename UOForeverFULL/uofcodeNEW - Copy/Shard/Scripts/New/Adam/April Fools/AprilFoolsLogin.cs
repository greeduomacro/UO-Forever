using System.Collections.Generic;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;


namespace Server.Scripts.New.Adam.NewGuild
{
    public abstract class AprilFoolsLogin
    {
        public static List<PlayerMobile> UsedGump;

        public static void Initialize()
        {
            //EventSink.Login += OnLogin;
            UsedGump = new List<PlayerMobile>();
        }

        private static void OnLogin(LoginEventArgs e)
        {
            if (e != null && e.Mobile != null && e.Mobile is PlayerMobile && UsedGump != null && !UsedGump.Contains(e.Mobile as PlayerMobile))
            {
                e.Mobile.SendGump(new AprilFoolsGump1());
            }
        }
    }
}
