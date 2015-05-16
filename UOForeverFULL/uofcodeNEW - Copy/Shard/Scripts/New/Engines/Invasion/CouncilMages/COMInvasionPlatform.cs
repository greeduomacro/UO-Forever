using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Regions;

namespace Server.Engines.Invasion
{
	public class COMInvasionPlatform : BaseAddon
	{
		private int m_BoneAsh;
		private int m_DemonicBoneAsh;
		private int m_EtherealResidue;
		private int m_LichDust;

		[CommandProperty( AccessLevel.GameMaster )]
		public int BoneAsh{ get{ return m_BoneAsh; } set{ m_BoneAsh = value; UpdateStatus(); } }
		[CommandProperty( AccessLevel.GameMaster )]
		public int DemonicBoneAsh{ get{ return m_DemonicBoneAsh; } set{ m_DemonicBoneAsh = value; UpdateStatus(); } }
		[CommandProperty( AccessLevel.GameMaster )]
		public int EtherealResidue{ get{ return m_EtherealResidue; } set{ m_EtherealResidue = value; UpdateStatus(); } }
		[CommandProperty( AccessLevel.GameMaster )]
		public int LichDust{ get{ return m_LichDust; } set{ m_LichDust = value; UpdateStatus(); } }

		private List<Tuple<COMInvasionMage,Point3D>> m_Mages;
		public List<Tuple<COMInvasionMage,Point3D>> Mages{ get{ return m_Mages; } }

		public bool Full{ get{ return m_BoneAsh >= 60000 && m_DemonicBoneAsh >= 2000 && m_EtherealResidue >= 60000 && m_LichDust >= 60000; } }

		public void UpdateStatus()
		{
			if ( Full && m_Mages.Count > 0 )
				Timer.DelayCall( TimeSpan.FromSeconds( 3.0 ) , new TimerCallback( Stage1 ) );
		}

		public void Stage1()
		{
			COMInvasionMage randomMage = m_Mages[Utility.Random( m_Mages.Count )].Item1;
			randomMage.Say( "We cannot keep the portal open much longer!" );

			Timer.DelayCall( TimeSpan.FromSeconds( 4.0 ) , new TimerCallback( Stage2 ) );
		}

		public void Stage2()
		{
			COMInvasionMage randomMage = m_Mages[Utility.Random( m_Mages.Count )].Item1;
			randomMage.Say( "We must leave immediately!" );

			Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ) , new TimerCallback( Stage3 ) );
		}

		public void Stage3()
		{
			//Gate out the Mages
			foreach ( Tuple<COMInvasionMage,Point3D> tuple in m_Mages )
			{
				COMInvasionMage mage = tuple.Item1;
				GateHide hideball = new GateHide();
				hideball.AccessLevel = AccessLevel.Player;

				mage.PlaceInBackpack( hideball );
				mage.Use( hideball );
				Timer.DelayCall( TimeSpan.FromSeconds( 10.0 ) , new TimerCallback( mage.Delete ) );
			}

			m_Mages.Clear();

			Timer.DelayCall( TimeSpan.FromSeconds( 4.0 ) , new TimerCallback( Stage4 ) );
		}

		public void Stage4()
		{
			COMInvasionBoss boss = new COMInvasionBoss();
			GateHide hideball = new GateHide();
			hideball.AccessLevel = AccessLevel.Player;
			hideball.RedGate = true;
			boss.PlaceInBackpack( hideball );
			boss.Blessed = true;
			boss.CantWalk = true;
			boss.Hidden = true;
			boss.MoveToWorld( new Point3D( 600, 2133, 0 ), Map.Felucca );
			boss.Use( hideball );
			Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 8.0 ) , new TimerStateCallback<Mobile>( Stage5 ), boss );
		}

		public void Stage5( Mobile boss )
		{
			boss.Blessed = false;
			boss.Hidden = false;
			boss.CantWalk = false;
			boss.Say( "Fools!! I will destroy Sosaria!" );
		}

		[Constructable]
		public COMInvasionPlatform()
		{
			m_Mages = new List<Tuple<COMInvasionMage,Point3D>>();

			//Orb Platform
			AddComponent( 0x3657, 0, 0, 5 );
			AddComponent( 0x3658, -1, 0, 5 );
			AddComponent( 0x3659, 0, -1, 5 );

			//Orb
			AddComponent( 0x3660, 0, 0, 5, 1310 );

			//East Stairs
			AddComponent( 0x76E, 1, 0, 0, 1072 );
			AddComponent( 0x76E, 1, -1, 0, 1072 );
			AddComponent( 0x778, 1, -2, 0, 1072 );

			//North Stairs
			AddComponent( 0x76F, 0, -2, 0, 1072 );
			AddComponent( 0x76F, -1, -2, 0, 1072 );
			AddComponent( 0x775, -2, -2, 0, 1072 );

			//West Stairs
			AddComponent( 0x770, -2, -1, 0, 1072 );
			AddComponent( 0x770, -2, 0, 0, 1072 );
			AddComponent( 0x777, -2, 1, 0, 1072 );

			//South Stairs
			AddComponent( 0x76D, -1, 1, 0, 1072 );
			AddComponent( 0x76D, 0, 1, 0, 1072 );
			AddComponent( 0x776, 1, 1, 0, 1072 );

			m_Mages.Add( new Tuple<COMInvasionMage,Point3D>( new COMInvasionMage( this, Direction.Down ), new Point3D( -3, -3, 0 ) ) );
			m_Mages.Add( new Tuple<COMInvasionMage,Point3D>( new COMInvasionMage( this, Direction.Left ), new Point3D( 2, -3, 0 ) ) );
			m_Mages.Add( new Tuple<COMInvasionMage,Point3D>( new COMInvasionMage( this, Direction.Up ), new Point3D( 2, 2, 0 ) ) );
			m_Mages.Add( new Tuple<COMInvasionMage,Point3D>( new COMInvasionMage( this, Direction.Right ), new Point3D( -3, 2, 0 ) ) );
		}

		public void AddComponent( int id, int x, int y, int z )
		{
			AddComponent( id, x, y, z, 0 );
		}

		public void AddComponent( int id, int x, int y, int z, int hue )
		{
			AddonComponent ac = new AddonComponent( id );

			ac.Hue = hue;

			AddComponent( ac, x, y, z );
		}

		public override void OnLocationChange( Point3D oldLoc )
		{
			base.OnLocationChange( oldLoc );

			if ( Deleted )
				return;

			foreach ( Tuple<COMInvasionMage,Point3D> tuple in m_Mages )
			{
				tuple.Item1.Location = new Point3D( X + tuple.Item2.X, Y + tuple.Item2.Y, Z + tuple.Item2.Z );
				tuple.Item1.UpdateDirection();
			}
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			if ( Deleted )
				return;

			foreach ( Tuple<COMInvasionMage,Point3D> tuple in m_Mages )
			{
				tuple.Item1.Map = Map;
				tuple.Item1.UpdateDirection();
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			GuardedRegion reg = Region.Find( Location, Map ).GetRegion( typeof( GuardedRegion ) ) as GuardedRegion;

			if ( reg != null && !reg.IsPartOf( "Buccaneer's Den" ) && !reg.IsPartOf( "Wind" ) ) //moved OUT of a guarded town region
				reg.Disabled = false; //Enable gaurds
		}

		public COMInvasionPlatform( Serial serial ) : base( serial )
		{
			m_Mages = new List<Tuple<COMInvasionMage,Point3D>>();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 1 ); // version

			writer.Write( m_BoneAsh );
			writer.Write( m_DemonicBoneAsh );
			writer.Write( m_EtherealResidue );
			writer.Write( m_LichDust );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			if ( version >= 1 )
			{
				m_BoneAsh = reader.ReadInt();
				m_DemonicBoneAsh = reader.ReadInt();
				m_EtherealResidue = reader.ReadInt();
				m_LichDust = reader.ReadInt();
			}
		}
	}
}