using Server.Engines.XmlSpawner2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class GargoyleRune : Item
    {
        [Constructable]
        public GargoyleRune()
            : base(0x483B + Utility.Random(25) * 3)
        {
            Weight = 1.0;
            Hue = 39;
        }

        [Constructable]
        public GargoyleRune(int itemID)
            : base(itemID)
        {
        }

        public GargoyleRune(Serial serial)
            : base(serial)
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

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                if (XmlScript.HasTrigger(this, TriggerName.onEquip) && UberScriptTriggers.Trigger(this, (Mobile)parent, TriggerName.onEquip))
                {
                    ((Mobile)parent).AddToBackpack(this); // override, put it in their pack
                    base.OnAdded(parent);
                    return;
                }

                if (Server.Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, (Mobile)parent))
                {
                    Server.Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, (Mobile)parent);
                }
                else
                {
                    ((Mobile)parent).AddToBackpack(this);
                }
            }
            else if (parent is Item)
            {
                Item parentItem = (Item)parent;
                if (XmlScript.HasTrigger(this, TriggerName.onAdded))
                    UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onAdded, parentItem);
            }

            base.OnAdded(parent);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                UberScriptTriggers.Trigger(this, (Mobile)parent, TriggerName.onUnequip);
            }
            else if (parent is Item)
            {
                Item parentItem = (Item)parent;
                UberScriptTriggers.Trigger(this, parentItem.RootParentEntity as Mobile, TriggerName.onRemove, parentItem);
            }

            Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
            base.OnRemoved(parent);
        }

        public override void OnDelete()
        {
            if (XmlScript.HasTrigger(this, TriggerName.onDelete))
                UberScriptTriggers.Trigger(this, this.RootParentEntity as Mobile, TriggerName.onDelete);
            base.OnDelete();
        }
    }
}
