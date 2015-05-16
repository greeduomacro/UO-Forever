using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a deep sea serpents corpse" )]
	public class DeepSeaSerpent : BaseCreature
	{
		public override string DefaultName{ get{ return "a deep sea serpent"; } }
        DateTime m_CreationTimer;
		[Constructable]
		public DeepSeaSerpent() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 150;
			BaseSoundID = 447;

			Hue = Utility.Random( 0x8A0, 5 );

			SetStr( 251, 425 );
			SetDex( 87, 135 );
			SetInt( 87, 155 );

			SetHits( 151, 255 );

			SetDamage( 6, 14 );

            m_CreationTimer = DateTime.UtcNow + TimeSpan.FromMinutes(30);

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 60.1, 75.0 );
			SetSkill( SkillName.Tactics, 60.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 70.0 );

			Fame = 6000;
			Karma = -6000;

			VirtualArmor = 60;
			CanSwim = true;
			CantWalk = true;

			if ( Utility.RandomBool() )
				PackItem( new SulfurousAsh( 4 ) );
			else
				PackItem( new BlackPearl( 4 ) );

			//PackItem( new SpecialFishingNet() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

        public DateTime CreationTimer
        {
            get { return m_CreationTimer; }
            set { m_CreationTimer = value; }
        }
        public override void OnThink()
        {
            if (CreationTimer <= DateTime.UtcNow)
            {
                PublicOverheadMessage(MessageType.Emote, 0x3B2, true, "*Dives back down to the bottom of the sea*");
                this.Delete();
            }
            base.OnThink();
        }
		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override bool HasBreath{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int Scales{ get{ return 12; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Blue; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomGreyHue(); } }

		public DeepSeaSerpent( Serial serial ) : base( serial )
		{
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
	}
}