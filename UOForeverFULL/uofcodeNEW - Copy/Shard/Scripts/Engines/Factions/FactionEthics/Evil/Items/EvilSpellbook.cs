using Server.Engines.Ethics;
using Server.Ethics;
using Server.Ethics.Evil;
using Server.Mobiles;

namespace Server.Items
{
    public class Evilspellbook : Item, IEthicsItem
    {
        private EthicsItem m_EthicState;

        public EthicsItem EthicsItemState { get { return m_EthicState; } set { m_EthicState = value; } }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            DeathMoveResult result = base.OnParentDeath(parent);

            Ethic parentState = Ethic.Find(parent);

            if (parentState != null && result == DeathMoveResult.MoveToCorpse && m_EthicState != null && parentState == m_EthicState.Ethic)
            {
                return DeathMoveResult.MoveToBackpack;
            }
            else
            {
                return result;
            }
        }

        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            DeathMoveResult result = base.OnParentDeath(parent);
            Ethic parentState = Ethic.Find(parent);

            if (parentState != null && result == DeathMoveResult.MoveToCorpse && m_EthicState != null && parentState == m_EthicState.Ethic)
            {
                return DeathMoveResult.MoveToBackpack;
            }
            else
            {
                return result;
            }
        }

        [Constructable]
        public Evilspellbook()
            : base(0x2253)
        {
            Name = "Unholy Tome";
            Layer = Layer.OneHanded;
            EthicsItem.Imbue(this, Ethic.Evil, Ethic.Evil.Definition.PrimaryHue);
        }

        public Evilspellbook(Serial serial)
            : base(serial)
        { }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            if (from is PlayerMobile && Player.Find(from) != null && Player.Find(from).Ethic == Ethic.Evil)
            {
                new EthicSpellbookUI(from as PlayerMobile, null, new EvilEthic()).Send();
            }
            else
            {
                from.SendMessage(54, "Only those following the path of evil may use this spellbook!");
                Ethic.DestoryItem(from, this);
            }
        }

        public override bool OnEquip(Mobile from)
        {
            if (from is PlayerMobile && Player.Find(from) != null && Player.Find(from).Ethic != Ethic.Evil)
            {
                from.SendMessage(54, "Only those following the path of evil may use this spellbook!");
                Ethic.DestoryItem(from, this);
                return false;
            }
            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_EthicState != null)
            {
                LabelTo(from, 1153, 1042519);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            Layer = Layer.OneHanded;
        }
    }
}
