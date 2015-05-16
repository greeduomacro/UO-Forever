using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class MoongateHide : BaseGMJewel
	{
		public override bool CastArea{ get{ return true; } }

		public override void HideEffects( Mobile from )
		{
			Effects.PlaySound( from.Location, from.Map, m_GateSound );
			Moongate gate = new Moongate( false );
			if ( m_RedGate )
				gate.ItemID = 0xDDA;
			gate.Hue = m_GateHue;
			gate.MoveToWorld( from.Location, from.Map );
			gate.TargetMap = Map.Internal;

			Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 1.0 ) , new TimerStateCallback<Mobile>( ChangeHide ), from );
			Timer.DelayCall<Moongate>( TimeSpan.FromSeconds( 3.5 ) , new TimerStateCallback<Moongate>( KillGate ), gate );
		}

		public void ChangeHide( Mobile from )
		{
			from.Hidden = !from.Hidden;
		}

		public void KillGate( Moongate gate )
		{
			if ( gate != null && !gate.Deleted )
			{
				Effects.SendLocationParticles( EffectItem.Create( gate.Location, gate.Map, EffectItem.DefaultDuration ), 0x376A, 9, 20, 5042 );
				Effects.PlaySound( gate.Location, gate.Map, 0x201 );
				gate.Delete();
			}
		}

		private int m_GateHue;
		private int m_GateSound;
		private bool m_RedGate;

		[CommandProperty( AccessLevel.GameMaster )]
		public int GateHue{ get{ return m_GateHue; } set{ m_GateHue = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int GateSound{ get{ return m_GateSound; } set{ m_GateSound = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RedGate{ get{ return m_RedGate; } set{ m_RedGate = value; } }

		[Constructable]
		public MoongateHide() : base( AccessLevel.GameMaster, 0x305, 0x1ECD )
		{
			Name = "GM Moongate Ball";
			m_GateSound = 0x20E;
			m_GateHue = 0;
		}
		public MoongateHide( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 1 ); // version

			writer.Write( m_RedGate );
			writer.Write( m_GateHue );
			writer.Write( m_GateSound );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
				{
					m_RedGate = reader.ReadBool();
					goto case 0;
				}
				case 0:
				{
					m_GateHue = reader.ReadInt();
					m_GateSound = reader.ReadInt();
					break;
				}
			}
		}
	}
}