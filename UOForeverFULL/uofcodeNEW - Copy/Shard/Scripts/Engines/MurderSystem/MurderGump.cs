#region References
using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;

using Server.Commands;
using Server.Items;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace Server.Engines.MurderSystem
{
	public abstract class HeadNegotiateGump : SuperGump
	{
		public Item Head { get; protected set; }

		protected HeadNegotiateGump(PlayerMobile user, Item head)
			: base(user, null, 50, 50)
		{
			Head = head;
		}

		protected override void Compile()
		{
			if (Head.HeldBy != null)
			{
				Head.HeldBy.DropHolding();
			}

			base.Compile();
		}

		protected override void OnClick()
		{
			if (Head.HeldBy != null)
			{
				Head.HeldBy.DropHolding();
			}

			base.OnClick();
		}

		protected override void OnSend()
		{
			if (Head.HeldBy != null)
			{
				Head.HeldBy.DropHolding();
			}

			base.OnSend();
		}

		protected override void OnRefreshed()
		{
			if (Head.HeldBy != null)
			{
				Head.HeldBy.DropHolding();
			}

			base.OnRefreshed();
		}

		protected override void OnClosed(bool all)
		{
			if (Head.HeldBy != null)
			{
				Head.HeldBy.DropHolding();
			}

			base.OnClosed(all);
		}
	}

	public class VictimBuyHeadGump : HeadNegotiateGump
	{
		private readonly PlayerMobile _Killer;

		private int _Offer;
		private string _Message;

		public VictimBuyHeadGump(PlayerMobile victim, PlayerMobile killer, Item head)
			: base(victim, head)
		{
			_Killer = killer;

			_Offer = 0;
			_Message = String.Empty;

			CanDispose = false;
			CanClose = false;
			CanResize = false;
			CanMove = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add("bg", () => AddBackground(0, 0, 510, 378, 5170));

			layout.Add(
				"offer",
				() =>
				{
					AddLabel(166, 40, 32, @"Offer Amount:");
					AddTextEntryLimited(
						268,
						41,
						200,
						17,
						2622,
						_Offer.ToString(CultureInfo.InvariantCulture),
						9,
						(e, t) =>
						{
							if (!Int32.TryParse(t, out _Offer) || _Offer < 0)
							{
								_Offer = 0;
							}
						});
				});

			layout.Add("separator", () => AddImageTiled(146, 68, 330, 2, 9201));

			layout.Add(
				"message",
				() =>
				{
					AddLabel(41, 81, 90, "Write Message:");
					AddBackground(36, 107, 199, 223, 3500);
					AddTextEntry(59, 135, 152, 165, 2622, String.Empty, (e, t) => _Message = t ?? String.Empty);
				});

			layout.Add(
				"question",
				() =>
				{
					AddLabel(247, 124, 2622, @"Would you like to attempt to buy");
					AddLabel(247, 145, 2622, @"your head back from them?");
				});

			layout.Add("accept", () => AddButton(327, 207, 247, 248, b => OnAccept()));
			layout.Add("cancel", () => AddButton(403, 207, 243, 241, b => OnCancel()));
		}

		private void OnAccept()
		{
			if (_Offer <= 0)
			{
				User.SendMessage("Offer amount must be more than nothing!");
				Refresh(true);
				return;
			}

			if (_Offer > MurderSystemController._HeadRansomCap)
			{
				_Offer = MurderSystemController._HeadRansomCap;

				User.SendMessage(
					"There is a cap of {0:#,0} for the offer! Please enter a lower number.", MurderSystemController._HeadRansomCap);
				Refresh(true);
				return;
			}

			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

			int bal = Banker.GetBalance(User, cType);

			if (bal < _Offer)
			{
				_Offer = bal;

				User.SendMessage("You cannot afford this amount!  Please make a new offer.");
				Refresh(true);
				return;
			}

			new KillerReceiveOfferGump(_Killer, User, Head, _Offer, _Message).Send();
			User.SendMessage("Your offer request has been sent to {0}!", _Killer.RawName);

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Victim -> Killer][OFFER][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Killer,
					Head,
					_Offer,
					cType.Name,
					_Message));
		}
		
		private void OnCancel()
		{
			User.SendMessage("You decide not to negotiate with {0}.", _Killer.RawName);
		}
	}

	public class KillerSellHeadGump : HeadNegotiateGump
	{
		private readonly PlayerMobile _Victim;

		private int _Offer;
		private string _Message;

		public KillerSellHeadGump(PlayerMobile killer, PlayerMobile victim, Item head)
			: base(killer, head)
		{
			_Victim = victim;

			_Offer = 0;
			_Message = String.Empty;

			CanDispose = false;
			CanClose = false;
			CanResize = false;
			CanMove = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add("bg", () => AddBackground(0, 0, 510, 378, 5170));

			layout.Add(
				"offer",
				() =>
				{
					AddLabel(166, 40, 32, @"Offer Amount:");
					AddTextEntryLimited(
						268,
						41,
						200,
						17,
						2622,
						_Offer.ToString(CultureInfo.InvariantCulture),
						9,
						(e, t) =>
						{
							if (!Int32.TryParse(t, out _Offer) || _Offer < 0)
							{
								_Offer = 0;
							}
						});
				});

			layout.Add("separator", () => AddImageTiled(146, 68, 330, 2, 9201));

			layout.Add(
				"message",
				() =>
				{
					AddLabel(41, 81, 90, "Write Message:");
					AddBackground(36, 107, 199, 223, 3500);
					AddTextEntry(59, 135, 152, 165, 2622, String.Empty, (e, t) => _Message = t ?? String.Empty);
				});

			layout.Add(
				"question",
				() =>
				{
					AddLabel(247, 124, 2622, @"Would you like to attempt to sell");
					AddLabel(247, 145, 2622, @"their head back to them?");
				});

			layout.Add("accept", () => AddButton(327, 207, 247, 248, b => OnAccept()));
			layout.Add("cancel", () => AddButton(403, 207, 243, 241, b => OnCancel()));
		}

		private void OnAccept()
		{
			if (_Offer <= 0)
			{
				User.SendMessage("Offer amount must be more than nothing!");
				Refresh(true);
				return;
			}

			if (_Offer > MurderSystemController._HeadRansomCap)
			{
				_Offer = MurderSystemController._HeadRansomCap;

				User.SendMessage(
					"There is a cap of {0:#,0} for the offer! Please enter a lower number.", MurderSystemController._HeadRansomCap);
				Refresh(true);
				return;
			}

			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

			// We don't REALLY want to tell the killer that the victim has no gold
			// It should be handled by the offer gump where they are forced to accept/decline/counter.
			/*
			int bal = Banker.GetBalance(_Victim, cType);

			if (bal < _Offer)
			{
				_Offer = bal;

				User.SendMessage("They cannot afford this amount!  Please make a new offer.");
				Refresh(true);
				return;
			}
			*/

			new VictimReceiveOfferGump(_Victim, User, Head, _Offer, _Message).Send();
			User.SendMessage("Your offer demand has been sent to {0}!", _Victim.RawName);

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Killer -> Victim][OFFER][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Victim,
					Head,
					_Offer,
					cType.Name,
					_Message));
		}

		private void OnCancel()
		{
			User.SendMessage("You decide not to negotiate with {0}.", _Victim.RawName);
		}
	}

	public class VictimReceiveOfferGump : HeadNegotiateGump
	{
		private readonly PlayerMobile _Killer;
		private readonly int _Offer;
		private readonly string _Message;

		public VictimReceiveOfferGump(PlayerMobile victim, PlayerMobile killer, Item head, int offer, string message)
			: base(victim, head)
		{
			_Killer = killer;
			_Offer = offer;
			_Message = message;

			CanDispose = false;
			CanClose = false;
			CanResize = false;
			CanMove = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"open", () => AddButton(21, 0, 30, 30, b => new VictimResponseGump(User, _Killer, Head, _Offer, _Message).Send()));
			layout.Add("offer", () => AddLabel(0, 32, 32, "Offer from " + _Killer.RawName));
		}
	}

	public class VictimResponseGump : HeadNegotiateGump
	{
		private readonly PlayerMobile _Killer;
		private readonly int _Offer;
		private readonly string _Message;

		private int _CounterOffer;
		private string _ReplyMessage;

		private readonly BitArray _Options = new BitArray(new[] {true, false, false});

		public VictimResponseGump(PlayerMobile victim, PlayerMobile killer, Item head, int offer, string message)
			: base(victim, head)
		{
			_Killer = killer;
			_Offer = offer;
			_Message = message;

			_CounterOffer = 0;
			_ReplyMessage = String.Empty;

			CanClose = false;
			CanDispose = false;
			CanResize = false;
			CanMove = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add("bg", () => AddBackground(0, 0, 510, 470, 5170));

			layout.Add(
				"title",
				() =>
				AddHtml(
					20, 5, 470, 40, String.Format("Offer from {0}".WrapUOHtmlColor(Color.OrangeRed), _Killer.RawName), false, false));

			layout.Add(
				"message",
				() =>
				{
					AddLabel(36, 36, 90, @"Message:");
					AddHtml(36, 68, 200, 150, _Message ?? String.Empty, true, true);
				});

			layout.Add(
				"response",
				() =>
				{
					AddLabel(276, 36, 90, @"Write Response:");
					AddBackground(276, 66, 200, 225, 3500);
					AddTextEntryLimited(
						300, 90, 150, 175, 2622, _ReplyMessage ?? String.Empty, 1000, (e, t) => _ReplyMessage = t ?? String.Empty);
				});

			layout.Add(
				"offer",
				() =>
				{
					AddLabel(36, 230, 32, @"Ransom");
					AddImageTiled(36, 230, 200, 33, 93); // Spacer
					AddImage(36, 270, 53); // Amnt.
					AddLabel(80, 265, 2622, _Offer.ToString("#,0"));
				});

			layout.Add(
				"menu/accept",
				() =>
				{
					AddRadio(36, 305, 2152, 2154, _Options.Get(0), (r, s) => _Options.Set(0, s));
					AddLabel(70, 310, 2622, @"Accept");
				});

			layout.Add(
				"menu/counteroffer",
				() =>
				{
					AddRadio(192, 305, 2152, 2154, _Options.Get(1), (r, s) => _Options.Set(1, s));
					AddLabel(226, 310, 2622, @"Counteroffer");
				});

			layout.Add(
				"menu/decline",
				() =>
				{
					AddRadio(390, 305, 2152, 2154, _Options.Get(2), (r, s) => _Options.Set(2, s));
					AddLabel(424, 310, 2622, @"Decline");
				});

			layout.Add(
				"counteroffer",
				() =>
				{
					AddLabel(36, 360, 32, "Counteroffer Amount:");
					AddTextEntryLimited(
						190,
						360,
						280,
						20,
						2622,
						_CounterOffer.ToString(CultureInfo.InvariantCulture),
						9,
						(e, t) =>
						{
							if (!Int32.TryParse(t, out _CounterOffer) || _CounterOffer < 0)
							{
								_CounterOffer = 0;
							}
						});
					AddImageTiled(36, 360, 440, 33, 93); // Spacer
				});

			layout.Add(
				"ok",
				() => AddButton(
					224,
					406,
					247,
					248,
					b =>
					{
						if (_Options.Get(0))
						{
							OnAccept();
						}
						else if (_Options.Get(1))
						{
							OnCounteroffer();
						}
						else if (_Options.Get(2))
						{
							OnDecline();
						}
						else
						{
							Close();
						}
					}));
		}

		private void OnAccept()
		{
			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

            if (Head == null || Head.Deleted)
            {
                User.SendMessage(
                    "Your head was turned over to the authorities by {0} during negotations.", _Killer.RawName);
                _Killer.SendMessage("You turned over {0}'s to the authorities during negotations and cannot accept this offer.", User.RawName);
                return;
            }

            if (Head.RootParentEntity != _Killer)
            {
                User.SendMessage(
                    "Your head does not appear to be possessed by {0} any longer.", _Killer.RawName);
                _Killer.SendMessage("You must retain the head in your pack or bank to engage in negotations for {0}'s head.", User.RawName);
                return;
            }

			if (!Banker.Withdraw(User, cType, _Offer))
			{
				User.SendMessage("You cannot afford this amount!  Either counter the offer or decline it.");
				Refresh(true);
				return;
			}

			User.SendMessage(
				"{0:#,0} {1} has been withdrawn from your bank and paid to {2}.", _Offer, cType.Name, _Killer.RawName);
			_Killer.SendMessage("{0:#,0} {1} has been paid and added to your bank box by {2}.", _Offer, cType.Name, User.RawName);

			Banker.Deposit(_Killer, cType, _Offer);

			BankBox bank = User.FindBank(Head.Expansion) ?? User.BankBox;

			if (bank != null)
			{
				bank.DropItem(Head);
			}

			if (Head is Head2)
			{
				Head2.AllHeads.Remove((Head2)Head);
			}

			foreach (var g in GetInstances<HeadNegotiateGump>(_Killer, true).Where(g => g.Head == Head))
			{
				g.Close(true);
			}

			User.SendMessage("Your severed head has been deposited into your bank box.");

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Victim -> Killer][ACCEPTED][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Killer,
					Head,
					_Offer,
					cType.Name,
					_Message));
		}

		private void OnCounteroffer()
		{
			if (_CounterOffer <= 0)
			{
				User.SendMessage("Counteroffer amount must be more than nothing!");
				Refresh(true);
				return;
			}

			if (_CounterOffer > MurderSystemController._HeadRansomCap)
			{
				_CounterOffer = MurderSystemController._HeadRansomCap;

				User.SendMessage(
					"There is a cap of {0:#,0} for the counteroffer! Please enter a lower number.",
					MurderSystemController._HeadRansomCap);
				Refresh(true);
				return;
			}

			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

			int bal = Banker.GetBalance(User, cType);

			if (bal < _CounterOffer)
			{
				_CounterOffer = bal;

				User.SendMessage("You cannot afford this amount!  Please make a new counteroffer.");
				Refresh(true);
				return;
			}

			new KillerReceiveOfferGump(_Killer, User, Head, _CounterOffer, _ReplyMessage).Send();
			User.SendMessage("Your counteroffer request has been sent to {0}!", _Killer.RawName);

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Victim -> Killer][COUNTER][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Killer,
					Head,
					_CounterOffer,
					cType.Name,
					_ReplyMessage));
		}

		private void OnDecline()
		{
			User.SendMessage("You decide not to negotiate with {0}.", _Killer.RawName);
			_Killer.SendMessage("{0} has declined your offer.", User.RawName);

			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Victim -> Killer][DECLINED][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Killer,
					Head,
					_Offer,
					cType.Name,
					_Message));
		}
	}

	public class KillerReceiveOfferGump : HeadNegotiateGump
	{
		private readonly PlayerMobile _Victim;
		private readonly int _Offer;
		private readonly string _Message;

		public KillerReceiveOfferGump(PlayerMobile killer, PlayerMobile victim, Item head, int amount, string message)
			: base(killer, head)
		{
			_Victim = victim;
			_Offer = amount;
			_Message = message;

			CanDispose = false;
			CanClose = false;
			CanResize = false;
			CanMove = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"open", () => AddButton(21, 0, 30, 30, b => new KillerResponseGump(User, _Victim, Head, _Offer, _Message).Send()));
			layout.Add("offer", () => AddLabel(0, 32, 32, "Offer from " + _Victim.RawName));
		}
	}

	public class KillerResponseGump : HeadNegotiateGump
	{
		private readonly PlayerMobile _Victim;
		private readonly int _Offer;
		private readonly string _Message;

		private int _CounterOffer;
		private string _ReplyMessage;

		private readonly BitArray _Options = new BitArray(new[] {true, false, false});

		public KillerResponseGump(PlayerMobile killer, PlayerMobile victim, Item head, int offer, string message)
			: base(killer, head)
		{
			_Victim = victim;
			_Offer = offer;
			_Message = message;

			_CounterOffer = 0;
			_ReplyMessage = String.Empty;

			CanClose = false;
			CanDispose = false;
			CanResize = false;
			CanMove = true;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add("bg", () => AddBackground(0, 0, 510, 470, 5170));

			layout.Add(
				"title",
				() =>
				AddHtml(
					20, 5, 470, 40, String.Format("Offer from {0}".WrapUOHtmlColor(Color.OrangeRed), _Victim.RawName), false, false));

			layout.Add(
				"message",
				() =>
				{
					AddLabel(36, 36, 90, @"Message:");
					AddHtml(36, 68, 200, 150, _Message ?? String.Empty, true, true);
				});

			layout.Add(
				"response",
				() =>
				{
					AddLabel(276, 36, 90, @"Write Response:");
					AddBackground(276, 66, 200, 225, 3500);
					AddTextEntryLimited(
						300, 90, 150, 175, 2622, _ReplyMessage ?? String.Empty, 1000, (e, t) => _ReplyMessage = t ?? String.Empty);
				});

			layout.Add(
				"offer",
				() =>
				{
					AddLabel(36, 230, 32, @"Ransom");
					AddImageTiled(36, 230, 200, 33, 93); // Spacer
					AddImage(36, 270, 53); // Amnt.
					AddLabel(80, 265, 2622, _Offer.ToString("#,0"));
				});

			layout.Add(
				"menu/accept",
				() =>
				{
					AddRadio(36, 305, 2152, 2154, _Options.Get(0), (r, s) => _Options[0] = s);
					AddLabel(70, 310, 2622, @"Accept");
				});

			layout.Add(
				"menu/counteroffer",
				() =>
				{
					AddRadio(192, 305, 2152, 2154, _Options.Get(1), (r, s) => _Options[1] = s);
					AddLabel(226, 310, 2622, @"Counteroffer");
				});

			layout.Add(
				"menu/decline",
				() =>
				{
					AddRadio(390, 305, 2152, 2154, _Options.Get(2), (r, s) => _Options[2] = s);
					AddLabel(424, 310, 2622, @"Decline");
				});

			layout.Add(
				"counteroffer",
				() =>
				{
					AddLabel(36, 360, 32, "Counteroffer Amount:");
					AddTextEntryLimited(
						190,
						360,
						280,
						20,
						2622,
						_CounterOffer.ToString(CultureInfo.InvariantCulture),
						9,
						(e, t) =>
						{
							if (!Int32.TryParse(t, out _CounterOffer) || _CounterOffer < 0)
							{
								_CounterOffer = 0;
							}
						});
					AddImageTiled(36, 360, 440, 33, 93); // Spacer
				});

			layout.Add(
				"ok",
				() => AddButton(
					224,
					406,
					247,
					248,
					b =>
					{
						if (_Options.Get(0))
						{
							OnAccept();
						}
						else if (_Options.Get(1))
						{
							OnCounteroffer();
						}
						else if (_Options.Get(2))
						{
							OnDecline();
						}
						else
						{
							Refresh(true);
						}
					}));
		}

		private void OnAccept()
		{
			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

            if (Head == null || Head.Deleted)
            {
                _Victim.SendMessage(
                    "Your head was turned over to the authorities by {0} during negotations.", User.RawName);
                User.SendMessage("You turned over {0}'s to the authorities during negotations and cannot accept this offer.", User.RawName);
                return;
            }

            if (Head.RootParentEntity != User)
            {
                _Victim.SendMessage(
                    "Your head does not appear to be possessed by {0} any longer.", User.RawName);
                User.SendMessage("You must retain the head in your pack or bank to engage in negotations for {0}'s head.", User.RawName);
                return;
            }

			if (!Banker.Withdraw(_Victim, cType, _Offer))
			{
				User.SendMessage("{0} could not afford this price.  Please make another offer later.", _Victim.RawName);

				_Victim.SendMessage(
					"You could not afford to pay the counteroffer you made.  The amount of {0} in your bank has declined since you made the counteroffer.",
					cType.Name);

				LoggingCustom.Log(
					"MurdererNegotiate.txt",
					String.Format(
						"[Killer -> Victim][FAILED][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
						ServerTime.ShortDateTimeNow,
						User,
						_Victim,
						Head,
						_Offer,
						cType.Name,
						_Message));
				return;
			}
			
			_Victim.SendMessage(
				"{0:#,0} {1} has been withdrawn from your bank and paid to {2}.", _Offer, cType.Name, User.RawName);
			User.SendMessage("{0:#,0} {1} has been paid and added to your bank box by {2}.", _Offer, cType.Name, _Victim.RawName);

			Banker.Deposit(User, cType, _Offer);

			BankBox bank = _Victim.FindBank(Head.Expansion) ?? _Victim.BankBox;

			if (bank != null)
			{
				bank.DropItem(Head);
			}

			if (Head is Head2)
			{
				Head2.AllHeads.Remove((Head2)Head);
			}

			foreach (var g in GetInstances<HeadNegotiateGump>(_Victim, true).Where(g => g.Head == Head))
			{
				g.Close(true);
			}

			_Victim.SendMessage("Your severed head has been deposited into your bank box.");

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Killer -> Victim][ACCEPTED][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Victim,
					Head,
					_Offer,
					cType.Name,
					_Message));
		}

		private void OnCounteroffer()
		{
			if (_CounterOffer <= 0)
			{
				User.SendMessage("Counteroffer amount must be more than nothing!");
				Refresh(true);
				return;
			}

			if (_CounterOffer > MurderSystemController._HeadRansomCap)
			{
				_CounterOffer = MurderSystemController._HeadRansomCap;

				User.SendMessage(
					"There is a cap of {0:#,0} for the counteroffer! Please enter a lower number.",
					MurderSystemController._HeadRansomCap);
				Refresh(true);
				return;
			}

			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

			// We don't REALLY want to tell the killer that the victim has no gold
			// It should be handled by the offer gump where they are forced to accept/decline/counter.
			/*
			int bal = Banker.GetBalance(_Victim, cType);

			if (bal < _CounterOffer)
			{
				_CounterOffer = bal;

				User.SendMessage("They cannot afford this amount!  Please make a new counteroffer.");
				Refresh(true);
				return;
			}
			*/

			new VictimReceiveOfferGump(_Victim, User, Head, _CounterOffer, _ReplyMessage).Send();
			User.SendMessage("Your counteroffer demand has been sent to {0}!", _Victim.RawName);

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Killer -> Victim][COUNTER][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Victim,
					Head,
					_CounterOffer,
					cType.Name,
					_ReplyMessage));
		}

		private void OnDecline()
		{
			User.SendMessage("You decide not to negotiate with {0}.", _Victim.RawName);
			_Victim.SendMessage("{0} has declined your offer.", User.RawName);

			Type cType = Head.Expansion == Expansion.T2A ? typeof(Silver) : typeof(Gold);

			LoggingCustom.Log(
				"MurdererNegotiate.txt",
				String.Format(
					"[Killer -> Victim][DECLINED][{0}]:\t[{1} -> {2}]\t[{3}]\t[{4:#,0} {5}]\t\"{6}\"",
					ServerTime.ShortDateTimeNow,
					User,
					_Victim,
					Head,
					_Offer,
					cType.Name,
					_Message));
		}
	}
}