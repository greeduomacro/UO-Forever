using System;
using System.Linq;

using Server.Items;
using Server.Network;

using VitaNex.IO;

namespace Server
{
	public static class ShardInfo
	{
		public static readonly bool IsTestCenter = Insensitive.Contains(IOUtility.GetBaseDirectory(), "TEST");
		
		public static readonly string DisplayName = "Ultima Online Forever";
		public static readonly string ShardListingName = "Ultima Forever";
		public static readonly string Website = "http://www.uoforever.com";
		public static readonly string DevTeamEmail = "admin@uoforever.com";
		public static readonly string DevTeamName = "UOForever Developers";
		public static readonly string AccountConfirmationAddress = "admin@uoforever.com";

		public const Expansion CoreExpansion = Expansion.UOR;

		[CallPriority(Int32.MinValue)]
		public static void Configure()
		{
			Core.Expansion = CoreExpansion;

			// Override the global expansion defaults.
			foreach (var i in ExpansionInfo.Table)
			{
				// All expansions use the High Seas supported features.
				i.SupportedFeatures = FeatureFlags.ExpansionHS;

				// All facets are supported.
				i.ClientFlags = ClientFlags.TerMur;

				// Character list is limited to the Renaissance expansion.
				i.CharacterListFlags = CharacterListFlags.ExpansionUOR;

				// Custom house foundations include all available tiles.
				i.CustomHousingFlag = 0xD02C0;
			}

			Mobile.InsuranceEnabled = false;
			Mobile.GuildClickMessage = true;
			Mobile.AsciiClickMessage = true;
			Mobile.VisibleDamageType = VisibleDamageType.Related;

			ObjectPropertyList.Enabled = true;
			
			if (ObjectPropertyList.Enabled)
			{
				// single click for everything is overridden to check object property list
				PacketHandlers.SingleClickProps = true;
			}

			Item.DefaultDyeType = typeof(DyeTub);
		}
	}
}