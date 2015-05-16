/***************************************************************************
 *                                  Body.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region References
using System;
using System.IO;
#endregion

namespace Server
{
	public enum BodyType : byte
	{
		Empty,
		Monster,
		Sea,
		Animal,
		Human,
		Equipment
	}

	public struct Body
	{
		private static readonly BodyType[] _Types;

		static Body()
		{
			if (File.Exists("Data/bodyTable.cfg"))
			{
				using (StreamReader ip = new StreamReader("Data/bodyTable.cfg"))
				{
					_Types = new BodyType[0x1000];

					string line;

					while ((line = ip.ReadLine()) != null)
					{
						if (line.Length == 0 || line.StartsWith("#"))
						{
							continue;
						}

						var split = line.Split('\t');

						BodyType type;
						int bodyID;

						if (int.TryParse(split[0], out bodyID) && Enum.TryParse(split[1], true, out type) && bodyID >= 0 &&
							bodyID < _Types.Length)
						{
							_Types[bodyID] = type;
						}
						else
						{
							Console.WriteLine("Warning: Invalid bodyTable entry:");
							Console.WriteLine(line);
						}
					}
				}
			}
			else
			{
				Console.WriteLine("Warning: Data/bodyTable.cfg does not exist");

				_Types = new BodyType[0];
			}
		}

		public Body(int bodyID)
			: this()
		{
			BodyID = bodyID;
		}

		public BodyType Type { get { return BodyID >= 0 && BodyID < _Types.Length ? _Types[BodyID] : BodyType.Empty; } }

		public bool IsHuman
		{
			get
			{
				return (BodyID >= 0 && BodyID < _Types.Length && _Types[BodyID] == BodyType.Human && BodyID != 402 && BodyID != 403 &&
						BodyID != 607 && BodyID != 608 && BodyID != 970) || BodyID == 694 || BodyID == 695;
			}
		}

		public bool IsMale
		{
			get
			{
				return BodyID == 183 || BodyID == 185 || BodyID == 400 || BodyID == 402 || BodyID == 605 || BodyID == 607 ||
					   BodyID == 750 || BodyID == 666 || BodyID == 694 || BodyID == 1253;
			}
		}

		public bool IsFemale
		{
			get
			{
				return BodyID == 184 || BodyID == 186 || BodyID == 401 || BodyID == 403 || BodyID == 606 || BodyID == 608 ||
					   BodyID == 751 || BodyID == 667 || BodyID == 695;
			}
		}

		public bool IsGargoyle { get { return BodyID == 666 || BodyID == 667; } }

		public bool IsGhost
		{
			get
			{
				return BodyID == 402 || BodyID == 403 || BodyID == 607 || BodyID == 608 || BodyID == 694 || BodyID == 695 ||
					   BodyID == 970;
			}
		}

		public bool IsMonster { get { return BodyID >= 0 && BodyID < _Types.Length && _Types[BodyID] == BodyType.Monster; } }
		public bool IsAnimal { get { return BodyID >= 0 && BodyID < _Types.Length && _Types[BodyID] == BodyType.Animal; } }
		public bool IsEmpty { get { return BodyID >= 0 && BodyID < _Types.Length && _Types[BodyID] == BodyType.Empty; } }
		public bool IsSea { get { return BodyID >= 0 && BodyID < _Types.Length && _Types[BodyID] == BodyType.Sea; } }
		public bool IsEquipment { get { return BodyID >= 0 && BodyID < _Types.Length && _Types[BodyID] == BodyType.Equipment; } }

		public int BodyID { get; private set; }

		public override string ToString()
		{
			return string.Format("0x{0:X}", BodyID);
		}

		public override int GetHashCode()
		{
			return BodyID;
		}

		public override bool Equals(object o)
		{
			return o is Body && ((Body)o).BodyID == BodyID;
		}

		public static bool operator ==(Body l, Body r)
		{
			return l.BodyID == r.BodyID;
		}

		public static bool operator !=(Body l, Body r)
		{
			return l.BodyID != r.BodyID;
		}

		public static bool operator >(Body l, Body r)
		{
			return l.BodyID > r.BodyID;
		}

		public static bool operator >=(Body l, Body r)
		{
			return l.BodyID >= r.BodyID;
		}

		public static bool operator <(Body l, Body r)
		{
			return l.BodyID < r.BodyID;
		}

		public static bool operator <=(Body l, Body r)
		{
			return l.BodyID <= r.BodyID;
		}

		public static implicit operator int(Body a)
		{
			return a.BodyID;
		}

		public static implicit operator Body(int a)
		{
			return new Body(a);
		}
	}
}