#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Server.Accounting;
using Server.Engines.CustomTitles;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.SkillHandlers;

using VitaNex;
using VitaNex.Notify;
#endregion

namespace Server.Engines.Conquests
{
	public abstract class Conquest : PropertyObject, IEquatable<Conquest>
	{
		public virtual TimeSpan DefTimeoutReset { get { return TimeSpan.Zero; } }

		public virtual bool DefSecret { get { return false; } }
		public virtual bool DefYoung { get { return false; } }
		public virtual bool DefScaleRewards { get { return false; } }

		public virtual int DefPoints { get { return 10; } }

		public virtual string DefName { get { return GetType().Name.SpaceWords(); } }
		public virtual string DefDesc { get { return String.Empty; } }

		public virtual string DefCategory { get { return "Misc"; } }

		public virtual int DefItemID { get { return 3823; } }
		public virtual int DefHue { get { return 0; } }

		public virtual KnownColor DefColor { get { return KnownColor.White; } }

		public virtual int DefTierMax { get { return 1; } }
		public virtual int DefProgressMax { get { return 1; } }
		public virtual double DefProgressFactor { get { return 0.0; } }

		public virtual int DefSoundOnProgress { get { return 0; } }
		public virtual int DefSoundOnComplete { get { return 743; } }
		public virtual int DefSoundOnWorldFirst { get { return 744; } }

		public virtual bool DefAccountBound { get { return false; } }
		public virtual bool DefAccountWide { get { return false; } }

        public virtual bool DefRewardTierComplete { get { return false; } }

		private bool _Enabled;

		public List<Type> Rewards { get; private set; }

		[CommandProperty(Conquests.Access, true)]
		public ConquestSerial UID { get; private set; }

		[CommandProperty(Conquests.Access, true)]
		public bool Deleted { get; private set; }

		[CommandProperty(Conquests.Access)]
		public bool Enabled
		{
			get { return _Enabled; }
			set
			{
				if (_Enabled && !value)
				{
					_Enabled = false;

					OnDisabled();
				}
				else if (!_Enabled && value)
				{
					_Enabled = true;

					OnEnabled();
				}
			}
		}

		[CommandProperty(Conquests.Access)]
		public TimeSpan TimeoutReset { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool AccountBound { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool AccountWide { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool Secret { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool Young { get; set; }

		[CommandProperty(Conquests.Access)]
		public bool ScaleRewards { get; set; }

        [CommandProperty(Conquests.Access)]
        public bool RewardTierComplete { get; set; }

		[CommandProperty(Conquests.Access)]
		public int Points { get; set; }

		[CommandProperty(Conquests.Access)]
		public string Name { get; set; }

		[CommandProperty(Conquests.Access)]
		public string Desc { get; set; }

		[CommandProperty(Conquests.Access)]
		public string Category { get; set; }

		[CommandProperty(Conquests.Access)]
		public int ItemID { get; set; }

		[CommandProperty(Conquests.Access)]
		public Point2D ItemIDOffset { get; set; }

		[CommandProperty(Conquests.Access)]
		public int Hue { get; set; }

		[CommandProperty(Conquests.Access)]
		public KnownColor Color { get; set; }

		[CommandProperty(Conquests.Access)]
		public int TierMax { get; set; }

		[CommandProperty(Conquests.Access)]
		public int ProgressMax { get; set; }

		[CommandProperty(Conquests.Access)]
		public double ProgressFactor { get; set; }

		[CommandProperty(Conquests.Access)]
		public int SoundOnProgress { get; set; }

		[CommandProperty(Conquests.Access)]
		public int SoundOnComplete { get; set; }

		[CommandProperty(Conquests.Access)]
		public int SoundOnWorldFirst { get; set; }

		[CommandProperty(Conquests.Access)]
		public string TagKey { get { return String.Format("[{0}][{1}]", GetType().Name, UID); } }

		public Conquest()
		{
			Rewards = new List<Type>();

			UID = new ConquestSerial();

			EnsureDefaults();
		}

		public Conquest(GenericReader reader)
			: base(reader)
		{ }

		public virtual void EnsureDefaults()
		{
			_Enabled = false;

			TimeoutReset = DefTimeoutReset;

			AccountBound = DefAccountBound;
			AccountWide = DefAccountWide;

			Secret = DefSecret;
			Young = DefYoung;
			ScaleRewards = DefScaleRewards;

			Points = DefPoints;

			Name = DefName;
			Desc = DefDesc;

			Category = DefCategory;

			ItemID = DefItemID;
			Hue = DefHue;
			Color = DefColor;

			TierMax = DefTierMax;
			ProgressMax = DefProgressMax;
			ProgressFactor = DefProgressFactor;

			SoundOnProgress = DefSoundOnProgress;
			SoundOnComplete = DefSoundOnComplete;
			SoundOnWorldFirst = DefSoundOnWorldFirst;

		    RewardTierComplete = DefRewardTierComplete;
		}

		public override void Clear()
		{
			EnsureDefaults();
		}

		public override void Reset()
		{
			EnsureDefaults();
		}

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			Deleted = true;

			Conquests.ConquestRegistry.Remove(UID);

			foreach (ConquestProfile p in Conquests.Profiles.Values)
			{
				p.RemoveState(this);
			}
		}

		protected virtual void OnEnabled()
		{
			foreach (
				ConquestProfile p in
					Conquests.Profiles.Values.AsParallel().Where(p => p != null && Conquests.Validate(this, p.Owner)))
			{
			    p.EnsureState(this);

				foreach (
					var s in
						p.Where(
							s =>
							s.Completed && this is ConquestCompletedConquest &&
							Insensitive.Contains(s.Conquest.Category, (((ConquestCompletedConquest)this).CategoryReq))))
				{
                    Conquests.CheckProgress<ConquestCompletedConquest>(p.Owner, new ConquestCompletedContainer(s));
				}
				//until lee implements his changes, handle it like I originally was doing
				//Conquests.InvalidateRecursiveConquests(p.Owner);
			}
		}

		protected virtual void OnDisabled()
		{
			foreach (
				ConquestProfile p in
					Conquests.Profiles.Values.AsParallel().Where(p => p != null && !Conquests.Validate(this, p.Owner)))
			{
				p.RemoveState(this);
			}
		}

		public abstract int GetProgress(ConquestState state, object args);

		public virtual void OnProgress(ConquestState s, int offset)
		{
			if (!Enabled || Deleted || s == null || s.Conquest != this || s.User == null || offset <= 0)
			{
				return;
			}

			if (!Secret && SoundOnProgress > 0)
			{
                s.User.PlaySound(SoundOnProgress);
			}

			if (Conquests.CMOptions.ModuleDebug)
			{
                Conquests.CMOptions.ToConsole("{0} progress: {1} ({2}{3})", Name, s.User, offset >= 0 ? "+" : String.Empty, offset);
			}
		}

		public virtual void OnTierComplete(ConquestState s)
		{
            if (!Enabled || Deleted || s == null || s.Conquest != this || s.User == null)
			{
				return;
			}

            ConquestProfile p = Conquests.EnsureProfile(s.User);

			if (p != null)
			{
				p.Credit += Points;
			}

            if (s.TiersComplete || RewardTierComplete)
		    {
		        GiveRewards(s);
		    }

		    if (SoundOnComplete > 0)
			{
				s.User.PlaySound(SoundOnComplete);
			}

		    if (!CentralGump.CentralGump.EnsureProfile(s.User).IgnoreConquests)
		    {
		        ConquestCompletedGump g = ConquestCompletedGump.Acquire(s);

		        if (g != null)
		        {
		            g.Send();
		        }
		    }

		    if (Conquests.CMOptions.ModuleDebug)
			{
				Conquests.CMOptions.ToConsole("{0} tier complete: {1} ({2})", Name, s.User, s.Tier);
			}
		}

		public virtual void OnWorldFirst(ConquestState s)
		{
            if (!Enabled || Deleted || s == null || s.Conquest != this || s.User == null)
			{
				return;
			}

			if (SoundOnWorldFirst > 0)
			{
				s.User.PlaySound(SoundOnWorldFirst);
			}

			var msg = new StringBuilder();

			msg.AppendLine("World First Conquest!".WrapUOHtmlTag("big"));
			msg.AppendLine("\"{0}\"", Name.WrapUOHtmlColor(Color));
            msg.AppendLine("Unlocked by {0}", s.User.RawName.WrapUOHtmlColor(s.User.GetNotorietyColor()));

			string text = msg.ToString().WrapUOHtmlColor(KnownColor.White);

		    if (!Conquests.CMOptions.SupressWorldFirsts)
		    {
		        Notify.Broadcast<ConquestNotifyGump>(text, true, 1.0, 10.0);
		    }

		    if (Conquests.CMOptions.ModuleDebug)
			{
                Conquests.CMOptions.ToConsole("{0} world first: {1}", Name, s.User, s.Tier);
			}
		}

		public void InvalidateRewardInfo()
		{
			Rewards.RemoveAll(t => t == null || !t.IsConstructable());

			Rewards.ForEach(
				type =>
				{
					ConquestRewardInfo info = ConquestRewardInfo.EnsureInfo(this, type);

					if (info == null)
					{
						Rewards.Remove(type);
					}
				});
		}

		public void AddReward(Type t)
		{
			if (t == null || Rewards.Contains(t))
			{
				return;
			}

			Rewards.Add(t);
			InvalidateRewardInfo();
		}

		public bool RemoveReward(Type t)
		{
			if (t != null && Rewards.Remove(t))
			{
				InvalidateRewardInfo();
				return true;
			}

			return false;
		}

		public virtual void GiveRewards(ConquestState s)
		{
            if (!Enabled || Deleted || s == null || s.Conquest != this || s.User == null)
			{
				return;
			}
			
			if (AccountBound)
			{
                var account = s.User.Account as Account;

				if (account == null)
				{
					return;
				}

				string tag = account.GetTag(TagKey);

				if (!String.IsNullOrWhiteSpace(tag))
				{
					return;
				}

				// Look for all players in the account that aren't the owner of the current conquest state.
				for (int i = 0; i < account.Length; i++)
				{
					var pm = account[i] as PlayerMobile;

                    if (pm == null || pm == s.User)
					{
						continue;
					}

					ConquestProfile prof = Conquests.EnsureProfile(pm);

					ConquestState state;

					if (!prof.TryGetState(this, out state) || state == null || state == s || !state.Completed)
					{
						continue;
					}

					account.SetTag(TagKey, state.CompletedDate.ToSimpleString("t@h:m@ d-m-y"));
					return; // get outta here, no rewards!
				}

				account.SetTag(TagKey, s.CompletedDate.ToSimpleString("t@h:m@ d-m-y"));
			}
			else
			{
                var account = s.User.Account as Account;

				if (account != null)
				{
					account.RemoveTag(TagKey); // Remove the tag, just in case it was bound previously but isn't now.
				}
			}

			InvalidateRewardInfo();

			if (Rewards.Count == 0)
			{
				return;
			}

			var rewards = new List<IEntity>();
			var attachments = new List<IXmlAttachment>();

			Rewards.ForEach(
				type =>
				{
					if (type == null)
					{
						return;
					}

					int count = ScaleRewards ? s.Tier : 1;

					while (--count >= 0)
					{
						var reward = type.CreateInstanceSafe<object>();

						if (reward is Item)
						{
							var rewardItem = (Item)reward;

							if (!OnBeforeGiveReward(s, rewardItem))
							{
								rewardItem.Delete();
								return;
							}

							OnGiveReward(s, rewardItem);
							
							if (!rewardItem.Deleted)
							{
								rewards.Add(rewardItem);
							}
						}
						else if (reward is BaseCreature)
						{
							var rewardCreature = (BaseCreature)reward;

							if (!OnBeforeGiveReward(s, rewardCreature))
							{
								rewardCreature.Delete();
								return;
							}

							OnGiveReward(s, rewardCreature);

							if (!rewardCreature.Deleted)
							{
								rewards.Add(rewardCreature);
							}
						}
						else if (reward is Mobile)
						{
							var rewardMobile = (Mobile)reward;

							if (!OnBeforeGiveReward(s, rewardMobile))
							{
								rewardMobile.Delete();
								return;
							}

							OnGiveReward(s, rewardMobile);

							if (!rewardMobile.Deleted)
							{
								rewards.Add(rewardMobile);
							}
						}
						else if (reward is XmlAttachment)
						{
							var a = (XmlAttachment)reward;

							if (!OnBeforeGiveReward(s, a))
							{
								a.Delete();
								return;
							}

							OnGiveReward(s, a);

							if (!a.Deleted)
							{
								attachments.Add(a);
							}
						}
					}
				});

			OnRewarded(s, rewards, attachments, true);
		}

		protected virtual bool OnBeforeGiveReward(ConquestState s, Item reward)
		{
            return Enabled && !Deleted && s != null && s.User != null && reward != null && !reward.Deleted;
		}

		protected virtual bool OnBeforeGiveReward(ConquestState s, Mobile reward)
		{
            return Enabled && !Deleted && s != null && s.User != null && reward != null && !reward.Deleted;
		}

		protected virtual bool OnBeforeGiveReward(ConquestState s, BaseCreature reward)
		{
            return Enabled && !Deleted && s != null && s.User != null && reward != null && !reward.Deleted;
		}

		protected virtual bool OnBeforeGiveReward(ConquestState s, XmlAttachment reward)
		{
            return Enabled && !Deleted && s != null && s.User != null && reward != null && !reward.Deleted;
		}

		protected virtual void OnGiveReward(ConquestState s, Item reward)
		{
            if (s == null || s.User == null || reward == null || reward.Deleted)
			{
				return;
			}

			if (reward is TitleScroll)
			{
				TitleScroll scroll = (TitleScroll)reward;

                scroll.BoundToMobile = s.User;

                s.User.Backpack.DropItem(scroll);

                if (!scroll.AutoConsume(s.User))
				{
                    s.User.BankBox.DropItem(scroll);
				}
			}
			else if (reward is HueScroll)
			{
				HueScroll scroll = (HueScroll)reward;

                scroll.BoundToMobile = s.User;

                s.User.Backpack.DropItem(scroll);

                if (!scroll.AutoConsume(s.User))
				{
                    s.User.BankBox.DropItem(scroll);
				}
			}
			else
			{
                s.User.BankBox.DropItem(reward);
			}
		}

		protected virtual void OnGiveReward(ConquestState s, Mobile reward)
		{
            if (s == null || s.User == null || reward == null || reward.Deleted)
			{
				return;
			}

            if (s.User.IsOnline())
			{
                reward.MoveToWorld(s.User.Location, s.User.Map);
			}
			else
			{
				reward.Delete();
			}
		}

		protected virtual void OnGivereward(ConquestState s, BaseCreature reward)
		{
            if (s == null || s.User == null || reward == null || reward.Deleted)
			{
				return;
			}

            reward.Control(s.User);

			AnimalTaming.ScaleSkills(reward, 0.90);

			if (reward.StatLossAfterTame)
			{
				AnimalTaming.ScaleStats(reward, 0.50);
			}

			if (reward.Stable())
			{
				return;
			}

            if (s.User.IsOnline())
			{
                reward.MoveToWorld(s.User.Location, s.User.Map);
			}
			else
			{
				reward.Delete();
			}
		}

		protected virtual void OnGiveReward(ConquestState s, XmlAttachment reward)
		{
            if (s == null || s.User == null || reward == null || reward.Deleted)
			{
				return;
			}

            if (!XmlAttach.AttachTo(s.User, reward))
			{
				reward.Delete();
			}
		}

		protected virtual void OnRewarded(
			ConquestState s, List<IEntity> rewards, List<IXmlAttachment> attachments, bool message)
		{
            if (s == null || s.User == null || !message)
			{
				return;
			}

			int count = rewards.Count + attachments.Count;

			if (count > 0)
			{
                s.User.SendMessage(
					85,
					"You received {0:#,0} reward{1} and {2:#,0} point{3} for your conquest!",
					rewards.Count,
					rewards.Count != 1 ? "s" : String.Empty,
					s.Conquest.Points,
					s.Conquest.Points != 1 ? "s" : String.Empty);
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			return obj is Conquest ? Equals((Conquest)obj) : base.Equals(obj);
		}

		public virtual bool Equals(Conquest other)
		{
			return !ReferenceEquals(other, null) && UID.Equals(other.UID);
		}

		public override sealed int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(4);

			UID.Serialize(writer);

			switch (version)
			{
				case 4:
					writer.Write(AccountWide);
					goto case 3;
				case 3:
					writer.Write(RewardTierComplete);
					goto case 2;
				case 2:
					writer.Write(ItemIDOffset);
					goto case 1;
				case 1:
					writer.Write(AccountBound);
					goto case 0;
				case 0:
					{
						writer.Write(Deleted);
						writer.Write(Enabled);

						writer.Write(TimeoutReset);

						writer.Write(Secret);
						writer.Write(Young);
						writer.Write(ScaleRewards);

						writer.Write(Points);

						writer.Write(Name);
						writer.Write(Desc);

						writer.Write(Category);

						writer.Write(ItemID);
						writer.Write(Hue);
						writer.WriteFlag(Color);

						writer.Write(TierMax);
						writer.Write(ProgressMax);
						writer.Write(ProgressFactor);

						writer.Write(SoundOnProgress);
						writer.Write(SoundOnComplete);
						writer.Write(SoundOnWorldFirst);

						writer.WriteBlockList(Rewards, t => writer.WriteType(t));
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			UID = new ConquestSerial(reader);

			switch (version)
			{
				case 4:
					AccountWide = reader.ReadBool();
					goto case 3;
				case 3:
					RewardTierComplete = reader.ReadBool();
					goto case 2;
				case 2:
					ItemIDOffset = reader.ReadPoint2D();
					goto case 1;
				case 1:
					AccountBound = reader.ReadBool();
					goto case 0;
				case 0:
					{
						Deleted = reader.ReadBool();
						_Enabled = reader.ReadBool();

						TimeoutReset = reader.ReadTimeSpan();

						Secret = reader.ReadBool();
						Young = reader.ReadBool();
						ScaleRewards = reader.ReadBool();

						Points = reader.ReadInt();

						Name = reader.ReadString();
						Desc = reader.ReadString();

						Category = reader.ReadString();

						ItemID = reader.ReadInt();
						Hue = reader.ReadInt();
						Color = reader.ReadFlag<KnownColor>();

						TierMax = reader.ReadInt();
						ProgressMax = reader.ReadInt();
						ProgressFactor = reader.ReadDouble();

						SoundOnProgress = reader.ReadInt();
						SoundOnComplete = reader.ReadInt();
						SoundOnWorldFirst = reader.ReadInt();

						Rewards = reader.ReadBlockList(reader.ReadType);
					}
					break;
			}
		}

		public static bool operator ==(Conquest left, Conquest right)
		{
			return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
		}

		public static bool operator !=(Conquest left, Conquest right)
		{
			return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : !left.Equals(right);
		}
	}
}