using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Misc;
using Server.Engines.XmlSpawner2;

namespace Server.Mobiles
{
	[CorpseName( "a necromancer corpse" )]
	public class EvilNecromancer : BaseCreature
	{
		public override string DefaultName{ get{ return "a necromatic guard"; } }

		[Constructable]
		public EvilNecromancer () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Kills = 5;
			Body = 400;
			BaseSoundID = 0x45A;
			Hue = 33;
			SetStr( 116, 150 );
			SetDex( 91, 115 );
			SetInt( 80, 100 );
			SetHits( 50, 80 );
			SetDamage( 4, 14 );

			

			
			
			
			
			

			SetSkill( SkillName.EvalInt, 85.1, 89.5 );
			SetSkill( SkillName.Magery, 75.1, 80.5 );
			SetSkill( SkillName.MagicResist, 90.1, 95.0 );
			SetSkill( SkillName.Tactics, 65.1, 95.0 );
			SetSkill( SkillName.Wrestling, 85.1, 105.0 );
			SetSkill( SkillName.Meditation, 85.1, 110.0 );

			Fame = 35000;
			Karma = -3500;

			VirtualArmor = 40;

			PackGem();
			PackGem();

			PackReg( 3 );
			PackItem( new Arrow( 10 ) );
			PackGold( 75, 150 );
			PackScroll( 1, 4 );

			AddItem( new Boots( Utility.RandomRedHue() ) );
			AddItem( new Robe( Utility.RandomRedHue() ) );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 2; } }
		public override int Meat{ get{ return 1; } }

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			Item item = aggressor.FindItemOnLayer( Layer.Helm );

			if ( item is NecromaticMask )
			{
                aggressor.Damage(30);
				item.Delete();
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x365 );
				aggressor.SendMessage( "The necromancer destroyed your mask!" );
			}
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is NecromaticMask )
				return false;

			return base.IsEnemy( m );
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			return true;
		}

		private GHCastTimer m_CastTimer;

		public override void OnSpeech( SpeechEventArgs e )
		{
            if (XmlScript.HasTrigger(this, TriggerName.onSpeech) && UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
            {
                return;
            }
            base.OnSpeech( e );
			Mobile from = e.Mobile;

			if ( from.FindItemOnLayer( Layer.Helm ) is NecromaticMask )
			{
				if ( e.Speech.IndexOf( "nwapslleh" ) >= 0 )
				{
					GreaterHealSpell gheal = new GreaterHealSpell( this, null ); // get your spell
					if ( gheal.Cast() ) // if it casts the spell
					{
						m_CastTimer = new GHCastTimer( gheal, from, gheal.GetCastDelay() );
						m_CastTimer.Start();
					}
				}
			}
		}

		private class GHCastTimer : Timer
		{
			private GreaterHealSpell m_Spell;
			private Mobile m_Target;

			public GHCastTimer( GreaterHealSpell spell, Mobile target, TimeSpan castDelay ) : base( castDelay )
			{
				m_Spell = spell;
				m_Target = target;
				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				m_Spell.Target( m_Target );
			}
		}

		public EvilNecromancer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}