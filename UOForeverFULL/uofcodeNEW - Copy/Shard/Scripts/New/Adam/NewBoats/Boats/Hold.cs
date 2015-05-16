using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class NewHold : BaseShipContainer, IFacingChange
    {
        public override bool IsDecoContainer { get { return false; } }

        public NewHold(NewBaseBoat boat, Point3D initOffset)
            : base(boat, 0x3EAE, initOffset)
        {
        }

        public NewHold(Serial serial)
            : base(serial)
        {
        }

        public void SetFacing(Direction OldFacing, Direction newFacing)
        {
            switch (newFacing)
            {
                case Direction.East: SetItemIDOnSmooth(0x3E65); break;
                case Direction.West: SetItemIDOnSmooth(0x3E93); break;
                case Direction.North: SetItemIDOnSmooth(0x3EAE); break;
                case Direction.South: SetItemIDOnSmooth(0x3EB9); break;
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (Transport == null || !Transport.IsOnBoard(from) || Transport.IsMoving)
                return false;

            return base.OnDragDrop(from, item);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (Transport == null || !Transport.IsOnBoard(from) || Transport.IsMoving)
                return false;

            return base.OnDragDropInto(from, item, p);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this && (Transport == null || !Transport.IsOnBoard(from) || Transport.IsMoving))
                return false;

            return base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (Transport == null || !Transport.IsOnBoard(from) || Transport.IsMoving)
                return false;

            return base.CheckLift(from, item, ref reject);
        }

        public override void OnDoubleClick(Mobile from)
        {
            NewBaseBoat ship = Transport as NewBaseBoat;
            if (ship == null || !ship.IsOnBoard(from))
            {
                if (ship.TillerManItem != null)
                    ship.TillerManItem.Say(502490); // You must be on the ship to open the hold.
                if (ship.TillerManMobile != null)
                    ship.TillerManMobile.TillerManSay(502490); // You must be on the ship to open the hold.
            }
            else if (ship.IsMovingShip)
            {
                if (ship.TillerManItem != null)
                    ship.TillerManItem.Say(502491); // I can not open the hold while the ship is moving.
                if (ship.TillerManMobile != null)
                    ship.TillerManMobile.TillerManSay(502491); // I can not open the hold while the ship is moving.
            }
            else
            {
                base.OnDoubleClick(from);
            }
        }
       
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}