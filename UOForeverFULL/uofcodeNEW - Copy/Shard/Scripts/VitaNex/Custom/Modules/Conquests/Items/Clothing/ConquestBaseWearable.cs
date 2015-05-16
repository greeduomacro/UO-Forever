using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Server.Scripts.VitaNex.Custom.Modules.Conquests.Items
{
    public class ConquestBaseWerable : BaseClothing
    {
        [CommandProperty(AccessLevel.GameMaster)]
        private PlayerMobile BoundMobile { get; set; }

        public override bool Dye(Mobile from, IDyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        [Constructable]
        public ConquestBaseWerable()
            : this(2212)
        { }

        [Constructable]
        public ConquestBaseWerable(int itemid, Layer layer)
            : base(itemid, layer)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            NotScissorable = true;
        }

        public ConquestBaseWerable(Serial serial)
            : base(serial)
        { }

        public override bool OnDragLift(Mobile m)
        {
            bool allow = base.OnDragLift(m);

            if (allow && BoundMobile == null && m is PlayerMobile && RootParent == m)
            {
                BoundMobile = m as PlayerMobile;
            }

            if (m as PlayerMobile != BoundMobile && m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendMessage(54, "You cannot pick this item up as it is not bound to you.");
                allow = false;
            }

            return allow;
        }

        public override void OnSingleClick(Mobile @from)
        {
            if (BoundMobile == null && from is PlayerMobile && RootParent == from)
            {
                BoundMobile = from as PlayerMobile;
            }

            base.OnSingleClick(@from);

            if (BoundMobile != null)
            {
                LabelTo(from,"Bound to: " + BoundMobile.RawName, 54);
            }
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            from.SendMessage(54, "This is a soul-bound item and cannot be traded.");
            to.SendMessage(54, "This is a soul-bound item and cannot be traded.");
            return false;
        }

        public override bool CanEquip(Mobile m)
        {
            base.CanEquip(m);

            if (BoundMobile == m as PlayerMobile)
            {
                return true;
            }

            m.SendMessage(54, "You cannot equip this item as it is not bound to you!");
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(BoundMobile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            BoundMobile = reader.ReadMobile<PlayerMobile>();
        }
    }
}
