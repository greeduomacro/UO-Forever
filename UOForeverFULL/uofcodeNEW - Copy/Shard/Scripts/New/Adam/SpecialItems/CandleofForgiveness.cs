using System;
using Server.Mobiles;

namespace Server.Items
{
    public class CandleofForgiveness : BaseEquipableLight
    {
        public override int LitItemID { get { return 0xA0F; } }
        public override int UnlitItemID { get { return 0xA28; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile BoundMobile { get; set; }

        [Constructable]
        public CandleofForgiveness()
            : base(0xA28)
        {
            Name = "Candle of Forgiveness";
            Hue = 1266;
            LootType = LootType.Blessed;

            if (Burnout)
                Duration = TimeSpan.FromMinutes(20);
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle150;
            Weight = 1.0;
        }

        public CandleofForgiveness(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDragLift(Mobile m)
        {
            bool allow = base.OnDragLift(m);

            if (allow && BoundMobile == null && m is PlayerMobile && RootParent == m && m.AccessLevel == AccessLevel.Player)
            {
                BoundMobile = m as PlayerMobile;
            }

            if (m as PlayerMobile != BoundMobile && m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendMessage(54, "You cannot pick this item up as it is not bound to you.");
                allow = false;
            }

            return allow;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (BoundMobile == null && from is PlayerMobile && RootParent == from && from.AccessLevel == AccessLevel.Player)
            {
                BoundMobile = from as PlayerMobile;
            }

            if (BoundMobile == from && from is PlayerMobile && RootParent == from)
            {
                base.OnDoubleClick(from);
            }
            else
            {
                from.SendMessage(54, "Only the owner of this item can use it.");
            }
        }

        public override void OnSingleClick(Mobile @from)
        {
            if (BoundMobile == null && from is PlayerMobile && RootParent == from && from.AccessLevel == AccessLevel.Player)
            {
                BoundMobile = from as PlayerMobile;
            }

            base.OnSingleClick(@from);

            if (BoundMobile != null)
            {
                LabelTo(from, "Bound to: " + BoundMobile.RawName, 54);
            }
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            from.SendMessage(54, "This is a soul-bound item and cannot be traded.");
            to.SendMessage(54, "This is a soul-bound item and cannot be traded.");
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(BoundMobile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            BoundMobile = reader.ReadMobile<PlayerMobile>();
        }
    }
}