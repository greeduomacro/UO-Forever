#region References
using System;
using System.Linq;

using Server.Accounting;
using Server.Mobiles;

using VitaNex;
#endregion

namespace Server.Engines.Conquests
{
	public sealed class ConquestState : PropertyObject, IEquatable<ConquestState>
	{
		private bool _Initializing = true;

		private int _Progress;

		[CommandProperty(Conquests.Access, true)]
		public ConquestSerial UID { get; private set; }

		[CommandProperty(Conquests.Access, true)]
		public Conquest Conquest { get; private set; }

		[CommandProperty(Conquests.Access, true)]
		public PlayerMobile Owner { get; private set; }

        [CommandProperty(Conquests.Access, true)]
        public PlayerMobile User { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ConquestExists { get { return Conquest != null && !Conquest.Deleted; } }

		[CommandProperty(Conquests.Access)]
		public string Name
		{
			//
			get { return ConquestExists && !String.IsNullOrWhiteSpace(Conquest.Name) ? Conquest.Name : String.Empty; }
		}

		[CommandProperty(Conquests.Access)]
		public string Description
		{
			//
			get { return ConquestExists && !String.IsNullOrWhiteSpace(Conquest.Desc) ? Conquest.Desc : String.Empty; }
		}

		[CommandProperty(Conquests.Access, true)]
		public int Tier { get; private set; }

		[CommandProperty(Conquests.Access)]
		public int TierMax
		{
			//
			get { return ConquestExists ? Math.Max(1, Conquest.TierMax) : -1; }
		}

		[CommandProperty(Conquests.Access)]
		public bool TiersComplete
		{
			//
			get { return !_Initializing && Tier >= TierMax; }
			set { Tier = value ? TierMax : 0; }
		}

		[CommandProperty(Conquests.Access)]
		public double ProgressFactor
		{
			//
			get { return ConquestExists ? Conquest.ProgressFactor : 0.0; }
		}

		[CommandProperty(Conquests.Access)]
		public int Progress
		{
			get { return _Progress; }
			set
			{
				if (_Initializing)
				{
					_Progress = value;
					return;
				}

				int diff = value - _Progress;

				if (diff > 0)
				{
					UpdateProgress(diff);
				}
				else
				{
					_Progress = value;
				}

				if (Conquest.AccountWide)
				{
					AccountPropagate();
				}
			}
		}

		[CommandProperty(Conquests.Access)]
		public int ProgressMax
		{
			get
			{
				if (!ConquestExists)
				{
					return -1;
				}

				var factored = (int)Math.Ceiling(Tier * (Conquest.ProgressMax * ProgressFactor));

				return Math.Max(1, Conquest.ProgressMax + factored);
			}
		}

		[CommandProperty(Conquests.Access)]
		public bool ProgressComplete
		{
			//
			get { return !_Initializing && Progress >= ProgressMax; }
			set { Progress = value ? ProgressMax : 0; }
		}

		[CommandProperty(Conquests.Access)]
		public bool Completed
		{
			//
			get { return !_Initializing && TiersComplete; }
		}

		[CommandProperty(Conquests.Access, true)]
		public DateTime CompletedDate { get; private set; }

		[CommandProperty(Conquests.Access, true)]
		public bool WorldFirst { get; private set; }

		[CommandProperty(Conquests.Access, true)]
		public DateTime LastProgress { get; private set; }
		
		public ConquestState(PlayerMobile owner, Conquest conquest)
		{
			UID = new ConquestSerial();

			Conquest = conquest;
			Owner = owner;

			EnsureDefaults();
		}

		public ConquestState(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
			_Initializing = true;
			
			Tier = 0;
			_Progress = 0;

			if (WorldFirst)
			{
				ConquestState s = Conquests.FindReplacementWorldFirst(Conquest);

				WorldFirst = false;

				if (s != null)
				{
					s.WorldFirst = true;
					s.Conquest.OnWorldFirst(s);
				}
			}
			
			LastProgress = DateTime.MinValue;
			CompletedDate = DateTime.MinValue;

			_Initializing = false;
		}

		public override void Clear()
		{
			EnsureDefaults();
		}

		public override void Reset()
		{
			EnsureDefaults();
		}

		public void UpdateProgress(int offset)
		{
			if (offset <= 0 || !ConquestExists)
			{
				return;
			}

			if (Completed)
			{
				_Progress = ProgressMax;
				return;
			}

			DateTime now = DateTime.Now;

			if (Conquest.TimeoutReset > TimeSpan.Zero && LastProgress + Conquest.TimeoutReset < now)
			{
				_Progress = 0;
				return;
			}

			LastProgress = now;

			var args = new ConquestProgressContainer(this, offset);

			Conquests.HandleProgress(args);

			offset = args.Offset;
			
			_Progress += offset;

			Conquest.OnProgress(this, offset);

			while (ProgressComplete && !Completed)
			{
				int pMax = ProgressMax;

				++Tier;
				
				Conquest.OnTierComplete(this);

				Conquests.HandleTierCompleted(new ConquestTierCompletedContainer(this, Tier));

				if (TiersComplete)
				{
					_Progress = pMax;
					break;
				}

				_Progress -= pMax;
			}

			if (!Completed)
			{
				return;
			}

		    _Progress = ProgressMax;

			CompletedDate = DateTime.Now;

			ConquestState wf = Conquests.FindWorldFirst(Conquest);

            if (wf == null && (Owner.AccessLevel == AccessLevel.Player || Owner.AccessLevel > AccessLevel.Player && Conquests.CMOptions.StaffWorldFirsts))
			{
				WorldFirst = true;

				Conquest.OnWorldFirst(this);
			}

            Conquests.HandleCompleted(new ConquestCompletedContainer(this));
		}

	    public void CopyState(ConquestState s)
	    {
	        Tier = s.Tier;
	        TiersComplete = s.TiersComplete;
	        _Progress = s._Progress;
	        CompletedDate = s.CompletedDate;
	        LastProgress = s.LastProgress;
	        WorldFirst = s.WorldFirst;
	    }

		public void AccountPropagate()
		{
			var acc = Owner.Account as Account;

			if (acc == null)
			{
				return;
			}

			foreach (var p in
				acc.Mobiles.Where(m => m != null && !m.Deleted && m != Owner)
				   .OfType<PlayerMobile>()
				   .Select(Conquests.EnsureProfile)
				   .Where(p => p != null && Conquests.Validate(Conquest, p.Owner)))
			{
				ConquestState s = p.EnsureState(Conquest);

				if (s == null || s == this)
				{
					continue;
				}

				int d = _Progress - s._Progress;

                if (_Progress > s._Progress || CompletedDate < s.CompletedDate || WorldFirst)
                {
                    s.CopyState(this);
				}
			}
		}

		public override string ToString()
		{
			return Name ?? String.Empty;
		}

		public override bool Equals(object obj)
		{
			return obj is ConquestState ? Equals((ConquestState)obj) : base.Equals(obj);
		}

		public bool Equals(ConquestState other)
		{
			return !ReferenceEquals(other, null) && UID.Equals(other.UID);
		}

		public override int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			UID.Serialize(writer);

			Conquests.WriteConquest(writer, Conquest);

			writer.Write(Owner);

			switch (version)
			{
				case 0:
					{
						writer.Write(LastProgress);
						writer.Write(CompletedDate);

						writer.Write(WorldFirst);

						writer.Write(Tier);
						writer.Write(_Progress);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			UID = new ConquestSerial(reader);

			Conquest = Conquests.ReadConquest(reader);

			Owner = reader.ReadMobile<PlayerMobile>();

			_Initializing = true;

			switch (version)
			{
				case 0:
					{
						LastProgress = reader.ReadDateTime();
						CompletedDate = reader.ReadDateTime();

						WorldFirst = reader.ReadBool();

						Tier = reader.ReadInt();
						_Progress = reader.ReadInt();
					}
					break;
			}

			_Initializing = false;
		}

		public static bool operator ==(ConquestState left, ConquestState right)
		{
			if (ReferenceEquals(left, null))
			{
				return ReferenceEquals(right, null);
			}

			return left.Equals(right);
		}

		public static bool operator !=(ConquestState left, ConquestState right)
		{
			if (ReferenceEquals(left, null))
			{
				return !ReferenceEquals(right, null);
			}

			return !left.Equals(right);
		}
	}
}