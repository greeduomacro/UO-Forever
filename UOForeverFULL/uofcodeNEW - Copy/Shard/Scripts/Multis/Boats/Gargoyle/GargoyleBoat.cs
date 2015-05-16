using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class GargoyleBoat : BaseBoat
    {
        public override int NorthID { get { return (Hits > DamagedHits ? 0x24 : (Hits > CriticalHits ? 0x28 : 0x2C)); } }
        public override int EastID { get { return (Hits > DamagedHits ? 0x25 : (Hits > CriticalHits ? 0x29 : 0x2D)); } }
        public override int SouthID { get { return (Hits > DamagedHits ? 0x26 : (Hits > CriticalHits ? 0x2A : 0x2E)); } }
        public override int WestID { get { return (Hits > DamagedHits ? 0x27 : (Hits > CriticalHits ? 0x2B : 0x2F)); } }

        public override int HoldDistance { get { return -5; } }
        public override int TillerManDistance { get { return -3; } }

        public override Point2D StarboardOffset { get { return new Point2D(2, -1); } }
        public override Point2D PortOffset { get { return new Point2D(-2, -1); } }

        public override Point3D MarkOffset { get { return new Point3D(0, -2, 16); } }

        public override BaseDockedBoat DockedBoat { get { return new GargoyleBoatDocked(this); } }

        [Constructable]
        public GargoyleBoat()
        {
            //BoatComponents.Add(new BoatComponent(this, new Point3D(0, 0, 0), new int[,] { { 34165, 34468, 33561, 33863, }, { 39184, 34733, 19159, 35670, }, { 36274, 35951, 36558, 36868 } }));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, -1, 16), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, -1, 16), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, 3, 16), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, 3, 16), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, -6, 16), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, -6, 16), 0));
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 2, 0), new int[,] { { 34208, 34510, 33604, 33906, }, { 39212, 34812, 19188, 35718, }, { 36322, 36020, 36623, 36923 } })); // steering wheel
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 5, 0), new int[,] { { 33994, 34297, 33387, 33692, }, { 34902, 34598, 19003, 35506, }, { 36084, 35100, 36386, 36688 } })); 
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 5, 0), new int[,] { { 33995, 34298, 33388, 33693, }, { 34903, 34599, 19004, 35507, }, { 36085, 35101, 36387, 36689 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 5, 0), new int[,] { { 34000, 34303, 33393, 33698, }, { 34908, 34604, 19009, 35512, }, { 36090, 35106, 36392, 36694 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 5, 0), new int[,] { { 33996, 34299, 33389, 33694, }, { 34904, 34600, 19005, 35508, }, { 36086, 35102, 36388, 36690 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 5, 0), new int[,] { { 33998, 34301, 33391, 33696, }, { 34906, 34602, 19007, 35510, }, { 36088, 35104, 36390, 0 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 6, 0), new int[,] { { 34001, 34304, 33394, 33699, }, { 34909, 34605, 19010, 35513, }, { 36091, 35107, 36393, 36695 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 6, 0), new int[,] { { 34002, 34305, 33395, 33700, }, { 34910, 34606, 19011, 35514, }, { 36092, 35108, 36394, 36696 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 6, 0), new int[,] { { 34007, 34310, 33400, 33705, }, { 34915, 34611, 19016, 35519, }, { 36097, 35113, 36399, 36701 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 6, 0), new int[,] { { 34003, 34306, 33396, 33701, }, { 34911, 34607, 19012, 35515, }, { 36093, 35109, 36395, 36697 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 6, 0), new int[,] { { 34005, 34308, 33398, 33703, }, { 34913, 34609, 19014, 35517, }, { 36095, 35111, 36397, 36699 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 7, 0), new int[,] { { 34008, 34311, 33401, 33706, }, { 34916, 34612, 19017, 35520, }, { 36098, 35114, 36400, 36702 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 7, 0), new int[,] { { 34009, 34312, 33402, 33707, }, { 34917, 34613, 19018, 35521, }, { 36099, 35115, 36401, 36703 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 7, 0), new int[,] { { 34014, 34317, 33407, 33712, }, { 34922, 34618, 19023, 35526, }, { 36104, 35120, 36406, 36708 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 7, 0), new int[,] { { 34010, 34313, 33403, 33708, }, { 34918, 34614, 19019, 35522, }, { 36100, 35116, 36402, 36704 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 7, 0), new int[,] { { 34012, 34315, 33405, 33710, }, { 34920, 34616, 19021, 35524, }, { 36102, 35118, 36404, 36706 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 8, 0), new int[,] { { 34015, 34318, 33408, 33713, }, { 34923, 34619, 19024, 35527, }, { 36105, 35121, 36407, 36709 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 8, 0), new int[,] { { 34016, 34319, 33409, 33714, }, { 34924, 34620, 19025, 35528, }, { 36106, 35122, 36408, 36710 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 8, 0), new int[,] { { 34021, 34324, 33414, 33719, }, { 34929, 34625, 19030, 35533, }, { 36111, 35127, 36413, 36715 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 8, 0), new int[,] { { 34017, 34320, 33410, 33715, }, { 34925, 34621, 19026, 35529, }, { 36107, 0, 36409, 36711 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 8, 0), new int[,] { { 34019, 34322, 33412, 33717, }, { 34927, 34623, 19028, 35531, }, { 36109, 35125, 36411, 36713 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 1, 0), new int[,] { { 33962, 34265, 33355, 33660, }, { 34870, 34566, 18977, 35474, }, { 36058, 35074, 36360, 36662 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 1, 0), new int[,] { { 33964, 34267, 33357, 33662, }, { 34872, 34568, 18979, 35476, }, { 36060, 35076, 36362, 36664 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, -8, 0), new int[,] { { 34070, 34373, 33463, 33768, }, { 34978, 34674, 19059, 35582, }, { 36140, 35156, 36442, 36744 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, -5, 0), new int[,] { { 34045, 34348, 33438, 33743, }, { 34953, 34649, 19040, 35557, }, { 36121, 35137, 36423, 36725 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, -5, 0), new int[,] { { 34047, 34350, 33440, 33745, }, { 34955, 34651, 19042, 35559, }, { 36123, 35139, 36425, 36727 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, -2, 0), new int[,] { { 33929, 34232, 33322, 33627, }, { 34837, 34533, 18956, 35441, }, { 36037, 35053, 36339, 36641 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, -2, 0), new int[,] { { 33934, 34237, 33327, 33632, }, { 34842, 34538, 18958, 35446, }, { 36039, 35055, 36341, 36643 } })); // cannon

            PPlank.Visible = false;
            SPlank.Visible = false;
            Hold.Visible = true;
            TillerMan.Z -= 3;
        }

        public GargoyleBoat(Serial serial)
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

    public class GargoyleBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116739; } }
        public override BaseBoat Boat { get { return new GargoyleBoat(); } }

        [Constructable]
        public GargoyleBoatDeed()
            : base(0x24, new Point3D(0, -1, 0))
        {
        }

        public GargoyleBoatDeed(Serial serial)
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

    public class GargoyleBoatDocked : BaseDockedBoat
    {
        public override BaseBoat Boat { get { return new GargoyleBoat(); } }
        public override int LabelNumber { get { return 1116748; } } 

        public GargoyleBoatDocked(BaseBoat boat)
            : base(0x24, new Point3D(0, -1, 0), boat)
        {
        }

        public GargoyleBoatDocked(Serial serial)
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