#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CustomTitles;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Network;
using VitaNex.Commands;

#endregion

namespace Server.Mobiles
{
    [CorpseName("Erasmus' corpse")]
    public class RoyalAlchemist : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public RoyalAlchemist()
            : base("")
        {
            Name = "Erasmus";
            Body = 400;
            Hue = 33777;
            SpecialTitle = "Royal Alchemist";
            TitleHue = 1926;

            Blessed = true;

            CantWalk = true;

            SpeechHue = YellHue = 34;

            VirtualArmor = 36;

            AddItem(new Robe(Utility.RandomMetalHue()));
            AddItem(new Sandals(Utility.RandomMetalHue()));

            HairItemID = 8265;
            FacialHairItemID = 8254;

            XmlScript script = new XmlScript("Quests/CrippledKing/alchemist.us");

            XmlAttach.AttachTo(this, script);

            HairHue = Utility.RandomMetalHue();
            FacialHairHue = Utility.RandomMetalHue();
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBRoyalAlchemist());
        }

        public RoyalAlchemist(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}