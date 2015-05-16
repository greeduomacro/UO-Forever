#region References
using Server.Items;
#endregion

namespace Server.Scripts.Engines.ValorSystem
{
	[Flipable(7774, 7775)]
	public class ValorBoard : Item
	{
		[Constructable]
		public ValorBoard()
			: base(7774)
		{
			Movable = false;
			Name = "Valor Board";
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!m.InRange(GetWorldLocation(), 2) || ValorRewardController.Instance == null)
			{
				return;
			}
			m.CloseGump(typeof(ValorBoardGump));
			m.CloseGump(typeof(ValorBoardGumpBritish));
			m.CloseGump(typeof(ValorBoardGumpSide));
			m.SendGump(new ValorBoardGumpBritish());
			m.SendGump(new ValorBoardGump(ValorRewardController.Instance, 0, null, null, null, false));
			m.SendGump(new ValorBoardGumpSide(m, m, ValorRewardController.Instance));
		}

		public ValorBoard(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}