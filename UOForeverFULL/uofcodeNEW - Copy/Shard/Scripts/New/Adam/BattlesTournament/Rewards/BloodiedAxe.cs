#region Header
//   Vorspire    _,-'/-'/  Axe.cs
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2014  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #        The MIT License (MIT)          #
#endregion

#region References
using System;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using VitaNex.FX;

#endregion

namespace VitaNex.Items
{
    public class BloodiedAxe : BaseThrowable<IEntity>
	{
		[Constructable]
		public BloodiedAxe()
			: this(1)
		{ }

		[Constructable]
		public BloodiedAxe(int amount)
			: base(11560, amount)
		{
			Name = "Bloodied Axe";

			Weight = 0.0;
			Stackable = true;
			Consumable = false;

            LootType = LootType.Blessed;

		    Hue = 1157;

		    AllowCombat = true;

			TargetFlags = TargetFlags.Harmful;

			DismountUser = true;

			ThrowSound = 513;
			ImpactSound = 1310;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.FromSeconds(15.0);

			RequiredSkillValue = 0.0;
		}

        public BloodiedAxe(Serial serial)
			: base(serial)
		{ }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "This axe has been drenched in blood.", 137);
        }

        public override bool CanThrowAt(Mobile from, IEntity target, bool message)
        {
            if (from == null || from.Deleted || target == null || target.Deleted || !base.CanThrowAt(from, target, message))
            {
                return false;
            }

            if (target is Mobile && !from.CanBeHarmful(target as Mobile, false, true))
            {
                from.SendMessage(54, "You cannot harm them");
                return false;              
            }
            if (target is BaseCreature)
            {
                return CanThrowAtMobile(target as BaseCreature);
            }

            if (target is Item)
            {
                return CanThrowAtItem(target as Item);
            }

            if (target is PlayerMobile)
            {
                from.SendMessage(54, "You cannot throw this axe at other players.");
                return false;
            }

            return false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m == null || m.Deleted)
            {
                return;
            }

            if (!IsChildOf(m.Backpack))
            {
                return;
            }

            if (!CanThrow(m, true))
            {
                return;
            }

            if (m.Spell != null)
            {
                m.Spell.OnCasterUsingObject(this);
            }

            if (ClearHands)
            {
                m.ClearHands();
            }

            if (DismountUser)
            {
                EtherealMount.StopMounting(m);
            }

            if (DismountUser && m.Mounted)
            {
                BaseMount.Dismount(m);
            }

            m.Target = BeginTarget(m);
            OnBeginTarget(m, true);
        }

        protected override void OnThrownAt(Mobile m, IEntity target)
        {
            if (m == null || m.Deleted || target == null)
            {
                return;
            }

            if (ImpactSound >= 0)
            {
                Effects.PlaySound(target.Location, target.Map, ImpactSound);
            }

            ThrownLast = DateTime.UtcNow;

            LastUsed = DateTime.UtcNow + ThrowRecovery;

            if (target is BaseCreature)
            {
                var creature = target as BaseCreature;
                if (creature.Alive)
                {
                    m.DoHarmful(creature);
                    creature.Damage(Utility.RandomMinMax(5, 20), m);
                }

                int range = 1;
                int zOffset = 10;

                Point3D src = target.Location.Clone3D(0, 0, zOffset);
                var points = src.GetAllPointsInRange(target.Map, range, range);

                Effects.PlaySound(target.Location, target.Map, 0x19C);

                Timer.DelayCall(
                    TimeSpan.FromMilliseconds(100),
                    () =>
                    {
                        foreach (Point3D trg in points)
                        {
                            int bloodID = Utility.RandomMinMax(4650, 4655);

                            new MovingEffectInfo(src, trg.Clone3D(0, 0, 2), target.Map, bloodID).MovingImpact(
                                info =>
                                {
                                    var blood = new Blood
                                    {
                                        ItemID = bloodID
                                    };
                                    blood.MoveToWorld(info.Target.Location, info.Map);

                                    Effects.PlaySound(info.Target, info.Map, 0x028);
                                });
                        }
                    });
            }

            if (ThrowRecovery > TimeSpan.Zero)
            {
                if (UpdateTimer == null)
                {
                    UpdateTimer = PollTimer.FromSeconds(
                        1.0,
                        () =>
                        {
                            ClearProperties();
                            Delta(ItemDelta.Properties);

                            DateTime readyWhen = ThrownLast + ThrowRecovery;

                            if (DateTime.UtcNow < readyWhen)
                            {
                                return;
                            }

                            m.EndAction(GetType());

                            if (UpdateTimer == null)
                            {
                                return;
                            }

                            UpdateTimer.Running = false;
                            UpdateTimer = null;
                        });
                }
                else
                {
                    UpdateTimer.Running = true;
                }
            }
            else
            {
                if (UpdateTimer != null)
                {
                    UpdateTimer.Running = false;
                    UpdateTimer = null;
                }

                ClearProperties();
                Delta(ItemDelta.Properties);
                m.EndAction(GetType());
            }
        }

        public bool CanThrowAtMobile(BaseCreature target)
        {
            if (!target.Alive || target.Blessed)
            {
                return false;
            }

            if (target.ControlMaster is PlayerMobile)
            {
                return false;
            }

            return true;
        }

        public bool CanThrowAtItem(Item item)
        {
            if (item.ParentEntity == null)
            {
                return true;
            }

            return false;
        }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					{
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					{
					}
					break;
			}
		}
	}
}