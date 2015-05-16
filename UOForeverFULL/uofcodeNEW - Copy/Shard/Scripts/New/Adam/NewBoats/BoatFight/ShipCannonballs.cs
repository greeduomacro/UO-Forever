using System;

namespace Server.Items
{
    public abstract class ShipCannonball : BaseShipProjectile
    {
        public ShipCannonball()
            : this(1)
        {
        }

        public ShipCannonball(int amount)
            : base(amount, 0xE74)
        {
        }

        public ShipCannonball(Serial serial)
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
    }

    public class IronShipCannonball : ShipCannonball
    {
        [Constructable]
        public IronShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public IronShipCannonball(int amount)
            : base(amount)
        {
            this.Name = "Iron Ship Cannonball";
        }

        public IronShipCannonball(Serial serial)
            : base(serial)
        {
        }

        public override int AnimationID
        {
            get
            {
                return 3699;
            }
        }

        public override int ShipDamage
        {
            get
            {
                return 20;
            }
        }

        public override int PlayerDamage
        {
            get
            {
                return 15;
            }
        }

        public override int DamageRange
        {
            get
            {
                return 2;
            }
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

    public class FieryShipCannonball : ShipCannonball
    {
        [Constructable]
        public FieryShipCannonball()
            : this(1)
        {
        }

        [Constructable]
        public FieryShipCannonball(int amount)
            : base(amount)
        {
            this.Hue = 33;
            this.Name = "Fiery Ship Cannonball";
        }

        public FieryShipCannonball(Serial serial)
            : base(serial)
        {
        }

        // use a fireball animation when fired
        public override int AnimationID
        {
            get
            {
                return 0x36D4;
            }
        }
        public override int AnimationHue
        {
            get
            {
                return 0;
            }
        }

        public override int DamageRange
        {
            get
            {
                return 2;
            }
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

    public class ShipGrapeShot : ShipCannonball
    {
        [Constructable]
        public ShipGrapeShot()
            : this(1)
        {
        }

        [Constructable]
        public ShipGrapeShot(int amount)
            : base(amount)
        {
            Name = "Ship Grape Shot";
            Hue = 1175;
        }

        public ShipGrapeShot(Serial serial)
            : base(serial)
        {
        }

        public override int AnimationID
        {
            get
            {
                return 0x3767;
            }
        }

        public override int DamageRange
        {
            get
            {
                return 4;
            }
        }

        public override int AnimationHue
        {
            get
            {
                return 1175;
            }
        }


        public override int ShipDamage
        {
            get
            {
                return 0;
            }
        }

        public override int PlayerDamage
        {
            get
            {
                return 25;
            }
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

    public class ChainShot : ShipCannonball
    {
        [Constructable]
        public ChainShot()
            : this(1)
        {
        }

        [Constructable]
        public ChainShot(int amount)
            : base(amount)
        {
            Name = "Ship Chain Shot";
            ItemID = 0x41A2;
            Hue = 2301;
        }

        public ChainShot(Serial serial)
            : base(serial)
        {
        }

        public override int AnimationID
        {
            get
            {
                return 0x41A2;
            }
        }

        public override int AnimationHue
        {
            get
            {
                return 2301;
            }
        }

        public override int ShipDamage
        {
            get
            {
                return 25;
            }
        }

        public override int PlayerDamage
        {
            get
            {
                return 0;
            }
        }

        public override int DamageRange
        {
            get
            {
                return 5;
            }
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