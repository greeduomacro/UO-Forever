using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
    public class BaseShipProjectile : Item, IShipProjectile
    {
        public BaseShipProjectile()
            : this(1, 0xE74)
        {
        }

        public BaseShipProjectile(int amount)
            : this(amount, 0xE74)
        {
        }

        public BaseShipProjectile(int amount, int itemid)
            : base(itemid)
        {
            this.Weight = 5;
            this.Stackable = true;
            this.Amount = amount;
        }

        public BaseShipProjectile(Serial serial)
            : base(serial)
        {
        }

        public virtual int AnimationID
        {
            get
            {
                return 0xE73;
            }
        }
        public virtual int AnimationHue
        {
            get
            {
                return 0;
            }
        }

        public virtual int ShipDamage
        {
            get
            {
                return 0;
            }
        }

        public virtual int PlayerDamage
        {
            get
            {
                return 0;
            }
        }

        public virtual int DamageRange
        {
            get
            {
                return 0;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            // check the range between the player and projectiles
            if ((this.Parent == null && !from.InRange(this.Location, 2)) ||
                (this.RootParent is Mobile && !from.InRange(((Mobile)this.RootParent).Location, 2)) ||
                (this.RootParent is Container && !from.InRange(((Container)this.RootParent).Location, 2))
            )
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.SendMessage(61, "Which cannon would you like to use this type of projectile?");
            from.Target = new ShipWeaponTarget(this);
        }

        public void OnHit(Mobile from, IShipWeapon weapon, IEntity target, Point3D targetloc)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            // version 0
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    break;
            }
        }

        private class ShipWeaponTarget : Target
        {
            private readonly BaseShipProjectile m_projectile;
            public ShipWeaponTarget(BaseShipProjectile projectile)
                : base(2, true, TargetFlags.None)
            {
                this.m_projectile = projectile;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || this.m_projectile == null || from.Map == null)
                    return;

                IShipWeapon weapon = null;

                if (targeted is IShipWeapon)
                {
                    // load the cannon
                    weapon = (IShipWeapon)targeted;
                }
                else if (targeted is ShipComponent)
                {
                    weapon = ((ShipComponent)targeted).Addon as IShipWeapon;
                }

                if (weapon == null || weapon.Map == null)
                {
                    from.SendMessage("This is not a cannon.");
                    return;
                }

                // load the cannon
                weapon.LoadWeapon(from, this.m_projectile); 
            }
        }
    }
}