using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class ImportPremSpawners
	{
		public static void Initialize()
		{
			CommandSystem.Register( "ImportPremSpawners", AccessLevel.Owner, new CommandEventHandler( ImportPremSpawners_OnCommand ) );
		}

		private static void ImportPremSpawners_OnCommand( CommandEventArgs e )
		{
			if ( !Directory.Exists( "Export/Spawners" ) )
				Directory.CreateDirectory( "Export/Spawners" );

			string file = e.ArgString.Trim().ToLower();

			string filePath = Path.Combine( "Export/Spawners", file );

			if ( File.Exists( filePath ) )
			{
				//overrides
				int mapo = -1;
				double mino = -1;
				double maxo = -1;

				using ( StreamReader ip = new StreamReader( filePath ) )
				{
					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						string[] split = line.Split( '|' );
						string[] splitA = line.Split( ' ' );

						if ( splitA.Length == 2 )
						{
							if ( splitA[0].ToLower() == "overridemap" )
								mapo = Utility.ToInt32( splitA[1] );
							if ( splitA[0].ToLower() == "overridemintime" )
								mino = Utility.ToDouble( splitA[1] );
							if ( splitA[0].ToLower() == "overridemaxtime" )
								maxo = Utility.ToDouble( splitA[1] );

							if ( mino > maxo )
							{
								double temp = mino;
								mino = maxo;
								maxo = mino;
							}
						}

						switch( split[0].ToLower() ) 
						{
							case "##":
								break;
							case "*":
							{
								for ( int i = 1; i < 7; i++ )
								{
									if ( !String.IsNullOrEmpty( split[i] ) )
									{
										Spawner spawner = new Spawner();
										spawner.Entries.AddRange( GetEntries( Utility.ToInt32(split[15+i]), split[i].Split(':') ) );

										spawner.Location = new Point3D( Utility.ToInt32( split[7] ), Utility.ToInt32( split[8] ), Utility.ToInt32( split[9] ) );

										switch( mapo > -1 ? mapo : Utility.ToInt32( split[10] ) )
										{
											case 1: spawner.Map = Map.Felucca; break;
											case 2: spawner.Map = Map.Trammel; break;
											case 3: spawner.Map = Map.Ilshenar; break;
											case 4: spawner.Map = Map.Malas; break;
											case 5: spawner.Map = Map.Tokuno; break;
										}

										double min = mino > -1 ? mino : Utility.ToDouble( split[11] );
										double max = maxo > -1 ? maxo : Utility.ToDouble( split[12] );

										if ( min > max )
										{
											double temp = min;
											min = max;
											max = temp;
										}

										spawner.Count = Utility.ToInt32(split[15+i]);

										spawner.MinDelay = TimeSpan.FromMinutes( min );
										spawner.MaxDelay = TimeSpan.FromMinutes( max );

										spawner.HomeRange = Utility.ToInt32( split[13] );
										spawner.WalkingRange = Utility.ToInt32( split[14] );
									}
								}

								break;
							}
						}
					}
				}
			}
			else
				e.Mobile.SendMessage( String.Format( "The file Export/Spawners/{0} doesn't exist", file ) );
		}

		public static List<SpawnerEntry> GetEntries( int count, string[] list )
		{
			List<SpawnerEntry> entries = new List<SpawnerEntry>();

			//list of types
			for ( int i = 0; i < list.Length; i++ )
				entries.Add( new SpawnerEntry( list[i], 100, count ) );

			return entries;
		}
	}
}