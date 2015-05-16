#region References

using System;
using Server.Network;

#endregion

namespace Server.Items
{
    public class DecorativeSword : Item
    {
        private DateTime LastUsed;

        [Constructable]
        public DecorativeSword() : base(9934)
        {
            Name = "Decorative Sword of Battle";
            LootType = LootType.Blessed;
        }

        public DecorativeSword(Serial serial)
            : base(serial)
        {}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from,
                "A decorative Sword of Battle, crafted in rememberance of the 2014 Battles Tournament and all those who died competing.",
                137);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (DateTime.UtcNow >= LastUsed)
            {
                LastUsed = DateTime.UtcNow + TimeSpan.FromMinutes(20);
                Effects.SendIndividualFlashEffect(from, (FlashType) 2);
                from.SolidHueOverride = 2498;
                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    from.BoltEffect(2049);
                    from.Say("I HAVE THE POWER!");
                    Timer.DelayCall(TimeSpan.FromSeconds(3), () => { from.SolidHueOverride = -1; });
                });
            }
            else
            {
                TimeSpan nextuse = LastUsed - DateTime.UtcNow;
                from.SendMessage("You cannot use this again for another " + nextuse.Minutes + " minutes.");
            }
            base.OnDoubleClick(from);
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