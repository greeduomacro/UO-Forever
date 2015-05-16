#region References
using System;
using System.Linq;

using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace Server.PvPTemplates
{
	public static partial class PvPTemplates
	{
		public static event Action<TemplateProfile> OnProfileCreated;
		public static event Action<TemplateProfile> OnProfileDeleted;

		public static event Action<TemplateProfile, Template> OnTemplateSelected;
		public static event Action<TemplateProfile, Template> OnTemplateCreated;
		public static event Action<TemplateProfile, Template> OnTemplateDeleted;

		public static void InvokeProfileCreated(TemplateProfile profile)
		{
			if (profile != null && !profile.Deleted && OnProfileCreated != null)
			{
				OnProfileCreated(profile);
			}
		}

		public static void InvokeProfileDeleted(TemplateProfile profile)
		{
			if (profile != null && profile.Deleted && OnProfileDeleted != null)
			{
				OnProfileDeleted(profile);
			}
		}

		public static void InvokeTemplateSelected(TemplateProfile profile, Template oldTemplate)
		{
			if (profile != null && !profile.Deleted && OnTemplateSelected != null)
			{
				OnTemplateSelected(profile, oldTemplate);
			}
		}

		public static void InvokeTemplateCreated(TemplateProfile profile, Template template)
		{
			if (profile != null && !profile.Deleted && template != null && OnTemplateCreated != null)
			{
				OnTemplateCreated(profile, template);
			}
		}

		public static void InvokeTemplateDeleted(TemplateProfile profile, Template template)
		{
			if (profile != null && !profile.Deleted && template != null && OnTemplateDeleted != null)
			{
				OnTemplateDeleted(profile, template);
			}
		}
	}
}