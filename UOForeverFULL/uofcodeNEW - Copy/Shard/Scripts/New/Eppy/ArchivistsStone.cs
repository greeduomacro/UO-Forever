using System;
using Server.Mobiles;

namespace Server.Items
{
    class ArchivistsStone : Item
    {   
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile BoundMobile { get; set; }

        [Constructable]
        public ArchivistsStone()
            : base(0x023E)
        {
            Name = "Archivist's Stone";           
            LootType = LootType.Blessed;           

            Weight = 1.0;
        }

        public ArchivistsStone(Serial serial)
            : base(serial)
        {
        }


        public override bool OnDragLift(Mobile m)
        {
            bool allow = base.OnDragLift(m);

            if (allow && BoundMobile == null && m is PlayerMobile && RootParent == m && m.AccessLevel >= AccessLevel.Counselor)
            {
                BoundMobile = m as PlayerMobile;
            }

            if (m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendMessage(54, "This stone is only movable by a GM.");
                allow = false;
            }           

            return allow;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (BoundMobile == null && from is PlayerMobile && RootParent == from && from.AccessLevel >= AccessLevel.Counselor)
            {
                BoundMobile = from as PlayerMobile;
            }
                        
            if (BoundMobile == from && from is PlayerMobile && RootParent == from && from.AccessLevel >= AccessLevel.Counselor)
            {
                if (BoundMobile.BodyValue == 0)
                {
                    BoundMobile.BodyValue = BoundMobile.Race.Body(BoundMobile);

                    // If the owner is already a counselor then no need to do the below
                    //BoundMobile.NameMod = null;
                    //BoundMobile.Squelched = false;
                }
                else
                {
                    BoundMobile.BodyValue = 0;

                    // If the owner is already a counselor then no need to do the below
                    //BoundMobile.NameMod = string.Empty;
                    //BoundMobile.Squelched = true;                    
                }                
            }
            else
            {
                from.SendMessage(54, "Only the owner of this item can use it.");
            }
        }

        public override void OnSingleClick(Mobile @from)
        {
            if (BoundMobile == null && from is PlayerMobile && RootParent == from && from.AccessLevel >= AccessLevel.Counselor)
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
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(BoundMobile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            BoundMobile = reader.ReadMobile<PlayerMobile>();
        }
}
}
