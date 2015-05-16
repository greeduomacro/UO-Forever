#region References

using System;
using Server.Network;
using VitaNex.FX;

#endregion
namespace Server.Items
{
    public class VitriolStatue : Item
    {
        DateTime LastSpit { get; set; }

        [Constructable]
        public VitriolStatue() : base(11650)
        {
            Name = "Statue of an Abomination";
            Movable = true;
        }

        public VitriolStatue(Serial serial)
            : base(serial)
        {}

        public override bool HandlesOnMovement
        {
            get { return true; }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (DateTime.UtcNow >= LastSpit && m.InRange(Location, 6) && m.CanSee(this))
            {
                PublicOverheadMessage(MessageType.Emote, 61, true, "*spits*");
                Effects.PlaySound(m.Location, m.Map, 0x19C);

                Timer.DelayCall(
                    TimeSpan.FromMilliseconds(100),
                    () =>
                    {
                            int bloodID = Utility.RandomMinMax(4650, 4655);

                            new MovingEffectInfo(this, m, m.Map, bloodID, 60).MovingImpact(
                                info =>
                                {
                                    var blood = new Blood
                                    {
                                        Name = "slime",
                                        ItemID = bloodID,
                                        Hue = 61
                                    };
                                    blood.MoveToWorld(info.Target.Location, info.Map);

                                    Effects.PlaySound(info.Target, info.Map, 0x028);
                                });
                    });
                LastSpit = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            }
            base.OnMovement(m, oldLocation);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}