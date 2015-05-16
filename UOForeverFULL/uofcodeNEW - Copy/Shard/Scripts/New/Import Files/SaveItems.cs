using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;

namespace Server
{
	public static class SaveItemHandler
	{
		public static void Initialize()
		{
			//SaveItems();
			//LoadItems();
		}

		public static void LoadItems()
		{
			Console.Write( "Start Loading items..." );

			string filePath = "itemstosave.xml";

			if ( !File.Exists( filePath ) )
				return;

			XmlDocument doc = new XmlDocument();
			doc.Load( filePath );

			XmlElement root = doc["items"];

			foreach ( XmlElement node in root.GetElementsByTagName( "item" ) )
			{
				string itemstring = Utility.GetText( node["type"], null );
				if ( !String.IsNullOrEmpty( itemstring ) )
				{
					Type itemtype = ScriptCompiler.FindTypeByFullName( itemstring, true );

					if ( itemtype != null )
					{
						Item item = Activator.CreateInstance( itemtype ) as Item;

						if ( item != null )
						{
							item.ItemID = Utility.GetXMLInt32( Utility.GetText( node["itemid"], "0" ), 0 );
							item.Hue = Utility.GetXMLInt32( Utility.GetText( node["hue"], "0" ), 0 );
							item.Movable = false;

							int x = Utility.GetXMLInt32( Utility.GetAttribute( node, "x" ), 0 );
							int y = Utility.GetXMLInt32( Utility.GetAttribute( node, "y" ), 0 );
							int z = Utility.GetXMLInt32( Utility.GetAttribute( node, "z" ), 0 );
							Map map = Map.Parse( Utility.GetAttribute( node, "map" ) );

							item.MoveToWorld( new Point3D( x, y, z ), map );

							if ( item is FillableContainer )
								((FillableContainer)item).ContentType = (FillableContentType)Utility.GetXMLInt32( Utility.GetText( node["contenttype"], "0" ), 0 );

							if ( item is LockableContainer )
							{
								LockableContainer cont = item as LockableContainer;

								cont.MaxLockLevel = Utility.GetXMLInt32( Utility.GetText( node["maxlocklevel"], "0" ), 0 );
								cont.LockLevel = Utility.GetXMLInt32( Utility.GetText( node["maxlocklevel"], "0" ), 0 );
								cont.RequiredSkill = Utility.GetXMLInt32( Utility.GetText( node["maxlocklevel"], "0" ), 0 );

								cont.Locked = Utility.GetText( node["locked"], "false" ) == "true";
								cont.TrapOnLockpick = Utility.GetText( node["traponlockpick"], "false" ) == "true";
							}

							if ( item is BaseLight )
							{
								BaseLight light = item as BaseLight;
								light.Protected = Utility.GetText( node["protected"], "true" ) == "true";
								light.Duration = Utility.GetXMLTimeSpan( Utility.GetText( node["duration"], "0" ), TimeSpan.Zero );
								light.Burning = Utility.GetText( node["burning"], "true" ) == "true";
							}
						}
						else
							Console.WriteLine( "Error loading: {0}", itemtype.FullName );
					}
					else
						Console.WriteLine( "Error loading type from xml: {0}", itemstring );
				}
				else
					Console.WriteLine( "Error loading string from xml: {0}", itemstring );
			}

			Console.WriteLine( "done." );
		}

		public static void SaveItems()
		{
			List<Item> items = new List<Item>();

			Console.Write( "Start Caching items..." );
/*
			foreach ( Item itemcheck in World.Items.Values )
			{
				if ( BaseHouse.FindHouseAt( itemcheck ) == null && !itemcheck.Movable && itemcheck.RootParent == null && itemcheck.Map != null && itemcheck.Map != Map.Internal )
				{
					if ( itemcheck is BaseCraftableItem )
						items.Add( itemcheck );
					else if ( itemcheck is BaseFurnitureContainer )
						items.Add( itemcheck );
					else if ( itemcheck is LockableContainer && !(itemcheck is LockableBarrel) && !(itemcheck is MarkContainer) )
						items.Add( itemcheck );
					else if ( itemcheck is BaseLight )
						items.Add( itemcheck );
				}
			}
*/
			Console.WriteLine( "done." );

			Console.Write( "Start Saving items..." );

			string filePath = "itemstosave.xml";

			using ( StreamWriter op = new StreamWriter( filePath ) )
			{
				XmlTextWriter xml = new XmlTextWriter( op );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;

				xml.WriteStartDocument( true );

				xml.WriteStartElement( "items" );

				xml.WriteAttributeString( "count", items.Count.ToString() );

				for ( int i = 0;i < items.Count; i++ )
				{
					Item item = items[i];

					xml.WriteStartElement( "item" );

					xml.WriteAttributeString( "x", item.X.ToString() );
					xml.WriteAttributeString( "y", item.Y.ToString() );
					xml.WriteAttributeString( "z", item.Z.ToString() );
					xml.WriteAttributeString( "map", item.Map.ToString() );

					xml.WriteStartElement( "type" );
					xml.WriteString( item.GetType().FullName );
					xml.WriteEndElement();

					xml.WriteStartElement( "hue" );
					xml.WriteString( item.Hue.ToString() );
					xml.WriteEndElement();

					xml.WriteStartElement( "itemid" );
					xml.WriteString( item.ItemID.ToString() );
					xml.WriteEndElement();

					if ( item is FillableContainer )
					{
						xml.WriteStartElement( "contenttype" );
						xml.WriteValue( (int)((FillableContainer)item).ContentType );
						xml.WriteEndElement();
					}

					if ( item is LockableContainer )
					{
						xml.WriteStartElement( "maxlocklevel" );
						xml.WriteValue( ((LockableContainer)item).MaxLockLevel );
						xml.WriteEndElement();

						xml.WriteStartElement( "locklevel" );
						xml.WriteValue( ((LockableContainer)item).LockLevel );
						xml.WriteEndElement();

						xml.WriteStartElement( "requiredskill" );
						xml.WriteValue( ((LockableContainer)item).RequiredSkill );
						xml.WriteEndElement();

						xml.WriteStartElement( "locked" );
						xml.WriteString( ((LockableContainer)item).Locked ? "true" : "false" );
						xml.WriteEndElement();

						xml.WriteStartElement( "traponlockpick" );
						xml.WriteString( ((LockableContainer)item).TrapOnLockpick ? "true" : "false" );
						xml.WriteEndElement();
					}

					if ( item is BaseLight )
					{
						xml.WriteStartElement( "protected" );
						xml.WriteString( ((BaseLight)item).Protected ? "true" : "false" );
						xml.WriteEndElement();

						xml.WriteStartElement( "duration" );
						xml.WriteValue( ((BaseLight)item).Duration );
						xml.WriteEndElement();

						xml.WriteStartElement( "burning" );
						xml.WriteString( ((BaseLight)item).Burning ? "true" : "false" );
						xml.WriteEndElement();
					}

					xml.WriteEndElement();
				}

				xml.WriteEndElement();

				xml.Close();
			}

			Console.WriteLine( "done." );
		}
	}
}