#region References

using Server.Engines.EventScores;
using Server.Mobiles;

#endregion

namespace Server.Items
{
    [FlipableAttribute(0x1576, 0x1577)]
    public class EmblemofBattle : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public string EventName { get; set; }

        [Constructable]
        public EmblemofBattle()
            : base(0x1577)
        {
            Name = "emblem of battle";
            Hue = 1258;
            Weight = 1.0;
            Stackable = false;
            EventName = "Emblem of Battle";
        }

        public EmblemofBattle(Serial serial)
            : base(serial)
        {}

        public override void OnDoubleClick(Mobile from)
        {
            /*if (IsChildOf(from.Backpack) && from is PlayerMobile)
            {
                PlayerEventScoreProfile profile = EventScores.EnsureProfile(from as PlayerMobile);
                if (profile.AddScav(EventName, 300, from as PlayerMobile))
                {
                    from.SendMessage(54, "You absorb the essence of the emblem.");
                    Consume();
                }
                else
                {
                    from.SendMessage(54, "You cannot absorb another emblem of battle.");
                }    
            }

            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }*/
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(EventName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            EventName = reader.ReadString();
        }
    }
}