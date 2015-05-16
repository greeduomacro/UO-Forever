#region References
using System;

using Server.Accounting;
using Server.Mobiles;
#endregion

namespace Server.Engines.Conquests
{
	public class AccountAgeConquest : Conquest
	{
		public override string DefCategory { get { return "Account"; } }

		public override bool DefAccountBound { get { return true; } }

		public virtual TimeSpan DefAccountAge { get { return TimeSpan.FromDays(365); } }

		[CommandProperty(Conquests.Access)]
		public TimeSpan AccountAge { get; set; }

		[CommandProperty(Conquests.Access)]
		public int AccountAgeDays
		{
			//
			get { return AccountAge.Days; }
			set { AccountAge = new TimeSpan(value, AccountAge.Hours, AccountAge.Minutes, AccountAge.Seconds, AccountAge.Milliseconds); }
		}

		public AccountAgeConquest()
		{ }

		public AccountAgeConquest(GenericReader reader)
			: base(reader)
		{ }

		public override void EnsureDefaults()
		{
			base.EnsureDefaults();

			AccountAge = DefAccountAge;
		}

		public override int GetProgress(ConquestState state, object args)
		{
			return GetProgress(state, args as Account);
		}

		protected virtual int GetProgress(ConquestState state, Account account)
		{
            if (state.User == null)
                return 0;

			if (account == null || DateTime.UtcNow - account.Created < AccountAge)
			{
				return 0;
			}
			
			return 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			int version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(AccountAge);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.GetVersion();

			switch (version)
			{
				case 0:
					AccountAge = reader.ReadTimeSpan();
					break;
			}
		}
	}
}