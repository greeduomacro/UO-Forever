#region References

using System;
using Server.Factions;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Targeting;

#endregion

namespace Server.Ethics.Evil
{
    public sealed class UnholyItem : Power
    {
        public static readonly int GoldRequired = 100;

        private static readonly SpellInfo m_Info = new SpellInfo("Desecrate Item", "", 230);

        public UnholyItem()
            : base(null, m_Info)
        {
            m_Definition = new PowerDefinition(
                100,
                "Desecrate Item",
                "Vidda K'balc",
                "Desecrate an item, turning it your ethic color.  This will also bless clothing.",
                21015
                );
        }

        public UnholyItem(Player Caster)
            : base(Caster.Mobile, m_Info)
        {
            m_Definition = new PowerDefinition(
                100,
                "Desecrate Item",
                "Vidda K'balc",
                "Desecrate an item, turning it your ethic color.  This will also bless clothing.",
                21015
                );
            EthicCaster = Caster;
        }

        public override void BeginInvoke(Player from)
        {
            new UnholyItem(from).Cast();
        }

        public override void OnCast()
        {
            EthicCaster.Mobile.BeginTarget(12, false, TargetFlags.None, new TargetStateCallback(Power_OnTarget),
                EthicCaster);
            EthicCaster.Mobile.SendMessage("Which item do you wish to imbue?");

            FinishSequence();
        }

        private void Power_OnTarget(Mobile fromMobile, object obj, object state)
        {
            var from = state as Player;

            var item = obj as Item;

            if (item == null || item.Deleted)
            {
                return;
            }

            if (item is IEthicsItem)
            {
                EthicsItem ethicItem = EthicsItem.Find(item);

                if (item.Parent != fromMobile)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "You may only imbue items you are wearing.");
                }
                else if (ethicItem != null)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 32, false,
                        "This item has already been imbued with an evil essence.");
                }
                else if (ethicItem != null && ethicItem.Ethic != Ethic.Find(fromMobile))
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "The magic surrounding this item repels your attempts to imbue.");
                }
                else if (item is IFactionItem && ((IFactionItem) item).FactionItemState != null)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "The magic surrounding this item is too chaotic to imbue.");
                }
                else if (item.LootType == LootType.Cursed)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "The magic surrounding this item resists your attempts to imbue.");
                }
                else if (item.LootType == LootType.Blessed && !(item is Spellbook) || item.BlessedFor != null)
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        "The magic surrounding this item is too strong to imbue.");
                }
                else if (!fromMobile.Backpack.ConsumeTotal(typeof(Gold), GoldRequired))
                {
                    fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                        String.Format("You must sacrifice {0} gold piece{1} to imbue this item.", GoldRequired,
                            (GoldRequired != 1) ? "s" : String.Empty));
                }
                else if (CheckInvoke(from))
                {
                    /*if ( ethicItem != null )
						ethicItem.StartExpiration();
					else*/
                    EthicsItem.Imbue(item, Ethic.Evil, Ethic.Evil.Definition.PrimaryHue);

                    fromMobile.FixedEffect(0x375A, 10, 20);
                    fromMobile.PlaySound(0x209);

                    fromMobile.LocalOverheadMessage(MessageType.Regular, 32, false,
                        "You have imbued this item with an evil essence.");

                    FinishInvoke(from);
                }
            }
            else
            {
                fromMobile.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "That cannot be imbued!");
            }
        }
    }
}