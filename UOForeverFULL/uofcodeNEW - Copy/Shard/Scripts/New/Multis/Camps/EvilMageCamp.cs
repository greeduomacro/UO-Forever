using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public class EvilMageCamp : BaseCamp
    {
        private BaseCreature m_Prisoner;

        [Constructable]
        public EvilMageCamp() : base(0x0FEA)
        {
        }

        public override void AddComponents()
        {
            DecayDelay = TimeSpan.FromMinutes(60.0);

			Item blood1 = new Item( Utility.Random( 0x1CF2, 33 ) );
			Item blood2 = new Item( Utility.Random( 0x1CF2, 33 ) );

            // pent and abbatoir and ankh
            AddItem( new PentagramAddon(), 0, 0, 0 );
            AddItem( new AbbatoirAddon(), 1, 2, 0 );
            AddItem( new Item(0x1D97), 2, 0, 0 );
            AddItem( new Item(0x1D98), 2, -1, 0 );

			if ( Utility.RandomBool() )
			{
				// floating table with blood
				AddItem(new Item(0x1201), 0, -2, 0);
				AddItem(new Item(0x1203), 0, -3, 0);
				AddItem(new Item(0x1CE7), 0, -3, 6);
				AddItem(blood1, 0, -2, 0);
				AddItem(blood2, 0, -3, 0);
				// floating table without blood
				AddItem(new Item(0x1203), 2, -2, 0);
				AddItem(new Item(0x1202), 2, -3, 0);
				AddItem(new Item(0x1CE8), 2, -2, 6);
			}
			else
			{
				AddItem(new Item(0x1201), 3, 0, 0);
				AddItem(new Item(0x1203), 3, -1, 0);
				AddItem(new Item(0x1203), 3, -2, 0);
				AddItem(new Item(0x1202), 3, -3, 0);
				AddItem(new Item(0x1CE7), 3, -1, 6);
				AddItem(new Item(0x1CE8), 3, -2, 6);
				AddItem(blood1, 3, 0, 0);
				AddItem(blood2, 3, -1, 0);
			}

            // curtains north to south
            AddItem(new Item(0x160D), 0, 5, 0);
            AddItem(new Item(0x160D), 0, 3, 0);
            AddItem(new Item(0x160D), 0, 2, 0);
            AddItem(new Item(0x160D), 0, 1, 0);
            AddItem(new Item(0x160D), 0, 0, 0);
            AddItem(new Item(0x160D), 0, -1, 0);
            AddItem(new Item(0x160D), 0, -2, 0);
            AddItem(new Item(0x160D), 0, -3, 0);
            AddItem(new Item(0x160D), 0, -4, 0);
            AddItem(new Item(0x160D), 0, -5, 0);

            // curtains east to west
            AddItem(new Item(0x160E), 1, -6, 0);
            AddItem(new Item(0x160E), 2, -6, 0);
            AddItem(new Item(0x160E), 3, -6, 0);
            AddItem(new Item(0x160E), 4, -6, 0);
            AddItem(new Item(0x160E), 5, -6, 0);

            // last curtain corner thing
            AddItem(new Item(0x160E), -2, -8, 0);
            AddItem(new Item(0x160D), -2, -8, 0);

            // rest of the deco
            AddItem(new Item(0x1A02), 1, -6, 0);
            AddItem(new Item(0x1A03), 4, -6, 0);
            AddItem(new Item(0x1A09), -3, -6, 0);
            AddItem(new Item(0x1A09), -3, -4, 0);

            // some mages
            for (int i = 0; i < 3; i++)
                AddMobile( new EvilMage(), 6, Utility.Random(-7, 15), Utility.Random(-7, 15), 0 );

            // some mage lords
            for (int i = 0; i < 3; i++)
                AddMobile( new EvilMageLord(), 6, Utility.Random(-7, 15), Utility.Random(-7, 15), 0 );

            // an escort
            switch ( Utility.Random( 6 ) )
            {
                case 0: 
                    m_Prisoner = new Noble(); 
                    break;
                case 1:
                    m_Prisoner = new EscortableMage();
                    break;
                case 2:
                    m_Prisoner = new Merchant();
                    break;
                case 3:
                    m_Prisoner = new Messenger();
                    break;
                default: 
                    m_Prisoner = new SeekerOfAdventure(); 
                    break;
            }

            m_Prisoner.IsPrisoner = true;
            m_Prisoner.CantWalk = true;

            m_Prisoner.YellHue = Utility.RandomList( 0x57, 0x67, 0x77, 0x87, 0x117 );
            AddMobile( m_Prisoner, 2, 0, 0, 0 );
        }

        public EvilMageCamp(Serial serial) : base(serial)
        {
        }

        public override void OnEnter(Mobile m)
        {
            if ( m.Player && m_Prisoner != null && m_Prisoner.CantWalk )
            {
                int number;

                switch (Utility.Random(8))
                {
                    case 0: number = 502261; break; // HELP!
                    case 1: number = 502262; break; // Help me!
                    case 2: number = 502263; break; // Canst thou aid me?!
                    case 3: number = 502264; break; // Help a poor prisoner!
                    case 4: number = 502265; break; // Help! Please!
                    case 5: number = 502266; break; // Aaah! Help me!
                    case 6: number = 502267; break; // Go and get some help!
                    default: number = 502268; break; // Quickly, I beg thee! Unlock my chains! If thou dost look at me close thou canst see them.	
                }

                m_Prisoner.Yell( number );
            }
        }

        // Don't refresh decay timer
        public override void OnExit( Mobile m )
        {
        }

        public override void AddItem( Item item, int xOffset, int yOffset, int zOffset )
        {
            if ( item != null )
                item.Movable = false;

            base.AddItem( item, xOffset, yOffset, zOffset );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( (int)0 ); // version

            writer.WriteMobile<BaseCreature>( m_Prisoner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

			m_Prisoner = reader.ReadMobile<BaseCreature>();
        }
    }
}