#region References
using System;
using System.Collections.Generic;
using System.Globalization;

using Server.Engines.ConPVP;
using Server.Engines.Conquests;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Targeting;

using VitaNex.Modules.AutoPvP;
using VitaNex.Modules.AutoPvP.Battles;
#endregion

namespace Server.Commands
{
	public class BandSelfCommand
	{
		public static void Initialize()
		{
			/*
            CommandSystem.Register( "bandself", AccessLevel.Player, new CommandEventHandler( BandSelf_OnCommand ) );
			CommandSystem.Register( "band", AccessLevel.Player, new CommandEventHandler( Band_OnCommand ) );
            */
		}

		public static void BandSelf_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if (from.Alive)
			{
				if (from.AccessLevel >= AccessLevel.Counselor || DateTime.UtcNow >= from.NextActionTime)
				{
					var bandage = from.Backpack.FindItemByType(typeof(Bandage), true) as Bandage;

					if (bandage != null && !bandage.Deleted)
					{
						from.RevealingAction();

						if (BandageContext.BeginHeal(from, from) != null)
						{
							bandage.Consume();
						}

						from.NextActionTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);
					}
				}
				else
				{
					from.SendActionMessage();
				}
			}
			else
			{
				from.SendLocalizedMessage(500949); // You can't do that when you're dead.
			}
		}

		public static void Band_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if (from.AccessLevel >= AccessLevel.Counselor || DateTime.UtcNow >= from.NextActionTime)
			{
				var bandage = from.Backpack.FindItemByType(typeof(Bandage), true) as Bandage;

				if (bandage != null && !bandage.Deleted)
				{
					from.Use(bandage);
					from.NextActionTime = DateTime.UtcNow + TimeSpan.FromSeconds(0.5);
				}
			}
			else
			{
				from.SendActionMessage();
			}
		}
	}
}

namespace Server.Items
{
	public class Bandage : Item
	{
		public static int GetRange(Expansion e)
		{
			return e >= Expansion.AOS ? 2 : 1;
		}

		public override double DefaultWeight { get { return 0.1; } }

		[Constructable]
		public Bandage()
			: this(1)
		{ }

		[Constructable]
		public Bandage(int amount)
			: base(0xE21)
		{
			Stackable = true;
			Amount = amount;
			Dyable = true;
		}

		public override bool DisplayDyable { get { return false; } }

		public Bandage(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from.InRange(GetWorldLocation(), GetRange(from.Expansion)))
			{
				from.RevealingAction();

				from.SendLocalizedMessage(500948); // Who will you use the bandages on?

				from.Target = new InternalTarget(this);
			}
			else
			{
				from.SendLocalizedMessage(500295); // You are too far away to do that.
			}
		}

		private class InternalTarget : Target
		{
			private readonly Bandage m_Bandage;

			public InternalTarget(Bandage bandage)
				: base(GetRange(bandage.Expansion), false, TargetFlags.Beneficial)
			{
				m_Bandage = bandage;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Bandage.Deleted)
				{
					return;
				}

				var entity = targeted as IEntity;

				if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) &&
					UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, m_Bandage))
				{
					return;
				}

				if (targeted is BaseCreature &&
					(((BaseCreature)targeted).Pseu_CanBeHealed == false || ((BaseCreature)targeted).ChampSpawn != null))
				{
					from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500951); // You cannot heal that.
				}
				else if (targeted is Mobile)
				{
					if (from.InRange(m_Bandage.GetWorldLocation(), GetRange(from.Expansion)))
					{
						if (BandageContext.BeginHeal(from, (Mobile)targeted) != null)
						{
							var region1 = from.Region as CustomRegion;

							if (!DuelContext.IsFreeConsume(from) && (region1 == null || !region1.PlayingGame(from)))
							{
								IUOFBattle battle = null;

								if (from is PlayerMobile)
								{
									battle = AutoPvP.FindBattleI<IUOFBattle>((PlayerMobile)from);
								}

								if (battle == null || !battle.NoConsume)
								{
									m_Bandage.Consume();
								}
							}
						}
					}
					else
					{
						from.SendLocalizedMessage(500295); // You are too far away to do that.
					}
				}
				else if (targeted is PlagueBeastInnard)
				{
					if (((PlagueBeastInnard)targeted).OnBandage(from))
					{
						m_Bandage.Consume();
					}
				}
				else
				{
					from.SendLocalizedMessage(500970); // Bandages can not be used on that.
				}
			}

			protected override void OnNonlocalTarget(Mobile from, object targeted)
			{
				if (targeted is PlagueBeastInnard)
				{
					if (((PlagueBeastInnard)targeted).OnBandage(from))
					{
						m_Bandage.Consume();
					}
				}
				else
				{
					base.OnNonlocalTarget(from, targeted);
				}
			}
		}
	}

	public class BandageContext
	{
		private readonly Mobile m_Healer;
		private readonly Mobile m_Patient;
		private int m_Slips;
		private Timer m_Timer;

		public Mobile Healer { get { return m_Healer; } }
		public Mobile Patient { get { return m_Patient; } }
		public int Slips { get { return m_Slips; } set { m_Slips = value; } }
		public Timer Timer { get { return m_Timer; } }

		public void Slip()
		{
			m_Healer.SendLocalizedMessage(500961); // Your fingers slip!
			++m_Slips;
		}

		public BandageContext(Mobile healer, Mobile patient, TimeSpan delay)
		{
			m_Healer = healer;
			m_Patient = patient;

			m_Timer = new InternalTimer(this, delay);
			m_Timer.Start();
		}

		public void StopHeal()
		{
			m_Table.Remove(m_Healer);

			if (m_Timer != null)
			{
				m_Timer.Stop();
			}

			m_Timer = null;
		}

		private static readonly Dictionary<Mobile, BandageContext> m_Table = new Dictionary<Mobile, BandageContext>();

		public static BandageContext GetContext(Mobile healer)
		{
			BandageContext bc = null;
			m_Table.TryGetValue(healer, out bc);
			return bc;
		}

		public static SkillName GetPrimarySkill(Mobile m)
		{
			if (!m.Player && (m.Body.IsMonster || m.Body.IsAnimal))
			{
				return SkillName.Veterinary;
			}

			return SkillName.Healing;
		}

		public static SkillName GetSecondarySkill(Mobile m)
		{
			if (!m.Player && (m.Body.IsMonster || m.Body.IsAnimal))
			{
				return SkillName.AnimalLore;
			}

			return SkillName.Anatomy;
		}

		public void EndHeal()
		{
			StopHeal();

			int healerNumber;
			int patientNumber;

			bool playSound = true;
			bool checkRes = false;
			bool checkPois = false;

			SkillName primarySkill = GetPrimarySkill(m_Patient);
			SkillName secondarySkill = GetSecondarySkill(m_Patient);

			var petPatient = m_Patient as BaseCreature;

			if (!m_Healer.Alive)
			{
				healerNumber = 500962; // You were unable to finish your work before you died.
				patientNumber = -1;
				playSound = false;
			}
			else if (!m_Healer.InRange(m_Patient, Bandage.GetRange(m_Healer.Expansion)))
			{
				healerNumber = 500963; // You did not stay close enough to heal your target.
				patientNumber = -1;
				playSound = false;
			}
			else if (!m_Patient.Alive || (petPatient != null && petPatient.IsDeadPet))
			{
				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing - 68.0) / 50.0) - (m_Slips * 0.02);

				//TODO: Dbl check doesn't check for faction of the horse here?
				if (((checkRes = (healing >= 80.0 && anatomy >= 80.0)) && chance > Utility.RandomDouble()) ||
					(m_Patient.EraSE && petPatient is FactionWarHorse && petPatient.ControlMaster == m_Healer))
				{
					if (m_Patient.Map == null || !m_Patient.Map.CanFit(m_Patient.Location, 16, false, false))
					{
						healerNumber = 501042; // Target can not be resurrected at that location.
						patientNumber = 502391; // Thou can not be resurrected there!
					}
					else if (m_Patient.Region != null && m_Patient.Region.IsPartOf("Khaldun"))
					{
						healerNumber = 1010395; // The veil of death in this area is too strong and resists thy efforts to restore life.
						patientNumber = -1;
					}
					else
					{
						healerNumber = 500965; // You are able to resurrect your patient.
						patientNumber = -1;

						m_Patient.PlaySound(0x214);
						m_Patient.FixedEffect(0x376A, 10, 16);

						if (petPatient != null && petPatient.IsDeadPet)
						{
							Mobile master = petPatient.ControlMaster;

							if (master != null && m_Healer == master)
							{
								petPatient.ResurrectPet();

								foreach (Skill s in petPatient.Skills)
								{
								    double skillloss = s.Base * 0.01;
									s.Base -= skillloss;
								}
                                Conquests.CheckProgress<ResConquest>(m_Healer as PlayerMobile, petPatient);
							}
							else if (master != null && master.InRange(petPatient, 3))
							{
								healerNumber = 503255; // You are able to resurrect the creature.

								master.CloseGump(typeof(PetResurrectGump));
								master.SendGump(new PetResurrectGump(m_Healer, petPatient));

                                Conquests.CheckProgress<ResConquest>(m_Healer as PlayerMobile, petPatient);
							}
							else
							{
								bool found = false;

								List<Mobile> friends = petPatient.Friends;

								for (int i = 0; friends != null && i < friends.Count; ++i)
								{
									Mobile friend = friends[i];

									if (!friend.InRange(petPatient, 3))
									{
										continue;
									}

									healerNumber = 503255; // You are able to resurrect the creature.

									friend.CloseGump(typeof(PetResurrectGump));
									friend.SendGump(new PetResurrectGump(m_Healer, petPatient));

									found = true;
                                    Conquests.CheckProgress<ResConquest>(m_Healer as PlayerMobile, petPatient);
									break;
								}

								if (!found)
								{
									healerNumber = 1049670; // The pet's owner must be nearby to attempt resurrection.
								}
							}
						}
						else
						{
							m_Patient.CloseGump(typeof(ResurrectGump));
							/*PlayerMobile pres = m_Patient as PlayerMobile;

							if (pres.MurderBounty > 0)
								m_Patient.SendGump( new ResurrectGump( m_Patient, m_Healer, pres.MurderBounty ) );
							else*/
							m_Patient.SendGump(new ResurrectGump(m_Patient, m_Healer));
                            Conquests.CheckProgress<ResConquest>(m_Healer as PlayerMobile, petPatient);
						}
					}
				}
				else
				{
					if (petPatient != null && petPatient.IsDeadPet)
					{
						healerNumber = 503256; // You fail to resurrect the creature.
					}
					else
					{
						healerNumber = 500966; // You are unable to resurrect your patient.
					}

					patientNumber = -1;
				}
			}
			else if (m_Patient.Poisoned)
			{
				m_Healer.SendLocalizedMessage(500969); // You finish applying the bandages.

				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing - 30.0) / 50.0) - (m_Patient.Poison.Level * 0.1) - (m_Slips * 0.02);

				if ((checkPois = (healing >= 60.0 && anatomy >= 60.0)) && chance > Utility.RandomDouble())
				{
					if (m_Patient.CurePoison(m_Healer))
					{
						healerNumber = (m_Healer == m_Patient) ? -1 : 1010058; // You have cured the target of all poisons.
						patientNumber = 1010059; // You have been cured of all poisons.
					}
					else
					{
						healerNumber = -1;
						patientNumber = -1;
					}
				}
				else
				{
					healerNumber = 1010060; // You have failed to cure your target!
					patientNumber = -1;
				}
			}
			else if (BleedAttack.IsBleeding(m_Patient))
			{
				healerNumber = 1060088; // You bind the wound and stop the bleeding
				patientNumber = 1060167; // The bleeding wounds have healed, you are no longer bleeding!

				BleedAttack.EndBleed(m_Patient, false);
			}
			else if (MortalStrike.IsWounded(m_Patient))
			{
				healerNumber = (m_Healer == m_Patient ? 1005000 : 1010398);
				patientNumber = -1;
				playSound = false;
			}
			else if (m_Patient.Hits == m_Patient.HitsMax)
			{
				healerNumber = 500967; // You heal what little damage your patient had.
				patientNumber = -1;
			}
			else
			{
				patientNumber = -1;

				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing + 10.0) / 100.0) - (m_Slips * 0.02);

				if (chance > Utility.RandomDouble())
				{
					healerNumber = 500969; // You finish applying the bandages.

					double min = (anatomy / 5.5) + (healing / 5.5) + 4.0;
					double max = (anatomy / 5.0) + (healing / 2.5) + 10.0;

					double toHeal = min + (Utility.RandomDouble() * (max - min));

					if ((m_Patient.Body.IsMonster || m_Patient.Body.IsAnimal) && m_Patient.NetState == null)
						// Alan Mod: player-controlled mobs heal like regular players
					{
						toHeal += m_Patient.HitsMax / 100;
					}

					toHeal -= m_Slips * 4;

					if (toHeal < 1)
					{
						toHeal = 1;
						healerNumber = 500968; // You apply the bandages, but they barely help.
					}

					var healfinal = (int)toHeal;

					int healmessage = Math.Min(m_Patient.HitsMax - m_Patient.Hits, healfinal);

					m_Patient.Heal(healfinal, m_Healer, false);

					if (healmessage > 0)
					{
						m_Patient.PrivateOverheadMessage(
							MessageType.Regular, 0x42, false, healmessage.ToString(CultureInfo.InvariantCulture), m_Patient.NetState);
						if (m_Healer != m_Patient)
						{
							m_Patient.PrivateOverheadMessage(
								MessageType.Regular, 0x42, false, healmessage.ToString(CultureInfo.InvariantCulture), m_Healer.NetState);
						}
					}
				}
				else
				{
					healerNumber = 500968; // You apply the bandages, but they barely help.
					playSound = false;
				}
			}

			if (healerNumber != -1)
			{
				m_Healer.SendLocalizedMessage(healerNumber);
			}

			if (patientNumber != -1)
			{
				m_Patient.SendLocalizedMessage(patientNumber);
			}

			if (playSound)
			{
				m_Patient.PlaySound(0x57);
			}

			double minSkill = 0.0;
			double maxSkill = 90.0;

			if (checkRes)
			{
				minSkill = 65.0;
				maxSkill = 120.0;
			}
			else if (checkPois)
			{
				minSkill = 45.0;
				maxSkill = 120.0;
			}

			m_Healer.CheckSkill(secondarySkill, minSkill, maxSkill);
			m_Healer.CheckSkill(primarySkill, minSkill, maxSkill);
		}

		private class InternalTimer : Timer
		{
			private readonly BandageContext m_Context;

			public InternalTimer(BandageContext context, TimeSpan delay)
				: base(delay)
			{
				m_Context = context;
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				m_Context.EndHeal();
			}
		}

		public static BandageContext BeginHeal(Mobile healer, Mobile patient)
		{
			bool isDeadPet = (patient is BaseCreature && ((BaseCreature)patient).IsDeadPet);

			if (patient is Golem)
			{
				healer.SendLocalizedMessage(500970); // Bandages cannot be used on that.
			}
			else if (patient is BaseCreature && ((BaseCreature)patient).IsAnimatedDead)
			{
				healer.SendLocalizedMessage(500951); // You cannot heal that.
			}
			else if (!patient.Poisoned && patient.Hits == patient.HitsMax && !BleedAttack.IsBleeding(patient) && !isDeadPet)
			{
				healer.SendLocalizedMessage(500955); // That being is not damaged!
			}
			else if (!patient.Alive && (patient.Map == null || !patient.Map.CanFit(patient.Location, 16, false, false)))
			{
				healer.SendLocalizedMessage(501042); // Target cannot be resurrected at that location.
			}
			else if (healer.CanBeBeneficial(patient, true, true))
			{
				healer.DoBeneficial(patient);

				bool onSelf = (healer == patient);
				double poisonDelay = (patient.Poisoned ? ((100.0 - healer.Skills[SkillName.Poisoning].Value) / 40.0) : 0.0);
				int dex = healer.Dex;

				double seconds;
				double resDelay = (patient.Alive ? 0.0 : 5.0);

				if (onSelf)
				{
					/*if (healer.EraAOS)
					{
						seconds = 5.0 + (0.5 * ((double)(120 - dex) / 10));
					}
					else
					{
						seconds = 9.4 + (0.6 * ((double)(120 - dex) / 10));
					}*/

					seconds = 10.0 + ((100 - dex) / 18.75) + poisonDelay;
				}
				else
				{
					/*if (healer.EraAOS && GetPrimarySkill(patient) == SkillName.Veterinary)
					{
						seconds = 2.0;
					}
					else if (healer.EraAOS)
					{
						if (dex < 204)
						{
							seconds = 3.2 - (Math.Sin((double)dex / 130) * 2.5) + resDelay;
						}
						else
						{
							seconds = 0.7 + resDelay;
						}
					}
					else
					{
						if (dex >= 100)
						{
							seconds = 3.0 + resDelay;
						}
						else if (dex >= 40)
						{
							seconds = 4.0 + resDelay;
						}
						else
						{
							seconds = 5.0 + resDelay;
						}
					}*/

					seconds = 3.0 + ((100 - dex) / 37.5) + resDelay + poisonDelay;
				}

				BandageContext context = GetContext(healer);

				if (context != null)
				{
					context.StopHeal();
				}

				//seconds *= 1000;

				context = new BandageContext(healer, patient, TimeSpan.FromSeconds(seconds));

				m_Table[healer] = context;

				if (!onSelf)
				{
					patient.SendLocalizedMessage(1008078, false, healer.Name); //  : Attempting to heal you.
				}

				healer.SendLocalizedMessage(500956); // You begin applying the bandages.
				return context;
			}

			return null;
		}
	}
}