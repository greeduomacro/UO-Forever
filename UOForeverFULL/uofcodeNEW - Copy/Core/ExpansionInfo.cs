/***************************************************************************
 *                          ExpansionInfo.cs
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
#endregion

namespace Server
{
	public enum Expansion
	{
		None = 0,
		T2A,
		UOR,
		UOTD,
		LBR,
		AOS,
		SE,
		ML,
		SA,
		HS
	}

	[Flags]
	public enum ClientFlags
	{
		None = 0x00000000,
		Felucca = 0x00000001,
		Trammel = 0x00000002,
		Ilshenar = 0x00000004,
		Malas = 0x00000008,
		Tokuno = 0x00000010,
		TerMur = 0x00000020,
		Unk1 = 0x00000040,
		Unk2 = 0x00000080,
		UOTD = 0x00000100
	}

	[Flags]
	public enum FeatureFlags
	{
		None = 0x00000000,
		T2A = 0x00000001,
		UOR = 0x00000002,
		UOTD = 0x00000004,
		LBR = 0x00000008,
		AOS = 0x00000010,
		SixthCharacterSlot = 0x00000020,
		SE = 0x00000040,
		ML = 0x00000080,
		EigthAgeSplash = 0x00000100, //Unk1
		NinthAgeSplash = 0x00000200, //Unk2
		TenthAgeSplash = 0x00000400, //Unk3
		IncHouseBank = 0x00000800, //Unk4
		SeventhCharacterSlot = 0x00001000,
		RolePlayFaces = 0x00002000, //Unk5
		TrialAccount = 0x00004000, //Unk6
		EleventhAgeSplash = 0x00008000, //Unk7
		SA = 0x00010000,
		HS = 0x00020000,
		GothicHousing = 0x00040000,
		RusticHousing = 0x00080000,

		ExpansionNone = None,
		ExpansionT2A = T2A,
		ExpansionUOR = ExpansionT2A | UOR,
		ExpansionUOTD = ExpansionUOR | UOTD,
		ExpansionLBR = ExpansionUOTD | LBR,
		ExpansionAOS = ExpansionLBR | AOS | EleventhAgeSplash,
		ExpansionSE = ExpansionAOS | SE,
		ExpansionML = ExpansionSE | ML | NinthAgeSplash,
		ExpansionSA = ExpansionML | SA | GothicHousing | RusticHousing,
		ExpansionHS = ExpansionSA | HS
	}

	[Flags]
	public enum CharacterListFlags
	{
		None = 0x00000000,
		Unk1 = 0x00000001,
		ConfigButton = 0x00000002, //Unk2
		OneCharacterSlot = 0x00000004,
		ContextMenus = 0x00000008,
		SlotLimit = 0x00000010,
		AOS = 0x00000020,
		SixthCharacterSlot = 0x00000040,
		SE = 0x00000080,
		ML = 0x00000100,
		Unk4 = 0x00000200,
		KR = 0x00000400, //Unk5
		Unk6 = 0x00000800,
		SeventhCharacterSlot = 0x00001000,
		Unk7 = 0x00002000,
		Unk8 = 0x00004000,
		NewFeluccaAreas = 0x00008000,

		ExpansionNone = ContextMenus, //
		ExpansionT2A = ContextMenus, //
		ExpansionUOR = ContextMenus, // None
		ExpansionUOTD = ContextMenus, //
		ExpansionLBR = ContextMenus, //
		ExpansionAOS = ContextMenus | AOS,
		ExpansionSE = ExpansionAOS | SE,
		ExpansionML = ExpansionSE | ML,
		ExpansionSA = ExpansionML,
		ExpansionHS = ExpansionSA
	}

	public class ExpansionInfo
	{
		public static ExpansionInfo CoreExpansion { get { return GetInfo(Core.Expansion); } }

		public static ExpansionInfo[] Table { get; private set; }

		static ExpansionInfo()
		{
			Table = new[]
			{
				new ExpansionInfo(
					0,
					"None",
					//
					ClientFlags.None,
					FeatureFlags.ExpansionNone,
					CharacterListFlags.ExpansionNone,
					0x00000000),
				new ExpansionInfo(
					1,
					"The Second Age",
					//
					ClientFlags.Felucca,
					FeatureFlags.ExpansionT2A,
					CharacterListFlags.ExpansionT2A,
					0x00000000),
				new ExpansionInfo(
					2,
					"Renaissance",
					//
					ClientFlags.Trammel,
					FeatureFlags.ExpansionUOR,
					CharacterListFlags.ExpansionUOR,
					0x00000000),
				new ExpansionInfo(
					3,
					"Third Dawn",
					//
					ClientFlags.Ilshenar,
					FeatureFlags.ExpansionUOTD,
					CharacterListFlags.ExpansionUOTD,
					0x00000000),
				new ExpansionInfo(
					4,
					"Blackthorn's Revenge",
					//
					ClientFlags.Ilshenar,
					FeatureFlags.ExpansionLBR,
					CharacterListFlags.ExpansionLBR,
					0x00000000),
				new ExpansionInfo(
					5,
					"Age of Shadows",
					//
					ClientFlags.Malas,
					FeatureFlags.ExpansionAOS,
					CharacterListFlags.ExpansionAOS,
					0x00000000),
				new ExpansionInfo(
					6,
					"Samurai Empire",
					//
					ClientFlags.Tokuno,
					FeatureFlags.ExpansionSE,
					CharacterListFlags.ExpansionSE,
					0x000000C0), // 0x20 | 0x80
				new ExpansionInfo(
					7,
					"Mondain's Legacy",
					//
					new ClientVersion("5.0.0a"),
					FeatureFlags.ExpansionML,
					CharacterListFlags.ExpansionML,
					0x000002C0), // 0x20 | 0x80 | 0x200
				new ExpansionInfo(
					8,
					"Stygian Abyss",
					//
					ClientFlags.TerMur,
					FeatureFlags.ExpansionSA,
					CharacterListFlags.ExpansionSA,
					0xD02C0), // 0x20 | 0x80 | 0x200 | 0x10000 | 0x40000 | 0x80000
				new ExpansionInfo(
					9,
					"High Seas",
					//
					new ClientVersion("7.0.9.0"),
					FeatureFlags.ExpansionHS,
					CharacterListFlags.ExpansionHS,
					0xD02C0) // 0x20 | 0x80 | 0x200 | 0x10000 | 0x40000 | 0x80000
			};
		}

		public static FeatureFlags GetFeatures(Expansion ex)
		{
			var info = GetInfo(ex);

			if (info != null)
			{
				return info.SupportedFeatures;
			}

			switch (ex)
			{
				case Expansion.None:
					return FeatureFlags.ExpansionNone;
				case Expansion.T2A:
					return FeatureFlags.ExpansionT2A;
				case Expansion.UOR:
					return FeatureFlags.ExpansionUOR;
				case Expansion.UOTD:
					return FeatureFlags.ExpansionUOTD;
				case Expansion.LBR:
					return FeatureFlags.ExpansionLBR;
				case Expansion.AOS:
					return FeatureFlags.ExpansionAOS;
				case Expansion.SE:
					return FeatureFlags.ExpansionSE;
				case Expansion.ML:
					return FeatureFlags.ExpansionML;
				case Expansion.SA:
					return FeatureFlags.ExpansionSA;
				case Expansion.HS:
					return FeatureFlags.ExpansionHS;
			}

			return FeatureFlags.ExpansionNone;
		}

		public static ExpansionInfo GetInfo(Expansion ex)
		{
			return GetInfo((int)ex);
		}

		public static ExpansionInfo GetInfo(int ex)
		{
			int v = ex;

			if (v < 0 || v >= Table.Length)
			{
				v = 0;
			}

			return Table[v];
		}
		
		public int ID { get; private set; }
		public string Name { get; set; }

		public ClientFlags ClientFlags { get; set; }
		public FeatureFlags SupportedFeatures { get; set; }
		public CharacterListFlags CharacterListFlags { get; set; }
		public ClientVersion RequiredClient { get; set; }
		public int CustomHousingFlag { get; set; }

		public ExpansionInfo(
			int id,
			string name,
			ClientFlags clientFlags,
			FeatureFlags supportedFeatures,
			CharacterListFlags charListFlags,
			int customHousingFlag)
			: this(id, name, supportedFeatures, charListFlags, customHousingFlag)
		{
			ClientFlags = clientFlags;
		}

		public ExpansionInfo(
			int id,
			string name,
			ClientVersion requiredClient,
			FeatureFlags supportedFeatures,
			CharacterListFlags charListFlags,
			int customHousingFlag)
			: this(id, name, supportedFeatures, charListFlags, customHousingFlag)
		{
			RequiredClient = requiredClient;
		}

		private ExpansionInfo(
			int id, string name, FeatureFlags supportedFeatures, CharacterListFlags charListFlags, int customHousingFlag)
		{
			ID = id;
			Name = name;

			SupportedFeatures = supportedFeatures;
			CharacterListFlags = charListFlags;
			CustomHousingFlag = customHousingFlag;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}