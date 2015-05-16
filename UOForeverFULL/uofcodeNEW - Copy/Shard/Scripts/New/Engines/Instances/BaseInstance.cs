using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Regions;
using Server.Mobiles;

namespace Server.Engines.Instances
{
	public abstract class BaseInstance : BaseInstanceMap
	{
		public static void Configure()
		{
			EventSink.PlayerDeath += new PlayerDeathEventHandler( OnPlayerDeath );
		}

		//private InstanceType m_Type;

		private DeleteTimer m_DeleteTimer;
		private DateTime m_ExpireDate;

		private Point3D m_ReturnPoint;
		private Map m_ReturnMap;

		private Point3D m_DestPoint;
		private Map m_DestMap;

		//private bool m_Deleted;

		public TimeSpan ExpireDate{ get{ return m_ExpireDate - DateTime.UtcNow; } }
		public bool Expired{ get{ return DateTime.UtcNow >= m_ExpireDate; } }

		public Point3D ReturnPoint{ get{ return m_ReturnPoint; } set{ m_ReturnPoint = value; } }
		public Map ReturnMap{ get{ return m_ReturnMap; } set{ m_ReturnMap = value; } }

		public Point3D DestPoint{ get{ return m_DestPoint; } set{ m_DestPoint = value; } }
		public Map DestMap{ get{ return m_DestMap; } set{ m_DestMap = value; } }

		public BaseInstance(Map map, string name, Point3D retloc, Map retmap) : this( map, name, retloc, retmap, DateTime.MaxValue )
		{
		}

		public BaseInstance(Map map, string name, Point3D retloc, Map retmap, DateTime exp) : base( map, name, MapRules.FeluccaRules )
		{
			m_ExpireDate = exp;
			m_ReturnPoint = retloc;
			m_ReturnMap = retmap;

			if ( m_ExpireDate < DateTime.MaxValue )
			{
				m_DeleteTimer = new DeleteTimer(this, m_ExpireDate - DateTime.UtcNow);
				m_DeleteTimer.Start();
			}
		}

		public BaseInstance( Serial serial ) : base( serial )
		{
		}

		public void RemoveInstance()
		{
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m.Player && (m.Map == this || m.LogoutMap == this) )
				{
					if ( m.NetState != null )
						m.MoveToWorld( m_ReturnPoint, m_ReturnMap );
					else
					{
						m.LogoutMap = m_ReturnMap;
						m.LogoutLocation = m_ReturnPoint;
					}
				}
			}

			if ( m_DeleteTimer != null )
			{
				m_DeleteTimer.Stop();
				m_DeleteTimer = null;
			}

			Delete();
		}

		public override  void Delete()
		{
			RemoveInstance();

			base.OnDelete();
		}

		private class DeleteTimer : Timer
		{
			private BaseInstance m_Instance;

			public DeleteTimer(BaseInstance instance, TimeSpan dur) : base(dur)
			{
				m_Instance = instance;
			}

			protected override void OnTick()
			{
				m_Instance.RemoveInstance();
				Stop();
			}
		}

		private static void OnPlayerDeath( PlayerDeathEventArgs e )
		{
			new DeathTimer( e.Mobile ).Start();
		}

		private class DeathTimer : Timer
		{
			public static TimeSpan DeathDelay =  TimeSpan.FromSeconds( 30 );//changed timer to 6 sec from 30

			private Mobile m_From;

			public DeathTimer( Mobile m ) : base( DeathDelay )
			{
				m_From = m;

				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				if ( !m_From.Alive && m_From.Map is BaseInstance ) //Does it have to be the same map that they died on?
				{
					BaseInstance instance = m_From.Map as BaseInstance;

					m_From.MoveToWorld( instance.DestPoint, instance.DestMap );
					m_From.Resurrect();
					m_From.Hits = m_From.HitsMax;
					m_From.Mana = m_From.ManaMax;
					m_From.Stam = m_From.StamMax;
					//A death message?
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

			writer.Write( (int) 0 ); // version

			writer.WriteDeltaTime( m_ExpireDate );
			writer.Write( m_ReturnPoint );
			writer.Write( m_ReturnMap );
			writer.Write( m_DestPoint );
			writer.Write( m_DestMap );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			m_ExpireDate = reader.ReadDeltaTime();
			m_ReturnPoint = reader.ReadPoint3D();
			m_ReturnMap = reader.ReadMap();
			m_DestPoint = reader.ReadPoint3D();
			m_DestMap = reader.ReadMap();

			if ( m_ExpireDate < DateTime.MaxValue )
			{
				m_DeleteTimer = new DeleteTimer( this, m_ExpireDate - DateTime.UtcNow );
				m_DeleteTimer.Start();
			}
		}
	}
}
