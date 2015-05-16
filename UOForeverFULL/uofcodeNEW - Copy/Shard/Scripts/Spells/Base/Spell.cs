#region References
using System;
using System.Collections.Generic;

using Server.Engines.ConPVP;
using Server.Engines.Conquests;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells.Second;
using Server.Targeting;
#endregion

namespace Server.Spells
{
	public abstract class Spell : ISpell
	{
		private readonly Mobile m_Caster;
		private readonly Item m_Scroll;
		private readonly SpellInfo m_Info;
		private SpellState m_State;
		private DateTime m_StartCastTime;

		public SpellState State { get { return m_State; } set { m_State = value; } }
		public Mobile Caster { get { return m_Caster; } }
		public SpellInfo Info { get { return m_Info; } }
		public string Name { get { return m_Info.Name; } }
		public string Mantra { get { return m_Info.Mantra; } }
		public Type[] Reagents { get { return m_Info.Reagents; } }
		public Item Scroll { get { return m_Scroll; } }
		public DateTime StartCastTime { get { return m_StartCastTime; } }

		private static readonly TimeSpan NextSpellDelay = TimeSpan.FromSeconds(0.75);
		private static TimeSpan AnimateDelay = TimeSpan.FromSeconds(1.50);
		// public static TimeSpan GetCastDelay = TimeSpan.FromSeconds(0.25);

		public virtual SkillName CastSkill { get { return SkillName.Magery; } }
		public virtual SkillName DamageSkill { get { return SkillName.EvalInt; } }

		public virtual bool RevealOnCast { get { return true; } }
		public virtual bool ClearHandsOnCast { get { return true; } }
		public virtual bool ShowHandMovement { get { return true; } }

		public virtual bool DelayedDamage { get { return false; } }

		public virtual bool DelayedDamageStacking { get { return true; } }
		//In reality, it's ANY delayed Damage spell Post-AoS that can't stack, but, only
		//Expo & Magic Arrow have enough delay and a short enough cast time to bring up
		//the possibility of stacking 'em.  Note that a MA & an Explosion will stack, but
		//of course, two MA's won't.

		private static readonly Dictionary<Type, DelayedDamageContextWrapper> m_ContextTable =
			new Dictionary<Type, DelayedDamageContextWrapper>();

		private class DelayedDamageContextWrapper
		{
			private readonly Dictionary<Mobile, Timer> m_Contexts = new Dictionary<Mobile, Timer>();

			public void Add(Mobile m, Timer t)
			{
				Timer oldTimer;
				if (m_Contexts.TryGetValue(m, out oldTimer))
				{
					oldTimer.Stop();
					m_Contexts.Remove(m);
				}

				m_Contexts.Add(m, t);
			}

			public void Remove(Mobile m)
			{
				m_Contexts.Remove(m);
			}
		}

		public void StartDelayedDamageContext(Mobile m, Timer t)
		{
			if (DelayedDamageStacking)
			{
				return; //Sanity
			}

			DelayedDamageContextWrapper contexts;

			if (!m_ContextTable.TryGetValue(GetType(), out contexts))
			{
				contexts = new DelayedDamageContextWrapper();
				m_ContextTable.Add(GetType(), contexts);
			}

			contexts.Add(m, t);
		}

		public void RemoveDelayedDamageContext(Mobile m)
		{
			DelayedDamageContextWrapper contexts;

			if (!m_ContextTable.TryGetValue(GetType(), out contexts))
			{
				return;
			}

			contexts.Remove(m);
		}

		public void HarmfulSpell(Mobile m)
		{
			if (m is BaseCreature)
			{
				((BaseCreature)m).OnHarmfulSpell(m_Caster);
			}
		}

		public Spell(Mobile caster, Item scroll, SpellInfo info)
		{
			m_Caster = caster;
			m_Scroll = scroll;
			m_Info = info;
		}

		public virtual bool IsCasting { get { return m_State == SpellState.Casting; } }

		public virtual void OnCasterHurt()
		{
			//Confirm: Monsters and pets cannot be disturbed.
			if (!Caster.Player && Caster is BaseCreature &&
				!(Caster.NetState != null && ((BaseCreature)Caster).Pseu_AllowInterrupts))
			{
				return;
			}

			if (IsCasting)
			{
				double d;
				ProtectionSpell.Registry.TryGetValue(m_Caster, out d);

				if (d <= 0.0 || d <= Utility.RandomDouble() * 100.0)
				{
					Disturb(DisturbType.Hurt, false, true);
				}
			}
		}

		public virtual void OnCasterKilled()
		{
			Disturb(DisturbType.Kill);
		}

		public virtual void OnConnectionChanged()
		{
			FinishSequence();
		}

		public virtual bool OnCasterMoving(Direction d)
		{
			if (IsCasting && BlocksMovement && m_Caster.AccessLevel < AccessLevel.GameMaster)
			{
				m_Caster.SendLocalizedMessage(500111); // You are frozen and can not move.
				return false;
			}

			return true;
		}

		public virtual bool OnCasterEquiping(Item item)
		{
			if (IsCasting)
			{
				Disturb(DisturbType.EquipRequest);
			}

			return true;
		}

		public virtual bool OnCasterUsingObject(object o)
		{
			if (m_State == SpellState.Sequencing)
			{
				Disturb(DisturbType.UseRequest);
			}

			return true;
		}

		public virtual bool OnCastInTown(Region r)
		{
			return m_Info.AllowTown;
		}

		public virtual bool ConsumeReagents()
		{
			var bc = m_Caster as BaseCreature;

			if (bc != null)
			{
				if (!bc.Deleted && bc.NetState != null)
				{
					if (!bc.Pseu_ConsumeReagents)
					{
						return true;
					}
				}
				else
				{
					if (m_Scroll != null)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
			}
			else if (m_Scroll != null || !m_Caster.Player || m_Caster.AccessLevel > AccessLevel.Counselor)
			{
				return true;
			}

			if (AosAttributes.GetValue(m_Caster, AosAttribute.LowerRegCost) > Utility.Random(100))
			{
				return true;
			}

			var region1 = m_Caster.Region as CustomRegion;

			if (region1 != null && region1.NoRegs() || region1 != null && region1.PlayingGame(m_Caster))
			{
				return true;
			}

			if (DuelContext.IsFreeConsume(m_Caster))
			{
				return true;
			}

			Container pack = m_Caster.Backpack;

			if (pack == null)
			{
				return false;
			}

			if (pack.ConsumeTotal(m_Info.Reagents, m_Info.Amounts) == -1)
			{
				return true;
			}

			return false;
		}

		public virtual double GetInscribeSkill(Mobile m)
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );

			return m.Skills[SkillName.Inscribe].Value;
		}

		public virtual int GetInscribeFixed(Mobile m)
		{
			// There is no chance to gain
			// m.CheckSkill( SkillName.Inscribe, 0.0, 120.0 );

			return m.Skills[SkillName.Inscribe].Fixed;
		}

		public virtual int GetDamageFixed(Mobile m)
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );

			return m.Skills[DamageSkill].Fixed;
		}

		public virtual double GetDamageSkill(Mobile m)
		{
			//m.CheckSkill( DamageSkill, 0.0, m.Skills[DamageSkill].Cap );

			return m.Skills[DamageSkill].Value;
		}

		public virtual double GetResistSkill(Mobile m)
		{
			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual double GetDamageScalar(Mobile target)
		{
			double scalar = 1.0;

			double casterEI = m_Caster.Skills[DamageSkill].Value;
			double targetRS = target.Skills[SkillName.MagicResist].Value;

		    if (target.Player && m_Caster is BaseCreature && m_Caster.IsControlled() && casterEI > 100.0)
		    {
		        casterEI = 100.0;
		    }

			//m_Caster.CheckSkill( DamageSkill, 0.0, 120.0 );

			if ((target.Player && m_Caster.Player && Math.Min(casterEI, 100.0) > Math.Min(targetRS, 100.0)) ||
				((!target.Player || !m_Caster.Player) && casterEI > targetRS))
			{
				scalar = (1.0 + ((casterEI - targetRS) / 500.0));
			}
			else
			{
				scalar = (1.0 + ((casterEI - targetRS) / 200.0));
			}

			// magery damage bonus, -25% at 0 skill, +0% at 100 skill, +5% at 120 skill

            if (!target.Player || target.Player && m_Caster.Player || target.Player && m_Caster is BaseCreature && !m_Caster.IsControlled() || target.Player && m_Caster is BaseCreature && m_Caster.IsControlled() && m_Caster.Skills[CastSkill].Value < 100.0)
            {
                scalar += (m_Caster.Skills[CastSkill].Value - 100.0) / 400.0;
            }

			if (target is BaseCreature)
			{
				if (((BaseCreature)target).TakesNormalDamage == false)
				{
					scalar *= 2.0; // Double magery damage to monsters/animals
				}
				((BaseCreature)target).AlterDamageScalarFrom(m_Caster, ref scalar);
			}

			if (m_Caster is BaseCreature)
			{
				((BaseCreature)m_Caster).AlterDamageScalarTo(target, ref scalar);
			}

			scalar *= GetSlayerDamageScalar(target);

			target.Region.SpellDamageScalar(m_Caster, target, ref scalar);

			return scalar;
		}

		public virtual double GetSlayerDamageScalar(Mobile defender)
		{
			// ==== Alan Mod =====
			if (defender is PlayerMobile || defender is HumanMob) // there are no slayers that should slay them
			{
				return 1.0;
			}
			// first check for XmlSlayer attachments
			List<XmlSlayer> slayerattachments = XmlAttach.GetSlayerAttachments(m_Caster);
			if (slayerattachments != null)
			{
				foreach (XmlSlayer slayerattachment in slayerattachments)
				{
					if (slayerattachment.Slayer == SlayerName.None)
					{
						continue;
					}
					if (slayerattachment.Slayer == SlayerName.All)
					{
						return SpellDamageController._SlayerMultiplier;
					}

					SlayerEntry attachmentSlayer = SlayerGroup.GetEntryByName(slayerattachment.Slayer);

					if (attachmentSlayer != null && attachmentSlayer.Slays(defender))
					{
						return SpellDamageController._SlayerMultiplier;
					}
				}
			}
			// ==== End Alan Mod ====

			Spellbook atkBook = Spellbook.FindEquippedSpellbook(m_Caster);

			if (atkBook != null)
			{
				if (atkBook.Slayer == SlayerName.All)
				{
					return SpellDamageController._SlayerMultiplier;
				}
				if (atkBook.Slayer2 == SlayerName.All)
				{
					return SpellDamageController._SlayerMultiplier;
				}

				SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(atkBook.Slayer);
				SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName(atkBook.Slayer2);

				if (atkSlayer != null && atkSlayer.Slays(defender) || atkSlayer2 != null && atkSlayer2.Slays(defender))
				{
					defender.FixedEffect(0x37B9, 10, 5); //TODO: Confirm this displays on OSIs
					return SpellDamageController._SlayerMultiplier;
				}
			}

			/*
			ISlayer defISlayer = Spellbook.FindEquippedSpellbook( defender );

			if( defISlayer == null )
				defISlayer = defender.Weapon as ISlayer;

			if( defISlayer != null )
			{
				SlayerEntry defSlayer = SlayerGroup.GetEntryByName( defISlayer.Slayer );
				SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName( defISlayer.Slayer2 );

				if( defSlayer != null && defSlayer.Group.OppositionSuperSlays( m_Caster ) || defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays( m_Caster ) )
					scalar = 1.1;
			}
             */

			return 1.0;
		}

		public virtual void DoFizzle()
		{
			m_Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502632); // The spell fizzles.

			if (m_Caster.Player)
			{
				if (m_Caster.EraAOS)
				{
					m_Caster.FixedParticles(0x3735, 1, 30, 9503, EffectLayer.Waist);
				}
				else
				{
					m_Caster.FixedEffect(0x3735, 6, 30);
				}

				m_Caster.PlaySound(0x5C);
			}
		}

		private CastTimer m_CastTimer;
		private AnimTimer m_AnimTimer;

		public void Disturb(DisturbType type)
		{
			Disturb(type, true, false);
		}

		public virtual bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
		{
			if (resistable && (m_Scroll is BaseWand || m_Scroll is GnarledStaff))
			{
				return false;
			}

			return true;
		}

		public void Disturb(DisturbType type, bool firstCircle, bool resistable)
		{
			if (!CheckDisturb(type, firstCircle, resistable))
			{
				return;
			}

			if (m_State == SpellState.Casting)
			{
				if (!firstCircle && !m_Caster.EraAOS && this is MagerySpell && ((MagerySpell)this).Circle == SpellCircle.First)
				{
					return;
				}

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb(type, true);

				if (m_CastTimer != null)
				{
					m_CastTimer.Stop();
				}

				if (m_AnimTimer != null)
				{
					m_AnimTimer.Stop();
				}

				if ( /*m_Caster.AOS && m_Caster.Player &&*/ type == DisturbType.Hurt)
				{
					DoHurtFizzle();
				}

				m_Caster.NextSpellTime = DateTime.UtcNow + GetDisturbRecovery();
			}
			else if (m_State == SpellState.Sequencing)
			{
				if (!firstCircle && !m_Caster.EraAOS && this is MagerySpell && ((MagerySpell)this).Circle == SpellCircle.First)
				{
					return;
				}

				m_State = SpellState.None;
				m_Caster.Spell = null;

				OnDisturb(type, false);

				Target.Cancel(m_Caster);

				if ( /*Core.AOS && m_Caster.Player &&*/ type == DisturbType.Hurt)
				{
					DoHurtFizzle();
				}
			}
		}

		public virtual void DoHurtFizzle()
		{
			m_Caster.FixedEffect(0x3735, 6, 30);
			m_Caster.PlaySound(0x5C);
		}

		public virtual void OnDisturb(DisturbType type, bool message)
		{
			if (message)
			{
				m_Caster.SendLocalizedMessage(500641); // Your concentration is disturbed, thus ruining thy spell.
			}
		}

		public virtual bool CheckCast()
		{
			return true;
		}

		public virtual void SayMantra()
		{
			if (m_Scroll is BaseWand || m_Scroll is GnarledStaff)
			{
				return;
			}

			if (m_Info.Mantra != null && m_Info.Mantra.Length > 0 &&
				(m_Caster.Player || m_Caster.Body.IsHuman || (m_Caster is BaseCreature && ((BaseCreature)m_Caster).PowerWords)))
			{
				m_Caster.PublicOverheadMessage(MessageType.Spell, m_Caster.SpeechHue, true, m_Info.Mantra, false);
			}
		}

		public virtual bool BlockedByHorrificBeast { get { return true; } }
		public virtual bool BlockedByAnimalForm { get { return true; } }
		public virtual bool BlocksMovement { get { return true; } }

		public virtual bool CheckNextSpellTime { get { return !(m_Scroll is BaseWand || m_Scroll is GnarledStaff); } }

		public bool Cast()
		{
			m_StartCastTime = DateTime.UtcNow;

			if (!m_Caster.CheckAlive())
			{
				return false;
			}
			else if (m_Caster.Spell != null && m_Caster.Spell.IsCasting)
			{
				m_Caster.SendLocalizedMessage(502642); // You are already casting a spell.
			}
			else if (!(m_Scroll is BaseWand || m_Scroll is GnarledStaff) && (m_Caster.Paralyzed || m_Caster.Frozen))
			{
				m_Caster.SendLocalizedMessage(502643); // You can not cast a spell while frozen.
			}
			else if (CheckNextSpellTime && DateTime.UtcNow < m_Caster.NextSpellTime)
			{
				m_Caster.SendLocalizedMessage(502644); // You have not yet recovered from casting a spell.
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).PeacedUntil > DateTime.UtcNow)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
			}
				#region Dueling
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).DuelContext != null &&
					 !((PlayerMobile)m_Caster).DuelContext.AllowSpellCast(m_Caster, this))
			{ }
				#endregion

			else if (m_Caster.Mana >= ScaleMana(GetMana()))
			{
				if (m_Caster.Spell == null && m_Caster.CheckSpellCast(this) && CheckCast())
				{
					if (XmlScript.HasTrigger(m_Caster, TriggerName.onBeginCast) &&
						UberScriptTriggers.Trigger(m_Caster, m_Caster, TriggerName.onBeginCast, null, null, this))
					{
						return false;
					}
					else if (m_Caster.Region.OnBeginSpellCast(m_Caster, this))
					{
						m_State = SpellState.Casting;
						m_Caster.Spell = this;

						if (!(m_Scroll is BaseWand || m_Scroll is GnarledStaff) && RevealOnCast)
						{
							m_Caster.RevealingAction();
						}

						SayMantra();

						TimeSpan castDelay = GetCastDelay();
						var count = (int)Math.Ceiling(castDelay.TotalSeconds / AnimateDelay.TotalSeconds);
						if (ShowHandMovement && (m_Caster.Body.IsHuman || (m_Caster.Player && m_Caster.Body.IsMonster)))
						{
							if (count != 0)
							{
								m_AnimTimer = new AnimTimer(this, count);
								m_AnimTimer.Start();
							}

							if (m_Info.LeftHandEffect > 0)
							{
								Caster.FixedParticles(0, 10, 5, m_Info.LeftHandEffect, EffectLayer.LeftHand);
							}

							if (m_Info.RightHandEffect > 0)
							{
								Caster.FixedParticles(0, 10, 5, m_Info.RightHandEffect, EffectLayer.RightHand);
							}
						}
							//BaseCreature Animations For Casting
						else if (m_Caster is BaseCreature)
						{
							if (count != 0 && Utility.Random(3) == 0 || (!m_Caster.Deleted && m_Caster.NetState != null))
								// 1 in 3 chance that they do the animation
							{
								m_AnimTimer = new AnimTimer(this, count);
								m_AnimTimer.Start();
							}
						}

						if (ClearHandsOnCast)
						{
							m_Caster.ClearHands();
						}

						if (m_Caster.EraML)
						{
							WeaponAbility.ClearCurrentAbility(m_Caster);
						}

						m_CastTimer = new CastTimer(this, castDelay);
						m_CastTimer.Start();

						OnBeginCast();

						return true;
					}
				}
				return false;
			}
			else
			{
				m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana
			}

			return false;
		}

		public abstract void OnCast();

		public virtual void OnBeginCast()
		{ }

		public virtual void GetCastSkills(out double min, out double max)
		{
			min = max = 0; //Intended but not required for overriding.
		}

		public virtual bool CheckFizzle()
		{
			if (m_Scroll is BaseWand || m_Scroll is GnarledStaff || m_Scroll is Runebook)
			{
				return true;
			}

			double minSkill, maxSkill;

			GetCastSkills(out minSkill, out maxSkill);

			if (DamageSkill != CastSkill)
			{
				Caster.CheckSkill(DamageSkill, 0.0, Caster.Skills[DamageSkill].Cap);
			}

			return Caster.CheckSkill(CastSkill, minSkill, maxSkill);
		}

		public abstract int GetMana();

		public virtual int ScaleMana(int mana)
		{
			double scalar = 1.0;

			// Lower Mana Cost = 40%
			int lmc = AosAttributes.GetValue(m_Caster, AosAttribute.LowerManaCost);
			if (lmc > 40)
			{
				lmc = 40;
			}

			scalar -= (double)lmc / 100;

			return (int)(mana * scalar);
		}

		public virtual TimeSpan GetDisturbRecovery()
		{
			if (m_Caster != null && m_Caster.EraAOS)
			{
				return TimeSpan.Zero;
			}

			double delay = 1.0 - Math.Sqrt((DateTime.UtcNow - m_StartCastTime).TotalSeconds / GetCastDelay().TotalSeconds);

			if (delay < 0.2)
			{
				delay = 0.2;
			}

			return TimeSpan.FromSeconds(delay);
		}

		public virtual int CastRecoveryBase { get { return 1; } } //6
		public virtual int CastRecoveryFastScalar { get { return 1; } }
		public virtual int CastRecoveryPerSecond { get { return 3; } } //4
		public virtual int CastRecoveryMinimum { get { return 0; } }

		public virtual TimeSpan GetCastRecovery()
		{
			return NextSpellDelay;
		}

		public abstract TimeSpan CastDelayBase { get; }

		public virtual double CastDelayFastScalar { get { return 1; } }
		public virtual double CastDelaySecondsPerTick { get { return 0.25; } }
		public virtual TimeSpan CastDelayMinimum { get { return TimeSpan.FromSeconds(0.25); } }

		//public virtual int CastDelayBase{ get{ return 3; } }
		//public virtual int CastDelayFastScalar{ get{ return 1; } }
		//public virtual int CastDelayPerSecond{ get{ return 4; } }
		//public virtual int CastDelayMinimum{ get{ return 1; } }

		public virtual TimeSpan GetCastDelay()
		{
			if (m_Scroll is BaseWand || m_Scroll is GnarledStaff)
			{
				return TimeSpan.Zero;
			}

			// Faster casting cap of 2 (if not using the protection spell)
			// Faster casting cap of 0 (if using the protection spell)
			// Paladin spells are subject to a faster casting cap of 4
			// Paladins with magery of 70.0 or above are subject to a faster casting cap of 2
			int fcMax = 2; // 4

			if (CastSkill == SkillName.Magery || CastSkill == SkillName.Necromancy ||
				(CastSkill == SkillName.Chivalry && m_Caster.Skills[SkillName.Magery].Value >= 70.0))
			{
				fcMax = 2;
			}

			int fc = AosAttributes.GetValue(m_Caster, AosAttribute.CastSpeed);

			if (fc > fcMax)
			{
				fc = fcMax;
			}

			if (ProtectionSpell.Registry.ContainsKey(m_Caster))
			{
				fc -= 2;
			}

			TimeSpan fcDelay = TimeSpan.FromSeconds(-(CastDelayFastScalar * fc * CastDelaySecondsPerTick));

			TimeSpan delay = CastDelayBase + fcDelay;

			if (delay < CastDelayMinimum)
			{
				delay = CastDelayMinimum;
			}

			return delay;
		}

		public virtual void FinishSequence()
		{
			m_State = SpellState.None;

			if (m_Caster.Spell == this)
			{
				m_Caster.Spell = null;
			}
		}

		public virtual int ComputeKarmaAward()
		{
			return 0;
		}

		public virtual bool CheckSequence()
		{
			int mana = ScaleMana(GetMana());
			if (m_Caster.Deleted || !m_Caster.Alive || m_Caster.Spell != this || m_State != SpellState.Sequencing)
			{
				DoFizzle();
			}
			else if (m_Scroll != null && !(m_Scroll is Runebook) &&
					 (m_Scroll.Amount <= 0 || m_Scroll.Deleted || m_Scroll.RootParent != m_Caster ||
					  (m_Scroll is BaseWand && (((BaseWand)m_Scroll).Charges <= 0 || m_Scroll.Parent != m_Caster)) ||
					  (m_Scroll is GnarledStaff && (((GnarledStaff)m_Scroll).Charges <= 0 || m_Scroll.Parent != m_Caster))))
			{
				DoFizzle();
			}
			else if (!ConsumeReagents())
			{
				m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502630); // More reagents are needed for this spell.
			}
			else if (m_Caster.Mana < mana)
			{
				m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502625); // Insufficient mana for this spell.
			}
			else if (m_Caster.Region is CustomRegion && !(m_Caster.Region).OnBeginSpellCast(m_Caster, this))
			{
				m_Caster.SendMessage("You cannot cast that spell here.");
			}
			else if (m_Caster.EraAOS && (m_Caster.Frozen || m_Caster.Paralyzed))
			{
				m_Caster.SendLocalizedMessage(502646); // You cannot cast a spell while frozen.
				DoFizzle();
			}
			else if (m_Caster is PlayerMobile && ((PlayerMobile)m_Caster).PeacedUntil > DateTime.UtcNow)
			{
				m_Caster.SendLocalizedMessage(1072060); // You cannot cast a spell while calmed.
				DoFizzle();
			}
			else if (CheckFizzle())
			{
				m_Caster.Mana -= mana;

				if (m_Scroll is SpellScroll)
				{
					m_Scroll.Consume();
				}
				else if (m_Scroll is BaseWand)
				{
					((BaseWand)m_Scroll).ConsumeCharge(m_Caster);
				}
				else if (m_Scroll is GnarledStaff)
				{
					((GnarledStaff)m_Scroll).ConsumeCharge(m_Caster);
				}

				if (m_Scroll is BaseWand || m_Scroll is GnarledStaff)
				{
					m_Caster.RevealingAction();

					bool m = m_Scroll.Movable;

					m_Scroll.Movable = false;

					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}

					m_Scroll.Movable = m;
				}
				else
				{
					if (ClearHandsOnCast)
					{
						m_Caster.ClearHands();
					}
				}

				int karma = ComputeKarmaAward();

				if (karma != 0)
				{
					Titles.AwardKarma(Caster, karma, true);
				}

                if (m_Caster is PlayerMobile)
                {
                    Conquests.CheckProgress<SuccessfulSpellConquest>(m_Caster as PlayerMobile, m_Caster.Spell);
                }

				return true;
			}
			else
			{
				DoFizzle();
			}

			return false;
		}

		public bool CheckBSequence(Mobile target)
		{
			return CheckBSequence(target, false);
		}

		public bool CheckBSequence(Mobile target, bool allowDead)
		{
			if (!target.Alive && !allowDead)
			{
				m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
				return false;
			}
			else if (Caster.CanBeBeneficial(target, true, allowDead) && CheckSequence())
			{
				Caster.DoBeneficial(target); //None are timed, so we do it here
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool CheckHSequence(Mobile target)
		{
			if (!target.Alive)
			{
				m_Caster.SendLocalizedMessage(501857); // This spell won't work on that!
				return false;
			}

			return Caster.CanBeHarmful(target) && CheckSequence();
			/*
			else if ( Caster.CanBeHarmful( target ) && CheckSequence() )
			{
				Caster.DoHarmful( target );
				return true;
			}
			else
			{
				return false;
			}
*/
		}

		private class AnimTimer : Timer
		{
			private readonly Spell m_Spell;

			public AnimTimer(Spell spell, int count)
				: base(TimeSpan.Zero, AnimateDelay, count)
			{
				m_Spell = spell;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if (m_Spell.State != SpellState.Casting || m_Spell.Caster.Spell != m_Spell)
				{
					Stop();
					return;
				}

				if (!m_Spell.Caster.Mounted && m_Spell.m_Info.Action >= 0)
				{
					if (m_Spell.Caster.Body.IsHuman)
					{
						m_Spell.Caster.Animate(m_Spell.m_Info.Action, 7, 1, true, false, 0);
					}
					else if (m_Spell.Caster.Body.IsMonster) // && m_Spell.Caster.Player)
					{
						m_Spell.Caster.Animate(12, 7, 1, true, false, 0);
					}
				}

				if (!Running)
				{
					m_Spell.m_AnimTimer = null;
				}
			}
		}

		private class CastTimer : Timer
		{
			private readonly Spell m_Spell;

			public CastTimer(Spell spell, TimeSpan castDelay)
				: base(castDelay)
			{
				m_Spell = spell;

				Priority = TimerPriority.TwentyFiveMS;
			}

			protected override void OnTick()
			{
				if (m_Spell.State == SpellState.Casting && m_Spell.Caster.Spell == m_Spell)
				{
					m_Spell.State = SpellState.Sequencing;
					m_Spell.m_CastTimer = null;
					m_Spell.Caster.OnSpellCast(m_Spell);
					m_Spell.Caster.Region.OnSpellCast(m_Spell.Caster, m_Spell);
					m_Spell.Caster.NextSpellTime = DateTime.UtcNow + m_Spell.GetCastRecovery(); // Spell.NextSpellDelay;
					if (!m_Spell.m_Caster.Deleted && m_Spell.m_Caster.NetState != null && m_Spell.m_Caster is BaseCreature)
						// pseudoseer controlled
					{
						m_Spell.m_Caster.NextSpellTime += ((BaseCreature)m_Spell.m_Caster).Pseu_SpellDelay;
					}

					Target originalTarget = m_Spell.Caster.Target;

					m_Spell.OnCast();

					if (m_Spell.Caster.Player && m_Spell.Caster.Target != null && m_Spell.Caster.Target != originalTarget)
					{
						m_Spell.Caster.Target.BeginTimeout(m_Spell.Caster, TimeSpan.FromSeconds(30.0));
					}

					m_Spell.m_CastTimer = null;
				}
			}
		}
	}
}