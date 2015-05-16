using System;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Targets
{
	public class MoveReagentsTarget : Target
	{
		private BaseContainer m_Container;
		private int m_Amount;

		public MoveReagentsTarget( BaseContainer cont, int amount ) : base( 10, true, TargetFlags.None )
		{
			m_Container = cont;
			m_Amount = amount;
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if ( o is BaseReagent )
			{
				BaseReagent reg = o as BaseReagent;
				if ( reg.Movable && reg.IsChildOf( from ) )
				{
					if ( !reg.OnDroppedOnto( from, m_Container ) )
						from.SendMessage( "You cannot move that into the container." );
				}
				else
					from.SendMessage( "That must be in your backpack or bankbox to move it." );
			}
			else if ( o is BaseContainer )
			{
				BaseContainer to = o as BaseContainer;

				if ( to.IsChildOf( from ) )
				{
					bool movedItems = true;
					for ( int i = to.Items.Count-1;i >= 0; i-- )
					{
						if ( to.Items[i] is BaseReagent )
						{
							BaseReagent reg = to.Items[i] as BaseReagent;
							if ( reg != null )
							{
								if ( reg.Movable )
								{
									if ( m_Amount < 0 || reg.Amount <= m_Amount )
									{
										if ( !reg.OnDroppedOnto( from, m_Container ) )
											movedItems = false;
									}
									else
									{
										BaseReagent newreg = Activator.CreateInstance( reg.GetType() ) as BaseReagent;
										newreg.Amount = m_Amount;
										if ( !newreg.OnDroppedOnto( from, m_Container ) )
										{
											movedItems = false;
											newreg.Delete();
										}
										else
											reg.Consume( m_Amount );
									}
								}
								else
									movedItems = false;
							}
						}
					}

					if ( !movedItems )
						from.SendMessage( "Some reagents could not be moved." );
				}
				else
					from.SendMessage( "That must be in your backpack or bank box to move reagents from it." );
			}
		}
	}
}