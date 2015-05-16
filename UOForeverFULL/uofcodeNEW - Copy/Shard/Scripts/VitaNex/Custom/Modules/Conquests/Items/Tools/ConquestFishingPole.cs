using System;
using System.Collections;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;

namespace Server.Items
{
	public class ConquestFishingPole : Item
	{
        [CommandProperty(AccessLevel.GameMaster)]
        private PlayerMobile BoundMobile { get; set; }

		[Constructable]
		public ConquestFishingPole() : base( 0x0DBF )
		{
			Layer = Layer.OneHanded;
			Weight = 8.0;
		    Hue = 1177;
            LootType = LootType.Blessed;
		    Name = "Mastercraft 5000 - Gold Edition";
		}

        public ConquestFishingPole(Serial serial)
            : base(serial)
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            Point3D loc = GetWorldLocation();

            if (!from.InLOS(loc) || !from.InRange(loc, 2))
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that
            else
                Fishing.System.BeginHarvesting(from, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHarvestTool.AddContextMenuEntries(from, this, list, Fishing.System);
        }

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
                LabelTo(from, "Bound to: " + BoundMobile.RawName, 54);
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

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            writer.Write(BoundMobile);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            BoundMobile = reader.ReadMobile<PlayerMobile>();
		}
	}
}