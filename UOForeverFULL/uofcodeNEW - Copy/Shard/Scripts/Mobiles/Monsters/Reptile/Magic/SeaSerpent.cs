using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a sea serpents corpse" )]
	[TypeAlias( "Server.Mobiles.Seaserpant" )]
	public class SeaSerpent : BaseCreature
	{
		public override string DefaultName{ get{ return "a sea serpent"; } }
        DateTime m_CreationTime;

		[Constructable]
		public SeaSerpent() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 150;
			BaseSoundID = 447;

			Hue = Utility.Random( 0x530, 9 );

			SetStr( 168, 225 );
			SetDex( 58, 85 );
			SetInt( 53, 95 );

			SetHits( 110, 127 );

			SetDamage( 7, 13 );

            CreationTimer = DateTime.UtcNow + TimeSpan.FromMinutes(30);

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 60.1, 75.0 );
			SetSkill( SkillName.Tactics, 60.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 70.0 );

			Fame = 6000;
			Karma = -6000;

			VirtualArmor = 30;
			CanSwim = true;
			CantWalk = true;

			if ( Utility.RandomBool() )
				PackItem( new SulfurousAsh( 4 ) );
			else
				PackItem( new BlackPearl( 4 ) );

			PackItem( new RawFishSteak() );

			//PackItem( new SpecialFishingNet() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

        public DateTime CreationTimer
        {
            get { return m_CreationTime; }
            set { m_CreationTime = value; }
        }
        public override void OnThink()
        {
            if (CreationTimer <= DateTime.UtcNow)
            {
                PublicOverheadMessage(MessageType.Emote, 0x3B2,true,"*Dives back down to the bottom of the sea*");
                this.Delete();
            }
            base.OnThink();
        }

		public override bool HasBreath{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 2; } }

		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 8; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Blue; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomGreyHue(); } }

		public SeaSerpent( Serial serial ) : base( serial )
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