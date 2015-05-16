#region References
using System;
using System.IO;

using Server.Commands;
using Server.Games;
using Server.Mobiles;
using Server.Spells;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public enum TriggerName : ulong
	{
		NoTrigger = 0L,
		onBeforeDeath = 0x00000001UL,
		onDeath = 0x00000002UL,
		onDelete = 0x00000004UL,
		onDragDrop = 0x00000008UL,
		onSwing = 0x00000010UL,
		onTakenHit = 0x00000020UL,
		onGivenHit = 0x00000040UL,
		onPoisonTick = 0x00000080UL,
		onBeginCast = 0x00000100UL,
		onSpeech = 0x00000200UL, // others speaking
		onSay = 0x00000400UL, // self speaking
		onMiss = 0x0000800UL,
		onDodge = 0x00001000UL,
		onUse = 0x00002000UL,
		onDragLift = 0x00004000UL,
		onDropToWorld = 0x00008000UL,
		onExpire = 0x00010000UL,
		onTick = 0x00020000UL,
		onTenMS = 0x00040000UL,
		onTwentyFiveMS = 0x00080000UL,
		onFiftyMS = 0x00100000UL,
		onTwoFiftyMS = 0x00200000UL,
		onOneSecond = 0x00400000UL,
		onFiveSeconds = 0x00800000UL,
		onOneMinute = 0x01000000UL,
		onCreate = 0x02000000UL, // after a script has been added, run this
		onMove = 0x04000000UL,
		onNearbyMove = 0x08000000UL, // for UberScriptItems ONLY
		onMoveOff = 0x10000000UL,
		onMoveOver = 0x20000000UL,
		onGumpResponse = 0x40000000UL,
		onDisconnected = 0x80000000UL,
		onLogin = 0x100000000UL,
		onDropIntoContainr = 0x200000000UL,
		onTarget = 0x400000000UL, // specifically for UberScriptTarget, accessed through SENDTARGET(mob) function
		onTargeted = 0x800000000UL,
		// will take a while to implement, but this will handle the case where the obj with the attachment is targetted by ANYTHING
		onUnequip = 0x1000000000UL,
		onEquip = 0x2000000000UL,
		onRemove = 0x4000000000UL,
		onAdded = 0x8000000000UL,
		onArmorHit = 0x10000000000UL,
		onWeaponHit = 0x20000000000UL,
		onCraft = 0x40000000000UL,
		onSingleClick = 0x80000000000UL,
		onWaypoint = 0x100000000000UL,
		onActivate = 0x200000000000UL,
		onDeactivate = 0x400000000000UL,
		onDropIntoContainer = 0x800000000000UL,
        onMoveIntoRegion = 0x1000000000000UL,
        onEnterRegion = 0x2000000000000UL,
        onExitRegion = 0x4000000000000UL
	}

	public class UberScriptTriggers
	{
		public static bool HasTrigger(string function)
		{
			TriggerName tn;

			return Enum.TryParse(function, true, out tn);
		}

		public static void Initialize()
		{
			EventSink.Speech += EventSink_Speech;
		}

		private static void EventSink_Speech(SpeechEventArgs args)
		{
			Mobile m = args.Mobile;

			if (m == null)
			{
				return;
			}

			DateTime now = DateTime.Now;

			// check for logging (pseudoseer or counselor)
			if (m.AccessLevel == AccessLevel.Counselor)
			{
				LoggingCustom.LogCounselor(now + "\t" + m.Name + ":\t" + args.Speech);
			}

			if (m.Account != null && PseudoSeerStone.Instance != null &&
				PseudoSeerStone.Instance.PseudoSeers.ContainsKey(m.Account))
			{
				LoggingCustom.LogPseudoseer(now + "\t" + m.Account + "\t" + m.Name + ":\t" + args.Speech);
			}

			if (m is PlayerMobile && ((PlayerMobile)m).Companion)
			{
				LoggingCustom.Log(
					Path.Combine(new[] {CompanionListGump.LogFileLocation, m.Name + ".txt"}),
					now + "\t" + m.Name + ":\t" + args.Speech);
				LoggingCustom.LogCompanion(now + "\t" + m.Name + ":\t" + args.Speech);
			}

			if (!args.Blocked) // might turn it to true if return override encountered
			{
				if (XmlScript.HasTrigger(m, TriggerName.onSay))
				{
					args.Blocked = Trigger(m, m, TriggerName.onSay, null, args.Speech);
				}
				args.Handled = args.Blocked;
			}
			else if (XmlScript.HasTrigger(m, TriggerName.onSay))
			{
				Trigger(m, m, TriggerName.onSay, null, args.Speech);
			}
		}

		/// <summary>
		///     Checks for any triggers existing on "thisMob", returns true if a trigger signalled to override the
		///     handler that it came from
		/// </summary>
		/// <returns>true if a trigger execution signalled to override the handler that called Trigger</returns>
		public static bool Trigger(
			IEntity thisObj,
			Mobile trigMob,
			TriggerName trigName,
			Item item = null,
			string spoken = null,
			Spell spell = null,
			int damage = 0,
			object targeted = null,
			SkillName skillName = SkillName.Spellweaving,
			double skillValue = 0.0) // spellweaving is just a placeholder
		{
			var scripts = XmlAttach.GetScripts(thisObj);
			bool result = false;

			if (scripts != null)
			{
				TriggerObject trigObject = new TriggerObject
				{
					TrigName = trigName,
					This = thisObj,
					TrigMob = trigMob,
					TrigItem = item,
					Spell = spell,
					Damage = damage,
					Speech = spoken,
					Targeted = targeted,
					SkillName = skillName,
					SkillValue = skillValue
				};

				int count = 0;

				foreach (XmlScript script in scripts)
				{
					count++;

					// make sure each script has its own TriggerObjects
					if (script.Execute(count < scripts.Count ? trigObject.Dupe() : trigObject, true))
					{
						// returns true if there is an override
						result = true;
					}
				}
			}

			return result;
		}
	}
}