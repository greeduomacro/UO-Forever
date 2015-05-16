#region References

using System;
using System.Collections.Generic;
using Server.Engines.Conquests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells;
using Server.Targeting;

#endregion

namespace Server.Ethics.Evil
{
    public sealed class HolyUnion : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Holy Union", "", 230);

        public HolyUnion()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                50,
                "Holy Union",
                "Vingir Yeliab",
                "Expend a moderate amount of life force to resurrect an ally or pet",
                23001
                );
        }

        public HolyUnion(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                100,
                "Holy Union",
                "Vingir Yeliab",
                "Expend a moderate amount of life force to resurrect an ally or pet",
                23001
                );
            EthicCaster = Caster;
        }

        public override void BeginInvoke(Player from)
        {
            new HolyUnion(from).Cast();
        }

        public override void OnCast()
        {
            EthicCaster.Mobile.BeginTarget(12, false, TargetFlags.None, new TargetStateCallback(Power_OnTarget),
                EthicCaster);
            EthicCaster.Mobile.SendMessage("Who do you wish to resurrect?");

            FinishSequence();
        }

        private void Power_OnTarget(Mobile fromMobile, object obj, object state)
        {
            var from = state as Player;

            var to = obj as Mobile;

            if (to == null)
            {
                EthicCaster.Mobile.SendMessage(54, "You must target a dead ally or bonded pet!");
                return;
            }
            if (!to.Alive && to is PlayerMobile && !(to.Region.IsPartOf(typeof(HouseRegion))))
            {
                to.PlaySound(0x214);
                to.FixedEffect(0x376A, 10, 16);
                to.CloseGump(typeof(ResurrectGump));
                to.SendGump(new ResurrectGump(to, EthicCaster.Mobile));
            }
            else if (to.IsDeadBondedPet && !(EthicCaster.Mobile.Region.IsPartOf(typeof(HouseRegion))))
            {
                var pet = to as BaseCreature;

                if (pet != null)
                {
                    if (pet.Map == null || !pet.Map.CanFit(pet.Location, 16, false, false))
                    {
                        EthicCaster.Mobile.SendMessage(54, "Target cannot be resurrected at that location.");     
                    }

                    if (pet.IsDeadPet)
                    {
                        Mobile master = pet.ControlMaster;

                        if (master != null && EthicCaster.Mobile == master)
                        {
                            pet.ResurrectPet();

                            foreach (Skill s in pet.Skills)
                            {
                                s.Base -= 0.1;
                            }
                            Conquests.CheckProgress<ResConquest>(master as PlayerMobile, pet);
                        }
                        else if (master != null && master.InRange(pet, 3))
                        {
                            EthicCaster.Mobile.SendMessage(54, "You were able to resurrect the creature."); 

                            master.CloseGump(typeof(PetResurrectGump));
                            master.SendGump(new PetResurrectGump(EthicCaster.Mobile, pet));

                            Conquests.CheckProgress<ResConquest>(EthicCaster.Mobile as PlayerMobile, pet);
                        }
                        else
                        {
                            bool found = false;

                            List<Mobile> friends = pet.Friends;

                            for (int i = 0; friends != null && i < friends.Count; ++i)
                            {
                                Mobile friend = friends[i];

                                if (!friend.InRange(pet, 3))
                                {
                                    continue;
                                }

                                EthicCaster.Mobile.SendMessage(54, "You were able to resurrect the creature."); 

                                friend.CloseGump(typeof(PetResurrectGump));
                                friend.SendGump(new PetResurrectGump(EthicCaster.Mobile, pet));

                                found = true;
                                Conquests.CheckProgress<ResConquest>(EthicCaster.Mobile as PlayerMobile, pet);
                                break;
                            }

                            if (!found)
                            {
                                EthicCaster.Mobile.SendMessage(54, "The pet owner must be near to attempt the resurrection."); 
                            }
                        }
                    }
                }
            }
            else if (to.Region.IsPartOf(typeof(HouseRegion)))
            {
                EthicCaster.Mobile.SendMessage(54, "You cannot resurrect them while they are in a house.");                
            }
            else
            {
                EthicCaster.Mobile.SendMessage(54, "You must target a dead ally or bonded pet!");
            }
            FinishInvoke(EthicCaster);
            FinishSequence();
        }
    }
}