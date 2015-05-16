#region References

using System;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Targeting;

#endregion

namespace Server.Ethics.Hero
{
    public sealed class HolyBlade : Power
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Sanctify Armament", "", 230);

        public static readonly int SapphiresRequired = 3;

        public HolyBlade()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                15,
                "Sanctify Armament",
                "Erst Reyam",
                "Sanctify a weapon or piece of armor, strengthening it against the opposing ethic.",
                20741
                );
        }

        public HolyBlade(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                15,
                "Sanctify Armament",
                "Erst Reyam",
                "Sanctify a weapon or piece of armor, strengthening it against the opposing ethic.",
                20741
                );
            EthicCaster = Caster;
        }

        public override void OnCast()
        {
            EthicCaster.Mobile.BeginTarget(12, false, TargetFlags.None, new TargetStateCallback(Power_OnTarget),
                EthicCaster);
            EthicCaster.Mobile.SendMessage("Which item do you wish to sanctify?");

            FinishSequence();
        }


        public override void BeginInvoke(Player from)
        {
            new HolyBlade(from).Cast();
        }

        private void Power_OnTarget(Mobile fromMobile, object obj, object state)
        {
            var from = state as Player;

            var item = obj as Item;

            if (item == null || item.Deleted)
            {
                return;
            }

            if (item is BaseArmor || item is BaseWeapon)
            {
                EthicsItem ethicItem = EthicsItem.Find(item);

                if (item.Parent != fromMobile)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "You may only imbue items you are wearing.");
                }
                else if (ethicItem == null)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "This item has not been imbued with a holy blessing.");
                }
                else if (ethicItem.Ethic != Ethic.Find(fromMobile))
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "The magic surrounding this item repels your attempts to imbue.");
                }
                else if (item is BaseWeapon && ((BaseWeapon) item).DamageLevel != WeaponDamageLevel.Regular ||
                         item is BaseArmor && ((BaseArmor) item).ProtectionLevel != ArmorProtectionLevel.Regular)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "You cannot imbue magic items!");
                }
                else if (!fromMobile.Backpack.ConsumeTotal(typeof(Sapphire), SapphiresRequired))
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        String.Format("You must sacrifice {0} sapphire{1} to imbue this item.", SapphiresRequired,
                            (SapphiresRequired != 1) ? "s" : String.Empty));
                }
                else if (CheckInvoke(from))
                {
                    if (!ethicItem.IsRunic)
                    {
                        ethicItem.StartExpiration();
                        ethicItem.MakeRunic();

                        fromMobile.FixedEffect(0x375A, 10, 20, 1153, 0);
                        fromMobile.PlaySound(0x209);

                        FinishInvoke(from);
                        fromMobile.LocalOverheadMessage(MessageType.Regular, 90, false,
                            "You have sanctified this item.");
                    }
                    else
                    {
                        fromMobile.LocalOverheadMessage(MessageType.Regular, 90, false,
                            "You have already sanctified this item.");
                    }
                }
            }
            else
            {
                fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500352);
                    // This is neither weapon nor armor.
            }
        }
    }
}