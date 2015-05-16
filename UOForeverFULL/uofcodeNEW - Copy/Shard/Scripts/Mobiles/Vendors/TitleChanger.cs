using System.Collections.Generic;
using Server.Scripts.Engines.ValorSystem;

namespace Server.Mobiles
{
    public class TitleChanger : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public TitleChanger()
            : base("the title changer")
        {
        }

        public override void InitSBInfo()
        {
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            var from = e.Mobile;

            if (!e.Handled && from.InRange(this, 5) && e.Speech.ToLower() == "vendor buy")
            {
                if (from is PlayerMobile)
                    from.SendGump(new ValorBoardGumpChangeTitle(from, 0));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile)
                from.SendGump(new ValorBoardGumpChangeTitle(from, 0));
        }

        public TitleChanger(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

    }
}