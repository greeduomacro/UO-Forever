using System;
using Server;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

    public enum ShipCannonDirection : byte
    {
        Port = 0,
        Fore = 1,
        Starboard = 2,
        Aft = 3
    }

    [Flipable(0x14F0, 0x14EF)]
    public abstract class ShipCannonDeed : Item
    {
        public virtual ShipCannon Cannon(BaseBoat boat, Point3D offset, ShipCannonDirection dir) { throw new NotImplementedException(); }

        [Constructable]
        public ShipCannonDeed()
            : base(0x14F0)
        {
            Weight = 1.0;
        }

        public ShipCannonDeed(Serial serial)
            : base(serial)
        {
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

            if (Weight == 0.0)
                Weight = 1.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        private class InternalTarget : Target
        {
            private ShipCannonDeed m_Deed;

            public InternalTarget(ShipCannonDeed deed)
                : base(-1, true, TargetFlags.None)
            {
                m_Deed = deed;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || m_Deed.Deleted)
                    return;

                if (m_Deed.IsChildOf(from.Backpack))
                {
                    if (targeted is BoatComponent)
                    {
                        BoatComponent spot = (BoatComponent)targeted;
                        if (spot.Boat == null || spot.Boat.Deleted || spot.Deleted || spot.ComponentType != BoatComponentType.CannonSpot || !spot.Boat.Contains(from))
                        {
                            from.SendLocalizedMessage(500269); // You cannot build that there.
                            return;
                        }

                        ShipCannonDirection cannonDir = ShipCannonDirection.Fore;
                        int baseItemID = spot.DirectionalItemIDs[0, 0];
                        foreach (int cannonID in BoatComponent.ForeCannonItemIDs) { if (baseItemID == cannonID) { cannonDir = ShipCannonDirection.Fore; } }
                        foreach (int cannonID in BoatComponent.PortCannonItemIDs) { if (baseItemID == cannonID) { cannonDir = ShipCannonDirection.Port; } }
                        foreach (int cannonID in BoatComponent.StarboardCannonItemIDs) { if (baseItemID == cannonID) { cannonDir = ShipCannonDirection.Starboard; } }
                        foreach (int cannonID in BoatComponent.AftCannonItemIDs) { if (baseItemID == cannonID) { cannonDir = ShipCannonDirection.Aft; } }
                       
                        Point3D offset = spot.Offset;
                        offset.Z += spot.Boat.MarkOffset.Z; // offset from the ground up
                        ShipCannon addon = m_Deed.Cannon(spot.Boat, offset, cannonDir);
                        addon.MoveToWorld(Point3D.Zero, Map.Felucca); // get it out of the way (it blocks itself ironically)
                        Server.Spells.SpellHelper.GetSurfaceTop(ref p);

                        if (!map.CanFit(p.X, p.Y, p.Z, addon.ItemData.Height, false, true, (addon.Z == 0)))
                        {
                            from.SendLocalizedMessage(500269); // You cannot build that there.
                            addon.Delete();
                        }
                        else
                        {
                            addon.MoveToWorld(new Point3D(p), map);

                            m_Deed.Delete();
                            from.SendMessage("You place the cannon.");
                        }
                    }
                    else
                    {
                        from.SendMessage(38, "You can only place cannons at appropriate cannon locations on a boat.");
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }
        }
    }

    public class ShipCannon : BoatComponent, IChopable
    {        
        public virtual string UberScriptFileName { get { return null; } }
        public virtual ShipCannonDeed Deed { get { return null; } }
        
        private ShipCannonDirection m_CannonDirection = ShipCannonDirection.Fore;
        [CommandProperty(AccessLevel.GameMaster)]
        public ShipCannonDirection CannonDirection { get { return m_CannonDirection; } set { m_CannonDirection = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point2D CannonDirectionVector
        {
            get
            {
                if (Boat == null) return new Point2D(0, -1); // shouldn't ever happen
                switch (Boat.Facing)
                {
                    case Direction.North:
                        {
                            if (CannonDirection == ShipCannonDirection.Fore) { return new Point2D(0, -1); }
                            else if (CannonDirection == ShipCannonDirection.Starboard) { return new Point2D(1, 0); }
                            else if (CannonDirection == ShipCannonDirection.Aft) { return new Point2D(0, 1); }
                            else if (CannonDirection == ShipCannonDirection.Port) { return new Point2D(-1, 0); }
                            break;
                        }
                    case Direction.East:
                        {
                            if (CannonDirection == ShipCannonDirection.Fore) { return new Point2D(1, 0); }
                            else if (CannonDirection == ShipCannonDirection.Starboard) { return new Point2D(0, 1); }
                            else if (CannonDirection == ShipCannonDirection.Aft) { return new Point2D(-1, 0); }
                            else if (CannonDirection == ShipCannonDirection.Port) { return new Point2D(0, -1); }
                            break;
                        }
                    case Direction.South:
                        {
                            if (CannonDirection == ShipCannonDirection.Fore) { return new Point2D(0, 1); }
                            else if (CannonDirection == ShipCannonDirection.Starboard) { return new Point2D(-1, 0); }
                            else if (CannonDirection == ShipCannonDirection.Aft) { return new Point2D(0, -1); }
                            else if (CannonDirection == ShipCannonDirection.Port) { return new Point2D(1, 0); }
                            break;
                        }
                    case Direction.West:
                        {
                            if (CannonDirection == ShipCannonDirection.Fore) { return new Point2D(-1, 0); }
                            else if (CannonDirection == ShipCannonDirection.Starboard) { return new Point2D(0, -1); }
                            else if (CannonDirection == ShipCannonDirection.Aft) { return new Point2D(1, 0); }
                            else if (CannonDirection == ShipCannonDirection.Port) { return new Point2D(0, 1); }
                            break;
                        }
                }
                return new Point2D(0, -1);
            }
        }

        public DateTime CanBeChoppedDateTime = DateTime.MinValue;
        
        public ShipCannon(BaseBoat boat, Point3D offset, ShipCannonDirection dir) : base(boat, offset, null) 
        {
            m_CannonDirection = dir;
            SetFacing(boat.Facing);
            if (UberScriptFileName != null)
            {
                XmlScript script = new XmlScript(UberScriptFileName);
                script.Name = "cannon";
                XmlAttach.AttachTo(this, script);
            }
            boat.BoatComponents.Add(this);
            CanBeChoppedDateTime = DateTime.UtcNow + TimeSpan.FromMinutes(1.0);
        }

        public ShipCannon(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((byte)m_CannonDirection);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_CannonDirection = (ShipCannonDirection)reader.ReadByte();
        }

        public virtual void OnChop(Mobile from)
        {
            if (CanBeChoppedDateTime > DateTime.UtcNow)
            {
                from.SendMessage(38, "You cannot move the cannon so soon after it has been placed!");
                return;
            }

            if (Boat != null && Boat.CanCommand(from) && Boat.BoatComponents.Contains(this))
            {
                if (!Boat.Contains(from))
                {
                    from.SendMessage(38, "You must be on the boat to chop the cannon!");
                    return;
                }

                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                /*
                int hue = 0;

                if (RetainDeedHue)
                {
                    for (int i = 0; hue == 0 && i < m_Components.Count; ++i)
                    {
                        AddonComponent c = m_Components[i];

                        if (c.Hue != 0)
                            hue = c.Hue;
                    }
                }*/

                Delete();

                Boat.BoatComponents.Remove(this);

                ShipCannonDeed deed = Deed;

                if (deed != null)
                {
                    //if (RetainDeedHue)
                    //    deed.Hue = hue;

                    from.AddToBackpack(deed);
                }
            }
            else
            {
                from.SendMessage(38, "You must be able to control the boat to chop the cannon!");
            }
        }
    }
    
    public class SmallShipCannonDeed : ShipCannonDeed
    {
        public override ShipCannon Cannon(BaseBoat boat, Point3D offset, ShipCannonDirection dir) { return new SmallShipCannon(boat, offset, dir); }
        public override int LabelNumber { get { return 1095790; } } 

        [Constructable]
        public SmallShipCannonDeed() : base() { Name = "a small boat cannon deed"; }

        public SmallShipCannonDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); } // version
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class SmallShipCannon : ShipCannon
	{
        public override string UberScriptFileName { get { return "boats\\smallcannon.us"; } }
        public override ShipCannonDeed Deed { get { return new SmallShipCannonDeed(); } }
        
        public override void SetFacing(Direction dir)
        {
            if (Boat == null) return;
            switch (dir)
            {
                case Direction.North:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x4218; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x4219; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x4216; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x4217; }
                        break;
                    }
                case Direction.East:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x4219; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x4216; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x4217; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x4218; }
                        break;
                    }
                case Direction.South:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x4216; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x4217; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x4218; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x4219; }
                        break;
                    }
                case Direction.West:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x4217; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x4218; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x4219; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x4216; }
                        break;
                    }
            }
        }

        public SmallShipCannon(BaseBoat boat, Point3D offset, ShipCannonDirection dir) : base(boat, offset, dir) { }

        public SmallShipCannon(Serial serial) : base(serial) { }
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

    public class MediumShipCannonDeed : ShipCannonDeed
    {
        public override ShipCannon Cannon(BaseBoat boat, Point3D offset, ShipCannonDirection dir) { return new MediumShipCannon(boat, offset, dir); }

        [Constructable]
        public MediumShipCannonDeed() : base() { Name = "a medium boat cannon deed"; }

        public MediumShipCannonDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); } // version
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }
    }

    public class MediumShipCannon : ShipCannon
    {
        public override string UberScriptFileName { get { return "alan\\cannons\\medium.txt"; } }
        public override ShipCannonDeed Deed { get { return new MediumShipCannonDeed(); } }
        
        public override void SetFacing(Direction dir)
        {
            if (Boat == null) return;
            switch (dir)
            {
                case Direction.North:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x421C; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x421D; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x421A; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x421B; }
                        break;
                    }
                case Direction.East:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x421D; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x421A; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x421B; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x421C; }
                        break;
                    }
                case Direction.South:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x421A; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x421B; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x421C; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x421D; }
                        break;
                    }
                case Direction.West:
                    {
                        if (CannonDirection == ShipCannonDirection.Fore) { ItemID = 0x421B; }
                        else if (CannonDirection == ShipCannonDirection.Starboard) { ItemID = 0x421C; }
                        else if (CannonDirection == ShipCannonDirection.Aft) { ItemID = 0x421D; }
                        else if (CannonDirection == ShipCannonDirection.Port) { ItemID = 0x421A; }
                        break;
                    }
            }
        }

        public MediumShipCannon(BaseBoat boat, Point3D offset, ShipCannonDirection dir) : base(boat, offset, dir) { }

        public MediumShipCannon(Serial serial) : base(serial) { }
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