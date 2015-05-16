#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using VitaNex.FX;

#endregion

namespace Server.Items
{
    public class AsylumStatue : Item
    {
        public override bool HandlesOnMovement { get { return true; } }

        [Constructable]
        public AsylumStatue() : base(0x12D8)
        {
            Name = "Asylum Guardian";
            DoesNotDecay = true;
            Hue = 1157;
            Movable = false;
        }

        public AsylumStatue(Serial serial)
            : base(serial)
        {}

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            IPooledEnumerable mobs = GetMobilesInRange(5);
            List<PlayerMobile> players =
                (from Mobile mob in mobs
                    where mob is PlayerMobile && mob.AccessLevel >= AccessLevel.Player && mob.Alive
                    select mob as PlayerMobile).ToList();

            foreach (PlayerMobile playerMobile in players)
            {
                if (!playerMobile.FindItemOnLayer(Layer.Neck).TypeEquals(typeof(BlindingAnkh)) && playerMobile.AccessLevel == AccessLevel.Player)
                {
                    DoExplode(playerMobile);
                }
            }


            base.OnMovement(m, oldLocation);
        }

        public void DoExplode(PlayerMobile pm)
        {
            pm.Emote("*The gaze of the asylum guardian melts the flesh from your bones and causes your organs to explode.*");
            int range = Utility.RandomMinMax(5, 7);
            int zOffset = pm.Mounted ? 20 : 10;

            Point3D src = pm.Location.Clone3D(0, 0, zOffset);
            Point3D[] points = src.GetAllPointsInRange(pm.Map, 0, range);

            Effects.PlaySound(pm.Location, pm.Map, 0x19C);

            pm.FixedParticles(0x36BD, 20, 10, 5044, 137, 0, EffectLayer.Head);
            pm.PlaySound(0x307);

            Timer.DelayCall(
                TimeSpan.FromMilliseconds(100),
                () =>
                {
                    int i = 0;
                    int place = 0;
                    int[] BodyPartArray = {7584, 7583, 7586, 7585, 7588, 7587};
                    foreach (Point3D trg in points)
                    {
                        i++;
                        int bodypartID = Utility.RandomMinMax(4650, 4655);

                        if (Utility.RandomDouble() <= 0.1 && place < BodyPartArray.Count())
                        {
                            bodypartID = BodyPartArray[place];
                            place++;
                        }
                        new MovingEffectInfo(src, trg.Clone3D(0, 0, 2), pm.Map, bodypartID).MovingImpact(
                            info =>
                            {
                                Item bodypart;
                                if (bodypartID <= 4655 && bodypartID >= 4650)
                                {
                                    bodypart = new Blood
                                    {
                                        ItemID = bodypartID
                                    };
                                    bodypart.MoveToWorld(info.Target.Location, info.Map);
                                }

                                switch (bodypartID)
                                {
                                    case 7584:
                                        bodypart = new Head();
                                        bodypart.MoveToWorld(info.Target.Location, info.Map);
                                        break;

                                    case 7583:
                                        bodypart = new Torso();
                                        bodypart.MoveToWorld(info.Target.Location, info.Map);
                                        break;

                                    case 7586:
                                        bodypart = new RightArm();
                                        bodypart.MoveToWorld(info.Target.Location, info.Map);
                                        break;

                                    case 7585:
                                        bodypart = new LeftArm();
                                        bodypart.MoveToWorld(info.Target.Location, info.Map);
                                        break;

                                    case 7588:
                                        bodypart = new RightLeg();
                                        bodypart.MoveToWorld(info.Target.Location, info.Map);
                                        break;

                                    case 7587:
                                        bodypart = new LeftLeg();
                                        bodypart.MoveToWorld(info.Target.Location, info.Map);
                                        break;
                                }

                                Effects.PlaySound(info.Target, info.Map, 0x028);
                            });
                    }
                });

            pm.Damage(pm.Hits + 5);

            Timer.DelayCall(
                TimeSpan.FromMilliseconds(100),
                () =>
                {
                    var corpse = pm.Corpse as Corpse;

                    if (corpse != null && !corpse.Deleted)
                    {
                        corpse.TurnToBones();
                    }
                });
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