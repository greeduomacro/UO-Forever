using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Engines.XmlSpawner2;

namespace Server.SkillHandlers
{
    public class ArmsLore
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ArmsLore].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTarget();

            m.SendLocalizedMessage(500349); // What item do you wish to get information about?

            return TimeSpan.FromSeconds(1.0);
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(2, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IEntity entity = targeted as IEntity;
                if (XmlScript.HasTrigger(entity, TriggerName.onTargeted) && UberScriptTriggers.Trigger(entity, from, TriggerName.onTargeted, null, null, null, 0, null, SkillName.ArmsLore, from.Skills[SkillName.ArmsLore].Value))
                {
                    return;
                }
                
                if (from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 100))
                {
                    // Alan mod: show exact durability points if succeeded
                    if (targeted is BaseWeapon)
                    {
                        BaseWeapon weap = (BaseWeapon)targeted;

                        from.SendMessage("This weapon has " + weap.HitPoints + "/" + weap.MaxHitPoints + " durability.");

                        int damage = (weap.DamageMax + weap.DamageMin) / 2;

                        if (weap.Quality == WeaponQuality.Exceptional)
                            damage += (int)(damage * 0.2);

                        if (weap.DamageLevel > WeaponDamageLevel.Regular)
                            damage += (int)((2.0 * (int)weap.DamageLevel) - 1.0);

                        int hand = (weap.Layer == Layer.OneHanded ? 0 : 1);

                        if (damage < 3)
                            damage = 0;
                        else
                            damage = ((Math.Min(damage, 30) - 1) / 5) + 1;

                        //damage = (int)Math.Ceiling( Math.Min( damage, 30 ) / 5.0 );

                        /*
                    else if ( damage < 6 )
                        damage = 1;
                    else if ( damage < 11 )
                        damage = 2;
                    else if ( damage < 16 )
                        damage = 3;
                    else if ( damage < 21 )
                        damage = 4;
                    else if ( damage < 26 )
                        damage = 5;
                    else
                        damage = 6;
                         * */

                        WeaponType type = weap.Type;

                        damage *= 9;

                        if (type == WeaponType.Ranged)
                            from.SendLocalizedMessage(1038224 + damage);
                        else if (type == WeaponType.Piercing)
                            from.SendLocalizedMessage(1038218 + hand + damage);
                        else if (type == WeaponType.Slashing)
                            from.SendLocalizedMessage(1038220 + hand + damage);
                        else if (type == WeaponType.Bashing)
                            from.SendLocalizedMessage(1038222 + hand + damage);
                        else
                            from.SendLocalizedMessage(1038216 + hand + damage);

                        if (weap.Poison != null && weap.PoisonCharges > 0)
                            from.SendLocalizedMessage(1038284); // It appears to have poison smeared on it.

                        /*				if ( weap is GlacialStaff && from.Skills[SkillName.ArmsLore].Value >= 90.0 )
                                        {
                                            GlacialStaff staff = (GlacialStaff)weap;
                                            int spells = 0; //Assume it has Ice Ball

                                            if ( staff.GetFlag( GlacialSpells.Freeze ) )
                                            {
                                                spells++;
                                                if ( staff.GetFlag( GlacialSpells.IceStrike ) )
                                                    spells++;
                                            }

                                            from.SendLocalizedMessage( 1038213 + spells ); 
                                        }
                                    }*/
                    }
                    else if (targeted is BaseArmor)
                    {
                        BaseArmor arm = (BaseArmor)targeted;

                        from.SendMessage("This armor has " + arm.HitPoints + "/" + arm.MaxHitPoints + " durability.");
                        
                        from.SendLocalizedMessage(1038295 + (int)Math.Ceiling(Math.Min(arm.ArmorRating(null), 35) / 5.0));
                    }
                    else if (targeted is SwampDragon && ((SwampDragon)targeted).HasBarding)
                    {
                        SwampDragon pet = (SwampDragon)targeted;

                        int perc = (4 * pet.BardingHP) / pet.BardingMaxHP;

                        if (perc < 0)
                            perc = 0;
                        else if (perc > 4)
                            perc = 4;

                        pet.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1053021 - perc, from.NetState);

                    }
                    else
                        from.SendLocalizedMessage(500352); // This is neither weapon nor armor.
    
                }
                else
                {
                    //from.SendLocalizedMessage(500353); // You are not certain...

                    // Alan mod: on failure, show the regular (dumb) general durability values
                    if (targeted is BaseWeapon)
                    {
                        BaseWeapon weap = (BaseWeapon)targeted;

                        if (weap.MaxHitPoints != 0)
                        {
                            int hp = (int)((weap.HitPoints / (double)weap.MaxHitPoints) * 10);

                            if (hp < 0)
                                hp = 0;
                            else if (hp > 9)
                                hp = 9;

                            from.SendLocalizedMessage(1038285 + hp);
                        }
                    }
                    else if (targeted is BaseArmor)
                    {
                        BaseArmor arm = (BaseArmor)targeted;

                        if (arm.MaxHitPoints != 0)
                        {
                            int hp = (int)((arm.HitPoints / (double)arm.MaxHitPoints) * 10);

                            if (hp < 0)
                                hp = 0;
                            else if (hp > 9)
                                hp = 9;

                            from.SendLocalizedMessage(1038285 + hp);
                        }
                    }
                    else if (targeted is SwampDragon && ((SwampDragon)targeted).HasBarding)
                    {
                        SwampDragon pet = (SwampDragon)targeted;

                        int perc = (4 * pet.BardingHP) / pet.BardingMaxHP;

                        if (perc < 0)
                            perc = 0;
                        else if (perc > 4)
                            perc = 4;

                        pet.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1053021 - perc, from.NetState);

                    }
                    else
                        from.SendLocalizedMessage(500352); // This is neither weapon nor armor.
                }
            }
        }
    }
}