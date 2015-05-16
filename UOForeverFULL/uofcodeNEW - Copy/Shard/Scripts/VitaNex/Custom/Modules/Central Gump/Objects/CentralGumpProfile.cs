#region References

using System;
using System.Collections.Generic;
using Server.Mobiles;
using VitaNex;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.CentralGump
{
    public sealed class CentralGumpProfile : PropertyObject
    {
        [CommandProperty(CentralGump.Access, true)]
        public PlayerMobile Owner { get; private set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool BuyHead { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool FactionPoint { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool IgnoreNotifies { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool IgnoreTournament { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool IgnoreBattles { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool IgnoreConquests { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool IgnoreMoongates { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool FameTitle { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool MiniGump { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool DisablePvPTemplate { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool DisableTemplateEquipment { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool LoginGump { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public DateTime LastPartyJoin { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public DateTime LastBritainTeleport { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public DateTime LastDungeonTeleport { get; set; }

        [CommandProperty(CentralGump.Access, true)]
        public bool PublicLegends { get; set; }

        public CentralGumpProfile(PlayerMobile pm)
        {
            Owner = pm;
            IgnoreNotifies = false;
            IgnoreTournament = false;
            IgnoreBattles = false;
            IgnoreConquests = false;
            IgnoreMoongates = false;
            FameTitle = true;
            MiniGump = true;
            DisablePvPTemplate = false;
            DisableTemplateEquipment = false;
            LoginGump = true;
            BuyHead = false;
            FactionPoint = false;

            PublicLegends = false;
        }

        public CentralGumpProfile(GenericReader reader)
            : base(reader)
        {}

        public override void Reset()
        {}

        public override void Clear()
        {}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = writer.SetVersion(3);

            switch (version)
            {
                case 3:
                    {
                        writer.Write(FactionPoint);
                    }
                    goto case 2;
                case 2:
                    {
                        writer.Write(PublicLegends);
                    }
                    goto case 1;
                case 1:
                {
                    writer.Write(BuyHead);
                }
                    goto case 0;
                case 0:
                {
                    writer.Write(IgnoreNotifies);
                    writer.Write(IgnoreTournament);
                    writer.Write(IgnoreBattles);
                    writer.Write(IgnoreConquests);
                    writer.Write(IgnoreMoongates);
                    writer.Write(FameTitle);
                    writer.Write(MiniGump);
                    writer.Write(DisablePvPTemplate);
                    writer.Write(DisableTemplateEquipment);
                    writer.Write(LoginGump);
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
                case 3:
                    {
                        FactionPoint = reader.ReadBool();
                    }
                    goto case 2;
                case 2:
                    {
                        PublicLegends = reader.ReadBool();
                    }
                    goto case 1;
                case 1:
                {
                    BuyHead = reader.ReadBool();
                }
                goto case 0;
                case 0:
                {
                    IgnoreNotifies = reader.ReadBool();
                    IgnoreTournament = reader.ReadBool();
                    IgnoreBattles = reader.ReadBool();
                    IgnoreConquests = reader.ReadBool();
                    IgnoreMoongates = reader.ReadBool();
                    FameTitle = reader.ReadBool();
                    MiniGump = reader.ReadBool();
                    DisablePvPTemplate = reader.ReadBool();
                    DisableTemplateEquipment = reader.ReadBool();
                    LoginGump = reader.ReadBool();
                }
                    break;
            }
        }
    }
}