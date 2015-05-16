#region References

using Server.Commands.Generic;
using Server.Items;
using Server.Targeting;

#endregion

namespace Server.Commands
{
    public class Heal
    {
        [CommandAttribute("Heal", AccessLevel.EventMaster)]
        public static void Heal_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new HealTarget(e.Mobile);
        }

        private class HealTarget : Target
        {
            private readonly Mobile _M;

            public HealTarget(Mobile m)
                : base(-1, true, TargetFlags.None)
            {
                _M = m;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (!BaseCommand.IsAccessible(from, o))
                {
                    from.SendMessage("That is not accessible.");
                }
                else if (o is Mobile)
                {
                    var m = (Mobile) o;

                    if (!m.Alive)
                    {
                        m.Resurrect();
                    }

                    m.Hits = m.HitsMax;
                    m.Mana = m.ManaMax;
                    m.Stam = m.StamMax;

                    m.Hunger = 20;
                    m.Thirst = 20;

                    m.Poison = null;
                }
                else if (o is Item)
                {
                    var item = (Item) o;

                    if (item is BaseArmor)
                    {
                        ((BaseArmor) item).HitPoints = ((BaseArmor) item).MaxHitPoints;
                    }

                    if (item is BaseWeapon)
                    {
                        ((BaseWeapon) item).HitPoints = ((BaseWeapon) item).MaxHitPoints;
                    }
                }
            }
        }
    }
}