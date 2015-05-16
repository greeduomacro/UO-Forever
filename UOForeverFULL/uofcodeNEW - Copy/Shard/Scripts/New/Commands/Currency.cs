#region References
using System;

using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

using VitaNex.Targets;
#endregion

namespace Server
{
	public class Currency
	{
		public static void Initialize()
		{
			CommandSystem.Register("Consume", AccessLevel.GameMaster, OnConsumeCurrencyCommand);
		}

		[Usage("Consume <amount>")]
		[Description("Consumes money from a player.")]
		private static void OnConsumeCurrencyCommand(CommandEventArgs e)
		{
			if (e.Length != 1)
			{
				e.Mobile.SendMessage("Invalid Arguements");
				return;
			}

			int amount = Utility.ToInt32(e.Arguments[0]);

			if (amount <= 0)
			{
				e.Mobile.SendMessage("Invalid amount specified.");
			}
			else
			{
				e.Mobile.Target = new ConsumeCurrencyTarget(amount);
				e.Mobile.SendMessage("Who would you like to consume money/checks from?");
			}
		}

		private class ConsumeCurrencyTarget : MobileSelectTarget<PlayerMobile>
		{
			private readonly int m_Amount;

			public ConsumeCurrencyTarget(int amount)
				: base(null, null, -1, false, TargetFlags.None)
			{
				m_Amount = amount;
			}

			protected override void OnTarget(Mobile from, PlayerMobile targeted)
			{
				if (targeted == null)
				{
					from.SendMessage("Invalid target specified.");
					return;
				}

				var m = (Mobile)targeted;

				Type cType = m.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

				if (Banker.WithdrawPackAndBank(m, cType, m_Amount))
				{
					from.SendMessage("Consumed {0:#,0} {1}.", m_Amount, cType.Name);
				}
				else
				{
					from.SendMessage("{0} lacks that much {1}.", m.Name, cType.Name);
				}
			}
		}
	}
}