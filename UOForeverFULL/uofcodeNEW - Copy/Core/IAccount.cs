using System;

namespace Server.Accounting
{
	public interface IAccount : IComparable<IAccount>, IEquatable<IAccount>
	{
		[CommandProperty(AccessLevel.Administrator, true)]
		string Username { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		AccessLevel AccessLevel { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		int Length { get; }

		[CommandProperty(AccessLevel.Administrator)]
		int Limit { get; }

		[CommandProperty(AccessLevel.Administrator)]
		int Count { get; }

		Mobile this[int index] { get; set; }

		void Delete();
		void SetPassword( string password );
		bool CheckPassword( string password );

        Mobile GetPseudoSeerLastCharacter();
        void SetPseudoSeerLastCharacter(Mobile m);
	}
}