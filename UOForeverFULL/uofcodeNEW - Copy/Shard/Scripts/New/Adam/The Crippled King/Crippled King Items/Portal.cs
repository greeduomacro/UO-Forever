#region References

using System;
using Server.Engines.XmlSpawner2;
using Server.Network;

#endregion

namespace Server.Items
{
    public class CrippledKingPortal : Item
    {
        [Constructable]
        public CrippledKingPortal()
            : base(3948)
        {
            Name = "a dimensional gateway";
            Movable = false;
            Hue = 1266;
            DoesNotDecay = true;
        }

        public CrippledKingPortal(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from,
                "A strange mist swirls in the portal.  You can faintly make out the outline of a large structure off in the distance.",
                54);
        }

        public override void OnDoubleClick(Mobile from)
        {
            StoneCheck(from);
        }

        public override bool OnMoveOver(Mobile from)
        {
            StoneCheck(from);
            return true;
        }

        public void StoneCheck(Mobile from)
        {
            XmlValue xmlValue = XmlAttach.GetValueAttachment(from, "petrificationimmune");

            if (xmlValue == null || xmlValue.Value == 0)
            {
                ParsedGumps.SendGump(null, from, "Quests/CrippledKing/portal.xml", true);
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(10), Teleport_Callback, from);
            }
        }

        public void Teleport_Callback(Mobile from)
        {
            Effects.SendIndividualFlashEffect(from, (FlashType) 2);
            from.MoveToWorld(new Point3D(1363, 1105, -26), Map.Ilshenar);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.GetVersion();
        }
    }
}