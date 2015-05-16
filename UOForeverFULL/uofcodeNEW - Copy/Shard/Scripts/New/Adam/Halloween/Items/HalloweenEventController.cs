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
    public sealed class HalloweenEventController : Item
    {
        #region Instancing

        public static HalloweenEventController Instance { get; private set; }

        public static void Configure()
        {
            CommandSystem.Register("HalloweenSettings", AccessLevel.Seer, OnEventSettingsCommand);
        }

        private static void OnEventSettingsCommand(CommandEventArgs e)
        {
            if (e.Mobile == null || e.Mobile.Deleted)
            {
                return;
            }

            if (Instance == null)
            {
                Instance = new HalloweenEventController
                {
                    Location = e.Mobile.Location,
                    Map = e.Mobile.Map
                };
            }

            e.Mobile.SendGump(new PropertiesGump(e.Mobile, Instance));
        }


        public static void UpdateInstancePosition(HalloweenEventController attemptedConstruct)
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
        public static bool Halloween { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double HalloweenCorruptChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double HalloweenSkinDyeChance { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public static double CostumeChance { get; set; }

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

        [Constructable]
        public HalloweenEventController()
            : base(0xEDC)
        {
            Name = "Halloween Event Settings Controller";

            Movable = false;
            Visible = false;

            Hue = 1358;

            if (Instance != null && Instance != this)
            {
                // there can only be one seasonal event controller game stone in the world
                Instance.Location = Location;

                CommandHandlers.BroadcastMessage(
                    AccessLevel.Seer,
                    1161,
                    "Existing Halloween Event Controller has been moved to this location.");

                Timer.DelayCall(TimeSpan.FromSeconds(1), UpdateInstancePosition, this);
            }
            else
            {
                Instance = this;
            }
        }

        public HalloweenEventController(Serial serial)
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

            int version = writer.SetVersion(0);


            switch (version)
            {
                case 0:
                {
                    writer.Write(Halloween);
                    writer.Write(HitsBuff);
                    writer.Write(StrBuff);
                    writer.Write(IntBuff);
                    writer.Write(DexBuff);
                    writer.Write(SpeedBuff);
                    writer.Write(FameBuff);
                    writer.Write(KarmaBuff);
                    writer.Write(DamageBuff);
                    writer.Write(CostumeChance);
                    writer.Write(HalloweenCorruptChance);
                    writer.Write(HalloweenSkinDyeChance);
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
                case 0:
                {
                    Halloween = reader.ReadBool();
                    HitsBuff = reader.ReadDouble();
                    StrBuff = reader.ReadDouble();
                    IntBuff = reader.ReadDouble();
                    DexBuff = reader.ReadDouble();
                    SpeedBuff = reader.ReadDouble();
                    FameBuff = reader.ReadDouble();
                    KarmaBuff = reader.ReadDouble();
                    DamageBuff = reader.ReadInt();
                    CostumeChance = reader.ReadDouble();
                    HalloweenCorruptChance = reader.ReadDouble();
                    HalloweenSkinDyeChance = reader.ReadDouble();
                }
                    break;
            }

            Instance = this;
        }
    }
}