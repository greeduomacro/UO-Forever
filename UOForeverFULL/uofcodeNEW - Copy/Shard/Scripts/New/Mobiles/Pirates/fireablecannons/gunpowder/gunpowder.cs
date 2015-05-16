using System;
using Server;

namespace Server.Items
{
	public class Gunpowder : Item
	{
		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Gunpowder() : this( 1 )
		{
		}

		[Constructable]
		public Gunpowder( int amount ) : base( 0xF26 )
		{
			Stackable = true;
			Hue = 243;
			Amount = amount;
		}

		public Gunpowder( Serial serial ) : base( serial )
		{
		}

        public override string DefaultName { get { return "Raw GunPowder"; } }
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                // The item must be in your backpack to use it.
                from.SendLocalizedMessage(1060640);
            }
            else if (from.Skills.Tinkering.Value < 92.1)
            {
                // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that ability.
                from.SendLocalizedMessage(1063013, "92.1\ttinkering");
            }
            else if (from.NextSkillTime > DateTime.UtcNow)
            {
                // You must wait a few seconds before you can use that item.
                from.SendLocalizedMessage(1070772);
            }
            else
            {
                Consume();
                from.AddToBackpack(new GunpowderDust());
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}