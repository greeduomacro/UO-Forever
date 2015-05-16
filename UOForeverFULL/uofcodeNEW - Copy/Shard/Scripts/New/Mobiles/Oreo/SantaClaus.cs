//Customized By boba

using System;
using Server.Items;

namespace Server.Mobiles

              {
              [CorpseName( " corpse of Santa" )]
              public class Santa : BaseCreature
              {
                  [Constructable]
                                    public Santa() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
                            {
                                               Name = "Santa";
					                           Body = 400;
						                       Female = false; 
						                       Hue = 33779;
						                       Title = "Claus";
								       HairItemID = 8253;
								       HairHue = 1155;

                                               //Body = 149; // Uncomment these lines and input values
                                               //BaseSoundID = 0x4B0; // To use your own custom body and sound.
                                               SetStr( 300, 600 );
                                               SetDex( 200, 400 );
                                               SetInt( 100, 200 );
                                               SetHits( 100000, 300000 );
                                               SetDamage( 20, 25 );
                                               
                                               
                                               
                                               

                                               
                                               
                                               
                                               
                                               

			SetSkill( SkillName.Fencing, 100.0, 120.0 );
			SetSkill( SkillName.Macing, 100.0, 120.0 );
			SetSkill( SkillName.MagicResist, 100.0, 120.0 );
			SetSkill( SkillName.Swords, 100.0, 120.0 );
			SetSkill( SkillName.Tactics, 100.0, 120.0 );
			SetSkill( SkillName.Wrestling, 100.0, 120.0 );


			//m_Timer = new TeleportTimer( this );
			//m_Timer.Start();



            Fame = 5000;
            Karma = 5000;
            VirtualArmor = 65;
            
			AddItem( NotCorpseCont( new FancyShirt(33) ) );
			AddItem( NotCorpseCont( new ShortPants(33) ));
			AddItem( NotCorpseCont( new LeatherGloves(33) ) );
			AddItem( NotCorpseCont( new WizardsHat(33) ));
			AddItem( NotCorpseCont( new Boots(33) ) );
			PackGold( 5120, 6130 );
                                 }

            public override void GenerateLoot()
		{
			switch ( Utility.Random( 30 ))
			{
				case 0: PackItem( new Santa2012gifts() ); break;
				case 1: PackItem( new ChristmasCastleAddon() ); break;				
		 }
		}
		
		       // public override bool HasBreath{ get{ return false ; } }
				public override int BreathFireDamage{ get{ return 11; } }
				public override int BreathColdDamage{ get{ return 11; } }
//                public override bool IsScaryToPets{ get{ return true; } }
				public override bool AutoDispel{ get{ return true; } }
                public override bool BardImmune{ get{ return true; } }
                public override bool Unprovokable{ get{ return true; } }
                public override Poison HitPoison{ get{ return Poison. Deadly ; } }
                public override bool AlwaysMurderer{ get{ return true; } }
//				public override bool IsScaredOfScaryThings{ get{ return false; } }






		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)from;

				if ( bc.Controlled || bc.BardTarget == this )
					damage = 0; // Immune to pets and provoked creatures
			}
		}
		private class TeleportTimer : Timer
		{
			private Mobile m_Owner;

			private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				0, -1,
				0,  1,
				1, -1,
				1,  0,
				1,  1
			};

			public TeleportTimer( Mobile owner ) : base( TimeSpan.FromSeconds( 9.0 ), TimeSpan.FromSeconds( 15.0 ) )
			{
				m_Owner = owner;
			}

			protected override void OnTick()
			{
				if ( m_Owner.Deleted )
				{
					Stop();
					return;
				}

				Map map = m_Owner.Map;

				if ( map == null )
					return;

				if ( 0.5 < Utility.RandomDouble() )
					return;

				Mobile toTeleport = null;

				foreach ( Mobile m in m_Owner.GetMobilesInRange( 16 ) )
				{
					if ( m != m_Owner && m.Player && m_Owner.CanBeHarmful( m ) && m_Owner.CanSee( m ) )
					{
						toTeleport = m;
						break;
					}
				}

				if ( toTeleport != null )
				{
					int offset = Utility.Random( 8 ) * 2;

					Point3D to = m_Owner.Location;

					for ( int i = 0; i < m_Offsets.Length; i += 2 )
					{
						int x = m_Owner.X + m_Offsets[(offset + i) % m_Offsets.Length];
						int y = m_Owner.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

						if ( map.CanSpawnMobile( x, y, m_Owner.Z ) )
						{
							to = new Point3D( x, y, m_Owner.Z );
							break;
						}
						else
						{
							int z = map.GetAverageZ( x, y );

							if ( map.CanSpawnMobile( x, y, z ) )
							{
								to = new Point3D( x, y, z );
								break;
							}
						}
					}

					Mobile m = toTeleport;

					Point3D from = m.Location;

					m.Location = to;

					Server.Spells.SpellHelper.Turn( m_Owner, toTeleport );
					Server.Spells.SpellHelper.Turn( toTeleport, m_Owner );

					m.ProcessDelta();

					Effects.SendLocationParticles( EffectItem.Create( from, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
					Effects.SendLocationParticles( EffectItem.Create(   to, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

					m.PlaySound( 0x1FE );

					m_Owner.Combatant = toTeleport;
				}
			}
		}


public Santa( Serial serial ) : base( serial )
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
