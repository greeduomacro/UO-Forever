#region References

using System;
using Server.Network;

#endregion

namespace Server.Items
{
    public class SingingCrystalBall : BaseTrap
    {
        [Constructable]
        public SingingCrystalBall() : base(18058)
        {
            Name = "Singing Crystal Ball";
            Weight = 10;
            Movable = true;
            Stackable = false;
            LootType = LootType.Blessed;
            Light = LightType.Circle150;
        }

        public override bool PassivelyTriggered { get { return true; } }
        public override TimeSpan PassiveTriggerDelay { get { return TimeSpan.FromMinutes(0.1); } }
        public override int PassiveTriggerRange { get { return 3; } }
        public override TimeSpan ResetDelay { get { return TimeSpan.FromMinutes(0.1); } }

        public override void OnTrigger(Mobile from)
        {
            if (from.Alive && CheckRange(from.Location, 4))
            {
                PublicOverheadMessage(MessageType.Label, 54, true, "*emits a sound*");
                switch (Utility.Random(61))
                {
                    case 0:
                        from.PlaySound(745);
                        break;
                    case 1:
                        from.PlaySound(586);
                        break;
                    case 2:
                        from.PlaySound(1048);
                        break;
                    case 3:
                        from.PlaySound(1027);
                        break;
                    case 4:
                        from.PlaySound(581);
                        break;
                    case 5:
                        from.PlaySound(138);
                        break;
                    case 6:
                        from.PlaySound(139);
                        break;
                    case 7:
                        from.PlaySound(142);
                        break;
                    case 8:
                        from.PlaySound(526);
                        break;
                    case 9:
                        from.PlaySound(508);
                        break;
                    case 10:
                        from.PlaySound(510);
                        break;
                    case 11:
                        from.PlaySound(511);
                        break;
                    case 12:
                        from.PlaySound(506);
                        break;
                    case 13:
                        from.PlaySound(498);
                        break;
                    case 14:
                        from.PlaySound(362);
                        break;
                    case 15:
                        from.PlaySound(372);
                        break;
                    case 16:
                        from.PlaySound(412);
                        break;
                    case 17:
                        from.PlaySound(442);
                        break;
                    case 18:
                        from.PlaySound(775);
                        break;
                    case 19:
                        from.PlaySound(579);
                        break;
                    case 20:
                        from.PlaySound(584);
                        break;
                    case 21:
                        from.PlaySound(589);
                        break;
                    case 22:
                        from.PlaySound(522);
                        break;
                    case 23:
                        from.PlaySound(532);
                        break;
                    case 24:
                        from.PlaySound(792);
                        break;
                    case 25:
                        from.PlaySound(788);
                        break;
                    case 26:
                        from.PlaySound(783);
                        break;
                    case 27:
                        from.PlaySound(779);
                        break;
                    case 28:
                        from.PlaySound(590);
                        break;
                    case 29:
                        from.PlaySound(592);
                        break;
                    case 30:
                        from.PlaySound(593);
                        break;
                    case 31:
                        from.PlaySound(594);
                        break;
                    case 32:
                        from.PlaySound(598);
                        break;
                    case 33:
                        from.PlaySound(603);
                        break;
                    case 34:
                        from.PlaySound(604);
                        break;
                    case 35:
                        from.PlaySound(517);
                        break;
                    case 36:
                        from.PlaySound(520);
                        break;
                    case 37:
                        from.PlaySound(530);
                        break;
                    case 38:
                        from.PlaySound(497);
                        break;
                    case 39:
                        from.PlaySound(480);
                        break;
                    case 40:
                        from.PlaySound(489);
                        break;
                    case 41:
                        from.PlaySound(484);
                        break;
                    case 42:
                        from.PlaySound(492);
                        break;
                    case 43:
                        from.PlaySound(495);
                        break;
                    case 44:
                        from.PlaySound(496);
                        break;
                    case 45:
                        from.PlaySound(821);
                        break;
                    case 46:
                        from.PlaySound(910);
                        break;
                    case 47:
                        from.PlaySound(365);
                        break;
                    case 48:
                        from.PlaySound(168);
                        break;
                    case 49:
                        from.PlaySound(478);
                        break;
                    case 50:
                        from.PlaySound(479);
                        break;
                    case 51:
                        from.PlaySound(631);
                        break;
                    case 52:
                        from.PlaySound(632);
                        break;
                    case 53:
                        from.PlaySound(649);
                        break;
                    case 54:
                        from.PlaySound(654);
                        break;
                    case 55:
                        from.PlaySound(716);
                        break;
                    case 56:
                        from.PlaySound(721);
                        break;
                    case 57:
                        from.PlaySound(723);
                        break;
                    case 58:
                        from.PlaySound(726);
                        break;
                    case 59:
                        from.PlaySound(729);
                        break;
                    case 60:
                        from.PlaySound(755);
                        break;
                }
            }
        }

        public SingingCrystalBall(Serial serial) : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 3); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}