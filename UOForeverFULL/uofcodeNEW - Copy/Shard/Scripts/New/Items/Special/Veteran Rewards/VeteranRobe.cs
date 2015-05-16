using System;
using Server;
using Server.Mobiles;
using Server.Accounting;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class VeteranRobe : Robe, IVeteranClothing, IRewardItem
	{
		public bool IsRewardItem{ get{ return true; } set{} }
		
		private int m_LabelNumber;
		public override int LabelNumber
		{
			get
			{
				if ( m_LabelNumber > 0 )
					return m_LabelNumber;

				return base.LabelNumber;
			}
		}

		private byte m_Level;

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public int Number
		{
			get{ return m_LabelNumber; }
			set{ m_LabelNumber = value; InvalidateProperties(); }
		}

		[Constructable]
		public VeteranRobe() : base( 0 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		private void SetAttributes()
		{
			SetAttributes( true );
		}

		private void SetAttributes( bool message )
		{
			PlayerMobile from = null;
			Account acct = null;

			if ( Parent is PlayerMobile )
			{
				from = Parent as PlayerMobile;
				acct = from.Account as Account;
			}

			if ( acct != null )
			{
				m_Level = (byte)RewardSystem.GetRewardLevel( acct );
				if ( m_Level > 0 )
				{
					RewardLabelHue labelhue = RewardSystem.LabelHues[m_Level];
					m_LabelNumber = labelhue.RobeLabel;
					Hue = labelhue.Hue;

					AddSkillGainMod( from, message );
				}
				else
				{
					m_Level = 0;
					m_LabelNumber = 0;
					Hue = 0;
				}
			}
		}

		private void AddSkillGainMod( PlayerMobile pm, bool message )
		{
			pm.AddSkillGainMod( "VeteranSkillGainMod-RobeDress", (SkillName)(-1), 0.05 * m_Level, TimeSpan.Zero );

			if ( message )
			{
				string msg = String.Empty;
				switch ( m_Level )
				{
					case 10: default: msg = "quite drastically"; break;
					case 8: case 9: msg = "drastically"; break;
					case 6: case 7: msg = "quite a bit"; break;
					case 4: case 5: msg = "somewhat"; break;
					case 2: case 3: msg = "meagerly"; break;
					case 1: msg = "a little bit"; break;
				}

				pm.SendMessage( String.Format( "Your ability to learn new skills has increased {0}.", msg ) );
			}
		}

		private void RemoveSkillGainMod( PlayerMobile pm, bool message )
		{
			pm.RemoveSkillGainMod( "VeteranSkillGainMod-RobeDress" );

			if ( message )
				pm.SendMessage( "Your ability to learn new skills has normalized." );
		}

		public override void OnAdded( object parent )
		{
			SetAttributes();
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is PlayerMobile )
				RemoveSkillGainMod( (PlayerMobile)parent, m_Level > 0 );
		}

		public VeteranRobe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			if ( Parent is PlayerMobile )
				((PlayerMobile)Parent).RemoveSkillGainMod( "VeteranSkillGainMod-RobeDress" );

			SetAttributes( false );

			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			SetAttributes( false );
		}
	}
}