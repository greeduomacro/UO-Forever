#region References

using System;
using Server.Network;

#endregion

namespace Server.Items
{
    [Flipable(0x1953, 0x1950)]
    public class MongbatDartBoard : Item
    {
        public bool East { get { return ItemID == 0x1953; } }
        public int RndHitSound { get { return Utility.RandomList(424, 425); } }
        public int RndMissSound { get { return Utility.RandomList(422, 423); } }
        public int DirectHitSound { get { return 426; } }

        [Constructable]
        public MongbatDartBoard() : this(true)
        {}

        [Constructable]
        public MongbatDartBoard(bool east) : base(east ? 0x1953 : 0x1950)
        {}

        public MongbatDartBoard(Serial serial) : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            var weapon = from.Weapon as BaseWeapon;

            Direction dir;
            if (from.Location != Location)
            {
                dir = from.GetDirectionTo(this);
            }
            else if (East)
            {
                dir = Direction.West;
            }
            else
            {
                dir = Direction.North;
            }

            from.Direction = dir;

            bool canThrow = true;

            if (!from.InRange(this, 5) || !from.InLOS(this))
            {
                canThrow = false;
            }

            else if (East)
            {
                canThrow = (dir == Direction.Left || dir == Direction.West || dir == Direction.Up);
            }
            else
            {
                canThrow = (dir == Direction.Up || dir == Direction.North || dir == Direction.Right);
            }

            if (canThrow)
            {
                if (from.FindItemOnLayer(Layer.OneHanded) == null)
                {
                    PunchMongbat(from, weapon);
                }
                else if (weapon is BaseKnife)
                {
                    ThrowKnife(from);
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public void PunchMongbat(Mobile from, BaseWeapon weapon)
        {
            if (!from.InRange(this, 1))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "You are too far away to punch that thing.");
                return;
            }
            else
            {
                from.Direction = from.GetDirectionTo(GetWorldLocation());
                weapon.PlaySwingAnimation(from);
                Timer.DelayCall(TimeSpan.FromSeconds(0.75), new TimerCallback(OnMongbatPunch));
            }
        }

        public void ThrowKnife(Mobile from)
        {
            var knife = from.Weapon as BaseKnife;

            if (knife == null)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500751); // Try holding a knife...
                return;
            }

            from.Animate(from.Mounted ? 26 : 9, 7, 1, true, false, 0);
            from.MovingEffect(this, knife.ItemID, 7, 1, false, false);
            from.PlaySound(0x238);

            double rand = Utility.RandomDouble();

            if (rand < 0.05)
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Right between the eyes! 50 Points!");
                Effects.PlaySound(Location, Map, 426);
                OnMongbatHit(from);
            }
            else if (rand < 0.20)
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "A Shot to the neck! 20 points.");
                Effects.PlaySound(Location, Map, Utility.RandomList(424, 425));
                OnMongbatHit(from);
            }
            else if (rand < 0.45)
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "A shot to the body, 10 points.");
                Effects.PlaySound(Location, Map, Utility.RandomList(424, 425));
                OnMongbatHit(from);
            }
            else if (rand < 0.70)
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Clipped its wing, 5 pointer.");
                Effects.PlaySound(Location, Map, Utility.RandomList(424, 425));
                OnMongbatHit(from);
            }
            else if (rand < 0.85)
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "Bounced off him..  1 point.");
                Effects.PlaySound(Location, Map, Utility.RandomList(424, 425));
                OnMongbatHit(from);
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, 500757); // Missed.
                Effects.PlaySound(Location, Map, Utility.RandomList(422, 423));
            }
        }

        public virtual void OnMongbatPunch()
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(424, 425));
            ItemID = (ItemID == 0x1953 ? 0x1954 : 0x1951);
            Timer.DelayCall(TimeSpan.FromSeconds(0.45), new TimerCallback(OnMongbatReset));
        }

        public virtual void OnMongbatHit(Mobile from)
        {
            if (!from.Alive)
            {
                return;
            }

            ItemID = (ItemID == 0x1953 ? 0x1954 : 0x1951);
            Timer.DelayCall(TimeSpan.FromSeconds(0.45), new TimerCallback(OnMongbatReset));
        }

        public virtual void OnMongbatReset()
        {
            ItemID = (ItemID == 0x1954 ? 0x1953 : 0x1950); // reset
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}