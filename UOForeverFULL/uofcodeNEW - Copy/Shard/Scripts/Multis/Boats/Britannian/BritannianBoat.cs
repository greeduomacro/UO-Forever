using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class BritannianBoat : BaseBoat
    {
        public override int NorthID { get { return Hits > 50 ? 0x40 : 0x44; } }
        public override int EastID { get { return Hits > 50 ? 0x41 : 0x45; } }
        public override int SouthID { get { return Hits > 50 ? 0x42 : 0x46; } }
        public override int WestID { get { return Hits > 50 ? 0x43 : 0x47; } }

        public override int HoldDistance { get { return 5; } }
        public override int TillerManDistance { get { return -5; } }

        public override Point2D StarboardOffset { get { return new Point2D(2, -1); } }
        public override Point2D PortOffset { get { return new Point2D(-2, -1); } }

        public override Point3D MarkOffset { get { return new Point3D(0, 0, 3); } }

        public override BaseDockedBoat DockedBoat { get { return new BritannianDockedBoat(this); } }

        [Constructable]
        public BritannianBoat()
        {
        }

        public BritannianBoat(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class BritannianBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1041209; } } // large ship deed
        public override BaseBoat Boat { get { return new BritannianBoat(); } }

        [Constructable]
        public BritannianBoatDeed()
            : base(0x40, new Point3D(0, -1, 0))
        {
        }

        public BritannianBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }

    public class BritannianDockedBoat : BaseDockedBoat
    {
        public override BaseBoat Boat { get { return new BritannianBoat(); } }

        public BritannianDockedBoat(BaseBoat boat)
            : base(0x40, new Point3D(0, -1, 0), boat)
        {
        }

        public BritannianDockedBoat(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }
    }
}