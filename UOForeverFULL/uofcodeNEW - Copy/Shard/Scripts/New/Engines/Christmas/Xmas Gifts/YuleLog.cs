using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    [Flipable(0x1BDD, 0x1BE0)]
    public class YuleLog : BaseHolidayGift
    {
        public override bool IsAccessibleTo(Mobile m)
        {
            if (!BaseHouse.CheckAccessible(m, this))
                return true;

            return base.IsAccessibleTo(m);
        }

        private Item m_Fire;

        public void TurnOff()
        {
            if (m_Fire != null)
            {
                m_Fire.Delete();
                m_Fire = null;
            }
        }

        public void TurnOn()
        {
            if (m_Fire == null)
                m_Fire = new Item();

            m_Fire.ItemID = 0x19AB;
            m_Fire.Movable = false;
            m_Fire.MoveToWorld(new Point3D(X, Y, Z + ItemData.Height + 3), Map);
        }

        [Constructable]
        public YuleLog(int year)
        {
            ItemID = 0x1BDD;

            HolidayName = "Christmas";
            HolidayYear = year;

            Name = "a yule log";

            Stackable = false;
            Hue = 1140;

            LootType = LootType.Blessed;
        }

        public YuleLog(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((Item)m_Fire);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1: m_Fire = reader.ReadItem();
                    goto case 0;

                case 0: break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsCoOwner(from))
                {
                    if (m_Fire != null)
                        TurnOff();
                    else
                    {
                        TurnOn();
                        Effects.PlaySound(from, from.Map, 0x345);
                    }
                }
                else
                    from.SendLocalizedMessage(502436); // That is not accessible.
            }
            else
                from.SendLocalizedMessage(502692); // This must be in a house and be locked down to work.
        }

        public override void OnLocationChange(Point3D old)
        {
            if (m_Fire != null)
                m_Fire.MoveToWorld(new Point3D(X, Y, Z + ItemData.Height + 3), Map);
        }
    }
}
