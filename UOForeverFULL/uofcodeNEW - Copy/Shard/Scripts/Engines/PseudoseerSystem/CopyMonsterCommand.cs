using System;
using System.Reflection;
using Server.Mobiles;
using Server.Targeting;
using Server.Games;

namespace Server.Commands
{
    class CopyMonsterCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("copymonster", CreaturePossession.FullAccessStaffLevel, new CommandEventHandler(Copy_Monster_Attributes_Command));
        }

        public static MonsterAttributes currentAttributesClipboard = null;

        [Usage("copymonster [to]")]
        [Description("Copies skills and stats from one NPC to another. Used without an argument, the NPC's stats and skills are captured. " +
            "Including the 'to' argument allows the application of the attributes to the targeted creature.")]
        public static void Copy_Monster_Attributes_Command(CommandEventArgs e)
        {
            if (e.Arguments.Length == 0)
            {
                e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(Copy_Monster_From));
                e.Mobile.SendMessage("Target monster to copy stats and skills from.");
            }
            else if (e.Arguments[0].ToLower() == "to")
            {
                if (currentAttributesClipboard == null)
                {
                    e.Mobile.SendMessage("No monster's attributes have been copied. Use \"[copymonster\" to do this.");
                }
                else
                {
                    e.Mobile.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Copy_Monster_To));
                }
            }
            else
            {
                e.Mobile.SendMessage("Invalid argument");
            }
        }

        public static void Copy_Monster_From(Mobile from, Object o)
        {
            if (o is BaseCreature)
            {
                currentAttributesClipboard = new MonsterAttributes((BaseCreature)o);
                from.SendMessage("Select a monster to copy these attributes to.");
                from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(Copy_Monster_To));
            }
            else
            {
                from.SendMessage("Not a valid target!");
            }
        }

        public static void Copy_Monster_To(Mobile from, Object o)
        {
            if (currentAttributesClipboard == null) return;
            if (o is BaseCreature)
            {
                currentAttributesClipboard.ApplyTo((BaseCreature)o);
            }
            else if (o is PlayerMobile)
            {
                from.SendMessage("Cannot copy attributes to a PlayerMobile!");
            }
            else if (o is StaticTarget || o is LandTarget)
            {
                IPoint3D Ilocation = (IPoint3D)o;
                Point3D location = new Point3D(Ilocation.X, Ilocation.Y, Ilocation.Z);
                ConstructorInfo constructorInfoObj = currentAttributesClipboard.MonsterType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public, null,
                    CallingConventions.HasThis, System.Type.EmptyTypes, null);
                BaseCreature newMonster = (BaseCreature)constructorInfoObj.Invoke(null);
                newMonster.MoveToWorld(location, Map.Felucca);
                currentAttributesClipboard.ApplyTo(newMonster);
            }
            else
            {
                from.SendMessage("Not a valid target!");
            }
        }

        public class MonsterAttributes
        {
            public readonly Type MonsterType;
            int DamageMax;
            int DamageMin;

            int RawStr;
            int RawDex;
            int RawInt;
            int HitsMax;
            int Mana;
            int Stam;
            int Hue;
            int VirtualArmor; // not sure if this should be physical resistance?
            string Name;

            Skill[] skills;

            public MonsterAttributes(BaseCreature m)
            {
                MonsterType = m.GetType();
                RawStr = m.RawStr;
                RawDex = m.RawDex;
                RawInt = m.RawInt;
                HitsMax = m.HitsMax;
                Mana = m.ManaMax;
                Stam = m.StamMax;
                VirtualArmor = m.VirtualArmor; // not sure if this should be physical resistance?
                DamageMax = m.DamageMax;
                DamageMin = m.DamageMin;
                Hue = m.Hue;
                Name = m.Name;

                SkillInfo[] info = SkillInfo.Table;

                skills = new Skill[info.Length];
                for (int i = 0; i < m.Skills.Length; i++)
                {
                    skills[i] = m.Skills[i];
                }
            }

            public void ApplyTo(BaseCreature m)
            {
                m.RawStr = RawStr;
                m.RawDex = RawDex;
                m.RawInt = RawInt;
                m.HitsMaxSeed = HitsMax;
                m.Hits = HitsMax;
                m.Mana = Mana;
                m.Stam = Stam;
                m.VirtualArmor = VirtualArmor; // not sure if this should be physical resistance?
                m.DamageMin = DamageMin;
                m.DamageMax = DamageMax;
                m.Hue = Hue;
                m.Name = Name;

                for (int i = 0; i < skills.Length; i++)
                {
                    m.Skills[i].Base = skills[i].Base;
                }
            }
        }

    }
}
