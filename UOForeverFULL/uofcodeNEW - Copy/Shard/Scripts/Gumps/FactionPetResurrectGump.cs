// **********
// RunUO Shard - FactionPetResurrectGump.cs
// **********

#region References

using System;
using Server.Mobiles;
using Server.Network;
using Server.Engines.XmlSpawner2;

#endregion

namespace Server.Gumps
{
    public class FactionPetResurrectGump : Gump
    {
        private BaseCreature m_Pet;
        private readonly double _hitsScalar;
        private int _price;

        public FactionPetResurrectGump(Mobile from, BaseCreature pet, int price)
            : this(from, pet, 0.0, price)
        {
        }

        public FactionPetResurrectGump(Mobile from, BaseCreature pet, double hitsScalar, int price)
            : base(50, 50)
        {
            from.CloseGump(typeof (FactionPetResurrectGump));

            m_Pet = pet;
            _hitsScalar = hitsScalar;
            _price = price;

            AddPage(0);

            AddBackground(10, 10, 265, 160, 0x242C);

            AddItem(205, 40, 0x4);
            AddItem(227, 40, 0x5);

            AddItem(180, 78, 0xCAE);
            AddItem(195, 90, 0xCAD);
            AddItem(218, 95, 0xCB0);

            AddHtmlLocalized(30, 30, 150, 75, 1049665, false, false);
            // <div align=center>Wilt thou sanctify the resurrection of:</div>
            AddHtml(30, 70, 150, 25, String.Format("<div align=CENTER>{0}</div>", pet.Name), true, false);


            AddLabel(40, 100, 1153, "Cost: " + price + " gold coins");
            AddButton(40, 120, 0x81A, 0x81B, 0x1, GumpButtonType.Reply, 0); // Okay
            AddButton(110, 120, 0x819, 0x818, 0x2, GumpButtonType.Reply, 0); // Cancel
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Pet.Deleted || !m_Pet.IsBonded || !m_Pet.IsDeadPet)
                return;

            Mobile from = state.Mobile;

            if (info.ButtonID == 1)
            {
                if (m_Pet.Map == null || !m_Pet.Map.CanFit(m_Pet.Location, 16, false, false))
                    from.SendLocalizedMessage(503256); // You fail to resurrect the creature.

                else
                {
                    if (UberScriptFunctions.Methods.TAKEGOLDFROM(null, from, _price))
                    {
                        m_Pet.PlaySound(0x214);
                        m_Pet.FixedEffect(0x376A, 10, 16);
                        m_Pet.ResurrectPet();
                    }
                    else
                    {
                        from.SendMessage("You can't afford that!");
                    }
                }
            }
        }
    }
}