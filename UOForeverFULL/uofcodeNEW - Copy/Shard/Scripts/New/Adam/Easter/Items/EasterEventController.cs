#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Items
{
    public sealed class EasterEventController : Item
    {
        #region Instancing

        public static EasterEventController Instance { get; private set; }

        public static void Configure()
        {
            CommandSystem.Register("EasterSettings", AccessLevel.Seer, OnEventSettingsCommand);
            CommandSystem.Register("FinishEasterEvent", AccessLevel.Administrator, OnFinishEasterCommand);
        }

        private static void OnEventSettingsCommand(CommandEventArgs e)
        {
            if (e.Mobile == null || e.Mobile.Deleted)
            {
                return;
            }

            if (Instance == null)
            {
                Instance = new EasterEventController
                {
                    Location = e.Mobile.Location,
                    Map = e.Mobile.Map
                };
            }

            e.Mobile.SendGump(new PropertiesGump(e.Mobile, Instance));
        }

        private static void OnFinishEasterCommand(CommandEventArgs e)
        {
            if (e.Mobile == null || e.Mobile.Deleted)
            {
                return;
            }

            if (Instance != null && Easter)
            {
                Easter = false;
                EasterCorruptChance = 0;

                if (ParticipantList != null)
                {
                    List<PlayerMobile> topThree = (from entry in ParticipantList
                        orderby entry.Value descending
                        select entry.Key)
                        .Take(3)
                        .ToList();
                    if (topThree.Count >= 1 && topThree[0] != null)
                    {
                        topThree[0].BankBox.DropItem(new StatuePillarSouth()
                        {
                            Name = "Guardian of Easter - First Place Easter 2014",
                            Hue = 1266,
                            Movable = true,
                            LootType = LootType.Blessed
                        });
                        topThree[0].SendMessage(
                            "You were awarded a Guardian of Easter statue for turning in the most purified Easter eggs!!!");
                    }
                    if (topThree.Count >= 2 && topThree[1] != null)
                    {
                        topThree[1].BankBox.DropItem(new StatuePillarSouth()
                        {
                            Name = "Guardian of Easter - Second Place Easter 2014",
                            Hue = 1166,
                            Movable = true,
                            LootType = LootType.Blessed
                        });
                        topThree[1].SendMessage(
                            "You were awarded a Guardian of Easter statue for turning in the second most purified Easter eggs!!");
                    }
                    if (topThree.Count >= 2 && topThree[2] != null)
                    {
                        topThree[2].BankBox.DropItem(new StatuePillarSouth()
                        {
                            Name = "Guardian of Easter - Third Place Easter 2014",
                            Hue = 1165,
                            Movable = true,
                            LootType = LootType.Blessed
                        });
                        topThree[2].SendMessage(
                            "You were awarded a Guardian of Easter statue for turning in the third most purified Easter eggs!!");
                    }
                }
                /*if (PointList != null)
                {
                    PointList.Clear();
                }
                if (ParticipantList != null)
                {
                    ParticipantList.Clear();
                }*/
                NetState.Instances.Where(ns => ns != null && ns.Mobile is PlayerMobile)
                    .ForEach(
                        ns =>
                            ns.Mobile.SendNotification(
                                "The Ultima Online Forever 2014 Easter event is officially over!  We hope everyone had a happy Easter!",
                                true, 1.0, 10.0));
            }
        }

        public static void UpdateInstancePosition(EasterEventController attemptedConstruct)
        {
            if (attemptedConstruct == null)
            {
                return;
            }

            if (Instance == null) // should never happen, but if it does, make this the instance
            {
                Instance = attemptedConstruct;
            }
            else if (attemptedConstruct.Location != Point3D.Zero) // move the instance to it's location and delete it
            {
                Instance.Location = attemptedConstruct.Location;
                attemptedConstruct.Destroy();
            }
        }

        #endregion

        [CommandProperty(AccessLevel.Seer)]
        public static bool Easter { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double EasterCorruptChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double EasterVeryEasyDropChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double EasterEasyDropChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double EasterMediumDropChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double EasterHardDropChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double EasterChampDropChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double HitsBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double StrBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double IntBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double DexBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double SpeedBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double FameBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double KarmaBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static int DamageBuff { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static bool EasterDropRares { get; set; }

        public static Dictionary<PlayerMobile, int> ParticipantList { get; set; }

        public static Dictionary<PlayerMobile, int> PointList { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static int FirstTierCost { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static int SecondTierCost { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static int ThirdTierCost { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static int FourthTierCost { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static int FifthTierCost { get; set; }

        [Constructable]
        public EasterEventController()
            : base(0xEDC)
        {
            Name = "Easter Event Settings Controller";

            Movable = false;
            Visible = false;

            if (Instance != null && Instance != this)
            {
                // there can only be one seasonal event controller game stone in the world
                Instance.Location = Location;

                CommandHandlers.BroadcastMessage(
                    AccessLevel.Seer,
                    1161,
                    "Existing Easter Event Controller has been moved to this location.");

                Timer.DelayCall(TimeSpan.FromSeconds(1), UpdateInstancePosition, this);
            }
            else
            {
                Instance = this;
            }

            ParticipantList = new Dictionary<PlayerMobile, int>();
            PointList = new Dictionary<PlayerMobile, int>();
        }

        public EasterEventController(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.Seer)
            {
                from.SendGump(new PropertiesGump(from, this));
            }
            else
            {
                from.SendMessage("Sorry, but you don't have permission to access this.");
            }

            base.OnDoubleClick(from);
        }

        private void Destroy()
        {
            base.Delete();
        }

        public override void Delete()
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(1);


            switch (version)
            {
                case 1:
                {
                    writer.Write(FirstTierCost);
                    writer.Write(SecondTierCost);
                    writer.Write(ThirdTierCost);
                    writer.Write(FourthTierCost);
                    writer.Write(FifthTierCost);
                    goto case 0;
                }
                case 0:
                {
                    if (ParticipantList == null)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(ParticipantList.Count);

                        foreach (KeyValuePair<PlayerMobile, int> kvp in ParticipantList)
                        {
                            writer.Write(kvp.Key);
                            writer.Write(kvp.Value);
                        }
                    }

                    if (PointList == null)
                    {
                        writer.Write(0);
                    }
                    else
                    {
                        writer.Write(PointList.Count);

                        foreach (KeyValuePair<PlayerMobile, int> kvp in PointList)
                        {
                            writer.Write(kvp.Key);
                            writer.Write(kvp.Value);
                        }
                    }


                    writer.Write(Easter);
                    writer.Write(HitsBuff);
                    writer.Write(StrBuff);
                    writer.Write(IntBuff);
                    writer.Write(DexBuff);
                    writer.Write(SpeedBuff);
                    writer.Write(FameBuff);
                    writer.Write(KarmaBuff);
                    writer.Write(DamageBuff);
                    writer.Write(EasterChampDropChance);
                    writer.Write(EasterCorruptChance);
                    writer.Write(EasterMediumDropChance);
                    writer.Write(EasterEasyDropChance);
                    writer.Write(EasterVeryEasyDropChance);
                    writer.Write(EasterHardDropChance);
                    writer.Write(EasterDropRares);
                }
                    break;
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                {
                    FirstTierCost = reader.ReadInt();
                    SecondTierCost = reader.ReadInt();
                    ThirdTierCost = reader.ReadInt();
                    FourthTierCost = reader.ReadInt();
                    FifthTierCost = reader.ReadInt();
                    goto case 0;
                }
                case 0:
                {
                    int participantcount = reader.ReadInt();
                    if (participantcount > 0)
                    {
                        ParticipantList = new Dictionary<PlayerMobile, int>();

                        for (int i = 0; i < participantcount; i++)
                        {
                            var p = reader.ReadMobile<PlayerMobile>();
                            int num = reader.ReadInt();
                            ParticipantList.Add(p, num);
                        }
                    }

                    int pointcount = reader.ReadInt();
                    if (pointcount > 0)
                    {
                        PointList = new Dictionary<PlayerMobile, int>();

                        for (int i = 0; i < pointcount; i++)
                        {
                            var p = reader.ReadMobile<PlayerMobile>();
                            int num = reader.ReadInt();
                            PointList.Add(p, num);
                        }
                    }

                    Easter = reader.ReadBool();
                    HitsBuff = reader.ReadDouble();
                    StrBuff = reader.ReadDouble();
                    IntBuff = reader.ReadDouble();
                    DexBuff = reader.ReadDouble();
                    SpeedBuff = reader.ReadDouble();
                    FameBuff = reader.ReadDouble();
                    KarmaBuff = reader.ReadDouble();
                    DamageBuff = reader.ReadInt();
                    EasterChampDropChance = reader.ReadDouble();
                    EasterCorruptChance = reader.ReadDouble();
                    EasterMediumDropChance = reader.ReadDouble();
                    EasterEasyDropChance = reader.ReadDouble();
                    EasterVeryEasyDropChance = reader.ReadDouble();
                    EasterHardDropChance = reader.ReadDouble();
                    EasterDropRares = reader.ReadBool();
                }
                    break;
            }

            Instance = this;
        }
    }
}