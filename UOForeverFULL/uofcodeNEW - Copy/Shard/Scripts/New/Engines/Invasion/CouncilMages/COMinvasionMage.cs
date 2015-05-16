using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Regions;
using Server.Engines.XmlSpawner2;

namespace Server.Engines.Invasion
{
	public class COMInvasionMage : BaseCreature
	{
		public override bool DisallowAllMoves{ get{ return true; } }
		public override bool ShowFameTitle { get { return false; } }

		private COMInvasionPlatform m_Platform;
		public COMInvasionPlatform Platform{ get{ return m_Platform; } set{ m_Platform = value; } }

		private Direction m_Direction;

		public void UpdateDirection()
		{
			Direction = m_Direction;
		}

		//[Constructable]
		public COMInvasionMage( COMInvasionPlatform platform, Direction direction ) : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Platform = platform;
			Blessed = true;
			m_Direction = Direction = direction;

			SpeechHue = Utility.RandomDyedHue();
			Title = "member of the council of mages";
			Hue = Utility.RandomSkinHue();

			Body = 0x190;
			Name = NameList.RandomName( "male" );
			AddItem( new HoodedShroudOfShadows( 1325 ) );

			SetStr( 100 );
			SetDex( 100 );
			SetInt( 100 );

			SetDamage( 85, 100 );

			SetSkill( SkillName.Fencing, 100 );
			SetSkill( SkillName.Macing, 100 );
			SetSkill( SkillName.MagicResist, 100 );
			SetSkill( SkillName.Swords, 100 );
			SetSkill( SkillName.Tactics, 100 );
			SetSkill( SkillName.Wrestling, 100 );
			SetSkill( SkillName.Magery, 100 );
			SetSkill( SkillName.EvalInt, 100 );

			Fame = 1000;
			Karma = -1000;

			AddItem( new Sandals( 1325 ) );
			PackItem( new Gold( 10 ) );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
            // trigger returns true if returnoverride
            if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) && UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, dropped))
                return true;
            if ( m_Platform != null )
			{
				if ( dropped is LichDust )
				{
					if ( m_Platform.LichDust < 60000 )
					{
						m_Platform.LichDust += dropped.Amount;
						dropped.Delete();

						if ( m_Platform.LichDust < 60000 )
							Say( String.Format( "We need {0} more lich's dust!", 60000 - m_Platform.LichDust ) );
						else
							Say( "We have enough lich's dust!" );
					}
					else
					{
						Say( "We do not need lich's dust!" );
						return false;
					}
				}
				else if ( dropped is BoneAsh )
				{
					if ( m_Platform.BoneAsh < 60000 )
					{
						m_Platform.BoneAsh += dropped.Amount;
						dropped.Delete();

						if ( m_Platform.BoneAsh < 60000 )
							Say( String.Format( "We need {0} more ash from bones!", 60000 - m_Platform.BoneAsh ) );
						else
							Say( "We have enough ash from bones!" );
					}
					else
					{
						Say( "We do not need ash from bones!" );
						return false;
					}
				}
				else if ( dropped is DemonicBoneAsh )
				{
					if ( m_Platform.DemonicBoneAsh < 2000 )
					{
						m_Platform.DemonicBoneAsh += dropped.Amount;
						dropped.Delete();

						if ( m_Platform.DemonicBoneAsh < 2000 )
							Say( String.Format( "We need {0} more ash from demonic bones!", 2000 - m_Platform.DemonicBoneAsh ) );
						else
							Say( "We have enough ash from demonic bones!" );
					}
					else
					{
						Say( "We do not need ash from demonic bones!" );
						return false;
					}
				}
				else if ( dropped is EtherealResidue )
				{
					if ( m_Platform.EtherealResidue < 60000 )
					{
						m_Platform.EtherealResidue += dropped.Amount;
						dropped.Delete();

						if ( m_Platform.EtherealResidue < 60000 )
							Say( String.Format( "We need {0} more ethereal residue!", 60000 - m_Platform.EtherealResidue ) );
						else
							Say( "We have enough ethereal residue!" );
					}
					else
					{
						Say( "We do not need ethereal residue!" );
						return false;
					}
				}
				else
				{
					Say( "You insult us with that!" );
					return false;
				}

				Animate( 32, 5, 1, true, false, 0 );
				return true;
			}

			return false;
		}

		public override void OnRegionChange( Region Old, Region New )
		{
			base.OnRegionChange( Old, New );

			GuardedRegion oldReg = Old.GetRegion( typeof( GuardedRegion ) ) as GuardedRegion;
			GuardedRegion newReg = New.GetRegion( typeof( GuardedRegion ) ) as GuardedRegion;

			if ( oldReg != null && !oldReg.IsPartOf( "Buccaneer's Den" ) && !oldReg.IsPartOf( "Wind" ) ) //moved OUT of a guarded town region
				oldReg.Disabled = false; //Enable gaurds

			if ( newReg != null )
				newReg.Disabled = true;
		}

		public COMInvasionMage( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( (int)m_Direction );

			writer.Write( m_Platform );

			if ( m_Platform != null )
				writer.Write( new Point3D( Location.X - m_Platform.X, Location.Y - m_Platform.Y, Location.Z - m_Platform.Z ) );
			else
				writer.Write( new Point3D( 0, 0, 0 ) );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Direction = (Direction)reader.ReadInt();

			Direction = m_Direction;

			m_Platform = reader.ReadItem<COMInvasionPlatform>();

			if ( m_Platform != null )
				m_Platform.Mages.Add( new Tuple<COMInvasionMage,Point3D>( this, reader.ReadPoint3D() ) );
		}
	}
}