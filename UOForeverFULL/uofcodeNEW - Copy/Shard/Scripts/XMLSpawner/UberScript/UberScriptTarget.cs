#region References
using Server.Targeting;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class UberScriptTarget : Target
	{
		private readonly object m_Source;
		private readonly XmlScript m_Script;

		public UberScriptTarget(XmlScript script, object source)
			: this(script, source, true)
		{ }

		public UberScriptTarget(XmlScript script, object source, bool allowGround)
			: base(18, allowGround, TargetFlags.None)
		{
			m_Script = script;
			m_Source = source;
		}

		public UberScriptTarget(XmlScript script, object source, bool allowGround, bool checkLOS)
			: base(18, allowGround, TargetFlags.None)
		{
			m_Script = script;
			m_Source = source;
			CheckLOS = checkLOS;
		}

		protected override void OnTarget(Mobile from, object targeted)
		{
			if (m_Script == null)
			{
				return;
			}

			TriggerObject trigObject = new TriggerObject {
				This = m_Source,
				TrigMob = @from,
				TrigName = TriggerName.onTarget,
				Targeted = targeted
			};

			m_Script.Execute(trigObject, true);
		}
	}
}