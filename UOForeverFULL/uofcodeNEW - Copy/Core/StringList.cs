using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Server
{
	public enum LState
	{
		Original, //From a cliloc file, localized to the player.
		Extended, //Added custom to the shard
		Replaced //Replacing a string from a cliloc
	}

	public class StringList
	{
		private static Dictionary<string, StringList> m_StringLists = new Dictionary<string, StringList>();
		public static Dictionary<string, StringList> StringLists{ get{ return m_StringLists; } }

		private static StringList m_Default;
		public static StringList Default{ get{ return m_Default; } }

		private Dictionary<int, StringEntry> m_Table;
		private string m_Language;

		public Dictionary<int, StringEntry> Table{ get{ return m_Table; } }
		public string Language{ get{ return m_Language; } }
		public StringEntry this[int number]{ get{ StringEntry text; m_Table.TryGetValue( number, out text ); return text; } }

		public static StringList AddLanguage( string lang )
		{
			StringList list = null;
			if ( !String.IsNullOrEmpty( lang ) )
			{
				list = new StringList( lang.ToUpper() );
				m_StringLists.Add( lang, list );
			}
			return list;
		}

		public static StringList GetList( string lang )
		{
			StringList list;
			if ( String.IsNullOrEmpty( lang ) )
				list = Default;
			else
				m_StringLists.TryGetValue( lang.ToUpper(), out list );
			return list;
		}

		public StringList() : this( "ENU" )
		{
		}

		public StringList( string language )
		{
			m_Language = language.ToUpper();
			m_Table = new Dictionary<int, StringEntry>();
			if ( m_Language == "ENU" )
				m_Default = this;
		}

		//C# argument support
		public static Regex FormatExpression = new Regex( @"~(\d)+_.*?~", RegexOptions.IgnoreCase );

		public static string MatchComparison( Match m )
		{
			return "{" + (Utility.ToInt32( m.Groups[1].Value ) - 1) + "}";
		}

		public static string FormatArguments( string entry )
		{
			return FormatExpression.Replace( entry, new MatchEvaluator( MatchComparison ) );
		}

		//UO tabbed argument coversion
		public string CombinedWithArguments( int number, string args )
		{
			return CombinedWithObjArguments( number, args.Split( new char[]{ '\t' } ) );
		}

		public string CombinedWithObjArguments( int number, params object[] args )
		{
			StringEntry entry = this[number];
			if ( entry != null )
				return entry.CombinedWithObjArguments( args );
			else
				throw new Exception( String.Format( "No string entry exists for {0}", number ) );
		}
	}

	public class StringEntry
	{
		private LState m_State;
		private int m_Number;
		private string m_Text;

		public LState State{ get{ return m_State; } set{ m_State = value; } }
		public int Number{ get{ return m_Number; } }
		public string Text{ get{ return m_Text; } }

		public string CombinedWithArguments( string args )
		{
			return CombinedWithObjArguments( args.Split( new char[]{ '\t' } ) );
		}

		public string CombinedWithObjArguments( params object[] args )
		{
			return String.Format( m_Text, args );
		}

		public string PrependArguments( string args )
		{
			return String.Format( "{0}{1}", args, m_Text );
		}

		public string AppendArguments( string args )
		{
			return String.Format( "{0}{1}", m_Text, args );
		}

		public StringEntry( int number, LState state, string text )
		{
			m_State = state;
			m_Number = number;
			m_Text = text;
		}

		public StringEntry( GenericReader reader )
		{
			m_State = (LState)reader.ReadEncodedInt();
			m_Number = reader.ReadInt();
			m_Text = reader.ReadString();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int)m_State );
			writer.Write( m_Number );
			writer.Write( m_Text );
		}
	}

	public class ClilocHandler
	{
		public static void AddCliloc( string lang )
		{
			StringList list = StringList.GetList( lang );
			if ( list == null )
				list = StringList.AddLanguage( lang );

			string path = Core.FindDataFile( String.Format( "cliloc.{0}", lang ) );

			if ( path == null )
			{
				Console.WriteLine( "Warning: cliloc.{0} not found", lang );
				return;
			}

			using ( BinaryReader bin = new BinaryReader( new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read ), Encoding.ASCII ) )
			{
				bin.ReadInt32();
				bin.ReadInt16();

				Encoding utf8 = Encoding.GetEncoding( "UTF-8", new EncoderReplacementFallback(""), new DecoderReplacementFallback("") );

				while ( bin.PeekChar() != -1 )
				{
					int number = bin.ReadInt32();
					bin.ReadByte(); //State in Cliloc
					int length = bin.ReadInt16(); //Max of 65535 characters

					StringEntry entry = new StringEntry( number, LState.Original, StringList.FormatArguments( utf8.GetString( bin.ReadBytes( length ) ) ) );

					list.Table[number] = entry;
				}

				bin.Close();
			}
		}

		public static void AddExtended( string lang )
		{
			StringList list = StringList.GetList( lang );
			if ( list == null )
				list = StringList.AddLanguage( lang );

			XmlDocument doc = new XmlDocument();
			doc.Load( Path.Combine( Core.BaseDirectory, String.Format( "Data/Localization/{0}.xml", lang ) ) );

			XmlElement root = doc["locs"];

			foreach ( XmlElement loc in root.GetElementsByTagName( "loc" ) )
			{
				int number = Utility.GetXMLInt32( Utility.GetAttribute( loc, "num", "0" ), -1 );
				StringEntry entry = list[number];

				if ( number > 500000 )
				{
					string text = Utility.GetText( loc, String.Empty );

					if ( entry != null )
						entry.State = LState.Replaced;
					else
						entry = new StringEntry( number, LState.Extended, text );
				}
			}
		}
	}
}