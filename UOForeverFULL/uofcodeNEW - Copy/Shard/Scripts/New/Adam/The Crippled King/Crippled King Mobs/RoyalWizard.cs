#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CustomTitles;
using Server.Items;
using Server.Network;
using VitaNex.FX;

#endregion

namespace Server.Mobiles
{
    [CorpseName("Thanus' corpse")]
    public class RoyalWizard : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShardsAquired { get; set; }

        private Dictionary<PlayerMobile, int> ShardsGiven;

        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        [Constructable]
        public RoyalWizard()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Thanus";
            Body = 400;
            Hue = 33777;
            SpecialTitle = "Royal Wizard";
            TitleHue = 1926;

            Blessed = true;

            CantWalk = true;

            SpeechHue = YellHue = 34;

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 110);
            }

            VirtualArmor = 36;

            ShardsAquired = 0;

            ShardsGiven = new Dictionary<PlayerMobile, int>();

            AddItem(new Robe(1254));
            AddItem(new Sandals(Utility.RandomYellowHue()));

            var staff = new AncientWildStaff();
            staff.Name = "ancient wizard's staff";
            staff.Hue = 1260;
            staff.Identified = true;
            staff.LootType = LootType.Newbied;
            AddItem(Immovable(staff));

            HairItemID = 8265;
            FacialHairItemID = 8254;

            HairHue = 1462;
            FacialHairHue = 1462;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (InRange(from, 5))
            {
                return true;
            }
            return base.HandlesOnSpeech(from);
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile m = e.Mobile;

            if (!e.Handled && m.InRange(Location, 5))
            {
                string speech = e.Speech.Trim().ToLower();


                if ((speech.IndexOf("help") > -1 || speech.IndexOf("amount") > -1 || speech.IndexOf("need") > -1 ||
                     speech.IndexOf("quest") > -1) && ShardsAquired < 500)
                {
                    e.Handled = true;

                    Say(
                        "I require material from the Crippled King's plane of existence. I have seen strange stone warriors that appear to be alien to our world working in concert with the Blackthorn Coven.");
                    Say(
                        "Bring me pristine shards from their bodies and I'll see what I can conjure up for you.");
                }
                else if ((speech.IndexOf("shard") > -1 || speech.IndexOf("shards") > -1) && ShardsAquired < 500)
                {
                    e.Handled = true;

                    Say(
                        "Do I sense stone shards on you? If that is what I think it is, hand it over here and we'll get you on your way.");
                }
                else if (ShardsAquired == 10)
                {
                    Say(
                        "We have already opened a portal to the Marble Keep. Enter through the portal in front of me if you wish to travel there.");
                }
            }

            base.OnSpeech(e);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            var giver = from as PlayerMobile;
            if (giver == null)
            {
                return false;
            }
            if (dropped is StoneFragment && ShardsAquired < 9)
            {
                if (ShardsAquired < 5)
                {
                    Say(
                        "I still require more stone shards to anchor the portal to the Crippled King's plane of existence.");
                }
                else if (ShardsAquired < 9 && ShardsAquired >= 5)
                {
                    Say(
                        "The gateway is almost prepared, I still need a few more stone shards to ensure the portal's preciseness.");
                }

                if (ShardsGiven != null)
                {
                    if (ShardsGiven.ContainsKey(giver))
                    {
                        ShardsGiven[giver]++;
                    }
                    else
                    {
                        ShardsGiven.Add(giver, 1);
                    }
                }
                ShardsAquired++;
                dropped.Delete();
            }
            else if (dropped is StoneFragment && ShardsAquired == 9)
            {
                NetState.Instances.Where(ns => ns != null && ns.Mobile is PlayerMobile)
                    .ForEach(
                        ns =>
                            ns.Mobile.SendNotification(
                                "The portal to the Marble Keep has been opened. Head to Thanus' arcane circle at Lord British's castle if you wish to explore the Marble Keep.",
                                true, 1.0, 10.0));
                if (ShardsGiven != null)
                {
                    if (ShardsGiven.ContainsKey(giver))
                    {
                        ShardsGiven[giver]++;
                    }
                    else
                    {
                        ShardsGiven.Add(giver, 1);
                    }
                }
                ShardsAquired++;
                Say(
                    "The Portal is opening! Hurry, move off the platform. Standing too close to the portal when it opens could prove fatal!");
                Timer.DelayCall(TimeSpan.FromSeconds(30), OpenPortal);
            }
            else if (dropped is StoneFragment && ShardsAquired == 10)
            {
                Say(
                    "You should keep those. I have no need for them and the gateway has already been anchored to the Marble Keep.");
                return false;
            }

            return true;
        }

        public void OpenPortal()
        {
            var portal = new CrippledKingPortal();
            portal.MoveToWorld(new Point3D(1365, 1666, 35), Map.Felucca);
            Say(
                "The Portal to the Crippled King's keep is open. Tread carefully, we know not what awaits you on the other side.");

            var loc = new Point3D(1365, 1666, 35);
            BaseExplodeEffect e = ExplodeFX.Air.CreateInstance(
                loc, Map, 3, 0, TimeSpan.FromMilliseconds(1000 - ((10) * 100)), null, () =>
                {
                    foreach (Mobile player in AcquireTargets(loc, 3))
                    {
                        player.Damage(100, this);
                    }
                });
            e.Send();

            if (ShardsGiven != null)
            {
                foreach (KeyValuePair<PlayerMobile, int> kvp in ShardsGiven)
                {
                    kvp.Key.BankBox.DropItem(new CrippledKingFragment());
                    kvp.Key.SendMessage(
                        "A refined stone fragment has been added to your bank for participating in the opening of the gateway to the Crippled King's keep.");
                }
                Dictionary<PlayerMobile, int> topThree = (from entry in ShardsGiven
                    orderby entry.Value descending
                    select entry)
                    .Take(3)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
                foreach (KeyValuePair<PlayerMobile, int> kvp in topThree)
                {
                    kvp.Key.BankBox.DropItem(new TitleScroll("The Pathmaker"));
                    kvp.Key.SendMessage(
                        "You were in the top 3 for Stone Shards contributions! The title scroll: The Pathmaker has been placed in your bankbox.");
                }
            }
        }

        public List<Mobile> AcquireTargets(Point3D p, int range)
        {
            return
                p.GetMobilesInRange(Map, range)
                    .Where(
                        m =>
                            m != null && !m.Deleted && m != this && m.Alive && CanBeHarmful(m, false, true) &&
                            (m.Party == null || m.Party != Party) &&
                            (m.Player || (m is BaseCreature && ((BaseCreature) m).GetMaster() is PlayerMobile)))
                    .ToList();
        }

        public RoyalWizard(Serial serial)
            : base(serial)
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);

            writer.Write(ShardsAquired);
            if (ShardsGiven == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(ShardsGiven.Count);

                foreach (KeyValuePair<PlayerMobile, int> kvp in ShardsGiven)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            ShardsAquired = reader.ReadInt();
            int notesCount = reader.ReadInt();

            if (notesCount > 0)
            {
                ShardsGiven = new Dictionary<PlayerMobile, int>();

                for (int i = 0; i < notesCount; i++)
                {
                    var p = reader.ReadMobile<PlayerMobile>();
                    int num = reader.ReadInt();
                    ShardsGiven.Add(p, num);
                }
            }
        }
    }
}