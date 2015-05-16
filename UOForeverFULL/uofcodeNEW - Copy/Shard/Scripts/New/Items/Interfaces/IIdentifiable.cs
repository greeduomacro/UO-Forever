using System;

namespace Server
{
	public interface IIdentifiable
	{
		bool Identified{ get; set; }

		void OnIdentified();
	}
}