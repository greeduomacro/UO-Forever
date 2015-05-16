#region References

using System;
using Server.Mobiles;

#endregion

namespace Server.Items
{
    internal class AsylumTeleportationDoor : Item
    {
        public override bool HandlesOnMovement { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleportLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AsylumKeyType KeyType { get; set; }

        [Constructable]
        public AsylumTeleportationDoor()
            : base(464)
        {
            Name = "mysterious wall";
            DoesNotDecay = true;
            Hue = 1175;
            Movable = false;
        }

        public AsylumTeleportationDoor(Serial serial)
            : base(serial)
        {}

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            var pm = m as PlayerMobile;
            if (m.InRange(Location, 1) && m.Backpack != null && pm != null && DateTime.UtcNow >= pm.AsylumDoorNextUse)
            {
                if (KeyType == AsylumKeyType.None)
                {
                    if (TeleportLocation != Point3D.Zero)
                    {
                        pm.AsylumDoorNextUse = DateTime.UtcNow + TimeSpan.FromSeconds(60);
                        BaseCreature.TeleportPets(m, TeleportLocation, m.Map, false);
                        m.MoveToWorld(TeleportLocation, Map);
                    }
                }
                else
                {
                    if (SearchPackForItem(m.Backpack))
                    {
                        if (TeleportLocation != Point3D.Zero)
                        {
                            pm.AsylumDoorNextUse = DateTime.UtcNow + TimeSpan.FromSeconds(60);
                            BaseCreature.TeleportPets(m, TeleportLocation, m.Map, false);
                            m.MoveToWorld(TeleportLocation, Map);
                        }
                    }
                }
            }
            else if (pm != null && pm.InRange(Location, 1))
            {
                TimeSpan nextuse = pm.AsylumDoorNextUse - DateTime.UtcNow;
                pm.SendMessage("You cannot use an asylum door for another " + nextuse.Seconds + " seconds.");
            }

            base.OnMovement(m, oldLocation);
        }

        public bool SearchPackForItem(Container pack)
        {
            if (pack != null && !pack.Deleted)
            {
                foreach (Item item in pack.Items.ToArray())
                {
                    if (item is Container)
                    {
                        if (SearchPackForItem(item as Container))
                        {
                            return true;
                        }
                    }
                    else if (item is AsylumKey)
                    {
                        var key = item as AsylumKey;
                        if (key.KeyType == KeyType)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(TeleportLocation);

            writer.Write((int) KeyType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    TeleportLocation = reader.ReadPoint3D();
                    KeyType = (AsylumKeyType) reader.ReadInt();
                }
                    break;
            }
        }
    }
}