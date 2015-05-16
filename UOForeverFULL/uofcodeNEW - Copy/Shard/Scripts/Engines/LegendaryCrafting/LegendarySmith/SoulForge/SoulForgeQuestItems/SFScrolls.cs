#region References

using Server.Mobiles;
using VitaNex.SuperGumps.UI;

#endregion

namespace Server.Engines.Craft
{
    public class DragonBoneSFScroll : Item
    {
        [Constructable]
        public DragonBoneSFScroll()
            : base(8816)
        {
            Name = "a tattered scroll";
            Stackable = false;
            Weight = 1.0;
            Hue = 0;
            Movable = false;
            DoesNotDecay = true;
        }

        public DragonBoneSFScroll(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            string html =
                "..dragon bone shards provide an aura of strength and durability that is unmatched by any other relic. Only the bones of a dragon can contain the corruption emanating from the Soulforge and withstand the flames burning within. The shards are ground up and used in the creation of the more mundane parts of the Soulforge.";

            if (from is PlayerMobile)
            {
                var scroll = new SFScrollUI(from as PlayerMobile, null, null, null, null, html).
                    Send<PlayerScoreResultsGump>();

                var notepad = from.Backpack.FindItemByType<TranscriptionBook>(true,
                    i => i != null && !i.Deleted);

                if (notepad != null && notepad.AddScroll(this))
                {
                    from.SendMessage(54, "You transcribe the scroll on your notepad.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class RikktorSFScroll : Item
    {
        [Constructable]
        public RikktorSFScroll()
            : base(8816)
        {
            Name = "a tattered scroll";
            Stackable = false;
            Weight = 1.0;
            Hue = 0;
            Movable = false;
            DoesNotDecay = true;
        }

        public RikktorSFScroll(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            string html =
                "...the smouldering heart of Rikktor's contribution to the creation of the Soulforge is much akin to the process of taking a flint to tinder. When combined with the corrupt soul of the Harrower, it ignites the dragon hearts to create the everlasting flame of the Soulforge. The heart of this great dragon-king continues to burn long after its removal from the great beast's chest.";

            if (from is PlayerMobile)
            {
                var scoreboard = new SFScrollUI(from as PlayerMobile, null, null, null, null, html).
                    Send<PlayerScoreResultsGump>();

                var notepad = from.Backpack.FindItemByType<TranscriptionBook>(true,
                    i => i != null && !i.Deleted);

                if (notepad != null && notepad.AddScroll(this))
                {
                    from.SendMessage(54, "You transcribe the scroll on your notepad.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DragonHeartSFScroll : Item
    {
        [Constructable]
        public DragonHeartSFScroll()
            : base(8816)
        {
            Name = "a tattered scroll";
            Stackable = false;
            Weight = 1.0;
            Hue = 0;
            Movable = false;
            DoesNotDecay = true;
        }

        public DragonHeartSFScroll(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            string html =
                "...intact dragon hearts contain the essence of undying flame and magic. The fuel of the forge itself, the hearts produce a heat unrivaled by any other worldly material. While aflame they continue to beat and cause a continuous pulsing of heat to emenate from deep within the forge.";

            if (from is PlayerMobile)
            {
                var scoreboard = new SFScrollUI(from as PlayerMobile, null, null, null, null, html).
                    Send<PlayerScoreResultsGump>();

                var notepad = from.Backpack.FindItemByType<TranscriptionBook>(true,
                    i => i != null && !i.Deleted);

                if (notepad != null && notepad.AddScroll(this))
                {
                    from.SendMessage(54, "You transcribe the scroll on your notepad.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DevourerSoulSFScroll : Item
    {
        [Constructable]
        public DevourerSoulSFScroll()
            : base(8816)
        {
            Name = "a tattered scroll";
            Stackable = false;
            Weight = 1.0;
            Hue = 0;
            Movable = false;
            DoesNotDecay = true;
        }

        public DevourerSoulSFScroll(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            string html =
                "..the Devourer's soul absorbs the corruption that radiates from the soul of the Harrower trapped within the forge. Lacking it, the forge would only produce cursed items. The very nature of the Devourer is used to remove the corruption from the Harrower's soul and make the forge safe for use.";

            if (from is PlayerMobile)
            {
                var scoreboard = new SFScrollUI(from as PlayerMobile, null, null, null, null, html).
                    Send<PlayerScoreResultsGump>();

                var notepad = from.Backpack.FindItemByType<TranscriptionBook>(true,
                    i => i != null && !i.Deleted);

                if (notepad != null && notepad.AddScroll(this))
                {
                    from.SendMessage(54, "You transcribe the scroll on your notepad.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class HarrowerSoulSFScroll : Item
    {
        [Constructable]
        public HarrowerSoulSFScroll()
            : base(8816)
        {
            Name = "a tattered scroll";
            Stackable = false;
            Weight = 1.0;
            Hue = 0;
            Movable = false;
            DoesNotDecay = true;
        }

        public HarrowerSoulSFScroll(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            string html =
                "...a harrower soul can, similar to its living form, change the form of elements and give rebirth to decay and ruin. The torturous demonic nature of the Harrower is contained within its soul and radiates corruption into the forge. As it is the greatest catalyst used in making the Soulforge, it is only fitting that it is also the most difficult relic to obtain.";

            if (from is PlayerMobile)
            {
                var scoreboard = new SFScrollUI(from as PlayerMobile, null, null, null, null, html).
                    Send<SFScrollUI>();

                var notepad = from.Backpack.FindItemByType<TranscriptionBook>(true,
                    i => i != null && !i.Deleted);

                if (notepad != null && notepad.AddScroll(this))
                {
                    from.SendMessage(54, "You transcribe the scroll on your notepad.");
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}