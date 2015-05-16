using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Multis
{
	public class PreviewStatic : Static
	{
		private Mobile m_Previewer;
		private int m_PreviewID;

        public PreviewStatic() : this( 0x80, null )
        {
        }

		public PreviewStatic( int itemID, Mobile prev ) : base( 0x219A )
		{
			m_Previewer = prev;
			m_PreviewID = itemID;
		}

		public PreviewStatic( int itemID, int count, Mobile prev ) : base( 0x219A )
		{
			m_Previewer = prev;
			m_PreviewID = Utility.Random( itemID, count );
		}

		public PreviewStatic( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
			writer.Write( m_Previewer );
			writer.Write( m_PreviewID );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			m_Previewer = reader.ReadMobile();
			m_PreviewID = reader.ReadInt();
		}

		protected override Packet GetWorldPacketFor( NetState state )
		{
			Mobile mob = state.Mobile;

			if ( mob != null && mob == m_Previewer )
				return new PreviewPacket( this, m_PreviewID );

			return base.GetWorldPacketFor( state );
		}
	}

	public class SurveyPreviewHouse : BaseMulti
	{
		private List<Item> m_Components;
		private Timer m_Timer;
		private Mobile m_Previewer;
		private int m_PreviewID;

		private static List<Mobile> m_PreviewHouseList = new List<Mobile>();
		public static List<Mobile> PreviewHouseList{ get{ return m_PreviewHouseList; } }

		public SurveyPreviewHouse( int multiID, Mobile prev ) : base( 0x219A )
		{
			m_Previewer = prev;
			m_PreviewID = multiID;
			m_Components = new List<Item>();

			MultiComponentList mcl = MultiData.GetComponents( m_PreviewID );

			for ( int i = 1; i < mcl.List.Length; ++i )
			{
				MultiTileEntry entry = mcl.List[i];

				if ( entry.m_Flags == 0 )
				{
					Item item = new PreviewStatic( entry.m_ItemID, m_Previewer );
					item.Name = TileData.ItemTable[entry.m_ItemID].Name;

					item.MoveToWorld( new Point3D( X + entry.m_OffsetX, Y + entry.m_OffsetY, Z + entry.m_OffsetZ ), Map );

					m_Components.Add( item );
				}
			}

			m_Timer = new DecayTimer( this, prev );
			m_Timer.Start();
			m_PreviewHouseList.Add( prev );
		}
/* For some reason this rejects movement by non-previewers
		public override MultiComponentList Components
		{
			get
			{
				return MultiData.GetComponents( m_PreviewID );
			}
		}
*/
		protected override Packet GetWorldPacketFor( NetState state )
		{
			Mobile mob = state.Mobile;

			if ( mob != null && ( mob.AccessLevel >= AccessLevel.GameMaster || mob == m_Previewer ) )
				return new PreviewPacket( this, m_PreviewID );

			return base.GetWorldPacketFor( state );
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			base.OnLocationChange( oldLocation );

			if ( m_Components == null )
				return;

			int xOffset = X - oldLocation.X;
			int yOffset = Y - oldLocation.Y;
			int zOffset = Z - oldLocation.Z;

			for ( int i = 0; i < m_Components.Count; ++i )
			{
				Item item = m_Components[i];

				item.MoveToWorld( new Point3D( item.X + xOffset, item.Y + yOffset, item.Z + zOffset ), this.Map );
			}
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			if ( m_Components == null )
				return;

			for ( int i = 0; i < m_Components.Count; ++i )
			{
				Item item = m_Components[i];

				item.Map = this.Map;
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if ( m_Components == null )
				return;

			for ( int i = 0; i < m_Components.Count; ++i )
			{
				Item item = m_Components[i];

				item.Delete();
			}
		}

		public override void OnAfterDelete()
		{
//			if ( m_Timer != null )
//				m_Timer.Stop();

			m_Timer = null;

			base.OnAfterDelete();
		}

		public SurveyPreviewHouse( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			writer.Write( m_Components );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
					m_Components = reader.ReadStrongItemList();

					break;
				}
			}

			Delete();
		}

		private class DecayTimer : Timer
		{
			private Item m_Item;
			private Mobile m_Previewer;

			public DecayTimer( Item item, Mobile prev ) : base( TimeSpan.FromSeconds( 20.0 ) )
			{
				m_Item = item;
				m_Previewer = prev;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Item.Delete();
				if ( m_Previewer != null )
					SurveyPreviewHouse.PreviewHouseList.Remove( m_Previewer );
			}
		}
	}

	public sealed class PreviewPacket : Packet
	{
		public PreviewPacket( Item item, int itemid ) : base( 0x1A )
		{
			this.EnsureCapacity( 20 );

			// 14 base length
			// +2 - Amount
			// +2 - Hue
			// +1 - Flags

			uint serial = (uint)item.Serial.Value;
			int itemID = itemid & 0x3FFF;
			int amount = item.Amount;
			Point3D loc = item.Location;
			int x = loc.X;
			int y = loc.Y;
			int hue = item.Hue;
			int flags = item.GetPacketFlags();
			int direction = (int)item.Direction;

			if ( amount != 0 )
			{
				serial |= 0x80000000;
			}
			else
			{
				serial &= 0x7FFFFFFF;
			}

			m_Stream.Write( (uint) serial );

			if ( item is BaseMulti )
				m_Stream.Write( (short) (itemID | 0x4000) );
			else
				m_Stream.Write( (short) itemID );

			if ( amount != 0 )
			{
				m_Stream.Write( (short) amount );
			}

			x &= 0x7FFF;

			if ( direction != 0 )
			{
				x |= 0x8000;
			}

			m_Stream.Write( (short) x );

			y &= 0x3FFF;

			if ( hue != 0 )
			{
				y |= 0x8000;
			}

			if ( flags != 0 )
			{
				y |= 0x4000;
			}

			m_Stream.Write( (short) y );

			if ( direction != 0 )
				m_Stream.Write( (byte) direction );

			m_Stream.Write( (sbyte) loc.Z );

			if ( hue != 0 )
				m_Stream.Write( (ushort) hue );

			if ( flags != 0 )
				m_Stream.Write( (byte) flags );
		}
	}
}