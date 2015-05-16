using System;
using Server.Mobiles;
using Server.Commands;

namespace Server.ContextMenus
{
	public class GMRevivifyEntry : ContextMenuEntry
	{
		private Mobile m_Target;

		public GMRevivifyEntry( Mobile target ) : base( 6243, 12 )
		{
			m_Target = target;
			Color = 0xFFD80;
		}

		public override void OnClick()
		{
			m_Target.PlaySound( 0x214 );
			m_Target.FixedEffect( 0x376A, 10, 16 );

			if ( m_Target.IsDeadBondedPet )
			{
				BaseCreature bc = m_Target as BaseCreature;
				
				if ( bc != null ) // sanity check
					bc.ResurrectPet();
			}
			else if ( !m_Target.CheckAlive() )
				m_Target.Resurrect();

			m_Target.Hits = m_Target.HitsMax;
			m_Target.Stam = m_Target.StamMax;
			m_Target.Mana = m_Target.ManaMax;

			CommandLogging.WriteLine( Owner.From, "{0} {1} resurrecting {2}", Owner.From.AccessLevel, CommandLogging.Format( Owner.From ), CommandLogging.Format( m_Target ) );
		}
	}
}