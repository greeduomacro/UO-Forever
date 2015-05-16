#region References

using System;
using Server.Network;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Items
{
    public class WheelofCorptune : Item
    {
        public bool Running { get; set; }

        private Timer m_Timer;
        public Timer Timer { get { return m_Timer; } }

        public DateTime Cooldown { get; set; }

        [Constructable]
        public WheelofCorptune()
            : base(8198)
        {
            Name = "Wheel of Corptune!";
            Movable = true;
            DoesNotDecay = true;
            Amount = 3;
            Running = false;
            Cooldown = DateTime.UtcNow;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (DateTime.UtcNow >= Cooldown)
            {
                if (Timer == null || !Timer.Running)
                {
                    PublicOverheadMessage(MessageType.Emote, 61, true, "Wheel of Corptune!");
                    m_Timer = new InternalTimer(this);
                    m_Timer.Start();
                    Cooldown = DateTime.UtcNow + TimeSpan.FromHours(12);
                }
                else
                {
                    from.SendMessage(61, "You must wait until the corpse has finished spinning!");
                }
            }
            else
            {
                var nextuse = Cooldown - DateTime.UtcNow;
                from.SendMessage("You cannot use this again for another " + nextuse.Hours + " hour(s).");
            }
        }

        private class InternalTimer : Timer
        {
            private int cycles;
            private int count;
            private readonly WheelofCorptune corpse;

            public InternalTimer(WheelofCorptune owner)
                : base(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.05))
            {
                corpse = owner;
                cycles = 0;
                count = 1;
            }

            protected override void OnTick()
            {
                if (cycles == 15)
                {
                    new MovingEffectInfo(new Point3D(corpse.X, corpse.Y, 100), corpse, corpse.Map, 3823, 0, 10,
                        EffectRender.Lighten)
                        .MovingImpact(
                            e =>
                            {
                                int amount = Utility.RandomMinMax(200, 400);

                                if (amount <= 0)
                                {
                                    return;
                                }

                                var g = new Gold(amount);
                                g.MoveToWorld(e.Target.Location, e.Map);

                                new EffectInfo(e.Target, e.Map, 14202, 51, 10, 40, EffectRender.Lighten).Send();
                                Effects.PlaySound(e.Target, e.Map, g.GetDropSound());
                            });
                    Stop();
                    return;
                }
                if (count == 9)
                {
                    count = 1;
                    cycles++;
                }
                corpse.Hue = Utility.RandomBrightHue();
                corpse.Direction = (Direction) count;
                count++;
            }
        }

        public WheelofCorptune(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(Cooldown);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Cooldown = reader.ReadDateTime();
        }
    }
}