#region References

using Server.Guilds;
using Server.Network;
using VitaNex.Items;

#endregion

namespace Server.Gumps
{
    public class NewPlayerGuildHelpGump : Gump
    {
        public virtual FireworkStars StarsEffect { get { return FireworkStars.Willow; } }
        public virtual int Sound { get { return 776; } }
        public virtual int Radius { get { return 10; } }

        public virtual int[] Stars
        {
            get { return new[] {14170, 14155, 14138, 10980, 10296, 10297, 10298, 10299, 10300, 10301}; }
        }

        public virtual int[] Hues { get { return new int[0]; } }
        private readonly Guild _Guild;

        public NewPlayerGuildHelpGump(Guild guild, Mobile m)
            : base(0, 0)
        {
            _Guild = guild;
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(27, 83, 424, 357, 9200);
            AddImage(127, 174, 1418);
            AddImageTiled(30, 85, 21, 349, 10464);
            AddImageTiled(426, 87, 21, 349, 10464);
            AddImage(417, 27, 10441);
            AddImage(205, 44, 9000);
            AddLabel(63, 158, 54, ("Congratulations on joining the new player's guild, " + m.Name));
            AddLabel(55, 185, 2047, "Here are a few things you need to know about your guild:");
            AddLabel(55, 205, 2047, "-The NEW guild is run by " + guild.Leader.Name);
            AddLabel(55, 225, 2047, "-To type in guild chat, type \\ then your message");
            AddLabel(55, 250, 2047, "-To type in alliance chat, type shift+\\ then your message");
            AddLabel(55, 275, 2047, "-To resign from your guild, type I resign from my guild");
            AddButton(55, 340, 247, 248, 1, GumpButtonType.Reply, 0);
            AddImageTiled(63, 176, 380, 1, 5410);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                default:
                {
                    if (_Guild != null)
                    {
                        int hue = Utility.RandomList(0x47E, 0x47F, 0x480, 0x482, 0x66D);
                        int renderMode = Utility.RandomList(0, 2, 3, 4, 5, 7);

                        from.SendMessage(54,
                            "Congratulations on joining the new player's guild!  The leader of your new guild is {0}.",
                            _Guild.Leader.Name);
                        Effects.SendLocationEffect(from.Location, from.Map, 0x373A + (0x10 * Utility.Random(4)), 16, 10,
                            hue, renderMode);
                        StarsEffect.DoStarsEffect(from.Location, from.Map, Radius, Sound, Stars, Hues);
                    }
                    break;
                }
            }
        }
    }
}