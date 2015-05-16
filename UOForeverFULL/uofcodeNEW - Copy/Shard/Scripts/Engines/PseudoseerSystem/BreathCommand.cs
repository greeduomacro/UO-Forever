using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;

namespace Server.Commands
{
    class BreathCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("breath", AccessLevel.Player, new CommandEventHandler(Breath_Command));
        }

        public static void Breath_Command(CommandEventArgs e)
        {
            if (e.Mobile is BaseCreature)
            {
                BaseCreature creature = (BaseCreature)e.Mobile;
                if (creature.CanBreath)
                {
                    if (DateTime.UtcNow >= creature.NextBreathTime)
                    {
                        if (e.Length > 0 && e.Arguments[0].ToLower() == "target")
                        {
                            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(FireBreathTarget));
                        }
                        else
                        {
                            if (creature.Combatant != null)
                            {
                                Mobile target = creature.Combatant;
                                AttemptFireBreath(creature, target);
                            }
                            else
                            {
                                creature.SendMessage("You aren't in combat with anything.  Use \"[breath target\" to breath on a specific target, "
                                    + "or use \"[breath\" again after entering combat.");
                            }
                        }
                    }
                    else
                    {
                        e.Mobile.SendMessage("You are not ready to breath fire again!  You must wait another " + (int)(creature.NextBreathTime - DateTime.UtcNow).TotalSeconds + " seconds!");
                    }
                }
                else
                {
                    e.Mobile.Say("[breath " + e.ArgString);
                }
            }
            else
            {
                e.Mobile.Say("[breath " + e.ArgString);
            }
        }

        private static void FireBreathTarget(Mobile from, object o)
        {
            if (!(o is Mobile) || ((Mobile)o).Alive == false )
            {
                from.SendMessage("That is not a living target!");
                return;
            }
            if (from is BaseCreature)
            {
                AttemptFireBreath((BaseCreature)from, (Mobile)o);
            }
            else
            {
                from.SendMessage("You aren't a BaseCreature!  This shouldn't be possible... please contact Staff.");
            }
        }

        private static void AttemptFireBreath(BaseCreature creature, Mobile target)
        {
            if (!creature.InLOS(target))
            {
                creature.SendMessage("Target cannot be seen.");
                return;
            }
            if (!target.InRange(creature, creature.BreathRange))
            {
                creature.SendMessage("That is too far away.");
                return;
            }
            if (target != null && target.Alive && !target.IsDeadBondedPet && creature.CanBeHarmful(target) &&
                target.Map == creature.Map && !creature.IsDeadBondedPet &&
                !creature.BardPacified)
            {
                creature.BreathStart(target);
                creature.SetNextBreathTime();
            }
            else
            {
                creature.SendMessage("You can't breathe fire on that!");
            }
        }

    }
}
