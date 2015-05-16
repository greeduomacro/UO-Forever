using System;
using System.Collections.Generic;
using Server.Ethics;

namespace Server.Factions
{
	[TypeAlias( "Server.Factions.FactionPersistance" )]
	public class FactionPersistence : Item
	{
		private static FactionPersistence m_Instance;

		public static FactionPersistence Instance{ get{ return m_Instance; } }

		public override string DefaultName
		{
			get { return "Faction Persistence - Internal"; }
		}

		public FactionPersistence() : base( 1 )
		{
			Movable = false;

			if ( m_Instance == null || m_Instance.Deleted )
				m_Instance = this;
			else
				base.Delete();
		}

		private enum PersistedType
		{
			Terminator,
			Faction,
			Town
		}

		public FactionPersistence( Serial serial ) : base( serial )
		{
			m_Instance = this;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			List<Faction> factions = Faction.Factions;

			for ( int i = 0; i < factions.Count; ++i )
			{
				writer.WriteEncodedInt( (int) PersistedType.Faction );
				factions[i].State.Serialize( writer );
			}

			List<Town> towns = Town.Towns;

			for ( int i = 0; i < towns.Count; ++i )
			{
				writer.WriteEncodedInt( (int) PersistedType.Town );
				towns[i].State.Serialize( writer );
			}

			writer.WriteEncodedInt( (int) PersistedType.Terminator );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					PersistedType type;

					while ( (type = (PersistedType)reader.ReadEncodedInt()) != PersistedType.Terminator )
					{
						switch ( type )
						{
							case PersistedType.Faction: new FactionState( reader ); break;
							case PersistedType.Town: new TownState( reader ); break;
						}
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