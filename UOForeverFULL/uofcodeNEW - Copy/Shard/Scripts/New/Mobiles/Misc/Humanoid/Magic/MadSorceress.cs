using System;
using System.Collections;
using Server.Items;
using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Spells;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles
{
	public class MadSorceress : BaseCreature
	{
		public override bool IsScaryToPets{ get{ return true; } }
		public override string DefaultName{ get{ return "a mad sorceress"; } }

		[Constructable]
		public MadSorceress() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.2)
		{
			Hue = 33770;
			Body = 401;

			SetStr( 175, 275 );
			SetDex( 80, 95 );
			SetInt( 250, 300 );

			SetHits(250, 325);
			SetMana(350, 450);

			SetDamage( 7, 12 );

			SetSkill( SkillName.Tactics, 95.7, 98.4);
			SetSkill( SkillName.MagicResist, 77.4, 87.7);
			SetSkill( SkillName.Magery, 98.4, 99.7);
			SetSkill( SkillName.Parry, 97.4, 98.7);
			SetSkill( SkillName.Wrestling, 89.4, 95.7);
			SetSkill( SkillName.EvalInt, 75.4, 80.7);

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 10;
			Female = true;

			Item BronzeShield = new BronzeShield();
			BronzeShield.Movable = false;
			BronzeShield.Hue = 1156;
			EquipItem( BronzeShield );

			Item WizardsHat = new WizardsHat();
			WizardsHat.Movable=false;
			WizardsHat.Hue = 1153;
			EquipItem( WizardsHat );

			Item LeatherGloves = new LeatherGloves();
			LeatherGloves.Movable = false;
			LeatherGloves.Hue = 1153;
			EquipItem( LeatherGloves );

			Item Robe = new Robe();
			Robe.Movable = false;
			Robe.Hue = 1156;
			EquipItem( Robe );

			Item Sandals = new Sandals();
			Sandals.Movable = false;
			Sandals.Hue = 1153;
			EquipItem( Sandals );

			Item GoldEarrings = new GoldEarrings();
			GoldEarrings.Movable = false;
			GoldEarrings.Hue = 1153;
			EquipItem( GoldEarrings );

			Item GoldNecklace = new GoldNecklace();
			GoldNecklace.Movable = false;
			GoldNecklace.Hue = 1153;
			EquipItem( GoldNecklace );

			HairItemID = 0x203C;
			HairHue = 1156;

			int gems = Utility.RandomMinMax( 1, 5 );

			for ( int i = 0; i < gems; ++i )
				PackGem();

			switch ( Utility.Random( 6 ) )
			{
				case 0: PackItem( new BlackPearl( Utility.RandomMinMax( 10, 15 ) ) ); break;
				case 1: PackItem( new MandrakeRoot( Utility.RandomMinMax( 10, 15 ) ) ); break;
				case 2: PackItem( new SulfurousAsh( Utility.RandomMinMax( 10, 15 ) ) ); break;
			}

			PackGold( 200, 250 );
			PackScroll( 1, 8 );
			PackSlayer();

			switch ( Utility.Random( 10 ) )
			{
				case 0: PackItem( new MortarPestle() ); break;
				case 1: PackItem( new GreaterExplosionPotion() ); break;
			}

			switch ( Utility.Random( 2 ) )
			{
				case 0: PackWeapon( 0, 5 ); break;
				case 1: PackArmor( 0, 5 ); break;
			}

			switch ( Utility.Random( 3 ) )
			{
				case 0: PackWeapon( 0, 5 ); break;
				case 1: PackArmor( 0, 5 ); break;
			}

			switch ( Utility.Random( 4 ) )
			{
				case 0: PackWeapon( 1, 5 ); break;
				case 1: PackArmor( 1, 5 ); break;
			}
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override bool AutoDispel{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
	//	public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if ( to is BaseCreature )
				damage *= 10;
		}

		private DateTime m_NextBomb;
		private int m_Thrown;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.UtcNow >= m_NextBomb )
			{
				ThrowBomb( combatant );

				m_Thrown++;

				if ( 0.75 >= Utility.RandomDouble() && (m_Thrown % 2) == 1 ) // 75% chance to quickly throw another bomb
					m_NextBomb = DateTime.UtcNow + TimeSpan.FromSeconds( 2.0 );
				else
					m_NextBomb = DateTime.UtcNow + TimeSpan.FromSeconds( 4.0 + (9.0 * Utility.RandomDouble()) ); // 4-13 seconds
			}
		}

		public void ThrowBomb( Mobile m )
		{
			DoHarmful( m );

			this.MovingParticles( m, 0x1C19, 1, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );

			new InternalTimer( m, this ).Start();
		}

		private class InternalTimer : Timer
		{
			private Mobile m_Mobile, m_From;

			public InternalTimer( Mobile m, Mobile from ) : base( TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_From = from;
			}

			protected override void OnTick()
			{
				m_Mobile.PlaySound( 0x11D );
                m_Mobile.Damage(Utility.RandomMinMax(35, 50), m_From);
			}
		}

		public MadSorceress( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
		public override void OnDeath( Container c )
		{
/*
			Item item = null;
			switch( Utility.Random(500) )
				{
			case 0: c.DropItem( item = new Item(0xFA2) ); break;
			case 1: c.DropItem( item = new Item(0xFA3) ); break;
			case 2: c.DropItem( item = new Item(0xE18) ); break;
			case 3: c.DropItem( item = new Item(0xE19) ); break;
			case 4: c.DropItem( item = new Item(0xE15) ); break;
			case 5: c.DropItem( item = new Item(0xE16) ); break;
			        }
*/
			base.OnDeath( c );
		}
	}
}