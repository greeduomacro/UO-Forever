using System;
using Server.Items;

namespace Server.Ethics
{
	public interface IEthicsItem
	{
		EthicsItem EthicsItemState{ get; set; }
	}

	public class EthicsItem
	{
		public static readonly TimeSpan ExpirationPeriod = TimeSpan.FromHours( 1.5 );

		private Item m_Item;
		private Ethic m_Ethic;
		private DateTime m_Expiration;
		private int m_OrigHue;
		private bool m_IsRunic;

		public Item Item{ get{ return m_Item; } }
		public Ethic Ethic{ get{ return m_Ethic; } }
		public DateTime Expiration{ get{ return m_Expiration; } }
		public int OrigHue{ get{ return m_OrigHue; } }
		public bool IsRunic{ get{ return m_IsRunic; } }

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

		public void MakeRunic()
		{
			m_IsRunic = true;
		}

		public void CheckAttach()
		{
				Attach();
			if (HasExpired)
				Detach();
		}

		public void Attach()
		{
			if ( m_Item is IEthicsItem )
				((IEthicsItem)m_Item).EthicsItemState = this;

			if ( m_Ethic != null )
				m_Ethic.EthicItems.Add( this );
		}

		public void Detach()
		{
			m_IsRunic = false;
            //Attach();
		}

		public EthicsItem( Item item, Ethic ethic )
		{
			m_Item = item;
			m_Ethic = ethic;
		    if (item is IDurability)
		    {
		        ((IDurability)item).MaxHitPoints = 150;
                ((IDurability)item).HitPoints = 150;
		    }
		}

		public EthicsItem( GenericReader reader, Ethic ethic )
		{
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 2:
				{
					m_IsRunic = reader.ReadBool();
					m_OrigHue = reader.ReadInt();
					goto case 0;
				}
				case 1:
				{
					/*m_OrigLootType = (LootType)*/reader.ReadByte();
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

			m_Ethic = ethic;
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 2 );

			writer.Write( (bool) m_IsRunic );
			//writer.Write( (byte) m_OrigLootType );
			writer.Write( (int) m_OrigHue );
			writer.Write( (Item) m_Item );
			writer.Write( (DateTime) m_Expiration );
		}

		public static EthicsItem Find( Item item )
		{
			if ( item is IEthicsItem )
			{
				EthicsItem state = ((IEthicsItem)item).EthicsItemState;

				if ( state != null && state.HasExpired )
				{
					state.Detach();
				}

				return state;
			}

			return null;
		}

		public static Item Imbue( Item item, Ethic ethic, int hue )
		{
			if ( (item is IEthicsItem) )
			{
				EthicsItem state = Find( item );

				if ( state == null )
				{
					state = new EthicsItem( item, ethic );
					state.Attach();
				}

				state.m_OrigHue = item.Hue;
				item.Hue = hue;
			}

			return item;
		}
	}
}