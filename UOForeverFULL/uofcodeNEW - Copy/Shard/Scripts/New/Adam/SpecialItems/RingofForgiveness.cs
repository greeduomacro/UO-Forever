using System;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.Items
{
    public class RingofForgiveness : GoldRing
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile BoundMobile { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CoolDown { get; set; }

        [Constructable]
        public RingofForgiveness()
        {
            Name = "Ring of Forgiveness";
            Hue = 1266;
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public RingofForgiveness(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            var from = e.Mobile as PlayerMobile;

            if (from != null && from == RootParentEntity && !from.Alive)
            {
                if (e.Speech.ToLower().IndexOf("i seek forgiveness") > -1 && DateTime.UtcNow >= from.RingofForgiveness)
                {
                    if (from.Map == Map.ZombieLand)
                    {
                        from.SendMessage(54, "You cannot use this item while in the zombie event.  Please log out of the event first.");
                        return;
                    }
                    Effects.SendIndividualFlashEffect(e.Mobile, (FlashType)2);
                    if (from.Kills >= 5 && !(from.Region is HouseRegion))
                    {
                        from.SendMessage("You have been forgiven.");
                        BaseCreature.TeleportPets(from, new Point3D(2707, 2145, 0), Map.Felucca, false);
                        from.MoveToWorld(new Point3D(2707, 2145, 0), Map.Felucca);
                    }
                    else if (!(from.Region is HouseRegion))
                    {
                        from.SendMessage("You have been forgiven.");
                        BaseCreature.TeleportPets(from, new Point3D(1475, 1612, 20), Map.Felucca, false);
                        from.MoveToWorld(new Point3D(1475, 1612, 20), Map.Felucca);
                    }
                    from.RingofForgiveness = DateTime.UtcNow + TimeSpan.FromHours(12);
                }
            }
        }

        public override bool OnDragLift(Mobile m)
        {
            bool allow = base.OnDragLift(m);

            if (allow && BoundMobile == null && m is PlayerMobile && RootParent == m && m.AccessLevel == AccessLevel.Player)
            {
                BoundMobile = m as PlayerMobile;
            }

            if (m as PlayerMobile != BoundMobile && m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendMessage(54, "You cannot pick this item up as it is not bound to you.");
                allow = false;
            }

            return allow;
        }

        public override void OnSingleClick(Mobile @from)
        {
            if (BoundMobile == null && from is PlayerMobile && RootParent == from && from.AccessLevel == AccessLevel.Player)
            {
                BoundMobile = from as PlayerMobile;
            }

            base.OnSingleClick(@from);

            if (BoundMobile != null)
            {
                LabelTo(from, "Bound to: " + BoundMobile.RawName, 54);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (BoundMobile == null && from is PlayerMobile && RootParent == from && from.AccessLevel == AccessLevel.Player)
            {
                BoundMobile = from as PlayerMobile;
            }

            Effects.SendIndividualFlashEffect(from, (FlashType)2);
            base.OnDoubleClick(from);
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            from.SendMessage(54, "This is a soul-bound item and cannot be traded.");
            to.SendMessage(54, "This is a soul-bound item and cannot be traded.");
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(BoundMobile);

            writer.Write(CoolDown);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            BoundMobile = reader.ReadMobile<PlayerMobile>();

            CoolDown = reader.ReadDateTime();
        }
    }
}