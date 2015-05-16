using System;

namespace Server.Factions
{
	public interface IFactionItem
	{
		FactionItem FactionItemState{ get; set; }
	}

	public class FactionItem
	{
		public static readonly TimeSpan ExpirationPeriod = TimeSpan.FromDays( 7.0 );

		private Item m_Item;
		private Faction m_Faction;
		private DateTime m_Expiration;
		private int m_OrigHue;
		private LootType m_OrigLootType;

		public Item Item{ get{ return m_Item; } }
		public Faction Faction{ get{ return m_Faction; } }
		public DateTime Expiration{ get{ return m_Expiration; } }
		public int OrigHue{ get{ return m_OrigHue; } }
		public LootType OrigLootType{ get{ return m_OrigLootType; } }

		public bool HasExpired
		{
			get
			{
				if ( m_Item == null || m_Item.Deleted )
					return true;

				return ( m_Expiration != DateTime.MinValue && DateTime.UtcNow >= m_Expiration );
			}
		}

		public void StartExpiration()
		{
			m_Expiration = DateTime.UtcNow + ExpirationPeriod;
		}

		public void CheckAttach()
		{
			if ( !HasExpired )
				Attach();
			else
				Detach();
		}

		public void Attach()
		{
			if ( m_Item is IFactionItem )
				((IFactionItem)m_Item).FactionItemState = this;

			if ( m_Faction != null )
				m_Faction.State.FactionItems.Add( this );
		}

		public void Detach()
		{
			if ( m_Item is IFactionItem )
			{
				((IFactionItem)m_Item).FactionItemState = null;
				m_Item.Hue = m_OrigHue;
				m_Item.LootType = m_OrigLootType;
			}

			if ( m_Faction != null && m_Faction.State.FactionItems.Contains( this ) )
				m_Faction.State.FactionItems.Remove( this );
		}

		public FactionItem( Item item, Faction faction )
		{
			m_Item = item;
			m_Faction = faction;
		}

		public FactionItem( GenericReader reader, Faction faction )
		{
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
				{
					m_OrigLootType = (LootType)reader.ReadByte();
					m_OrigHue = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_Item = reader.ReadItem();
					m_Expiration = reader.ReadDateTime();
					break;
				}
			}

			m_Faction = faction;
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 1 );

			writer.Write( (byte) m_OrigLootType );
			writer.Write( (int) m_OrigHue );
			writer.Write( (Item) m_Item );
			writer.Write( (DateTime) m_Expiration );
		}

		public static int GetMaxWearables( Mobile mob )
		{
			PlayerState pl = PlayerState.Find( mob );

			if ( pl == null )
				return 0;

			if ( pl.Faction.IsCommander( mob ) )
				return 9;

			return pl.Rank.MaxWearables;
		}

		public static FactionItem Find( Item item )
		{
			if ( item is IFactionItem )
			{
				FactionItem state = ((IFactionItem)item).FactionItemState;

				if ( state != null && state.HasExpired )
				{
					state.Detach();
					state = null;
				}

				return state;
			}

			return null;
		}

		public static Item Imbue( Item item, Faction faction, bool expire, int hue )
		{
			if ( (item is IFactionItem) )
			{
				FactionItem state = Find( item );

				if ( state == null )
				{
					state = new FactionItem( item, faction );
					state.Attach();
				}

				if ( expire )
					state.StartExpiration();

				state.m_OrigHue = item.Hue;
				state.m_OrigLootType = item.LootType;
				item.Hue = hue;
				item.LootType = LootType.Blessed;
			}

			return item;
		}
	}
}