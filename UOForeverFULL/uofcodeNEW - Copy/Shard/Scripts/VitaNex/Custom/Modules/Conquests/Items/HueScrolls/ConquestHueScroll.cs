#region References
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Engines.CustomTitles
{
	public class ConquestHueScroll : HueScroll
	{
        [Constructable]
        public ConquestHueScroll(int hue)
            : base(Resolve(hue))
        {
            Name = "a title hue scroll";
            Weight = 2;
            Hue = 1195;
        }

        public ConquestHueScroll(Serial serial)
			: base(serial)
		{ }

        public override void OnDoubleClick(Mobile m)
        {
            var pm = (PlayerMobile)m;

            if (TitleHue == null)
            {
                pm.SendMessage(0x22, "THIS HUE DOES NOT EXIST, PLEASE PAGE IN.");
                Delete();
                return;
            }

            TitleProfile p = CustomTitles.EnsureProfile(pm);

            if (p == null)
            {
                return;
            }

            if (p.Contains(TitleHue))
            {
                pm.SendMessage(0x22, "YOU ALREADY OWN THIS HUE.  THIS SHOULDN'T HAPPEN.  PLEASE PAGE IN.");
                return;
            }

            GrantHue(pm, p);
        }

        private void GrantHue(PlayerMobile pm, TitleProfile p)
        {
            if (TitleHue == null || pm == null || p == null)
            {
                return;
            }

            if (p.Contains(TitleHue))
            {
                pm.SendMessage(0x22, "YOU ALREADY OWN THIS HUE.  THIS SHOULDN'T HAPPEN.  PLEASE PAGE IN.");
                return;
            }

            p.Add(TitleHue);

            pm.PrivateOverheadMessage(
                MessageType.Label, 1287, true,
                "*You have gained the title hue: " + TitleHue.Hue + "*", pm.NetState);

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