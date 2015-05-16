using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class ImportSpawners
	{
		public static void Initialize()
		{
			CommandSystem.Register( "ImportSpawners", AccessLevel.Owner, new CommandEventHandler( ImportSpawners_OnCommand ) );
		}

		private static void ImportSpawners_OnCommand( CommandEventArgs e )
		{
			if ( !Directory.Exists( "Export/Spawners" ) )
				Directory.CreateDirectory( "Export/Spawners" );

			string file = e.ArgString.Trim().ToLower();

			string filePath = Path.Combine( "Export/Spawners", file );

			if ( File.Exists( filePath ) )
			{

				XmlDocument doc = new XmlDocument();
				doc.Load( filePath );

				XmlElement root = doc["Spawners"];

				foreach ( XmlElement spawner in root.GetElementsByTagName( "Spawner" ) )
					LoadSpawner( spawner );
			}
			else
				e.Mobile.SendMessage( String.Format( "The file Export/Spawners/{0} doesn't exist", file ) );
		}

		public static void LoadSpawner( XmlElement node )
		{
			Spawner spawner = new Spawner();

			spawner.Name = Utility.GetText( node["Name"], null );
			spawner.Team = Utility.GetXMLInt32( Utility.GetText( node["Team"], "0" ), 0 );
			spawner.HomeRange = Utility.GetXMLInt32( Utility.GetText( node["HomeRange"], "0" ), 0 );
			spawner.WalkingRange = Utility.GetXMLInt32( Utility.GetText( node["WalkingRange"], "-1" ), -1 );
			spawner.Count = Utility.GetXMLInt32( Utility.GetText( node["Count"], "1" ), 1 );
			spawner.MinDelay = Utility.GetXMLTimeSpan( Utility.GetText( node["MinDelay"], "00:15:00" ), TimeSpan.FromMinutes( 15.0 ) );
			spawner.MaxDelay = Utility.GetXMLTimeSpan( Utility.GetText( node["MaxDelay"], "00:15:00" ), TimeSpan.FromMinutes( 15.0 ) );
			spawner.Location = Point3D.Parse( Utility.GetText( node["Location"], "(0,0,0)" ) );
			spawner.Map = Map.Parse( Utility.GetText( node["Map"], "Felucca" ) );

			XmlElement entries = node["Entries"];

			foreach ( XmlElement entry in entries.GetElementsByTagName( "Entry" ) )
				LoadEntry( spawner, entry );
		}

		public static void LoadEntry( Spawner spawner, XmlElement node )
		{
			string name = Utility.GetText( node["SpawnedName"], null );
			int prob = Utility.GetXMLInt32( Utility.GetText( node["SpawnedProbability"], "100" ), 100 );
			int count = Utility.GetXMLInt32( Utility.GetText( node["SpawnedMaxCount"], spawner.Count.ToString() ), spawner.Count );
			string parameters = Utility.GetText( node["Parameters"], null );
			string properties = Utility.GetText( node["Parameters"], null );

			SpawnerEntry entry = new SpawnerEntry( name, prob, count );
			entry.Parameters = parameters;
			entry.Properties = properties;

			spawner.Entries.Add( entry );
		}
	}
}