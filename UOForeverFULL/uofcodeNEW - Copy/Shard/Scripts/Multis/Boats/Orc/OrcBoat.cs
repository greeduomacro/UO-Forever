using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class OrcBoat : BaseBoat
    {
        public override int NorthID { get { return (Hits > DamagedHits ? 0x18 : (Hits > CriticalHits ? 0x1C : 0x20)); } }
        public override int EastID { get { return (Hits > DamagedHits ? 0x19 : (Hits > CriticalHits ? 0x1D : 0x21)); } }
        public override int SouthID { get { return (Hits > DamagedHits ? 0x1A : (Hits > CriticalHits ? 0x1E : 0x22)); } }
        public override int WestID { get { return (Hits > DamagedHits ? 0x1B : (Hits > CriticalHits ? 0x1F : 0x23)); } }

        public override int HoldDistance { get { return -10; } }
        public override int TillerManDistance { get { return -7; } }

        public override Point2D StarboardOffset { get { return new Point2D(2, -1); } }
        public override Point2D PortOffset { get { return new Point2D(-2, -1); } }

        public override Point3D MarkOffset { get { return new Point3D(0, 2, 14); } }

        public override BaseDockedBoat DockedBoat { get { return new OrcBoatDocked(this); } }

        [Constructable]
        public OrcBoat()
        {
            //BoatComponents.Add(new BoatComponent(this, new Point3D(0, 0, 0), new int[,] { { 31168, 31668, 30168, 30668, }, { 32257, 32457, 31857, 32057, }, { 33057, 33257, 32657, 32857 } }));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, 1, 14), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, 1, 14), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, -3, 14), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, -3, 14), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, 5, 14), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, 5, 14), 0));
            
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 7, 0), new int[,] { { 31141, 31642, 30141, 30641, }, { 32290, 32493, 31890, 32090, }, { 33090, 33290, 32692, 32891 } })); // steering wheel
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 7, 0), new int[,] { { 31140, 31641, 30140, 30640, }, { 32291, 32492, 31891, 32091, }, { 33092, 33291, 32691, 32892 } })); // steering wheel side
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 7, 0), new int[,] { { 31142, 31643, 30142, 30642, }, { 32293, 32491, 31892, 32093, }, { 33093, 33293, 32693, 32893 } })); // steering wheel side
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 9, 0), new int[,] { { 31117, 31617, 30117, 30617, }, { 32217, 32417, 31817, 32018, }, { 33017, 33217, 32617, 32817 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 9, 0), new int[,] { { 31115, 31615, 30115, 30615, }, { 32215, 32415, 31815, 32016, }, { 33015, 33215, 32615, 32815 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 9, 0), new int[,] { { 31120, 31620, 30120, 30620, }, { 32220, 32420, 31820, 32021, }, { 33020, 33220, 32620, 32820 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 10, 0), new int[,] { { 31124, 31624, 30124, 30624, }, { 32224, 32424, 31824, 32025, }, { 33024, 33224, 32624, 32824 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 10, 0), new int[,] { { 31122, 31622, 30122, 30622, }, { 32222, 32422, 31822, 32023, }, { 33022, 33222, 32622, 32822 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 10, 0), new int[,] { { 31127, 31627, 30127, 30627, }, { 32227, 32427, 31827, 32028, }, { 33027, 33227, 32627, 32827 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 6, 0), new int[,] { { 31093, 31593, 30093, 30593, }, { 32193, 32393, 31793, 31994, }, { 32993, 33193, 32593, 32793 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 6, 0), new int[,] { { 31097, 31597, 30097, 30597, }, { 32197, 32397, 31797, 31998, }, { 32997, 33197, 32597, 32797 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, -6, 0), new int[,] { { 31012, 31512, 30012, 30512, }, { 32112, 32312, 31712, 31913, }, { 32912, 33112, 32512, 32712 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, -2, 0), new int[,] { { 31037, 31537, 30037, 30537, }, { 32137, 32337, 31737, 31938, }, { 32937, 33137, 32537, 32737 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, -2, 0), new int[,] { { 31041, 31541, 30041, 30541, }, { 32141, 32341, 31741, 31942, }, { 32941, 33141, 32541, 32741 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, -1, 0), new int[,] { { 31044, 31544, 30044, 30544, }, { 32144, 32344, 31744, 31945, }, { 32944, 33144, 32544, 32744 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, -1, 0), new int[,] { { 31048, 31548, 30048, 30548, }, { 32148, 32348, 31748, 31949, }, { 32948, 33148, 32548, 32748 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, -3, 0), new int[,] { { 31030, 31530, 30030, 30530, }, { 32130, 32330, 31730, 31931, }, { 32930, 33130, 32530, 32730 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, -3, 0), new int[,] { { 31034, 31534, 30034, 30534, }, { 32134, 32334, 31734, 31935, }, { 32934, 33134, 32534, 32734 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 5, 0), new int[,] { { 31086, 31586, 30086, 30586, }, { 32186, 32386, 31786, 31987, }, { 32986, 33186, 32586, 32786 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 5, 0), new int[,] { { 31090, 31590, 30090, 30590, }, { 32190, 32390, 31790, 31991, }, { 32990, 33190, 32590, 32790 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 2, 0), new int[,] { { 31065, 31565, 30065, 30565, }, { 32165, 32365, 31765, 31966, }, { 32965, 33165, 32565, 32765 } }));
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 2, 0), new int[,] { { 31069, 31569, 30069, 30569, }, { 32169, 32369, 31769, 31970, }, { 32969, 33169, 32569, 32769 } }));
            
            PPlank.Visible = false;
            SPlank.Visible = false;
            Hold.Visible = true;
            TillerMan.Z -= 3;
        }

        public OrcBoat(Serial serial)
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

    public class OrcBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116738; } } // large ship deed
        public override BaseBoat Boat { get { return new OrcBoat(); } }

        [Constructable]
        public OrcBoatDeed()
            : base(0x18, new Point3D(0, -1, 0))
        {
        }

        public OrcBoatDeed(Serial serial)
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

    public class OrcBoatDocked : BaseDockedBoat
    {
        public override BaseBoat Boat { get { return new OrcBoat(); } }
        public override int LabelNumber { get { return 1116747; } } 

        public OrcBoatDocked(BaseBoat boat)
            : base(0x18, new Point3D(0, -1, 0), boat)
        {
        }

        public OrcBoatDocked(Serial serial)
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