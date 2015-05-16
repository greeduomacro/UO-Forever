using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Network
{
	public class BandagePacket
	{
		public static void Initialize()
		{
			PacketHandlers.RegisterExtended( 0x2C, true, new OnPacketReceive( BandageRequest ) );
		}

		public static void BandageRequest( NetState state, PacketReader pvSrc )
		{
			Mobile from = state.Mobile;

			if ( from.Alive )
			{
				if ( from.AccessLevel >= AccessLevel.Counselor || DateTime.UtcNow >= from.NextActionTime )
				{
					Serial use = pvSrc.ReadInt32();
					Serial targ = pvSrc.ReadInt32();

					Bandage bandage = World.FindItem( use ) as Bandage;

					if ( bandage != null && !bandage.Deleted )
					{
						if ( from.InRange( bandage.GetWorldLocation(), 2 ) )
						{
							//from.RevealingAction();

							Mobile to = World.FindMobile( targ );

							if ( to != null )
							{
								if ( BandageContext.BeginHeal( from, to ) != null )
									bandage.Consume();
							}
							else
								from.SendLocalizedMessage( 500970 ); // Bandages can not be used on that.
						}
						else
							from.SendLocalizedMessage( 500295 ); // You are too far away to do that.

                        from.NextActionTime = DateTime.UtcNow + Mobile.ServerWideObjectDelay;
					}
				}
				else
					from.SendActionMessage();
			}
			else
				from.SendLocalizedMessage( 500949 ); // You can't do that when you're dead.
		}
	}
}