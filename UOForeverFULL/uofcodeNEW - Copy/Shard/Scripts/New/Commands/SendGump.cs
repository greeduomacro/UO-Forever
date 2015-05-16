using System;
using System.Text;
using System.Reflection;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Gumps;

namespace Server.Commands
{
	public class SendGump
	{
		public static void Initialize()
		{
			CommandSystem.Register( "SendGump", AccessLevel.GameMaster, new CommandEventHandler( SendGump_OnCommand ) );
		}

		public class SendGumpTarget : Target
		{
			private string[] m_Args;

			public SendGumpTarget( string[] args ) : base( -1, false, TargetFlags.None )
			{
				m_Args = args;
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( targ is Mobile )
					SendGump.GumpInvoke( from, (Mobile)targ, m_Args );
				else
					from.SendMessage( "You cannot send a gump that.");
			}
		}

		public static void FixArgs( ref string[] args )
		{
			string[] old = args;
			args = new string[args.Length - 1];

			Array.Copy( old, 1, args, 0, args.Length );
		}

		public static void SendCtor( Type type, ConstructorInfo ctor, Mobile from )
		{
			ParameterInfo[] paramList = ctor.GetParameters();

			StringBuilder sb = new StringBuilder();

			sb.Append( type.Name );

			for ( int i = 0; i < paramList.Length; ++i )
			{
				if ( i != 0 )
					sb.Append( ',' );

				sb.Append( ' ' );

				sb.Append( paramList[i].ParameterType.Name );
				sb.Append( ' ' );
				sb.Append( paramList[i].Name );
			}

			from.SendMessage( sb.ToString() );
		}

		public static object[] ParseValues( ParameterInfo[] paramList, string[] args )
		{
			object[] values = new object[args.Length];

			for ( int i = 0; i < args.Length; ++i )
			{
				object value = ParseValue( paramList[i].ParameterType, args[i] );

				if ( value != null )
					values[i] = value;
				else
					return null;
			}

			return values;
		}

		public static object ParseValue( Type type, string value )
		{
			try
			{
				if ( IsEnum( type ) )
					return Enum.Parse( type, value, true );
				else if ( IsType( type ) )
					return ScriptCompiler.FindTypeByName( value );
				else if ( IsParsable( type ) )
					return ParseParsable( type, value );
				else
				{
					object obj = value;

					if ( value != null && value.StartsWith( "0x" ) )
					{
						if ( IsSignedNumeric( type ) )
							obj = Convert.ToInt64( value.Substring( 2 ), 16 );
						else if ( IsUnsignedNumeric( type ) )
							obj = Convert.ToUInt64( value.Substring( 2 ), 16 );

						obj = Convert.ToInt32( value.Substring( 2 ), 16 );
					}

					if ( obj == null && !type.IsValueType )
						return null;
					else
						return Convert.ChangeType( obj, type );
				}
			}
			catch
			{
				return null;
			}
		}


		#region OtherStuff

		private static Type m_ConstructableType = typeof( ConstructableAttribute );

		public static bool IsConstructable( ConstructorInfo ctor )
		{
			return ctor.IsDefined( m_ConstructableType, false );
		}

		private static Type m_EnumType = typeof( Enum );

		public static bool IsEnum( Type type )
		{
			return type.IsSubclassOf( m_EnumType );
		}

		private static Type m_TypeType = typeof( Type );

		public static bool IsType( Type type )
		{
			return ( type == m_TypeType || type.IsSubclassOf( m_TypeType ) );
		}

		private static Type m_ParsableType = typeof( ParsableAttribute );

		public static bool IsParsable( Type type )
		{
			return type.IsDefined( m_ParsableType, false );
		}

		private static Type[] m_ParseTypes = new Type[]{ typeof( string ) };
		private static object[] m_ParseArgs = new object[1];

		public static object ParseParsable( Type type, string value )
		{
			MethodInfo method = type.GetMethod( "Parse", m_ParseTypes );

			m_ParseArgs[0] = value;

			return method.Invoke( null, m_ParseArgs );
		}

		private static Type[] m_SignedNumerics = new Type[]
		{
			typeof( Int64 ),
			typeof( Int32 ),
			typeof( Int16 ),
			typeof( SByte )
		};

		public static bool IsSignedNumeric( Type type )
		{
			for ( int i = 0; i < m_SignedNumerics.Length; ++i )
			if ( type == m_SignedNumerics[i] )
				return true;

			return false;
		}

		private static Type[] m_UnsignedNumerics = new Type[]
		{
			typeof( UInt64 ),
			typeof( UInt32 ),
			typeof( UInt16 ),
			typeof( Byte )
		};

		public static bool IsUnsignedNumeric( Type type )
		{
			for ( int i = 0; i < m_UnsignedNumerics.Length; ++i )
			if ( type == m_UnsignedNumerics[i] )
				return true;

			return false;
		}

		#endregion

		[Usage( "SendGump <name> [params]" )]
		[Description( "Sends a Gump to a targeted Mobile. Optional constructor parameters." )]
		public static void SendGump_OnCommand( CommandEventArgs e )
		{
			if ( e.Length >= 1 )
			{
				Type t = ScriptCompiler.FindTypeByName( e.GetString( 0 ) );

				if ( t == null )
					e.Mobile.SendMessage( "No Gump with that name was found." );
				else
					e.Mobile.Target = new SendGumpTarget( e.Arguments );
			}
			else
				e.Mobile.SendMessage( "Format: SendGump <name> [params]" );
		}

		public static void GumpInvoke( Mobile from, Mobile targ, string[] args )
		{
			string name = args[0];

			FixArgs( ref args );

			Type type = ScriptCompiler.FindTypeByName( name );

			if ( type == null )
				from.SendMessage( "No gump with that name was found." );
			else if ( type.IsSubclassOf( typeof( Gump ) ) )
			{
				bool sent = SendThatGump( from, targ, type, args );

				if ( sent )
					from.SendMessage("Gump sent to the player.");
				else
					SendGumpUsage( type, from );
			}
			else
				from.SendMessage( "No gump with that name was found" );
		}

		public static bool SendThatGump( Mobile from, Mobile targ, Type type, string[] args )
		{
			ConstructorInfo[] ctors = type.GetConstructors();

			for ( int i = 0; i < ctors.Length; ++i )
			{
				ConstructorInfo ctor = ctors[i];
				ParameterInfo[] paramList = ctor.GetParameters();

				if ( args.Length == paramList.Length )
				{
					object[] paramValues = ParseValues( paramList, args );

					if ( paramValues == null )
						continue;

					bool sent = SendIt( from, targ, ctor, paramValues );

					return sent;
				}
			}
			return false;
		}

		public static bool SendIt( Mobile from, Mobile targ, ConstructorInfo ctor, object[] values )
		{
			try
			{
				object built = ctor.Invoke( values );

				bool r = false;

				if ( built is Gump )
				{
					Gump g = (Gump)built;

					targ.SendGump( g );

					r = true;
				}

				return r;
			}
			catch ( Exception ex )
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public static void SendGumpUsage( Type type, Mobile from )
		{
			ConstructorInfo[] ctors = type.GetConstructors();
			bool foundCtor = false;

			for ( int i = 0; i < ctors.Length; ++i )
			{
				ConstructorInfo ctor = ctors[i];

				if ( !foundCtor )
				{
					foundCtor = true;
					from.SendMessage( "Usage:" );
				}

				SendCtor( type, ctor, from );
			}

			if ( !foundCtor )
				from.SendMessage( "That type is not marked constructable." );
		}
	}
}