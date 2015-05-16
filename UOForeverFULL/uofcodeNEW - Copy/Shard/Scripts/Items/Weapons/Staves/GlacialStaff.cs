using System;
using Server;
using Server.Misc;
using Server.Spells;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	[Flags]
	public enum GlacialSpells : byte
	{
		None			= 0x00,
		Freeze		= 0x01,
		IceStrike		= 0x02,
		IceBall		= 0x04
	}

	public class GlacialStaff : BlackStaff, IUsesRemaining
	{
		public override int ArtifactRarity { get{ return 7; } }
		public override string DefaultName{ get{ return "a glacial staff"; } }
		public override int LabelNumber{ get{ return 0; } }

		private GlacialSpells m_GlacialSpells;
		private GlacialSpells m_CurrentSpell;
		private int m_UsesRemaining;

		public bool ShowUsesRemaining{ get{ return true; } set{} }

    	[CommandProperty( AccessLevel.GameMaster )]
    	public int UsesRemaining{ get{ return m_UsesRemaining; } set{ m_UsesRemaining = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public GlacialSpells CurrentSpell{ get{ return m_CurrentSpell; } set{ m_CurrentSpell = value; } }

		[Constructable]
		public GlacialStaff() : this( 0 )
		{
		}

		[Constructable]
		public GlacialStaff( int usesremaining ) : base()
		{
			Identified = true;
			Hue = 0x480;
			//
			//

			//AosElementDamages[AosElementAttribute.Cold] = 20 + (5 * Utility.RandomMinMax( 0, 6 ));
			UsesRemaining = usesremaining;

			m_GlacialSpells = GetRandomSpells();
		}

		public override int InitMinHits{ get{ return 35; } }
		public override int InitMaxHits{ get{ return 40; } }

		public bool GetFlag( GlacialSpells flag )
		{
			return ( (m_GlacialSpells & flag) != 0 );
		}

		public void SetFlag( GlacialSpells flag, bool value )
		{
			if ( value )
				m_GlacialSpells |= flag;
			else
				m_GlacialSpells &= ~flag;
		}

		public static GlacialSpells GetRandomSpells()
		{
			return (GlacialSpells)(0x07 & ~( 1 << Utility.Random( 1, 2 ) ));
		}

		public override bool HandlesOnSpeech{ get{ return true; } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			base.OnSpeech( e );

			Mobile from = e.Mobile;

			if ( from == Parent && m_UsesRemaining > 0 )
			{
				if ( GetFlag( GlacialSpells.Freeze ) && e.Speech.ToLower().IndexOf( "an ex del" ) > -1 )
				{
					m_CurrentSpell = GlacialSpells.Freeze;
					from.NetState.Send( new PlaySound( 0xF6, from.Location ) );
				}
				else if ( GetFlag( GlacialSpells.IceStrike ) && e.Speech.ToLower().IndexOf( "in corp del" ) > -1 )
				{
					m_CurrentSpell = GlacialSpells.IceStrike;
					from.NetState.Send( new PlaySound( 0xF7, from.Location ) );
				}
				else if ( GetFlag( GlacialSpells.IceBall ) && e.Speech.ToLower().IndexOf( "des corp del" ) > -1 )
				{
					m_CurrentSpell = GlacialSpells.IceBall;
					from.NetState.Send( new PlaySound( 0xF8, from.Location ) );
				}
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			base.OnDoubleClick( from );

			if ( from != Parent )
				from.SendLocalizedMessage( 502641 ); // You must equip this item to use it.
			else if ( m_UsesRemaining <= 0 )
				from.SendLocalizedMessage( 1019073 ); // This item is out of charges.
			else if ( m_CurrentSpell == GlacialSpells.None )
				from.SendMessage( "The magic words must be spoken to activate this staff." ); // You do not have a spell set for this staff.
			else if ( from.Spell != null && from.Spell.IsCasting )
				from.SendLocalizedMessage( 502642 ); // You are already casting a spell.
			else if ( from.Paralyzed || from.Frozen )
				from.SendLocalizedMessage( 502643 ); // You can not cast a spell while frozen.
			else if ( !from.CanBeginAction( typeof( GlacialStaff ) ) )
				from.SendMessage( "You must rest before using the staff again." ); // You must rest before using this staff again.
			else
			{
					from.SendLocalizedMessage( 502014 );
					from.Target = new SpellTarget( this );
			}
		}

		private class SpellTarget : Target
		{
			private GlacialStaff m_Staff;
			private Mobile m_From;
			private Mobile m_To;

			public SpellTarget( GlacialStaff staff ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Staff = staff;
			}
/*
			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
				ReleaseIceLock( from );
				base.OnTargetCancel( from, cancelType );
			}
*/
			protected override void OnTarget( Mobile from, object targeted )
			{
				bool success = false;

				if ( m_Staff != null && !m_Staff.Deleted && m_Staff.UsesRemaining > 0 && from == m_Staff.Parent && targeted is Mobile )
				{
					Mobile to = (Mobile)targeted;
					m_To = to;
					m_From = from;
					if ( !from.CanSee( to ) || !from.InLOS( to ) )
						from.SendLocalizedMessage( 500237 );
					else if ( from.HarmfulCheck( to ) )
					{
						switch( m_Staff.CurrentSpell )
						{
							case GlacialSpells.Freeze: success = DoFreeze( from, to ); break;
							case GlacialSpells.IceStrike: success = DoIceStrike( from, to ); break;
							case GlacialSpells.IceBall: success = DoIceBall( from, to ); break;
						}

						if ( success )
						{
							from.BeginAction( typeof( GlacialStaff ) );
							Timer.DelayCall( TimeSpan.FromSeconds( 7.0 ), new TimerCallback( ReleaseIceLock ) );
							Timer.DelayCall<int>( TimeSpan.FromSeconds( 1.5 ), new TimerStateCallback<int>( ReleaseHueMod ), m_Staff.Hue );
							m_Staff.Hue = 1266;
							--m_Staff.UsesRemaining;
							if ( m_Staff.UsesRemaining <= 0 )
								m_Staff.Delete(); //No message on OSI?
							return;
						}
					}
				}
			}

			private void ReleaseIceLock()
			{
				m_From.EndAction( typeof( GlacialStaff ) );
			}

			private void ReleaseHueMod( int hue )
			{
				m_Staff.Hue = hue;
			}

			private void ReleaseSolidHueOverrideMod( int hue )
			{
				m_To.SolidHueOverride = hue;
			}

			private bool DoFreeze( Mobile from, Mobile to )
			{
				if ( to.Frozen || to.Paralyzed )
					from.SendMessage( "The target is already frozen." );
				else if ( from.Skills[SkillName.Magery].BaseFixedPoint < 300 )
					from.SendMessage( "You are not sure how to use the magic within the staff." );
				else if ( from.Mana < 15 )
					from.SendMessage( "You lack the concentration to use the staff." );
				else
				{
					Mobile caster = from;
					caster.RevealingAction();
					SpellHelper.Turn( caster, to );
					SpellHelper.CheckReflect( (int)SpellCircle.Fifth, ref from, ref to );

					to.Paralyze( TimeSpan.FromSeconds( 7.0 ) );
					Timer.DelayCall<int>( TimeSpan.FromSeconds( 2.5 ), new TimerStateCallback<int>( ReleaseSolidHueOverrideMod ), to.SolidHueOverride );
					to.SolidHueOverride = 1264;
					m_To = to;

					caster.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1008127, caster.NetState );
					to.PlaySound( 0x204 );
					caster.Animate( 218, 7, 1, true, false, 0 );
					Effects.SendTargetEffect( to, 0x376A, 16 );
					m_From.Mana -= 15;
					return true;
				}

				return false;
			}

			private bool DoIceStrike( Mobile from, Mobile to )
			{
				if ( from.Skills[SkillName.Magery].BaseFixedPoint < 300 )
					from.SendMessage( "You are not sure how to use the magic within the staff." );
				else if ( from.Mana < 10 )
					from.SendMessage( "You lack the concentration to use the staff." );
				else
				{
					Mobile caster = from;
					caster.RevealingAction();
					SpellHelper.Turn( caster, to );
					SpellHelper.CheckReflect( (int)SpellCircle.Sixth, ref from, ref to );

					caster.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1008127, caster.NetState );
					caster.PlaySound( 0x208 );
					caster.Animate( 218, 7, 1, true, false, 0 );
					Effects.SendTargetEffect( to, 0x3709, 32,0x47E,3 );
                    to.Damage((int)(caster.Mana * (0.5 + (Utility.RandomDouble() * 0.1))), from);
					caster.Mana = 0;
					return true;
				}
				return false;
			}

			private bool DoIceBall( Mobile from, Mobile to )
			{
				if ( from.Skills[SkillName.Magery].BaseFixedPoint < 300 )
					from.SendMessage( "You are not sure how to use the magic within the staff." );
				else if ( from.Mana < 12 )
					from.SendMessage( "You lack the concentration to use the staff." );
				else
				{
					Mobile caster = from;
					caster.RevealingAction();
					SpellHelper.Turn( caster, to );
					SpellHelper.CheckReflect( (int)SpellCircle.Third, ref from, ref to );

					caster.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 1008127, caster.NetState );
					caster.PlaySound( 0x15E );
					caster.Animate( 218, 7, 1, true, false, 0 );
					Effects.SendMovingEffect( caster, to, 0x36D4, 7, 0, false, true , 0x47E , 3);
					to.Damage( Utility.Random( 10, 6 ), from);
					from.Mana -= 12;
					return true;
				}

				return false;
			}
		}

		public GlacialStaff( Serial serial ) : base( serial )
		{
		}

		public override void OnAdded( object parent )
		{
			Mobile from = parent as Mobile;
			if ( from is PlayerMobile && !( from.FindItemOnLayer( Layer.Gloves ) is FrozenGloves ) && Utility.RandomBool() )
			{
                from.Damage(Utility.Random(5, 11), from);
				from.PlaySound( 0x10B );
				from.SendMessage( "Your fingers freeze as they grip the staff." ); // Your fingers freeze as they grip the staff.
			}

			if ( from != null )
				base.OnAdded( parent );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 );

			writer.WriteEncodedInt( (int)m_GlacialSpells );
			writer.Write( m_UsesRemaining );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			switch ( version )
			{
				case 0:
				{
					m_GlacialSpells = (GlacialSpells)reader.ReadEncodedInt();
					m_UsesRemaining = reader.ReadInt();
					break;
				}
			}
		}
	}
}