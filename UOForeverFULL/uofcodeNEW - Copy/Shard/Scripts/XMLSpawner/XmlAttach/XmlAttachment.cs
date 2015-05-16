#region References
using System;

using Server.Items;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public interface IXmlAttachment
	{
		ASerial Serial { get; }

		string Name { get; set; }

		TimeSpan Expiration { get; set; }

		DateTime ExpirationEnd { get; }

		DateTime CreationTime { get; }

		bool Deleted { get; }

		bool DoDelete { get; set; }

		bool CanActivateInBackpack { get; }

		bool CanActivateEquipped { get; }

		bool CanActivateInWorld { get; }

		bool HandlesOnSpeech { get; }

		void OnSpeech(SpeechEventArgs args);

		bool HandlesOnMovement { get; }

		void OnMovement(MovementEventArgs args);

		bool HandlesOnKill { get; }

		void OnKill(Mobile killed, Mobile killer);

		void OnBeforeKill(Mobile killed, Mobile killer);

		bool HandlesOnKilled { get; }

		void OnKilled(Mobile killed, Mobile killer);

		void OnBeforeKilled(Mobile killed, Mobile killer);

		/*
		bool HandlesOnSkillUse { get; }

		void OnSkillUse( Mobile m, Skill skill, bool success);
		*/

		IEntity AttachedTo { get; set; }

		IEntity OwnedBy { get; set; }

		bool CanEquip(Mobile from);

		void OnEquip(Mobile from);

		void OnRemoved(object parent);

		void OnAttach();

		void OnReattach();

		void OnUse(Mobile from);

		void OnUser(object target);

		bool BlockDefaultOnUse(Mobile from, object target);

		bool OnDragLift(Mobile from, Item item);

		string OnIdentify(Mobile from);

		string DisplayedProperties(Mobile from);

		void AddProperties(ObjectPropertyList list);

		string AttachedBy { get; }

		void OnDelete();

		void Delete();

		void InvalidateParentProperties();

		void SetAttachedBy(string name);

		void OnTrigger(object activator, Mobile from);

		void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven);

		int OnArmorHit(Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven);

		void Serialize(GenericWriter writer);

		void Deserialize(GenericReader reader);
	}

	public abstract class XmlAttachment : IXmlAttachment
	{
		// ----------------------------------------------
		// Private fields
		// ----------------------------------------------
		private ASerial m_Serial;

		private string m_Name;

		private IEntity m_AttachedTo;

		private IEntity m_OwnedBy;

		private string m_AttachedBy;

		private bool m_Deleted;

		private AttachmentTimer m_ExpirationTimer;

		private TimeSpan m_Expiration = TimeSpan.Zero; // no expiration by default

		private DateTime m_ExpirationEnd;

		private DateTime m_CreationTime; // when the attachment was made

		// ----------------------------------------------
		// Public properties
		// ----------------------------------------------
		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime CreationTime { get { return m_CreationTime; } }

		public bool Deleted { get { return m_Deleted; } }

		public bool DoDelete
		{
			get { return false; }
			set
			{
				if (value)
				{
					Delete();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int SerialValue { get { return m_Serial.Value; } }

		public ASerial Serial { get { return m_Serial; } set { m_Serial = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan Expiration
		{
			get
			{
				// if the expiration timer is running then return the remaining time
				if (m_ExpirationTimer != null)
				{
					return m_ExpirationEnd - DateTime.UtcNow;
				}
				else
				{
					return m_Expiration;
				}
			}
			set
			{
				m_Expiration = value;
				// if it is already attached to something then set the expiration timer
				if (m_AttachedTo != null)
				{
					DoTimer(m_Expiration);
				}
			}
		}

		public DateTime ExpirationEnd { get { return m_ExpirationEnd; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool CanActivateInBackpack { get { return true; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool CanActivateEquipped { get { return true; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool CanActivateInWorld { get { return true; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool HandlesOnSpeech { get { return false; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool HandlesOnMovement { get { return false; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool HandlesOnKill { get { return false; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool HandlesOnKilled { get { return false; } }

		protected string m_ExpireAction = null;

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string ExpireAction { get { return m_ExpireAction; } set { m_ExpireAction = value; } }

		protected string m_ExpireActCondition = null;

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string ExpireActCondition { get { return m_ExpireActCondition; } set { m_ExpireActCondition = value; } }

		protected Mobile m_ExpireTrigMob = null;

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual Mobile ExpireTrigMob { get { return m_ExpireTrigMob; } set { m_ExpireTrigMob = value; } }

		/*
		[CommandProperty( AccessLevel.GameMaster )]
		public virtual bool HandlesOnSkillUse { get{return false; } }
		*/

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string Name { get { return m_Name; } set { m_Name = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual IEntity Attached { get { return m_AttachedTo; } }

		public virtual IEntity AttachedTo { get { return m_AttachedTo; } set { m_AttachedTo = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string AttachedBy { get { return m_AttachedBy; } }

		public virtual IEntity OwnedBy { get { return m_OwnedBy; } set { m_OwnedBy = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual IEntity Owner { get { return m_OwnedBy; } }

		// ----------------------------------------------
		// Private methods
		// ----------------------------------------------
		private void DoTimer(TimeSpan delay)
		{
			m_ExpirationEnd = DateTime.UtcNow + delay;

			if (m_ExpirationTimer != null)
			{
				m_ExpirationTimer.Stop();
			}

			m_ExpirationTimer = new AttachmentTimer(this, delay);
			m_ExpirationTimer.Start();
		}

		// a timer that can be implement limited lifetime attachments
		private class AttachmentTimer : Timer
		{
			private readonly XmlAttachment m_Attachment;

			public AttachmentTimer(XmlAttachment attachment, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.OneSecond;

				m_Attachment = attachment;
			}

			protected override void OnTick()
			{
				m_Attachment.OnExpiration();
				m_Attachment.Delete();
			}
		}

		public void OnExpiration()
		{
			if (XmlScript.HasTrigger(AttachedTo, TriggerName.onExpire))
			{
				UberScriptTriggers.Trigger(AttachedTo, AttachedTo as Mobile, TriggerName.onExpire, AttachedTo as Item);
			}

			// with uberscript, I don't really see the need for this anymore
			/*
            // now check for any conditions as well
            // check for any condition that must be met for this entry to be processed
            if (!BaseXmlSpawner.CheckCondition(ExpireActCondition, null, m_OwnedBy as Mobile))
                return;

            BaseXmlSpawner.ExecuteActions(m_OwnedBy as Mobile, null, ExpireAction);
             */
		}

		// ----------------------------------------------
		// Constructors
		// ----------------------------------------------
		public XmlAttachment()
		{
			m_CreationTime = DateTime.UtcNow;

			// get the next unique serial id
			m_Serial = ASerial.NewSerial();

			// register the attachment in the serial keyed hashtable
			XmlAttach.HashSerial(m_Serial, this);
		}

		// needed for deserialization
		public XmlAttachment(ASerial serial)
		{
			m_Serial = serial;
		}

		// ----------------------------------------------
		// Public methods
		// ----------------------------------------------

		public static void Initialize()
		{
			XmlAttach.CleanUp();
		}

		public virtual bool CanEquip(Mobile from)
		{
			return true;
		}

		public virtual void OnEquip(Mobile from)
		{ }

		public virtual void OnRemoved(object parent)
		{ }

		public virtual void OnAttach()
		{
			// start up the expiration timer on attachment
			if (m_Expiration > TimeSpan.Zero)
			{
				DoTimer(m_Expiration);
			}
		}

		public virtual void OnReattach()
		{ }

		public virtual void OnUse(Mobile from)
		{ }

		public virtual void OnUser(object target)
		{ }

		public virtual bool BlockDefaultOnUse(Mobile from, object target)
		{
			return false;
		}

		public virtual bool OnDragLift(Mobile from, Item item)
		{
			return true;
		}

		public void SetAttachedBy(string name)
		{
			m_AttachedBy = name;
		}

		public virtual void OnSpeech(SpeechEventArgs args)
		{ }

		public virtual void OnMovement(MovementEventArgs args)
		{ }

		public virtual void OnKill(Mobile killed, Mobile killer)
		{ }

		public virtual void OnBeforeKill(Mobile killed, Mobile killer)
		{ }

		public virtual void OnKilled(Mobile killed, Mobile killer)
		{ }

		public virtual void OnBeforeKilled(Mobile killed, Mobile killer)
		{ }

		/*
		public virtual void OnSkillUse( Mobile m, Skill skill, bool success)
		{
		}
		*/

		public virtual void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
		{ }

		public virtual int OnArmorHit(Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven)
		{
			return 0;
		}

		public virtual string OnIdentify(Mobile from)
		{
			return null;
		}

		public virtual string DisplayedProperties(Mobile from)
		{
			return OnIdentify(from);
		}

		public virtual void AddProperties(ObjectPropertyList list)
		{ }

		public void InvalidateParentProperties()
		{
			if (AttachedTo is Item)
			{
				((Item)AttachedTo).InvalidateProperties();
			}
		}

		public void SafeItemDelete(Item item)
		{
			Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(DeleteItemCallback), new object[] {item});
		}

		public void DeleteItemCallback(object state)
		{
			var args = (object[])state;

			Item item = args[0] as Item;

			if (item != null)
			{
				// delete the item
				item.Delete();
			}
		}

		public void SafeMobileDelete(Mobile mob)
		{
			Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(DeleteMobileCallback), new object[] {mob});
		}

		public void DeleteMobileCallback(object state)
		{
			var args = (object[])state;

			Mobile mob = args[0] as Mobile;

			if (mob != null)
			{
				// delete the mobile
				mob.Delete();
			}
		}

		public void Delete()
		{
			if (m_Deleted)
			{
				return;
			}

			m_Deleted = true;

			if (m_ExpirationTimer != null)
			{
				m_ExpirationTimer.Stop();
			}

			OnDelete();

			// dereference the attachment object
			AttachedTo = null;
			OwnedBy = null;
		}

		public virtual void OnDelete()
		{ }

		public virtual void OnTrigger(object activator, Mobile from)
		{ }

		[Flags]
		public enum SaveFlag
		{
			None = 0x00000000,
			ExpireActCondition = 0x00000001,
			ExpireTrigMob = 0x00000002,
			ExpireAction = 0x00000004,
			Name = 0x000000008,
			Expiration = 0x000000010,
			ExpirationEnd = 0x000000020,
			CreationTime = 0x000000040,
			AttachedBy = 0x00000080,
			// don't do AttachedTo; that's handled in the way XmlAttachments are serialized and desrialized (see XmlAttach)
			OwnedByItem = 0x00000100,
			OwnedByMobile = 0x00000200
		}

		private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
		{
			if (setIf)
			{
				flags |= toSet;
			}
		}

		private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
		{
			return ((flags & toGet) != 0);
		}

		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(4);
			// starting with version 4: SaveFlags approach
			SaveFlag flags = SaveFlag.None;

			SetSaveFlag(ref flags, SaveFlag.ExpireActCondition, m_ExpireActCondition != null);
			SetSaveFlag(ref flags, SaveFlag.ExpireAction, m_ExpireAction != null);
			SetSaveFlag(ref flags, SaveFlag.ExpireTrigMob, m_ExpireTrigMob != null);
			SetSaveFlag(ref flags, SaveFlag.Name, m_Name != null);
			SetSaveFlag(ref flags, SaveFlag.Expiration, m_Expiration != TimeSpan.Zero);
			SetSaveFlag(ref flags, SaveFlag.ExpirationEnd, m_ExpirationTimer != null);
			SetSaveFlag(ref flags, SaveFlag.CreationTime, m_CreationTime != null);
			SetSaveFlag(ref flags, SaveFlag.AttachedBy, m_AttachedBy != null);
			SetSaveFlag(ref flags, SaveFlag.OwnedByItem, m_OwnedBy is Item);
			SetSaveFlag(ref flags, SaveFlag.OwnedByMobile, m_OwnedBy is Mobile);

			writer.Write((int)flags);

			if (GetSaveFlag(flags, SaveFlag.ExpireActCondition))
			{
				writer.Write(m_ExpireActCondition);
			}
			if (GetSaveFlag(flags, SaveFlag.ExpireAction))
			{
				writer.Write(m_ExpireAction);
			}
			if (GetSaveFlag(flags, SaveFlag.ExpireTrigMob))
			{
				writer.Write(m_ExpireTrigMob);
			}
			if (GetSaveFlag(flags, SaveFlag.AttachedBy))
			{
				writer.Write(m_AttachedBy);
			}
			if (GetSaveFlag(flags, SaveFlag.OwnedByMobile))
			{
				writer.Write((Mobile)OwnedBy);
			}
			else if (GetSaveFlag(flags, SaveFlag.OwnedByItem))
			{
				writer.Write((Item)OwnedBy);
			}
			if (GetSaveFlag(flags, SaveFlag.Name))
			{
				writer.Write(Name);
			}
			// if there are any active timers, then serialize
			if (GetSaveFlag(flags, SaveFlag.Expiration))
			{
				writer.Write(m_Expiration);
			}
			if (GetSaveFlag(flags, SaveFlag.ExpirationEnd))
			{
				writer.Write(m_ExpirationEnd - DateTime.UtcNow);
			}
			if (GetSaveFlag(flags, SaveFlag.CreationTime))
			{
				writer.Write(m_CreationTime);
			}

			// Old inefficient approach:
			/*
            // version 3
            writer.Write((string)m_ExpireAction);
            writer.Write((string)m_ExpireActCondition);
            writer.Write((Mobile)m_ExpireTrigMob);

			// version 2
			writer.Write(m_AttachedBy);
			// version 1
			if (OwnedBy is Item)
			{
				writer.Write((int)0);
				writer.Write((Item)OwnedBy);
			}
			else
				if (OwnedBy is Mobile)
				{
					writer.Write((int)1);
					writer.Write((Mobile)OwnedBy);
				}
				else
					writer.Write((int)-1);

			// version 0
			writer.Write(Name);
			// if there are any active timers, then serialize
			writer.Write(m_Expiration);
			if (m_ExpirationTimer != null)
			{
				writer.Write(m_ExpirationEnd - DateTime.UtcNow);
			}
			else
			{
				writer.Write(TimeSpan.Zero);
			}
			writer.Write(m_CreationTime);
             */
		}

		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			SaveFlag flags = SaveFlag.None;
			if (version >= 4)
			{
				flags = (SaveFlag)reader.ReadInt();
			}

			switch (version)
			{
				case 4:
					if (GetSaveFlag(flags, SaveFlag.ExpireActCondition))
					{
						m_ExpireActCondition = reader.ReadString();
					}
					if (GetSaveFlag(flags, SaveFlag.ExpireAction))
					{
						m_ExpireAction = reader.ReadString();
					}
					if (GetSaveFlag(flags, SaveFlag.ExpireTrigMob))
					{
						m_ExpireTrigMob = reader.ReadMobile();
					}
					if (GetSaveFlag(flags, SaveFlag.AttachedBy))
					{
						m_AttachedBy = reader.ReadString();
					}
					if (GetSaveFlag(flags, SaveFlag.OwnedByMobile))
					{
						OwnedBy = reader.ReadMobile();
					}
					else if (GetSaveFlag(flags, SaveFlag.OwnedByItem))
					{
						OwnedBy = reader.ReadItem();
					}
					if (GetSaveFlag(flags, SaveFlag.Name))
					{
						Name = reader.ReadString();
					}
					// if there are any active timers, then serialize
					if (GetSaveFlag(flags, SaveFlag.Expiration))
					{
						m_Expiration = reader.ReadTimeSpan();
					}
					if (GetSaveFlag(flags, SaveFlag.ExpirationEnd))
					{
						TimeSpan remainingTime = reader.ReadTimeSpan();
						if (remainingTime > TimeSpan.Zero)
						{
							DoTimer(remainingTime);
						}
					}
					if (GetSaveFlag(flags, SaveFlag.CreationTime))
					{
						m_CreationTime = reader.ReadDateTime();
					}

					// DO NOT GO ON TO CASE 3 OR BELOW!
					break;
					// =============== OLD DESERIALIZATION =====================
				case 3:
					m_ExpireAction = reader.ReadString();
					m_ExpireActCondition = reader.ReadString();
					m_ExpireTrigMob = reader.ReadMobile();
					goto case 2;
				case 2:
					m_AttachedBy = reader.ReadString();
					goto case 1;
				case 1:
					int owned = reader.ReadInt();
					if (owned == 0)
					{
						OwnedBy = reader.ReadItem();
					}
					else if (owned == 1)
					{
						OwnedBy = reader.ReadMobile();
					}
					else
					{
						OwnedBy = null;
					}

					goto case 0;
				case 0:
					// version 0
					Name = reader.ReadString();
					m_Expiration = reader.ReadTimeSpan();
					TimeSpan remaining = reader.ReadTimeSpan();

					if (remaining > TimeSpan.Zero)
					{
						DoTimer(remaining);
					}

					m_CreationTime = reader.ReadDateTime();
					break;
			}
		}
	}
}