#region References
using System;

using Server.Mobiles;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class XmlUse : XmlAttachment
	{
		private bool m_BlockDefaultUse;
		private string m_Condition; // additional condition required for use
		private string m_TargetingAction; // action performed when the target cursor is brought up

		private string m_TargetCondition;
					   // condition test applied when target is selected to determine whether it is appropriate

		private string m_TargetFailureAction; // action performed if target condition is not met
		private string m_SuccessAction; // action performed on successful use or targeting

		private string m_FailureAction;
					   // action performed if the player cannot use the object for reasons other than range, refractory, or maxuses

		private string m_RefractoryAction; // action performed if the object is used before the refractory interval expires
		private string m_MaxUsesAction; // action performed if the object is used when the maxuses are exceeded

		private int m_NUses;
		private int m_MaxRange = 3; // must be within 3 tiles to use by default
		private int m_MaxTargetRange = 30; // must be within 30 tiles to target by default
		private int m_MaxUses;
		private TimeSpan m_Refractory = TimeSpan.Zero;
		public DateTime m_EndTime;
		private bool m_RequireLOS;
		private bool m_AllowCarried = true;
		private bool m_TargetingEnabled;
		private bool m_AllowGhostUse;
		private bool m_MustBeInPack;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool MustBeInPack { get { return m_MustBeInPack; } set { m_MustBeInPack = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowGhostUse { get { return m_AllowGhostUse; } set { m_AllowGhostUse = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool TargetingEnabled { get { return m_TargetingEnabled; } set { m_TargetingEnabled = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowCarried { get { return m_AllowCarried; } set { m_AllowCarried = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool RequireLOS { get { return m_RequireLOS; } set { m_RequireLOS = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxRange { get { return m_MaxRange; } set { m_MaxRange = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxTargetRange { get { return m_MaxTargetRange; } set { m_MaxTargetRange = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int NUses { get { return m_NUses; } set { m_NUses = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxUses { get { return m_MaxUses; } set { m_MaxUses = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan Refractory { get { return m_Refractory; } set { m_Refractory = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool BlockDefaultUse { get { return m_BlockDefaultUse; } set { m_BlockDefaultUse = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string Condition { get { return m_Condition; } set { m_Condition = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string TargetCondition { get { return m_TargetCondition; } set { m_TargetCondition = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string TargetingAction { get { return m_TargetingAction; } set { m_TargetingAction = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string TargetFailureAction { get { return m_TargetFailureAction; } set { m_TargetFailureAction = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string SuccessAction { get { return m_SuccessAction; } set { m_SuccessAction = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string FailureAction { get { return m_FailureAction; } set { m_FailureAction = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string RefractoryAction { get { return m_RefractoryAction; } set { m_RefractoryAction = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public string MaxUsesAction { get { return m_MaxUsesAction; } set { m_MaxUsesAction = value; } }

		public XmlUse(ASerial serial)
			: base(serial)
		{ }

		[Attachable]
		public XmlUse()
		{ }

		[Attachable]
		public XmlUse(int maxuses)
		{
			MaxUses = maxuses;
		}

		[Attachable]
		public XmlUse(int maxuses, double refractory)
		{
			MaxUses = maxuses;
			Refractory = TimeSpan.FromSeconds(refractory);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(5);
			// version 5
			writer.Write(m_MustBeInPack);
			// version 4
			writer.Write(m_AllowGhostUse);
			// version 3
			writer.Write(m_MaxTargetRange);
			// version 2
			writer.Write(m_TargetingEnabled);
			writer.Write(m_TargetingAction);
			writer.Write(m_TargetCondition);
			writer.Write(m_TargetFailureAction);
			// version 1
			writer.Write(m_AllowCarried);
			// version 0
			writer.Write(m_RequireLOS);
			writer.Write(m_MaxRange);
			writer.Write(m_Refractory);
			writer.Write(m_EndTime - DateTime.UtcNow);
			writer.Write(m_MaxUses);
			writer.Write(m_NUses);
			writer.Write(m_BlockDefaultUse);
			writer.Write(m_Condition);
			writer.Write(m_SuccessAction);
			writer.Write(m_FailureAction);
			writer.Write(m_RefractoryAction);
			writer.Write(m_MaxUsesAction);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			switch (version)
			{
				case 5:
					m_MustBeInPack = reader.ReadBool();
					goto case 4;
				case 4:
					m_AllowGhostUse = reader.ReadBool();
					goto case 3;
				case 3:
					m_MaxTargetRange = reader.ReadInt();
					goto case 2;
				case 2:
					m_TargetingEnabled = reader.ReadBool();
					m_TargetingAction = reader.ReadString();
					m_TargetCondition = reader.ReadString();
					m_TargetFailureAction = reader.ReadString();
					goto case 1;
				case 1:
					m_AllowCarried = reader.ReadBool();
					goto case 0;
				case 0:
					// version 0
					m_RequireLOS = reader.ReadBool();
					m_MaxRange = reader.ReadInt();
					Refractory = reader.ReadTimeSpan();
					TimeSpan remaining = reader.ReadTimeSpan();
					m_EndTime = DateTime.UtcNow + remaining;
					m_MaxUses = reader.ReadInt();
					m_NUses = reader.ReadInt();
					m_BlockDefaultUse = reader.ReadBool();
					m_Condition = reader.ReadString();
					m_SuccessAction = reader.ReadString();
					m_FailureAction = reader.ReadString();
					m_RefractoryAction = reader.ReadString();
					m_MaxUsesAction = reader.ReadString();
					break;
			}
		}

		public void ExecuteActions(Mobile mob, object target, string actions)
		{
			BaseXmlSpawner.ExecuteActions(mob, target, actions);
		}

		private void ExecuteAction(Mobile mob, object target, string action)
		{
			BaseXmlSpawner.ExecuteAction(mob, target, action);
		}

		// return true to allow use
		private bool CheckCondition(Mobile from, object target)
		{
			return BaseXmlSpawner.CheckCondition(Condition, from, target);
		}

		// return true to allow use
		private bool CheckTargetCondition(Mobile from, object target)
		{
			return BaseXmlSpawner.CheckCondition(TargetCondition, from, target);
		}

		// return true to allow use
		private bool CheckRange(Mobile from, object target)
		{
			if (from == null || !(target is IEntity) || MaxRange < 0)
			{
				return false;
			}

			Map map = ((IEntity)target).Map;
			Point3D loc = ((IEntity)target).Location;

			if (map != from.Map)
			{
				return false;
			}

			// check for allowed use in pack OR must use in pack (which, if true, ignores the allowcarried flag)
			if (target is Item)
			{
				Item targetitem = (Item)target;
				// is it carried by the user?
				if (MustBeInPack)
				{
					if (targetitem.RootParent == from)
					{
						return true;
					}
					else
					{
						//from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
						return false;
					}
				}
				else
				{
					if (targetitem.RootParent == from)
					{
						return AllowCarried;
					}
					else
						// block use in other containers or on other mobiles
						if (targetitem.Parent != null)
						{
							return false;
						}
				}
			}

			bool haslos = true;
			if (RequireLOS)
			{
				// check los as well
				haslos = from.InLOS(target);
			}

			return from.InRange(loc, MaxRange) && haslos;
		}

		public bool CheckMaxUses
		{
			get
			{
				// is there a use limit?
				if (MaxUses > 0 && NUses >= MaxUses)
				{
					return false;
				}

				return true;
			}
		}

		public bool CheckRefractory
		{
			get
			{
				// is there a refractory limit?
				// if it is still refractory then return
				if (Refractory > TimeSpan.Zero && DateTime.UtcNow < m_EndTime)
				{
					return false;
				}

				return true;
			}
		}

		public void OutOfRange(Mobile from)
		{
			if (from == null)
			{
				return;
			}

			from.SendLocalizedMessage(500446); // That is too far away.
		}

		public class XmlUseTarget : Target
		{
			private object m_objectused;
			private readonly XmlUse m_xa;

			public XmlUseTarget(int range, object objectused, XmlUse xa)
				: base(range, true, TargetFlags.None)
			{
				m_objectused = objectused;
				m_xa = xa;
				CheckLOS = false;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (from == null || targeted == null || m_xa == null)
				{
					return;
				}

				// success
				if (m_xa.CheckTargetCondition(from, targeted))
				{
					m_xa.ExecuteActions(from, targeted, m_xa.SuccessAction);

					m_xa.m_EndTime = DateTime.UtcNow + m_xa.Refractory;
					m_xa.NUses++;
				}
				else
				{
					m_xa.ExecuteActions(from, targeted, m_xa.TargetFailureAction);
				}
			}
		}

		private void TryToTarget(Mobile from, object target, XmlUse xa)
		{
			if (from == null)
			{
				return;
			}

			ExecuteActions(from, target, TargetingAction);

			if (xa != null)
			{
				from.Target = new XmlUseTarget(xa.MaxTargetRange, target, xa);
			}
		}

		private void TryToUse(Mobile from, object target)
		{
			if (from == null)
			{
				return;
			}

			if (!from.Alive && !AllowGhostUse)
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019048);
				return;
			}

			if (CheckRange(from, target) && CheckCondition(from, target) && CheckMaxUses && CheckRefractory)
			{
				// check for targeting
				if (TargetingEnabled)
				{
					TryToTarget(from, target, this);
				}
				else
				{
					// success
					ExecuteActions(from, target, SuccessAction);

					m_EndTime = DateTime.UtcNow + Refractory;
					NUses++;
				}
			}
			else
			{
				// failure
				if (!CheckRange(from, target))
				{
					OutOfRange(from);
				}
				else if (!CheckRefractory)
				{
					ExecuteActions(from, target, RefractoryAction);
				}
				else if (!CheckMaxUses)
				{
					ExecuteActions(from, target, MaxUsesAction);
				}

				else
				{
					ExecuteActions(from, target, FailureAction);
				}
			}
		}

		// disable the default use of the target
		public override bool BlockDefaultOnUse(Mobile from, object target)
		{
			return (BlockDefaultUse ||
					!(CheckRange(from, target) && CheckCondition(from, target) && CheckMaxUses && CheckRefractory));
		}

		// this is called when the attachment is on the user
		public override void OnUser(object target)
		{
			Mobile from = AttachedTo as Mobile;

			TryToUse(from, target);
		}

		// this is called when the attachment is on the target being used
		public override void OnUse(Mobile from)
		{
			object target = AttachedTo;

			// if a target tries to use itself, then ignore it, it will be handled by OnUser
			if (target == from)
			{
				return;
			}

			TryToUse(from, target);
		}
	}
}