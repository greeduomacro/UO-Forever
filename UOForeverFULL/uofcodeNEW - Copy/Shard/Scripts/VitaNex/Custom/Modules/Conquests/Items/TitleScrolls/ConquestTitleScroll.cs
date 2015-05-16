#region References

using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Engines.CustomTitles
{
    public class ConquestTitleScroll : TitleScroll
    {
        public ConquestTitleScroll(Serial serial)
            : base(serial)
        {}

        [Constructable]
        public ConquestTitleScroll(string title)
            : base(Resolve(title))
        {
            Name = "a title scroll";
            Weight = 2;
            Hue = 1195;
        }

        public override void OnDoubleClick(Mobile m)
        {
            var pm = (PlayerMobile) m;

            if (Title == null)
            {
                pm.SendMessage(0x22, "THIS TITLE DOES NOT EXIST, PLEASE PAGE IN.");
                Delete();
                return;
            }

            TitleProfile p = CustomTitles.EnsureProfile(pm);

            if (p == null)
            {
                return;
            }

            if (p.Contains(Title))
            {
                pm.SendMessage(0x22, "YOU ALREADY OWN THIS TITLE.  THIS SHOULDN'T HAPPEN.  PLEASE PAGE IN.");
                return;
            }

            GrantTitle(pm, p);
        }

        private void GrantTitle(PlayerMobile pm, TitleProfile p)
        {
            if (Title == null || pm == null || p == null)
            {
                return;
            }

            if (p.Contains(Title))
            {
                pm.SendMessage(0x22, "YOU ALREADY OWN THIS TITLE.  THIS SHOULDN'T HAPPEN.  PLEASE PAGE IN.");
                return;
            }

            p.Add(Title);

            pm.PrivateOverheadMessage(
                MessageType.Label, 1287, true,
                "*You have gained the title: " + (!pm.Female ? Title.MaleTitle : Title.FemaleTitle) + "*", pm.NetState);

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();
        }
    }
}