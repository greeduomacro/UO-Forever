#region References

using System;
using Server.Mobiles;
using Server.Network;

#endregion

namespace Server.Items
{
    public class EvolutionEgg : Item
    {
        private DateTime _Hatch;
        private bool _CanHatch;

        [Constructable]
        public EvolutionEgg() : base(0x47E6)
        {
            Weight = 0.0;
            Name = "a dragon egg";
            Hue = Utility.RandomList(1109, 1154, 1173, 2655, 2963, 1155, 1284, 971, 972, 691, 742, 811, 900);
            _CanHatch = true;
        }

        [Constructable]
        public EvolutionEgg(int hue)
            : base(0x47E6)
        {
            Weight = 0.0;
            Name = "a dragon egg";
            Hue = hue;
            _CanHatch = true;
        }

        public EvolutionEgg(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("You must have the dragon egg in your backpack to hatch it.");
            }
            else if (from.Skills.AnimalTaming.Base >= 50 && from.Followers <= 2)
            {
                from.SendMessage(54, "The dragon egg begins to hatch..");
                Hatch(from);
                Consume();
            }
            else
            {
                from.SendMessage("You do not have the required skill to hatch this egg.");
            }
        }

        public void Hatch(Mobile from)
        {
            Effects.SendFlashEffect(from, (FlashType)2);

            var dragon = new MetaDragon();

            dragon.Hue = Hue;
            dragon.Map = from.Map;
            dragon.Location = from.Location;

            dragon.Controlled = true;

            dragon.ControlMaster = from;

            dragon.IsBonded = true; 
  
        
            Consume();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(_Hatch);
            writer.Write(_CanHatch);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                {
                    _Hatch = reader.ReadDateTime();
                    _CanHatch = reader.ReadBool();
                }
                    break;
            }
        }
    }
}