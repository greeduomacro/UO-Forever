using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.XmlSpawner2;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using VitaNex.FX;
using VitaNex.Network;

namespace Server.Items
{
    public abstract class BaseShipWeapon : BaseGalleonItem, IShipWeapon
    {
        public bool Storing = false;
        private int m_Facing;
        private BaseShipProjectile m_Projectile;
        private DateTime m_NextFiringTime;
        private bool m_FixedFacing = false;
        private bool m_Draggable = true;
        private bool m_Packable = true;
        public BaseShipWeapon(BaseGalleon galleon, int northItemID, Point3D initOffset)
			:base(galleon, northItemID, initOffset)
        {
        }

        public BaseShipWeapon(Serial serial)
            : base(serial)
        {
        }
		
		[CommandProperty(AccessLevel.GameMaster, true)]
        public override bool ShareHue
        {
            get { return false; }
        }

        public virtual double WeaponLoadingDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for loading this weapon
        public virtual double WeaponStorageDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for packing away this weapon
        public virtual double DamageReductionWhenDamaged
        {
            get
            {
                return 0.4;
            }
        }// scale damage from 40-100% depending on the damage it has taken 
        public virtual double RangeReductionWhenDamaged
        {
            get
            {
                return 0.7;
            }
        }// scale range from 70-100% depending on the damage it has taken 
        public virtual int MinTargetRange
        {
            get
            {
                return 1;
            }
        }// target must be further away than this
        public virtual int MinStorageRange
        {
            get
            {
                return 2;
            }
        }// player must be at least this close to store the weapon
        public virtual int MinFiringRange
        {
            get
            {
                return 3;
            }
        }// player must be at least this close to fire the weapon
        public virtual bool CheckLOS
        {
            get
            {
                return true;
            }
        }// whether the weapon needs to consider line of sight when selecting a target
        public virtual int StoredWeaponID
        {
            get
            {
                return 3644;
            }
        }// itemid used when the weapon is packed up (crate by default)
        /*public override BaseAddonDeed Deed
        *{
        *    get
        *    {
        *        return null;
        *    }
        *}
		*/
        public abstract Type[] AllowedProjectiles { get; }

        // default damage multiplier for the weapon
        public virtual double WeaponDamageFactor
        {
            get
            {
                return 1;
            }
        }
        // default range multiplier for the weapon
        public virtual double WeaponRangeFactor
        {
            get
            {
                return 1;
            }
        }
        public virtual BaseShipProjectile Projectile
        {
            get
            {
                return this.m_Projectile;
            }
            set
            {
                this.m_Projectile = value;
                // invalidate component properties
                /*if (this.Components != null)
                *{
                *    foreach (AddonComponent c in this.Components)
                *    {
                *        c.InvalidateProperties();
                *    }
                *}
				*/
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsDraggable
        {
            get
            {
                return this.m_Draggable;
            }
            set
            {
                this.m_Draggable = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsPackable
        {
            get
            {
                return this.m_Packable;
            }
            set
            {
                this.m_Packable = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool FixedFacing
        {
            get
            {
                return this.m_FixedFacing;
            }
            set
            {
                this.m_FixedFacing = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Facing
        {
            get
            {
                return this.m_Facing;
            }
            set
            {
                this.m_Facing = value;
                if (this.m_Facing < 0)
                    this.m_Facing = 3;
                if (this.m_Facing > 3)
                    this.m_Facing = 0;
                this.UpdateDisplay();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextFiring
        {
            get
            {
                return this.m_NextFiringTime - DateTime.UtcNow;
            }
            set
            {
                this.m_NextFiringTime = DateTime.UtcNow + value;
            }
        }
        public virtual Point3D ProjectileLaunchPoint
        {
            get
            {
                return (this.Location);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxDurability { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Durability { get; set; }

        public abstract void UpdateDisplay();

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Projectile != null)
            {
                this.m_Projectile.Delete();
            }
        }

        public virtual void StoreWeapon(Mobile from)
        {
            if (from == null)
                return;

            if (!from.InRange(this.Location, 2) || from.Map != this.Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            // 15 second delay to pack up the cannon
            Timer.DelayCall(TimeSpan.FromSeconds(this.WeaponStorageDelay), new TimerStateCallback(StoreWeapon_Callback),
                new object[] { from, this });

            from.SendMessage("Packing up the {0}...", this.Name);

            this.Storing = true;
        }

        public virtual void StoreWeapon_Callback(object state)
        {
            object[] args = (object[])state;

            Mobile from = (Mobile)args[0];
            BaseShipWeapon weapon = (BaseShipWeapon)args[1];

            if (weapon == null || weapon.Deleted || from == null)
                return;

            // make sure that there is only one person nearby
            IPooledEnumerable moblist = from.Map.GetMobilesInRange(weapon.Location, this.MinStorageRange);
            int count = 0;
            if (moblist != null)
            {
                foreach (Mobile m in moblist)
                {
                    if (m.Player)
                        count++;
                }
            }
            if (count > 1)
            {
                from.SendMessage("Too many players nearby. Storage failed.");
                return;
            }

            // make sure that the player is still next to the weapon
            if (!from.InRange(weapon.Location, this.MinStorageRange) || from.Map != weapon.Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                from.SendMessage("{0} not stored.", weapon.Name);
                return;
            }

            // use the crate itemid while stored
            weapon.ItemID = this.StoredWeaponID;
            weapon.Visible = true;
            weapon.Movable = true;
            from.AddToBackpack(weapon);

            // hide the components
            /*foreach (AddonComponent i in weapon.Components)
            *{
            *    if (i != null)
            *    {
            *        i.Internalize();
            *    }
            *}
            */

            from.SendMessage("{0} stored.", weapon.Name);
            weapon.Storing = false;
        }

        public virtual void LoadWeapon(Mobile from, BaseShipProjectile projectile)
        {
            if (projectile == null)
                return;

            if (this.m_Projectile != null && !this.m_Projectile.Deleted)
            {
                from.SendMessage("{0} unloaded", this.m_Projectile.Name);
                from.AddToBackpack(this.m_Projectile);
            }

            if (projectile.Amount > 1)
            {
                //projectile.Amount--;
                //Projectile = projectile.Dupe(1);
                this.Projectile = Mobile.LiftItemDupe(projectile, projectile.Amount - 1) as BaseShipProjectile;
            }
            else
            {
                this.Projectile = projectile;
            }

            Projectile = projectile;

            if (this.m_Projectile != null)
            {
                m_Projectile.Internalize();
                from.SendMessage("{0} loaded.", this.m_Projectile.Name);
            }
        }

        public virtual void PlaceWeapon(Mobile from, Point3D location, Map map)
        {
            this.MoveToWorld(location, map);
            this.UpdateDisplay();
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D point)
        {
            bool dropped = base.OnDroppedToWorld(from, point);

            if (dropped)
            {
                this.ItemID = 1;
                this.Visible = false;
                this.Movable = false;
                this.UpdateDisplay();
            }
            return dropped;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Parent != null)
                return;

            if (Projectile == null)
            {
                from.SendMessage(61, "You have not yet loaded a projectile into this cannon.");
                return;
            }
            if (this.NextFiring.Seconds > 0)
            {
                from.SendMessage(61, "This cannon has not yet cooled down.");
                return;
            }
            else
                from.Target = new ShipTarget(this, from);
        }

        public void DoFireEffect(IPoint3D target, Mobile from)
        {
            Map map = Map;

            if (target == null || map == null)
            {
                return;
            }

            var startloc = new Point3D(Location);
            IPoint3D point = target;

            switch (Facing)
            {
                case 3:
                    {
                        Effects.SendLocationEffect(new Point3D(X, Y + 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
                case 1:
                    {
                        Effects.SendLocationEffect(new Point3D(X, Y - 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
                case 2:
                    {
                        Effects.SendLocationEffect(new Point3D(X + 1, Y, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
                case 0:
                    {
                        Effects.SendLocationEffect(new Point3D(X - 1, Y + 1, Z - 2), map, Utility.RandomList(0x3728), 16, 1);
                        break;
                    }
            }
            Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));

            var queue = new EffectQueue();
            queue.Deferred = false;

            queue.Add(
                new MovingEffectInfo(
                    startloc,
                    point,
                    Map,
                    m_Projectile.AnimationID,
                    m_Projectile.AnimationHue > 0 ? m_Projectile.AnimationHue-1 : 0,
                    10,
                    EffectRender.Normal,
                    TimeSpan.FromSeconds(0.1),
                    () =>
                    {
                        if (Projectile.ShipDamage > 0)
                        {
                            for (int count = 8; count > 0; count--)
                            {
                                IPoint3D location = new Point3D(target.X + Utility.RandomMinMax(-1, 1),
                                    target.Y + Utility.RandomMinMax(-1, 1), target.Z);
                                int effect = Utility.RandomList(0x36B0);
                                Effects.SendLocationEffect(location, map, effect, 25, 1);
                                Effects.PlaySound(target, map, Utility.RandomList(0x11B, 0x11C, 0x11D));
                            }
                        }
                        foreach (Mobile mob in AcquireAllTargets(new Point3D(point), Projectile.DamageRange))
                        {
                            if (from.CanBeHarmful(mob))
                            {
                                mob.Damage(Projectile.PlayerDamage, from);
                                if (from.IsHarmfulCriminal(mob) && !from.Criminal)
                                    from.CriminalAction(true);
                                else if (from.IsHarmfulCriminal(mob))
                                    from.CriminalAction(false);
                            }
                        }

                        foreach (Item item in from.Map.GetItemsInRange(new Point3D(point), Projectile.DamageRange))
                        {
                            if (item is MainMast)
                            {
                                var mast = item as MainMast;
                                if (mast.HullDurability < Projectile.ShipDamage)
                                    mast.HullDurability = 0;
                                else
                                {
                                    mast.HullDurability -= (ushort)Projectile.ShipDamage;
                                }
                                mast.PublicOverheadMessage(MessageType.Label, 61, true, mast.HullDurability + "/" + mast.MaxHullDurability);
                            }

                            if (item is BaseShipWeapon)
                            {
                                var cannon = item as BaseShipWeapon;
                                if (cannon.Durability < Projectile.ShipDamage)
                                    cannon.Durability = 0;
                                else
                                {
                                    cannon.Durability -= Projectile.ShipDamage;
                                }
                            }

                            if (item is NewBaseBoat)
                            {
                                var boat = item as NewBaseBoat;
                                if (boat.HullDurability < Projectile.ShipDamage)
                                    boat.HullDurability = 0;
                                else
                                {
                                    boat.HullDurability -= (ushort)Projectile.ShipDamage;
                                }
                                boat.PublicOverheadMessage(MessageType.Label, 61, true, boat.HullDurability + "/" + boat.MaxHullDurability);
                            }
                        }
                    }));
            queue.Process();
            Projectile.Delete();
            Projectile = null;
            this.m_NextFiringTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);
        }

        public List<Mobile> AcquireAllTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m.Alive)
                    .ToList();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            // version 1
            writer.Write(this.m_FixedFacing);
            writer.Write(this.m_Draggable);
            writer.Write(this.m_Packable);
            // version 0
            writer.Write(this.m_Facing);
            writer.Write(this.m_Projectile);
            writer.Write(this.m_NextFiringTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    this.m_FixedFacing = reader.ReadBool();
                    this.m_Draggable = reader.ReadBool();
                    this.m_Packable = reader.ReadBool();
                    goto case 0;
                case 0:
                    this.m_Facing = reader.ReadInt();
                    this.m_Projectile = reader.ReadItem<BaseShipProjectile>();
                    this.m_NextFiringTime = reader.ReadDateTime();
                    break;
            }
        }

        private bool ContainsInterface(Type[] typearray, Type type)
        {
            if (typearray == null || type == null)
                return false;

            foreach (Type t in typearray)
            {
                if (t == type)
                    return true;
            }

            return false;
        }

        private class ShipTarget : Target
        {
            private readonly BaseShipWeapon m_weapon;
            private readonly Mobile m_from;
            public ShipTarget(BaseShipWeapon weapon, Mobile from)
                : base(30, true, TargetFlags.None)
            {
                this.m_weapon = weapon;
                this.m_from = from;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_weapon == null || m_weapon.Deleted)
                {
                    return;
                }

                var p = targeted as IPoint3D;

                if (p == null)
                {
                    return;
                }

                if (!Utility.InRange(new Point3D(p), m_weapon.Location, 0))
                {
                    bool allow = false;

                    int x = p.X - m_weapon.X;
                    int y = p.Y - m_weapon.Y;

                    switch (m_weapon.Facing)
                    {
                        case 1:
                            if (y < 0 && Math.Abs(x) <= -y)
                            {
                                allow = true;
                            }

                            break;
                        case 2:
                            if (x > 0 && Math.Abs(y) <= x)
                            {
                                allow = true;
                            }

                            break;
                        case 3:
                            if (y > 0 && Math.Abs(x) <= y)
                            {
                                allow = true;
                            }

                            break;
                        case 0:
                            if (x < 0 && Math.Abs(y) <= -x)
                            {
                                allow = true;
                            }

                            break;
                    }

                    if (allow && Utility.InRange(new Point3D(p), m_weapon.Location, 200) && from.InRange(m_weapon, 10))
                    {
                        m_weapon.DoFireEffect(p, from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1076203); // Target out of range.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1076215); // Cannon must be aimed farther away.
                }
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                from.SendLocalizedMessage(1076203); // Target out of range.
            }
        }		
    }
}