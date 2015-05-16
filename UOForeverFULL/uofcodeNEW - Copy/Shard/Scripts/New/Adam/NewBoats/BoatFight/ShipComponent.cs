using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class ShipComponent : AddonComponent
    {
        public ShipComponent(int itemID)
            : base(itemID)
        {
        }

        public ShipComponent(int itemID, string name)
            : base(itemID)
        {
            this.Name = name;
        }

        public ShipComponent(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsDraggable
        {
            get
            {
                if (this.Addon is IShipWeapon)
                {
                    return ((IShipWeapon)this.Addon).IsDraggable;
                }
                return false;
            }
            set
            {
                if (this.Addon is IShipWeapon)
                {
                    ((IShipWeapon)this.Addon).IsDraggable = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPackable
        {
            get
            {
                if (this.Addon is IShipWeapon)
                {
                    return ((IShipWeapon)this.Addon).IsPackable;
                }
                return false;
            }
            set
            {
                if (this.Addon is IShipWeapon)
                {
                    ((IShipWeapon)this.Addon).IsPackable = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool FixedFacing
        {
            get
            {
                if (this.Addon is IShipWeapon)
                {
                    return ((IShipWeapon)this.Addon).FixedFacing;
                }
                return false;
            }
            set
            {
                if (this.Addon is IShipWeapon)
                {
                    ((IShipWeapon)this.Addon).FixedFacing = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Facing
        {
            get
            {
                if (this.Addon is IShipWeapon)
                {
                    return ((IShipWeapon)this.Addon).Facing;
                }
                return 0;
            }
            set
            {
                if (this.Addon is IShipWeapon)
                {
                    ((IShipWeapon)this.Addon).Facing = value;
                }
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.Addon != null)
            {
                this.Addon.OnDoubleClick(from);
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (this.Addon is IShipWeapon)
            {
                IShipWeapon weapon = (IShipWeapon)this.Addon;

                if (!weapon.FixedFacing)
                {
                    list.Add(new RotateNextEntry(weapon));
                    list.Add(new RotatePreviousEntry(weapon));
                }

                if (weapon.IsPackable)
                {
                    list.Add(new BackpackEntry(from, weapon));
                }
            }
            base.GetContextMenuEntries(from, list);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            IShipWeapon weapon = this.Addon as IShipWeapon;

            if (weapon == null)
                return;

            if (weapon.Projectile == null || weapon.Projectile.Deleted)
            {
                //list.Add(1061169, "empty"); // range ~1_val~
                list.Add(1042975); // It's empty
            }
            else
            {
                list.Add(500767); // Reloaded
                list.Add(1060658, "Type\t{0}", weapon.Projectile.Name); // ~1_val~: ~2_val~

            }
        }

        public override void OnMapChange()
        {
            if (this.Addon != null && this.Map != Map.Internal)
                this.Addon.Map = this.Map;
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

        private class RotateNextEntry : ContextMenuEntry
        {
            private readonly IShipWeapon m_weapon;
            public RotateNextEntry(IShipWeapon weapon)
                : base(406)
            {
                this.m_weapon = weapon;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null)
                    this.m_weapon.Facing++;
            }
        }

        private class RotatePreviousEntry : ContextMenuEntry
        {
            private readonly IShipWeapon m_weapon;
            public RotatePreviousEntry(IShipWeapon weapon)
                : base(405)
            {
                this.m_weapon = weapon;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null)
                    this.m_weapon.Facing--;
            }
        }

        private class BackpackEntry : ContextMenuEntry
        {
            private readonly IShipWeapon m_weapon;
            private readonly Mobile m_from;
            public BackpackEntry(Mobile from, IShipWeapon weapon)
                : base(2139)
            {
                this.m_weapon = weapon;
                this.m_from = from;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null)
                {
                    this.m_weapon.StoreWeapon(this.m_from);
                }
            }
        }


        private class SetupEntry : ContextMenuEntry
        {
            private readonly IShipWeapon m_weapon;
            private readonly Mobile m_from;
            public SetupEntry(Mobile from, IShipWeapon weapon)
                : base(97)
            {
                this.m_weapon = weapon;
                this.m_from = from;
            }

            public override void OnClick()
            {
                if (this.m_weapon != null && this.m_from != null)
                {
                    this.m_weapon.PlaceWeapon(this.m_from, this.m_from.Location, this.m_from.Map);
                }
            }
        }
    }
}