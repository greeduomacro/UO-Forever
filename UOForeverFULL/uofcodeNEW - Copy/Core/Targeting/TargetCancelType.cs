/***************************************************************************
 *                            TargetCancelType.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: TargetCancelType.cs 4 2006-06-15 04:28:39Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;

namespace Server.Targeting
{
	public enum TargetCancelType
	{
		Overridden,
		Cancelled,
		Disconnected,
		Timeout,
        /// <summary>
		/// Vita-Nex: Core Compatibility, equal to Overridden
		/// </summary>
		Overriden = Overridden,
		/// <summary>
		/// Vita-Nex: Core Compatibility, equal to Cancelled
		/// </summary>
		Canceled = Cancelled
	}
}