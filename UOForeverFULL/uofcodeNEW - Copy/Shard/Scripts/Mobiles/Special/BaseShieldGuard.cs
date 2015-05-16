#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
using Server.Engines.CentralGump;
using Server.Engines.Conquests;
using Server.Engines.MurderSystem;
using Server.Engines.XmlSpawner2;
using Server.Ethics.Evil;
using Server.Ethics.Hero;
using Server.Guilds;
using Server.Items;
using Server.Network;
using Server.Ethics;

using VitaNex;
#endregion

namespace Server.Mobiles
{
    public class HeadTurnInObj
    {
        public DateTime TimeTurnedIn;
        public PlayerMobile TurnedIn;
        public PlayerMobile HeadOwner;

        public HeadTurnInObj(DateTime time, PlayerMobile turnedin, PlayerMobile owner)
        {
            TimeTurnedIn = time;
            TurnedIn = turnedin;
            HeadOwner = owner;
        }
    }
	public abstract class BaseShieldGuard : BaseCreature
	{
		private readonly TimeSpan m_SpeechDelay = TimeSpan.FromSeconds(300.0); // time between speech
		public DateTime m_NextSpeechTime;
        public Timer PendingStatlossTimer { get; set; }

        public static List<HeadTurnInObj> PendingStatloss { get; set; } 

		public BaseShieldGuard()
			: base(AIType.AI_Melee, FightMode.Aggressor, 14, 1, 0.8, 1.6)
		{
			InitStats(1000, 1000, 1000);
			Title = "the guard";

			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

            PendingStatloss = new List<HeadTurnInObj>();

            PendingStatlossTimer = new InternalTimer();
			if (Female == Utility.RandomBool())
			{
				Body = 0x191;
				Name = NameList.RandomName("female");

				AddItem(new FemalePlateChest());
				AddItem(new PlateArms());
				AddItem(new PlateLegs());

				switch (Utility.Random(2))
				{
					case 0:
						AddItem(new Doublet(Utility.RandomNondyedHue()));
						break;
					case 1:
						AddItem(new BodySash(Utility.RandomNondyedHue()));
						break;
				}

				switch (Utility.Random(2))
				{
					case 0:
						AddItem(new Skirt(Utility.RandomNondyedHue()));
						break;
					case 1:
						AddItem(new Kilt(Utility.RandomNondyedHue()));
						break;
				}
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");

				AddItem(new PlateChest());
				AddItem(new PlateArms());
				AddItem(new PlateLegs());

				switch (Utility.Random(3))
				{
					case 0:
						AddItem(new Doublet(Utility.RandomNondyedHue()));
						break;
					case 1:
						AddItem(new Tunic(Utility.RandomNondyedHue()));
						break;
					case 2:
						AddItem(new BodySash(Utility.RandomNondyedHue()));
						break;
				}
			}

			Utility.AssignRandomHair(this);

			if (Utility.RandomBool())
			{
				Utility.AssignRandomFacialHair(this, HairHue);
			}

			var weapon = new VikingSword
			{
				Movable = false
			};

			AddItem(weapon);

			BaseShield shield = Shield;
			shield.Movable = false;
			AddItem(shield);

			PackGold(250, 300);

			Skills[SkillName.Anatomy].Base = 120.0;
			Skills[SkillName.Tactics].Base = 120.0;
			Skills[SkillName.Swords].Base = 120.0;
			Skills[SkillName.MagicResist].Base = 120.0;
			Skills[SkillName.DetectHidden].Base = 100.0;
		}

		public abstract int Keyword { get; }
		public abstract BaseShield Shield { get; }
		public abstract int SignupNumber { get; }
		public abstract GuildType Type { get; }

		public override bool HandlesOnSpeech(Mobile from)
		{
			if (from.InRange(Location, 2))
			{
				return true;
			}

			return base.HandlesOnSpeech(from);
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (XmlScript.HasTrigger(this, TriggerName.onSpeech) &&
				UberScriptTriggers.Trigger(this, e.Mobile, TriggerName.onSpeech, null, e.Speech))
			{
				return;
			}
			if (!e.Handled && e.HasKeyword(Keyword) && e.Mobile.InRange(Location, 2))
			{
				e.Handled = true;

				Mobile from = e.Mobile;
				var g = from.Guild as Guild;

			    var ethic = Ethics.Player.Find(e.Mobile);

                if (g == null && ethic == null || g != null && g.Type != Type && ethic == null)
				{
					Say(SignupNumber);
				}
                else if (ethic != null && Shield is OrderShield && ethic.Ethic is EvilEthic ||
                         ethic != null && Shield is ChaosShield && ethic.Ethic is HeroEthic)
                {
                    Say("Begone!  You do not follow the proper ethic to wield one of our order's shields.");
                }
				else
				{
					Container pack = from.Backpack;
					BaseShield shield = Shield;
					Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);

					if ((pack != null && pack.FindItemByType(shield.GetType()) != null) ||
						(twoHanded != null && shield.GetType().IsInstanceOfType(twoHanded)))
					{
						Say(1007110); // Why dost thou ask about virtue guards when thou art one?
						shield.Delete();
					}
					else if (from.PlaceInBackpack(shield))
					{
						Say(Utility.Random(1007101, 5));
						Say(1007139); // I see you are in need of our shield, Here you go.
						from.AddToBackpack(shield);
					}
					else
					{
						from.SendLocalizedMessage(502868); // Your backpack is too full.
						shield.Delete();
					}
				}
			}

			base.OnSpeech(e);
		}

		public BaseShieldGuard(Serial serial)
			: base(serial)
		{ }

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			// trigger returns true if returnoverride
			if (XmlScript.HasTrigger(this, TriggerName.onDragDrop) &&
				UberScriptTriggers.Trigger(this, from, TriggerName.onDragDrop, dropped))
			{
				return true;
			}

			bool bReturn = false;

			if (dropped is Head2)
			{
				var h = (Head2)dropped;

				if (h.Player == null || h.Player.Deleted)
				{
					Say("This belonged to a nobody!");
					h.Delete();
					return true;
				}

				if (h.Player != null)
				{
					Mobile mob = h.Player;

					from.CloseGump(typeof(HeadNegotiateGump));
					mob.CloseGump(typeof(HeadOwnerListGump));

					if (mob.Kills >= 5 && !h.Expired)
					{
						if (!h.Player.InStat)
						{
							LoggingCustom.Log("MurdererNegotiate.txt", DateTime.Now + "\t" + mob + "\t" + "STATTED by " + from.Name);

						    if (CentralGump.EnsureProfile(mob as PlayerMobile).BuyHead)
						    {
						        if (PendingStatloss == null)
						        {
						            PendingStatloss = new List<HeadTurnInObj>();
						        }

						        if (PendingStatlossTimer == null || !PendingStatlossTimer.Running)
						        {
						            PendingStatlossTimer = new InternalTimer();
                                    PendingStatlossTimer.Start();
						        }

						        if (!PendingStatloss.Exists(x => x.HeadOwner == mob as PlayerMobile))
						        {
						            var headobject = new HeadTurnInObj(DateTime.UtcNow + TimeSpan.FromMinutes(5), from as PlayerMobile, mob as PlayerMobile);
                                    PendingStatloss.Add(headobject);

                                    Conquests.CheckProgress<ItemConquest>(from as PlayerMobile, h, "turnin");
                                    bReturn = true;
                                    h.Delete();
						        }
                                else
						        {
						            from.SendMessage(61,
						                "This player is currently pending statloss.  Try to turn the head in again in 5 minutes.");
						        }
						    }
						    else
						    {
                                Conquests.CheckProgress<ItemConquest>(from as PlayerMobile, h, "turnin");
                                bReturn = true;
                                BeginSkillLoss(mob);
                                h.Delete();
						    }

							if (mob.Kills <= 10)
							{
								Say("My thanks for slaying this vile person.");
							}

							if (mob.Kills > 10 && mob.Kills < 25)
							{
								Say("Thank you for ridding Brittania of this evil fiend.");
							}

							if (mob.Kills >= 25)
							{
								Say("Thank you for destroying " + mob.Name + ", Britannia is a much safer place with that scum gone.");
							}
						}
						else //Owner of head is currently in statloss
						{
							Say("Begone.  This murderer has already met justice!");
							Say("I'll take that head, you just run along now.");
							bReturn = true;
							h.Delete();
						}
					}
					else if (h.Expired) //if head is decayed, set to true and change hue
					{
						Say("Why are you trying to give me this decayed piece of trash?");
						h.Expire();
					}
					else
					{
						Say("You disgusting miscreant!  Why are you giving me an innocent person's head?");
					}
				}
				else
				{
					Say("I suspect treachery....");
					Say("I'll take that head, you just run along now.");
					bReturn = true;
					h.Delete();
				}
			}

			return bReturn;
		}

		private static PollTimer _StatLossDecayTimer;

		public static void Configure()
		{
			if (_StatLossDecayTimer == null || !_StatLossDecayTimer.Running)
			{
				_StatLossDecayTimer = PollTimer.FromMinutes(
					30.0,
					// Every 30 minutes (this used to be on FoodDecay timer...)
					() =>
					{
						Mobile[] mobs = NetState.Instances.Where(ns => ns != null && ns.Mobile != null).Select(ns => ns.Mobile).ToArray();

						foreach (Mobile m in mobs)
						{
							StatLossDecay(m);
						}
					},
					() => NetState.Instances.Count > 0);
			}
		}

		public static void StatLossDecay(Mobile mob)
		{
			var pm = mob as PlayerMobile;

			if (pm == null || pm.StatEnd == DateTime.MinValue)
			{
				return; // quick check for not previously in stat loss
			}

			if (pm.InStat)
			{
				ApplySkillLoss(pm);
			}
			else
			{
				pm.StatEnd = DateTime.MinValue;
				ClearSkillLoss(pm);
				LoggingCustom.Log("MurdererNegotiate.txt", DateTime.Now + "\t" + pm + "\t" + "Finished Statloss");
			}
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			if (m.Player && m.AccessLevel == AccessLevel.Player && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild &&
				m.Hidden == false && m.InRange(this, 3) && DateTime.UtcNow >= m_NextSpeechTime)
			{
				if (Utility.RandomDouble() < 0.03)
				{
					m_NextSpeechTime = DateTime.UtcNow + m_SpeechDelay;

					switch (Utility.Random(5))
					{
						case 0:
							Say("Beware a thief is in our midst.");
							break;
						case 1:
							Say("Beware, {0}. For I know your true intentions.", m.Name);
							break;
						case 2:
							Say("Back away dirty thief, my possesions are no concern of yours.");
							break;
						case 3:
							Say("Citizens! {0} seeks to relieve you of your belongings.", m.Name);
							break;
						case 4:
							Say("Take heed scum, any thieving in these parts and thou shall feel my steel.");
							break;
					}
				}
			}

			base.OnMovement(m, oldLocation);
		}

		#region Skill/Stat Loss
		private static readonly Dictionary<Mobile, SkillLossContext> m_SkillLoss = new Dictionary<Mobile, SkillLossContext>();

		private class SkillLossContext
		{
			public Timer m_Timer;
			public List<SkillMod> m_SkillMods;
			//public List<StatMod> m_StatMods;
		}

		public static void ApplySkillLoss(Mobile mob)
		{
			var pm = mob as PlayerMobile;

			if (pm == null || pm.StatEnd < DateTime.UtcNow)
			{
				return;
			}

			try
			{
				TimeSpan lossperiod = pm.StatEnd - DateTime.UtcNow;
				double loss = 0.40; // 40% loss

				ClearSkillLoss(pm);

				var context = new SkillLossContext();
				m_SkillLoss[pm] = context;

				List<SkillMod> mods = context.m_SkillMods = new List<SkillMod>();

				foreach (Skill sk in pm.Skills)
				{
					double baseValue = sk.Base;

					if (baseValue > 0)
					{
						SkillMod mod = new DefaultSkillMod(sk.SkillName, true, -(baseValue * loss));

						mods.Add(mod);
						mob.AddSkillMod(mod);
					}
				}

				mob.AddStatMod(new StatMod(StatType.Str, "Murder Penalty Str", -(int)(mob.RawStr * loss), lossperiod));
				mob.AddStatMod(new StatMod(StatType.Dex, "Murder Penalty Dex", -(int)(mob.RawDex * loss), lossperiod));
				mob.AddStatMod(new StatMod(StatType.Int, "Murder Penalty Int", -(int)(mob.RawInt * loss), lossperiod));

				context.m_Timer = Timer.DelayCall(lossperiod, m => ClearSkillLoss(m), mob);
			}
			catch
			{ }
		}

		public static void BeginSkillLoss(Mobile mob)
		{
			var pm = mob as PlayerMobile;

			if (pm == null || pm.Kills < 5)
			{
				return;
			}

			try
			{
				mob.SendMessage("A player has turned in your head!");

				TimeSpan lossperiod = TimeSpan.FromMinutes(30 * mob.Kills);

				if (lossperiod > TimeSpan.FromMinutes(2880))
				{
					lossperiod = TimeSpan.FromMinutes(2880);
				}

				mob.SendMessage("You have entered statloss for: " + 30 * mob.Kills + " minutes.");

				if (mob.Kills >= 100)
				{
					mob.SendMessage("Your renown as a bloodthirsty murderer has lessened.");
					mob.Kills = mob.Kills / 2;
				}

				pm.StatEnd = DateTime.UtcNow + lossperiod;

				ApplySkillLoss(mob);
			}
			catch
			{ }
		}

		public static bool ClearSkillLoss(Mobile mob)
		{
			try
			{
				SkillLossContext context;

				if (m_SkillLoss.TryGetValue(mob, out context) && context != null)
				{
					m_SkillLoss.Remove(mob);

					List<SkillMod> mods = context.m_SkillMods;

					foreach (SkillMod t in mods)
					{
						mob.RemoveSkillMod(t);
					}

					context.m_Timer.Stop();

					mob.RemoveStatMod("Murder Penalty Str");
					mob.RemoveStatMod("Murder Penalty Dex");
					mob.RemoveStatMod("Murder Penalty Int");

					return true;
				}
			}
			catch
			{ }

			return false;
		}

		public static bool InSkillLoss(Mobile mob)
		{
			SkillLossContext context;

			return m_SkillLoss.TryGetValue(mob, out context) && context != null;
		}
		#endregion

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{

            PendingStatloss = new List<HeadTurnInObj>();
			base.Deserialize(reader);

			reader.ReadInt();
		}

        private class InternalTimer : Timer
        {
            public InternalTimer()
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(15.0))
            {
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                foreach (var head in PendingStatloss.ToArray())
                {
                    if (DateTime.UtcNow > head.TimeTurnedIn)
                    {
                        BeginSkillLoss(head.HeadOwner);
                        PendingStatloss.Remove(head);
                    }
                    else
                    {
                        var nextuse = head.TimeTurnedIn - DateTime.UtcNow;
                        head.HeadOwner.SendMessage("You have " + nextuse.Minutes + " minutes left to buy back your head for 30k.  To do so, say I wish to buy my head.");
                    }
                }

                if (PendingStatloss.Count <= 0)
                    Stop();
            }
        }
	}
}