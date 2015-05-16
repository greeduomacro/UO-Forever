using System;
using Server;
using Server.Ethics;
using Server.Ethics.Evil;
using Server.Ethics.Hero;
using Server.Mobiles;

namespace Server.Items
{
    public class HeroChargerOfTheFallen : EtherealMount
    {
        [CommandProperty(AccessLevel.GameMaster)]
        private PlayerMobile BoundMobile { get; set; }

        [Constructable]
        public HeroChargerOfTheFallen()
            : base(0x2D9C, 0x3E92)
        {
            Name = "Hero Spirit Charger";
            Hue = 2955;
        }

        public override int EtherealHue { get { return 0; } }

        public HeroChargerOfTheFallen(Serial serial)
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

        public override void OnDoubleClick(Mobile @from)
        {
            Ethic hero = Ethic.Find(from);
            if (!(hero is HeroEthic) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You must follow the path of the hero to use this mount!");
                return;               
            }
            if (BoundMobile != from as PlayerMobile && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You cannot use this item as it is not bound to you!");
                return;
            }

            base.OnDoubleClick(@from);
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

    public class EvilChargerOfTheFallen : EtherealMount
    {
        [CommandProperty(AccessLevel.GameMaster)]
        private PlayerMobile BoundMobile { get; set; }

        [Constructable]
        public EvilChargerOfTheFallen()
            : base(0x2D9C, 0x3E92)
        {
            Name = "Evil Spirit Charger";
            Hue = 1175;
        }

        public override int EtherealHue { get { return 0; } }

        public EvilChargerOfTheFallen(Serial serial)
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

        public override void OnDoubleClick(Mobile @from)
        {
            Ethic evil = Ethic.Find(from);
            if (!(evil is EvilEthic) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You must follow the path of evil to use this mount!");
                return;
            }
            if (BoundMobile != from as PlayerMobile && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You cannot use this item as it is not bound to you!");
                return;
            }

            base.OnDoubleClick(@from);
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

    public class HeroSpiritSteed : EtherealMount
    {
        [CommandProperty(AccessLevel.GameMaster)]
        private PlayerMobile BoundMobile { get; set; }

        [Constructable]
        public HeroSpiritSteed()
            : base(0x2617, 0x3EBB)
        {
            Name = "Hero Spirit Steed";
            Hue = 2955;
        }

        public override int EtherealHue { get { return 0; } }

        public HeroSpiritSteed(Serial serial)
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

        public override void OnDoubleClick(Mobile @from)
        {
            Ethic hero = Ethic.Find(from);
            if (!(hero is HeroEthic) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You must follow the path of the hero to use this mount!");
                return;
            }
            if (BoundMobile != from as PlayerMobile && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You cannot use this item as it is not bound to you!");
                return;
            }

            base.OnDoubleClick(@from);
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

    public class EvilSpiritSteed : EtherealMount
    {
        [CommandProperty(AccessLevel.GameMaster)]
        private PlayerMobile BoundMobile { get; set; }

        [Constructable]
        public EvilSpiritSteed()
            : base(0x2617, 0x3EBB)
        {
            Name = "Evil Spirit Steed";
            Hue = 1175;
        }

        public override int EtherealHue { get { return 0; } }

        public EvilSpiritSteed(Serial serial)
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

        public override void OnDoubleClick(Mobile @from)
        {
            Ethic evil = Ethic.Find(from);
            if (!(evil is EvilEthic) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You must follow the path of evil to use this mount!");
                return;
            }
            if (BoundMobile != from as PlayerMobile && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage(54, "You cannot use this item as it is not bound to you!");
                return;
            }

            base.OnDoubleClick(@from);
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