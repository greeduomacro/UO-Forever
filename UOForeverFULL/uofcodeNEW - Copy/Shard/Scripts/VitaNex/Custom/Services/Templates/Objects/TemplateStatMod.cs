#region References
using System;

using VitaNex;
#endregion

namespace Server.PvPTemplates
{
	public sealed class TemplateStatMod : UniqueStatMod
	{
		public TemplateStatMod(StatType stat, string name, int value)
			: base(stat, name, value, TimeSpan.Zero)
		{ }
	}
}