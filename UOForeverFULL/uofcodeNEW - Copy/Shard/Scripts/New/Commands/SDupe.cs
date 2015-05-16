using System;
using System.Collections;
#if Framework_4_0
using System.Threading.Tasks;
#endif
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Server.Guilds;
using Server.Mobiles;
using Server.Commands.Generic;
using Server.Targeting;
using CPA = Server.CommandPropertyAttribute;
using Server.Items;

namespace Server.Commands
{
	public class SDupe
	{
		public static void Initialize()
		{
			//CommandSystem.Register( "sdupe", AccessLevel.Lead, new CommandEventHandler( Dupe_OnCommand ) );
		}

		[Usage( "sdupe" )]
		[Description( "Duplicates an Item or Mobile through serialization." )]
		public static void Dupe_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "What would you like to dupe?" );
			e.Mobile.Target = new DupeTarget();
		}
	}

	public class DupeTarget : Target
	{
		public DupeTarget() : base( -1, false, TargetFlags.None )
		{
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( targeted is PlayerMobile )
				from.SendMessage( "That's not the way to increase the player count!" );
			else
			{
				IEntity entity = targeted as IEntity;

				if ( entity == null || !( entity is Item || entity is Mobile) )
					return;

				from.SendMessage( "Duping object {0}", entity.Serial );

				new DupeThread( from, entity );
			}
		}
	}

	public class DupeThread
	{
		private Mobile m_From;
		private IEntity m_Entity;
		//private Thread m_Thread;
		private Container m_DupeBag;
		private List<IEntity> m_NewEntities;
		private List<IEntity> m_Entities;
		private Entity m_EntityInfo;
		private object m_Parent;

		public List<IEntity> Entities{ get{ return m_Entities; } }
		public List<IEntity> NewEntities{ get{ return m_NewEntities; } }

		public DupeThread( Mobile m, IEntity entity )
		{
			m_From = m;
			m_Entity = entity;
			m_NewEntities = new List<IEntity>();
			m_Entities = new List<IEntity>();

#if Framework_4_0
			Task dupeTask = Task.Factory.StartNew(() => DupeEntity());
#else
			m_Thread = new Thread( new ThreadStart( DupeEntity ) );
			m_Thread.Name = "Server.Commands.DupeThread";
			//m_Thread.Priority = ThreadPriority.BelowNormal;
			m_Thread.Start();
#endif
		}

		public int GetEntityIndex( Serial serial )
		{
			for ( int i = 0; i < m_Entities.Count; i++ )
				if ( m_Entities[i].Serial == serial )
					return i;

			return -1;
		}

		public void DupeEntity()
		{
			if ( !HasAccess( m_Entity.GetType() ) )
				return;

			if ( m_Entity is Item )
			{
				Mobile rootparent = ((Item)m_Entity).RootParent as Mobile;
				if ( rootparent != null && m_From.AccessLevel < rootparent.AccessLevel )
				{
					m_From.SendMessage( "You cannot dupe what does not belong to you." );
					return;
				}
			}

			//Set saves off?
			m_EntityInfo = new Entity( m_Entity.Serial, m_Entity.Location, m_Entity.Map );
			m_Parent = m_Entity is Item ? ((Item)m_Entity).Parent : null;

			if ( m_Entity is Mobile )
				((Mobile)m_Entity).Internalize();
			else if ( m_Entity is Item )
				((Item)m_Entity).Internalize();

			m_Entities.Add( m_Entity );

			if ( m_Entity is Mobile && !AddEntitiesFromList( ((Mobile)m_Entity).Items ) )
				return;

			if ( m_Entity is Item && !AddEntitiesFromList( ((Item)m_Entity).LookupItems() ) )
				return;
			else
			{
				m_DupeBag = new Bag();
				m_DupeBag.Hue = 6;
				m_DupeBag.Name = "Duped on " + DateTime.Today.ToString( "dd.MM.yyyy" );
			}

			IEntity entity = Dupe(); //Main duping method; returns null if error or failure.

			if ( entity is Item )
			{
				if ( m_DupeBag != null )
				{
					m_DupeBag.DropItem( (Item)entity );
					m_DupeBag.UpdateTotals();
					m_DupeBag.InvalidateProperties();
					m_DupeBag.ProcessDelta();
				}
				else
					((Item)entity).MoveToWorld( m_EntityInfo.Location, m_EntityInfo.Map );
			}
			else if ( entity is Mobile )
				((Mobile)entity).MoveToWorld( m_EntityInfo.Location, m_EntityInfo.Map );

			if ( m_Entity is Item )
			{
				Item origitem = m_Entity as Item;

				if ( m_Parent != null )
				{
					if ( m_Parent is Mobile )
						((Mobile)m_Parent).EquipItem( origitem );
					else if ( m_Parent is Container )
					{
						((Container)m_Parent).DropItem( origitem );
						origitem.Location = m_EntityInfo.Location;
						origitem.ProcessDelta();
					}
				}
				else
					origitem.MoveToWorld( m_EntityInfo.Location, m_EntityInfo.Map );
			}
			else if ( m_Entity is Mobile )
				((Mobile)m_Entity).MoveToWorld( m_EntityInfo.Location, m_EntityInfo.Map );

			m_From.AddToBackpack( m_DupeBag );

			if ( m_NewEntities.Count > 0 )
			{
				m_From.SendGump( new InterfaceGump( m_From, new string[] { "Entity" }, new ArrayList( m_NewEntities ), 0, null ) );

				StringBuilder sb = new StringBuilder();
				sb.AppendFormat( "{0} {1} duping ", m_From.AccessLevel, CommandLogging.Format( m_From ) );
				sb.AppendFormat( "at {0} in {1}: ", m_From.Location, m_From.Map );
				int serial = (m_Entity is Item ? ((Item)m_Entity).Serial : ((Mobile)m_Entity).Serial);
				sb.AppendFormat( "{0} objects via target {1:X}", m_NewEntities.Count, serial );
				CommandLogging.WriteLine( m_From, sb.ToString() );

				sb = new StringBuilder();
				StringBuilder sbm = new StringBuilder();

				sb.Append( "Serials been duped:" );
				sbm.Append( "Serials:" );

				for ( int i = 0; i < m_Entities.Count; i++ )
				{
					sb.AppendFormat( " 0x{0:X};", m_Entities[i].Serial );
					sbm.AppendFormat( " 0x{0:X};", m_NewEntities[i].Serial );
				}
				CommandLogging.WriteLine( m_From, sb.ToString() );
				CommandLogging.WriteLine( m_From, sbm.ToString() );
			}

			m_From.SendMessage( "Duping completed." );
		}

		public IEntity AddEntity( Type type )
		{
			ConstructorInfo ctor = type.GetConstructor( new Type[] { typeof( Serial ) } );

			if ( type.IsSubclassOf( typeof( Item ) ) )
			{
				Item item = ctor.Invoke( new object[] { Serial.NewItem } ) as Item;
				World.AddItem( item );
				return item;
			}
			else //Its a mob!
			{
				Mobile mobile = ctor.Invoke( new object[] { Serial.NewMobile } ) as Mobile;
				World.AddMobile( mobile );
				return mobile;
			}
		}

		public IEntity Dupe()
		{
			MemoryStream stream = new MemoryStream();
			DupeWriter writer = new DupeWriter( stream, this );
			DupeReader reader = new DupeReader( stream, this );

			try
			{
				m_From.SendMessage( "Objects Found: {0}", m_Entities.Count );
				foreach ( IEntity entity in m_Entities )
				{ //We add it at the same time we serialize, makes for less looping
					IEntity newent = AddEntity( entity.GetType() );

					m_NewEntities.Add( newent );

					if ( entity is Item )
						((Item)entity).Serialize( writer );
					else //Its a mob!
						((Mobile)entity).Serialize( writer );
				}

				writer.Flush();
				reader.Seek( 0, SeekOrigin.Begin );

				for ( int i = 0; i < m_NewEntities.Count; i++ )
				{
					IEntity entity = m_NewEntities[i];

					if ( entity is Item )
						((Item)entity).Deserialize( reader );
					else //Its a mob!
						((Mobile)entity).Deserialize( reader );
				}

				if ( !reader.End() )
				{
					m_From.SendMessage( "The buffer is not empty.  There was an error in deserialization." );
					return null;
				}
			}
			catch ( Exception e )
			{
				m_From.SendMessage( e.Message );
				Console.WriteLine( e );

				for( int i = m_NewEntities.Count-1; i >= 0; i-- )
					m_NewEntities[i].Delete();

				m_NewEntities.Clear();
			}
			finally
			{
				writer.Close();
				reader.Close();
				stream.Close();
			}

			if ( m_NewEntities.Count > 0 )
				return m_NewEntities[0];
			else
			{
				m_From.SendMessage( "Error duping objects.  No objects duped." );
				return null;
			}
		}

		public bool AddEntitiesFromList( List<Item> items )
		{
			bool success = true;

			if ( items != null )
			{
				foreach( Item item in items )
				{
					if ( HasAccess( item.GetType() ) )
					{
						if ( !m_Entities.Contains( item ) )
						{
							m_Entities.Add( item );
							if ( item is Item )
								if ( !AddEntitiesFromList( ((Item)item).LookupItems() ) )
									success = false;
						}
					}
					else
					{
						success = false;
						break;
					}
				}
			}

			return success;
		}

		public bool HasAccess( Type type )
		{
			if ( type != null )
			{
				PropertyInfo[] allProps = type.GetProperties( BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public );

				foreach ( PropertyInfo prop in allProps )
				{
					CPA attr = Properties.GetCPA( prop );
					if ( attr != null && ((prop.CanRead && m_From.AccessLevel < attr.ReadLevel) || (prop.CanWrite && m_From.AccessLevel < attr.WriteLevel)) )
					{
						m_From.SendMessage( "The item {0} contains the property {1}, which you do not have access to.", type.Name, prop.Name );
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}

	public class DupeWriter : GenericWriter
	{
		private bool PrefixStrings = true;
		private MemoryStream m_File;

		protected virtual int BufferSize
		{
			get
			{
				return 64 * 1024;
			}
		}

		private byte[] m_Buffer;

		private int m_Index;

		private Encoding m_Encoding;

		private DupeThread m_Thread;

		public DupeWriter( MemoryStream strm, DupeThread thread )
		{
			m_Thread = thread;
			m_Encoding = Utility.UTF8;
			m_Buffer = new byte[BufferSize];
			m_File = strm;
		}

		public void Flush()
		{
			if ( m_Index > 0 )
			{
				m_Position += m_Index;

				m_File.Write( m_Buffer, 0, m_Index );
				m_Index = 0;
			}
		}

		private long m_Position;

		public override long Position
		{
			get
			{
				return m_Position + m_Index;
			}
		}

		public Stream UnderlyingStream
		{
			get
			{
				if ( m_Index > 0 )
					Flush();

				return m_File;
			}
		}

		public override void Close()
		{
			if ( m_Index > 0 )
				Flush();

			m_File.Close();
		}

		public override void WriteEncodedInt( int value )
		{

			uint v = (uint)value;

			while ( v >= 0x80 )
			{
				if ( (m_Index + 1) > BufferSize )
					Flush();

				m_Buffer[m_Index++] = (byte)(v | 0x80);
				v >>= 7;
			}

			if ( (m_Index + 1) > BufferSize )
				Flush();

			m_Buffer[m_Index++] = (byte)v;
		}

		private byte[] m_CharacterBuffer;
		private int m_MaxBufferChars;
		private const int LargeByteBufferSize = 256;

		internal void InternalWriteString( string value )
		{
			int length = m_Encoding.GetByteCount( value );

			WriteEncodedInt( length );

			if ( m_CharacterBuffer == null )
			{
				m_CharacterBuffer = new byte[LargeByteBufferSize];
				m_MaxBufferChars = LargeByteBufferSize / m_Encoding.GetMaxByteCount( 1 );
			}

			if ( length > LargeByteBufferSize )
			{
				int current = 0;
				int charsLeft = value.Length;

				while ( charsLeft > 0 )
				{
					int charCount = (charsLeft > m_MaxBufferChars) ? m_MaxBufferChars : charsLeft;
					int byteLength = m_Encoding.GetBytes( value, current, charCount, m_CharacterBuffer, 0 );

					if ( (m_Index + byteLength) > BufferSize )
						Flush();

					Buffer.BlockCopy( m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength );
					m_Index += byteLength;

					current += charCount;
					charsLeft -= charCount;
				}
			}
			else
			{
				int byteLength = m_Encoding.GetBytes( value, 0, value.Length, m_CharacterBuffer, 0 );

				if ( (m_Index + byteLength) > BufferSize )
					Flush();

				Buffer.BlockCopy( m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength );
				m_Index += byteLength;
			}
		}

		public override void Write( string value )
		{

			if ( PrefixStrings )
			{
				if ( value == null )
				{
					if ( (m_Index + 1) > BufferSize )
						Flush();

					m_Buffer[m_Index++] = 0;
				}
				else
				{
					if ( (m_Index + 1) > BufferSize )
						Flush();

					m_Buffer[m_Index++] = 1;

					InternalWriteString( value );
				}
			}
			else
			{
				InternalWriteString( value );
			}
		}

		public override void Write( DateTimeOffset value )
		{
			Write( value.Ticks );
			Write( value.Offset.Ticks );
		}

		public override void Write( DateTime value )
		{

			Write( value.Ticks );
		}

		public override void WriteDeltaTime( DateTime value )
		{

			long ticks = value.Ticks;
			long now = DateTime.UtcNow.Ticks;

			TimeSpan d;

			try { d = new TimeSpan( ticks - now ); }
			catch { if ( ticks < now ) d = TimeSpan.MaxValue; else d = TimeSpan.MaxValue; }

			Write( d );
		}

		public override void Write( IPAddress value )
		{
			Write( Utility.GetLongAddressValue( value ) );
		}

		public override void Write( TimeSpan value )
		{

			Write( value.Ticks );
		}

		public override void Write( decimal value )
		{

			int[] bits = Decimal.GetBits( value );

			for ( int i = 0; i < bits.Length; ++i )
				Write( bits[i] );
		}

		public override void Write( long value )
		{

			if ( (m_Index + 8) > BufferSize )
				Flush();

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Buffer[m_Index + 4] = (byte)(value >> 32);
			m_Buffer[m_Index + 5] = (byte)(value >> 40);
			m_Buffer[m_Index + 6] = (byte)(value >> 48);
			m_Buffer[m_Index + 7] = (byte)(value >> 56);
			m_Index += 8;
		}

		public override void Write( ulong value )
		{

			if ( (m_Index + 8) > BufferSize )
				Flush();

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Buffer[m_Index + 4] = (byte)(value >> 32);
			m_Buffer[m_Index + 5] = (byte)(value >> 40);
			m_Buffer[m_Index + 6] = (byte)(value >> 48);
			m_Buffer[m_Index + 7] = (byte)(value >> 56);
			m_Index += 8;
		}

		public override void Write( int value )
		{

			if ( (m_Index + 4) > BufferSize )
				Flush();

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Index += 4;
		}

		public override void Write( uint value )
		{

			if ( (m_Index + 4) > BufferSize )
				Flush();

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Buffer[m_Index + 2] = (byte)(value >> 16);
			m_Buffer[m_Index + 3] = (byte)(value >> 24);
			m_Index += 4;
		}

		public override void Write( short value )
		{

			if ( (m_Index + 2) > BufferSize )
				Flush();

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Index += 2;
		}

		public override void Write( ushort value )
		{

			if ( (m_Index + 2) > BufferSize )
				Flush();

			m_Buffer[m_Index] = (byte)value;
			m_Buffer[m_Index + 1] = (byte)(value >> 8);
			m_Index += 2;
		}
/*
		public unsafe override void Write( double value )
		{
			if( (m_Index + 8) > m_Buffer.Length )
				Flush();

			fixed( byte* pBuffer = m_Buffer )
				*((double*)(pBuffer + m_Index)) = value;

			m_Index += 8;
		}

		public unsafe override void Write( float value )
		{
			if( (m_Index + 4) > m_Buffer.Length )
				Flush();

			fixed( byte* pBuffer = m_Buffer )
				*((float*)(pBuffer + m_Index)) = value;

			m_Index += 4;
		}
*/

		public override void Write( double value )
		{
			byte[] eightBytes = BitConverter.GetBytes( value );
			for ( int i = 0; i < 8; i++ )
			{
				Write( eightBytes[i] );
			}
		}

		public override void Write( float value )
		{
			byte[] fourBytes = BitConverter.GetBytes( value );
			for ( int i = 0; i < 4; i++ )
			{
				Write( fourBytes[i] );
			}
		}

		private char[] m_SingleCharBuffer = new char[1];

		public override void Write( char value )
		{
			if ( (m_Index + 8) > BufferSize )
				Flush();

			m_SingleCharBuffer[0] = value;

			int byteCount = m_Encoding.GetBytes( m_SingleCharBuffer, 0, 1, m_Buffer, m_Index );
			m_Index += byteCount;
		}

		public override void Write( byte value )
		{
			if ( (m_Index + 1) > BufferSize )
				Flush();

			m_Buffer[m_Index++] = value;
		}

		public override void Write( sbyte value )
		{
			if ( (m_Index + 1) > BufferSize )
				Flush();

			m_Buffer[m_Index++] = (byte)value;
		}

		public override void Write( bool value )
		{
			if ( (m_Index + 1) > BufferSize )
				Flush();

			m_Buffer[m_Index++] = (byte)(value ? 1 : 0);
		}

		public override void Write( Point3D value )
		{
			Write( value.X );
			Write( value.Y );
			Write( value.Z );
		}

		public override void Write( Point2D value )
		{
			Write( value.X );
			Write( value.Y );
		}

		public override void Write( Rectangle2D value )
		{
			Write( value.Start );
			Write( value.End );
		}

		public override void Write( Rectangle3D value )
		{
			Write( value.Start );
			Write( value.End );
		}

		public override void Write( Map value )
		{
			int flags = 0;
			if ( value is BaseInstanceMap )
				flags |= 0x01;

			WriteEncodedInt( flags );

			if ( ( flags & 0x01 ) != 0 )
				Write( value.MapIndex );
			else if( value != null )
				Write( (byte)value.MapIndex );
			else
				Write( (byte)0xFF );
		}

		public override void WriteEntity( IEntity value )
		{
			if ( value == null )
				Write( Serial.MinusOne );
			else if ( value is Item && ((Item)value).Deleted )
				Write( Serial.MinusOne );
			else if ( value is Mobile && ((Mobile)value).Deleted )
				Write( Serial.MinusOne );
			else
			{
				int index = m_Thread.GetEntityIndex( value.Serial );
				if ( index > -1 )
				{
					Write( (int)1 );
					Write( index );
				}
				else
				{
					Write( (int)2 );
					Write( value.Serial );
				}
			}
		}

		public override void Write( Item value )
		{
			if ( value == null || value.Deleted )
			{
				Write( (int)0 );
				//Write( Serial.MinusOne );
			}
			else
			{
				int index = m_Thread.GetEntityIndex( value.Serial );
				if ( index > -1 )
				{
					Write( (int)1 );
					Write( index );
				}
				else
				{
					Write( (int)2 );
					Write( value.Serial );
				}
			}
		}

		public override void Write( Mobile value )
		{
			if ( value == null || value.Deleted )
			{
				Write( (int)0 );
				//Write( Serial.MinusOne );
			}
			else
			{
				int index = m_Thread.GetEntityIndex( value.Serial );
				if ( index > -1 )
				{
					Write( (int)1 );
					Write( index );
				}
				else
				{
					Write( (int)2 );
					Write( value.Serial );
				}
			}
		}

		public override void Write( BaseGuild value )
		{
			if( value == null )
				Write( 0 );
			else
				Write( value.Id );
		}

		public override void WriteItem<T>( T value )
		{

			Write( value );
		}

		public override void WriteMobile<T>( T value )
		{

			Write( value );
		}

		public override void WriteGuild<T>( T value )
		{

			Write( value );
		}

		public override void WriteMobileList( ArrayList list )
		{

			WriteMobileList( list, false );
		}

		public override void WriteMobileList( ArrayList list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( ((Mobile)list[i]).Deleted )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( (Mobile)list[i] );
		}

		public override void WriteItemList( ArrayList list )
		{

			WriteItemList( list, false );
		}

		public override void WriteItemList( ArrayList list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( ((Item)list[i]).Deleted )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( (Item)list[i] );
		}

		public override void WriteGuildList( ArrayList list )
		{

			WriteGuildList( list, false );
		}

		public override void WriteGuildList( ArrayList list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( ((BaseGuild)list[i]).Disbanded )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( (BaseGuild)list[i] );
		}

		public override void Write( List<Item> list )
		{
			Write( list, false );
		}

		public override void Write( List<Item> list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( list[i].Deleted )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( list[i] );
		}

		public override void WriteItemList<T>( List<T> list )
		{

			WriteItemList<T>( list, false );
		}

		public override void WriteItemList<T>( List<T> list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( list[i].Deleted )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( list[i] );
		}

		public override void Write( HashSet<Item> set )
		{
			Write( set, false );
		}

		public override void Write( HashSet<Item> set, bool tidy )
		{
			if( tidy )
			{
				set.RemoveWhere( item => item.Deleted );
			}

			Write( set.Count );

			foreach( Item item in set )
			{
				Write( item );
			}
		}

		public override void WriteItemSet<T>( HashSet<T> set )
		{
			WriteItemSet( set, false );
		}

		public override void WriteItemSet<T>( HashSet<T> set, bool tidy ) 
		{
			if( tidy )
			{
				set.RemoveWhere( item => item.Deleted );
			}

			Write( set.Count );

			foreach( Item item in set )
			{
				Write( item );
			}
		}

		public override void Write( List<Mobile> list )
		{

			Write( list, false );
		}

		public override void Write( List<Mobile> list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( list[i].Deleted )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( list[i] );
		}

		public override void WriteMobileList<T>( List<T> list )
		{

			WriteMobileList<T>( list, false );
		}

		public override void WriteMobileList<T>( List<T> list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( list[i].Deleted )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( list[i] );
		}

		public override void Write( HashSet<Mobile> set )
		{
			Write( set, false );
		}

		public override void Write( HashSet<Mobile> set, bool tidy )
		{
			if( tidy )
			{
				set.RemoveWhere( mobile => mobile.Deleted );
			}

			Write( set.Count );

			foreach( Mobile mob in set )
			{
				Write( mob );
			}
		}

		public override void WriteMobileSet<T>( HashSet<T> set )
		{
			WriteMobileSet( set, false );
		}

		public override void WriteMobileSet<T>( HashSet<T> set, bool tidy )
		{
			if( tidy )
			{
				set.RemoveWhere( mob => mob.Deleted );
			}

			Write( set.Count );

			foreach( Mobile mob in set )
			{
				Write( mob );
			}
		}

		public override void Write( List<BaseGuild> list )
		{

			Write( list, false );
		}

		public override void Write( List<BaseGuild> list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( list[i].Disbanded )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( list[i] );
		}

		public override void WriteGuildList<T>( List<T> list )
		{
			WriteGuildList<T>( list, false );
		}

		public override void WriteGuildList<T>( List<T> list, bool tidy )
		{

			if ( tidy )
			{
				for ( int i = 0; i < list.Count; )
				{
					if ( list[i].Disbanded )
						list.RemoveAt( i );
					else
						++i;
				}
			}

			Write( list.Count );

			for ( int i = 0; i < list.Count; ++i )
				Write( list[i] );
		}

		public override void Write( HashSet<BaseGuild> set )
		{
			Write( set, false );
		}

		public override void Write( HashSet<BaseGuild> set, bool tidy )
		{
			if( tidy )
			{
				set.RemoveWhere( guild => guild.Disbanded );
			}

			Write( set.Count );

			foreach( BaseGuild guild in set )
			{
				Write( guild );
			}
		}

		public override void WriteGuildSet<T>( HashSet<T> set )
		{
			WriteGuildSet( set, false );
		}

		public override void WriteGuildSet<T>( HashSet<T> set, bool tidy )
		{
			if( tidy )
			{
				set.RemoveWhere( guild => guild.Disbanded );
			}

			Write( set.Count );

			foreach( BaseGuild guild in set )
			{
				Write( guild );
			}
		}

		public override void Write( Race value )
		{

			if ( value != null )
				Write( (byte)value.RaceIndex );
			else
				Write( (byte)0xFF );
		}
	}

	public class DupeReader : GenericReader
	{

		private BinaryReader m_File;

		private DupeThread m_Thread;

		public DupeReader( MemoryStream stream, DupeThread thread ) : base()
		{
			this.m_File = new BinaryReader( stream );
			m_Thread = thread;
		}

		public void Close()
		{
			m_File.Close();
		}

		public long Position
		{
			get
			{
				return m_File.BaseStream.Position;
			}
		}

		public long Seek( long offset, SeekOrigin origin )
		{
			return m_File.BaseStream.Seek( offset, origin );
		}

		public override DateTimeOffset ReadDateTimeOffset()
		{
			long ticks = m_File.ReadInt64();
			TimeSpan offset = new TimeSpan( m_File.ReadInt64() );

			return new DateTimeOffset( ticks, offset );
		}

		public override string ReadString()
		{
			if ( ReadByte() != 0 )
				return m_File.ReadString();
			else
				return null;
		}

		public override DateTime ReadDeltaTime()
		{
			long ticks = m_File.ReadInt64();
			long now = DateTime.UtcNow.Ticks;

			if ( ticks > 0 && (ticks + now) < 0 )
				return DateTime.MaxValue;
			else if ( ticks < 0 && (ticks + now) < 0 )
				return DateTime.MinValue;

			try { return new DateTime( now + ticks ); }
			catch { if ( ticks > 0 ) return DateTime.MaxValue; else return DateTime.MinValue; }
		}

		public override IPAddress ReadIPAddress()
		{
			return new IPAddress( m_File.ReadInt64() );
		}

		public override int ReadEncodedInt()
		{
			int v = 0, shift = 0;
			byte b;

			do
			{
				b = m_File.ReadByte();
				v |= (b & 0x7F) << shift;
				shift += 7;
			} while ( b >= 0x80 );

			return v;
		}

		public override DateTime ReadDateTime()
		{
			return new DateTime( m_File.ReadInt64() );
		}

		public override TimeSpan ReadTimeSpan()
		{
			return new TimeSpan( m_File.ReadInt64() );
		}

		public override decimal ReadDecimal()
		{
			return m_File.ReadDecimal();
		}

		public override long ReadLong()
		{
			return m_File.ReadInt64();
		}

		public override ulong ReadULong()
		{
			return m_File.ReadUInt64();
		}

		public override int ReadInt()
		{
			return m_File.ReadInt32();
		}

		public override uint ReadUInt()
		{
			return m_File.ReadUInt32();
		}

		public override short ReadShort()
		{
			return m_File.ReadInt16();
		}

		public override ushort ReadUShort()
		{
			return m_File.ReadUInt16();
		}

		public override double ReadDouble()
		{
			return m_File.ReadDouble();
		}

		public override float ReadFloat()
		{
			return m_File.ReadSingle();
		}

		public override char ReadChar()
		{
			return m_File.ReadChar();
		}

		public override byte ReadByte()
		{
			return m_File.ReadByte();
		}

		public override sbyte ReadSByte()
		{
			return m_File.ReadSByte();
		}

		public override bool ReadBool()
		{
			return m_File.ReadBoolean();
		}

		public override Point3D ReadPoint3D()
		{
			return new Point3D( ReadInt(), ReadInt(), ReadInt() );
		}

		public override Point2D ReadPoint2D()
		{
			return new Point2D( ReadInt(), ReadInt() );
		}

		public override Rectangle2D ReadRect2D()
		{
			return new Rectangle2D( ReadPoint2D(), ReadPoint2D() );
		}

		public override Rectangle3D ReadRect3D()
		{
			return new Rectangle3D( ReadPoint3D(), ReadPoint3D() );
		}

		public override Map ReadMap()
		{
			int flags = ReadEncodedInt();
			if ( ( flags & 0x01 ) != 0 )
				return World.FindMap( ReadInt() );
			else
				return Map.Maps[ReadByte()];
		}

		public override IEntity ReadEntity()
		{
			switch ( ReadInt() )
			{
				default:
				case 0: return null;
				case 1: return m_Thread.NewEntities[ReadInt()] as IEntity;
				case 2:
				{
					Serial serial = ReadInt();
					IEntity entity = World.FindEntity( serial );
					if ( entity == null )
						return new Entity( serial, new Point3D( 0, 0, 0 ), Map.Internal );
					else
						return entity;
				}
			}
		}

		public override Item ReadItem()
		{
			switch ( ReadInt() )
			{
				default:
				case 0: return null;
				case 1: return m_Thread.NewEntities[ReadInt()] as Item;
				case 2: return World.FindItem( ReadInt() );
			}
		}

		public override Mobile ReadMobile()
		{
			switch ( ReadInt() )
			{
				default:
				case 0: return null;
				case 1: return m_Thread.NewEntities[ReadInt()] as Mobile;
				case 2: return World.FindMobile( ReadInt() );
			}
		}

		public override BaseGuild ReadGuild()
		{
			return BaseGuild.Find( ReadInt() );
		}

		public override T ReadItem<T>()
		{
			return ReadItem() as T;
		}

		public override T ReadMobile<T>()
		{
			return ReadMobile() as T;
		}

		public override T ReadGuild<T>()
		{
			return ReadGuild() as T;
		}

		public override ArrayList ReadItemList()
		{
			int count = ReadInt();

			if ( count > 0 )
			{
				ArrayList list = new ArrayList( count );

				for ( int i = 0; i < count; ++i )
				{
					Item item = ReadItem();

					if ( item != null )
					{
						list.Add( item );
					}
				}

				return list;
			}
			else
			{
				return new ArrayList();
			}
		}

		public override ArrayList ReadMobileList()
		{
			int count = ReadInt();

			if ( count > 0 )
			{
				ArrayList list = new ArrayList( count );

				for ( int i = 0; i < count; ++i )
				{
					Mobile m = ReadMobile();

					if ( m != null )
					{
						list.Add( m );
					}
				}

				return list;
			}
			else
			{
				return new ArrayList();
			}
		}

		public override ArrayList ReadGuildList()
		{
			int count = ReadInt();

			if ( count > 0 )
			{
				ArrayList list = new ArrayList( count );

				for ( int i = 0; i < count; ++i )
				{
					BaseGuild g = ReadGuild();

					if ( g != null )
					{
						list.Add( g );
					}
				}

				return list;
			}
			else
			{
				return new ArrayList();
			}
		}

		public override HashSet<Item> ReadItemSet()
		{
			return ReadItemSet<Item>();
		}

		public override HashSet<T> ReadItemSet<T>()
		{
			int count = ReadInt();

			if( count > 0 )
			{
				HashSet<T> set = new HashSet<T>();

				for( int i = 0; i < count; ++i )
				{
					T item = ReadItem() as T;

					if( item != null )
					{
						set.Add( item );
					}
				}

				return set;
			}
			else
			{
				return new HashSet<T>();
			}
		}

		public override List<Item> ReadStrongItemList()
		{
			return ReadStrongItemList<Item>();
		}

		public override List<T> ReadStrongItemList<T>()
		{
			int count = ReadInt();

			if ( count > 0 )
			{
				List<T> list = new List<T>( count );

				for ( int i = 0; i < count; ++i )
				{
					T item = ReadItem() as T;

					if ( item != null )
					{
						list.Add( item );
					}
				}

				return list;
			}
			else
			{
				return new List<T>();
			}
		}

		public override List<Mobile> ReadStrongMobileList()
		{
			return ReadStrongMobileList<Mobile>();
		}

		public override List<T> ReadStrongMobileList<T>()
		{
			int count = ReadInt();

			if ( count > 0 )
			{
				List<T> list = new List<T>( count );

				for ( int i = 0; i < count; ++i )
				{
					T m = ReadMobile() as T;

					if ( m != null )
					{
						list.Add( m );
					}
				}

				return list;
			}
			else
			{
				return new List<T>();
			}
		}

		public override HashSet<Mobile> ReadMobileSet()
		{
			return ReadMobileSet<Mobile>();
		}

		public override HashSet<T> ReadMobileSet<T>()
		{
			int count = ReadInt();

			if( count > 0 )
			{
				HashSet<T> set = new HashSet<T>();

				for( int i = 0; i < count; ++i )
				{
					T item = ReadMobile() as T;

					if( item != null )
					{
						set.Add( item );
					}
				}

				return set;
			}
			else
			{
				return new HashSet<T>();
			}
		}

		public override List<BaseGuild> ReadStrongGuildList()
		{
			return ReadStrongGuildList<BaseGuild>();
		}

		public override List<T> ReadStrongGuildList<T>()
		{
			int count = ReadInt();

			if ( count > 0 )
			{
				List<T> list = new List<T>( count );

				for ( int i = 0; i < count; ++i )
				{
					T g = ReadGuild() as T;

					if ( g != null )
					{
						list.Add( g );
					}
				}

				return list;
			}
			else
			{
				return new List<T>();
			}
		}

		public override HashSet<BaseGuild> ReadGuildSet()
		{
			return ReadGuildSet<BaseGuild>();
		}

		public override HashSet<T> ReadGuildSet<T>()
		{
			int count = ReadInt();

			if( count > 0 )
			{
				HashSet<T> set = new HashSet<T>();

				for( int i = 0; i < count; ++i )
				{
					T item = ReadGuild() as T;

					if( item != null )
					{
						set.Add( item );
					}
				}

				return set;
			}
			else
			{
				return new HashSet<T>();
			}
		}

		public override bool End()
		{
			return m_File.PeekChar() == -1;
		}

		public override Race ReadRace()
		{
			return Race.Races[ReadByte()];
		}
	}
}