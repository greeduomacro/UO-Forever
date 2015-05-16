using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	[TypeAlias( "Server.Items.DisguisePersistance" )]
	public class DisguisePersistence : Item
	{
		private static DisguisePersistence m_Instance;

		public static DisguisePersistence Instance{ get{ return m_Instance; } }

		public override string DefaultName
		{
			get { return "Disguise Persistence - Internal"; }
		}

		public DisguisePersistence() : base( 1 )
		{
			Movable = false;

			if ( m_Instance == null || m_Instance.Deleted )
				m_Instance = this;
			else
				base.Delete();
		}

		public DisguisePersistence( Serial serial ) : base( serial )
		{
			m_Instance = this;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			int timerCount = DisguiseTimers.Timers.Count;

			writer.Write( timerCount );

			foreach ( KeyValuePair<Mobile, Timer> kvp in DisguiseTimers.Timers )
			{
				Mobile m = kvp.Key;

				writer.Write( m );
				writer.Write( kvp.Value.Next - DateTime.UtcNow );
				writer.Write( m.NameMod );
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					int count = reader.ReadInt();

					for ( int i = 0; i < count; ++i )
					{
						Mobile m = reader.ReadMobile();
						DisguiseTimers.CreateTimer( m, reader.ReadTimeSpan() );
						m.NameMod = reader.ReadString();
					}

					break;
				}
			}
		}

		public override void Delete()
		{
		}
	}
}