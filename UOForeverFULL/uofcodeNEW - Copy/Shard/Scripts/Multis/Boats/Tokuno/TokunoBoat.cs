using System;
using Server;
using Server.Items;

namespace Server.Multis
{
    public class TokunoBoat : BaseBoat
    {        
        public override int NorthID { get { return (Hits > DamagedHits ? 0x30 : (Hits > CriticalHits ? 0x34 : 0x38)); } }
        public override int EastID { get { return (Hits > DamagedHits ? 0x31 : (Hits > CriticalHits ? 0x35 : 0x39)); } }
        public override int SouthID { get { return (Hits > DamagedHits ? 0x32 : (Hits > CriticalHits ? 0x36 : 0x3A)); } }
        public override int WestID { get { return (Hits > DamagedHits ? 0x33 : (Hits > CriticalHits ? 0x37 : 0x3B)); } }

        public override int HoldDistance { get { return -5; } }
        public override int TillerManDistance { get { return -5; } }

        public override Point2D StarboardOffset { get { return new Point2D(2, -1); } }
        public override Point2D PortOffset { get { return new Point2D(-2, -1); } }

        public override Point3D MarkOffset { get { return new Point3D(0, 2, 7); } }

        public override BaseDockedBoat DockedBoat { get { return new TokunoBoatDocked(this); } }

        [Constructable]
        public TokunoBoat()
        {
            //BoatComponents.Add(new BoatComponent(this, new Point3D(0, 0, 0), new int[,] { { 37579, 37625, 37487, 37533, }, { 38311, 38357, 38219, 38265, }, { 39017, 38357, 39067, 39117 } }));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, -2, 7), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, -2, 7), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(2, 3, 7), 0));
            BoatComponents.Add(new BoatRope(this, new Point3D(-2, 3, 7), 0));
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 7, 1), new int[,] { { 37654, 37656, 37650, 37652, }, { 38386, 38388, 38382, 39152, }, { 39148, 39145, 39150, 39152 } })); // steering
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 4, 0), new int[,] { { 37239, 37374, 36969, 37104, }, { 37971, 38106, 37701, 37836, }, { 38569, 38434, 38709, 38849 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 4, 0), new int[,] { { 37238, 37373, 36968, 37103, }, { 37970, 38105, 37700, 37835, }, { 38568, 38433, 38708, 38848 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 4, 0), new int[,] { { 37240, 37375, 36970, 37105, }, { 37972, 38107, 37702, 37837, }, { 38570, 38435, 38710, 38850 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, 5, 0), new int[,] { { 37232, 37367, 36962, 37097, }, { 37964, 38099, 37694, 37829, }, { 38562, 38427, 38702, 38842 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(1, 5, 0), new int[,] { { 37231, 37366, 36961, 37096, }, { 37963, 38098, 37693, 37828, }, { 38561, 38426, 38701, 38841 } })); // hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-1, 5, 0), new int[,] { { 37233, 37368, 36963, 37098, }, { 37965, 38100, 37695, 37830, }, { 38563, 38428, 38703, 38843 } })); //hold
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, 2, 0), new int[,] { { 37255, 37390, 36985, 37120, }, { 37987, 38122, 37717, 37852, }, { 38585, 38450, 38725, 38865 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, 2, 0), new int[,] { { 37251, 37386, 36981, 37116, }, { 37983, 38118, 37713, 37848, }, { 38581, 38446, 38721, 38861 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(0, -9, 0), new int[,] { { 37324, 37459, 37054, 37189, }, { 38056, 38191, 37786, 37921, }, { 38654, 38519, 38794, 38934 } })); // cannon
            BoatComponents.Add(new BoatComponent(this, new Point3D(-2, -3, 0), new int[,] { { 37290, 37425, 37020, 37155, }, { 38022, 38157, 37752, 37887, }, { 38620, 38485, 38760, 38900 } })); // cannon 
            BoatComponents.Add(new BoatComponent(this, new Point3D(2, -3, 0), new int[,] { { 37286, 37421, 37016, 37151, }, { 38018, 38153, 37748, 37883, }, { 38616, 38481, 38756, 38896 } })); // cannon


            PPlank.Visible = false;
            SPlank.Visible = false;
            Hold.Visible = true;
            Hold.Z -= 1;
            TillerMan.Z -= 6;
        }

        public TokunoBoat(Serial serial)
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

    public class TokunoBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116740; } } // large ship deed
        public override BaseBoat Boat { get { return new TokunoBoat(); } }

        [Constructable]
        public TokunoBoatDeed()
            : base(0x30, new Point3D(0, -1, 0))
        {
        }

        public TokunoBoatDeed(Serial serial)
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

    public class TokunoBoatDocked : BaseDockedBoat
    {
        public override BaseBoat Boat { get { return new TokunoBoat(); } }
        public override int LabelNumber { get { return 1116749; } } 

        public TokunoBoatDocked(BaseBoat boat)
            : base(0x30, new Point3D(0, -1, 0), boat)
        {
        }

        public TokunoBoatDocked(Serial serial)
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