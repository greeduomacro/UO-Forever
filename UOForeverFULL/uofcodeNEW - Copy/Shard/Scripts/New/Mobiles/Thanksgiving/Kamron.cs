using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using Server.Spells;

using System.Collections.Generic;
 
namespace Server.Mobiles
{
    [CorpseName( "Kamron corpse" )]
    public class KamronThanksgivingQuest : Mobile
    {
        public virtual bool IsInvulnerable{ get{ return true; } }
        [Constructable]
        public KamronThanksgivingQuest()
        {
            Name = "Kamron";
                        Title = "Master of thanksgiving";
			Hue = 33770;
            CantWalk = true;
			Body = 0x191;
			switch ( Utility.Random( 2 ) )
			{
				case 0: AddItem( new LongPants( GetRandomHue() ) ); break;
				case 1: AddItem( new ShortPants( GetRandomHue() ) ); break;
			}
			switch ( Utility.Random( 3 ) )
			{
				case 0: AddItem( new FancyShirt( GetRandomHue() ) ); break;
				case 1: AddItem( new Doublet( GetRandomHue() ) ); break;
				case 2: AddItem( new Shirt( GetRandomHue() ) ); break;
			}
			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new Shoes( GetShoeHue() ) ); break;
				case 1: AddItem( new Boots( GetShoeHue() ) ); break;
				case 2: AddItem( new Sandals( GetShoeHue() ) ); break;
				case 3: AddItem( new ThighBoots( GetShoeHue() ) ); break;
			}
			AddItem( new StrawHat() );
            AddItem( new Pitchfork() );
            Blessed = true;
            }
        public KamronThanksgivingQuest( Serial serial ) : base( serial )
        {
        }
 		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}
		public virtual int GetShoeHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomNeutralHue();
		}
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            list.Add(new ThanksgivingGumpEntry(from, this));
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        public class ThanksgivingGumpEntry : ContextMenuEntry
        {
            private Mobile m_Mobile;
            private Mobile m_Giver;
           
            public ThanksgivingGumpEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
            {
                m_Mobile = from;
                m_Giver = giver;
            }
 
            public override void OnClick()
            {
               
 
                          if( !( m_Mobile is PlayerMobile ) )
                    return;
               
                PlayerMobile mobile = (PlayerMobile) m_Mobile;
 
                {
                    if ( ! mobile.HasGump( typeof( ThanksgivingGump ) ) )
                    {
                        mobile.SendGump( new ThanksgivingGump( mobile ));
                       
                    }
                }
            }
        }
    }
}