#region References

using Server.Ethics;
using Server.Ethics.Hero;
using Server.Guilds;

#endregion

namespace Server.Items
{
    public class OrderShield : BaseShield
    {
        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 125; } }

        public override int ArmorBase { get { return 30; } }

        public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

        [Constructable]
        public OrderShield()
            : base(0x1BC4)
        {
            Weight = 7.0;
        }

        public override bool AllowEquippedCast(Mobile from)
        {
            return false;
        }

        public OrderShield(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            if (Weight == 6.0)
            {
                Weight = 7.0;
            }
        }

        public override bool OnEquip(Mobile from)
        {
            return Validate(from) && base.OnEquip(from);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Validate(Parent as Mobile))
            {
                base.OnSingleClick(from);
            }
            else
            {
                LabelToExpansion(from);
            }
        }

        public virtual bool Validate(Mobile m)
        {
            if (EraAOS || m == null || !m.Player || m.AccessLevel != AccessLevel.Player)
            {
                return true;
            }

            var ethic = Player.Find(m);

            var g = m.Guild as Guild;

            if (g == null && ethic == null || g != null && g.Type != GuildType.Order && ethic == null || ethic != null && !(ethic.Ethic is HeroEthic))
            {
                m.FixedEffect(0x3728, 10, 13);
                Delete();

                return false;
            }

            return true;
        }
    }
}