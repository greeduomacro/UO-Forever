

#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Spells;
using VitaNex.FX;
using VitaNex.Network;

#endregion

namespace Server.Mobiles
{
    [CorpseName("stone guardian rubble")]
    public class StoneGuardian : BaseCreature
    {
        private static readonly SkillName[] _Skills = ((SkillName) 0).GetValues<SkillName>();

        public override string DefaultName { get { return "a stone guardian"; } }
        public override bool CanDestroyObstacles { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public bool IsStone { get; set; }

        public Timer TurntoStoneTimer { get; set; }

        [Constructable]
        public StoneGuardian()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a stone guardian";
            Body = 400;
            Hue = 2407;

            SpecialTitle = "Cursed Citizen";
            TitleHue = 891;

            SetStr(767, 945);
            SetDex(66, 75);
            SetInt(46, 70);

            SetHits(476, 552);

            SetDamage(20, 25);

            foreach (SkillName skill in _Skills)
            {
                SetSkill(skill, 80.0, 140.0);
            }

            GiveEquipment();

            IsStone = true;
            Frozen = true;
            Blessed = true;

            SpeechHue = YellHue = EmoteHue = 731;

            VirtualArmor = 90;

            PackGold(3000, 3500);
            PackMagicItems(5, 5, 0.95, 0.95);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(5, 5, 0.80, 0.65);
            PackMagicItems(6, 6, 0.80, 0.65);
        }

        public StoneGuardian(Serial serial)
            : base(serial)
        {}

        public virtual void GiveEquipment()
        {
            RadiantWarSword weapon = new RadiantWarSword();
            weapon.Hue = 2407;
            weapon.Identified = true;
            weapon.Movable = false;
            AddItem(Immovable(weapon));

            RoyalShield shield = new RoyalShield();
            shield.Hue = 2407;
            shield.Identified = true;
            shield.Movable = false;
            AddItem(Immovable(shield));

            RoyalPlateHelm helm = new RoyalPlateHelm();
            helm.Hue = 2407;
            helm.Identified = true;
            AddItem(Immovable(helm));

            RoyalPlateArms arms = new RoyalPlateArms();
            arms.Hue = 2407;
            arms.Identified = true;
            AddItem(Immovable(arms));

            RoyalPlateGloves gloves = new RoyalPlateGloves();
            gloves.Hue = 2407;
            gloves.Identified = true;
            AddItem(Immovable(gloves));

            RoyalPlateChest tunic = new RoyalPlateChest();
            tunic.Hue = 2407;
            tunic.Identified = true;
            AddItem(Immovable(tunic));

            RoyalPlateLegs legs = new RoyalPlateLegs();
            legs.Hue = 2407;
            legs.Identified = true;
            AddItem(Immovable(legs));

            RoyalPlateGorget gorget = new RoyalPlateGorget();
            gorget.Hue = 2407;
            gorget.Identified = true;
            AddItem(Immovable(gorget));

            RoyalPlateBoots boots = new RoyalPlateBoots();
            boots.Hue = 2407;
            boots.Identified = true;
            AddItem(Immovable(boots));           
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!this.InCombat() && TurntoStoneTimer == null && IsStone == false)
            {
                TurntoStoneTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), TurntoStone);
                return;
            }
        }

        public override void OnDeath(Container c)
        {
            Say(Utility.RandomBool() ? "thank you..." : "freedom at last...");
            base.OnDeath(c);
        }

        public override bool OnBeforeDeath()
        {
            if (0.05 >= Utility.RandomDouble())
                PackItem(new StoneFragment());
            return base.OnBeforeDeath();
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            double distance = GetDistanceToSqrt(m.Location);
            if ((m is PlayerMobile && m.AccessLevel == AccessLevel.Player || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && m.Alive && !m.Hidden && distance <= 5 && Frozen)
            {
                IsStone = false;
                Blessed = false;
                Emote("*comes to life*");
                Say(Utility.RandomBool() ? "free me...." : "end my torment...");
                Frozen = false;
            }
            base.OnMovement(m, oldLocation);
        }

        public void TurntoStone()
        {
            TurntoStoneTimer = null;
            if (!this.InCombat())
            {
                IsStone = true;
                Hits += 150;
                Emote("*turns back to stone*");
                Frozen = true;
                Blessed = true;
            }            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}