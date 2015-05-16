using System;
using Server.Items;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public class HolidaySnowflake : BaseHolidayGift
	{
        public override bool IsAccessibleTo(Mobile m)
        {
            if (!BaseHouse.CheckAccessible(m, this))
                return true;

            return base.IsAccessibleTo(m);
        }

        private Item m_Sparkle;

        public void TurnOff()
        {
            if (m_Sparkle != null)
            {
                m_Sparkle.Delete();
                m_Sparkle = null;
            }
        }

        public void TurnOn()
        {
            if (m_Sparkle == null)
                m_Sparkle = new Item();

            m_Sparkle.ItemID = 0x3779;
            m_Sparkle.Hue = this.Hue;
            m_Sparkle.Movable = false;
            m_Sparkle.MoveToWorld(new Point3D(X, Y, Z + ItemData.Height + 1), Map);
        }

        private static int[] m_Hues = new int[]
		{
			0x6,
			0xB,
			0x10,
			0x15,
			0x1A,
			0x1F,
			0x24,
			0x29,
			0x2E,
			0x33,
			0x38,
			0x3D,
			0x42,
			0x47,
			0x4C,
			0x51,
			0x56,
			0x5B,
			0x60,
			0x65,
		};

		[Constructable]
		public HolidaySnowflake(int year)
		{
            ItemID = 0x232F;

            Hue = Utility.RandomList(m_Hues);

            HolidayName = "Christmas";
            HolidayYear = year;

			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public HolidaySnowflake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.Write((Item)m_Sparkle);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            switch (version)
            {
                case 1: m_Sparkle = reader.ReadItem();
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
                    if (m_Sparkle != null)
                        TurnOff();
                    else
                    {
                        TurnOn();
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
            if (m_Sparkle != null)
                m_Sparkle.MoveToWorld(new Point3D(X, Y, Z + ItemData.Height + 1), Map);
        }
	}
}