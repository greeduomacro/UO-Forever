#region References

using Server.Accounting;
using Server.Ethics;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Gumps
{
    public class NewPlayerGuildJoinGump : Gump
    {
        private readonly Guild _Guild;

        public NewPlayerGuildJoinGump(Guild guild, Mobile m)
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
            AddLabel(63, 158, 54, ("Welcome to UO:Forever, " + m.Name));
            AddLabel(55, 185, 2047, "If you would like to join the new player's guild, click the");
            AddLabel(55, 205, 2047, "okay button below. The new player's guild will help you get");
            AddLabel(55, 225, 2047, "started on your journey. They will provide you with valuable");
            AddLabel(55, 250, 2047, "supplies and knowledge that will aid you on your adventures");
            AddLabel(55, 275, 2047, "throughout the world of Sosaria!");
            AddButton(55, 340, 247, 248, 1, GumpButtonType.Reply, 0);
            AddButton(147, 340, 242, 243, 0, GumpButtonType.Reply, 0);
            AddImageTiled(63, 176, 253, 1, 5410);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var from = sender.Mobile as PlayerMobile;

            switch (info.ButtonID)
            {
                case 1:
                {
                    if (from != null && _Guild != null && from.Guild == null)
                    {
                        _Guild.AddMember(from);
                        if (NewGuildPersistence.Instance != null && NewGuildPersistence.JoinedIPs != null)
                            NewGuildPersistence.JoinedIPs.Add(sender.Address);

                        from.SendGump(new NewPlayerGuildHelpGump(_Guild, from));
                    }
                    break;
                }
            }
        }
    }
}