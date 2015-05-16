using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;
using Server.Spells;

namespace Server.Mobiles
{
	public class BarbarianShaman : BaseBarbarian
	{
		[Constructable]
		public BarbarianShaman() : base( false, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.5, 0.5 )
		{
			Title = "a barbarian shaman";
			AddItem( new Kilt( 0x1bb ) );
			AddItem( new BodySash( 0x1bb ) );
			AddItem( new Boots( 0x1bb ) );
			AddItem( new GnarledStaff() );
			FacialHairItemID = 0x204B;
			FacialHairHue = HairHue;
			AddItem( new TribalMask() );

			SetSkill( SkillName.EvalInt, 40.0 );
			SetSkill( SkillName.Magery, 75.0 );
			SetSkill( SkillName.Meditation, 50.0 );
			SetSkill( SkillName.MagicResist, 100.0 );
			SetSkill( SkillName.Tactics, 65.0, 75.0 );
			SetSkill( SkillName.Wrestling, 75.1, 80.0 );

			Fame = 7500;
			Karma = -7500;

			SetStr( 150, 175 );
			SetDex( 65, 100 );
			SetInt( 85, 100 );

			SetHits( 250, 275 );

			SetDamage( 5, 10 );

			VirtualArmor = 30;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Rich );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

        // WISP IS BUGGED FOR SOME REASON...?? Players can control them???
        /*  
		private DateTime m_NextAttack;
		private BaseCreature m_DarkWisp;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
			{
				if ( m_DarkWisp != null )
					m_DarkWisp.Delete();
			}
			else if ( m_DarkWisp != null && !m_DarkWisp.Deleted )
				m_DarkWisp.Combatant = combatant;
			else if ( !Paralyzed && DateTime.UtcNow >= m_NextAttack )
			{
				DarkWispAttack( combatant );
				m_NextAttack = DateTime.UtcNow + TimeSpan.FromSeconds( 8.0 + (8.0 * Utility.RandomDouble()) );
			}
		}

		public void DarkWispAttack( Mobile m )
		{
			DoHarmful( m );

			m_DarkWisp = new DarkWisp();
			m_DarkWisp.ControlSlots = 1;
			SpellHelper.Summon( m_DarkWisp, m, 0x215, TimeSpan.FromMinutes( 10.0 ), false, false );
			m_DarkWisp.Combatant = m;
			m_DarkWisp.PlaySound( 1481 );
		}
         * */

		public BarbarianShaman( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}