#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using VitaNex.Targets;

#endregion

namespace Server.Poker
{
	public class PokerDealer : Mobile
	{
		public static void Initialize()
		{
			CommandSystem.Register("AddPokerSeat", AccessLevel.Administrator, AddPokerSeat_OnCommand);
			CommandSystem.Register("PokerKick", AccessLevel.Seer, PokerKick_OnCommand);

			EventSink.Disconnected += EventSink_Disconnected;
		}

		private double _MRake;
		private int _MMaxPlayers;
		private bool _MActive;

		public static int Jackpot { get; set; }

		[CommandProperty(AccessLevel.Seer)]
		public bool TournamentMode { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool ClearSeats
		{
			get { return false; }
			set
			{
				if (value)
				{
					Seats.Clear();
				}
			}
		}

        [CommandProperty(AccessLevel.Administrator)]
        public string TableName { get; set; }

        [CommandProperty(AccessLevel.Administrator)]
        public bool IsDonation { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int RakeMax { get; set; }

		[CommandProperty(AccessLevel.Seer)]
		public int MinBuyIn { get; set; }

		[CommandProperty(AccessLevel.Seer)]
		public int MaxBuyIn { get; set; }

		[CommandProperty(AccessLevel.Seer)]
		public int SmallBlind { get; set; }

		[CommandProperty(AccessLevel.Seer)]
		public int BigBlind { get; set; }

        [CommandProperty(AccessLevel.Seer)]
        public int TotalRake { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public Point3D ExitLocation { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public Map ExitMap { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public double Rake
		{
			get { return _MRake; }
			set
			{
				if (value > 1)
				{
					_MRake = 1;
				}
				else if (value < 0)
				{
					_MRake = 0;
				}
				else
				{
					_MRake = value;
				}
			}
		}

		[CommandProperty(AccessLevel.Seer)]
		public int MaxPlayers
		{
			get { return _MMaxPlayers; }
			set
			{
				if (value > 22)
				{
					_MMaxPlayers = 22;
				}
				else if (value < 0)
				{
					_MMaxPlayers = 0;
				}
				else
				{
					_MMaxPlayers = value;
				}
			}
		}

		[CommandProperty(AccessLevel.Seer)]
		public bool Active
		{
			get { return _MActive; }
			set
			{
				var toRemove = new List<PokerPlayer>();

				if (!value)
				{
					toRemove.AddRange(Game.Players.Players.Where(player => player.Mobile != null));
				}

				foreach (PokerPlayer p in toRemove)
				{
					p.Mobile.SendMessage(
						0x22,
						"The poker dealer has been set to inactive by a game master, and you are now being removed from the poker game and being refunded the money that you currently have.");
					Game.RemovePlayer(p);
				}

				_MActive = value;
			}
		}

		public PokerGame Game { get; set; }
		public List<Point3D> Seats { get; set; }

		[Constructable]
		public PokerDealer()
			: this(10)
		{ }

		[Constructable]
		public PokerDealer(int maxPlayers)
		{
			Blessed = true;
			Frozen = true;
			InitStats(100, 100, 100);

			Title = "the poker dealer";
			Hue = Utility.RandomSkinHue();
			NameHue = 0x35;
			Female = Utility.RandomBool();

			if (Female)
			{
				Body = 0x191;
				Name = NameList.RandomName("female");
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");
			}

			Dress();

			MaxPlayers = maxPlayers;
			Seats = new List<Point3D>();
			_MRake = 0.10; //10% rake default
			RakeMax = 5000; //5k maximum rake default
			Game = new PokerGame(this);
		}

		private void Dress()
		{
			AddItem(new FancyShirt(0));

			Item pants = new LongPants();
			pants.Hue = 1;
			AddItem(pants);

			Item shoes = new Shoes();
			shoes.Hue = 1;
			AddItem(shoes);

			Item sash = new BodySash();
			sash.Hue = 1;
			AddItem(sash);

			Utility.AssignRandomHair(this);
		}

		public static JackpotInfo JackpotWinners { get; set; }

		public override void OnDoubleClick(Mobile from)
		{
		    var p = from as PlayerMobile;

			if (!_MActive)
			{
				from.SendMessage(0x9A, "This table is inactive");
			}
			else if (!InRange(from.Location, 8))
			{
				from.PrivateOverheadMessage(MessageType.Regular, 0x22, true, "I am too far away to do that", from.NetState);
			}
			else if (MinBuyIn == 0 || MaxBuyIn == 0)
			{
				from.SendMessage(0x9A, "This table is inactive");
			}
			else if (MinBuyIn > MaxBuyIn)
			{
				from.SendMessage(0x9A, "This table is inactive");
			}
			else if (p != null && p.PokerGame != null && Game != p.PokerGame)
			{
				from.SendMessage(0x9A, "You cannot join two poker games at the same time.");
			}
            else if (Seats.Count < _MMaxPlayers)
            {
                from.SendMessage(0x9A, "This table is inactive");
            }
			else if (Game.GetIndexFor(from) != -1)
			{
                from.CloseGump(typeof(PokerRebuy));
                from.SendGump(new PokerRebuy(from, Game));
			}
			else if (Game.Players.Count >= _MMaxPlayers)
			{
				from.SendMessage(0x22, "This table is full");
				base.OnDoubleClick(from);
			}
			else if (Game.Players.Count < _MMaxPlayers && from.Alive)
			{
			    if (Game.Players.Players.Any(player => player.Mobile.NetState != null && @from.NetState != null &&
			                                           player.Mobile.NetState.Address.Equals(@from.NetState.Address)))
			    {
                    if (from.AccessLevel <= AccessLevel.Player)
			            return;
			    }
				from.CloseGump(typeof(PokerJoinGump));
				from.SendGump(new PokerJoinGump(from, Game));
			}
		}

		public override void OnDelete()
		{
			var toRemove = Game.Players.Players.Where(player => player.Mobile != null).ToList();

			foreach (PokerPlayer p in toRemove)
			{
				p.Mobile.SendMessage(
					0x22,
					"The poker dealer has been deleted, and you are now being removed from the poker game and being refunded the money that you currently have.");
				Game.RemovePlayer(p);
			}

			base.OnDelete();
		}

		public static void PokerKick_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if (from == null)
			{
				return;
			}

			var list = from.GetMobilesInRange(0);

			foreach (var pm in list.OfType<PlayerMobile>().Where(pm=>pm.PokerGame!=null))
			{
				PokerGame game = pm.PokerGame;

				PokerPlayer player = game.GetPlayer(pm);

				if (player == null)
				{
					continue;
				}

				game.RemovePlayer(player);
				from.SendMessage("They have been removed from the poker table");

				list.Free();
				return;
			}

			list.Free();

			from.SendMessage("No one found to kick from a poker table. Make sure you are standing on top of them.");
		}

		private static void EventSink_Disconnected(DisconnectedEventArgs e)
		{
			Mobile from = e.Mobile;

			if (from == null)
			{
				return;
			}

			if (!(from is PlayerMobile))
			{
				return;
			}

			var pm = (PlayerMobile)from;

			PokerGame game = pm.PokerGame;

			if (game == null)
			{
				return;
			}

			PokerPlayer player = game.GetPlayer(from);

			if (player != null)
			{
			    Timer.DelayCall(TimeSpan.FromMinutes(1), () =>
			    {
			        if (!player.IsOnline())
			        {
			            player.RequestLeave = true;
			        }
			    });
			}
		}

		public static void AddPokerSeat_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if (from == null)
			{
				return;
			}

		    if (!e.Arguments.Any())
		    {
		        GetDealerPSeat(from);
		    }
		    else
		    {

		        string args = e.ArgString.ToLower();
		        string[] argLines = args.Split(' ');
		        int x, y, z;

		        if (!Int32.TryParse(argLines[0], out x) || !Int32.TryParse(argLines[1], out y) ||
		            !Int32.TryParse(argLines[2], out z))
		        {
		            from.SendMessage(0x22, "Usage: [AddPokerSeat <x> <y> <z>");
		            return;
		        }

		        bool success = false;

		        var list = from.GetMobilesInRange(0);

		        foreach (var m in list.OfType<PokerDealer>())
		        {
		            var seat = new Point3D(x, y, z);

		            if (m.AddPokerSeat(from, seat) != -1)
		            {
		                from.SendMessage(0x22, "A new seat was successfully created.");
		                success = true;
		                break;
		            }

		            from.SendMessage(
		                0x22,
		                "There is no more room at that table for another seat. Try increasing the value of MaxPlayers first.");
		            success = true;
		            break;
		        }

		        list.Free();

		        if (!success)
		        {
		            from.SendMessage(0x22, "No poker dealers were found in range. (Try standing on top of the dealer)");
		        }
		    }
		}

        public static void GetDealerPSeat(Mobile user)
        {
            if (user != null && !user.Deleted)
            {
                user.Target = new MobileSelectTarget<Mobile>(FoundDealer, m => { });
            }
        }

        public static void FoundDealer(Mobile User, Mobile target)
	    {
            if (target is PokerDealer)
            {
                var dealer = target as PokerDealer;

                if (dealer.AddPokerSeat(User, User.Location) != -1)
                {
                    User.SendMessage(0x22, "A new seat was successfully created.");
                }
            }
	    }

		public int AddPokerSeat(Mobile from, Point3D seat)
		{
			if (Seats.Count >= _MMaxPlayers)
			{
				return -1;
			}

			Seats.Add(seat);
			return 0;
		}

		public bool SeatTaken(Point3D seat)
		{
			for (int i = 0; i < Game.Players.Count; ++i)
			{
				if (Game.Players[i].Seat == seat)
				{
					return true;
				}
			}

			return false;
		}

		public int RakeGold(int gold)
		{
			double amount = gold * _MRake;
			return (int)(amount > RakeMax ? RakeMax : amount);
		}

		public PokerDealer(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(2); //version

            writer.Write(TotalRake);
            writer.Write(TableName);
            writer.Write(IsDonation);
			writer.Write(_MActive);
			writer.Write(SmallBlind);
			writer.Write(BigBlind);
			writer.Write(MinBuyIn);
			writer.Write(MaxBuyIn);
			writer.Write(ExitLocation);
			writer.Write(ExitMap);
			writer.Write(_MRake);
			writer.Write(RakeMax);
			writer.Write(_MMaxPlayers);

			writer.Write(Seats.Count);

			foreach (Point3D s in Seats)
			{
				writer.Write(s);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
                case 2:
			    {
			        TotalRake = reader.ReadInt();
			    }
                    goto case 1;
                case 1:
			    {
			        TableName = reader.ReadString();
			        IsDonation = reader.ReadBool();
			    }
			        goto case 0;
				case 0:
					{
						_MActive = reader.ReadBool();
						SmallBlind = reader.ReadInt();
						BigBlind = reader.ReadInt();
						MinBuyIn = reader.ReadInt();
						MaxBuyIn = reader.ReadInt();
						ExitLocation = reader.ReadPoint3D();
						ExitMap = reader.ReadMap();
						_MRake = reader.ReadDouble();
						RakeMax = reader.ReadInt();
						_MMaxPlayers = reader.ReadInt();

						int count = reader.ReadInt();
						Seats = new List<Point3D>();

						for (int i = 0; i < count; ++i)
						{
							Seats.Add(reader.ReadPoint3D());
						}
					}
					break;
			}

			Game = new PokerGame(this);
		}

		public class JackpotInfo
		{
			public List<PokerPlayer> Winners { get; private set; }
			public ResultEntry Hand { get; private set; }
			public DateTime Date { get; private set; }
			public Type TypeOfCurrency { get; private set; }

			public JackpotInfo(List<PokerPlayer> winners, ResultEntry hand, DateTime date, Type currency)
			{
				Winners = winners;
				Hand = hand;
				Date = date;
				TypeOfCurrency = currency;
			}
		}
	}
}