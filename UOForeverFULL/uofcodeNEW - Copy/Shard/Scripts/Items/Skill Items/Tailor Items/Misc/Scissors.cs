#region References
using System;

using Server.Gumps;
using Server.Network;
using Server.Targeting;
#endregion

namespace Server.Items
{
	public interface IScissorable
	{
		bool Scissor(Mobile from, Scissors scissors);
	}

	[Flipable(0xf9f, 0xf9e)]
	public class Scissors : Item
	{
		[Constructable]
		public Scissors()
			: base(0xF9F)
		{
			Weight = 1.0;
		}

		public Scissors(Serial serial)
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

		public override void OnDoubleClick(Mobile from)
		{
			from.SendLocalizedMessage(502434); // What should I use these scissors on?

			from.Target = new InternalTarget(this);
		}

		private class InternalTarget : Target
		{
			private readonly Scissors m_Item;

			public InternalTarget(Scissors item)
				: base(2, false, TargetFlags.None)
			{
				m_Item = item;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Item.Deleted)
				{
					return;
				}

				if (targeted is Item && !((Item)targeted).IsStandardLoot())
				{
					// Scissors can not be used on that to produce anything.
					// from.SendLocalizedMessage(502440); 

					if (targeted is IScissorable)
					{
						from.SendGump(new BlessedClothingCutGump((IScissorable)targeted, m_Item));
					}
					else
					{
						from.SendLocalizedMessage(502440);
					}
				}
				else if (m_Item.EraAOS && targeted == from)
				{
					// That doesn't seem like the smartest thing to do.
					// That was an encounter you don't wish to repeat.
					// Ha! You missed!
					from.SendLocalizedMessage(1062845 + Utility.Random(3));
				}
				else if (m_Item.EraSE && Utility.RandomDouble() > 0.20 && (from.Direction & Direction.Running) != 0 &&
						 DateTime.UtcNow - from.LastMoveTime < from.ComputeMovementSpeed(from.Direction))
				{
					// Didn't your parents ever tell you not to run with scissors in your hand?!
					from.SendLocalizedMessage(1063305);
				}
				else if (targeted is Item && !((Item)targeted).Movable)
				{
					if (targeted is IScissorable && (targeted is PlagueBeastInnard || targeted is PlagueBeastMutationCore))
					{
						var obj = (IScissorable)targeted;

						if (obj.Scissor(from, m_Item))
						{
							from.PlaySound(0x248);
						}
					}
				}
				else if (targeted is IScissorable)
				{
					var obj = (IScissorable)targeted;

					if (obj.Scissor(from, m_Item))
					{
						from.PlaySound(0x248);
					}
				}
				else
				{
					from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
				}
			}

			protected override void OnNonlocalTarget(Mobile from, object targeted)
			{
				if (targeted is IScissorable && (targeted is PlagueBeastInnard || targeted is PlagueBeastMutationCore))
				{
					var obj = (IScissorable)targeted;

					if (obj.Scissor(from, m_Item))
					{
						from.PlaySound(0x248);
					}
				}
				else
				{
					base.OnNonlocalTarget(from, targeted);
				}
			}
		}
	}

	public class BlessedClothingCutGump : Gump
	{
		private readonly IScissorable m_Target;
		private readonly Scissors m_Scissors;

		private enum Buttons
		{
			Cancel,
			Confirm,
		}

		public BlessedClothingCutGump(IScissorable target, Scissors scissors)
			: base(150, 50)
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			m_Target = target;
			m_Scissors = scissors;

			AddBackground(0, 0, 250, 170, 0x13BE);
			AddBackground(10, 10, 200, 150, 0xBB8);

			AddHtml(20, 30, 140, 60, "WARNING: If you cut that blessed cloth, the blessing will be lost!", false, false);
				// Do you wish to re-deed this decoration?

			AddHtmlLocalized(55, 100, 150, 25, 1011011, false, false); // CONTINUE
			AddButton(20, 100, 0xFA5, 0xFA7, (int)Buttons.Confirm, GumpButtonType.Reply, 0);

			AddHtmlLocalized(55, 125, 150, 25, 1011012, false, false); // CANCEL
			AddButton(20, 125, 0xFA5, 0xFA7, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			var item = m_Target as Item;

			if (item == null || item.Deleted || m_Scissors == null)
			{
				return;
			}

			if (info.ButtonID == (int)Buttons.Confirm)
			{
				Mobile m = sender.Mobile;
				if (item.RootParentEntity == m)
				{
					if (m_Target.Scissor(m, m_Scissors))
					{
						m.PlaySound(0x248);
					}
				}
				else
				{
					m.SendMessage("That item must be in your backpack to cut it!");
				}
			}
		}
	}
}