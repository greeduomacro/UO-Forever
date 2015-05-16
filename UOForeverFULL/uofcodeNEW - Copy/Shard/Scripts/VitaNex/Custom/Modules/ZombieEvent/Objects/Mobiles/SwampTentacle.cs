using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "an horrifying corpse" )]
	public class HorrifyingTentacle : BaseCreature
	{
		[Constructable]
		public HorrifyingTentacle() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 66;
		    Name = "horrifying tentacles";
            BaseSoundID = 0x18e;

		    Hue = 1157;

			SetStr( 767, 945 );
			SetDex( 66, 75 );
			SetInt( 46, 70 );

			SetHits( 476, 552 );

			SetDamage( 20, 25 );
			
			

			SetSkill( SkillName.Macing, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 125.1, 140.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 50;

			PackItem( new ShadowIronOre( 25 ) );
			PackItem( new IronIngot( 10 ) );

			if ( 0.05 > Utility.RandomDouble() )
				PackItem( new OrcishKinMask() );

			if ( 0.2 > Utility.RandomDouble() )
				PackItem( new OrcishKinMask(1175) );

			m_Orcs = new List<BaseCreature>();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
		}

		public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning && 0.40 > Utility.RandomDouble() )
				PackBagofRegs( Utility.RandomMinMax( 15, 25 ) );
		}

		//public override bool BardImmune{ get{ return !Core.AOS; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int Meat{ get{ return 2; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomBlackHue(); } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.SavagesAndOrcs; }
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }

		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( caster == this )
				return;

			SpawnOrc( caster );
		}

		private List<BaseCreature> m_Orcs;

		public void SpawnOrc( Mobile target )
		{
			Defrag();

			Map map = target.Map;

			if ( map != null && m_Orcs.Count <= 3 )
			{
				BaseCreature bc = new Corpser();
				//Should never be null.
				bc.Team = this.Team;
				bc.Combatant = target;
				bc.MoveToWorld( target.Location, target.Map );
				m_Orcs.Add( bc );
			}
		}

		public void Defrag()
		{
			for ( int i = m_Orcs.Count-1; i >= 0; i-- )
			{
				BaseCreature bc = m_Orcs[i];

				if ( bc == null || !bc.Alive || bc.Deleted )
					m_Orcs.RemoveAt( i );
			}
		}

		public override void OnDelete()
		{
			Defrag();

			foreach ( BaseCreature bc in m_Orcs )
				if ( bc != null && bc.Alive && !bc.Deleted )
					bc.Delete();

			base.OnDelete();
		}

        public override void OnDeath(Container c)
        {
            if (Utility.RandomDouble() < 0.2)
            {
                c.DropItem(new VileTentacles());
            }
            Engines.ZombieEvent.ZombieEvent.AddItem(c);
            base.OnDeath(c);
        }

        public HorrifyingTentacle(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );

			writer.WriteMobileList( m_Orcs, true );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			switch ( version )
			{
				case 1: m_Orcs = reader.ReadStrongMobileList<BaseCreature>(); break;
				case 0: m_Orcs = new List<BaseCreature>(); break;
			}

			
		}
	}
}