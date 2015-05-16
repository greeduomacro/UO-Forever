// **********
// RunUO Shard - MetalKiteShield.cs
// **********

namespace Server.Items
{
    public class MetalKiteShield : BaseShield
    {
        public override int InitMinHits
        {
            get { return 45; }
        }

        public override int InitMaxHits
        {
            get { return 60; }
        }


        public override int ArmorBase
        {
            get { return 16; }
        }

        [Constructable]
        public MetalKiteShield() : base(0x1B74)
        {
            Weight = 7.0;
            Dyable = true;
        }

        public MetalKiteShield(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Weight == 5.0)
                Weight = 7.0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);//version
        }
    }
}