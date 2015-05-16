using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class ExportSpawners
	{
		public static void Initialize()
		{
			CommandSystem.Register( "ExportSpawners", AccessLevel.Owner, new CommandEventHandler( ExportSpawners_OnCommand ) );
		}

		private static void ExportSpawners_OnCommand( CommandEventArgs e )
		{
			if ( !Directory.Exists( "Export/Spawners" ) )
				Directory.CreateDirectory( "Export/Spawners" );

			string filePath = Path.Combine( "Export/Spawners", String.Format( "spawners-{0}-{1}-{2}.xml", DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day ) );

			using ( StreamWriter op = new StreamWriter( filePath ) )
			{
				XmlTextWriter xml = new XmlTextWriter( op );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;

				xml.WriteStartDocument( true );

				xml.WriteStartElement( "Spawners" );

				List<Spawner> spawners = new List<Spawner>();

				foreach ( Item item in World.Items.Values )
					if ( item is Spawner && ((Spawner)item).Running )
						spawners.Add( (Spawner)item );

				xml.WriteAttributeString( "Count", spawners.Count.ToString() );

				foreach ( Spawner spawner in spawners )
					WriteSpawner( xml, spawner );

				xml.WriteEndElement();

				xml.Close();
			}
		}

		public static void WriteSpawner( XmlTextWriter xml, Spawner spawner )
		{
			xml.WriteStartElement( "Spawner" );

			if ( spawner is ProximitySpawner || spawner is MasterDistSpawner )
				xml.WriteAttributeString( "Type", spawner.GetType().Name );

			//xml.WriteAttributeString( "Active", spawner.Running.ToString() );

			if ( !String.IsNullOrEmpty( spawner.Name ) && spawner.Name != "Spawner" && spawner.Name != spawner.DefaultName )
			{
				xml.WriteStartElement( "Name" );
				xml.WriteString( spawner.Name );
				xml.WriteEndElement();
			}

			if ( spawner.Team != 0 )
			{
				xml.WriteStartElement( "Team" );
				xml.WriteString( spawner.Team.ToString() );
				xml.WriteEndElement();
			}

			xml.WriteStartElement( "HomeRange" );
			xml.WriteString( spawner.HomeRange.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "Count" );
			xml.WriteString( spawner.Count.ToString() );
			xml.WriteEndElement();

			if ( spawner.WalkingRange >= 0 )
			{
				xml.WriteStartElement( "WalkingRange" );
				xml.WriteString( spawner.WalkingRange.ToString() );
				xml.WriteEndElement();
			}

			xml.WriteStartElement( "MinDelay" );
			xml.WriteString( spawner.MinDelay.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "MaxDelay" );
			xml.WriteString( spawner.MinDelay.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "Location" );
			xml.WriteString( spawner.Location.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "Map" );
			xml.WriteString( spawner.Map.ToString() );
			xml.WriteEndElement();

			if ( spawner.WayPoint != null )
			{
				xml.WriteStartElement( "WayPoint" );
				xml.WriteString( spawner.WayPoint.Serial.ToString() );
				xml.WriteEndElement();
			}

			if ( spawner is ProximitySpawner )
			{
				ProximitySpawner prox = spawner as ProximitySpawner;

				xml.WriteAttributeString( "InstantFlag", prox.InstantFlag.ToString() );

				xml.WriteStartElement( "TriggerRange" );
				xml.WriteString( prox.TriggerRange.ToString() );
				xml.WriteEndElement();

				if ( prox.SpawnMessage != null )
				{
					xml.WriteStartElement( "SpawnMessage" );
					xml.WriteAttributeString( "Local", prox.SpawnMessage.Number.ToString() );
					xml.WriteString( prox.SpawnMessage.String );
					xml.WriteEndElement();
				}
			}
			else if ( spawner is MasterDistSpawner )
			{
				MasterDistSpawner master = spawner as MasterDistSpawner;

				xml.WriteStartElement( "IsChild" );
				xml.WriteString( master.IsChild.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "Children" );
				xml.WriteAttributeString( "Count", (master.Children.Count - ( master.IsChild ? 1 : 0 )).ToString() );

				foreach ( IChildDistSpawner child in master.Children )
					if ( child != spawner && child is ChildDistSpawner )
						WriteChildDistSpawner( xml, (ChildDistSpawner)child );

				xml.WriteEndElement(); // Children
			}

			xml.WriteStartElement( "Entries" );
			xml.WriteAttributeString( "Count", spawner.Entries.Count.ToString() );

			int totalProb = 0;

			foreach ( SpawnerEntry entry in spawner.Entries )
				if ( !String.IsNullOrEmpty( entry.SpawnedName ) )
					totalProb += entry.SpawnedProbability;

			xml.WriteAttributeString( "TotalProb", totalProb.ToString() );

			foreach ( SpawnerEntry entry in spawner.Entries )
				if ( !String.IsNullOrEmpty( entry.SpawnedName ) )
					WriteSpawnerEntry( xml, entry );

			xml.WriteEndElement(); // Entries

			xml.WriteEndElement(); // Spawner
		}

		public static void WriteChildDistSpawner( XmlTextWriter xml, ChildDistSpawner child )
		{
			xml.WriteStartElement( "Child" );

			xml.WriteStartElement( "Location" );
			xml.WriteString( child.Location.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "Map" );
			xml.WriteString( child.Map.ToString() );
			xml.WriteEndElement();

			if ( child.HomeRange > 0 )
			{
				xml.WriteStartElement( "HomeRange" );
				xml.WriteString( child.HomeRange.ToString() );
				xml.WriteEndElement();
			}

			if ( child.WalkingRange > 0 )
			{
				xml.WriteStartElement( "WalkingRange" );
				xml.WriteString( child.WalkingRange.ToString() );
				xml.WriteEndElement();
			}

			if ( child.WayPoint != null )
			{
				xml.WriteStartElement( "WayPoint" );
				xml.WriteString( child.WayPoint.Serial.ToString() );
				xml.WriteEndElement();
			}

			xml.WriteEndElement(); // Child
		}

		public static void WriteSpawnerEntry( XmlTextWriter xml, SpawnerEntry entry )
		{
			xml.WriteStartElement( "Entry" );

			xml.WriteStartElement( "SpawnedName" );
			xml.WriteString( entry.SpawnedName );
			xml.WriteEndElement();

			xml.WriteStartElement( "SpawnedProbability" );
			xml.WriteString( entry.SpawnedProbability.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "SpawnedMaxCount" );
			xml.WriteString( entry.SpawnedMaxCount.ToString() );
			xml.WriteEndElement();

			if ( !String.IsNullOrEmpty( entry.Parameters ) )
			{
				xml.WriteStartElement( "Parameters" );
				xml.WriteString( entry.Parameters );
				xml.WriteEndElement();
			}

			if ( !String.IsNullOrEmpty( entry.Properties ) )
			{
				xml.WriteStartElement( "Properties" );
				xml.WriteString( entry.Properties );
				xml.WriteEndElement();
			}

			if ( entry.Valid != EntryFlags.None )
			{
				xml.WriteStartElement( "Invalid" );
				xml.WriteString( ((int)entry.Valid).ToString() );
				xml.WriteEndElement();
			}

			xml.WriteEndElement(); // Entry
		}
	}
}