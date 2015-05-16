using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a pumpkin corpse" )]
	public class EvilPumpkin: BaseCreature
	{
		private DateTime m_NextAbility;

		[Constructable]
		public EvilPumpkin() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Evil Pumpkin";
			Body = 1246;
			Hue = 0;
			BaseSoundID = 178;

			SetStr( 800 );
			SetDex( 60 );
			SetInt( 100 );

			SetHits( 10000 );

			SetDamage( 15, 20 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );
			SetSkill( SkillName.Magery, 120.0 );
			SetSkill( SkillName.EvalInt, 120.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 40;

			//TODO: Random Drops x 5
		}

		public override void OnThink()
		{				
			if ( m_NextAbility < DateTime.UtcNow && this.Str < 1600 )
			{
				ArrayList alist = new ArrayList();
				IPooledEnumerable eable = this.Map.GetMobilesInRange( this.Location, 5 );
				foreach( Mobile m in eable )
					alist.Add( m );
				eable.Free();
				if ( alist != null && alist.Count > 0 )
				{
					for( int i = 0; i < alist.Count; i++ )
					{
						Mobile m = (Mobile)alist[i];
						if ( m is BaseCreature )
						{
							BaseCreature c = m as BaseCreature;
                            if (c.ControlMaster != null)
                                this.Hits += Utility.Random(5, 10);
						}
						else if ( m is PlayerMobile )
							this.Hits += Utility.Random( 1, 5 );
					}
				}
				m_NextAbility = DateTime.UtcNow + TimeSpan.FromSeconds( 2.0 );
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 5 );
		}
			public override void OnDeath( Container c )
		{
			base.OnDeath( c );	

			double rand = Utility.RandomDouble();

			if ( 0.08 > rand ) //Small Web
				c.AddItem( new SmallWebOrange() );
			else if ( 0.08 > rand ) //Small Web
				c.AddItem( new SmallWebSouth() );
		}

		public override bool Unprovokable{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public EvilPumpkin( Serial serial ) : base( serial )
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