using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Targeting;

namespace Server.Misc
{
	public class EasyUOControl
	{
		private static PacketHandler m_Real6C;
		private static Hashtable m_LastTarget;

		public static void Configure()
		{
			m_Real6C = PacketHandlers.GetHandler( 0x6C );
			m_LastTarget = new Hashtable();

			PacketHandlers.Register( 0x6C, 19, true, new OnPacketReceive( TargetResponse ) );
			EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnected );
		}

		public static void TargetResponse( NetState state, PacketReader pvSrc )
		{
			int type = pvSrc.ReadByte();
			int targetID = pvSrc.ReadInt32();
			int flags = pvSrc.ReadByte();
			Serial serial = pvSrc.ReadInt32();
			int x = pvSrc.ReadInt16(), y = pvSrc.ReadInt16();
			int z = pvSrc.ReadInt16();
			int graphic = pvSrc.ReadInt16();

			pvSrc.Seek( 1, System.IO.SeekOrigin.Begin );

			Mobile from = state.Mobile;

			if ( from == null || from.Target == null )
				return;

			if ( x == 0 && y == 0 && z == 0 && serial != from.Serial )
			{
				bool ok = false;
				if ( serial.IsItem )
				{
					Item i = World.FindItem( serial );
					if ( i != null && i.Location == Point3D.Zero )
						ok = true;
				}
				else if ( serial.IsMobile )
				{
					Mobile m = World.FindMobile( serial );
					if ( m != null && m.Location == Point3D.Zero )
						ok = true;
				}

				object o = m_LastTarget[from];
				if ( !ok && o != null && o is Serial && serial != (Serial)o )
				{
					from.SendAsciiMessage( "This malformed target has been blocked." );
					from.Target.Cancel( from, TargetCancelType.Canceled );
					return;
				}
			}

			if ( from.Serial != serial )
				m_LastTarget[from] = serial;

			m_Real6C.OnReceive( state, pvSrc );	
		}

		public static void EventSink_Disconnected( DisconnectedEventArgs args )
		{
			m_LastTarget.Remove( args.Mobile );
		}
	}
}

