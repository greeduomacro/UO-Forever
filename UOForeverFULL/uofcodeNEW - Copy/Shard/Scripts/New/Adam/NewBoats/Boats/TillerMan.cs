using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class NewTillerMan : BaseShipItem, IFacingChange
    {
        public NewTillerMan(NewBaseBoat boat, Point3D initOffset)
            : base(boat, 0x3E4E, initOffset)
        {
        }

        public NewTillerMan(Serial serial)
            : base(serial)
        {
        }

        public void Say(int number)
        {
            if(!Transport.IsDriven)
                PublicOverheadMessage(MessageType.Regular, 0x3B2, number);
        }

        public void Say(int number, string args)
        {
            if (!Transport.IsDriven)
                PublicOverheadMessage(MessageType.Regular, 0x3B2, number, args);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!Transport.IsDriven)
                Transport.TakeCommand(from);
            else
                Transport.LeaveCommand(from);
        }

        public void SetFacing(Direction oldFacing, Direction newFacing)
        {
            switch (newFacing)
            {
                case Direction.South: SetItemIDOnSmooth(0x3E4B); break;
                case Direction.North: SetItemIDOnSmooth(0x3E4E); break;
                case Direction.West: SetItemIDOnSmooth(0x3E50); break;
                case Direction.East: SetItemIDOnSmooth(0x3E55); break;
            }

            if (oldFacing == Server.Direction.North)
            {
                SetLocationOnSmooth(new Point3D(X - 1, Y, Z));
            }
            else if (newFacing == Server.Direction.North)
            {
                switch (oldFacing)
                {
                    case Server.Direction.South: SetLocationOnSmooth(new Point3D(X - 1, Y, Z)); break;
                    case Server.Direction.East: SetLocationOnSmooth(new Point3D(X, Y + 1, Z)); break;
                    case Server.Direction.West: SetLocationOnSmooth(new Point3D(X, Y - 1, Z)); break;
                }
            }
        }

        #region Serialization
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
        #endregion
    }
}