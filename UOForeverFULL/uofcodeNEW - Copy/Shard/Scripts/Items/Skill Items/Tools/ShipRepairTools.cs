using System;
using Server;
using Server.Engines.Craft;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    public class ShipRepairTools : BaseTool
    {   
        public override CraftSystem CraftSystem { get { return DefTinkering.CraftSystem; } } // not technically used
        public override int LabelNumber { get { return 1011071; } }

        public static string UberScriptFileName { get { return "boats\\repairtools.us"; } }

        [Constructable]
        public ShipRepairTools()
            : base(0x0BB6)
        {
            Weight = 1.0;
            if (UberScriptFileName != null)
            {
                XmlScript script = new XmlScript(UberScriptFileName);
                script.Name = "shiprepair";
                XmlAttach.AttachTo(this, script);
            }
        }

        [Constructable]
        public ShipRepairTools(int uses)
            : base(uses, 0x1EB8)
        {
            Weight = 1.0;
            if (UberScriptFileName != null)
            {
                XmlScript script = new XmlScript(UberScriptFileName);
                script.Name = "shiprepair";
                XmlAttach.AttachTo(this, script);
            }
        }

        public ShipRepairTools(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}