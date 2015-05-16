#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Items
{
	public enum CraftResource
	{
		None = 0,
		Iron = 1,
		DullCopper,
		ShadowIron,
		Copper,
		Bronze,
		Gold,
		Agapite,
		Verite,
		Valorite,

		BloodRock,
		BlackRock,

		RegularLeather = 101,
		SpinedLeather,
		HornedLeather,
		BarbedLeather,

		RedScales = 201,
		YellowScales,
		BlackScales,
		GreenScales,
		WhiteScales,
		BlueScales,
		BloodScales,

		RegularWood = 301,
		OakWood,
		AshWood,
		YewWood,
		Heartwood,
		Bloodwood,
		Frostwood
	}

	public enum OreType
	{
		None = CraftResource.None,
		Iron = CraftResource.Iron,
		DullCopper = CraftResource.DullCopper,
		ShadowIron = CraftResource.ShadowIron,
		Copper = CraftResource.Copper,
		Bronze = CraftResource.Bronze,
		Gold = CraftResource.Gold,
		Agapite = CraftResource.Agapite,
		Verite = CraftResource.Verite,
		Valorite = CraftResource.Valorite
	}

	public enum RockType
	{
		None = CraftResource.None,
		BloodRock = CraftResource.BloodRock,
		BlackRock = CraftResource.BlackRock,
	}

	public enum LeatherType
	{
		None = CraftResource.None,
		RegularLeather = CraftResource.RegularLeather,
		SpinedLeather = CraftResource.SpinedLeather,
		HornedLeather = CraftResource.HornedLeather,
		BarbedLeather = CraftResource.BarbedLeather
	}

	public enum ScaleType
	{
		None = CraftResource.None,
		RedScales = CraftResource.RedScales,
		YellowScales = CraftResource.YellowScales,
		BlackScales = CraftResource.BlackScales,
		GreenScales = CraftResource.GreenScales,
		WhiteScales = CraftResource.WhiteScales,
		BlueScales = CraftResource.BlueScales,
		BloodScales = CraftResource.BloodScales
	}

	public enum WoodType
	{
		None = CraftResource.None,
		RegularWood = CraftResource.RegularWood,
		OakWood = CraftResource.OakWood,
		AshWood = CraftResource.AshWood,
		YewWood = CraftResource.YewWood,
		Heartwood = CraftResource.Heartwood,
		Bloodwood = CraftResource.Bloodwood,
		Frostwood = CraftResource.Frostwood
	}

	public enum CraftResourceType
	{
		None,
		Metal,
		Leather,
		Scales,
		Wood
	}

	public static class CraftResourceExt
	{
		public static bool TryConvert(this CraftResource r, out OreType t)
		{
			if (r == CraftResource.None)
			{
				t = OreType.None;
				return true;
			}

			if (r < CraftResource.Iron)
			{
				t = OreType.None;
				return false;
			}

			if (r > CraftResource.Valorite)
			{
				t = OreType.None;
				return false;
			}

			t = (OreType)r;
			return true;
		}

		public static bool TryConvert(this CraftResource r, out RockType t)
		{
			if (r == CraftResource.None)
			{
				t = RockType.None;
				return true;
			}

			if (r < CraftResource.BloodRock)
			{
				t = RockType.None;
				return false;
			}

			if (r > CraftResource.BlackRock)
			{
				t = RockType.None;
				return false;
			}

			t = (RockType)r;
			return true;
		}

		public static bool TryConvert(this CraftResource r, out LeatherType t)
		{
			if (r == CraftResource.None)
			{
				t = LeatherType.None;
				return true;
			}

			if (r < CraftResource.RegularLeather)
			{
				t = LeatherType.None;
				return false;
			}

			if (r > CraftResource.BarbedLeather)
			{
				t = LeatherType.None;
				return false;
			}

			t = (LeatherType)r;
			return true;
		}

		public static bool TryConvert(this CraftResource r, out ScaleType t)
		{
			if (r == CraftResource.None)
			{
				t = ScaleType.None;
				return true;
			}

			if (r < CraftResource.RedScales)
			{
				t = ScaleType.None;
				return false;
			}

			if (r > CraftResource.BloodScales)
			{
				t = ScaleType.None;
				return false;
			}

			t = (ScaleType)r;
			return true;
		}

		public static bool TryConvert(this CraftResource r, out WoodType t)
		{
			if (r == CraftResource.None)
			{
				t = WoodType.None;
				return true;
			}

			if (r < CraftResource.RegularWood)
			{
				t = WoodType.None;
				return false;
			}

			if (r > CraftResource.Frostwood)
			{
				t = WoodType.None;
				return false;
			}

			t = (WoodType)r;
			return true;
		}
	}

	public class CraftAttributeInfo
	{
		public int WeaponFireDamage { get; set; }
		public int WeaponColdDamage { get; set; }
		public int WeaponPoisonDamage { get; set; }
		public int WeaponEnergyDamage { get; set; }
		public int WeaponChaosDamage { get; set; }
		public int WeaponDirectDamage { get; set; }
		public int WeaponDurability { get; set; }
		public int WeaponLuck { get; set; }
		public int WeaponGoldIncrease { get; set; }
		public int WeaponLowerRequirements { get; set; }

		public int ArmorPhysicalResist { get; set; }
		public int ArmorFireResist { get; set; }
		public int ArmorColdResist { get; set; }
		public int ArmorPoisonResist { get; set; }
		public int ArmorEnergyResist { get; set; }
		public int ArmorDurability { get; set; }
		public int ArmorLuck { get; set; }
		public int ArmorGoldIncrease { get; set; }
		public int ArmorLowerRequirements { get; set; }

		public int RunicMinAttributes { get; set; }
		public int RunicMaxAttributes { get; set; }
		public int RunicMinIntensity { get; set; }
		public int RunicMaxIntensity { get; set; }

		public static readonly CraftAttributeInfo Blank;
		public static readonly CraftAttributeInfo DullCopper;
		public static readonly CraftAttributeInfo ShadowIron;
		public static readonly CraftAttributeInfo Copper;
		public static readonly CraftAttributeInfo Bronze;
		public static readonly CraftAttributeInfo Golden;
		public static readonly CraftAttributeInfo Agapite;
		public static readonly CraftAttributeInfo Verite;
		public static readonly CraftAttributeInfo Valorite;

		public static readonly CraftAttributeInfo BloodRock;
		public static readonly CraftAttributeInfo BlackRock;

		public static readonly CraftAttributeInfo Spined;
		public static readonly CraftAttributeInfo Horned;
		public static readonly CraftAttributeInfo Barbed;

		public static readonly CraftAttributeInfo RedScales;
		public static readonly CraftAttributeInfo YellowScales;
		public static readonly CraftAttributeInfo BlackScales;
		public static readonly CraftAttributeInfo GreenScales;
		public static readonly CraftAttributeInfo WhiteScales;
		public static readonly CraftAttributeInfo BlueScales;
		public static readonly CraftAttributeInfo BloodScales;

		public static readonly CraftAttributeInfo OakWood;
		public static readonly CraftAttributeInfo AshWood;
		public static readonly CraftAttributeInfo YewWood;
		public static readonly CraftAttributeInfo Heartwood;
		public static readonly CraftAttributeInfo Bloodwood;
		public static readonly CraftAttributeInfo Frostwood;

		static CraftAttributeInfo()
		{
			Blank = new CraftAttributeInfo();

			BloodRock = new CraftAttributeInfo();
			BlackRock = new CraftAttributeInfo();

			DullCopper = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 6,
				ArmorDurability = 50,
				ArmorLowerRequirements = 20,
				WeaponDurability = 100,
				WeaponLowerRequirements = 50,
				RunicMinAttributes = 1,
				RunicMaxAttributes = 2,
				RunicMinIntensity = 10,
				RunicMaxIntensity = 35
			};

			ShadowIron = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 2,
				ArmorFireResist = 1,
				ArmorEnergyResist = 5,
				ArmorDurability = 100,
				WeaponColdDamage = 20,
				WeaponDurability = 50,
				RunicMinAttributes = 2,
				RunicMaxAttributes = 2,
				RunicMinIntensity = 20,
				RunicMaxIntensity = 45
			};

			Copper = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 1,
				ArmorFireResist = 1,
				ArmorPoisonResist = 5,
				ArmorEnergyResist = 2,
				WeaponPoisonDamage = 10,
				WeaponEnergyDamage = 20,
				RunicMinAttributes = 2,
				RunicMaxAttributes = 3,
				RunicMinIntensity = 25,
				RunicMaxIntensity = 50
			};

			Bronze = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 3,
				ArmorColdResist = 5,
				ArmorPoisonResist = 1,
				ArmorEnergyResist = 1,
				WeaponFireDamage = 40,
				RunicMinAttributes = 3,
				RunicMaxAttributes = 3,
				RunicMinIntensity = 30,
				RunicMaxIntensity = 65
			};

			Golden = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 1,
				ArmorFireResist = 1,
				ArmorColdResist = 2,
				ArmorEnergyResist = 2,
				ArmorLuck = 40,
				ArmorLowerRequirements = 30,
				WeaponLuck = 40,
				WeaponLowerRequirements = 50,
				RunicMinAttributes = 3,
				RunicMaxAttributes = 4,
				RunicMinIntensity = 35,
				RunicMaxIntensity = 75
			};

			Agapite = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 2,
				ArmorFireResist = 3,
				ArmorColdResist = 2,
				ArmorPoisonResist = 2,
				ArmorEnergyResist = 2,
				WeaponColdDamage = 30,
				WeaponEnergyDamage = 20,
				RunicMinAttributes = 4,
				RunicMaxAttributes = 4,
				RunicMinIntensity = 40,
				RunicMaxIntensity = 80
			};

			Verite = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 3,
				ArmorFireResist = 3,
				ArmorColdResist = 2,
				ArmorPoisonResist = 3,
				ArmorEnergyResist = 1,
				WeaponPoisonDamage = 40,
				WeaponEnergyDamage = 20,
				RunicMinAttributes = 4,
				RunicMaxAttributes = 5,
				RunicMinIntensity = 45,
				RunicMaxIntensity = 90
			};

			Valorite = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 4,
				ArmorColdResist = 3,
				ArmorPoisonResist = 3,
				ArmorEnergyResist = 3,
				ArmorDurability = 50,
				WeaponFireDamage = 10,
				WeaponColdDamage = 20,
				WeaponPoisonDamage = 10,
				WeaponEnergyDamage = 20,
				RunicMinAttributes = 5,
				RunicMaxAttributes = 5,
				RunicMinIntensity = 50,
				RunicMaxIntensity = 100
			};

			Spined = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 5,
				ArmorLuck = 40,
				RunicMinAttributes = 1,
				RunicMaxAttributes = 3,
				RunicMinIntensity = 20,
				RunicMaxIntensity = 40
			};

			Horned = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 2,
				ArmorFireResist = 3,
				ArmorColdResist = 2,
				ArmorPoisonResist = 2,
				ArmorEnergyResist = 2,
				RunicMinAttributes = 3,
				RunicMaxAttributes = 4,
				RunicMinIntensity = 30,
				RunicMaxIntensity = 70
			};

			Barbed = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 2,
				ArmorFireResist = 1,
				ArmorColdResist = 2,
				ArmorPoisonResist = 3,
				ArmorEnergyResist = 4,
				RunicMinAttributes = 4,
				RunicMaxAttributes = 5,
				RunicMinIntensity = 40,
				RunicMaxIntensity = 100
			};

			RedScales = new CraftAttributeInfo
			{
				ArmorFireResist = 10,
				ArmorColdResist = -3
			};

			YellowScales = new CraftAttributeInfo
			{
				ArmorPhysicalResist = -3,
				ArmorLuck = 20
			};

			BlackScales = new CraftAttributeInfo
			{
				ArmorPhysicalResist = 10,
				ArmorEnergyResist = -3
			};

			GreenScales = new CraftAttributeInfo
			{
				ArmorFireResist = -3,
				ArmorPoisonResist = 10
			};

			WhiteScales = new CraftAttributeInfo
			{
				ArmorPhysicalResist = -3,
				ArmorColdResist = 10
			};

			BlueScales = new CraftAttributeInfo
			{
				ArmorPoisonResist = -3,
				ArmorEnergyResist = 10
			};

			BloodScales = new CraftAttributeInfo();

			OakWood = new CraftAttributeInfo();
			AshWood = new CraftAttributeInfo();
			YewWood = new CraftAttributeInfo();
			Heartwood = new CraftAttributeInfo();
			Bloodwood = new CraftAttributeInfo();
			Frostwood = new CraftAttributeInfo();
		}
	}

	public class CraftResourceInfo
	{
		public int Hue { get; private set; }
		public int Number { get; private set; }
		public string Name { get; private set; }
		public CraftAttributeInfo AttributeInfo { get; private set; }
		public CraftResource Resource { get; private set; }
		public Type[] ResourceTypes { get; private set; }

		public CraftResourceInfo(
			int hue,
			int number,
			string name,
			CraftAttributeInfo attributeInfo,
			CraftResource resource,
			params Type[] resourceTypes)
		{
			Hue = hue;
			Number = number;
			Name = name;
			AttributeInfo = attributeInfo;
			Resource = resource;
			ResourceTypes = resourceTypes;

			foreach (Type t in resourceTypes)
			{
				CraftResources.RegisterType(t, resource);
			}
		}
	}

	public class CraftResources
	{
		public static CraftResourceInfo[] m_MetalInfo = new[]
		{
			new CraftResourceInfo(
				0x000,
				1053109,
				"Iron",
				CraftAttributeInfo.Blank,
				CraftResource.Iron,
				typeof(IronIngot),
				typeof(IronOre),
				typeof(Granite)),
			new CraftResourceInfo(
				1045,
				1053108,
				"Dull Copper",
				CraftAttributeInfo.DullCopper,
				CraftResource.DullCopper,
				typeof(DullCopperIngot),
				typeof(DullCopperOre),
				typeof(DullCopperGranite)),
			new CraftResourceInfo(
				1109,
				1053107,
				"Shadow Iron",
				CraftAttributeInfo.ShadowIron,
				CraftResource.ShadowIron,
				typeof(ShadowIronIngot),
				typeof(ShadowIronOre),
				typeof(ShadowIronGranite)),
			new CraftResourceInfo(
				1119,
				1053106,
				"Copper",
				CraftAttributeInfo.Copper,
				CraftResource.Copper,
				typeof(CopperIngot),
				typeof(CopperOre),
				typeof(CopperGranite)),
			new CraftResourceInfo(
				1752,
				1053105,
				"Bronze",
				CraftAttributeInfo.Bronze,
				CraftResource.Bronze,
				typeof(BronzeIngot),
				typeof(BronzeOre),
				typeof(BronzeGranite)),
			new CraftResourceInfo(
				1719,
				1053104,
				"Gold",
				CraftAttributeInfo.Golden,
				CraftResource.Gold,
				typeof(GoldIngot),
				typeof(GoldOre),
				typeof(GoldGranite)),
			new CraftResourceInfo(
				2430,
				1053103,
				"Agapite",
				CraftAttributeInfo.Agapite,
				CraftResource.Agapite,
				typeof(AgapiteIngot),
				typeof(AgapiteOre),
				typeof(AgapiteGranite)),
			new CraftResourceInfo(
				2002,
				1053102,
				"Verite",
				CraftAttributeInfo.Verite,
				CraftResource.Verite,
				typeof(VeriteIngot),
				typeof(VeriteOre),
				typeof(VeriteGranite)),
			new CraftResourceInfo(
				1348,
				1053101,
				"Valorite",
				CraftAttributeInfo.Valorite,
				CraftResource.Valorite,
				typeof(ValoriteIngot),
				typeof(ValoriteOre),
				typeof(ValoriteGranite)),
			new CraftResourceInfo(1779, 0, "Blood Rock", CraftAttributeInfo.BloodRock, CraftResource.BloodRock, new Type[0]),
			new CraftResourceInfo(1175, 0, "Black Rock", CraftAttributeInfo.BlackRock, CraftResource.BlackRock, new Type[0])
		};

		public static CraftResourceInfo[] m_ScaleInfo = new[]
		{
			new CraftResourceInfo(
				1645, 1053129, "Red Scales", CraftAttributeInfo.RedScales, CraftResource.RedScales, typeof(RedScales)),
			new CraftResourceInfo(
				2216, 1053130, "Yellow Scales", CraftAttributeInfo.YellowScales, CraftResource.YellowScales, typeof(YellowScales)),
			new CraftResourceInfo(
				1109, 1053131, "Black Scales", CraftAttributeInfo.BlackScales, CraftResource.BlackScales, typeof(BlackScales)),
			new CraftResourceInfo(
				2129, 1053132, "Green Scales", CraftAttributeInfo.GreenScales, CraftResource.GreenScales, typeof(GreenScales)),
			new CraftResourceInfo(
				2301, 1053133, "White Scales", CraftAttributeInfo.WhiteScales, CraftResource.WhiteScales, typeof(WhiteScales)),
			new CraftResourceInfo(
				2224, 1053134, "Blue Scales", CraftAttributeInfo.BlueScales, CraftResource.BlueScales, typeof(BlueScales)),
			new CraftResourceInfo(
				1157, 0, "Blood Scales", CraftAttributeInfo.BloodScales, CraftResource.BloodScales, typeof(BloodScales))
		};

		public static CraftResourceInfo[] m_LeatherInfo = new[]
		{
			new CraftResourceInfo(
				0x000, 1049353, "Normal", CraftAttributeInfo.Blank, CraftResource.RegularLeather, typeof(Leather), typeof(Hides)),
			new CraftResourceInfo(
				1508,
				1049354,
				"Spined",
				CraftAttributeInfo.Spined,
				CraftResource.SpinedLeather,
				typeof(SpinedLeather),
				typeof(SpinedHides)),
			new CraftResourceInfo(
				2304,
				1049355,
				"Horned",
				CraftAttributeInfo.Horned,
				CraftResource.HornedLeather,
				typeof(HornedLeather),
				typeof(HornedHides)),
			new CraftResourceInfo(
				1437,
				1049356,
				"Barbed",
				CraftAttributeInfo.Barbed,
				CraftResource.BarbedLeather,
				typeof(BarbedLeather),
				typeof(BarbedHides))
		};

		public static CraftResourceInfo[] m_AOSLeatherInfo = new[]
		{
			new CraftResourceInfo(
				0x000, 1049353, "Normal", CraftAttributeInfo.Blank, CraftResource.RegularLeather, typeof(Leather), typeof(Hides)),
			new CraftResourceInfo(
				0x8AC,
				1049354,
				"Spined",
				CraftAttributeInfo.Spined,
				CraftResource.SpinedLeather,
				typeof(SpinedLeather),
				typeof(SpinedHides)),
			new CraftResourceInfo(
				0x845,
				1049355,
				"Horned",
				CraftAttributeInfo.Horned,
				CraftResource.HornedLeather,
				typeof(HornedLeather),
				typeof(HornedHides)),
			new CraftResourceInfo(
				0x851,
				1049356,
				"Barbed",
				CraftAttributeInfo.Barbed,
				CraftResource.BarbedLeather,
				typeof(BarbedLeather),
				typeof(BarbedHides))
		};

		public static CraftResourceInfo[] m_WoodInfo = new[]
		{
			new CraftResourceInfo(
				0x000, 1011542, "Normal", CraftAttributeInfo.Blank, CraftResource.RegularWood, typeof(Log), typeof(Board)),
			new CraftResourceInfo(
				0x7DA, 1072533, "Oak", CraftAttributeInfo.OakWood, CraftResource.OakWood, typeof(OakLog), typeof(OakBoard)),
			new CraftResourceInfo(
				0x4A7, 1072534, "Ash", CraftAttributeInfo.AshWood, CraftResource.AshWood, typeof(AshLog), typeof(AshBoard)),
			new CraftResourceInfo(
				0x4A8, 1072535, "Yew", CraftAttributeInfo.YewWood, CraftResource.YewWood, typeof(YewLog), typeof(YewBoard)),
			new CraftResourceInfo(
				0x4A9,
				1072536,
				"Heartwood",
				CraftAttributeInfo.Heartwood,
				CraftResource.Heartwood,
				typeof(HeartwoodLog),
				typeof(HeartwoodBoard)),
			new CraftResourceInfo(
				0x4AA,
				1072538,
				"Bloodwood",
				CraftAttributeInfo.Bloodwood,
				CraftResource.Bloodwood,
				typeof(BloodwoodLog),
				typeof(BloodwoodBoard)),
			new CraftResourceInfo(
				0x47F,
				1072539,
				"Frostwood",
				CraftAttributeInfo.Frostwood,
				CraftResource.Frostwood,
				typeof(FrostwoodLog),
				typeof(FrostwoodBoard))
		};

		/// <summary>
		///     Returns true if '<paramref name="resource" />' is None, Iron, RegularLeather or RegularWood. False if otherwise.
		/// </summary>
		public static bool IsStandard(CraftResource resource)
		{
			return (resource == CraftResource.None || resource == CraftResource.Iron || resource == CraftResource.RegularLeather ||
					resource == CraftResource.RegularWood);
		}

		private static Dictionary<Type, CraftResource> m_TypeTable;

		/// <summary>
		///     Registers that '<paramref name="resourceType" />' uses '<paramref name="resource" />' so that it can later be queried by
		///     <see
		///         cref="CraftResources.GetFromType" />
		/// </summary>
		public static void RegisterType(Type resourceType, CraftResource resource)
		{
			if (m_TypeTable == null)
			{
				m_TypeTable = new Dictionary<Type, CraftResource>();
			}

			m_TypeTable[resourceType] = resource;
		}

		/// <summary>
		///     Returns the <see cref="CraftResource" /> value for which '<paramref name="resourceType" />' uses -or- CraftResource.None if an unregistered type was specified.
		/// </summary>
		public static CraftResource GetFromType(Type resourceType)
		{
			if (m_TypeTable == null)
			{
				return CraftResource.None;
			}

			CraftResource resource;
			m_TypeTable.TryGetValue(resourceType, out resource);

			return resource;
		}

		/// <summary>
		///     Returns a <see cref="CraftResourceInfo" /> instance describing '<paramref name="resource" />' -or- null if an invalid resource was specified.
		/// </summary>
		public static CraftResourceInfo GetInfo(CraftResource resource)
		{
			CraftResourceInfo[] list = null;

			switch (GetType(resource))
			{
				case CraftResourceType.Metal:
					list = m_MetalInfo;
					break;
				case CraftResourceType.Leather:
					list = m_LeatherInfo;
					break;
				case CraftResourceType.Scales:
					list = m_ScaleInfo;
					break;
				case CraftResourceType.Wood:
					list = m_WoodInfo;
					break;
			}

			if (list != null)
			{
				int index = GetIndex(resource);

				if (index >= 0 && index < list.Length)
				{
					return list[index];
				}
			}

			return null;
		}

		/// <summary>
		///     Returns a <see cref="CraftResourceType" /> value indiciating the type of '<paramref name="resource" />'.
		/// </summary>
		public static CraftResourceType GetType(CraftResource resource)
		{
			if (resource >= CraftResource.Iron && resource < CraftResource.RegularLeather)
			{
				return CraftResourceType.Metal;
			}

			if (resource >= CraftResource.RegularLeather && resource <= CraftResource.BarbedLeather)
			{
				return CraftResourceType.Leather;
			}

			if (resource >= CraftResource.RedScales && resource <= CraftResource.BloodScales)
			{
				return CraftResourceType.Scales;
			}

			if (resource >= CraftResource.RegularWood && resource <= CraftResource.Frostwood)
			{
				return CraftResourceType.Wood;
			}

			return CraftResourceType.None;
		}

		/// <summary>
		///     Returns the first <see cref="CraftResource" /> in the series of resources for which '<paramref name="resource" />' belongs.
		/// </summary>
		public static CraftResource GetStart(CraftResource resource)
		{
			switch (GetType(resource))
			{
				case CraftResourceType.Metal:
					return CraftResource.Iron;
				case CraftResourceType.Leather:
					return CraftResource.RegularLeather;
				case CraftResourceType.Scales:
					return CraftResource.RedScales;
				case CraftResourceType.Wood:
					return CraftResource.RegularWood;
			}

			return CraftResource.None;
		}

		/// <summary>
		///     Returns the index of '<paramref name="resource" />' in the seriest of resources for which it belongs.
		/// </summary>
		public static int GetIndex(CraftResource resource)
		{
			CraftResource start = GetStart(resource);

			if (start == CraftResource.None)
			{
				return 0;
			}

			return (resource - start);
		}

		/// <summary>
		///     Returns the <see cref="CraftResourceInfo.Number" /> property of '<paramref name="resource" />' -or- 0 if an invalid resource was specified.
		/// </summary>
		public static int GetLocalizationNumber(CraftResource resource)
		{
			CraftResourceInfo info = GetInfo(resource);

			return (info == null ? 0 : info.Number);
		}

		/// <summary>
		///     Returns the <see cref="CraftResourceInfo.Hue" /> property of '<paramref name="resource" />' -or- 0 if an invalid resource was specified.
		/// </summary>
		public static int GetHue(CraftResource resource)
		{
			CraftResourceInfo info = GetInfo(resource);

			return (info == null ? 0 : info.Hue);
		}

		/// <summary>
		///     Returns the <see cref="CraftResourceInfo.Name" /> property of '<paramref name="resource" />' -or- an empty string if the resource specified was invalid.
		/// </summary>
		public static string GetName(CraftResource resource)
		{
			CraftResourceInfo info = GetInfo(resource);

			return (info == null ? String.Empty : info.Name);
		}

		/// <summary>
		///     Returns the <see cref="CraftResource" /> value which represents '<paramref name="info" />' -or- CraftResource.None if unable to convert.
		/// </summary>
		public static CraftResource GetFromOreInfo(OreInfo info)
		{
			if (info.Name.IndexOf("Spined", StringComparison.Ordinal) >= 0)
			{
				return CraftResource.SpinedLeather;
			}

			if (info.Name.IndexOf("Horned", StringComparison.Ordinal) >= 0)
			{
				return CraftResource.HornedLeather;
			}

			if (info.Name.IndexOf("Barbed", StringComparison.Ordinal) >= 0)
			{
				return CraftResource.BarbedLeather;
			}

			if (info.Name.IndexOf("Leather", StringComparison.Ordinal) >= 0)
			{
				return CraftResource.RegularLeather;
			}

			if (info.Level == 0)
			{
				return CraftResource.Iron;
			}

			switch (info.Level)
			{
				case 1:
					return CraftResource.DullCopper;
				case 2:
					return CraftResource.ShadowIron;
				case 3:
					return CraftResource.Copper;
				case 4:
					return CraftResource.Bronze;
				case 5:
					return CraftResource.Gold;
				case 6:
					return CraftResource.Agapite;
				case 7:
					return CraftResource.Verite;
				case 8:
					return CraftResource.Valorite;
			}

			return CraftResource.None;
		}

		/// <summary>
		///     Returns the <see cref="CraftResource" /> value which represents '<paramref name="info" />', using '
		///     <paramref
		///         name="material" />
		///     ' to help resolve leather OreInfo instances.
		/// </summary>
		public static CraftResource GetFromOreInfo(OreInfo info, ArmorMaterialType material)
		{
			if (material == ArmorMaterialType.Studded || material == ArmorMaterialType.Leather ||
				material == ArmorMaterialType.Spined || material == ArmorMaterialType.Horned || material == ArmorMaterialType.Barbed)
			{
				switch (info.Level)
				{
					case 0:
						return CraftResource.RegularLeather;
					case 1:
						return CraftResource.SpinedLeather;
					case 2:
						return CraftResource.HornedLeather;
					case 3:
						return CraftResource.BarbedLeather;
				}

				return CraftResource.None;
			}

			return GetFromOreInfo(info);
		}
	}

	// NOTE: This class is only for compatability with very old RunUO versions.
	// No changes to it should be required for custom resources.
	public class OreInfo
	{
		public static readonly OreInfo None = new OreInfo(OreType.None, -1, 0x000, "None");
		public static readonly OreInfo Iron = new OreInfo(OreType.Iron, 0, 0x000, "Iron");
		public static readonly OreInfo DullCopper = new OreInfo(OreType.DullCopper, 1, 0x973, "Dull Copper");
		public static readonly OreInfo ShadowIron = new OreInfo(OreType.ShadowIron, 2, 0x966, "Shadow Iron");
		public static readonly OreInfo Copper = new OreInfo(OreType.Copper, 3, 0x96D, "Copper");
		public static readonly OreInfo Bronze = new OreInfo(OreType.Bronze, 4, 0x972, "Bronze");
		public static readonly OreInfo Gold = new OreInfo(OreType.Gold, 5, 0x8A5, "Gold");
		public static readonly OreInfo Agapite = new OreInfo(OreType.Agapite, 6, 0x979, "Agapite");
		public static readonly OreInfo Verite = new OreInfo(OreType.Verite, 7, 0x89F, "Verite");
		public static readonly OreInfo Valorite = new OreInfo(OreType.Valorite, 8, 0x8AB, "Valorite");

		public OreType Type { get; private set; }
		public int Level { get; private set; }
		public int Hue { get; private set; }
		public string Name { get; private set; }
		
		public OreInfo(OreType type, int level, int hue, string name)
		{
			Type = type;
			Level = level;
			Hue = hue;
			Name = name;
		}
	}
}