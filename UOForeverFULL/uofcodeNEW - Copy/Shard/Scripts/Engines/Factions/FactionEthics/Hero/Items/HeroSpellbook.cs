using Server.Engines.Ethics;
using Server.Ethics;
using Server.Ethics.Hero;
using Server.Mobiles;

namespace Server.Items
{
    public class Herospellbook : Item, IEthicsItem
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
        public Herospellbook()
            : base(0x2252)
        {
            Name = "Holy Tome";
            Layer = Layer.OneHanded;
            EthicsItem.Imbue(this, Ethic.Hero, Ethic.Hero.Definition.PrimaryHue);
        }

        public Herospellbook(Serial serial)
            : base(serial)
        { }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            if (from is PlayerMobile && Player.Find(from) != null && Player.Find(from).Ethic == Ethic.Hero)
            {
                new EthicSpellbookUI(from as PlayerMobile, null, new HeroEthic()).Send();
            }
            else
            {
                from.SendMessage(54, "Only those following the path of good may use this spellbook!");
                Ethic.DestoryItem(from, this);
            }
        }

        public override bool OnEquip(Mobile from)
        {
            if (from is PlayerMobile && Player.Find(from) != null && Player.Find(from).Ethic != Ethic.Hero)
            {
                from.SendMessage(54, "Only those following the path of good may use this spellbook!");
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
                LabelTo(from, 1153, 1042518);
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
