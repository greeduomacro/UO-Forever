#region References
using System;
using System.Collections.Generic;

using Server.ContextMenus;
#endregion

namespace Server.Mobiles
{
	public abstract class VanityCreature : BaseCreature
	{
		private static readonly TimeSpan _ThinkUpdateInterval = TimeSpan.FromSeconds(1.0);

		private DateTime _NextThinkUpdate;

		public override bool AllowNewPetFriend { get { return false; } }
		public override bool AlwaysAttackable { get { return false; } }
		public override bool AlwaysMurderer { get { return false; } }
		public override bool AreaPeaceImmune { get { return true; } }
		public override bool BardImmune { get { return true; } }
		public override bool BleedImmune { get { return true; } }
		public override bool CanAngerOnTame { get { return false; } }
		public override bool CanBreath { get { return false; } }
		public override bool CanDrop { get { return false; } }
		public override bool CanFlee { get { return false; } }
		public override bool CanMoveOverObstacles { get { return true; } }
		public override bool CanOpenDoors { get { return true; } }
		public override bool CanRummageCorpses { get { return false; } }
		public override bool CanTarget { get { return true; } }
		public override bool CanTeach { get { return false; } }
		public override bool ClickTitle { get { return false; } }
		public override bool Commandable { get { return false; } }
		public override bool DeleteCorpseOnDeath { get { return true; } }
		public override bool DeleteOnRelease { get { return true; } }
		public override bool DisplayWeight { get { return false; } }
		public override bool HasAura { get { return false; } }
		public override bool HasBreath { get { return false; } }
		public override bool IgnoreYoungProtection { get { return false; } }
		public override bool IsBondable { get { return true; } }
		public override bool IsDispellable { get { return false; } }
		public override bool IsScaredOfScaryThings { get { return false; } }
		public override bool IsScaryToPets { get { return false; } }
		public override bool KeepsItemsOnDeath { get { return true; } }
		public override bool NoHouseRestrictions { get { return true; } }
		public override bool PlayerRangeSensitive { get { return false; } }
		public override bool RegenThroughPoison { get { return true; } }
		public override bool ShowFameTitle { get { return false; } }
		public override bool StatLossAfterTame { get { return false; } }
		public override bool Unprovokable { get { return true; } }
		public override bool UnprovokableTarget { get { return true; } }
		
		public override Poison PoisonImmune { get { return Poison.Fatal; } }

		public override string DefaultName { get { return "Incognito"; } }

		public VanityCreature(AIType ai)
			: base(ai, FightMode.None, 16, 1, 0.1, 0.2)
		{
			Name = NameList.RandomName("gargoyle vendor");

			Female = Utility.RandomBool();
			Body = Female ? 667 : 666;

			Loyalty = MaxLoyalty;
			ControlSlots = 0;

			Tamable = false;
			Blessed = true;

			VirtualArmor = 100;

			DamageMin = DamageMax = 0;

			SetStr(100);
			SetDex(100);
			SetInt(100);

			SetHits(100);
			SetStam(100);
			SetMana(100);
		}

		public VanityCreature(Serial serial)
			: base(serial)
		{ }

		public override void OnThink()
		{
			base.OnThink();

			if (Deleted || Map == null || Map == Map.Internal)
			{
				return;
			}

			Combatant = null;
			Warmode = false;
			Loyalty = MaxLoyalty;

			if (ControlMaster == null)
			{
				IsBonded = false;
				return;
			}

			IsBonded = true;
			Hidden = ControlMaster.Hidden;

			var now = DateTime.UtcNow;

			if (_NextThinkUpdate < now)
			{
				return;
			}

			_NextThinkUpdate = now.Add(_ThinkUpdateInterval);

			OnThinkUpdate();
		}

		protected virtual void OnThinkUpdate()
		{
			ControlOrder = OrderType.Come;
			ControlTarget = ControlMaster;
		}
		
		public override void GetContextMenuEntries(Mobile m, List<ContextMenuEntry> list)
		{ }

		public override TimeSpan ComputeMovementSpeed(Direction dir, bool checkTurning)
		{
			return RunMount;
		}

		public override void OnDoubleClick(Mobile from)
		{ }

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			return false;
		}

		public override bool CanBeBegged(Mobile from)
		{
			return false;
		}

		public override bool CanBeDamaged()
		{
			return false;
		}

		public override bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
		{
			return false;
		}

		public override bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
		{
			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}