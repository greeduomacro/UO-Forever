namespace Server.Items
{
        [Furniture]
        [Flipable(0xE43, 0xE42)]
        public class CacheChest : Container
        {
            [CommandProperty(AccessLevel.GameMaster)]
            public bool Buried { get; set; }

            [Constructable]
            public CacheChest()
                : base(0xE42)
            {
                Name = "cache chest";
                Weight = 2.0;
                Breakable = true;
                DoesNotDecay = true;
                CreateMap();
            }

            public CacheChest(Serial serial)
                : base(serial)
            {
            }

            public void CreateMap()
            {
                var map = new CacheChestmap {CacheChest = this};
                DropItem(map);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write(Buried);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                Buried = reader.ReadBool();
                if (Weight == 15.0)
                    Weight = 2.0;
            }
        }
}