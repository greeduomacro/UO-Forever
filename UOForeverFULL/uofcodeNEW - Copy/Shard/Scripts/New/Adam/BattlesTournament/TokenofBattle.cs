#region References

using Server.Engines.EventScores;
using Server.Mobiles;

#endregion

namespace Server.Items
{
    public class TokenofBattle : Item
    {
        [Constructable]
        public TokenofBattle()
            : base(6251)
        {
            Name = "token of battle";
            Hue = 1258;
            Weight = 1.0;
            Stackable = false;
        }

        public TokenofBattle(Serial serial)
            : base(serial)
        {}

        protected override void OnExpansionChanged(Expansion old)
        {
            base.OnExpansionChanged(old);

            if (EraML)
            {
                Stackable = true;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && from is PlayerMobile)
            {
                PlayerEventScoreProfile profile = EventScores.EnsureProfile(from as PlayerMobile);
                profile.SpendablePoints += 2;
                from.SendMessage(54, "This token has granted you +2 spendable battle points!");
                Consume();
            }

            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}