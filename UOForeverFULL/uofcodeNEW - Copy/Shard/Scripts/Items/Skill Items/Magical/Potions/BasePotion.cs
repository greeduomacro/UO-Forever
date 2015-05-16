#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Engines.ConPVP;
using Server.Engines.Conquests;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Regions;

using VitaNex.Modules.AutoPvP;
using VitaNex.Modules.AutoPvP.Battles;
#endregion

namespace Server.Items
{
	public enum PotionEffect
	{
		Nightsight,
		CureLesser,
		Cure,
		CureGreater,
		Agility,
		AgilityGreater,
		Strength,
		StrengthGreater,
		PoisonLesser,
		Poison,
		PoisonGreater,
		PoisonDeadly,
		Refresh,
		RefreshTotal,
		HealLesser,
		Heal,
		HealGreater,
		ExplosionLesser,
		Explosion,
		ExplosionGreater,
		Conflagration,
		ConflagrationGreater,
		MaskOfDeath, // Mask of Death is not available in OSI but does exist in cliloc files
		MaskOfDeathGreater, // included in enumeration for compatability if later enabled by OSI
		ConfusionBlast,
		ConfusionBlastGreater,
		ManaRefresh,
		TotalManaRefresh,
		AmnesiaLesser,
		Amnesia,
		AmnesiaGreater,
		CrippleLesser,
		Cripple,
		CrippleGreater,
		LethargyLesser,
		Lethargy,
		LethargyGreater,
		Invisibility,
		Parasitic,
		Darkglow,
	}

	public abstract class BasePotion : Item, ICraftable, ICommodity
	{
		private PotionEffect m_PotionEffect;

		public PotionEffect PotionEffect
		{
			get { return m_PotionEffect; }
			set
			{
				m_PotionEffect = value;
				InvalidateProperties();
			}
		}

		public virtual int DescriptionNumber { get { return LabelNumber; } }
		public virtual bool IsDeedable { get { return EraML; } }

		public override int LabelNumber { get { return 1041314 + (int)m_PotionEffect; } }

		public BasePotion(int itemID, PotionEffect effect)
			: base(itemID)
		{
			m_PotionEffect = effect;

			Stackable = false;
			Weight = 1.0;
		}

		public BasePotion(Serial serial)
			: base(serial)
		{ }

		protected override void OnExpansionChanged(Expansion old)
		{
			base.OnExpansionChanged(old);

			if (EraML)
			{
				Stackable = true;
			}
		}

		public virtual bool RequireFreeHand { get { return true; } }

		public static bool HasFreeHand(Mobile m)
		{
			Item handOne = m.FindItemOnLayer(Layer.OneHanded);
			Item handTwo = m.FindItemOnLayer(Layer.TwoHanded);

			if (handTwo is BaseWeapon)
			{
				handOne = handTwo;
			}

			if (handTwo is BaseRanged)
			{
				var ranged = (BaseRanged)handTwo;

				if (ranged.Balanced)
				{
					return true;
				}
			}

			return (handOne == null || handTwo == null);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (Movable)
			{
				if (from.InRange(GetWorldLocation(), 1))
				{
					if (!RequireFreeHand && !(from.FindItemOnLayer(Layer.TwoHanded) is BaseRanged)|| HasFreeHand(from))
					{
						if (this is BaseExplosionPotion && Amount > 1)
						{
							var pot = GetType().CreateInstanceSafe<BaseExplosionPotion>();

							if (pot != null)
							{
								Amount--;

								if (from.Backpack != null && !from.Backpack.Deleted)
								{
									from.Backpack.DropItem(pot);
								}
								else
								{
									pot.MoveToWorld(from.Location, from.Map);
								}

								if (pot.Drink(from))
								{
									//EventSink.InvokeOnConsume(new OnConsumeEventArgs(from, pot));

                                    if (from is PlayerMobile)
                                    {
                                        Conquests.CheckProgress<ItemConquest>((PlayerMobile)from, this);

                                        //CheckProgress<ConsumeItemConquest>((PlayerMobile)e.Consumer, e);
                                    }
								}
							}
						}
						else
						{
							if (Drink(from))
							{
                                //EventSink.InvokeOnConsume(new OnConsumeEventArgs(from, pot));

                                if (from is PlayerMobile)
                                {
                                    Conquests.CheckProgress<ItemConquest>((PlayerMobile)from, this);

                                    //CheckProgress<ConsumeItemConquest>((PlayerMobile)e.Consumer, e);
                                }
							}
						}
					}
					else
					{
						from.SendLocalizedMessage(502172); // You must have a free hand to drink a potion.
					}
				}
				else
				{
					from.SendLocalizedMessage(502138); // That is too far away for you to use
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write((int)m_PotionEffect);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
				case 0:
					{
						m_PotionEffect = (PotionEffect)reader.ReadInt();
						break;
					}
			}

			if (version == 0)
			{
				Stackable = EraML;
			}
		}

		public abstract bool Drink(Mobile from);

		public static void PlayDrinkEffect(Mobile m)
		{
			m.RevealingAction();

			m.PlaySound(0x2D6);

			var region1 = m.Region as CustomRegion;
			var pm = m as PlayerMobile;

			var battle = AutoPvP.FindBattleI<IUOFBattle>(pm);

			if (battle == null && !DuelContext.IsFreeConsume(m) && (region1 == null || !region1.PlayingGame(m)))
			{
				m.AddToBackpack(new Bottle());
			}

			if (m.Body.IsHuman /*&& !m.Mounted*/)
			{
				m.Animate(34, 5, 1, true, false, 0);
			}
		}

		public static int EnhancePotions(Mobile m)
		{
			int EP = AosAttributes.GetValue(m, AosAttribute.EnhancePotions);
			int skillBonus = m.Skills.Alchemy.Fixed / 330 * 10;

			if (m.EraML && EP > 50 && m.AccessLevel <= AccessLevel.Player)
			{
				EP = 50;
			}

			return (EP + skillBonus);
		}

		public static TimeSpan Scale(Mobile m, TimeSpan v)
		{
			//if ( !m.EraAOS )
			return v;

			//double scalar = 1.0 + ( 0.01 * EnhancePotions( m ) );

			//return TimeSpan.FromSeconds( v.TotalSeconds * scalar );
		}

		public static double Scale(Mobile m, double v)
		{
			//if ( !m.EraAOS )
			return v;

			//double scalar = 1.0 + ( 0.01 * EnhancePotions( m ) );

			//return v * scalar;
		}

		public static int Scale(Mobile m, int v)
		{
			//if ( !m.EraAOS )
			return v;

			//return AOS.Scale( v, 100 + EnhancePotions( m ) );
		}

		public override bool StackWith(Mobile from, Item dropped, bool playSound)
		{
			if (dropped is BasePotion && ((BasePotion)dropped).m_PotionEffect == m_PotionEffect)
			{
				return base.StackWith(from, dropped, playSound);
			}

			return false;
		}

		#region ICraftable Members
		public int OnCraft(
			int quality,
			bool makersMark,
			Mobile from,
			CraftSystem craftSystem,
			Type typeRes,
			IBaseTool tool,
			CraftItem craftItem,
			int resHue)
		{
			if (craftSystem is DefAlchemy)
			{
				Container pack = from.Backpack;

				if (pack != null && m_PotionEffect <= PotionEffect.TotalManaRefresh)
				{
					List<PotionKeg> kegs = pack.FindItemsByType<PotionKeg>();

					foreach (
						PotionKeg keg in kegs.Where(keg => keg != null && keg.Held > 0 && keg.Held < 100 && keg.Type == PotionEffect))
					{
						++keg.Held;

						Consume();
						from.AddToBackpack(new Bottle());

						return -1; // signal placed in keg
					}
				}
			}

			return 1;
		}
		#endregion
	}
}