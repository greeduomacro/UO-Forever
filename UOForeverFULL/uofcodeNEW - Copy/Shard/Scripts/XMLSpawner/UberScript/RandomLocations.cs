#region References
using System;
using System.Collections.Generic;
using System.IO;

using Server.Commands;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class WeightedLocation
	{
		public Rectangle2D Rectangle;
		public int Weighting;

		public WeightedLocation(Point2D topLeft, Point2D bottomRight, int weighting)
		{
			Rectangle = new Rectangle2D(topLeft, bottomRight);
			Weighting = weighting;
		}
	}

	public class RandomLocations
	{
		public const string ConfigFileName = "Data/dungeonRectangles.txt";

		public static int TotalWeighting = 0;
		public static List<WeightedLocation> WeightedLocations = new List<WeightedLocation>();

		public static void Initialize()
		{
			Reload();
			CommandSystem.Register("RandomLocationsReload", AccessLevel.GameMaster, RandomLocationsReload_Command);
		}

		public static void RandomLocationsReload_Command(CommandEventArgs e)
		{
			Reload();
		}

		public static Rectangle2D GetRandomRectangle()
		{
			if (WeightedLocations.Count == 0)
			{
				LoggingCustom.Log(
					"ERROR-RandomLocations.txt", "WeightedLocations.Count == 0: resorting to default rectangle (5, 5, 5, 5)");
				return new Rectangle2D(5, 5, 5, 5); // default rectangle just in case
			}

			int roll = Utility.Random(TotalWeighting);

			foreach (WeightedLocation loc in WeightedLocations)
			{
				roll -= loc.Weighting;

				if (roll < 0)
				{
					return loc.Rectangle;
				}
			}

			return WeightedLocations[WeightedLocations.Count - 1].Rectangle;
		}

		public static void Reload()
		{
			WeightedLocations = new List<WeightedLocation>();
			TotalWeighting = 0;

			List<string> errors = new List<string>();

			string line;

			// Read the file and display it line by line.
			try
			{
				StreamReader file = new StreamReader(ConfigFileName);
				int lineNumber = 0;

				while ((line = file.ReadLine()) != null)
				{
					lineNumber++;

					line = line.Trim();

					if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
						continue;

					var args = line.Split();

					try
					{
						if (args.Length < 2)
						{
							throw new Exception(
								"Line " + lineNumber + ": " + line +
								"\nUncommented line did not have format weighting<space>rectangleTopLeftX,rectangleTopLeftY,rectangleBottomRightX,rectangleBottomRightY");
						}

						// syntax is 
						// weighting<space>rectangleTopLeftX,rectangleTopLeftY,rectangleBottomRightX,rectangleBottomRightY
						int weighting = int.Parse(args[0]);
						var coordinateVals = args[1].Split(',');
						int rectangleTopLeftX = int.Parse(coordinateVals[0]);
						int rectangleTopLeftY = int.Parse(coordinateVals[1]);
						int rectangleBottomRightX = int.Parse(coordinateVals[2]);
						int rectangleBottomRightY = int.Parse(coordinateVals[3]);

						WeightedLocations.Add(
							new WeightedLocation(
								new Point2D(rectangleTopLeftX, rectangleTopLeftY),
								new Point2D(rectangleBottomRightX, rectangleBottomRightY),
								weighting));
						TotalWeighting += weighting;
					}
					catch (Exception e)
					{
						errors.Add(e.Message);
					}
				}

				file.Close();
			}
			catch (Exception e)
			{
				errors.Add(e.ToString());
			}

			if (errors.Count > 0)
			{
				LoggingCustom.Log("ERROR-RandomLocations.txt", errors.ToArray());
			}
		}
	}
}