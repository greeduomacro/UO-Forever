#region References

using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Targeting;

#endregion

namespace Server.Items
{
    public class ArchwayAddon : BaseAddon
    {
        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
            {18408, -1, 0, -1}, {18407, 0, 0, 0}, {18409, 1, 0, -1} // 
        };

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleportLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool LocationSet { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public override BaseAddonDeed Deed { get { return new ArchwayAddonDeed(); } }

        [Constructable]
        public ArchwayAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
            {
                AddComponent(new ArchwayComponent(this, m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1],
                    m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
            }

            LocationSet = false;
            Level = SecureLevel.CoOwners;
        }

        public ArchwayAddon(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(TeleportLocation);
            writer.Write(LocationSet);
            writer.Write((int) Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            TeleportLocation = reader.ReadPoint3D();
            LocationSet = reader.ReadBool();
            Level = (SecureLevel) reader.ReadInt();
        }
    }

    public class ArchwayComponent : AddonComponent, ISecurable
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public ArchwayAddon Archway { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get { return Archway.Level; } set { Archway.Level = value; } }

        public override bool HandlesOnMovement { get { return true; } }

        [Constructable]
        public ArchwayComponent(ArchwayAddon archway, int itemID)
            : base(itemID)
        {
            Name = "a dimensional archway";
            Archway = archway;
            Level = archway.Level;
            Hue = 2947;
        }

        public ArchwayComponent(Serial serial)
            : base(serial)
        {}

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Player && m.X == X && m.Y == Y && m.Z <= Z && m.Z > Z - 5 && CheckAccess(m) && Archway.LocationSet)
            {
                Effects.SendIndividualFlashEffect(m, (FlashType) 2);
                m.MoveToWorld(Archway.TeleportLocation, Archway.Map);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null || !house.IsOwner(from))
            {
                return;
            }

            from.Target = new InternalTarget(Archway);
            from.SendMessage(54, "Target the teleporation location for your dimensional archway.");
        }

        public override void OnSingleClick(Mobile m)
        {
            base.OnSingleClick(m);

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null || !house.IsOwner(m))
            {
                return;
            }

            m.CloseGump(typeof(SetSecureLevelGump));
            m.SendGump(new SetSecureLevelGump(m, this, house));
        }

        public bool CheckAccess(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
            {
                return true;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsAosRules && (house.Public ? house.IsBanned(m) : !house.HasAccess(m)))
            {
                return false;
            }

            return house != null && house.HasSecureAccess(m, Level);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0); // version

            writer.Write(Archway);

            writer.Write((int) Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    Archway = reader.ReadItem<ArchwayAddon>();
                    Level = (SecureLevel) reader.ReadInt();
                }
                    break;
            }
        }
    }

    public class InternalTarget : Target
    {
        private readonly ArchwayAddon _Archway;

        public InternalTarget(ArchwayAddon archway)
            : base(100, true, TargetFlags.None)
        {
            _Archway = archway;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            var p = o as IPoint3D;

            BaseHouse house = BaseHouse.FindHouseAt(_Archway);

            if (p != null && house.Region.Contains(new Point3D(p)))
            {
                _Archway.TeleportLocation = new Point3D(p);
                _Archway.LocationSet = true;
            }
            else
            {
                from.SendMessage("The teleportation location must be inside of the house where the archway is located!");
            }
        }
    }

    public class ArchwayAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new ArchwayAddon(); } }

        [Constructable]
        public ArchwayAddonDeed()
        {
            Name = "an archway";
        }

        public ArchwayAddonDeed(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        private class InternalTarget : Target
        {
            private readonly BaseAddonDeed m_Deed;

            public InternalTarget(BaseAddonDeed deed)
                : base(-1, true, TargetFlags.None)
            {
                m_Deed = deed;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                var p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || m_Deed.Deleted)
                {
                    return;
                }

                if (m_Deed.IsChildOf(from.Backpack))
                {
                    BaseAddon addon = m_Deed.Addon;

                    SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;

                    var res = AddonFitResult.Valid;
                    house = BaseHouse.FindHouseAt(new Point3D(p), map, p.Z);
                    if (house == null)
                    {
                        res = AddonFitResult.NotInHouse;
                    }

                    if (res == AddonFitResult.Valid)
                    {
                        addon.MoveToWorld(new Point3D(p), map);
                    }
                    else if (res == AddonFitResult.Blocked)
                    {
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    }
                    else if (res == AddonFitResult.NotInHouse)
                    {
                        from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                    }
                    else if (res == AddonFitResult.DoorTooClose)
                    {
                        from.SendLocalizedMessage(500271); // You cannot build near the door.
                    }
                    else if (res == AddonFitResult.NoWall)
                    {
                        from.SendLocalizedMessage(500268); // This object needs to be mounted on something.
                    }

                    if (res == AddonFitResult.Valid)
                    {
                        m_Deed.Delete();
                        house.Addons.Add(addon);
                    }
                    else
                    {
                        addon.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }
        }
    }
}