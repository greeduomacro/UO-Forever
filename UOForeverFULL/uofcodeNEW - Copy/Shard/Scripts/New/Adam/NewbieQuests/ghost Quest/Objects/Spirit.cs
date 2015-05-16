#region References

using System;
using System.Drawing;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;
using VitaNex.SuperGumps.UI;
using EDI = Server.Mobiles.EscortDestinationInfo;

#endregion

namespace Server.Mobiles
{
    public class Spirit : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int MobKills { get; set; }

        private DateTime SpeechTimer;

        public override bool Commandable { get { return false; } } // Our master cannot boss us around!

        [Constructable]
        public Spirit()
            : base(AIType.AI_Melee, FightMode.Aggressor, 22, 1, 0.2, 1.0)
        {
            InitBody();
            InitOutfit();
        }

        public void InitBody()
        {
            Name = "a restless spirit";
            SetStr(90, 100);
            SetDex(90, 100);
            SetInt(15, 25);
            Blessed = true;
            CantWalk = true;

            SpeechHue = 33;

            Hue = 6;

            Body = 401;
        }
        
        public void InitOutfit()
        {
            AddItem(new DeathShroud());

            PackGold(200, 250);
        }

        public bool AcceptEscorter(Mobile m)
        {
            var player = m as PlayerMobile;

            if (player == null || player.NewbieQuestCompleted)
            {
                return false;
            }

            Mobile escorter = GetEscorter();

            if (escorter != null)
            {
                return false;
            }

            if (!m.Alive)
            {
                Say(500894);
                return false;
            }


            foreach (Mobile follower in player.AllFollowers)
            {
                if (follower is Spirit)
                {
                    Say("I see you are busy helping another.");
                    return false;
                }
            }
            if (SetControlMaster(m))
            {
                m_LastSeenEscorter = DateTime.UtcNow;

                if (m is PlayerMobile)
                {
                    ((PlayerMobile) m).LastEscortTime = DateTime.UtcNow;
                }

                Say("Lead on. Vengeance shall be mine.");
                StartFollow();
                return true;
            }

            return false;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            CheckAtDestination();
        }

        public override void OnRegionChange(Region Old, Region New)
        {
            base.OnRegionChange(Old, New);

            CheckAtDestination();
        }

        public void StartFollow()
        {
            StartFollow(GetEscorter());
        }

        public void StartFollow(Mobile escorter)
        {
            if (escorter == null)
            {
                return;
            }

            CantWalk = false;
            ActiveSpeed = 0.1;
            PassiveSpeed = 0.2;

            ControlOrder = OrderType.Follow;
            ControlTarget = escorter;

            CurrentSpeed = 0.1;
        }

        public void StopFollow()
        {
            ActiveSpeed = 0.2;
            PassiveSpeed = 1.0;

            ControlOrder = OrderType.None;
            ControlTarget = null;

            CurrentSpeed = 1.0;
            CantWalk = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            var player = from as PlayerMobile;

            if (player == null || player.NewbieQuestCompleted)
            {
                return;
            }

            if (ControlMaster == null)
            {
                var html = new StringBuilder();

                html.AppendLine(
                    "In life, I was a priestess of this crypt. I would tend the dead and help them on their final journey to the afterlife. All was peaceful here, until word got out that some of the dead were being intered with their belongings.\n"
                        .WrapUOHtmlColor(Color.DarkSeaGreen));
                html.AppendLine(
                    "This is when the "
                        .WrapUOHtmlColor(Color.DarkSeaGreen) + "tomb robbers".WrapUOHtmlColor(Color.Red) + " came.\n".WrapUOHtmlColor(Color.DarkSeaGreen));
                html.AppendLine(
                    "Where we stand now is the place of my...defilement...and resting place. They took my life for mere trinkets left for the dead.\n"
                        .WrapUOHtmlColor(Color.DarkSeaGreen));
                html.AppendLine(
                    "I was peaceful once, but I now crave "
                        .WrapUOHtmlColor(Color.DarkSeaGreen) +
                    "vengeance.\n".WrapUOHtmlColor(Color.Red));
                html.AppendLine(
                    "Give this to me and I will reward you dearly. Help me kill".WrapUOHtmlColor(Color.DarkSeaGreen) +
                    " 10 grave robbers".WrapUOHtmlColor(Color.Red) +
                    " and then guide me to my family's crypt so that I may rest.".WrapUOHtmlColor(Color.DarkSeaGreen));
                var UI =
                    new NewbieQuestGiverUI(from as PlayerMobile, null, null, null, "Quest Offer",
                        html.ToString(),
                        onAccept: x => { AcceptEscorter(from); }).
                        Send<SFQuestGiver>();
            }
            else if (ControlMaster == from && MobKills < 10)
            {
                var html = new StringBuilder();

                html.AppendLine(
                    "My quest for vengeance will be complete when we have killed "
                        .WrapUOHtmlColor(Color.DarkSeaGreen) + (10 - MobKills).ToString().WrapUOHtmlColor(Color.Red) +
                    " more grave robber(s).".WrapUOHtmlColor(Color.DarkSeaGreen));
                var UI =
                    new NewbieQuestGenericUI(from as PlayerMobile, null, null, null, "Quest Details",
                        html.ToString()).
                        Send<NewbieQuestGenericUI>();
            }
            else if (ControlMaster == from && MobKills >= 10)
            {
                Say("Please lead me back to my family's tomb.  It is located in the eastern end of the crypt.");
            }
        }

        public void AddKills()
        {
            MobKills++;
            if (MobKills >= 10)
            {
                Say("My thirst for vengeance has been slaked.  Please take me to my family tomb located in the eastern portion of the crypt.");
            }
        }

        private DateTime m_LastSeenEscorter;

        public Mobile GetEscorter()
        {
            if (!Controlled)
            {
                return null;
            }

            Mobile master = ControlMaster;

            if (master == null)
            {
                return null;
            }

            if (master.Deleted || master.Map != Map || !master.InRange(Location, 30) || !master.Alive)
            {
                StopFollow();

                TimeSpan lastSeenDelay = DateTime.UtcNow - m_LastSeenEscorter;

                if (lastSeenDelay >= TimeSpan.FromMinutes(2.0))
                {
                    master.SendLocalizedMessage(1042473); // You have lost the person you were escorting.
                    Say("Hmm, I seem to have been abandoned."); // Hmmm.  I seem to have lost my master.

                    SetControlMaster(null);

                    Delete();
                }
                else
                {
                    ControlOrder = OrderType.Stay;
                    return master;
                }
            }

            if (ControlOrder != OrderType.Follow)
            {
                StartFollow(master);
            }

            return master;
        }

        public bool CheckAtDestination()
        {
            if (Region.Name != "The Crypt" && Region.Name != "Newbie Dungeon" && ControlMaster != null)
            {
                Say("I'm sorry, I cannot wander too far from the crypt.  Come back and help me if you find the time.");
                CantWalk = true;
                Timer.DelayCall(TimeSpan.FromSeconds(3), DeleteMob);
                return false;
            }

            Mobile escorter = GetEscorter();

            var player = escorter as PlayerMobile;
            if (player == null)
                return false;

            if (MobKills >= 10 && !player.NewbieQuestCompleted)
            {
                Say(
                    "Vengeance makes for a bitter meal indeed. I cannot forgive what was done to me.  Perhaps you are better suited to these items than I. Farewell.");

                Container cont = escorter.Backpack;

                if (cont == null)
                {
                    cont = escorter.BankBox;
                }

                var candle = new CandleofForgiveness {BoundMobile = escorter as PlayerMobile};

                if (!cont.TryDropItem(escorter, candle, false))
                {
                    candle.MoveToWorld(escorter.Location, escorter.Map);
                }

                var ring = new RingofForgiveness {BoundMobile = escorter as PlayerMobile};

                if (!cont.TryDropItem(escorter, ring, false))
                {
                    ring.MoveToWorld(escorter.Location, escorter.Map);
                }

                player.NewbieQuestCompleted = true;

                CompleteMessage(escorter);
                StopFollow();
                Timer.DelayCall(TimeSpan.FromSeconds(10), DeleteMob);

                Titles.AwardFame(escorter, 10, true);

                return true;
            }
            if (MobKills < 10 && DateTime.UtcNow >= SpeechTimer)
            {
                SpeechTimer = DateTime.UtcNow + TimeSpan.FromMinutes(1);
                Say(
                    "My spirit cannot rest while my killers are loose in this crypt.  Please, avenge my death.  Make my killers pay.");
            }

            return false;
        }

        public void CompleteMessage(Mobile from)
        {
            var html = new StringBuilder();

            html.AppendLine(
                "You have just received the "
                    .WrapUOHtmlColor(Color.DarkSeaGreen) + "ring of forgiveness".WrapUOHtmlColor(Color.Orange) + " & the ".WrapUOHtmlColor(Color.DarkSeaGreen) + "candle of forgiveness".WrapUOHtmlColor(Color.Orange) + ".".WrapUOHtmlColor(Color.DarkSeaGreen));
            html.AppendLine(
                "Currently, the ring of forgiveness allows for one teleportation while dead to a healer. This will work once per 12 hours. To activate the rings abilities, you must be dead and then type:\n"
                    .WrapUOHtmlColor(Color.DarkSeaGreen) );
            html.AppendLine(
                "I seek forgiveness\n"
                    .WrapUOHtmlColor(Color.Red));
            html.AppendLine(
                "This will teleport you to the Britain healers if you are blue or the Buccaneer's Den healers if you are red. Pets will accompany you when you are teleported. We have planned many other uses for the ring which will be implemented in the near future.\n"
                    .WrapUOHtmlColor(Color.DarkSeaGreen));
            html.AppendLine(
                "Currently, the candle has no special uses.".WrapUOHtmlColor(Color.DarkSeaGreen));
            var UI =
                new NewbieQuestGenericUI(from as PlayerMobile, null, null, null, "Candle and Ring of Forgiveness",
                    html.ToString()).
                    Send<NewbieQuestGenericUI>();
        }

        public void DeleteMob()
        {
            Effects.SendIndividualFlashEffect(ControlMaster, (FlashType)2);
            Delete();
        }

        public Spirit(Serial serial)
            : base(serial)
        {}

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

        public override bool CanBeRenamedBy(Mobile from)
        {
            return (from.AccessLevel >= AccessLevel.GameMaster);
        }
    }
}