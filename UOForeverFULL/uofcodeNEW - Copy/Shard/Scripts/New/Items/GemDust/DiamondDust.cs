using System;
using Server;

namespace Server.Items
{
	public class DiamondDust: Item
	{
		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public DiamondDust() : this( 1 )
		{
		}

		[Constructable]
		public DiamondDust( int amount ) : base( 0x5745 )
		{
			Stackable = true;
            Hue = 496;
			Amount = amount;
        }
        private int CheckGrouping(Item a, Item b)
        {
            return 0;
        }

        private static readonly Type[] m_Types = new Type[] { typeof(DiamondDust), typeof(Food) };
        private static readonly int[] m_Amounts = new int[] { 10, 10 };

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                int consumed = from.Backpack.ConsumeTotalGrouped(m_Types, m_Amounts, true, null, new CheckItemGroup(CheckGrouping));

                if (consumed != -1)
                    from.SendMessage("You do not have the required components.");
                else
                {
                    Consume();
                    from.AddToBackpack(new DiamondPower());
                }
            }
        }

		public DiamondDust( Serial serial ) : base( serial )
		{
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