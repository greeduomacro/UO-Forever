using VitaNex;

namespace Server.PvPTemplates
{
	public sealed class TemplateSkillMod : UniqueSkillMod
	{
		public TemplateSkillMod(SkillName skill, string name, double value)
			: base(skill, name, false, value)
		{
			ObeyCap = false;
		}
	}
}