using Server.Network;

namespace Server.Items
{
    public class ChargeableTrapPouch : TrapableContainer
    {
        public override bool HueOnMagicTrap { get { return true; } }

        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return (HueOnMagicTrap && TrapPower > 0 && TrapType == TrapType.MagicTrap) ? 1161 : base.Hue; }
            set { base.Hue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get; set; }

        [Constructable]
        public ChargeableTrapPouch()
            : base(0xE79)
        {

            Name = "chargeable trapped pouch";
            Weight = 1.0;

            Charges = 30;

            TrapType = TrapType.MagicTrap;
            TrapPower = 4; //Change to depend on magery skill?
            TrapLevel = 0;	

            LootType = LootType.Blessed;
        }

        public ChargeableTrapPouch(Serial serial)
            : base(serial)
        {
        }

        private void SendMessageTo(Mobile to, int number, int hue)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new MessageLocalized(Serial, ItemID, MessageType.Regular, hue, 3, number, "", ""));
        }

        private void SendMessageTo(Mobile to, string text, int hue)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new UnicodeMessage(Serial, ItemID, MessageType.Regular, hue, 3, to.Language, "", text));
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            if ((RootParent == from  && TrapType == TrapType.MagicTrap))
                LabelTo(from, "(trapped) | Charges: " + Charges, 38);
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            from.SendMessage(54, "This pouch is unable to hold items.");
            return false;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            from.SendMessage(54, "This pouch is unable to hold items.");
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            from.SendMessage(54, "This pouch is unable to hold items.");
            return false;
        }

        public override bool ExecuteTrap(Mobile from)
        {
            if (TrapType != TrapType.None)
            {
                Point3D loc = this.GetWorldLocation();
                Map facet = this.Map;

                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    SendMessageTo(from, "That is trapped, but you open it with your godly powers.", 0x3B2);
                    return false;
                }

                switch (TrapType)
                {
                    case TrapType.MagicTrap:
                        {
                            if (from.InRange(loc, 1))
                                from.Damage(TrapPower);
                            //AOS.Damage( from, m_TrapPower, 0, 100, 0, 0, 0 );

                            Effects.PlaySound(loc, Map, 0x307);

                            Effects.SendLocationEffect(new Point3D(loc.X - 1, loc.Y, loc.Z), Map, 0x36BD, 15);
                            Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y, loc.Z), Map, 0x36BD, 15);

                            Effects.SendLocationEffect(new Point3D(loc.X, loc.Y - 1, loc.Z), Map, 0x36BD, 15);
                            Effects.SendLocationEffect(new Point3D(loc.X, loc.Y + 1, loc.Z), Map, 0x36BD, 15);

                            Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y + 1, loc.Z + 11), Map, 0x36BD, 15);

                            //Update item for hue purposes.
                            ReleaseWorldPackets();
                            Delta(ItemDelta.Update);

                            Charges--;

                            break;
                        }
                }

                if (Charges == 0)
                {
                    TrapType = TrapType.None;
                    TrapPower = 0;
                    TrapLevel = 0;
                }
                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Charges = reader.ReadInt();
        }
    }
}