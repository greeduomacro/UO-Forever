//Script Transformed By: Cherokee/Mule II aka. HotShot

using System; 
using System.Collections;
using Server.Items; 
using Server.Mobiles; 
using Server.Misc;
using Server.Network;

namespace Server.Items 
{ 
   	public class DaemonEgg: Item 
   	{ 
		public bool m_AllowEvolution;
		public Timer m_EvolutionTimer;
		private DateTime m_End;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowEvolution
		{
			get{ return m_AllowEvolution; }
			set{ m_AllowEvolution = value; }
		}

		[Constructable]
		public DaemonEgg() : base( 0x0C5D )
		{
			Weight = 0.0;
			Name = "a daemon egg";
			Hue = 38;
			AllowEvolution = false;

			m_EvolutionTimer = new EvolutionTimer( this, TimeSpan.FromDays( 1.0 ) );
			m_EvolutionTimer.Start();
			m_End = DateTime.UtcNow + TimeSpan.FromDays( 1.0 );
		}

            	public DaemonEgg( Serial serial ) : base ( serial ) 
            	{             
           	}

		public override void OnDoubleClick( Mobile from )
		{
                        if ( !IsChildOf( from.Backpack ) ) 
                        { 
                                from.SendMessage( "You must have the daemon's egg in your backpack to hatch it." ); 
                        } 
			else if ( this.AllowEvolution == true )
			{
				this.Delete();
				from.SendMessage( "You are now the proud owner of a daemon hatchling!!" );

				EvolutionDaemon daemon = new EvolutionDaemon();

         			daemon.Map = from.Map; 
         			daemon.Location = from.Location; 

				daemon.Controlled = true;

				daemon.ControlMaster = from;

				daemon.IsBonded = true;
			}
			else
			{
				from.SendMessage( "This egg is not yet ready to be hatched." );
			}
		}

           	public override void Serialize( GenericWriter writer ) 
           	{ 
              		base.Serialize( writer ); 
              		writer.Write( (int) 1 ); 
			writer.WriteDeltaTime( m_End );
           	} 
            
           	public override void Deserialize( GenericReader reader ) 
           	{ 
              		base.Deserialize( reader ); 
              		int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_End = reader.ReadDeltaTime();
					m_EvolutionTimer = new EvolutionTimer( this, m_End - DateTime.UtcNow );
					m_EvolutionTimer.Start();

					break;
				}
				case 0:
				{
					TimeSpan duration = TimeSpan.FromDays( 1.0 );

					m_EvolutionTimer = new EvolutionTimer( this, duration );
					m_EvolutionTimer.Start();
					m_End = DateTime.UtcNow + duration;

					break;
				} 
			}
           	} 

		private class EvolutionTimer : Timer
		{ 
			private DaemonEgg de;

			private int cnt = 0;

			public EvolutionTimer( DaemonEgg owner, TimeSpan duration ) : base( duration )
			{ 
				de = owner;
			}

			protected override void OnTick() 
			{
				cnt += 1;

				if ( cnt == 1 )
				{
					de.AllowEvolution = true;
				}
			}
		}
        } 
} 