using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Misc
{
	public class TrickorTreatPersistence : Item
	{
		private static TrickorTreatPersistence m_Instance;
		public static TrickorTreatPersistence Instance{ get{ return m_Instance; } }

		//Keep track of the NPCs that the players have gotten treats from.
		//This includes tricks too!
		private Dictionary<Mobile, List<Mobile>> m_Treats;
		public Dictionary<Mobile, List<Mobile>> Treats{ get{ return m_Treats; } }

		//Candy can be eaten at any time, so we make this static.
		//We also don't care to serialize this information.
		private static Dictionary<Mobile, CandyTimer> m_ToothPains = new Dictionary<Mobile, CandyTimer>();

		public static bool GaveTreat( Mobile from, Mobile vendor )
		{
			if ( m_Instance.m_Treats != null && from != null && vendor != null)
			{
				List<Mobile> list;
				m_Instance.m_Treats.TryGetValue( from, out list );
				return list != null && list.Contains( vendor );
			}

			return false;
		}

		public static void AteCandy( Mobile from )
		{
			CandyTimer timer;
			m_ToothPains.TryGetValue( from, out timer );
			
			if ( timer == null )
			{
				from.SendLocalizedMessage( 1077387 ); // You feel as if you could eat as much as you wanted!
				timer = new CandyTimer( from );
				m_ToothPains.Add( from, timer );
				timer.Start();
			}
			else
				timer.ToothacheLevel++;
		}

		public static void AteApple( Mobile from )
		{
			CandyTimer timer;
			m_ToothPains.TryGetValue( from, out timer );
			
			if ( timer != null )
			{
				timer.ToothacheLevel = 0;
				timer.Stop();
				m_ToothPains.Remove( from );
				from.SendLocalizedMessage( 1077393 ); // The extreme pain in your teeth subsides.
			}
		}

		public static bool TrickTreat( Mobile vendor, Mobile from )
		{
			if ( vendor != null && from != null )
			{
				HalloweenMask mask = from.FindItemOnLayer( Layer.FirstValid ) as HalloweenMask;
				if ( mask != null )
				{
					if ( GaveTreat( from, vendor ) )
						from.SendMessage( "You already received a gift from this vendor." );
					else
					{
						double beg = from.Skills[SkillName.Begging].Value / 100.0;
						bool success = from.CheckTargetSkill( SkillName.Begging, vendor, 0.0, 120.0 );
						//trick chance
						double tc = (0.01 * mask.Rarity) + (success ? (beg * 0.08) : 0.0);

						if ( (0.16-tc) > Utility.RandomDouble() ) //better tc, lower chance to be tricked
						{
							//do a trick
							switch ( Utility.Random( 3 ) )
							{
								case 0: BleedTrick( from ); break;
							}
						}
						else
						{
							//Item chance is 5% if successful, otherwise 2.5%
							double ic = success ? (beg * 0.05) : (beg * 0.025);
							if ( ic > Utility.RandomDouble() ) // You scored a special item!
							{
								//Of the items, there is a 40% chance of a rare, otherwise no chance
								if ( success && (beg * 0.40) > Utility.RandomDouble() )
								{
									//Give rare item
								}
								else
								{
									//Give special/unrare item
								}
							}
							else
							{
								//At GM begging 15% if successful, otherwise 5%
								double gtc = success ? (beg * 0.15) : 0.05;
								if ( gtc > Utility.RandomDouble() )
								{
									//Give good treat
								}
								else
								{
									//Give common treat
								}
							}
						}

						return true;
					}
				}
			}

			return false;
		}

		public static void BleedTrick( Mobile from )
		{
			Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 2.5 ), TimeSpan.FromSeconds( 2.5 ), 5, new TimerStateCallback<Mobile>( BleedCallback ), from );
		}

		public static void BleedCallback( Mobile from )
		{
			if ( !from.Mounted )
				from.Animate( 32, 5, 1, true, false, 0 );

			from.PlaySound( from.GetHurtSound() );

			int count = Utility.Random( 1, 3 );
			for ( int i = 0; i < count; i++ )
				BaseWeapon.CreateBlood( from.Location, from.Map, from.BloodHue, Utility.RandomBool() );
		}

		public override string DefaultName
		{
			get { return "Trick or Treat Persistence - Internal"; }
		}

		public static void Configure()
		{
			//EventSink.WorldSave += new WorldSaveEventHandler( Save );
			//EventSink.WorldLoad += new WorldLoadEventHandler( Load );
		}

		public static void Load()
		{
			if ( m_Instance == null && BaseHalloweenGiftGiver.IsHalloween() )
				new TrickorTreatPersistence();
		}

		public static void Save( WorldSaveEventArgs e )
		{
			if ( BaseHalloweenGiftGiver.IsHalloween() )
			{
				if ( m_Instance == null )
					new TrickorTreatPersistence();
			}
			else if ( m_Instance != null )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( m_Instance.DeleteInternal ) );
		}

		public TrickorTreatPersistence() : base( 1 )
		{
			Movable = false;

			if ( m_Instance == null || m_Instance.Deleted )
			{
				m_Instance = this;
				m_Treats = new Dictionary<Mobile, List<Mobile>>();
				//Do all of the introductory stuff
			}
			else
				DeleteInternal();
		}

		public TrickorTreatPersistence( Serial serial ) : base( serial )
		{
			m_Instance = this;
			m_Treats = new Dictionary<Mobile, List<Mobile>>();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			if ( BaseHalloweenGiftGiver.IsHalloween( DateTime.UtcNow ) )
			{
				writer.Write( true );
				writer.Write( m_Treats.Count );
				foreach ( KeyValuePair<Mobile, List<Mobile>> kvp in m_Treats )
				{
					writer.Write( kvp.Key );
					writer.Write( kvp.Value, true );
				}
			}
			else
			{
				writer.Write( false );
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( DeleteInternal ) );
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( reader.ReadBool() )
			{
				int count = reader.ReadInt(); //Dictionary count
				for ( int i = 0; i < count; i++ )
					m_Treats.Add( reader.ReadMobile(), reader.ReadStrongMobileList() );
			}

			if ( !BaseHalloweenGiftGiver.IsHalloween() )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( DeleteInternal ) );
		}

		private void DeleteInternal()
		{
			base.Delete();
		}

		public override void Delete()
		{
		}

		private class CandyTimer : Timer
		{
			private Mobile m_From;
			private int m_ToothacheLevel;
			private DateTime m_NextAcheCheck;

			public int ToothacheLevel { get{ return m_ToothacheLevel; } set{ m_ToothacheLevel = value; } }

			public CandyTimer( Mobile from )
				: base( TimeSpan.FromSeconds( 15.0 ), TimeSpan.FromSeconds( 15.0 ) )
			{
				m_From = from;
				m_ToothacheLevel = 1;
				m_NextAcheCheck = DateTime.UtcNow + TimeSpan.FromMinutes( 3.0 + ( Utility.RandomDouble() * 1.5 ) );
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				if ( m_ToothacheLevel > 2 ) // 3 candies or more, then its painful!
				{
					if ( m_From.Hits > m_ToothacheLevel )
						m_From.Damage( m_ToothacheLevel, m_From );
					m_From.PlaySound( m_From.GetHurtSound() );

					if ( !m_From.Mounted )
						m_From.Animate( 32, 5, 1, true, false, 0 );
					
					if ( m_ToothacheLevel > 6 )
						m_From.Say( Utility.RandomList( 1077388, 1077391, 1077392 ) );
					else
						m_From.Say( Utility.Random( 4 ) + 1077388 );
				}

				if ( DateTime.UtcNow >= m_NextAcheCheck )
				{
					if ( m_ToothacheLevel == 2 )
						m_From.SendLocalizedMessage( 1077393 ); // The extreme pain in your teeth subsides.

					m_ToothacheLevel--;

					if ( m_ToothacheLevel <= 0 )
					{
						Stop();
						TrickorTreatPersistence.m_ToothPains[m_From] = null;
					}
					else
						m_NextAcheCheck = DateTime.UtcNow + TimeSpan.FromMinutes( 3.0 + ( Utility.RandomDouble() * 1.5 ) );
				}
			}
		}
	}
}