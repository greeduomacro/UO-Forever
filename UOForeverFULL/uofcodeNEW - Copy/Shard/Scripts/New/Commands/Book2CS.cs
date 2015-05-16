using System;
using System.IO;
using Server;
using Server.Targeting;
using Server.Items;

namespace Server.Commands
{
	public class Book2CS
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Book2CS", AccessLevel.Lead, new CommandEventHandler( CSBook_OnCommand ) );
		}

		[Usage( "Book2CS" )]
		[Description( "Make a C# file from a book." )]

		public static void CSBook_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.SendMessage( "Select a book to export to a C# script:" );
			from.Target = new CSBookTarget();
		}

		private class CSBookTarget : Target
		{
			public CSBookTarget() : base( 12, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( targ is BaseBook )
					StartCS( from, targ as BaseBook );
			}
		}

		private static void StartCS( Mobile from, BaseBook book )
		{
			from.SendMessage( "Writing book to C# file..." );

			try
			{
				string writetitle = String.Format( "Book{0}", book.Serial );

				if ( !Directory.Exists( "CSBooks/" ) )
					Directory.CreateDirectory( "CSBooks/" );

				string filepath = Path.Combine( String.Format( "CSBooks/{0}.cs", writetitle ) );

				if ( File.Exists( filepath ) )
					from.SendMessage( "File already exists for this book.  Dupe this item to change its serial." );
				else
				{
					using ( StreamWriter op = new StreamWriter( filepath ) )
					{
						op.WriteLine("using System;" );
						op.WriteLine("using Server;" );
						op.WriteLine();
						op.WriteLine("namespace Server.Items" );
						op.WriteLine("{" );
						op.WriteLine("	public class {0} : BaseBook", writetitle );
						op.WriteLine("	{" );
						op.WriteLine("		public static readonly BookContent Content = new BookContent" );
						op.WriteLine("			(" );
						op.WriteLine("				\"{0}\", \"{1}\",", book.Title, String.IsNullOrEmpty( book.Author ) ? "unknown" : book.Author );
						BookPageInfo[] pages = book.Pages;
						int lastpage = 0;
						for ( int i = pages.Length-1;i >= 0; i-- )
						{
							if ( pages[i].Lines.Length > 0 ) //This page has something on it
							{
								lastpage = i;
								break;
							}
						}

						for ( int i = 0;i < lastpage; i++ )
						{
							BookPageInfo pageinfo = pages[i];
						op.WriteLine("				new BookPageInfo" );
						op.WriteLine("				(" );
							for ( int j = 0; j < pageinfo.Lines.Length; j++ )
						op.WriteLine("					\"{0}\"{1}", pageinfo.Lines[j], j < pageinfo.Lines.Length-1 ? "," : "" );
						op.WriteLine("				){0}", i < pages.Length-1 ? "," : "" );
						}
						op.WriteLine("			);" );
						op.WriteLine();
						op.WriteLine("		public override BookContent DefaultContent{ get{ return Content; } }" );
						op.WriteLine();
						op.WriteLine("		[Constructable]" );
						op.WriteLine("		public {0}() : base( {1}, false )", writetitle, book.ItemID );
						op.WriteLine("		{" );
						op.WriteLine("			LootType = LootType.Blessed;" );
						op.WriteLine("		{" );
						op.WriteLine("		public {0}( Serial serial ) : base( serial )", writetitle );
						op.WriteLine("		{" );
						op.WriteLine("		}" );
						op.WriteLine();
						op.WriteLine("		public override void Serialize( GenericWriter writer )" );
						op.WriteLine("		{" );
						op.WriteLine("			base.Serialize( writer );");
						op.WriteLine("			writer.WriteEncodedInt( (int)0 ); // version");
						op.WriteLine("		}" );
						op.WriteLine();
						op.WriteLine("		public override void Deserialize( GenericReader reader )" );
						op.WriteLine("		{" );
						op.WriteLine("			base.Deserialize( reader );" );
						op.WriteLine("			int version = reader.ReadEncodedInt();" );
						op.WriteLine("		}" );
						op.WriteLine("	}" );
						op.WriteLine("}" );
					}
					from.SendMessage( "finished." );
				}
			}
			catch ( Exception e )
			{
				from.SendMessage( "failed. {0}", e.ToString() );
			}
		}
	}
}