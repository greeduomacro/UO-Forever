using System;
using System.Collections;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a bloody corpse" )]
	public class BloodyPumpkin : BaseCreature
	{
		private DateTime m_NextAbility;
		[Constructable]
		public BloodyPumpkin() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Bloody Pumpkin";
			Body = 1247;
			Hue = 1157;
			BaseSoundID = 659;

			SetStr( 200 );
			SetDex( 60 );
			SetInt( 100 );

			SetHits( 6000 );

			SetDamage( 15, 20 );

			

			
			
			
			
			

			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Anatomy, 120.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 40;

			//TODO: Random Drops
		}

		public override void OnThink()
		{
			if ( m_NextAbility < DateTime.UtcNow )
			{
				ArrayList alist = new ArrayList();
				IPooledEnumerable eable = this.Map.GetObjectsInRange( this.Location, 5 );
				foreach( Object o in eable )
					alist.Add( o );
				eable.Free();
				if ( alist != null && alist.Count > 0 )
				{
					for( int i = 0; i < alist.Count; i++ )
					{
						Object o = (Object)alist[i];
						if ( o is Corpse )
							this.Hits += 100;
					}
				}
				m_NextAbility = DateTime.UtcNow + TimeSpan.FromSeconds( 30.0 );
			}
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 5 );
		}
			public override void OnDeath( Container c )
		{
			base.OnDeath( c );	

			double rand = Utility.RandomDouble();

			if ( 0.08 > rand ) //Small Web
				c.AddItem( new SmallWebBlack() );
			else if ( 0.08 > rand ) //Small Web
				c.AddItem( new SmallWebSouth() );
		}

		public BloodyPumpkin( Serial serial ) : base( serial )
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